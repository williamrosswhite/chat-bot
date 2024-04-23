using System.Text;
using backend;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using backend.Models;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

public class OpenAIClient
{
    private readonly ChatbotDBContext _context;
    private readonly HttpClient _httpClient;
    private readonly BlobServiceClient _blobServiceClient;

    public OpenAIClient(ChatbotDBContext context, BlobServiceClient blobServiceClient)
    {
        _context = context;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.openai.com/")
        };
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "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");
        _blobServiceClient = blobServiceClient;

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

            var imageUrl = await UploadBlobImage(base64String);

            image.ImageUrl = imageUrl;
            _context.Images.Add(image);
            _context.SaveChanges();

            
            return new OkObjectResult(result);
        }
        else
        {
            image.ImageUrl = "failed query";
            _context.Images.Add(image);
            _context.SaveChanges();

            throw new Exception($"Error calling OpenAI API: {response.StatusCode}");
        }
    }

    private async Task<string> UploadBlobImage(string base64String) 
    {
        // Convert base64 string to byte array
        byte[] imageBytes = Convert.FromBase64String(base64String);

        // Create the container and return a container client object
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("generated-images");

        // Create a local file in the ./data/ directory for uploading and downloading
        string localPath = "./data/";
        string fileName = Guid.NewGuid().ToString(); // Generate a new name for the image
        string localFilePath = Path.Combine(localPath, fileName);

        /*      For when i have a user
        // Create a local file in the ./data/ directory for uploading and downloading
        string localPath = "./data/";
        string userId = "someUserId"; // Replace this with the actual user ID
        string fileName = $"users/{userId}/{Guid.NewGuid()}"; // Generate a new name for the image
        string localFilePath = Path.Combine(localPath, fileName);
        */

        // Write the blob to a file
        await File.WriteAllBytesAsync(localFilePath, imageBytes);

        // Get a reference to a blob
        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        // Open the file and upload its data
        using FileStream uploadFileStream = File.OpenRead(localFilePath);
        Dictionary<string, string> metadata = new Dictionary<string, string>
        {
            { "UploadDate", DateTime.UtcNow.ToString() },
            { "UserId", "userId" } // Replace this with the actual user ID
        };
        BlobUploadOptions uploadOptions = new BlobUploadOptions 
        { 
            Metadata = metadata
        };
        await blobClient.UploadAsync(uploadFileStream, uploadOptions);
        uploadFileStream.Close();

        // Delete the local file
        File.Delete(localFilePath);

        // Return the URL of the uploaded blob
        return blobClient.Uri.AbsoluteUri;
    }
}