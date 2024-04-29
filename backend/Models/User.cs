namespace backend.Models
{
    using System.Collections.Generic;

    /// <summary>
    /// Represents a user.
    /// </summary>
    public class User
    {
        private string _username = string.Empty;
        private string _email = string.Empty;
        private string _password = string.Empty;

        /// <summary>
        /// Gets or sets the ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Gets or sets the username. If not defined, defaults to an empty string.
        /// </summary>
        public string Username
        {
            get => _username;
            set => _username = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the email. If not defined, defaults to an empty string.
        /// </summary>
        public string Email
        {
            get => _email;
            set => _email = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the password. If not defined, defaults to an empty string.
        /// </summary>
        public string Password
        {
            get => _password;
            set => _password = value ?? string.Empty;
        }

        /// <summary>
        /// Gets or sets the navigation property for associated images.
        /// </summary>
        public List<Image> Images { get; set; } = new List<Image>();

        /// <summary>
        /// Gets or sets the navigation property for associated messages.
        /// </summary>
        public List<Message> Messages { get; set; } = new List<Message>();
    }
}