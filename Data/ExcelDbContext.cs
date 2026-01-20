
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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Indexes for better performance
        modelBuilder.Entity<FileUpload>()
            .HasIndex(f => f.IsProcessed);

        modelBuilder.Entity<MainRecord>()
            .HasIndex(m => m.Email);

        modelBuilder.Entity<ErrorRecord>()
            .HasIndex(e => e.Email);

        modelBuilder.Entity<StagingRecord>()
            .HasIndex(s => s.Email);
    }
}
