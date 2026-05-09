using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Speakat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUserUuidToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "user_uuid",
                table: "users",
                type: "varchar(36)",
                maxLength: 36,
                nullable: false,
                defaultValue: "")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "streak_goal",
                table: "user_settings",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "uq_users_user_uuid",
                table: "users",
                column: "user_uuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "uq_users_user_uuid",
                table: "users");

            migrationBuilder.DropColumn(
                name: "user_uuid",
                table: "users");

            migrationBuilder.DropColumn(
                name: "streak_goal",
                table: "user_settings");
        }
    }
}
