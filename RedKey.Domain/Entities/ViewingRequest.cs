using RedKey.Domain.Common;
using RedKey.Domain.Entities.Identity;
using RedKey.Domain.Entities.Units;
using RedKey.Domain.Enums;

namespace RedKey.Domain.Entities;

public class ViewingRequest : BaseEntity
{
    private ViewingRequest() { }

    public Guid UnitId { get; private set; }
    public Guid CustomerUserId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public PreferredTime PreferredTime { get; private set; }
    public ViewingRequestStatus Status { get; private set; } = ViewingRequestStatus.New;
    /// <summary>The unit viewing fee when the request was sent (later price changes do not affect it).</summary>
    public decimal ViewingFee { get; private set; }
    public DateTime? CompletedAtUtc { get; private set; }
    /// <summary>The entry that posted the fee (null when free or not completed).</summary>
    public Guid? JournalEntryId { get; private set; }

    public PropertyUnit Unit { get; private set; } = null!;
    public ApplicationUser Customer { get; private set; } = null!;

    public static ViewingRequest Create(Guid unitId, Guid customerUserId, string name, string phoneNumber, PreferredTime preferredTime, decimal viewingFee) => new()
    {
        ViewingFee = viewingFee,
        UnitId = unitId,
        CustomerUserId = customerUserId,
        Name = name,
        PhoneNumber = phoneNumber,
        PreferredTime = preferredTime
    };

    /// <summary>Any status but completed (a completed request leaving that status clears its completion).</summary>
    public void SetStatus(ViewingRequestStatus status)
    {
        Status = status;
        if (status != ViewingRequestStatus.Completed)
        {
            CompletedAtUtc = null;
            JournalEntryId = null;
        }
    }

    public void Complete(DateTime nowUtc, Guid? journalEntryId)
    {
        Status = ViewingRequestStatus.Completed;
        CompletedAtUtc = nowUtc;
        JournalEntryId = journalEntryId;
    }
}
