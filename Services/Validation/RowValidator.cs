using ExcelParser.Models;
// Applies business validation rules to Excel rows.// 
//Identifies valid and invalid records based on required fields.//

public class RowValidator : IRowValidator
{
    private static readonly string[] ValidMaritalStatuses = { "Single", "Married", "Divorced", "Widowed" };

    public ValidationResult Validate(IEnumerable<ExcelRow> rows)
    {
        var valid = new List<MainRecord>();
        var invalid = new List<ErrorRecord>();

        foreach (var row in rows)
        {
            var errors = new List<string>();

            if (string.IsNullOrWhiteSpace(row.Name)) errors.Add("Name is required");
            if (string.IsNullOrWhiteSpace(row.Email)) errors.Add("Email is required");
            if (string.IsNullOrWhiteSpace(row.Salary)) 
                errors.Add("Salary is required");
            else if (!decimal.TryParse(row.Salary, out _)) 
                errors.Add("Salary must be a valid number");
            if (string.IsNullOrWhiteSpace(row.MaritalStatus)) 
                errors.Add("MaritalStatus is required");
            else if (!ValidMaritalStatuses.Contains(row.MaritalStatus, StringComparer.OrdinalIgnoreCase)) 
                errors.Add("MaritalStatus must be one of: " + string.Join(", ", ValidMaritalStatuses));

            if (!errors.Any())
            {
                valid.Add(new MainRecord
                {
                    Id = Guid.NewGuid(),
                    Name = row.Name!,
                    Email = row.Email!,
                    Salary = decimal.Parse(row.Salary!),
                    MaritalStatus = row.MaritalStatus!,
                    IsValid = true,
                    InsertedAt = DateTime.UtcNow
                });
            }
            else
            {
                invalid.Add(new ErrorRecord
                {
                    Id = Guid.NewGuid(),
                    Name = row.Name ?? "",
                    Email = row.Email ?? "",
                    Salary = row.Salary ?? "",
                    MaritalStatus = row.MaritalStatus ?? "",
                    IsValid = false,
                    Reason = string.Join("; ", errors),
                    LoggedAt = DateTime.UtcNow
                });
            }
        }

        return new ValidationResult(valid, invalid);
    }
}
