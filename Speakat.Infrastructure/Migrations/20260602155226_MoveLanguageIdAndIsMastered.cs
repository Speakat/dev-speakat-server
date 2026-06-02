using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Speakat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MoveLanguageIdAndIsMastered : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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
                name: "is_mastered",
                table: "flashcards");

            migrationBuilder.AddColumn<bool>(
                name: "is_mastered",
                table: "user_flashcards",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "language_id",
                table: "flashcards",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_flashcards_language_id",
                table: "flashcards",
                column: "language_id");

            migrationBuilder.AddForeignKey(
                name: "FK_flashcards_languages_language_id",
                table: "flashcards",
                column: "language_id",
                principalTable: "languages",
                principalColumn: "language_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_flashcards_languages_language_id",
                table: "flashcards");

            migrationBuilder.DropIndex(
                name: "IX_flashcards_language_id",
                table: "flashcards");

            migrationBuilder.DropColumn(
                name: "is_mastered",
                table: "user_flashcards");

            migrationBuilder.DropColumn(
                name: "language_id",
                table: "flashcards");

            migrationBuilder.AddColumn<long>(
                name: "language_id",
                table: "words",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<bool>(
                name: "is_mastered",
                table: "flashcards",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

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
    }
}
