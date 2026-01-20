using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ExcelParser.Migrations
{
    /// <inheritdoc />
    public partial class AddSalaryAndMaritalStatusToStaging : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "MaritalStatus",
                table: "StagingTable",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Salary",
                table: "StagingTable",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaritalStatus",
                table: "StagingTable");

            migrationBuilder.DropColumn(
                name: "Salary",
                table: "StagingTable");
        }
    }
}
