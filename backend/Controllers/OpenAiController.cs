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
            var response = await new OpenAIClient { }.ProcessChatPrompt(chatRequest);
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
        Console.WriteLine("imageRequest: " + imageRequest);
        Console.WriteLine("imagePromptText received: " + imageRequest.imagePromptText);

        if(imageRequest != null && imageRequest.imagePromptText != null) {
            // TODO: replace this with a proper class instance of OpenAIClient
            var openAIClient = new OpenAIClient();
            var result = await openAIClient.ProcessImagePrompt(imageRequest);

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