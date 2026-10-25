using IOMS.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace IOMS.DAL.Configurations;

internal class StorageLocationConfiguration : IEntityTypeConfiguration<StorageLocation>
{
    public void Configure(EntityTypeBuilder<StorageLocation> builder)
    {
        builder.Property(l => l.Code).IsRequired().HasMaxLength(20);

        // Egy polc egy rekeszének egy szintje csak egyszer szerepelhet
        builder.HasIndex(l => new { l.RackId, l.Bay, l.Level }).IsUnique();

        builder.ToTable(t =>
        {
            t.HasCheckConstraint("CK_StorageLocation_Position", "[Bay] > 0 AND [Level] > 0");
            t.HasCheckConstraint("CK_StorageLocation_MaxQuantity", "[MaxQuantity] > 0");
        });

        builder.HasOne(l => l.Rack)
            .WithMany(r => r.Locations)
            .HasForeignKey(l => l.RackId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasData(SeedData.StorageLocations);
    }
}
