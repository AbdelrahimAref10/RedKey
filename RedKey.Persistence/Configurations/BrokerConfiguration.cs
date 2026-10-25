using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedKey.Domain.Entities;

namespace RedKey.Persistence.Configurations;

internal sealed class BrokerConfiguration : IEntityTypeConfiguration<Broker>
{
    public void Configure(EntityTypeBuilder<Broker> builder)
    {
        builder.ToTable(TableNames.Brokers);

        builder.HasKey(b => b.Id);
        builder.Property(b => b.Id).ValueGeneratedNever();

        builder.Property(b => b.Code).HasMaxLength(20).IsRequired();
        builder.Property(b => b.FullName).HasMaxLength(150).IsRequired();
        builder.Property(b => b.PhoneNumber).HasMaxLength(20).IsRequired();
        builder.Property(b => b.Email).HasMaxLength(256).IsRequired();
        builder.Property(b => b.CompanyName).HasMaxLength(150);
        builder.Property(b => b.NationalId).HasMaxLength(20);
        builder.Property(b => b.City).HasMaxLength(100);
        builder.Property(b => b.Type).HasConversion<byte>();
        builder.Property(b => b.Status).HasConversion<byte>();

        builder.HasIndex(b => b.UserId).IsUnique();
        builder.HasIndex(b => b.Code).IsUnique();
        builder.HasIndex(b => b.Status);

        builder.HasOne(b => b.User)
            .WithOne(u => u.Broker)
            .HasForeignKey<Broker>(b => b.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
