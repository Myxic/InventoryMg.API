using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InventoryMg.DAL.Migrations
{
    public partial class AddedProductImagePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ProductImagePath",
                table: "Products",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProductImagePath",
                table: "Products");
        }
    }
}
