
using Microsoft.EntityFrameworkCore;
using ExcelParser.Models;

namespace ExcelParser.Data;
public class ExcelDbContext : DbContext
{
    public ExcelDbContext(DbContextOptions<ExcelDbContext> options) : base(options) { }
    public DbSet<StagingRecord> StagingTable => Set<StagingRecord>();
    public DbSet<MainRecord> MainTable => Set<MainRecord>();
    public DbSet<ErrorRecord> ErrorTable => Set<ErrorRecord>();
    public DbSet<FileUpload> FileUploads => Set<FileUpload>();
}
