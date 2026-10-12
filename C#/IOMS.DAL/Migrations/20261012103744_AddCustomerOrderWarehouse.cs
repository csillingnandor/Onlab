using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IOMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerOrderWarehouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "WarehouseId",
                table: "CustomerOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 1,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 2,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 3,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 4,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 5,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 6,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 7,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 8,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 9,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 10,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 11,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 12,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 13,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 14,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 15,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 16,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 17,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 18,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 19,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 20,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 21,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 22,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 23,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 24,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 25,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 26,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 27,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 28,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 29,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 30,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 31,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 32,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 33,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 34,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 35,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 36,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 37,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 38,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 39,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 40,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 41,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 42,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 43,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 44,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 45,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 46,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 47,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 48,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 49,
                column: "WarehouseId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "CustomerOrders",
                keyColumn: "Id",
                keyValue: 50,
                column: "WarehouseId",
                value: 1);

            // A seed rendeléseket a fenti UpdateData állítja; a később felvett rendelések 0-n maradnának,
            // ami a külső kulcs létrehozásakor hibát adna. Ezek is az 1-es (budapesti központi) raktárhoz kerülnek.
            migrationBuilder.Sql("UPDATE CustomerOrders SET WarehouseId = 1 WHERE WarehouseId = 0;");

            migrationBuilder.CreateIndex(
                name: "IX_CustomerOrders_WarehouseId",
                table: "CustomerOrders",
                column: "WarehouseId");

            migrationBuilder.AddForeignKey(
                name: "FK_CustomerOrders_Warehouses_WarehouseId",
                table: "CustomerOrders",
                column: "WarehouseId",
                principalTable: "Warehouses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CustomerOrders_Warehouses_WarehouseId",
                table: "CustomerOrders");

            migrationBuilder.DropIndex(
                name: "IX_CustomerOrders_WarehouseId",
                table: "CustomerOrders");

            migrationBuilder.DropColumn(
                name: "WarehouseId",
                table: "CustomerOrders");
        }
    }
}
