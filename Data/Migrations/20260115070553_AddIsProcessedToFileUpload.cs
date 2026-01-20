using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelParser.Migrations
{
    /// <inheritdoc />
    public partial class AddIsProcessedToFileUpload : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FileUploadId",
                table: "MainTable",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsProcessed",
                table: "FileUploads",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "ProcessedAt",
                table: "FileUploads",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FileUploadId",
                table: "ErrorTable",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FileUploadId",
                table: "MainTable");

            migrationBuilder.DropColumn(
                name: "IsProcessed",
                table: "FileUploads");

            migrationBuilder.DropColumn(
                name: "ProcessedAt",
                table: "FileUploads");

            migrationBuilder.DropColumn(
                name: "FileUploadId",
                table: "ErrorTable");
        }
    }
}
