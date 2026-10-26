using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RedKey.Domain.Entities;

namespace RedKey.Persistence.Configurations.Units;

internal sealed class ViewingRequestConfiguration : IEntityTypeConfiguration<ViewingRequest>
{
    public void Configure(EntityTypeBuilder<ViewingRequest> builder)
    {
        builder.ToTable(TableNames.ViewingRequests);
        builder.HasKey(v => v.Id);
        builder.Property(v => v.Id).ValueGeneratedNever();

        builder.Property(v => v.Name).HasMaxLength(150).IsRequired();
        builder.Property(v => v.PhoneNumber).HasMaxLength(20).IsRequired();
        builder.Property(v => v.ViewingFee).HasPrecision(18, 2);

        builder.HasIndex(v => new { v.Status, v.CreatedAtUtc });

        // Units are soft deleted, so the request history stays.
        builder.HasOne(v => v.Unit).WithMany().HasForeignKey(v => v.UnitId).OnDelete(DeleteBehavior.Restrict);
        builder.HasOne(v => v.Customer).WithMany().HasForeignKey(v => v.CustomerUserId).OnDelete(DeleteBehavior.Restrict);

        // Same visibility as the unit it belongs to (matches PropertyUnit's soft-delete filter).
        builder.HasQueryFilter(v => !v.Unit.IsDeleted);
    }
}

internal sealed class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> builder)
    {
        builder.ToTable(TableNames.Notifications);
        builder.HasKey(n => n.Id);
        builder.Property(n => n.Id).ValueGeneratedNever();

        builder.Property(n => n.TitleAr).HasMaxLength(200).IsRequired();
        builder.Property(n => n.TitleEn).HasMaxLength(200).IsRequired();
        builder.Property(n => n.BodyAr).HasMaxLength(1500).IsRequired();
        builder.Property(n => n.BodyEn).HasMaxLength(1500).IsRequired();
        builder.Property(n => n.LinkUrl).HasMaxLength(300).IsRequired();

        // Bell: "my unread / latest notifications".
        builder.HasIndex(n => new { n.RecipientUserId, n.IsRead, n.CreatedAtUtc });

        builder.HasOne(n => n.Recipient).WithMany().HasForeignKey(n => n.RecipientUserId).OnDelete(DeleteBehavior.Cascade);
    }
}

internal sealed class ContactMessageConfiguration : IEntityTypeConfiguration<ContactMessage>
{
    public void Configure(EntityTypeBuilder<ContactMessage> builder)
    {
        builder.ToTable(TableNames.ContactMessages);
        builder.HasKey(m => m.Id);
        builder.Property(m => m.Id).ValueGeneratedNever();

        builder.Property(m => m.Name).HasMaxLength(150).IsRequired();
        builder.Property(m => m.CountryIso).HasMaxLength(2).IsUnicode(false).IsFixedLength().IsRequired();
        builder.Property(m => m.DialCode).HasMaxLength(4).IsUnicode(false).IsRequired();
        builder.Property(m => m.PhoneNumber).HasMaxLength(15).IsUnicode(false).IsRequired();
        builder.Property(m => m.Message).HasMaxLength(2000).IsRequired();

        // Inbox: newest first, unread filter + count.
        builder.HasIndex(m => new { m.IsRead, m.CreatedAtUtc });
        builder.HasIndex(m => m.CreatedAtUtc);

        builder.HasOne(m => m.Sender).WithMany().HasForeignKey(m => m.SenderUserId).OnDelete(DeleteBehavior.Restrict);
    }
}
