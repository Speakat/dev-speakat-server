using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Speakat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFlashcardTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "words",
                columns: table => new
                {
                    word_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    language_id = table.Column<long>(type: "bigint", nullable: false),
                    word = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    definition = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    phonetic = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    audio_url = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_words", x => x.word_id);
                    table.ForeignKey(
                        name: "FK_words_languages_language_id",
                        column: x => x.language_id,
                        principalTable: "languages",
                        principalColumn: "language_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "flashcards",
                columns: table => new
                {
                    flashcard_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    word_id = table.Column<long>(type: "bigint", nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    is_mastered = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_flashcards", x => x.flashcard_id);
                    table.ForeignKey(
                        name: "FK_flashcards_words_word_id",
                        column: x => x.word_id,
                        principalTable: "words",
                        principalColumn: "word_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "user_flashcards",
                columns: table => new
                {
                    user_flashcard_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    flashcard_id = table.Column<long>(type: "bigint", nullable: false),
                    quest_id = table.Column<long>(type: "bigint", nullable: false),
                    recommendation_reason = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    saved_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_flashcards", x => x.user_flashcard_id);
                    table.ForeignKey(
                        name: "FK_user_flashcards_flashcards_flashcard_id",
                        column: x => x.flashcard_id,
                        principalTable: "flashcards",
                        principalColumn: "flashcard_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_flashcards_quests_quest_id",
                        column: x => x.quest_id,
                        principalTable: "quests",
                        principalColumn: "quest_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_flashcards_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_flashcards_word_id",
                table: "flashcards",
                column: "word_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_flashcards_flashcard_id",
                table: "user_flashcards",
                column: "flashcard_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_flashcards_quest_id",
                table: "user_flashcards",
                column: "quest_id");

            migrationBuilder.CreateIndex(
                name: "IX_user_flashcards_user_id",
                table: "user_flashcards",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_words_language_id",
                table: "words",
                column: "language_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_flashcards");

            migrationBuilder.DropTable(
                name: "flashcards");

            migrationBuilder.DropTable(
                name: "words");
        }
    }
}
