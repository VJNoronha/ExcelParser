using ExcelParser.Models;
using ExcelParser.Models.DTOs;

namespace ExcelParser.Services.Persistence;

public interface IRecordReadService
{
    Task<SummaryDto> GetSummaryAsync();
    Task<SummaryDto> GetSummaryAsync(Guid uploadId);
    Task<IEnumerable<MainRecord>> GetCompleteAsync();
    Task<IEnumerable<ErrorRecord>> GetIncompleteAsync();
    Task<int> CleanupFailedUploadsAsync();
}