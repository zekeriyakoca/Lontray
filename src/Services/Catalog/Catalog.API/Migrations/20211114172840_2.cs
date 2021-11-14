using Microsoft.EntityFrameworkCore.Migrations;

namespace Catalog.API.Migrations
{
    public partial class _2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "catalogapi");

            migrationBuilder.RenameTable(
                name: "CatalogTypes",
                newName: "CatalogTypes",
                newSchema: "catalogapi");

            migrationBuilder.RenameTable(
                name: "CatalogItems",
                newName: "CatalogItems",
                newSchema: "catalogapi");

            migrationBuilder.RenameTable(
                name: "CatalogFeature",
                newName: "CatalogFeature",
                newSchema: "catalogapi");

            migrationBuilder.RenameTable(
                name: "CatalogBrands",
                newName: "CatalogBrands",
                newSchema: "catalogapi");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "CatalogTypes",
                schema: "catalogapi",
                newName: "CatalogTypes");

            migrationBuilder.RenameTable(
                name: "CatalogItems",
                schema: "catalogapi",
                newName: "CatalogItems");

            migrationBuilder.RenameTable(
                name: "CatalogFeature",
                schema: "catalogapi",
                newName: "CatalogFeature");

            migrationBuilder.RenameTable(
                name: "CatalogBrands",
                schema: "catalogapi",
                newName: "CatalogBrands");
        }
    }
}
