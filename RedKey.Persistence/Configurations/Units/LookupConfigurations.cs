using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedKey.Domain.Entities;

namespace RedKey.Persistence.Configurations.Units;

internal sealed class CityConfiguration : IEntityTypeConfiguration<City>
{
    public void Configure(EntityTypeBuilder<City> builder)
    {
        builder.ToTable(TableNames.Cities);
        builder.HasKey(c => c.Id);

        builder.Property(c => c.NameAr).HasMaxLength(100).IsRequired();
        builder.Property(c => c.NameEn).HasMaxLength(100).IsRequired();

        builder.HasIndex(c => new { c.IsActive, c.SortOrder });
        builder.HasQueryFilter(c => !c.IsDeleted);
    }
}

internal sealed class AreaConfiguration : IEntityTypeConfiguration<Area>
{
    public void Configure(EntityTypeBuilder<Area> builder)
    {
        builder.ToTable(TableNames.Areas);
        builder.HasKey(a => a.Id);

        builder.Property(a => a.NameAr).HasMaxLength(100).IsRequired();
        builder.Property(a => a.NameEn).HasMaxLength(100).IsRequired();

        builder.HasIndex(a => new { a.CityId, a.IsActive, a.SortOrder });
        builder.HasOne(a => a.City).WithMany().HasForeignKey(a => a.CityId).OnDelete(DeleteBehavior.Restrict);
        builder.HasQueryFilter(a => !a.IsDeleted);
    }
}

internal sealed class UnitTypeConfiguration : IEntityTypeConfiguration<UnitType>
{
    public void Configure(EntityTypeBuilder<UnitType> builder)
    {
        builder.ToTable(TableNames.UnitTypes);
        builder.HasKey(t => t.Id);

        builder.Property(t => t.NameAr).HasMaxLength(100).IsRequired();
        builder.Property(t => t.NameEn).HasMaxLength(100).IsRequired();
        builder.Property(t => t.IconUrl).HasMaxLength(500);

        builder.HasIndex(t => new { t.IsActive, t.SortOrder });
        builder.HasQueryFilter(t => !t.IsDeleted);
    }
}
