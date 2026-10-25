using RedKey.Domain.Common;
using RedKey.Domain.Entities.Identity;

namespace RedKey.Domain.Entities;

public class Customer : BaseEntity
{
    private Customer() { }

    public Guid UserId { get; private set; }
    public string FullName { get; private set; } = string.Empty;
    public string PhoneNumber { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string? City { get; private set; }
    public string? Address { get; private set; }
    public DateOnly? DateOfBirth { get; private set; }

    public ApplicationUser User { get; private set; } = null!;

    public static Customer Create(
        Guid userId,
        string fullName,
        string phoneNumber,
        string email,
        string? city) => new()
        {
            UserId = userId,
            FullName = fullName,
            PhoneNumber = phoneNumber,
            Email = email,
            City = city
        };
}
