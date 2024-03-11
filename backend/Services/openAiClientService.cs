using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using backend;
using OpenAI.Chat;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

public class OpenAIClient
{
    private readonly HttpClient _httpClient;

    public OpenAIClient()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://api.openai.com/")
        };
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "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");
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

        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        var response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            return new OkObjectResult(result);
        }
        else
        {
            throw new Exception($"Error calling OpenAI API: {response.StatusCode}");
        }
    }
}