using MediatR;
using RedKey.Application.Common.Models;

namespace RedKey.Application.Features.ContactMessages.DeleteContactMessage;

/// <summary>Removes a message from the super admin inbox.</summary>
public sealed record DeleteContactMessageCommand(Guid Id) : IRequest<Result>;
