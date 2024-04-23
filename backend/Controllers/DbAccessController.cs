using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers
{
    [Route("api")]
    [ApiController]
    public class DbAccessController : ControllerBase
    {
        private readonly ChatbotDBContext _context;

        public DbAccessController(ChatbotDBContext context)
        {
            _context = context;
        }

        // Existing code...

        [HttpGet("images")]
        public async Task<IActionResult> GetImages()
        {
            Console.WriteLine("Getting images...");
            var images = await _context.Images.ToListAsync(); // replace 'Images' with the actual name of your DbSet

            return Ok(images);
        }
    }
}