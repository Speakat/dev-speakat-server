using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Speakat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNpcTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_quests_Prompt_prompt_id",
                table: "quests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Prompt",
                table: "Prompt");

            migrationBuilder.RenameTable(
                name: "Prompt",
                newName: "prompts");

            migrationBuilder.RenameColumn(
                name: "Scenario",
                table: "prompts",
                newName: "scenario");

            migrationBuilder.RenameColumn(
                name: "SuccessCriteria",
                table: "prompts",
                newName: "success_criteria");

            migrationBuilder.RenameColumn(
                name: "PromptId",
                table: "prompts",
                newName: "prompt_id");

            migrationBuilder.AlterColumn<string>(
                name: "scenario",
                table: "prompts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "success_criteria",
                table: "prompts",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "longtext")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_prompts",
                table: "prompts",
                column: "prompt_id");

            migrationBuilder.CreateTable(
                name: "npcs",
                columns: table => new
                {
                    npc_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    image_url = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    tone = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_npcs", x => x.npc_id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "quest_npcs",
                columns: table => new
                {
                    quest_npc_id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    quest_id = table.Column<long>(type: "bigint", nullable: false),
                    npc_id = table.Column<long>(type: "bigint", nullable: false),
                    role = table.Column<string>(type: "text", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_quest_npcs", x => x.quest_npc_id);
                    table.ForeignKey(
                        name: "FK_quest_npcs_npcs_npc_id",
                        column: x => x.npc_id,
                        principalTable: "npcs",
                        principalColumn: "npc_id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_quest_npcs_quests_quest_id",
                        column: x => x.quest_id,
                        principalTable: "quests",
                        principalColumn: "quest_id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_quest_npcs_npc_id",
                table: "quest_npcs",
                column: "npc_id");

            migrationBuilder.CreateIndex(
                name: "IX_quest_npcs_quest_id",
                table: "quest_npcs",
                column: "quest_id");

            migrationBuilder.AddForeignKey(
                name: "FK_quests_prompts_prompt_id",
                table: "quests",
                column: "prompt_id",
                principalTable: "prompts",
                principalColumn: "prompt_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_quests_prompts_prompt_id",
                table: "quests");

            migrationBuilder.DropTable(
                name: "quest_npcs");

            migrationBuilder.DropTable(
                name: "npcs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_prompts",
                table: "prompts");

            migrationBuilder.RenameTable(
                name: "prompts",
                newName: "Prompt");

            migrationBuilder.RenameColumn(
                name: "scenario",
                table: "Prompt",
                newName: "Scenario");

            migrationBuilder.RenameColumn(
                name: "success_criteria",
                table: "Prompt",
                newName: "SuccessCriteria");

            migrationBuilder.RenameColumn(
                name: "prompt_id",
                table: "Prompt",
                newName: "PromptId");

            migrationBuilder.AlterColumn<string>(
                name: "Scenario",
                table: "Prompt",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "SuccessCriteria",
                table: "Prompt",
                type: "longtext",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Prompt",
                table: "Prompt",
                column: "PromptId");

            migrationBuilder.AddForeignKey(
                name: "FK_quests_Prompt_prompt_id",
                table: "quests",
                column: "prompt_id",
                principalTable: "Prompt",
                principalColumn: "PromptId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
