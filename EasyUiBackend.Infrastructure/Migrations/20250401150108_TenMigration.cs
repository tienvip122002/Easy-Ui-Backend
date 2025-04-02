using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyUiBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TenMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Code",
                table: "UIComponents");

            migrationBuilder.AddColumn<string>(
                name: "Css",
                table: "UIComponents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Html",
                table: "UIComponents",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Js",
                table: "UIComponents",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Css",
                table: "UIComponents");

            migrationBuilder.DropColumn(
                name: "Html",
                table: "UIComponents");

            migrationBuilder.DropColumn(
                name: "Js",
                table: "UIComponents");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "UIComponents",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
