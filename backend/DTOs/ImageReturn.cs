namespace backend;

public class ImageReturn
{
    public string ImageUrl { get; set; }
    public string ImagePromptText { get; set; }
    public string Model { get; set; }
    public string Size { get; set; }
    public bool Style { get; set; }
    public bool Hd { get; set; }
    public int GuidanceScale { get; set; }
    public int InferenceDenoisingSteps { get; set; }
    public long Seed { get; set; }
    public int Samples { get; set; }
    public ImageReturn()
    {
        ImageUrl = "Image URL undefined";
        ImagePromptText = "Image prompt text undefined";
        Model = "Model undefined";
        Size = "Size undefined";
        Style = false;
        Hd = false;
        GuidanceScale = 0;
        InferenceDenoisingSteps = 0;
        Seed = 0;
        Samples = 0;
    }
}
