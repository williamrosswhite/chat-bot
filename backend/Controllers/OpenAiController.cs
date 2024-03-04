using System;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;

namespace backend.Controllers;

[Route("api")]
[ApiController]
public class YourController : ControllerBase
{
    public class MyModel
    {
        public string? MyVariable { get; set; }
    }

    [HttpPost("OpenAi")]
    public async Task<IActionResult> PostAsync([FromBody] ChatRequest chatRequest)
    {
        Console.WriteLine("chatRequest: " + chatRequest);
        if (chatRequest != null && chatRequest.messages != null && chatRequest.messages.Length >= 0)
        {
            foreach (Message message in chatRequest.messages)
            {
                Console.WriteLine("role: " + message.role);
                Console.WriteLine("content: " + message.content);
            }
        }
        else
        {
            Console.WriteLine("chatRequest is null or messages is null or messages length is less than 0");
        }

        if (chatRequest != null) // Add null check for chatRequest
        {
            var response = await new OpenAIClient { }.ProcessPrompt(chatRequest);
            return Ok(response);
        }
        else
        {
            // Handle the case when chatRequest is null
            return BadRequest("chatRequest is null");
        }
    }
}