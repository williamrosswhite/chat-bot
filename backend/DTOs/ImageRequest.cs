namespace backend;

public class ImageRequest
{
    public string? ImagePromptText { get; set; }
    public string Model { get; set; }
    public string Size { get; set; }
    public bool? Style { get; set; }
    public bool Hd { get; set; }
    public int? GuidanceScale { get; set; }
    public int? InferenceDenoisingSteps { get; set; }
    public long? Seed { get; set; }
    public int? Samples { get; set; }
    public ImageRequest()
    {
        Model = "Model undefined";
        Size = "Size undefined";
    }
}
