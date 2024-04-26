
namespace backend.Models
{
    using System;

    public class Image: ICloneable
    {
        public int Id { get; set; }

        public string? ImagePromptText { get; set; }

        public string Model { get; set; }

        public string Size { get; set; }

        public bool? Style { get; set; }

        public bool Hd { get; set; }

        public int? GuidanceScale { get; set; }

        public int? InferenceDenoisingSteps { get; set; }

        public long? Seed { get; set; }

        public int? Samples { get; set; }

        public string BlobName { get; set; }

        public DateTime TimeStamp { get; set; }

        // Foreign key for User
        public int UserId { get; set; }

        // Navigation property for User
        public User? User { get; set; }

        public Image()
        {
            Id = 0;
            Model = "Model undefined";            
            Size = "Size undefined";
            Hd = false;
            BlobName = "BlobName undefined";
            TimeStamp = DateTime.MinValue;
            UserId = 0;
        }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}