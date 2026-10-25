using IOMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOMS.DAL.Configurations;

internal class RackConfiguration : IEntityTypeConfiguration<Rack>
{
    public void Configure(EntityTypeBuilder<Rack> builder)
    {
        builder.Property(r => r.Code).IsRequired().HasMaxLength(10);
        builder.HasIndex(r => new { r.WarehouseId, r.Code }).IsUnique();

        // Irány szövegként tárolva ("Horizontal"), mint a rendelés státusza
        builder.Property(r => r.Orientation).HasConversion<string>().HasMaxLength(20);

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_Rack_Position", "[X] >= 0 AND [Y] >= 0");
            t.HasCheckConstraint("CK_Rack_Size", "[Length] > 0 AND [Levels] > 0");
        });

        // A raktár törlésével az elrendezése is törlődik (a WarehouseService csak készlet nélküli raktárt töröl)
        builder.HasOne(r => r.Warehouse)
            .WithMany(w => w.Racks)
            .HasForeignKey(r => r.WarehouseId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(SeedData.Racks);
    }
}
