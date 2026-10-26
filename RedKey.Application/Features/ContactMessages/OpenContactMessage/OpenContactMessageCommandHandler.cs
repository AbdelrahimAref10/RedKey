using MediatR;
using Microsoft.EntityFrameworkCore;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Localization;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Contracts.Persistence;
using RedKey.Application.Dtos.ContactMessages;

namespace RedKey.Application.Features.ContactMessages.OpenContactMessage;

public sealed class OpenContactMessageCommandHandler : IRequestHandler<OpenContactMessageCommand, Result<ContactMessageDetailsDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ILanguageContext _language;
    private readonly TimeProvider _time;

    public OpenContactMessageCommandHandler(IApplicationDbContext context, ILanguageContext language, TimeProvider time)
    {
        _context = context;
        _language = language;
        _time = time;
    }

    public async Task<Result<ContactMessageDetailsDto>> Handle(OpenContactMessageCommand request, CancellationToken cancellationToken)
    {
        var message = await _context.ContactMessages
            .Include(m => m.Sender)
            .FirstOrDefaultAsync(m => m.Id == request.Id, cancellationToken);
        if (message is null)
            return Result<ContactMessageDetailsDto>.NotFound(ErrorCodes.ContactMessages.NotFound);

        if (!message.IsRead)
        {
            message.MarkRead(_time.GetUtcNow().UtcDateTime);
            await _context.SaveChangesAsync(cancellationToken);
        }

        var ar = _language.IsArabic;
        return Result<ContactMessageDetailsDto>.Success(new ContactMessageDetailsDto
        {
            Id = message.Id,
            Name = message.Name,
            CountryIso = message.CountryIso,
            DialCode = message.DialCode,
            PhoneNumber = message.PhoneNumber,
            Subject = (int)message.Subject,
            SubjectName = EnumNames.Of(message.Subject, ar),
            Preview = message.Message,
            Message = message.Message,
            SenderEmail = message.Sender.Email ?? string.Empty,
            IsRead = true,
            ReadAtUtc = message.ReadAtUtc,
            CreatedAtUtc = message.CreatedAtUtc
        });
    }
}
