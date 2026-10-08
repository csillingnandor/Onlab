using IOMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOMS.DAL.Configurations;

internal class InventoryConfiguration : IEntityTypeConfiguration<Inventory>
{
    public void Configure(EntityTypeBuilder<Inventory> builder)
    {
        // Egy termék raktáranként egyszer szerepel: összetett kulcs, külön Id nélkül
        builder.HasKey(i => new { i.ProductId, i.WarehouseId });

        // A készlet nem lehet negatív (akkor sem, ha a kód hibásan vonna le)
        builder.ToTable(t => t.HasCheckConstraint("CK_Inventory_Quantity", "[Quantity] >= 0"));

        // A termék törlésével a készletsorai is törlődnek
        // (a törlést a ProductService amúgy is tiltja, ha rendelés hivatkozik rá)
        builder.HasOne(i => i.Product)
            .WithMany(p => p.Inventories)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        // Raktár nem törölhető, amíg készlet van benne nyilvántartva
        builder.HasOne(i => i.Warehouse)
            .WithMany(w => w.Inventories)
            .HasForeignKey(i => i.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(SeedData.Inventories);
    }
}