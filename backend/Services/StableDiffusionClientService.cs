using System.Text;
using backend;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using backend.Models;
using Polly.Fallback;
using Azure;

public class StableDiffusionClient
{
    private readonly ChatbotDBContext _context;
    private readonly HttpClient _httpClient;
    private readonly ImageService _imageService;
    private readonly string stableDiffusionKey;

    public StableDiffusionClient(ChatbotDBContext context, ImageService imageService)
    {
        _context = context;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://stablediffusionapi.com/")
        };
        
        stableDiffusionKey = Environment.GetEnvironmentVariable("STABLEDIFFUSION_API_KEY");

        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", stableDiffusionKey);
        _imageService = imageService;
    }

    public async Task<IActionResult> ProcessImagePrompt(ImageRequest imageRequest)
    {
        if(imageRequest == null) {
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

        Console.WriteLine("data: " + data);

        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);

        if (response != null && response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            Console.WriteLine("Response content read successfully.");

            if(result == null) {
                throw new ArgumentNullException("Result of reading response content is null.");
            }
            
            dynamic jsonResponse = JsonConvert.DeserializeObject(result);
            Console.WriteLine($"Json Resonse Content: {jsonResponse}");

            if(jsonResponse == null || jsonResponse.output == null) {
                throw new ArgumentNullException("Deserializing result failed.  jsonResponse or jsonResponse.output is null.");
            }

            var wubbs =  jsonResponse.meta;
            Console.WriteLine(wubbs);

            var dubbs =  wubbs.seed;
            Console.WriteLine(dubbs);

            List<Task<string>> uploadTasks = new List<Task<string>>();

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
                Seed = jsonResponse.meta?.seed ?? null
            };

            try {
                foreach (string imageUrl in jsonResponse?.output)
                {
                    try{
                        uploadTasks.Add(_imageService.UploadBlobImageFromStableDiffusion(imageUrl, timeStamp));
                    }
                    catch (RequestFailedException e) {
                        // If the upload fails, save the image to the database with error as BlobName
                        Image newImage = (Image)image.Clone();
                        newImage.BlobName = e.ToString();
                        _context.Images.Add(newImage);
                        _context.SaveChanges();
                    }
                }

                // Wait for all uploads to complete
                string[] blobNames = await Task.WhenAll(uploadTasks);

                // Save each image to the database
                foreach (var blobName in blobNames)
                {
                    Image newImage = (Image)image.Clone();
                    newImage.BlobName = blobName;
                    _context.Images.Add(newImage);
                }

                _context.SaveChanges();

                // Create a new object that includes both jsonResponse.output and jsonResponse.meta
                var returnObject = new
                {
                    output = jsonResponse.output,
                    seed = jsonResponse.meta.seed
                };

                // Convert the object to a JSON string
                string returnJson = JsonConvert.SerializeObject(returnObject);

                // Return the JSON string
                return new OkObjectResult(returnJson);
            }
            catch(Exception e) {
                throw new Exception($"Error calling Stable Diffusion API: {response.StatusCode}, Exception: {e.Message}");
            }
        }
        else
        {
            // Return an error response is null or not successful
            return new BadRequestObjectResult(response == null || !response.IsSuccessStatusCode ? "Response is null or not successful." : response.StatusCode.ToString());
        }
    }
}