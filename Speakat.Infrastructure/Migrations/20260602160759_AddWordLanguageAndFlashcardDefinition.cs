using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Speakat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddWordLanguageAndFlashcardDefinition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "language_id",
                table: "words",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "definition",
                table: "flashcards",
                type: "text",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_words_language_id",
                table: "words",
                column: "language_id");

            migrationBuilder.AddForeignKey(
                name: "FK_words_languages_language_id",
                table: "words",
                column: "language_id",
                principalTable: "languages",
                principalColumn: "language_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_words_languages_language_id",
                table: "words");

            migrationBuilder.DropIndex(
                name: "IX_words_language_id",
                table: "words");

            migrationBuilder.DropColumn(
                name: "language_id",
                table: "words");

            migrationBuilder.DropColumn(
                name: "definition",
                table: "flashcards");
        }
    }
}
