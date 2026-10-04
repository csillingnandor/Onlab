using IOMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOMS.DAL.Configurations;

internal class CustomerOrderConfiguration : IEntityTypeConfiguration<CustomerOrder>
{
    public void Configure(EntityTypeBuilder<CustomerOrder> builder)
    {
        // Státusz szövegként tárolva ("Pending"), hogy az adatbázisban is olvasható legyen
        builder.Property(o => o.Status).HasConversion<string>().HasMaxLength(20);

        // Vevő nem törölhető, amíg vannak rendelései
        builder.HasOne(o => o.Customer)
            .WithMany(c => c.Orders)
            .HasForeignKey(o => o.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasData(SeedData.CustomerOrders);
    }
}
