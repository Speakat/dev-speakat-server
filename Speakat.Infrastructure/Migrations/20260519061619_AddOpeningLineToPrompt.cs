using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Speakat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddOpeningLineToPrompt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "opening_line",
                table: "prompts",
                type: "text",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "voice",
                table: "npcs",
                type: "varchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "alloy")
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "opening_line",
                table: "prompts");

            migrationBuilder.DropColumn(
                name: "voice",
                table: "npcs");
        }
    }
}
