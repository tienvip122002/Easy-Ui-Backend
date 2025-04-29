using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyUiBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddComponentLikes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LikesCount",
                table: "UIComponents",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "ComponentLikes",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    UIComponentId = table.Column<Guid>(type: "uuid", nullable: false),
                    LikedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ComponentLikes", x => new { x.UserId, x.UIComponentId });
                    table.ForeignKey(
                        name: "FK_ComponentLikes_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ComponentLikes_UIComponents_UIComponentId",
                        column: x => x.UIComponentId,
                        principalTable: "UIComponents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ComponentLikes_UIComponentId",
                table: "ComponentLikes",
                column: "UIComponentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ComponentLikes");

            migrationBuilder.DropColumn(
                name: "LikesCount",
                table: "UIComponents");
        }
    }
}
