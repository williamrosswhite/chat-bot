public class User
{
    public User()
    {
        Username = string.Empty;
        Email = string.Empty;
        Password = string.Empty;
        Images = new List<Image>();
        Messages = new List<Message>();
    }

    public int Id { get; set; }

    public string Username { get; set; }
    
    public string Email { get; set; }

    public string Password { get; set; }

    // Navigation property for associated images
    public List<Image> Images { get; set; }
    
    // Navigation property for associated images
    public List<Message> Messages { get; set; }
}