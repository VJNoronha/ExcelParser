using ExcelParser.Data;
using ExcelParser.Models;
using Microsoft.AspNetCore.Http;

namespace ExcelParser.Services.Upload;

public class UploadService : IUploadService
{
    private readonly ExcelDbContext _db;
    private readonly string _uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "Uploads", "Ready");

    public UploadService(ExcelDbContext db)
    {
        _db = db;
        Directory.CreateDirectory(_uploadDirectory);
    }

    public async Task<UploadResult> UploadAsync(IFormFile file)
    {
        if (file == null || file.Length == 0)
            throw new ArgumentException("File is required");

        var uploadId = Guid.NewGuid();
        var fileName = $"{uploadId}_{file.FileName}";
        var filePath = Path.Combine(_uploadDirectory, fileName);

        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await file.CopyToAsync(stream);
        }

        var fileUpload = new FileUpload
        {
            Id = uploadId,
            FileName = file.FileName,
            Path = filePath,
            UploadedAt = DateTime.UtcNow,
            IsProcessed = false
        };

        _db.FileUploads.Add(fileUpload);
        await _db.SaveChangesAsync();

        return new UploadResult(uploadId, "File uploaded successfully");
    }
}