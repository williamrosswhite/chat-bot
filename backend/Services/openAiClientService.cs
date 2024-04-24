using System.Text;
using backend;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using backend.Models;

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

        var data = new Dictionary<string, object>
        {
            { "model", imageRequest.Model },
            { "prompt", imageRequest.ImagePromptText },
            { "size", imageRequest.Size },
            { "response_format", "b64_json" }
        };

        if(imageRequest.Hd == true) {
            data["quality"] = "hd";
        }

        if(imageRequest.Style == true) {
            data["style"] = "natural";
        }

        var image = new Image { 
            UserId = 1, 
            ImagePromptText = imageRequest.ImagePromptText, 
            Model = imageRequest.Model, 
            Size = imageRequest.Size, 
            Style = imageRequest.Style.GetValueOrDefault(), 
            Hd = imageRequest.Hd.GetValueOrDefault(), 
            TimeStamp = DateTime.Now 
        };

        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            
            var resultObject = JsonConvert.DeserializeObject<dynamic>(result);

            #nullable disable
            var base64String = resultObject["data"]?[0]?["b64_json"]?.ToString();
            #nullable enable

            if (base64String == null)
            {
                throw new InvalidOperationException("Could not find 'b64_json' in the response.");
            }

            var blobName = await _imageService.UploadBlobImageFromOpenAi(base64String, image);

            image.BlobName = blobName;
            _context.Images.Add(image);
            _context.SaveChanges();

            return new OkObjectResult(result);
        }
        else
        {
            image.BlobName = "BlobFailure";
            _context.Images.Add(image);
            _context.SaveChanges();

            throw new Exception($"Error calling OpenAI API: {response.StatusCode}");
        }
    }
}