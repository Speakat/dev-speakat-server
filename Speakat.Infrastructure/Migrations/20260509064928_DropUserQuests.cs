using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Speakat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class DropUserQuests : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_quests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_quests",
                columns: table => new
                {
                    user_quest_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    quest_id = table.Column<long>(type: "bigint", nullable: false),
                    user_id = table.Column<long>(type: "bigint", nullable: false),
                    completed_at = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    started_at = table.Column<DateTime>(type: "datetime(6)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_quests", x => x.user_quest_id);
                    table.ForeignKey(
                        name: "FK_user_quests_quests_quest_id",
                        column: x => x.quest_id,
                        principalTable: "quests",
                        principalColumn: "quest_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_user_quests_users_user_id",
                        column: x => x.user_id,
                        principalTable: "users",
                        principalColumn: "user_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_user_quests_quest_id",
                table: "user_quests",
                column: "quest_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_quests_user_id_quest_id",
                table: "user_quests",
                columns: new[] { "user_id", "quest_id" });
        }
    }
}
