using backend;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using dotenv.net;

DotEnv.Config(new DotEnvOptions());

var builder = WebApplication.CreateBuilder(args);

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

// Add BlobServiceClient to the services
builder.Services.AddSingleton(x => new BlobServiceClient(builder.Configuration.GetConnectionString("AzureBlobStorage")));

// Register OpenAIClient service
builder.Services.AddScoped<OpenAIClient>();

// Register OpenAIClient service
builder.Services.AddScoped<StableDiffusionClient>();

// Register ImageService
builder.Services.AddScoped<ImageService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    Console.WriteLine("Development Environment, must launch front end independently.");
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.RoutePrefix = string.Empty;
    });
} 
else {
    Console.WriteLine("Staging Environment, launching with internal static front end.");
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