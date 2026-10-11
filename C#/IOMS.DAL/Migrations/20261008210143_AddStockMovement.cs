using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IOMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddStockMovement : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "StockMovements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    WarehouseId = table.Column<int>(type: "int", nullable: false),
                    QuantityChange = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    SupplierOrderId = table.Column<int>(type: "int", nullable: true),
                    CustomerOrderId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockMovements", x => x.Id);
                    table.CheckConstraint("CK_StockMovement_QuantityChange", "[QuantityChange] <> 0");
                    table.ForeignKey(
                        name: "FK_StockMovements_CustomerOrders_CustomerOrderId",
                        column: x => x.CustomerOrderId,
                        principalTable: "CustomerOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Products_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StockMovements_SupplierOrders_SupplierOrderId",
                        column: x => x.SupplierOrderId,
                        principalTable: "SupplierOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StockMovements_Warehouses_WarehouseId",
                        column: x => x.WarehouseId,
                        principalTable: "Warehouses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "StockMovements",
                columns: new[] { "Id", "CreatedAt", "CustomerOrderId", "ProductId", "QuantityChange", "SupplierOrderId", "Type", "WarehouseId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 1, 42, null, "Opening", 1 },
                    { 2, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 2, 6, null, "Opening", 1 },
                    { 3, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 4, 25, null, "Opening", 1 },
                    { 4, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 5, 12, null, "Opening", 1 },
                    { 5, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 6, 3, null, "Opening", 1 },
                    { 6, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 7, 18, null, "Opening", 1 },
                    { 7, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 8, 9, null, "Opening", 1 },
                    { 8, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 9, 35, null, "Opening", 1 },
                    { 9, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 10, 14, null, "Opening", 1 },
                    { 10, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 11, 4, null, "Opening", 1 },
                    { 11, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 12, 40, null, "Opening", 1 },
                    { 12, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 13, 7, null, "Opening", 1 },
                    { 13, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 14, 22, null, "Opening", 1 },
                    { 14, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 15, 6, null, "Opening", 1 },
                    { 15, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 16, 50, null, "Opening", 1 },
                    { 16, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 17, 300, null, "Opening", 1 },
                    { 17, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 18, 80, null, "Opening", 1 },
                    { 18, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 19, 18, null, "Opening", 1 },
                    { 19, new DateTime(2026, 9, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, 20, 5, null, "Opening", 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_CustomerOrderId",
                table: "StockMovements",
                column: "CustomerOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_ProductId_WarehouseId_CreatedAt",
                table: "StockMovements",
                columns: new[] { "ProductId", "WarehouseId", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_SupplierOrderId",
                table: "StockMovements",
                column: "SupplierOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_StockMovements_WarehouseId",
                table: "StockMovements",
                column: "WarehouseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "StockMovements");
        }
    }
}
