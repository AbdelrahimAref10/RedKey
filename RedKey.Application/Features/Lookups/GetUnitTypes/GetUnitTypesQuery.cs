using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.Lookups.GetUnitTypes;

/// <summary>Active unit types (dropdowns + the landing "browse by type" section), translated.</summary>
public sealed record GetUnitTypesQuery : IRequest<Result<IReadOnlyList<UnitTypeDto>>>;
