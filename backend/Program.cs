using Backend.Data;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

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
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors("AllowAll");

app.MapControllers();
app.Run();


