using ExcelParser.Models;

namespace ExcelParser.Services.Upload;

public interface IUploadService
{
    Task<UploadResult> UploadAsync(IFormFile file);
}

public record UploadResult(
    Guid UploadId,
    string Message
);