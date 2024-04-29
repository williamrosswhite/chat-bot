using System.Text;
using backend;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using backend.Models;
using Newtonsoft.Json.Linq;
using Azure;
using System.Net.Http;

public class OpenAIClient
{
    private readonly HttpClient _client;
    private readonly ILogger<OpenAIClient> _logger;
    private readonly ChatbotDBContext _context;
    private readonly ImageService _imageService;

    public OpenAIClient(IHttpClientFactory clientFactory, ChatbotDBContext context, ImageService imageService, ILogger<OpenAIClient> logger)
    {
        _client = clientFactory.CreateClient("OpenAI");
        _logger = logger;
        _context = context;
        _imageService = imageService;
    }

    public async Task<string> ProcessChatPrompt(backend.ChatRequest chatRequest)
    {
        try
        {
            var payload = new {
                model = "gpt-3.5-turbo",
                chatRequest.Messages
            };
            
            var content = CreateJsonContent(payload);

            _logger.LogInformation($"Sending chat prompt to OpenAI API: {content}");

            var responseString = await SendRequest("v1/chat/completions", content);
            
            _logger.LogInformation($"Received response from OpenAI API: {responseString}");

            return responseString;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error processing chat prompt in {nameof(ProcessChatPrompt)}");
            throw;
        }
    }

    public async Task<IActionResult> ProcessImagePrompt(ImageRequest imageRequest)
    {
        if (imageRequest == null)
        {
            throw new ArgumentNullException(nameof(imageRequest), "Image request cannot be null.");
        }

        try
        {
            var url = "v1/images/generations";

            if (imageRequest.Model == null || imageRequest.ImagePromptText == null || imageRequest.Size == null)
            {
                throw new InvalidOperationException("One or more required properties are null.");
            }

            var timeStamp = DateTime.Now;

            var requestData = new Dictionary<string, object>
            {
                { "model", imageRequest.Model },
                { "prompt", imageRequest.ImagePromptText },
                { "size", imageRequest.Size },
                { "response_format", "url" },
            };

            if (imageRequest.Samples != null)
            {
                requestData["n"] = imageRequest.Samples;
            }

            if (imageRequest.Hd == true)
            {
                requestData["quality"] = "hd";
            }

            if(imageRequest.Style == true) {
                requestData["style"] = "natural";
            }

            var content = CreateJsonContent(requestData);

            var responseString = await SendRequest(url, content);

            var resultObject = JObject.Parse(responseString);

            if (resultObject["data"] is JArray dataArray)
            {
                List<Task<string>> uploadTasks = new List<Task<string>>();
                var image = CreateImage(imageRequest, timeStamp);

                List<ImageReturn> returnImages = new List<ImageReturn>();

                foreach (var item in dataArray)
                {
                    var imageUrl = item["url"]?.ToString();

                    if (imageUrl == null)
                    {
                        throw new InvalidOperationException("Could not find 'url' in the response.");
                    }

                    try {

                        uploadTasks.Add(_imageService.UploadBlobImage(image, imageUrl, timeStamp));

                        ImageReturn returnImage = _imageService.CreateImageReturn(image, imageUrl);
                        returnImages.Add(returnImage);
                    }
                    catch (RequestFailedException e) {
                        HandleUploadFailure(e, image);
                    }
                }

                var blobNames = await Task.WhenAll(uploadTasks);

                foreach (var blobName in blobNames)
                {
                    var newImage = CreateImage(imageRequest, timeStamp);
                    newImage.BlobName = blobName;
                    _context.Images.Add(newImage);
                }

                await _context.SaveChangesAsync();
                _logger.LogInformation("Images were successfully stored in the database.");

                return new OkObjectResult(returnImages);
            }
            else
            {
                _logger.LogError($"Invalid response from OpenAI API: {responseString}");
                return new BadRequestObjectResult("Invalid response from the server.");
            }
        }
        catch(Exception e) {
            _logger.LogError(e, $"Error processing image prompt in {nameof(ProcessImagePrompt)}");
            throw;
        }
    }

    private StringContent CreateJsonContent(object payload)
    {
        return new StringContent(
            System.Text.Json.JsonSerializer.Serialize(payload), 
            Encoding.UTF8, 
            "application/json"
        );
    }

    private async Task<string> SendRequest(string url, StringContent content)
    {
        var response = await _client.PostAsync(url, content);
        return await response.Content.ReadAsStringAsync();
    }

    private Image CreateImage(ImageRequest imageRequest, DateTime timeStamp)
    {
        return new Image
        {
            UserId = 1,
            ImagePromptText = imageRequest?.ImagePromptText,
            Model = imageRequest?.Model ?? string.Empty,
            Size = imageRequest?.Size ?? string.Empty,
            Style = imageRequest?.Style,
            Hd = imageRequest?.Hd ?? false,
            TimeStamp = timeStamp
        };
    }

    private void HandleUploadFailure(RequestFailedException e, Image image)
    {
        Image newImage = (Image)image.Clone();
        newImage.BlobName = e.ToString();
        _context.Images.Add(newImage);
        _context.SaveChangesAsync();
        _logger.LogError(e, "Error uploading image to blob storage");
    }
}