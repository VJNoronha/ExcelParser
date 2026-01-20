namespace ExcelParser.Models;

public class FileUpload
{
    public Guid Id { get; set; }
    public string FileName { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public DateTime UploadedAt { get; set; }

    // ✅ NEW
    public bool IsProcessed { get; set; }
    public DateTime? ProcessedAt { get; set; }
}
