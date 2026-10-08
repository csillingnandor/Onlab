using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IOMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddInventory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inventories",
                columns: table => new
                {
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventories", x => new { x.ProductId, x.WarehouseId });
                    table.CheckConstraint("CK_Inventory_Quantity", "[Quantity] >= 0");
                    table.ForeignKey(
                        name: "FK_Inventories_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Inventories_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Inventories",
                columns: new[] { "ProductId", "WarehouseId", "Quantity" },
                values: new object[,]
                {
                    { 1, 1, 42 },
                    { 2, 1, 6 },
                    { 4, 1, 25 },
                    { 5, 1, 12 },
                    { 6, 1, 3 },
                    { 7, 1, 18 },
                    { 8, 1, 9 },
                    { 9, 1, 35 },
                    { 10, 1, 14 },
                    { 11, 1, 4 },
                    { 12, 1, 40 },
                    { 13, 1, 7 },
                    { 14, 1, 22 },
                    { 15, 1, 6 },
                    { 16, 1, 50 },
                    { 17, 1, 300 },
                    { 18, 1, 80 },
                    { 19, 1, 18 },
                    { 20, 1, 5 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Inventories_WarehouseId",
                table: "Inventories",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inventories");
        }
    }
}
