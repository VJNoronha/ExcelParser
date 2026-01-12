
namespace ExcelParser.Models;
public class ErrorRecord
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public string Reason { get; set; } = "";
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
}
