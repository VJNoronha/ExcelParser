using Microsoft.AspNetCore.Mvc;
using ExcelParser.Data;
using ExcelParser.Models;
using Microsoft.Extensions.Logging;

namespace ExcelParser.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UploadController : ControllerBase
{
    private readonly IWebHostEnvironment _env;
    private readonly IServiceProvider _sp;
    private readonly ILogger<UploadController> _logger;

    public UploadController(IWebHostEnvironment env, IServiceProvider sp, ILogger<UploadController> logger)
    {
        _env = env;
        _sp = sp;
        _logger = logger;
    }

    [HttpPost]
    [Route("file")]
    public async Task<IActionResult> UploadFile(IFormFile file)
    {
        if (file == null || file.Length == 0) return BadRequest("No file uploaded");

        var uploads = Path.Combine(_env.ContentRootPath, "Uploads", "Ready");
        Directory.CreateDirectory(uploads);

        var filePath = Path.Combine(uploads, Path.GetRandomFileName() + Path.GetExtension(file.FileName));
        try
        {
            using (var stream = System.IO.File.Create(filePath))
            {
                await file.CopyToAsync(stream);
            }

            using var scope = _sp.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<ExcelDbContext>();
            db.FileUploads.Add(new FileUpload { FileName = file.FileName, Path = filePath });
            await db.SaveChangesAsync();

            _logger.LogInformation("File uploaded and saved: {FilePath}", filePath);
            return Ok(new { saved = filePath });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save uploaded file: {FileName}", file?.FileName);
            return StatusCode(500, "Failed to save file");
        }
    }
}

