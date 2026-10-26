using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.ContactMessages;
using RedKey.Application.Features.ContactMessages.Common;

namespace RedKey.Application.Features.ContactMessages.GetContactMessagesAdmin;

public sealed class GetContactMessagesAdminQueryHandler : IRequestHandler<GetContactMessagesAdminQuery, Result<ContactMessagesPageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILanguageContext _language;

    public GetContactMessagesAdminQueryHandler(IApplicationDbContext context, ILanguageContext language)
    {
        _context = context;
        _language = language;
    }

    public async Task<Result<ContactMessagesPageDto>> Handle(GetContactMessagesAdminQuery request, CancellationToken cancellationToken)
    {
        var paging = Paging.From(request.Page, request.PageSize);
        var ar = _language.IsArabic;
        var query = _context.ContactMessages.AsNoTracking();

        if (request.UnreadOnly == true)
            query = query.Where(m => !m.IsRead);
        if (!string.IsNullOrWhiteSpace(request.Search))
        {
            var s = request.Search.Trim();
            var digits = PhoneDigits.National(s);
            query = query.Where(m =>
                m.Name.Contains(s) ||
                m.Message.Contains(s) ||
                (digits.Length > 0 && m.PhoneNumber.Contains(digits)));
        }

        var total = await query.CountAsync(cancellationToken);
        var unread = await _context.ContactMessages.CountAsync(m => !m.IsRead, cancellationToken);
        var rows = await query
            .OrderByDescending(m => m.CreatedAtUtc)
            .Skip(paging.Skip).Take(paging.PageSize)
            .ToRows()
            .ToListAsync(cancellationToken);

        return Result<ContactMessagesPageDto>.Success(new ContactMessagesPageDto
        {
            Page = new PagedResult<ContactMessageListItemDto>
            {
                Items = rows.Select(r => r.ToListItem(ar)).ToList(),
                Page = paging.Page,
                PageSize = paging.PageSize,
                TotalCount = total
            },
            UnreadCount = unread
        });
    }
}
