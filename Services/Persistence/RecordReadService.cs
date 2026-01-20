using ExcelParser.Data;
using ExcelParser.Models;
using ExcelParser.Models.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace ExcelParser.Services.Persistence;

public class RecordReadService : IRecordReadService
{
    
    private readonly ExcelDbContext _db;
    private readonly ILogger<RecordReadService> _logger;

    public RecordReadService(ExcelDbContext db, ILogger<RecordReadService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<SummaryDto> GetSummaryAsync()
    {
        try
        {
            var totalUploads = await _db.FileUploads.CountAsync();
            var processedUploads = await _db.FileUploads.CountAsync(f => f.IsProcessed);
            // Get the most recent processed upload
            var latestProcessedUpload = await _db.FileUploads
                .Where(f => f.IsProcessed)
                .OrderByDescending(f => f.ProcessedAt)
                .FirstOrDefaultAsync();
            if (latestProcessedUpload == null)
            {
                return new SummaryDto(totalUploads, processedUploads, 0, 0);
            }
            var totalValidRecords = await _db.MainTable.CountAsync(m => m.FileUploadId == latestProcessedUpload.Id);
            var totalInvalidRecords = await _db.ErrorTable.CountAsync(e => e.FileUploadId == latestProcessedUpload.Id);

            return new SummaryDto(totalUploads, processedUploads, totalValidRecords, totalInvalidRecords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving summary");
            throw;
        }
    }

    public async Task<IEnumerable<MainRecord>> GetCompleteAsync()
    {
        try
        {
            return await _db.MainTable.ToListAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving complete records");
            throw;
        }
    }

    public async Task<SummaryDto> GetSummaryAsync(Guid uploadId)
    {
        try
        {
            var upload = await _db.FileUploads.FindAsync(uploadId);
            if (upload == null)
            {
                throw new KeyNotFoundException($"Upload with ID {uploadId} not found.");
            }
            var totalValidRecords = await _db.MainTable.CountAsync(m => m.FileUploadId == uploadId);
            var totalInvalidRecords = await _db.ErrorTable.CountAsync(e => e.FileUploadId == uploadId);
            var totalRecords = totalValidRecords + totalInvalidRecords;

            return new SummaryDto(1, upload.IsProcessed ? 1 : 0, totalValidRecords, totalInvalidRecords);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving summary for upload {UploadId}", uploadId);
            throw;
        }
    }

    public async Task<int> CleanupFailedUploadsAsync()
    {
        try
        {
            var processedUploads = await _db.FileUploads
                .Where(f => f.IsProcessed)
                .ToListAsync();

            var failedUploads = new List<FileUpload>();
            foreach (var upload in processedUploads)
            {
                var hasRecords = await _db.MainTable.AnyAsync(m => m.FileUploadId == upload.Id) ||
                                 await _db.ErrorTable.AnyAsync(e => e.FileUploadId == upload.Id);
                if (!hasRecords)
                {
                    failedUploads.Add(upload);
                }
            }

            _db.FileUploads.RemoveRange(failedUploads);
            var deletedCount = await _db.SaveChangesAsync();
            _logger.LogInformation("Cleaned up {Count} failed uploads", deletedCount);
            return deletedCount;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cleaning up failed uploads");
            throw;
        }
    }

    public Task<IEnumerable<ErrorRecord>> GetIncompleteAsync()
    {
        throw new NotImplementedException();
    }
}