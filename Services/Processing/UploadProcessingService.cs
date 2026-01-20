using ExcelParser.Data;
using ExcelParser.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
// Coordinates the full lifecycle of processing uploaded Excel files.//
 public class UploadProcessingService : IUploadProcessingService
{
    private readonly ExcelDbContext _db;
    private readonly IExcelReader _reader;
    private readonly IRowValidator _validator;
    private readonly IRecordWriter _writer;
    private readonly ILogger<UploadProcessingService> _logger;

    public UploadProcessingService(
        ExcelDbContext db,
        IExcelReader reader,
        IRowValidator validator,
        IRecordWriter writer,
        ILogger<UploadProcessingService> logger)
    {
        _db = db;
        _reader = reader;
        _validator = validator;
        _writer = writer;
        _logger = logger;
    }
    // Finds unprocessed uploads and processes them one by one.
    public async Task ProcessPendingUploadsAsync(CancellationToken token)
    {
        var uploads = await _db.FileUploads
            .Where(u => !u.IsProcessed)
            .ToListAsync(token);

        foreach (var upload in uploads)
        {
            await ProcessSingleUploadAsync(upload, token);
        }
    }

    // Processes a single uploaded Excel file://
    // - Reads rows//
    // - Validates rows//
    // - Writes valid & invalid records//
    // - Marks upload as processed//
    
    private async Task ProcessSingleUploadAsync(FileUpload upload, CancellationToken token)
    {
        try
        {
            _logger.LogInformation("Processing upload {UploadId}", upload.Id);

            var rows = _reader.Read(upload.Path);

            var validationResult = _validator.Validate(rows);

            await _writer.WriteAsync(
                upload.Id,
                validationResult.Valid,
                validationResult.Invalid,
                token
            );

            upload.IsProcessed = true;
            upload.ProcessedAt = DateTime.UtcNow;

            await _db.SaveChangesAsync(token);

            _logger.LogInformation(
                "Upload {UploadId} processed: {Valid} valid, {Invalid} invalid",
                upload.Id,
                validationResult.Valid.Count,
                validationResult.Invalid.Count
            );
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing upload {UploadId}", upload.Id);
            // Mark as processed to stop retrying
            upload.IsProcessed = true;
            upload.ProcessedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(token);
        }
    }
}
