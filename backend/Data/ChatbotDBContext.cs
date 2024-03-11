using Microsoft.EntityFrameworkCore;

namespace backend
{
    public class ChatbotDBContext : DbContext
    {
        public ChatbotDBContext(DbContextOptions<ChatbotDBContext> options)
            : base(options)
        {
        }

        public DbSet<Image> Images { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<User> Users { get; set; }

    }
}