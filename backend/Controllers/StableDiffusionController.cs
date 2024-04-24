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
        // Console.WriteLine($"Processing Request: " +
        //     $"prompt text: {imageRequest.ImagePromptText}, " +
        //     $"model: {imageRequest.Model}, " +
        //     $"size: {imageRequest.Size}, " +
        //     $"natural style: {imageRequest.Style}, " +
        //     $"hd: {imageRequest.Hd}");

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