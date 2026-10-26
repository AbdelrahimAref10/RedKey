using RedKey.Application.Contracts.Notifications;
using RedKey.Domain.Entities.Units;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Units.Common;

/// <summary>Texts + links of the unit review notifications (ar / en).</summary>
internal static class UnitNotifications
{
    public static NotificationMessage Submitted(PropertyUnit unit, string brokerName, bool isEdit) => new(
        NotificationType.UnitSubmitted,
        isEdit ? "وحدة اتعدّلت ومستنية مراجعة" : "وحدة جديدة مستنية مراجعة",
        isEdit ? "Updated unit awaiting review" : "New unit awaiting review",
        $"{brokerName} · {unit.Code} · {unit.TitleAr}",
        $"{brokerName} · {unit.Code} · {unit.TitleEn}",
        $"/admin/units/{unit.Id}",
        unit.Id);

    public static NotificationMessage Reviewed(PropertyUnit unit)
    {
        var note = unit.ReviewNote is null ? string.Empty : $" — {unit.ReviewNote}";
        var (type, titleAr, titleEn) = unit.Status switch
        {
            UnitStatus.Approved => (NotificationType.UnitApproved, "وحدتك اتنشرت", "Your unit is published"),
            UnitStatus.ChangesRequested => (NotificationType.UnitChangesRequested, "مطلوب تعديل على وحدتك", "Changes requested on your unit"),
            _ => (NotificationType.UnitRejected, "وحدتك اترفضت", "Your unit was rejected")
        };

        return new NotificationMessage(
            type,
            titleAr,
            titleEn,
            $"{unit.Code} · {unit.TitleAr}{note}",
            $"{unit.Code} · {unit.TitleEn}{note}",
            $"/broker/listings/{unit.Id}",
            unit.Id);
    }
}
