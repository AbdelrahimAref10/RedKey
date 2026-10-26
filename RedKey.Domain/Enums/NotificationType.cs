namespace RedKey.Domain.Enums;

public enum NotificationType : byte
{
    UnitSubmitted = 1,
    UnitApproved = 2,
    UnitChangesRequested = 3,
    UnitRejected = 4,
    /// <summary>To the super admins: a customer asked to view a unit.</summary>
    ViewingRequested = 5,
    /// <summary>To the customer: the admin moved his viewing request on.</summary>
    ViewingRequestUpdated = 6
}
