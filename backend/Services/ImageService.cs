
using backend;
using backend.Models;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using Azure.Storage.Sas;
using Newtonsoft.Json;
using Azure;
using Polly;

public class ImageService
{
    private readonly ChatbotDBContext _context;
    private readonly HttpClient _httpClient;
    private readonly BlobServiceClient _blobServiceClient;

    public ImageService(ChatbotDBContext context, HttpClient httpClient, BlobServiceClient blobServiceClient)
    {
        _context = context;
        _httpClient = httpClient;
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

    internal async Task<string> UploadBlobImageFromOpenAi(string base64String, ImageRequest imageRequest, DateTime timeStamp) 
    {
        try {
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

            var imageMetadata = new Dictionary<string, string>
            {
                { "model", imageRequest.Model },
                { "prompt", imageRequest.ImagePromptText ?? string.Empty },
                { "size", imageRequest.Size },
                { "response_format", "b64_json" },
                { "timestamp", timeStamp.ToString() }
            };

            #pragma warning disable CS8601
            if(imageRequest != null) {
                if (imageRequest.Samples != null)
                {
                    imageMetadata["n"] = imageRequest.Samples?.ToString();
                }

                if (imageRequest.Hd == true)
                {
                    imageMetadata["quality"] = "hd";
                }

                if (imageRequest.Style == true)
                {
                    imageMetadata["style"] = "natural";
                }
            }
            #pragma warning restore CS8601

            BlobUploadOptions uploadOptions = new BlobUploadOptions 
            { 
                Metadata = imageMetadata,
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
        catch (RequestFailedException e) {
            return e.ToString();
        }
    }

    internal async Task<string> UploadBlobImageFromStableDiffusion(string imageUrl, DateTime timeStamp) 
    {
        Console.WriteLine("Uploading image to blob storage...");

        // Define a policy that will handle exceptions and 404 status codes
        var policy = Policy
            .Handle<HttpRequestException>()
            .OrResult<HttpResponseMessage>(message => message.StatusCode == System.Net.HttpStatusCode.NotFound)
            .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));

        // Download the image using the policy to execute the HTTP request
        var imageResponse = await policy.ExecuteAsync(() => _httpClient.GetAsync(imageUrl));
        Console.WriteLine("Image downloaded successfully.");

        var imageBytes = await imageResponse.Content.ReadAsByteArrayAsync();
        Console.WriteLine("Image converted to byte array successfully.");

        // Create the container and return a container client object
        BlobContainerClient containerClient = _blobServiceClient.GetBlobContainerClient("generated-images");
        Console.WriteLine("Blob container client created successfully.");

        // Create a new blob and upload the image
        BlobClient blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString());
        using (var stream = new MemoryStream(imageBytes))
        {
            await blobClient.UploadAsync(stream);
        }

        Console.WriteLine($"Image uploaded to blob storage. Blob URL: {blobClient.Uri.AbsoluteUri}");

        // Return the URL of the uploaded blob
        return blobClient.Name;
    }

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