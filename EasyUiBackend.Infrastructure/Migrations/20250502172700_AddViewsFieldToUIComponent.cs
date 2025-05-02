using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyUiBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddViewsFieldToUIComponent : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Views",
                table: "UIComponents",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Views",
                table: "UIComponents");
        }
    }
}
