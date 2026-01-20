public interface IExcelReader
{
    IEnumerable<ExcelRow> Read(string filePath);
}
