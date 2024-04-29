using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class DbAccessController : ControllerBase
    {
        private readonly ImageService _imageService;
        private readonly ILogger<DbAccessController> _logger;

        public DbAccessController(ImageService imageService, ILogger<DbAccessController> logger)
        {
            _imageService = imageService ?? throw new ArgumentNullException(nameof(imageService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        [HttpGet("images")]
        public async Task<IActionResult> GetImages([FromQuery] int limit = 25, int offset = 0)
        {
            try
            {
                _logger.LogInformation($"Retrieving next {limit} most recent images...");
                var imageUrls = await _imageService.GetImageUrlsAsync(limit, offset);

                Response.Headers.Add("Cache-Control", "no-store");
                return Ok(imageUrls);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error getting images");
                return StatusCode(500, "Internal server error");
            }
        }
    }
}