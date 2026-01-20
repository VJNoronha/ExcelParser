
namespace ExcelParser.Models;
public class StagingRecord
{
    public System.Guid Id { get; set; } = System.Guid.NewGuid();
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Salary { get; set; } = "";
    public string MaritalStatus { get; set; } = "";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
