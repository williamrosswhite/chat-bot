namespace backend
{
    /// <summary>
    /// Represents a chat message.
    /// </summary>
    public class ChatMessage
    {
        private string _role = "role undefined";
        private string _content = "content undefined";

        /// <summary>
        /// Gets or sets the role. If not defined, defaults to "role undefined".
        /// </summary>
        public string role
        {
            get => _role;
            set => _role = value ?? "role undefined";
        }

        /// <summary>
        /// Gets or sets the content. If not defined, defaults to "content undefined".
        /// </summary>
        public string content
        {
            get => _content;
            set => _content = value ?? "content undefined";
        }
    }
}