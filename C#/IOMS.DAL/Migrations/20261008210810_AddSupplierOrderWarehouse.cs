using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IOMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierOrderWarehouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "SupplierOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "SupplierOrders",
                keyColumn: "Id",
                keyValue: 1,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "SupplierOrders",
                keyColumn: "Id",
                keyValue: 2,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "SupplierOrders",
                keyColumn: "Id",
                keyValue: 3,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "SupplierOrders",
                keyColumn: "Id",
                keyValue: 4,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "SupplierOrders",
                keyColumn: "Id",
                keyValue: 5,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "SupplierOrders",
                keyColumn: "Id",
                keyValue: 6,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "SupplierOrders",
                keyColumn: "Id",
                keyValue: 7,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "SupplierOrders",
                keyColumn: "Id",
                keyValue: 8,
                column: "WarehouseId",
                value: 1);

            // A seed rendeléseket a fenti UpdateData állítja; a később felvett rendelések 0-n maradnának,
            // ami a külső kulcs létrehozásakor hibát adna. Ezek is az 1-es (budapesti központi) raktárba kerülnek.
            migrationBuilder.Sql("UPDATE SupplierOrders SET WarehouseId = 1 WHERE WarehouseId = 0;");

            migrationBuilder.CreateIndex(
                name: "IX_SupplierOrders_WarehouseId",
                table: "SupplierOrders",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupplierOrders_Warehouses_WarehouseId",
                table: "SupplierOrders",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupplierOrders_Warehouses_WarehouseId",
                table: "SupplierOrders");

            migrationBuilder.DropIndex(
                name: "IX_SupplierOrders_WarehouseId",
                table: "SupplierOrders");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "SupplierOrders");
        }
    }
}
