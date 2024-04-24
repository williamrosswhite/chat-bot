
namespace backend.Models
{
    public class Image
    {
        public int Id { get; set; }

        public string? ImagePromptText { get; set; }

        public string? Model { get; set; }

        public string? Size { get; set; }

        public bool Style { get; set; }

        public bool Hd { get; set; }  

        public string? BlobName { get; set; }

        public DateTime TimeStamp { get; set; }

        // Foreign key for User
        public int UserId { get; set; }

        // Navigation property for User
        public User? User { get; set; }
    }
}