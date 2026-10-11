using IOMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOMS.DAL.Configurations;

internal class StockMovementConfiguration : IEntityTypeConfiguration<StockMovement>
{
    public void Configure(EntityTypeBuilder<StockMovement> builder)
    {
        // Típus szövegként tárolva ("Receipt"), hogy az adatbázisban is olvasható legyen
        builder.Property(m => m.Type).HasConversion<string>().HasMaxLength(20);

        // A 0 darabos mozgásnak nincs értelme
        builder.ToTable(t => t.HasCheckConstraint("CK_StockMovement_QuantityChange", "[QuantityChange] <> 0"));

        // Egy termék / raktár mozgásai időrendben (készlettörténet)
        builder.HasIndex(m => new { m.ProductId, m.WarehouseId, m.CreatedAt });

        // A termék törlésével a naplója is törlődik (ahogy az Inventory sorai)
        builder.HasOne(m => m.Product)
            .WithMany()
            .HasForeignKey(m => m.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Raktár és rendelés nem törölhető, amíg a napló hivatkozik rá
        builder.HasOne(m => m.Warehouse)
            .WithMany()
            .HasForeignKey(m => m.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.SupplierOrder)
            .WithMany()
            .HasForeignKey(m => m.SupplierOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(m => m.CustomerOrder)
            .WithMany()
            .HasForeignKey(m => m.CustomerOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(SeedData.StockMovements);
    }
}
