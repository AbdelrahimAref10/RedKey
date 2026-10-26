using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Lookups;

namespace RedKey.Application.Features.Lookups.GetLookups;

/// <summary>Every back-end enum, translated to the request language.</summary>
public sealed record GetLookupsQuery : IRequest<Result<LookupsDto>>;
