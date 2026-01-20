using OfficeOpenXml;
using ExcelParser.Models;
using System.Collections.Generic;
// Reads Excel files and converts them into raw row objects.//
public class ExcelReader : IExcelReader
{   
    private const int StartRow = 2;
    private const int NameColumn = 1;
    private const int EmailColumn = 2;
    private const int SalaryColumn = 3;
    private const int MaritalStatusColumn = 4;

    public ExcelReader()
    {
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    public IEnumerable<ExcelRow> Read(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Excel file not found: {filePath}");
        }
        using var pkg = new ExcelPackage(new FileInfo(filePath));
        if (!pkg.Workbook.Worksheets.Any())
        {
            throw new InvalidOperationException("The Excel file contains no worksheets.");
        }
        var sheet = pkg.Workbook.Worksheets.First();

        for (int i = StartRow; i <= sheet.Dimension.Rows; i++)
        {
            yield return new ExcelRow(
                sheet.Cells[i, NameColumn].Text?.Trim(),
                sheet.Cells[i, EmailColumn].Text?.Trim(),
                sheet.Cells[i, SalaryColumn].Text?.Trim(),
                sheet.Cells[i, MaritalStatusColumn].Text?.Trim()
            );
        }
    }
}
