using ExcelParser.Data;
using ExcelParser.Models;
/// Writes validated records to the database.//

public class RecordWriter : IRecordWriter
{
    private readonly ExcelDbContext _db;

    public RecordWriter(ExcelDbContext db)
    {
        _db = db;
    }

    public async Task WriteAsync(
        Guid uploadId,
        List<MainRecord> valid,
        List<ErrorRecord> invalid,
        CancellationToken token)
    {
        foreach (var v in valid) v.FileUploadId = uploadId;
        foreach (var i in invalid) i.FileUploadId = uploadId;

        _db.MainTable.AddRange(valid);
        _db.ErrorTable.AddRange(invalid);

        await _db.SaveChangesAsync(token);
    }
}
