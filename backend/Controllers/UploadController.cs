using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase {
  private readonly BlobServiceClient _blob;
  public UploadController(BlobServiceClient blob) => _blob = blob;

  [HttpPost]
  public async Task<IActionResult> Upload(IFormFile file) {
    var container = _blob.GetBlobContainerClient("uploads");
    await container.CreateIfNotExistsAsync();
    var client = container.GetBlobClient(file.FileName);
    using var stream = file.OpenReadStream();
    await client.UploadAsync(stream, overwrite: true);
    return Ok(new { Url = client.Uri.ToString() });
  }
}
