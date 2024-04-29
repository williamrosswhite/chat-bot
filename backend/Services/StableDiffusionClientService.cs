using System.Text;
using backend;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using backend.Models;
using Polly.Fallback;
using Azure;

public class StableDiffusionClientService
{
    private readonly IHttpClientFactory _clientFactory;
    private readonly ChatbotDBContext _dBcontext;
    private readonly ImageService _imageService;
    private readonly ILogger<StableDiffusionClientService> _logger;
    private readonly string stableDiffusionKey;

    public StableDiffusionClientService(IHttpClientFactory clientFactory, ChatbotDBContext dBcontext,  ImageService imageService, ILogger<StableDiffusionClientService> logger)
    {
        _clientFactory = clientFactory;
        _dBcontext = dBcontext;
        _imageService = imageService;
        _logger = logger;

        var localStableDiffusionKey = Environment.GetEnvironmentVariable("STABLEDIFFUSION_API_KEY");

        if (string.IsNullOrEmpty(localStableDiffusionKey))
        {
            _logger.LogError("STABLEDIFFUSION_API_KEY environment variable is not set");
            throw new InvalidOperationException("STABLEDIFFUSION_API_KEY environment variable is not set");
        }

        stableDiffusionKey = localStableDiffusionKey;     
    }

    public async Task<IActionResult> ProcessImagePrompt(ImageRequest imageRequest)
    {
        var client = _clientFactory.CreateClient("StableDiffusion");

        if (imageRequest == null) {
            _logger.LogError("Image request cannot be null.");
            throw new ArgumentNullException(nameof(imageRequest), "Image request cannot be null.");
        }

        string url = "api/v3/text2img";
        string[] dimensions = imageRequest.Size.Split('x');
        DateTime timeStamp = DateTime.Now;

        var data = new
        {
            key = stableDiffusionKey,
            prompt = imageRequest.ImagePromptText,
            width = (dimensions != null && dimensions.Length > 0) ? dimensions[0] : "",
            height = (dimensions != null && dimensions.Length > 1) ? dimensions[1] : "",
            samples = imageRequest.Samples,
            enhance_style = imageRequest.Hd == true ? "yes" : "no",
            num_inference_steps = 51,
            self_attention = "yes",
            guidance_scale = imageRequest.GuidanceScale.ToString(),
            seed = imageRequest.Seed
        };

        _logger.LogInformation("data: {data}", data);

        // Encode information to send to API
        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        HttpResponseMessage response;

        try {
            // Attempt to send request to Stable Diffusion API
            response = await client.PostAsync(url, content);

        } catch (HttpRequestException e) {
            _logger.LogError($"Error sending request to Stable Diffusion API: {e.Message}");
            throw new InvalidOperationException("STABLEDIFFUSION_API_KEY environment variable is not set");
        }

        if (response == null || !response.IsSuccessStatusCode)
        {
            _logger.LogError($"Error calling Stable Diffusion API: {response?.StatusCode}");
            return new BadRequestObjectResult(response == null ? "Response is null." : $"Response status code: {response.StatusCode}");
        }

        string result;

        try {
            // Attempt to read response content
            result = await response.Content.ReadAsStringAsync();

        } catch (Exception e) {
            _logger.LogError($"Error reading response content: {e.Message}");
            return new BadRequestObjectResult($"Error reading response content: {e.Message}");
        }

        if (string.IsNullOrEmpty(result)) {
            _logger.LogError("Result of reading response content is null.");
            return new BadRequestObjectResult("Result of reading response content is null.");
        }

        dynamic jsonResponse;

        try {
            // Attempt to deserialize response content
            jsonResponse = JsonConvert.DeserializeObject(result);

        } catch (JsonException e) {
            _logger.LogError($"Error deserializing response content: {e.Message}");
            return new BadRequestObjectResult($"Error deserializing response content: {e.Message}");
        }

        if (jsonResponse == null || jsonResponse.output == null) {
            _logger.LogError("Deserializing result failed. jsonResponse or jsonResponse.output is null.");
            return new BadRequestObjectResult("Deserializing result failed. jsonResponse or jsonResponse.output is null.");
        }

        List<Task<string>> uploadTasks = new List<Task<string>>();
        List<ImageReturn> returnImages = new List<ImageReturn>();

        foreach (string imageUrl in jsonResponse.output)
        {
            var image = new Image { 
                UserId = 1, 
                ImagePromptText = imageRequest.ImagePromptText, 
                Model = imageRequest.Model, 
                Size = imageRequest.Size, 
                Style = null,
                Hd = imageRequest.Hd,
                InferenceDenoisingSteps = imageRequest.InferenceDenoisingSteps,
                GuidanceScale = imageRequest.GuidanceScale,
                TimeStamp = timeStamp,
                Seed = jsonResponse.meta?.seed ?? "Seed undefined"
            };

            try{
                // Add attempt to upload image to blob storage to uploadTasks
                uploadTasks.Add(_imageService.UploadBlobImage(image, imageUrl, timeStamp));

                // Create ImageReturn object and add to returnImages
                ImageReturn returnImage = _imageService.CreateImageReturn(image, imageUrl);

                // Add seed to returnImage
                returnImage.Seed = jsonResponse.meta?.seed ?? default(long);

                // Add returnImage to returnImages
                returnImages.Add(returnImage);

            }
            catch (RequestFailedException e) {
                _logger.LogError($"Error uploading image to blob storage: {e.Message}");

                // Still add image to database with BlobName as exception message if blob upload fails
                Image newImage = (Image)image.Clone();
                newImage.BlobName = $"Error: {e.Message}. Original URL: {imageUrl}";
                _dBcontext.Images.Add(newImage);
                await _dBcontext.SaveChangesAsync();
            }
        }

        try {
            // Wait for all uploadTasks to complete
            string[] blobNames = await Task.WhenAll(uploadTasks);

            // Add images to database and save changes
            foreach (var blobName in blobNames)
            {
                var image = new Image { 
                    UserId = 1, 
                    ImagePromptText = imageRequest.ImagePromptText, 
                    Model = imageRequest.Model, 
                    Size = imageRequest.Size, 
                    Style = null,
                    Hd = imageRequest.Hd,
                    InferenceDenoisingSteps = imageRequest.InferenceDenoisingSteps,
                    GuidanceScale = imageRequest.GuidanceScale,
                    TimeStamp = timeStamp,
                    Seed = jsonResponse.meta?.seed ?? "Seed undefined"
                };

                image.BlobName = blobName;
                _dBcontext.Images.Add(image);
            }
            
            await _dBcontext.SaveChangesAsync();
            _logger.LogInformation("Images were successfully stored in the database.");

        } catch (Exception e) {
            _logger.LogError($"Error saving images to database: {e.Message}");
            return new BadRequestObjectResult($"Error saving images to database: {e.Message}");
        }

        return new OkObjectResult(returnImages);
    }
}