namespace backend
{
    /// <summary>
    /// Represents a chat message.
    /// </summary>
    public class ChatMessage
    {
        private string _role = "Role undefined";
        private string _content = "Content undefined";

        /// <summary>
        /// Gets or sets the role. If not defined, defaults to "Role undefined".
        /// </summary>
        public string Role
        {
            get => _role;
            set => _role = value ?? "Role undefined";
        }

        /// <summary>
        /// Gets or sets the content. If not defined, defaults to "Content undefined".
        /// </summary>
        public string Content
        {
            get => _content;
            set => _content = value ?? "Content undefined";
        }
    }
}