using MediatR;
using RedKey.Application.Common.Localization;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Localization;
using RedKey.Application.Dtos.Lookups;
using RedKey.Domain.Enums;

namespace RedKey.Application.Features.Lookups.GetLookups;

public sealed class GetLookupsQueryHandler : IRequestHandler<GetLookupsQuery, Result<LookupsDto>>
{
    private readonly ILanguageContext _language;

    public GetLookupsQueryHandler(ILanguageContext language)
    {
        _language = language;
    }

    public Task<Result<LookupsDto>> Handle(GetLookupsQuery request, CancellationToken cancellationToken)
    {
        var ar = _language.IsArabic;

        return Task.FromResult(Result<LookupsDto>.Success(new LookupsDto
        {
            UnitPurposes = Items<UnitPurpose>(ar),
            FinishingTypes = Items<FinishingType>(ar),
            PreferredTimes = Items<PreferredTime>(ar),
            UnitStatuses = Items<UnitStatus>(ar),
            ReviewDecisions = Items<UnitReviewDecision>(ar),
            ViewingRequestStatuses = Items<ViewingRequestStatus>(ar),
            ContactSubjects = Items<ContactSubject>(ar)
        }));
    }

    private static IReadOnlyList<LookupItemDto> Items<TEnum>(bool ar) where TEnum : struct, Enum =>
        EnumNames.All<TEnum>(ar).Select(e => new LookupItemDto { Value = e.Value, Name = e.Name }).ToList();
}
