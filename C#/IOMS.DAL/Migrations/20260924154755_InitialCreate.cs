using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace IOMS.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SKU = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    StockQuantity = table.Column<int>(type: "int", nullable: false),
                    MinStockLevel = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "Category", "MinStockLevel", "Name", "Price", "SKU", "Status", "StockQuantity" },
                values: new object[,]
                {
                    { 1, "Perifériák", 10, "Logitech MX Master 3S Egér", 39900m, "LOG-MX3S-BLK", "In Stock", 45 },
                    { 2, "Monitorok", 5, "Dell UltraSharp U2723QE Monitor", 219000m, "DEL-U2723QE", "Low Stock", 4 },
                    { 3, "Perifériák", 8, "Keychron K2 Pro Mechanikus Billentyűzet", 45900m, "KEY-K2PRO-RGB", "Out of Stock", 0 },
                    { 4, "Adattárolók", 5, "Samsung 990 Pro 2TB NVMe SSD", 68900m, "SAM-990PRO-2TB", "In Stock", 18 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
