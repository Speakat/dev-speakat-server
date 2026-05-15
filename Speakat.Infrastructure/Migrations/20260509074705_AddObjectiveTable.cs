using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Speakat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddObjectiveTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "objectives",
                columns: table => new
                {
                    objective_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(255)", maxLength: 255, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    description = table.Column<string>(type: "text", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_objectives", x => x.objective_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "quest_objectives",
                columns: table => new
                {
                    quest_objective_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    quest_id = table.Column<long>(type: "bigint", nullable: false),
                    objective_id = table.Column<long>(type: "bigint", nullable: false),
                    sort_order = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quest_objectives", x => x.quest_objective_id);
                    table.ForeignKey(
                        name: "FK_quest_objectives_objectives_objective_id",
                        column: x => x.objective_id,
                        principalTable: "objectives",
                        principalColumn: "objective_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_quest_objectives_quests_quest_id",
                        column: x => x.quest_id,
                        principalTable: "quests",
                        principalColumn: "quest_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "session_objectives",
                columns: table => new
                {
                    session_objective_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    session_id = table.Column<string>(type: "varchar(36)", maxLength: 36, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    quest_objective_id = table.Column<long>(type: "bigint", nullable: false),
                    is_achieved = table.Column<bool>(type: "tinyint(1)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_session_objectives", x => x.session_objective_id);
                    table.ForeignKey(
                        name: "FK_session_objectives_game_sessions_session_id",
                        column: x => x.session_id,
                        principalTable: "game_sessions",
                        principalColumn: "session_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_session_objectives_quest_objectives_quest_objective_id",
                        column: x => x.quest_objective_id,
                        principalTable: "quest_objectives",
                        principalColumn: "quest_objective_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_quest_objectives_objective_id",
                table: "quest_objectives",
                column: "objective_id");

            migrationBuilder.CreateIndex(
                name: "IX_quest_objectives_quest_id",
                table: "quest_objectives",
                column: "quest_id");

            migrationBuilder.CreateIndex(
                name: "IX_session_objectives_quest_objective_id",
                table: "session_objectives",
                column: "quest_objective_id");

            migrationBuilder.CreateIndex(
                name: "IX_session_objectives_session_id",
                table: "session_objectives",
                column: "session_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "session_objectives");

            migrationBuilder.DropTable(
                name: "quest_objectives");

            migrationBuilder.DropTable(
                name: "objectives");
        }
    }
}
