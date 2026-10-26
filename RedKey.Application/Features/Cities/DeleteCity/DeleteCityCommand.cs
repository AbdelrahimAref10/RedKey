using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.Cities.DeleteCity;

/// <summary>Deletes a city (and its areas) when no units use it; a city that still has units is deactivated instead.</summary>
public sealed record DeleteCityCommand(int Id) : IRequest<Result<LookupDeleteResultDto>>;
