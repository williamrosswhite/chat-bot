public class Message
{
    public Message() {
        Prompt = string.Empty;
        Response = string.Empty;
        TimeStamp = DateTime.Now; 
        UserId = 0;
        User = new User();
    }

    public int Id { get; set; }

    public string Prompt { get; set; }

    public string? Response { get; set; }

    public DateTime TimeStamp { get; set; }

    // Foreign key for User
    public int UserId { get; set; }

    // Navigation property for User
    public User User { get; set; }
}