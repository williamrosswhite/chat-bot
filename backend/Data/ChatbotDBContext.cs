using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend
{
    public class ChatbotDBContext : DbContext
    {
        public ChatbotDBContext(DbContextOptions<ChatbotDBContext> options)
            : base(options)
        {
        }

        #nullable disable
        
        public DbSet<Image> Images { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<User> Users { get; set; }
        
        #nullable enable
    }
}