using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.Areas.DeleteArea;

/// <summary>Deletes an area that has no units; an area that still has units is deactivated instead.</summary>
public sealed record DeleteAreaCommand(int Id) : IRequest<Result<LookupDeleteResultDto>>;
