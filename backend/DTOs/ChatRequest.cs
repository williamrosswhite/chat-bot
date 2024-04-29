namespace backend
{
    /// <summary>
    /// Represents a chat request.
    /// </summary>
    public class ChatRequest
    {
        /// <summary>
        /// Gets or sets the messages in the chat request.
        /// </summary>
        public ChatMessage[]? messages { get; set; }
    }
}