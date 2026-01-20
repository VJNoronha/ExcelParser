using ExcelParser.Models;
public record ValidationResult(
    List<MainRecord> Valid,
    List<ErrorRecord> Invalid
);
