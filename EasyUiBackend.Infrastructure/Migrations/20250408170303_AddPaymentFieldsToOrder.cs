using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EasyUiBackend.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentFieldsToOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentOrderId",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentProvider",
                table: "Orders",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentRequestId",
                table: "Orders",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentOrderId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentProvider",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "PaymentRequestId",
                table: "Orders");
        }
    }
}
