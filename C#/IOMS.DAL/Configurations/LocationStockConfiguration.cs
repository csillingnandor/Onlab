using IOMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOMS.DAL.Configurations;

internal class LocationStockConfiguration : IEntityTypeConfiguration<LocationStock>
{
    public void Configure(EntityTypeBuilder<LocationStock> builder)
    {
        // Egy termék egy helyen egyszer szerepel: összetett kulcs, mint az Inventory-nál
        builder.HasKey(s => new { s.LocationId, s.ProductId });

        // Üres helyhez nem tartozik sor, ezért a mennyiség mindig pozitív
        builder.ToTable(t => t.HasCheckConstraint("CK_LocationStock_Quantity", "[Quantity] > 0"));

        builder.HasOne(s => s.Location)
            .WithMany(l => l.Stocks)
            .HasForeignKey(s => s.LocationId)
            .OnDelete(DeleteBehavior.Cascade);

        // A termék törlésével a helyekhez rendelt készlete is törlődik (mint az Inventory sorai)
        builder.HasOne(s => s.Product)
            .WithMany()
            .HasForeignKey(s => s.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(SeedData.LocationStocks);
    }
}
