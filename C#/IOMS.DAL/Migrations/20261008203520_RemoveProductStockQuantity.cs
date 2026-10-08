using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace IOMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RemoveProductStockQuantity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Az oszlop törlése előtt a jelenlegi készlet átkerül az 1-es (budapesti központi) raktár készletsoraiba.
            // Az AddInventory óta az Inventories táblát csak a seed töltötte, ezért az 1-es raktár sorai a termékenkénti
            // StockQuantity-t kapják meg; így a seed óta módosított és a kézzel felvett termékek készlete sem vész el.
            migrationBuilder.Sql("""
                UPDATE i SET i.Quantity = p.StockQuantity
                FROM Inventories i
                JOIN Products p ON p.Id = i.ProductId
                WHERE i.WarehouseId = 1 AND p.StockQuantity > 0;

                DELETE i
                FROM Inventories i
                JOIN Products p ON p.Id = i.ProductId
                WHERE i.WarehouseId = 1 AND p.StockQuantity = 0;

                INSERT INTO Inventories (ProductId, WarehouseId, Quantity)
                SELECT p.Id, 1, p.StockQuantity
                FROM Products p
                WHERE p.StockQuantity > 0
                  AND NOT EXISTS (SELECT 1 FROM Inventories i WHERE i.ProductId = p.Id AND i.WarehouseId = 1);
                """);

            migrationBuilder.DropColumn(
                name: "StockQuantity",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockQuantity",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // A visszaállított oszlop a raktáronkénti készletek összegét kapja (nem a seed értékeket),
            // így a migráció óta történt készletváltozások sem vesznek el.
            migrationBuilder.Sql("""
                UPDATE p SET p.StockQuantity = ISNULL((SELECT SUM(i.Quantity) FROM Inventories i WHERE i.ProductId = p.Id), 0)
                FROM Products p;
                """);
        }
    }
}
