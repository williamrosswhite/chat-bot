using backend;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ChatbotDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlServerOptionsAction: sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);
        }));

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