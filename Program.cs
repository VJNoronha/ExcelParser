using ExcelParser.Data;
using ExcelParser.Services;
using ExcelParser.Services.Upload;
using ExcelParser.Services.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Database
builder.Services.AddDbContext<ExcelDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Services
builder.Services.AddScoped<IUploadProcessingService, UploadProcessingService>();
builder.Services.AddScoped<IExcelReader, ExcelReader>();
builder.Services.AddScoped<IRowValidator, RowValidator>();
builder.Services.AddScoped<IRecordWriter, RecordWriter>();
builder.Services.AddScoped<IRecordReadService, RecordReadService>();
builder.Services.AddScoped<IUploadService, UploadService>();

builder.Services.AddHostedService<ExcelBackgroundService>();

// Controllers
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy =
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });

// App Pipeline
var app = builder.Build();

app.UseCors(p => p.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader());

app.MapControllers();

app.Run();
