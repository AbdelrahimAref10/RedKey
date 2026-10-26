using RedKey.Domain.Common;
using RedKey.Domain.Entities.Identity;
using RedKey.Domain.Enums;

namespace RedKey.Domain.Entities;

/// <summary>A message a signed-in customer sends from the website contact form (super admin inbox).</summary>
public class ContactMessage : BaseEntity
{
    private ContactMessage() { }

    public Guid SenderUserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    /// <summary>ISO 3166 alpha-2 of the picked country (the flag), e.g. EG.</summary>
    public string CountryIso { get; private set; } = string.Empty;
    /// <summary>Country calling code without "+", e.g. 20.</summary>
    public string DialCode { get; private set; } = string.Empty;
    /// <summary>National number, digits only, without the leading 0 (WhatsApp ready: DialCode + PhoneNumber).</summary>
    public string PhoneNumber { get; private set; } = string.Empty;
    public ContactSubject Subject { get; private set; }
    public string Message { get; private set; } = string.Empty;
    public bool IsRead { get; private set; }
    public DateTime? ReadAtUtc { get; private set; }

    public ApplicationUser Sender { get; private set; } = null!;

    public static ContactMessage Create(
        Guid senderUserId,
        string name,
        string countryIso,
        string dialCode,
        string phoneNumber,
        ContactSubject subject,
        string message) => new()
    {
        SenderUserId = senderUserId,
        Name = name,
        CountryIso = countryIso,
        DialCode = dialCode,
        PhoneNumber = phoneNumber,
        Subject = subject,
        Message = message
    };

    public void MarkRead(DateTime utcNow)
    {
        if (IsRead) return;
        IsRead = true;
        ReadAtUtc = utcNow;
    }
}
