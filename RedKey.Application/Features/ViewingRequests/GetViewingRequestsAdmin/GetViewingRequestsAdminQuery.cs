using MediatR;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.ViewingRequests;

namespace RedKey.Application.Features.ViewingRequests.GetViewingRequestsAdmin;

/// <summary>Viewing requests for the super admin, newest first, paged.</summary>
/// <param name="Status">Value from the lookups endpoint (viewing request statuses). Null = all.</param>
/// <param name="Search">Customer name / phone or unit code.</param>
public sealed record GetViewingRequestsAdminQuery(int? Status, string? Search, int? Page, int? PageSize)
    : IRequest<Result<PagedResult<ViewingRequestAdminDto>>>;
