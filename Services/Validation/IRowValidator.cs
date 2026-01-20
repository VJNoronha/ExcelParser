public interface IRowValidator
{
    ValidationResult Validate(IEnumerable<ExcelRow> rows);
}
