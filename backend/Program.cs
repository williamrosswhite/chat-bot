using backend;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using dotenv.net;
using DotNetEnv;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Configure logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddDebug();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string? connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Could not find a connection string named 'DefaultConnection'.");
}

builder.Services.AddDbContext<ChatbotDBContext>(options =>
    options.UseSqlServer(connectionString,
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

// Add HttpClient with pre-configured settings for OpenAI
builder.Services.AddHttpClient("OpenAI", client =>
{
    client.BaseAddress = new Uri("https://api.openai.com/");
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", builder.Configuration["OPENAI_API_KEY"]);
});

// Add HttpClient with pre-configured settings for StableDiffusion
builder.Services.AddHttpClient("StableDiffusion", client =>
{
    client.BaseAddress = new Uri("https://stablediffusionapi.com/");
    client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", builder.Configuration["STABLE_DIFFUSION_KEY"]);
});

// Add BlobServiceClient to the services
builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobStorage")));

// Register OpenAIClient service
builder.Services.AddScoped<OpenAIClient>();

// Register OpenAIClient service
builder.Services.AddScoped<StableDiffusionClientService>();

// Register ImageService
builder.Services.AddScoped<ImageService>();

var app = builder.Build();

// Get a logger
var logger = app.Services.GetRequiredService<ILogger<Program>>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    logger.LogInformation("Development Environment, must launch front end independently.");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
} 
else {
    logger.LogInformation("Staging Environment, launching with internal static front end.");
    app.UseDefaultFiles();
    app.UseStaticFiles();
}

// Enable CORS
app.UseCors(builder => builder
    .WithOrigins("http://localhost:5111", "http://localhost:5112", "https://rwhite83-openai-test.azurewebsites.net/") 
    .AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials());

app.UseRouting();

app.UseAuthorization();

app.MapControllers();

app.Run();