using MediatR;
using RedKey.Application.Common.Models;

namespace RedKey.Application.Features.Cities.CreateCity;

public sealed record CreateCityCommand : IRequest<Result<int>>
{
    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public int SortOrder { get; init; }
}
