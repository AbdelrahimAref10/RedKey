using RedKey.Application.Contracts.Notifications;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.ViewingRequests.Common;

/// <summary>Texts + links of the viewing request notifications (ar / en).</summary>
internal static class ViewingRequestNotifications
{
    /// <summary>To the super admins; the link opens the request highlighted in the requests table.</summary>
    public static NotificationMessage Requested(Guid requestId, string customerName, string unitCode, string titleAr, string titleEn) => new(
        NotificationType.ViewingRequested,
        "طلب معاينة جديد",
        "New viewing request",
        $"{customerName} · {unitCode} · {titleAr}",
        $"{customerName} · {unitCode} · {titleEn}",
        $"/admin/requests?highlight={requestId}",
        requestId);

    /// <summary>To the customer, when the admin moves the request on.</summary>
    public static NotificationMessage StatusChanged(Guid requestId, Guid unitId, ViewingRequestStatus status, string unitCode, string titleAr, string titleEn)
    {
        var (bodyAr, bodyEn) = status switch
        {
            ViewingRequestStatus.Contacted => ("فريقنا هيتواصل معاك قريب لتحديد ميعاد المعاينة.", "Our team will contact you soon to set the viewing time."),
            ViewingRequestStatus.Completed => ("تمت المعاينة، شكراً لثقتك في RedKey. لو محتاج أي حاجة كلّمنا.", "The viewing is done — thank you for trusting RedKey. Contact us if you need anything."),
            ViewingRequestStatus.Closed => ("طلب المعاينة اتقفل. لو محتاج حاجة تانية كلّمنا.", "The viewing request is closed. Contact us if you need anything else."),
            _ => ("طلب المعاينة اتفتح تاني.", "The viewing request was reopened.")
        };

        return new NotificationMessage(
            NotificationType.ViewingRequestUpdated,
            $"تحديث طلب معاينة {unitCode}",
            $"Viewing request update · {unitCode}",
            $"{titleAr} — {bodyAr}",
            $"{titleEn} — {bodyEn}",
            $"/properties/{unitId}",
            requestId);
    }
}
