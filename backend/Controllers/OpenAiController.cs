using System;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using backend.Models;
using backend.Migrations;
using backend.Controllers;

namespace backend.Controllers;

[Route("api")]
[ApiController]
public class YourController : ControllerBase
{
    private readonly ChatbotDBContext _context;
    private readonly OpenAIClient _openAIClient;

    public YourController(ChatbotDBContext context, OpenAIClient openAIClient)
    {
        _context = context;
        _openAIClient = openAIClient;
    }
    
    public class MyModel
    {
        public string? MyVariable { get; set; }
    }

    [HttpPost("ChatRequest")]
    public async Task<IActionResult> PostAsync([FromBody] ChatRequest chatRequest)
    {
        Console.WriteLine("chatRequest: " + chatRequest);

        if (chatRequest != null && chatRequest.messages != null && chatRequest.messages.Length >= 0)
        {
            foreach (ChatMessage msg in chatRequest.messages)
            {
                Console.WriteLine("role: " + msg.role);
                Console.WriteLine("content: " + msg.content);
            }
        }
        else
        {
            Console.WriteLine("chatRequest is null or messages is null or messages length is less than 0");
        }

        if (chatRequest != null) // Add null check for chatRequest
        {
            // TODO: replace this with a proper class instance of OpenAIClient
            var response = await _openAIClient.ProcessChatPrompt(chatRequest);
            return Ok(response);
        }
        else
        {
            // Handle the case when chatRequest is null
            return BadRequest("chatRequest is null");
        }
    }

    [HttpPost("ImageRequest")]
    public async Task<IActionResult> ImageRequest([FromBody] ImageRequest imageRequest)
    {
        Console.WriteLine($"Processing Request: " +
            $"prompt text: {imageRequest.ImagePromptText}, " +
            $"model: {imageRequest.Model}, " +
            $"size: {imageRequest.Size}, " +
            $"natural style: {imageRequest.Style}, " +
            $"hd: {imageRequest.Hd}");

        if(imageRequest != null && imageRequest.ImagePromptText != null) {

            var result = await _openAIClient.ProcessImagePrompt(imageRequest);

            if (result != null)
            {
                return result;
            }
            else
            {
                // Handle the case when imageRequest is null
                return BadRequest("imageData is null");
            }
        } else {
            return BadRequest("imagePromptText is null");

        }
    }
}