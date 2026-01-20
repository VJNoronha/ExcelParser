using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelParser.Migrations
{
    /// <inheritdoc />
    public partial class AddSalaryAndMaritalStatusToErrorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                table: "MainTable",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "Salary",
                table: "MainTable",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                table: "ErrorTable",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Salary",
                table: "ErrorTable",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_StagingTable_Email",
                table: "StagingTable",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_MainTable_Email",
                table: "MainTable",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_FileUploads_IsProcessed",
                table: "FileUploads",
                column: "IsProcessed");

            migrationBuilder.CreateIndex(
                name: "IX_ErrorTable_Email",
                table: "ErrorTable",
                column: "Email");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_StagingTable_Email",
                table: "StagingTable");

            migrationBuilder.DropIndex(
                name: "IX_MainTable_Email",
                table: "MainTable");

            migrationBuilder.DropIndex(
                name: "IX_FileUploads_IsProcessed",
                table: "FileUploads");

            migrationBuilder.DropIndex(
                name: "IX_ErrorTable_Email",
                table: "ErrorTable");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "MainTable");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "MainTable");

            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "ErrorTable");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "ErrorTable");
        }
    }
}
