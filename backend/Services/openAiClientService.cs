using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using backend;

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

    public async Task<string> ProcessPrompt(ChatRequest chatRequest)
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
}