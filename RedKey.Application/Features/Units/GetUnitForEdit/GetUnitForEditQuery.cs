using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Units;
using RedKey.Application.Features.Units.Common;

namespace RedKey.Application.Features.Units.GetUnitForEdit;

/// <summary>Both languages of a unit, for the admin / broker edit form.</summary>
public sealed record GetUnitForEditQuery(Guid Id, UnitsScope Scope) : IRequest<Result<UnitEditDto>>;
