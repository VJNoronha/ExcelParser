public interface IUploadProcessingService
{
    Task ProcessPendingUploadsAsync(CancellationToken token);
}
