
namespace ExcelParser.Models;
public class MainRecord
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime InsertedAt { get; set; } = DateTime.UtcNow;
}
