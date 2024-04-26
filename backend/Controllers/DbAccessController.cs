using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class DbAccessController : ControllerBase
    {
        private readonly ImageService _imageService;

        public DbAccessController(ImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpGet("images")]
        public async Task<IActionResult> GetImages()
        {
            Console.WriteLine("Getting images...");
            var imageUrls = await _imageService.GetImageUrlsAsync();

            Response.Headers.Add("Cache-Control", "no-store");
            return Ok(imageUrls);
        }

        // [HttpGet("decodeAndStore")]
        // public async Task<IActionResult> DecodeAndStore()
        // {
        //     Console.WriteLine("Getting images...");
        //     await _imageService.DecodeAndStoreImages();

        //     return Ok();
        // }
    }
}