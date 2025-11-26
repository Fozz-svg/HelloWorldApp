using Backend.Data;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;

Console.WriteLine("--- BACKEND STARTING (VERSION: DEBUG-ROUTE) ---");
var builder = WebApplication.CreateBuilder(args);

// Configure URLs for Azure App Service
var port = Environment.GetEnvironmentVariable("PORT") ?? "8080";
builder.WebHost.UseUrls($"http://*:{port}");

builder.Services.AddControllers();
builder.Services.AddApplicationInsightsTelemetry();

// SQL connection: prefer appsettings.json, fallback to Azure env var
var sqlConn = builder.Configuration.GetConnectionString("AzureSql")
              ?? Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTION_STRING");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(sqlConn));

// Blob connection: prefer appsettings.json, fallback to Azure env var
var blobConn = builder.Configuration.GetConnectionString("AzureStorage")
               ?? Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");

builder.Services.AddSingleton(new BlobServiceClient(blobConn));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Auto-create database on startup
// TEMPORARILY DISABLED FOR TESTING
/*
try 
{
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        Console.WriteLine("Attempting DB connection...");
        db.Database.EnsureCreated();
        Console.WriteLine("DB Connection Successful!");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"DB Error: {ex.Message}");
}
*/
Console.WriteLine("Skipping DB initialization for testing");

app.UseCors("AllowAll");

app.MapGet("/health", () => "OK");
app.MapGet("/{*path}", (string path) => $"Catch-All Echo: /{path}");

app.MapControllers();
app.Run();


