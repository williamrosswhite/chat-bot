using System;
using Microsoft.AspNetCore.Mvc;
using OpenAI.Chat;
using backend.Models;
using backend.Migrations;
using backend.Controllers;

namespace backend.Controllers;

[Route("stablediffusion")]
[ApiController]
public class StableDiffusionController : ControllerBase
{
    private readonly StableDiffusionClient _stableDiffusionClient;

    public StableDiffusionController(StableDiffusionClient stableDiffusionClient)
    {
        _stableDiffusionClient = stableDiffusionClient;
    }

    [HttpPost("ImageRequest")]
    public async Task<IActionResult> ImageRequest([FromBody] ImageRequest imageRequest)
    {
        Console.WriteLine("attempting stable diffusion request");
        Console.WriteLine($"Processing Request: " +
            $"\nprompt text: {imageRequest.ImagePromptText}, " +
            $"\nmodel: {imageRequest.Model}, " +
            $"\nsize: {imageRequest.Size}, " +
            $"\nnatural style: {imageRequest.Style}, " +
            $"\nhd: {imageRequest.Hd}" +
            $"\nguidance scale: {imageRequest.GuidanceScale}" +
            $"\nsamples: {imageRequest.Samples}" +
            $"\nseed: {imageRequest.Seed}" +
            $"\ninference denoising steps: {imageRequest.InferenceDenoisingSteps}"
        );

        if(imageRequest != null && imageRequest.ImagePromptText != null) {

            var result = await _stableDiffusionClient.ProcessImagePrompt(imageRequest);

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