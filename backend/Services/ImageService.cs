using Microsoft.AspNetCore.Mvc;
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
    private readonly ILogger<ImageService> _logger;

    public ImageService(ChatbotDBContext context, HttpClient httpClient, BlobServiceClient blobServiceClient, ILogger<ImageService> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _blobServiceClient = blobServiceClient ?? throw new ArgumentNullException(nameof(blobServiceClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<List<Image>> GetImagesAsync()
    {
        try
        {
            return await _context.Images.ToListAsync().ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error retrieving images from the database in {nameof(GetImagesAsync)}.");
            throw;
        }    
    }

    internal async Task<IActionResult> GetImageUrlsAsync(int limit, int offset)
    {
        // Validate input
        if (limit < 0 || offset < 0)
        {
            _logger.LogError($"Invalid parameters in {nameof(GetImageUrlsAsync)}: limit and offset must be non-negative.");
            return new BadRequestObjectResult("Invalid parameters: limit and offset must be non-negative.");
        }

        try
        {
            var images = await GetImagesAsync().ConfigureAwait(false);

            // Take requested number of images in descending order by date from the provided offset
            images = images.OrderByDescending(image => image.TimeStamp).Skip(offset).Take(limit).ToList();

            var containerClient = _blobServiceClient.GetBlobContainerClient("generated-images");

            List<ImageReturn> returnImages = new List<ImageReturn>();

            foreach (var image in images)
            {
                if (image.BlobName is not null)
                {
                    var blobClient = containerClient.GetBlobClient(image.BlobName);

                    // Create a SAS token that's valid for one day
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

                    // Create ImageReturn object using CreateImageReturn method and add to returnImages
                    ImageReturn returnImage = CreateImageReturn(image, sasToken.ToString());
                    returnImages.Add(returnImage);
                }
                else
                {
                    _logger.LogWarning($"Image with ID {image.Id} has null BlobName in {nameof(GetImageUrlsAsync)}.");
                }
            }

            return new OkObjectResult(returnImages);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error generating image URLs in {nameof(GetImageUrlsAsync)}.");
            return new BadRequestObjectResult($"Error generating image URLs in {nameof(GetImageUrlsAsync)}: {ex.Message}");
        }
    }

    internal async Task<string> UploadBlobImage(Image image, string imageUrl, DateTime timeStamp) 
    {
        try
        {
            _logger.LogInformation($"Attempting upload of image to blob storage in {nameof(UploadBlobImage)}...");

            // Define a policy that will handle exceptions and 404 status codes
            var policy = Policy
                .Handle<HttpRequestException>()
                .OrResult<HttpResponseMessage>(message => message.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(5, retryAttempt)),        
                    onRetry: (exception, timeSpan, retryCount, context) =>
                    {
                        _logger.LogInformation($"Blob storage upload retry {retryCount}. Waiting {timeSpan.TotalSeconds} seconds before next attempt.");
                    });

            HttpResponseMessage imageResponse;

            try
            {
                // Download the image
                imageResponse = await policy.ExecuteAsync(() => _httpClient.GetAsync(imageUrl)).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error downloading image in {nameof(UploadBlobImage)}.");
                throw;
            }

            byte[] imageBytes;

            try
            {
                // Read the contents of the downloaded image
                imageBytes = await imageResponse.Content.ReadAsByteArrayAsync().ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error converting image to byte array in {nameof(UploadBlobImage)}.");
                throw;
            }

            _logger.LogInformation("Image successfully downloaded and read.  Attempting to upload to blob storage...");

            BlobContainerClient containerClient;

            try
            {
                // Create the container, return a container client object, then create new blob and upload the image
                containerClient = _blobServiceClient.GetBlobContainerClient("generated-images");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating blob container client in {nameof(UploadBlobImage)}.");
                throw;
            }

            BlobClient blobClient = containerClient.GetBlobClient(Guid.NewGuid().ToString());
            
            using var stream = new MemoryStream(imageBytes);
            var metadata = new Dictionary<string, string>
            {
                { "UserId", image.UserId.ToString() },
                { "Model", image.Model?.ToString() ?? string.Empty},
                { "Prompt", image.ImagePromptText?.ToString() ?? string.Empty },
                { "Size", image.Size.ToString() },
                { "Style", image.Style?.ToString() ?? string.Empty },
                { "Hd", image.Hd.ToString() },
                { "Timestamp", image.TimeStamp.ToString() },
            };

            var blobUploadOptions = new BlobUploadOptions
            {
                Metadata = metadata
            };

            try
            {
                // Upload the image to blob storage
                await blobClient.UploadAsync(stream, blobUploadOptions).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error uploading image to blob storage in {nameof(UploadBlobImage)}.");
                throw;
            }

            _logger.LogInformation($"Image uploaded to blob storage. Blob URL: {blobClient.Uri.AbsoluteUri}"); 

            // Return the URL of the uploaded blob
            return blobClient.Name;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error in {nameof(UploadBlobImage)} method.");
            throw;
        }
    }

    public ImageReturn CreateImageReturn(Image image, string imageUrl)
    {
        try
        {
            if (image is null)
            {
                throw new ArgumentNullException(nameof(image), "Image cannot be null.");
            }

            if (string.IsNullOrEmpty(imageUrl))
            {
                throw new ArgumentException("Image URL cannot be null or empty.", nameof(imageUrl));
            }

            return new ImageReturn
            {
                ImageUrl = imageUrl,
                ImagePromptText = image.ImagePromptText ?? string.Empty,
                Model = image.Model ?? string.Empty,
                Size = image.Size,
                Style = image.Style.HasValue ? image.Style.Value : false,
                Hd = image.Hd,
                GuidanceScale = image.GuidanceScale.HasValue ? image.GuidanceScale.Value : 0,
                InferenceDenoisingSteps = image.InferenceDenoisingSteps.HasValue ? image.InferenceDenoisingSteps.Value : 0,
                Seed = image.Seed.HasValue ? image.Seed.Value : 0,
                Samples = image.Samples.HasValue ? image.Samples.Value : 0
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Error creating ImageReturn object in {nameof(CreateImageReturn)}.");
            throw;
        }
    }
}