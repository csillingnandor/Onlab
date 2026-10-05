using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IOMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class ExtendSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "CustomerOrders",
                columns: new[] { "Id", "CustomerId", "OrderDate", "Status" },
                values: new object[,]
                {
                    { 5, 1, new DateTime(2026, 8, 1, 17, 45, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 7, 2, new DateTime(2026, 8, 4, 11, 30, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 10, 1, new DateTime(2026, 8, 8, 14, 30, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 13, 2, new DateTime(2026, 8, 12, 11, 15, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 18, 1, new DateTime(2026, 8, 19, 10, 0, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 19, 2, new DateTime(2026, 8, 20, 10, 30, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 38, 2, new DateTime(2026, 9, 16, 8, 45, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 39, 3, new DateTime(2026, 9, 17, 17, 15, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 43, 1, new DateTime(2026, 9, 23, 11, 15, 0, 0, DateTimeKind.Utc), "Cancelled" },
                    { 47, 1, new DateTime(2026, 9, 28, 9, 45, 0, 0, DateTimeKind.Utc), "Delivered" }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Email", "Name", "Phone" },
                values: new object[,]
                {
                    { 4, "gabor.toth@example.com", "Tóth Gábor", "+36 70 555 0101" },
                    { 5, "eszter.horvath@example.com", "Horváth Eszter", "+36 30 222 3344" },
                    { 6, "bence.varga@example.com", "Varga Bence", null },
                    { 7, "iroda@kissesfia.example.com", "Kiss és Fia Bt.", "+36 1 444 2020" },
                    { 8, "reka.molnar@example.com", "Molnár Réka", "+36 20 987 6543" },
                    { 9, "laszlo.nemeth@example.com", "Németh László", null },
                    { 10, "beszerzes@digitalpro.example.com", "DigitalPro Zrt.", "+36 1 777 8899" },
                    { 11, "dora.farkas@example.com", "Farkas Dóra", "+36 30 765 4321" },
                    { 12, "info@balogh-it.example.com", "Balogh Informatika Kft.", "+36 62 333 1100" }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "MinStockLevel", "Name", "Price", "SKU", "StockQuantity" },
                values: new object[,]
                {
                    { 9, "Perifériák", 10, "Logitech K380 billentyűzet", 14990m, "PER-K380", 35 },
                    { 10, "Perifériák", 6, "Razer DeathAdder V3 egér", 29990m, "PER-DAV3", 14 },
                    { 11, "Perifériák", 3, "LG 27GP850 27\" monitor", 159990m, "PER-27GP850", 4 },
                    { 12, "Tárolóeszközök", 15, "Kingston NV2 1TB SSD", 24990m, "STO-NV2-1T", 40 },
                    { 13, "Tárolóeszközök", 4, "Seagate IronWolf 8TB HDD", 89990m, "STO-IW-8T", 7 },
                    { 14, "Hálózati eszközök", 8, "TP-Link TL-SG108 switch", 9990m, "NET-SG108", 22 },
                    { 15, "Hálózati eszközök", 5, "Ubiquiti U6 Lite access point", 44990m, "NET-U6L", 6 },
                    { 16, "Kábelek és adapterek", 20, "UGREEN USB-C hub 7 az 1-ben", 12990m, "KAB-UGH7", 50 },
                    { 17, "Kábelek és adapterek", 100, "Cat6 patchkábel 2 m", 990m, "KAB-CAT6-2", 300 },
                    { 18, "Kábelek és adapterek", 30, "HDMI 2.1 kábel 2 m", 3990m, "KAB-HDMI21", 80 },
                    { 19, "Számítógép-alkatrészek", 6, "Corsair Vengeance 32GB DDR5", 39990m, "ALK-CV32D5", 18 },
                    { 20, "Számítógép-alkatrészek", 3, "AMD Ryzen 7 7700X processzor", 109990m, "ALK-R77700X", 5 }
                });

            migrationBuilder.InsertData(
                table: "SupplierOrders",
                columns: new[] { "Id", "OrderDate", "Status", "SupplierId" },
                values: new object[,]
                {
                    { 5, new DateTime(2026, 8, 20, 10, 15, 0, 0, DateTimeKind.Utc), "Delivered", 1 },
                    { 6, new DateTime(2026, 9, 2, 8, 45, 0, 0, DateTimeKind.Utc), "Delivered", 2 },
                    { 7, new DateTime(2026, 9, 29, 11, 0, 0, 0, DateTimeKind.Utc), "Shipped", 1 }
                });

            migrationBuilder.InsertData(
                table: "Suppliers",
                columns: new[] { "Id", "Email", "Name", "Phone" },
                values: new object[,]
                {
                    { 3, "rendeles@kabelvilag.example.com", "Kábelvilág Kft.", "+36 1 600 2500" },
                    { 4, "info@alkatreszpont.example.com", "AlkatrészPont Bt.", null }
                });

            migrationBuilder.InsertData(
                table: "Warehouses",
                columns: new[] { "Id", "Address", "Capacity", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 3, "9027 Győr, Ipari park 5.", 3000, 47.6875, 17.650400000000001, "Győri raktár" },
                    { 4, "6728 Szeged, Logisztikai út 3.", 1500, 46.253, 20.141400000000001, "Szegedi raktár" },
                    { 5, "7630 Pécs, Raktár utca 9.", 1200, 46.072699999999998, 18.232299999999999, "Pécsi raktár" }
                });

            migrationBuilder.InsertData(
                table: "CustomerOrderItems",
                columns: new[] { "Id", "CustomerOrderId", "ProductId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 9, 5, 11, 3, 159990m },
                    { 10, 5, 14, 2, 9990m },
                    { 11, 5, 8, 1, 42990m },
                    { 15, 7, 17, 12, 990m },
                    { 16, 7, 12, 1, 24990m },
                    { 17, 7, 10, 3, 29990m },
                    { 23, 10, 5, 3, 44990m },
                    { 29, 13, 5, 2, 44990m },
                    { 30, 13, 20, 2, 109990m },
                    { 40, 18, 16, 2, 12990m },
                    { 41, 18, 20, 2, 109990m },
                    { 42, 19, 10, 3, 29990m },
                    { 43, 19, 11, 3, 159990m },
                    { 44, 19, 9, 3, 14990m },
                    { 80, 38, 4, 3, 49990m },
                    { 81, 38, 18, 11, 3990m },
                    { 82, 38, 20, 3, 109990m },
                    { 83, 39, 4, 3, 49990m },
                    { 88, 43, 18, 15, 3990m },
                    { 89, 43, 5, 3, 44990m },
                    { 99, 47, 14, 3, 9990m },
                    { 100, 47, 6, 3, 5990m },
                    { 101, 47, 12, 9, 24990m },
                    { 102, 47, 10, 3, 29990m },
                    { 103, 47, 1, 4, 39990m }
                });

            migrationBuilder.InsertData(
                table: "CustomerOrders",
                columns: new[] { "Id", "CustomerId", "OrderDate", "Status" },
                values: new object[,]
                {
                    { 6, 5, new DateTime(2026, 8, 2, 14, 15, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 8, 7, new DateTime(2026, 8, 5, 13, 0, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 9, 5, new DateTime(2026, 8, 7, 9, 0, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 11, 4, new DateTime(2026, 8, 9, 17, 15, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 12, 9, new DateTime(2026, 8, 11, 8, 15, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 14, 12, new DateTime(2026, 8, 14, 15, 15, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 15, 5, new DateTime(2026, 8, 15, 11, 15, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 16, 12, new DateTime(2026, 8, 16, 8, 30, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 17, 4, new DateTime(2026, 8, 17, 17, 30, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 20, 5, new DateTime(2026, 8, 22, 16, 30, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 21, 9, new DateTime(2026, 8, 24, 10, 15, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 22, 5, new DateTime(2026, 8, 25, 11, 30, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 23, 8, new DateTime(2026, 8, 26, 13, 0, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 24, 10, new DateTime(2026, 8, 28, 8, 15, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 25, 10, new DateTime(2026, 8, 28, 18, 15, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 26, 10, new DateTime(2026, 8, 30, 10, 30, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 27, 11, new DateTime(2026, 9, 1, 13, 45, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 28, 12, new DateTime(2026, 9, 2, 13, 15, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 29, 11, new DateTime(2026, 9, 3, 14, 15, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 30, 4, new DateTime(2026, 9, 4, 16, 30, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 31, 7, new DateTime(2026, 9, 6, 9, 15, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 32, 5, new DateTime(2026, 9, 7, 16, 0, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 33, 11, new DateTime(2026, 9, 9, 12, 0, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 34, 5, new DateTime(2026, 9, 10, 12, 0, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 35, 8, new DateTime(2026, 9, 12, 13, 15, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 36, 7, new DateTime(2026, 9, 13, 16, 0, 0, 0, DateTimeKind.Utc), "Cancelled" },
                    { 37, 12, new DateTime(2026, 9, 15, 9, 0, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 40, 5, new DateTime(2026, 9, 19, 11, 30, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 41, 8, new DateTime(2026, 9, 19, 14, 0, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 42, 4, new DateTime(2026, 9, 21, 12, 0, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 44, 12, new DateTime(2026, 9, 25, 8, 0, 0, 0, DateTimeKind.Utc), "Shipped" },
                    { 45, 9, new DateTime(2026, 9, 25, 15, 45, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 46, 4, new DateTime(2026, 9, 27, 14, 15, 0, 0, DateTimeKind.Utc), "Delivered" },
                    { 48, 6, new DateTime(2026, 9, 30, 8, 0, 0, 0, DateTimeKind.Utc), "Shipped" },
                    { 49, 11, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Pending" },
                    { 50, 9, new DateTime(2026, 10, 2, 18, 30, 0, 0, DateTimeKind.Utc), "Pending" }
                });

            migrationBuilder.InsertData(
                table: "SupplierOrderItems",
                columns: new[] { "Id", "ProductId", "Quantity", "SupplierOrderId", "UnitCost" },
                values: new object[,]
                {
                    { 10, 9, 40, 5, 9500m },
                    { 11, 10, 20, 5, 19500m },
                    { 12, 11, 6, 5, 115000m },
                    { 13, 14, 30, 6, 6200m },
                    { 14, 15, 10, 6, 31000m },
                    { 15, 12, 60, 7, 16500m },
                    { 16, 3, 10, 7, 76000m }
                });

            migrationBuilder.InsertData(
                table: "SupplierOrders",
                columns: new[] { "Id", "OrderDate", "Status", "SupplierId" },
                values: new object[,]
                {
                    { 3, new DateTime(2026, 8, 5, 9, 0, 0, 0, DateTimeKind.Utc), "Delivered", 3 },
                    { 4, new DateTime(2026, 8, 12, 13, 30, 0, 0, DateTimeKind.Utc), "Delivered", 4 },
                    { 8, new DateTime(2026, 10, 2, 14, 0, 0, 0, DateTimeKind.Utc), "Pending", 4 }
                });

            migrationBuilder.InsertData(
                table: "CustomerOrderItems",
                columns: new[] { "Id", "CustomerOrderId", "ProductId", "Quantity", "UnitPrice" },
                values: new object[,]
                {
                    { 12, 6, 2, 3, 34990m },
                    { 13, 6, 11, 2, 159990m },
                    { 14, 6, 14, 2, 9990m },
                    { 18, 8, 8, 2, 42990m },
                    { 19, 8, 5, 1, 44990m },
                    { 20, 8, 7, 2, 32990m },
                    { 21, 9, 2, 2, 34990m },
                    { 22, 9, 20, 1, 109990m },
                    { 24, 11, 6, 1, 5990m },
                    { 25, 11, 8, 1, 42990m },
                    { 26, 12, 5, 2, 44990m },
                    { 27, 12, 13, 2, 89990m },
                    { 28, 12, 6, 1, 5990m },
                    { 31, 14, 10, 3, 29990m },
                    { 32, 14, 6, 3, 5990m },
                    { 33, 15, 14, 2, 9990m },
                    { 34, 15, 11, 1, 159990m },
                    { 35, 15, 15, 2, 44990m },
                    { 36, 16, 15, 2, 44990m },
                    { 37, 16, 19, 3, 39990m },
                    { 38, 16, 16, 3, 12990m },
                    { 39, 17, 7, 2, 32990m },
                    { 45, 20, 15, 2, 44990m },
                    { 46, 20, 18, 14, 3990m },
                    { 47, 20, 17, 9, 990m },
                    { 48, 21, 2, 1, 34990m },
                    { 49, 21, 18, 19, 3990m },
                    { 50, 21, 10, 2, 29990m },
                    { 51, 22, 1, 1, 39990m },
                    { 52, 22, 12, 1, 24990m },
                    { 53, 23, 14, 3, 9990m },
                    { 54, 23, 1, 1, 39990m },
                    { 55, 23, 6, 1, 5990m },
                    { 56, 24, 19, 1, 39990m },
                    { 57, 25, 18, 13, 3990m },
                    { 58, 25, 1, 2, 39990m },
                    { 59, 26, 11, 3, 159990m },
                    { 60, 26, 19, 3, 39990m },
                    { 61, 27, 1, 2, 39990m },
                    { 62, 28, 8, 3, 42990m },
                    { 63, 29, 7, 3, 32990m },
                    { 64, 30, 7, 3, 32990m },
                    { 65, 31, 8, 1, 42990m },
                    { 66, 31, 6, 1, 5990m },
                    { 67, 32, 13, 1, 89990m },
                    { 68, 32, 15, 3, 44990m },
                    { 69, 32, 11, 2, 159990m },
                    { 70, 33, 18, 11, 3990m },
                    { 71, 33, 16, 3, 12990m },
                    { 72, 33, 4, 1, 49990m },
                    { 73, 34, 1, 3, 39990m },
                    { 74, 35, 14, 1, 9990m },
                    { 75, 36, 6, 2, 5990m },
                    { 76, 36, 5, 1, 44990m },
                    { 77, 37, 11, 2, 159990m },
                    { 78, 37, 17, 18, 990m },
                    { 79, 37, 15, 1, 44990m },
                    { 84, 40, 2, 2, 34990m },
                    { 85, 40, 7, 1, 32990m },
                    { 86, 41, 8, 3, 42990m },
                    { 87, 42, 4, 2, 49990m },
                    { 90, 44, 15, 2, 44990m },
                    { 91, 44, 8, 1, 42990m },
                    { 92, 44, 2, 1, 34990m },
                    { 93, 45, 11, 3, 159990m },
                    { 94, 45, 20, 1, 109990m },
                    { 95, 46, 9, 2, 14990m },
                    { 96, 46, 12, 6, 24990m },
                    { 97, 46, 10, 5, 29990m },
                    { 98, 46, 1, 3, 39990m },
                    { 104, 48, 9, 2, 14990m },
                    { 105, 48, 12, 12, 24990m },
                    { 106, 48, 10, 4, 29990m },
                    { 107, 48, 1, 2, 39990m },
                    { 108, 49, 1, 6, 39990m },
                    { 109, 49, 2, 3, 34990m },
                    { 110, 49, 12, 8, 24990m },
                    { 111, 50, 6, 3, 5990m },
                    { 112, 50, 12, 10, 24990m },
                    { 113, 50, 10, 4, 29990m }
                });

            migrationBuilder.InsertData(
                table: "SupplierOrderItems",
                columns: new[] { "Id", "ProductId", "Quantity", "SupplierOrderId", "UnitCost" },
                values: new object[,]
                {
                    { 5, 17, 500, 3, 450m },
                    { 6, 18, 100, 3, 1900m },
                    { 7, 16, 40, 3, 7900m },
                    { 8, 19, 20, 4, 27500m },
                    { 9, 20, 8, 4, 79000m },
                    { 17, 19, 10, 8, 27500m },
                    { 18, 13, 6, 8, 64000m }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 56);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 57);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 58);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 59);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 60);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 61);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 62);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 63);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 64);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 65);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 66);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 67);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 68);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 69);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 70);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 71);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 72);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 73);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 74);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 75);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 76);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 77);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 78);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 79);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 80);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 81);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 82);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 83);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 84);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 85);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 86);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 87);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 88);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 89);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 90);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 91);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 92);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 93);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 94);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 95);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 96);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 97);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 98);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 99);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 100);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 101);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 102);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 103);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 104);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 105);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 106);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 107);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 108);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 109);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 110);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 111);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 112);

            migrationBuilder.DeleteData(
                table: "CustomerOrderItems",
                keyColumn: "Id",
                keyValue: 113);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "SupplierOrderItems",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Warehouses",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "SupplierOrders",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "SupplierOrders",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "SupplierOrders",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "SupplierOrders",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "SupplierOrders",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "SupplierOrders",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Suppliers",
                keyColumn: "Id",
                keyValue: 4);
        }
    }
}
