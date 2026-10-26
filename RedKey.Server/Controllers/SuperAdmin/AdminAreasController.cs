using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Lookups;
using RedKey.Application.Features.Areas.CreateArea;
using RedKey.Application.Features.Areas.DeleteArea;
using RedKey.Application.Features.Areas.GetAreasAdmin;
using RedKey.Application.Features.Areas.UpdateArea;
using RedKey.Server.Extensions;

namespace RedKey.Server.Controllers.SuperAdmin;

[Route("api/admin/areas")]
public sealed class AdminAreasController : SuperAdminControllerBase
{
    private readonly ISender _sender;

    public AdminAreasController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>The areas of one city.</summary>
    [HttpGet]
    [ProducesResponseType<PagedResult<AreaAdminDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAreas([FromQuery] int cityId, [FromQuery] string? search, [FromQuery] int? page, [FromQuery] int? pageSize, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetAreasAdminQuery(cityId, search, page, pageSize), cancellationToken)).ToActionResult();

    [HttpPost]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateAreaCommand command, CancellationToken cancellationToken) =>
        (await _sender.Send(command, cancellationToken)).ToActionResult();

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateAreaCommand command, CancellationToken cancellationToken) =>
        (await _sender.Send(command with { Id = id }, cancellationToken)).ToActionResult();

    /// <summary>Deletes the area when no units use it, otherwise deactivates it; the result says which.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType<LookupDeleteResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        (await _sender.Send(new DeleteAreaCommand(id), cancellationToken)).ToActionResult();
}
