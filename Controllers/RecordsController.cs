using ExcelParser.Services.Persistence;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/[controller]")]
public class RecordsController : ControllerBase
{
    private readonly IRecordReadService _service;

    public RecordsController(IRecordReadService service)
    {
        _service = service;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
    {
        return Ok(await _service.GetSummaryAsync());
    }

    [HttpGet("summary/{uploadId}")]
    public async Task<IActionResult> Summary(Guid uploadId)
    {
        return Ok(await _service.GetSummaryAsync(uploadId));
    }

    [HttpGet("complete")]
    public async Task<IActionResult> Complete()
    {
        return Ok(await _service.GetCompleteAsync());
    }

    [HttpDelete("cleanup")]
    public async Task<IActionResult> Cleanup()
    {
        var deletedCount = await _service.CleanupFailedUploadsAsync();
        return Ok(new { Message = $"Cleaned up {deletedCount} failed uploads" });
    }
}
