using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedKey.Domain.Entities;

namespace RedKey.Persistence.Configurations;

internal sealed class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.ToTable(TableNames.Customers);

        builder.HasKey(c => c.Id);
        builder.Property(c => c.Id).ValueGeneratedNever();

        builder.Property(c => c.FullName).HasMaxLength(150).IsRequired();
        builder.Property(c => c.PhoneNumber).HasMaxLength(20).IsRequired();
        builder.Property(c => c.Email).HasMaxLength(256).IsRequired();
        builder.Property(c => c.City).HasMaxLength(100);
        builder.Property(c => c.Address).HasMaxLength(300);

        builder.HasIndex(c => c.UserId).IsUnique();
        builder.HasIndex(c => c.PhoneNumber);

        builder.HasOne(c => c.User)
            .WithOne(u => u.Customer)
            .HasForeignKey<Customer>(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
