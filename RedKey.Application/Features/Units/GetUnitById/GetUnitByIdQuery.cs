using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Units;
using RedKey.Application.Features.Units.Common;

namespace RedKey.Application.Features.Units.GetUnitById;

/// <summary>Detail page of a unit, with the same visibility rules as <see cref="GetUnits.GetUnitsQuery"/>.</summary>
public sealed record GetUnitByIdQuery(Guid Id, UnitsScope Scope) : IRequest<Result<UnitDetailsDto>>;
