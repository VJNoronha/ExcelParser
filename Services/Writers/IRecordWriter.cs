using ExcelParser.Models;
public interface IRecordWriter
{
    Task WriteAsync(
        Guid uploadId,
        List<MainRecord> valid,
        List<ErrorRecord> invalid,
        CancellationToken token
    );
}
