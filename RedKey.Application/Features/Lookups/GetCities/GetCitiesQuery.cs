using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.Lookups.GetCities;

/// <summary>Active cities, translated.</summary>
public sealed record GetCitiesQuery : IRequest<Result<IReadOnlyList<CityDto>>>;
