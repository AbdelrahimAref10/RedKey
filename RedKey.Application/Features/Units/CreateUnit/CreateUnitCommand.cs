using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Features.Units.Common;

namespace RedKey.Application.Features.Units.CreateUnit;

/// <summary>Same command for the super admin (published right away) and brokers (needs review).</summary>
public sealed record CreateUnitCommand : UnitInput, IRequest<Result<Guid>>;
