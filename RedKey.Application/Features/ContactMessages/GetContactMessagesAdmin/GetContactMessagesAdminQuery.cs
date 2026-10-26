using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.ContactMessages;

namespace RedKey.Application.Features.ContactMessages.GetContactMessagesAdmin;

/// <summary>Super admin inbox, newest first.</summary>
/// <param name="UnreadOnly">True = only the unread ones.</param>
/// <param name="Search">Name, phone or message text.</param>
public sealed record GetContactMessagesAdminQuery(bool? UnreadOnly, string? Search, int? Page, int? PageSize)
    : IRequest<Result<ContactMessagesPageDto>>;
