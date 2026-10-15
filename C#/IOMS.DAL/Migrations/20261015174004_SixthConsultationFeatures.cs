using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IOMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class SixthConsultationFeatures : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Note",
                table: "StockMovements",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 1,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 2,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 3,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 4,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 5,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 6,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 7,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 8,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 9,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 10,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 11,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 12,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 13,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 14,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 15,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 16,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 17,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 18,
                column: "Note",
                value: null);

            migrationBuilder.UpdateData(
                table: "StockMovements",
                keyColumn: "Id",
                keyValue: 19,
                column: "Note",
                value: null);

            // Adatmigráció: a meglévő szerepkörök az új műveleteket egy rokon, már meglévő jogosultság alapján kapják meg,
            // hogy az oldal puszta megtekintése ne adjon új módosítási jogot (pl. aki csak nézi a raktárakat, ne törölhesse őket).
            // Az új Beszállítók oldalt (megtekintés) mindenki megkapja, aki a beszerzéseket eléri.
            migrationBuilder.Sql("""
                INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
                SELECT DISTINCT rc.RoleId, 'page', 'suppliers'
                FROM AspNetRoleClaims rc
                WHERE rc.ClaimType = 'page' AND rc.ClaimValue = 'supplier-orders'
                  AND NOT EXISTS (SELECT 1 FROM AspNetRoleClaims e
                                  WHERE e.RoleId = rc.RoleId AND e.ClaimType = 'page' AND e.ClaimValue = 'suppliers');

                -- Új művelet <- meglévő művelet; a raktárkezelés a termékkezelőké (raktárvezető), mert eddig ők módosították a készletet
                INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
                SELECT rc.RoleId, 'action', a.Action
                FROM AspNetRoleClaims rc
                JOIN (VALUES
                    ('products.edit', 'products.adjust-stock'),
                    ('products.edit', 'products.import'),
                    ('products.edit', 'warehouses.create'),
                    ('products.edit', 'warehouses.edit'),
                    ('products.edit', 'warehouses.delete'),
                    ('customer-orders.create', 'customer-orders.edit'),
                    ('supplier-orders.create', 'supplier-orders.edit'),
                    ('customers.create', 'customers.edit'),
                    ('customers.create', 'customers.delete'),
                    ('supplier-orders.create', 'suppliers.create'),
                    ('supplier-orders.create', 'suppliers.edit'),
                    ('supplier-orders.create', 'suppliers.delete')
                ) AS a(Source, Action) ON rc.ClaimValue = a.Source
                WHERE rc.ClaimType = 'action'
                  AND NOT EXISTS (SELECT 1 FROM AspNetRoleClaims e
                                  WHERE e.RoleId = rc.RoleId AND e.ClaimType = 'action' AND e.ClaimValue = a.Action);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DELETE FROM AspNetRoleClaims
                WHERE (ClaimType = 'page' AND ClaimValue = 'suppliers')
                   OR (ClaimType = 'action' AND ClaimValue IN (
                       'products.adjust-stock', 'products.import', 'warehouses.create', 'warehouses.edit', 'warehouses.delete',
                       'customer-orders.edit', 'supplier-orders.edit', 'customers.edit', 'customers.delete',
                       'suppliers.create', 'suppliers.edit', 'suppliers.delete'));
                """);
            migrationBuilder.DropColumn(
                name: "Note",
                table: "StockMovements");
        }
    }
}
