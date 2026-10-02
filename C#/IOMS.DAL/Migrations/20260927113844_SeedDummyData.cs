using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IOMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SeedDummyData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Email", "Name", "Phone" },
                values: new object[,]
                {
                    { 1, "anna.kovacs@example.com", "Kovács Anna", "+36 30 111 2222" },
                    { 2, "peter.nagy@example.com", "Nagy Péter", "+36 20 333 4444" },
                    { 3, "beszerzes@szabo-kft.example.com", "Szabó és Társa Kft.", null }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "MinStockLevel", "Name", "Price", "SKU", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "Perifériák", 10, "Logitech MX Master 3S egér", 39990m, "PER-MX3S", 42 },
                    { 2, "Perifériák", 8, "Keychron K2 billentyűzet", 34990m, "PER-K2", 6 },
                    { 3, "Perifériák", 5, "Dell P2723D 27\" monitor", 104990m, "PER-P2723D", 0 },
                    { 4, "Tárolóeszközök", 10, "Samsung 990 Pro 1TB SSD", 49990m, "STO-990P-1T", 25 },
                    { 5, "Tárolóeszközök", 6, "WD Red Plus 4TB HDD", 44990m, "STO-WDRP-4T", 12 },
                    { 6, "Tárolóeszközök", 15, "SanDisk Ultra 128GB pendrive", 5990m, "STO-SDU-128", 3 },
                    { 7, "Hálózati eszközök", 5, "TP-Link Archer AX55 router", 32990m, "NET-AX55", 18 },
                    { 8, "Hálózati eszközök", 4, "Ubiquiti UniFi Switch Lite 8", 42990m, "NET-USL8", 9 }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Email", "Name", "Phone" },
                values: new object[,]
                {
                    { 1, "rendeles@techdistri.example.com", "TechDistri Kft.", "+36 1 555 1000" },
                    { 2, "sales@netparts.example.com", "NetParts Zrt.", null }
                });

            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "Id", "Address", "Capacity", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 1, "1117 Budapest, Példa utca 1.", 5000, 47.472999999999999, 19.053000000000001, "Budapesti központi raktár" },
                    { 2, "4031 Debrecen, Minta út 12.", 2000, 47.531599999999997, 21.627300000000002, "Debreceni raktár" }
                });

            migrationBuilder.InsertData(
                table: "CustomerOrders",
                columns: new[] { "Id", "CustomerId", "OrderDate", "Status" },
                values: new object[,]
                {
                    { 1, 1, new DateTime(2026, 9, 1, 9, 30, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 2, 2, new DateTime(2026, 9, 10, 14, 5, 0, 0, DateTimeKind.Utc), "Shipped" },
                    { 3, 3, new DateTime(2026, 9, 20, 11, 0, 0, 0, DateTimeKind.Utc), "Pending" },
                    { 4, 1, new DateTime(2026, 9, 22, 16, 45, 0, 0, DateTimeKind.Utc), "Cancelled" }
                });

            migrationBuilder.InsertData(
                table: "SupplierOrders",
                columns: new[] { "Id", "OrderDate", "Status", "SupplierId" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 9, 5, 8, 0, 0, 0, DateTimeKind.Utc), "Delivered", 1 },
                    { 2, new DateTime(2026, 9, 21, 10, 15, 0, 0, DateTimeKind.Utc), "Pending", 2 }
                });

            migrationBuilder.InsertData(
                table: "CustomerOrderItems",
                columns: new[] { "Id", "CustomerOrderId", "ProductId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 1, 1, 1, 2, 39990m },
                    { 2, 1, 4, 1, 49990m },
                    { 3, 2, 7, 1, 32990m },
                    { 4, 2, 6, 5, 5990m },
                    { 5, 3, 3, 3, 104990m },
                    { 6, 3, 2, 3, 34990m },
                    { 7, 3, 8, 2, 42990m },
                    { 8, 4, 5, 2, 44990m }
                });

            migrationBuilder.InsertData(
                table: "SupplierOrderItems",
                columns: new[] { "Id", "ProductId", "Quantity", "SupplierOrderId", "UnitCost" },
                values: new object[,]
                {
                    { 1, 1, 20, 1, 28000m },
                    { 2, 4, 10, 1, 36000m },
                    { 3, 8, 5, 2, 31000m },
                    { 4, 7, 10, 2, 23500m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "SupplierOrders",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "SupplierOrders",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
