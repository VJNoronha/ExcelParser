namespace ExcelParser.Models;

public class MainRecord
{
    public Guid Id { get; set; }
    public Guid FileUploadId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public decimal Salary { get; set; }
    public string MaritalStatus { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public DateTime InsertedAt { get; set; }
}
