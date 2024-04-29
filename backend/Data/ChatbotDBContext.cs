using Microsoft.EntityFrameworkCore;
using backend.Models;

namespace backend
{
    /// <summary>
    /// Represents the database context for the chatbot.
    /// </summary>
    public class ChatbotDBContext : DbContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChatbotDBContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by a <see cref="DbContext"/>.</param>
        public ChatbotDBContext(DbContextOptions<ChatbotDBContext> options)
            : base(options)
        {
        }

        /// <summary>
        /// Gets the images in the database.
        /// </summary>
        public DbSet<Image> Images { get; private set; } = null!;

        /// <summary>
        /// Gets the messages in the database.
        /// </summary>
        public DbSet<Message> Messages { get; private set; } = null!;

        /// <summary>
        /// Gets the users in the database.
        /// </summary>
        public DbSet<User> Users { get; private set; } = null!;
    }
}