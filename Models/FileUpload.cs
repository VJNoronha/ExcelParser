namespace ExcelParser.Models;
public class FileUpload
{
    public int Id { get; set; }
    public string FileName { get; set; } = "";
    public string Path { get; set; } = "";
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;
}
