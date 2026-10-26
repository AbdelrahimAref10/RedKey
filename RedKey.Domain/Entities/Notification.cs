using RedKey.Domain.Common;
using RedKey.Domain.Entities.Identity;
using RedKey.Domain.Enums;

namespace RedKey.Domain.Entities;

public class Notification : BaseEntity
{
    private Notification() { }

    public Guid RecipientUserId { get; private set; }
    public NotificationType Type { get; private set; }
    public string TitleAr { get; private set; } = string.Empty;
    public string TitleEn { get; private set; } = string.Empty;
    public string BodyAr { get; private set; } = string.Empty;
    public string BodyEn { get; private set; } = string.Empty;
    /// <summary>Front-end route opened when the notification is clicked.</summary>
    public string LinkUrl { get; private set; } = string.Empty;
    public Guid? EntityId { get; private set; }
    public bool IsRead { get; private set; }
    public DateTime? ReadAtUtc { get; private set; }

    public ApplicationUser Recipient { get; private set; } = null!;

    public static Notification Create(
        Guid recipientUserId,
        NotificationType type,
        string titleAr,
        string titleEn,
        string bodyAr,
        string bodyEn,
        string linkUrl,
        Guid? entityId) => new()
        {
            RecipientUserId = recipientUserId,
            Type = type,
            TitleAr = titleAr,
            TitleEn = titleEn,
            BodyAr = bodyAr,
            BodyEn = bodyEn,
            LinkUrl = linkUrl,
            EntityId = entityId
        };

    public void MarkRead(DateTime nowUtc)
    {
        if (IsRead) return;
        IsRead = true;
        ReadAtUtc = nowUtc;
    }
}
