using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.ContactMessages;

namespace RedKey.Application.Features.ContactMessages.OpenContactMessage;

/// <summary>The super admin opens a message: returns it in full and marks it read.</summary>
public sealed record OpenContactMessageCommand(Guid Id) : IRequest<Result<ContactMessageDetailsDto>>;
