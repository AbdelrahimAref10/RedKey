using MediatR;
using RedKey.Application.Common.Models;

namespace RedKey.Application.Features.Areas.CreateArea;

public sealed record CreateAreaCommand : IRequest<Result<int>>
{
    public required int CityId { get; init; }
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public int SortOrder { get; init; }
}
