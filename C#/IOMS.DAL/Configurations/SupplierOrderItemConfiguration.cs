using IOMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOMS.DAL.Configurations;

internal class SupplierOrderItemConfiguration : IEntityTypeConfiguration<SupplierOrderItem>
{
    public void Configure(EntityTypeBuilder<SupplierOrderItem> builder)
    {
        builder.Property(i => i.UnitCost).HasColumnType("decimal(18,2)");

        // A rendelés törlésével a tételei is törlődnek
        builder.HasOne(i => i.SupplierOrder)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.SupplierOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Termék nem törölhető, amíg beszerzési tétel hivatkozik rá
        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(SeedData.SupplierOrderItems);
    }
}
