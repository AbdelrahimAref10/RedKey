using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.UnitTypes.DeleteUnitType;

/// <summary>Deletes a unit type that has no units; a type that still has units is deactivated instead.</summary>
public sealed record DeleteUnitTypeCommand(int Id) : IRequest<Result<LookupDeleteResultDto>>;
