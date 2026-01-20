namespace ExcelParser.Models.DTOs;

public record SummaryDto(
    int TotalUploads,
    int ProcessedUploads,
    int TotalValidRecords,
    int TotalInvalidRecords
);