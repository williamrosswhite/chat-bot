
using backend;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Newtonsoft.Json;

public class ImageService
{
    private readonly ChatbotDBContext _context;
    private readonly BlobServiceClient _blobServiceClient;

    public ImageService(ChatbotDBContext context, BlobServiceClient blobServiceClient)
    {
        _context = context;
        _blobServiceClient = blobServiceClient;
    }

    public async Task<List<Image>> GetImagesAsync()
    {
        return await _context.Images.ToListAsync();
    }

    internal async Task<List<string>> GetImageUrlsAsync()
    {
        var images = await GetImagesAsync();

        var containerClient = _blobServiceClient.GetBlobContainerClient("generated-images");

        var imageUrls = images.Where(image => image.BlobName != null).Select(image =>
        {
            var blobClient = containerClient.GetBlobClient(image.BlobName);

            var sasBuilder = new BlobSasBuilder
            {
                BlobContainerName = containerClient.Name,
                BlobName = blobClient.Name,
                Resource = "b",
                StartsOn = DateTimeOffset.UtcNow.AddHours(-1),
                ExpiresOn = DateTimeOffset.UtcNow.AddHours(24),
            };

            sasBuilder.SetPermissions(BlobSasPermissions.Read);

            var sasToken = blobClient.GenerateSasUri(sasBuilder);

            return sasToken.ToString();
        }).ToList();

        return imageUrls;
    }

    internal async Task<string> UploadBlobImageFromOpenAi(string base64String, Image image) 
    {
        // Convert base64 string to byte array
        byte[] imageBytes = Convert.FromBase64String(base64String);

        // Create the container and return a container client object
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("generated-images");

        // Create a local file in the ./data/ directory for uploading and downloading
        string localPath = "./data/";
        string fileName = Guid.NewGuid().ToString(); // Generate a new name for the image
        string localFilePath = Path.Combine(localPath, fileName);

        // Write the blob to a file
        await File.WriteAllBytesAsync(localFilePath, imageBytes);

        // Get a reference to a blob
        BlobClient blobClient = containerClient.GetBlobClient(fileName);

        // Open the file and upload its data
        using FileStream uploadFileStream = File.OpenRead(localFilePath);
        Dictionary<string, string> metadata = new Dictionary<string, string>
        {
            { "TimeStamp", image.TimeStamp.ToString() },
            { "UserId", image.UserId.ToString() },
            { "ImagePromptText", image.ImagePromptText },
            { "Model", image.Model },
            { "Size", image.Size },
            { "Style", image.Style.ToString() },
            { "Hd", image.Hd.ToString() }
        };
        BlobUploadOptions uploadOptions = new BlobUploadOptions 
        { 
            Metadata = metadata,
            HttpHeaders = new BlobHttpHeaders
            {
                ContentType = "image/png",
            }
        };
        await blobClient.UploadAsync(uploadFileStream, uploadOptions);
        uploadFileStream.Close();

        // Delete the local file
        File.Delete(localFilePath);

        // Return the URL of the uploaded blob
        return blobClient.Name;
    }

    // internal async Task<string> UploadBlobImageFromStableDiffusion(string imageUrl, object data)
    // {
    //     // Create the container and return a container client object
    //     BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("generated-images");

    //     // Generate a new name for the image
    //     string blobName = Guid.NewGuid().ToString();

    //     // Get a reference to a blob
    //     BlobClient blobClient = containerClient.GetBlobClient(blobName);

    //     // Download the image from the URL
    //     HttpClient httpClient = new HttpClient();
    //     //byte[] imageBytes = await httpClient.GetByteArrayAsync(imageUrl);

    //     // Deserialize it into a dictionary
    //     var dataDict = JsonConvert.DeserializeObject<Dictionary<string, object>>(data as string);

    //     // Now you can access the 'prompt' property
    //     string prompt = dataDict["prompt"].ToString();

    //     // Prepare the metadata
    //     Dictionary<string, string> metadata = new Dictionary<string, string>
    //     {
    //         { "TimeStamp", dataDict["TimeStamp"].ToString() },
    //         { "UserId", dataDict["UserId"].ToString() },
    //         { "ImagePromptText", prompt },
    //         { "Model", dataDict["Model"].ToString() },
    //         { "Size", dataDict["Size"].ToString() },
    //         { "Style", dataDict["Style"].ToString() },
    //         { "Hd", dataDict["Hd"].ToString() }
    //     };

    //     // Prepare the upload options
    //     BlobUploadOptions uploadOptions = new BlobUploadOptions
    //     {
    //         Metadata = metadata,
    //         HttpHeaders = new BlobHttpHeaders
    //         {
    //             ContentType = "image/png",
    //         }
    //     };

    //     // Upload the image to Blob Storage
    //     using MemoryStream memoryStream = new MemoryStream(imageBytes);
    //     await blobClient.UploadAsync(memoryStream, uploadOptions);

    //     // Return the URL of the uploaded blob
    //     return blobClient.Uri.AbsoluteUri;
    // }

    // public async Task DecodeAndStoreImages()
    // {
    //     Console.WriteLine("Decoding and storing images...");
    //     var images = await GetImagesAsync();
    //     Console.WriteLine("Images: " + images.Count.ToString());

    //     foreach (var image in images)
    //     {
    //         byte[] imageBytes;
    //         try
    //         {
    //             Console.WriteLine("Decoding image: " + image.Id.ToString() + "...");
    //             imageBytes = Convert.FromBase64String(image.ImageUrl);
    //         }
    //         catch (FormatException)
    //         {
    //             Console.WriteLine("Error decoding image: " + image.Id.ToString());
    //             // ImageUrl is not a base64 string
    //             continue;
    //         }

    //         // Create a local file in the ./images/ directory for saving the image
    //         string localPath = "./images/";
    //         Directory.CreateDirectory(localPath);

    //         string fileName = image.Id.ToString(); // Use the image's ID as the file name
    //         string localFilePath = Path.Combine(localPath, fileName);

    //         // Write the image to a file
    //         await File.WriteAllBytesAsync(localFilePath, imageBytes);
    //     }
    // }
}