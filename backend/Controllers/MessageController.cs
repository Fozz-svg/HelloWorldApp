using Microsoft.AspNetCore.Mvc;
using Backend.Data;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class MessageController : ControllerBase {
  private readonly AppDbContext _db;
  public MessageController(AppDbContext db) => _db = db;

  [HttpPost]
  public async Task<IActionResult> Post([FromBody] Message msg) {
    _db.Messages.Add(msg);
    await _db.SaveChangesAsync();
    return Ok(msg);
  }

  [HttpGet]
  public async Task<IActionResult> Get() => Ok(await _db.Messages.ToListAsync());
}
