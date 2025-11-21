using Backend.Data;
using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddApplicationInsightsTelemetry();

// SQL (use env var in Azure)
var conn = builder.Configuration.GetConnectionString("Default")
?? Environment.GetEnvironmentVariable("AZURE_SQL_CONNECTION_STRING");
builder.Services.AddDbContext<AppDbContext>(o => o.UseSqlServer(conn));

// Blob (use env var in Azure)
var blobConn = Environment.GetEnvironmentVariable("AZURE_STORAGE_CONNECTION_STRING");
builder.Services.AddSingleton(new BlobServiceClient(blobConn));

var app = builder.Build();
app.MapControllers();
app.Run();

