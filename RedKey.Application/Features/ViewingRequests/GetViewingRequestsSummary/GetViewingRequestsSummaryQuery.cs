using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.ViewingRequests;

namespace RedKey.Application.Features.ViewingRequests.GetViewingRequestsSummary;

/// <summary>Viewing requests count per status (super admin).</summary>
public sealed record GetViewingRequestsSummaryQuery : IRequest<Result<ViewingRequestsSummaryDto>>;
