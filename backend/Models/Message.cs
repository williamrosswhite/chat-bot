namespace backend.Models
{
    using System;

    /// <summary>
    /// Represents a message.
    /// </summary>
    public class Message
    {
        private string _prompt = string.Empty;
        private string _response = string.Empty;

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the prompt. If not defined, defaults to an empty string.
        /// </summary>
        public string Prompt
        {
            get => _prompt;
            set => _prompt = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the response. If not defined, defaults to an empty string.
        /// </summary>
        public string Response
        {
            get => _response;
            set => _response = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the timestamp.
        /// </summary>
        public DateTime TimeStamp { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the user ID.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the user.
        /// </summary>
        public User User { get; set; } = new User();
    }
}