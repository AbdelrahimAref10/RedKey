using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.Lookups.GetAreas;

/// <summary>Active areas of active cities, translated; <paramref name="CityId"/> limits them to one city.</summary>
public sealed record GetAreasQuery(int? CityId = null) : IRequest<Result<IReadOnlyList<AreaDto>>>;
