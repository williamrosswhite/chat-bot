using System;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using backend.Models;
using backend.Migrations;
using backend.Controllers;

namespace backend.Controllers
{
    [Route("openapi")]
    [ApiController]
    public class OpenAiController : ControllerBase
    {
        private readonly OpenAIClient _openAIClient;
        private readonly ILogger<OpenAiController> _logger;

        public OpenAiController(OpenAIClient openAIClient, ILogger<OpenAiController> logger)
        {
            _openAIClient = openAIClient ?? throw new ArgumentNullException(nameof(openAIClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("ChatRequest")]
        public async Task<IActionResult> PostAsync([FromBody] ChatRequest chatRequest)
        {
            _logger.LogInformation("Processing OpenAi Chat Request:");

            if (chatRequest?.messages != null)
            {
                for (int i = 0; i < chatRequest.messages.Length; i++)
                {
                    _logger.LogInformation("Processing message {MessageNumber} in chatRequest: role: {role}, content: {content}", 
                        i + 1, 
                        chatRequest.messages[i].role, 
                        chatRequest.messages[i].content);
                }
            }
            else
            {
                _logger.LogInformation("chatRequest is null or messages is null");
            }

            if (chatRequest != null)
            {
                try
                {
                    var response = await _openAIClient.ProcessChatPrompt(chatRequest);
                    return Ok(response);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing the chat prompt");
                    return StatusCode(500, "An error occurred while processing your request");
                }
            }
            else
            {
                return BadRequest("chatRequest is null");
            }
        }

        [HttpPost("ImageRequest")]
        public async Task<IActionResult> ImageRequest([FromBody] ImageRequest imageRequest)
        {
            _logger.LogInformation("Processing OpenAi Image Request: " +
                "ImagePromptText: {ImagePromptText}, " +
                "Model: {Model}, " +
                "Size: {Size}, " +
                "Style: {Style}, " +
                "Hd: {Hd}, " +
                "GuidanceScale: {GuidanceScale}, " +
                "InferenceDenoisingSteps: {InferenceDenoisingSteps}, " +
                "Seed: {Seed}, " +
                "Samples: {Samples}",
                imageRequest?.ImagePromptText,
                imageRequest?.Model,
                imageRequest?.Size,
                imageRequest?.Style,
                imageRequest?.Hd,
                imageRequest?.GuidanceScale,
                imageRequest?.InferenceDenoisingSteps,
                imageRequest?.Seed,
                imageRequest?.Samples);

            if(imageRequest != null && imageRequest.ImagePromptText != null) 
            {
                try
                {
                    var result = await _openAIClient.ProcessImagePrompt(imageRequest);

                    if (result != null)
                    {
                        return result;
                    }
                    else
                    {
                        return BadRequest("imageData is null");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while processing the image prompt");
                    return StatusCode(500, "An error occurred while processing your request");
                }
            } 
            else 
            {
                return BadRequest("imagePromptText is null");
            }
        }
    }
}