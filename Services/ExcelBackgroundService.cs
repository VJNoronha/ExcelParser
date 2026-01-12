
using OfficeOpenXml;
using ExcelParser.Data;
using ExcelParser.Models;
using Microsoft.EntityFrameworkCore;

namespace ExcelParser.Services;
public class ExcelBackgroundService : BackgroundService
{
    private readonly IServiceProvider _sp;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<ExcelBackgroundService> _logger;

    public ExcelBackgroundService(IServiceProvider sp, IWebHostEnvironment env, ILogger<ExcelBackgroundService> logger)
    {
        _sp = sp;
        _env = env;
        _logger = logger;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(5000, stoppingToken);
            var ready = Path.Combine(_env.ContentRootPath, "Uploads", "Ready");
            if (!Directory.Exists(ready)) continue;

            foreach (var file in Directory.GetFiles(ready, "*.xlsx"))
            {
                using var scope = _sp.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<ExcelDbContext>();

                try
                {
                    _logger.LogInformation("Processing file {File}", file);
                    
                    // Clear existing records from both tables
                    db.MainTable.RemoveRange(db.MainTable);
                    db.ErrorTable.RemoveRange(db.ErrorTable);
                    await db.SaveChangesAsync();
                    _logger.LogInformation("Cleared existing records from MainTable and ErrorTable");
                    
                    using var pkg = new ExcelPackage(new FileInfo(file));
                    var sheet = pkg.Workbook.Worksheets[0];

                    var now = DateTime.UtcNow;
                    var validRecords = new List<MainRecord>();
                    var invalidRecords = new List<ErrorRecord>();

                    for (int i = 2; i <= sheet.Dimension.Rows; i++)
                    {
                        var name = sheet.Cells[i, 1].Text?.Trim() ?? "";
                        var email = sheet.Cells[i, 2].Text?.Trim() ?? "";
                        var salary = sheet.Cells[i, 3].Text?.Trim() ?? "";
                        var maritalStatus = sheet.Cells[i, 4].Text?.Trim() ?? "";

                        
                        if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(email) && 
                            !string.IsNullOrEmpty(salary) && !string.IsNullOrEmpty(maritalStatus))
                        {
                            validRecords.Add(new MainRecord { Name = name, Email = email, InsertedAt = now });
                        }
                        else
                        {
                            
                            var reasons = new List<string>();
                            if (string.IsNullOrEmpty(name)) reasons.Add("Name");
                            if (string.IsNullOrEmpty(email)) reasons.Add("Email");
                            if (string.IsNullOrEmpty(salary)) reasons.Add("Salary");
                            if (string.IsNullOrEmpty(maritalStatus)) reasons.Add("MaritalStatus");
                            var reason = string.Join(", ", reasons);
                            invalidRecords.Add(new ErrorRecord { Name = name, Email = email, Reason = reason, LoggedAt = now });
                        }
                    }

                   
                    if (validRecords.Any())
                    {
                        db.MainTable.AddRange(validRecords);
                        _logger.LogInformation("Adding {Count} valid records to MainTable", validRecords.Count);
                    }

                    if (invalidRecords.Any())
                    {
                        db.ErrorTable.AddRange(invalidRecords);
                        _logger.LogInformation("Adding {Count} invalid records to ErrorTable", invalidRecords.Count);
                    }

                    if (validRecords.Any() || invalidRecords.Any())
                    {
                        await db.SaveChangesAsync();
                        _logger.LogInformation("Processed {Total} records from file", validRecords.Count + invalidRecords.Count);
                    }

                    try
                    {
                        File.Delete(file);
                        _logger.LogInformation("Deleted file {File}", file);
                    }
                    catch (Exception delEx)
                    {
                        _logger.LogWarning(delEx, "Failed to delete file {File}", file);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing file {File}", file);
                }
            }
        }
    }
}
