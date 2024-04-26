using System.Text;
using backend;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using backend.Models;
using Newtonsoft.Json.Linq;
using System.Linq.Expressions;
using Azure;

public class OpenAIClient
{
    private readonly ChatbotDBContext _context;
    private readonly HttpClient _httpClient;
    private readonly ImageService _imageService;

    public OpenAIClient(ChatbotDBContext context, ImageService imageService)
    {
        _context = context;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.openai.com/")
        };
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", Environment.GetEnvironmentVariable("OPENAI_API_KEY"));
        _imageService = imageService;
    }

    public async Task<string> ProcessChatPrompt(backend.ChatRequest chatRequest)
    {
        var payload = new {
            model = "gpt-3.5-turbo",
            chatRequest.messages
        };
        
        var content = new StringContent(
            System.Text.Json.JsonSerializer.Serialize(payload), 
            Encoding.UTF8, 
            "application/json"
        );

        Console.WriteLine("content: " + content);
        
        var response = await _httpClient.PostAsync("v1/chat/completions", content);
        
        var responseString = await response.Content.ReadAsStringAsync();

        return responseString;
    }

    public async Task<IActionResult> ProcessImagePrompt(ImageRequest imageRequest)
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

        var content = new StringContent(JsonConvert.SerializeObject(requestData), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);

        var responseString = await response.Content.ReadAsStringAsync();

        var resultObject = JObject.Parse(responseString);

        if (resultObject["data"] is JArray dataArray)
        {
            List<Task<string>> uploadTasks = new List<Task<string>>();

            var image = new Image
            {
                UserId = 1,
                ImagePromptText = imageRequest?.ImagePromptText,
                Model = imageRequest?.Model ?? string.Empty,
                Size = imageRequest?.Size ?? string.Empty,
                Style = imageRequest?.Style,
                Hd = imageRequest?.Hd ?? false,
                TimeStamp = timeStamp
            };

            try {

                foreach (var item in dataArray)
                {
                    var imageUrl = item["url"]?.ToString();

                    if (imageUrl == null)
                    {
                        throw new InvalidOperationException("Could not find 'url' in the response.");
                    }

                    try {
                        if (imageRequest != null) {
                            // Start uploading the image and add the task to the list
                            uploadTasks.Add(_imageService.UploadBlobImage(imageUrl, timeStamp));
                        }
                        else {
                            throw new ArgumentNullException(nameof(imageRequest), "Image request cannot be null.");
                        }
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
                var blobNames = await Task.WhenAll(uploadTasks);

                // Save each image to the database
                foreach (var blobName in blobNames)
                {
                    Image newImage = (Image)image.Clone();
                    newImage.BlobName = blobName;
                    _context.Images.Add(newImage);
                }

                _context.SaveChanges();

                return new OkObjectResult(responseString);
            }
            catch(Exception e) {
                throw new Exception($"Error calling OpenAI API: {response.StatusCode}, Exception: {e.Message}");
            }
        }
        else
        {
            // Return an error response when resultObject["data"] is not a JArray
            return new BadRequestObjectResult("Invalid response from the server.");
        }
    }
}
