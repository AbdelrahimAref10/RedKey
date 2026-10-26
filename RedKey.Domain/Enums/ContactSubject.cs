namespace RedKey.Domain.Enums;

/// <summary>What a contact message (website "send us a message" form) is about.</summary>
public enum ContactSubject : byte
{
    Buy = 1,
    Rent = 2,
    Invest = 3,
    ListUnit = 4,
    Other = 5
}
