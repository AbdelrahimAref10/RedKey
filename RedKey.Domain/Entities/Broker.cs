using RedKey.Domain.Common;
using RedKey.Domain.Entities.Identity;
using RedKey.Domain.Enums;

namespace RedKey.Domain.Entities;

public class Broker : BaseEntity
{
    private Broker() { }

    public Guid UserId { get; private set; }
    public string Code { get; private set; } = string.Empty;
    public string FullName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public BrokerType Type { get; private set; }
    public string? CompanyName { get; private set; }
    public string? NationalId { get; private set; }
    public string? City { get; private set; }
    public BrokerStatus Status { get; private set; } = BrokerStatus.Pending;
    public DateTime? ApprovedAtUtc { get; private set; }

    public ApplicationUser User { get; private set; } = null!;

    public static Broker Create(
        Guid userId,
        string fullName,
        string phoneNumber,
        string email,
        BrokerType type,
        string? companyName,
        string? nationalId,
        string? city) => new()
        {
            UserId = userId,
            Code = GenerateCode(),
            FullName = fullName,
            PhoneNumber = phoneNumber,
            Email = email,
            Type = type,
            CompanyName = type == BrokerType.Company ? companyName : null,
            NationalId = nationalId,
            City = city
        };

    private static string GenerateCode() =>
        $"BR-{Random.Shared.Next(100000, 999999)}";
}
