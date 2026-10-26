using MediatR;
using RedKey.Application.Common.Models;

namespace RedKey.Application.Features.Units.DeleteUnit;

/// <summary>Soft delete. The super admin deletes any unit, a broker only his own.</summary>
public sealed record DeleteUnitCommand(Guid Id) : IRequest<Result>;
