using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
namespace ExcelParser.Services;
public class ExcelBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<ExcelBackgroundService> _logger;

    public ExcelBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<ExcelBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    
    //Listens for background processing requests every 5 seconds//
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Excel background service started");

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);

            using var scope = _scopeFactory.CreateScope();
            var processor = scope.ServiceProvider
                .GetRequiredService<IUploadProcessingService>();

            try
            {
                await processor.ProcessPendingUploadsAsync(stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error in background processing loop");
            }
        }
    }
}
