namespace backend.Models
{
    using System;

    /// <summary>
    /// Represents an image.
    /// </summary>
    public class Image: ICloneable
    {
        private string _model = "Model undefined";
        private string _size = "Size undefined";
        private string _blobName = "BlobName undefined";

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the image prompt text.
        /// </summary>
        public string? ImagePromptText { get; set; }

        /// <summary>
        /// Gets or sets the model. If not defined, defaults to "Model undefined".
        /// </summary>
        public string Model
        {
            get => _model;
            set => _model = value ?? "Model undefined";
        }

        /// <summary>
        /// Gets or sets the size. If not defined, defaults to "Size undefined".
        /// </summary>
        public string Size
        {
            get => _size;
            set => _size = value ?? "Size undefined";
        }

        /// <summary>
        /// Gets or sets the style.
        /// </summary>
        public bool? Style { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether HD is enabled.
        /// </summary>
        public bool Hd { get; set; }

        /// <summary>
        /// Gets or sets the guidance scale.
        /// </summary>
        public int? GuidanceScale { get; set; }

        /// <summary>
        /// Gets or sets the inference denoising steps.
        /// </summary>
        public int? InferenceDenoisingSteps { get; set; }

        /// <summary>
        /// Gets or sets the seed.
        /// </summary>
        public long? Seed { get; set; }

        /// <summary>
        /// Gets or sets the number of samples.
        /// </summary>
        public int? Samples { get; set; }

        /// <summary>
        /// Gets or sets the blob name. If not defined, defaults to "BlobName undefined".
        /// </summary>
        public string BlobName
        {
            get => _blobName;
            set => _blobName = value ?? "BlobName undefined";
        }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public DateTime TimeStamp { get; set; } = DateTime.MinValue;

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public User? User { get; set; }

        public object Clone()
        {
            return this.MemberwiseClone();
        }
    }
}