using RedKey.Domain.Enums;

namespace RedKey.Application.Common.Localization;

/// <summary>
/// The only place enum display names live (ar / en). The front-end never hard-codes them:
/// it reads them from the lookups endpoints and from every DTO (value + name).
/// </summary>
public static class EnumNames
{
    private static readonly Dictionary<Type, Dictionary<int, (string Ar, string En)>> Names = new()
    {
        [typeof(UnitPurpose)] = new()
        {
            [(int)UnitPurpose.Sale] = ("بيع", "Sale"),
            [(int)UnitPurpose.Rent] = ("إيجار", "Rent"),
            [(int)UnitPurpose.Investment] = ("استثمار", "Investment")
        },
        [typeof(FinishingType)] = new()
        {
            [(int)FinishingType.CoreAndShell] = ("على الطوب", "Core & shell"),
            [(int)FinishingType.SemiFinished] = ("نصف تشطيب", "Semi-finished"),
            [(int)FinishingType.FullyFinished] = ("تشطيب كامل", "Fully finished"),
            [(int)FinishingType.Furnished] = ("مفروشة", "Furnished")
        },
        [typeof(UnitStatus)] = new()
        {
            [(int)UnitStatus.PendingReview] = ("مستنية مراجعة", "Pending review"),
            [(int)UnitStatus.Approved] = ("منشورة", "Published"),
            [(int)UnitStatus.ChangesRequested] = ("مطلوب تعديل", "Changes requested"),
            [(int)UnitStatus.Rejected] = ("اترفضت", "Rejected")
        },
        [typeof(UnitReviewDecision)] = new()
        {
            [(int)UnitReviewDecision.Approve] = ("اعتماد", "Approve"),
            [(int)UnitReviewDecision.RequestChanges] = ("اطلب تعديل", "Request changes"),
            [(int)UnitReviewDecision.Reject] = ("رفض", "Reject")
        },
        [typeof(PreferredTime)] = new()
        {
            [(int)PreferredTime.Morning] = ("الصبح", "Morning"),
            [(int)PreferredTime.Afternoon] = ("بعد الضهر", "Afternoon"),
            [(int)PreferredTime.Evening] = ("بالليل", "Evening")
        },
        [typeof(ViewingRequestStatus)] = new()
        {
            [(int)ViewingRequestStatus.New] = ("جديد", "New"),
            [(int)ViewingRequestStatus.Contacted] = ("اتكلمنا معاه", "Contacted"),
            [(int)ViewingRequestStatus.Closed] = ("مقفول", "Closed"),
            [(int)ViewingRequestStatus.Completed] = ("تمت المعاينة", "Viewing done")
        },
        [typeof(ContactSubject)] = new()
        {
            [(int)ContactSubject.Buy] = ("عايز أشتري وحدة", "I want to buy a unit"),
            [(int)ContactSubject.Rent] = ("عايز أأجّر", "I want to rent"),
            [(int)ContactSubject.Invest] = ("عايز أستثمر", "I want to invest"),
            [(int)ContactSubject.ListUnit] = ("عايز أعرض وحدتي", "I want to list my unit"),
            [(int)ContactSubject.Other] = ("حاجة تانية", "Something else")
        },
        [typeof(AccountType)] = new()
        {
            [(int)AccountType.Asset] = ("أصول", "Assets"),
            [(int)AccountType.Liability] = ("التزامات", "Liabilities"),
            [(int)AccountType.Equity] = ("حقوق ملكية", "Equity"),
            [(int)AccountType.Revenue] = ("إيرادات", "Revenues"),
            [(int)AccountType.Expense] = ("مصروفات", "Expenses")
        },
        [typeof(JournalSourceType)] = new()
        {
            [(int)JournalSourceType.ViewingFee] = ("رسوم معاينة", "Viewing fee")
        },
        [typeof(UserRole)] = new()
        {
            [(int)UserRole.SuperAdmin] = ("سوبر أدمن", "Super admin"),
            [(int)UserRole.Broker] = ("وسيط", "Broker"),
            [(int)UserRole.Customer] = ("عميل", "Customer")
        }
    };

    public static string Of<TEnum>(TEnum value, bool arabic) where TEnum : struct, Enum
    {
        var key = Convert.ToInt32(value);
        return Names.TryGetValue(typeof(TEnum), out var map) && map.TryGetValue(key, out var name)
            ? arabic ? name.Ar : name.En
            : value.ToString();
    }

    public static IReadOnlyList<(int Value, string Name)> All<TEnum>(bool arabic) where TEnum : struct, Enum =>
        Names[typeof(TEnum)].Select(kv => (kv.Key, arabic ? kv.Value.Ar : kv.Value.En)).ToList();

    public static bool IsDefined<TEnum>(int value) where TEnum : struct, Enum =>
        Names.TryGetValue(typeof(TEnum), out var map) && map.ContainsKey(value);
}
