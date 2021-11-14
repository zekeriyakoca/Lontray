using Microsoft.EntityFrameworkCore.Migrations;

namespace Ordering.API.Migrations
{
    public partial class _1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "PaymentMethods",
                newName: "PaymentMethods",
                newSchema: "ordering");

            migrationBuilder.RenameTable(
                name: "OrderStatus",
                newName: "OrderStatus",
                newSchema: "ordering");

            migrationBuilder.RenameTable(
                name: "Orders",
                newName: "Orders",
                newSchema: "ordering");

            migrationBuilder.RenameTable(
                name: "OrderItems",
                newName: "OrderItems",
                newSchema: "ordering");

            migrationBuilder.RenameTable(
                name: "CardType",
                newName: "CardType",
                newSchema: "ordering");

            migrationBuilder.RenameTable(
                name: "Buyers",
                newName: "Buyers",
                newSchema: "ordering");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "PaymentMethods",
                schema: "ordering",
                newName: "PaymentMethods");

            migrationBuilder.RenameTable(
                name: "OrderStatus",
                schema: "ordering",
                newName: "OrderStatus");

            migrationBuilder.RenameTable(
                name: "Orders",
                schema: "ordering",
                newName: "Orders");

            migrationBuilder.RenameTable(
                name: "OrderItems",
                schema: "ordering",
                newName: "OrderItems");

            migrationBuilder.RenameTable(
                name: "CardType",
                schema: "ordering",
                newName: "CardType");

            migrationBuilder.RenameTable(
                name: "Buyers",
                schema: "ordering",
                newName: "Buyers");
        }
    }
}
