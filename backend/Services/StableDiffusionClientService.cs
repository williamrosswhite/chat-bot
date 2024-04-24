using System.Text;
using backend;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using backend.Models;

public class StableDiffusionClient
{
    //private readonly ChatbotDBContext _context;
    private readonly HttpClient _httpClient;
    private readonly ImageService _imageService;

    public StableDiffusionClient(ImageService imageService)
    {
        //_context = context;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("https://stablediffusionapi.com/")
        };
        _httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", "qZG3Qi7M1g3tDJea7G80i8hgsFQovZE9Or2BU5oh0xt55A7WmfVg97nxAHUy");
        _imageService = imageService;
    }

    public async Task<IActionResult> ProcessImagePrompt(ImageRequest imageRequest)
    {
        var url = "api/v3/text2img";

        string stableDiffusionKey = Environment.GetEnvironmentVariable("STABLEDIFFUSION_API_KEY");

        var data = new
        {
            key = stableDiffusionKey,
            prompt = imageRequest.ImagePromptText,
            negative_prompt = "",
            width = "1024",
            height = "1024",
            samples = "1",
            num_inference_steps = "20",
            seed = "",
            guidance_scale = 7.5,
            safety_checker = "no",
            multi_lingual = "no",
            panorama = "no",
            self_attention = "no",
            upscale = "no",
            embeddings_model = "",
            webhook = "",
            track_id = ""
        };

        // var image = new Image { 
        //     UserId = 1, 
        //     ImagePromptText = imageRequest.ImagePromptText, 
        //     Model = imageRequest.Model, 
        //     Size = imageRequest.Size, 
        //     Style = imageRequest.Style.GetValueOrDefault(), 
        //     Hd = imageRequest.Hd.GetValueOrDefault(), 
        //     TimeStamp = DateTime.Now 
        // };

        //var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");
        //var response = await _httpClient.PostAsync(url, content);

        var content = new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);

        if (response.IsSuccessStatusCode)
        {
            var result = await response.Content.ReadAsStringAsync();
            dynamic jsonResponse = JsonConvert.DeserializeObject(result);
            string imageUrl = jsonResponse.image_url;

            // Download the image
            var imageResponse = await _httpClient.GetAsync(imageUrl);
            var imageBytes = await imageResponse.Content.ReadAsByteArrayAsync();

            //var blobName = await _imageService.UploadBlobImageFromStableDiffusion(imageUrl, data);

            Console.WriteLine(result);
            return new OkObjectResult(result);
        }
        else
        {
            Console.WriteLine($"Error: {response.StatusCode}");
            throw new Exception($"Error calling OpenAI API: {response.StatusCode}");
        }

        // if (response.IsSuccessStatusCode)
        // {
        //     var result = await response.Content.ReadAsStringAsync();
            
        //     var resultObject = JsonConvert.DeserializeObject<dynamic>(result);

        //     #nullable disable
        //     var base64String = resultObject["data"]?[0]?["b64_json"]?.ToString();
        //     #nullable enable

        //     if (base64String == null)
        //     {
        //         throw new InvalidOperationException("Could not find 'b64_json' in the response.");
        //     }

        //     var blobName = await _imageService.UploadBlobImage(base64String, image);

        //     image.BlobName = blobName;
        //     _context.Images.Add(image);
        //     _context.SaveChanges();

        //     return new OkObjectResult(result);
        // }
        // else
        // {
        //     image.BlobName = null;
        //     _context.Images.Add(image);
        //     _context.SaveChanges();

        //     throw new Exception($"Error calling OpenAI API: {response.StatusCode}");
        // }
    }
}