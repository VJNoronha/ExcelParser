using Microsoft.AspNetCore.Mvc;
using ExcelParser.Data;
using Microsoft.EntityFrameworkCore;

namespace ExcelParser.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecordsController : ControllerBase
{
    private readonly ExcelDbContext _db;
    private readonly ILogger<RecordsController> _logger;

    public RecordsController(ExcelDbContext db, ILogger<RecordsController> logger)
    {
        _db = db;
        _logger = logger;
    }

    [HttpGet("complete")]
    public async Task<IActionResult> GetCompleteRecords()
    {
        try
        {
            var records = await _db.MainTable.ToListAsync();
            _logger.LogInformation("Retrieved {Count} complete records", records.Count);
            return Ok(records);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving complete records");
            return StatusCode(500, "Error retrieving records");
        }
    }

    [HttpGet("incomplete")]
    public async Task<IActionResult> GetIncompleteRecords()
    {
        try
        {
            var records = await _db.ErrorTable.ToListAsync();
            _logger.LogInformation("Retrieved {Count} incomplete records", records.Count);
            return Ok(records);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving incomplete records");
            return StatusCode(500, "Error retrieving records");
        }
    }

    [HttpGet("summary")]
    public async Task<IActionResult> GetSummary()
    {
        try
        {
            var completeCount = await _db.MainTable.CountAsync();
            var incompleteCount = await _db.ErrorTable.CountAsync();

            return Ok(new
            {
                completeRecords = completeCount,
                incompleteRecords = incompleteCount
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving summary");
            return StatusCode(500, "Error retrieving summary");
        }
    }
}
