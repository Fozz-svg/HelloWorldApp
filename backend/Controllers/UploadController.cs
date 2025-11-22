// using Microsoft.AspNetCore.Mvc;
// using Azure.Storage.Blobs;

// [ApiController]
// [Route("api/[controller]")]
// public class UploadController : ControllerBase {
//   private readonly BlobServiceClient _blob;
//   public UploadController(BlobServiceClient blob) => _blob = blob;

//   [HttpPost]
//   public async Task<IActionResult> Upload(IFormFile file) {
//     var container = _blob.GetBlobContainerClient("uploads");
//     await container.CreateIfNotExistsAsync();
//     var client = container.GetBlobClient(file.FileName);
//     using var stream = file.OpenReadStream();
//     await client.UploadAsync(stream, overwrite: true);
//     return Ok(new { Url = client.Uri.ToString() });
//   }
// }
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly BlobServiceClient _blob;
    private readonly ILogger<UploadController> _logger;

    public UploadController(BlobServiceClient blob, ILogger<UploadController> logger)
    {
        _blob = blob;
        _logger = logger;
    }

    [HttpPost]
    public async Task<IActionResult> Upload(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            _logger.LogWarning("Upload attempt with empty or null file.");
            return BadRequest("File is empty or missing.");
        }

        _logger.LogInformation("Starting upload for file: {FileName}", file.FileName);

        var container = _blob.GetBlobContainerClient("uploads");
        await container.CreateIfNotExistsAsync();

        var client = container.GetBlobClient(file.FileName);

        using var stream = file.OpenReadStream();
        await client.UploadAsync(stream, overwrite: true);

        _logger.LogInformation("Upload complete. Blob URI: {BlobUri}", client.Uri.ToString());

        return Ok(new { Url = client.Uri.ToString() });
    }
}

