
using ExcelParser.Data;
using ExcelParser.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<ExcelDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHostedService<ExcelBackgroundService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ExcelDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        // Use migrations in production; apply pending migrations at startup for dev convenience
        db.Database.Migrate();
        logger.LogInformation("Database migrated/applied successfully.");
    }
    catch (Exception ex)
    {
        logger.LogError(ex, "Database migration failed.");
        throw;
    }
}

app.UseStaticFiles();

app.MapGet("/", () => Results.File(Path.Combine(app.Environment.ContentRootPath, "wwwroot", "index.html"), "text/html"));
app.MapGet("/dashboard", () => Results.File(Path.Combine(app.Environment.ContentRootPath, "wwwroot", "dashboard.html"), "text/html"));

app.MapControllers();
app.Run();
