namespace ExcelParser.Models;

public class ErrorRecord
{
    public Guid Id { get; set; }
    public Guid FileUploadId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Salary { get; set; } = string.Empty;
    public string MaritalStatus { get; set; } = string.Empty;
    public bool IsValid { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime LoggedAt { get; set; }
}

