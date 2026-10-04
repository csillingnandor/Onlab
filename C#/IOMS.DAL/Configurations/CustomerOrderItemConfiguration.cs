using IOMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOMS.DAL.Configurations;

internal class CustomerOrderItemConfiguration : IEntityTypeConfiguration<CustomerOrderItem>
{
    public void Configure(EntityTypeBuilder<CustomerOrderItem> builder)
    {
        builder.Property(i => i.UnitPrice).HasColumnType("decimal(18,2)");

        // A rendelés törlésével a tételei is törlődnek
        builder.HasOne(i => i.CustomerOrder)
            .WithMany(o => o.Items)
            .HasForeignKey(i => i.CustomerOrderId)
            .OnDelete(DeleteBehavior.Cascade);

        // Termék nem törölhető, amíg rendelési tétel hivatkozik rá
        builder.HasOne(i => i.Product)
            .WithMany()
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(SeedData.CustomerOrderItems);
    }
}
