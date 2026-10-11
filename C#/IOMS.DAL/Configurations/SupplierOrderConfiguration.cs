using IOMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOMS.DAL.Configurations;

internal class SupplierOrderConfiguration : IEntityTypeConfiguration<SupplierOrder>
{
    public void Configure(EntityTypeBuilder<SupplierOrder> builder)
    {
        // Státusz szövegként tárolva ("Pending"), hogy az adatbázisban is olvasható legyen
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);

        // Beszállító nem törölhető, amíg vannak rendelései
        builder.HasOne(o => o.Supplier)
            .WithMany(s => s.Orders)
            .HasForeignKey(o => o.SupplierId)
            .OnDelete(DeleteBehavior.Restrict);

        // Raktár nem törölhető, amíg beszállítói rendelés hivatkozik rá
        builder.HasOne(o => o.Warehouse)
            .WithMany()
            .HasForeignKey(o => o.WarehouseId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(SeedData.SupplierOrders);
    }
}
