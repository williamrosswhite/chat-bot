namespace backend;

public class ImageRequest
{
    public string? ImagePromptText { get; set; }
    public string? Model { get; set; }
    public string? Size { get; set; }
    public bool? Style { get; set; }
    public bool? Hd { get; set; }
}
