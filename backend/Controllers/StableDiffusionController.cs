using System;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using backend.Models;
using backend.Migrations;
using backend.Controllers;

namespace backend.Controllers
{
    [Route("stablediffusion")]
    [ApiController]
    public class StableDiffusionController : ControllerBase
    {
        private readonly StableDiffusionClientService _stableDiffusionClientService;
        private readonly ILogger<StableDiffusionController> _logger;

        public StableDiffusionController(StableDiffusionClientService stableDiffusionClientService, ILogger<StableDiffusionController> logger)
        {
            _stableDiffusionClientService = stableDiffusionClientService ?? throw new ArgumentNullException(nameof(stableDiffusionClientService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpPost("ImageRequest")]
        public async Task<IActionResult> ImageRequest([FromBody] ImageRequest imageRequest)
        {
            _logger.LogInformation("Processing StableDiffusion Image Request: " +
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
                    var result = await _stableDiffusionClientService.ProcessImagePrompt(imageRequest);

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