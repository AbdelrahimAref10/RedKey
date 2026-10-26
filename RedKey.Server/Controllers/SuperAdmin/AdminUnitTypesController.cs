using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Lookups;
using RedKey.Application.Features.UnitTypes.CreateUnitType;
using RedKey.Application.Features.UnitTypes.DeleteUnitType;
using RedKey.Application.Features.UnitTypes.GetUnitTypesAdmin;
using RedKey.Application.Features.UnitTypes.UpdateUnitType;
using RedKey.Server.Extensions;

namespace RedKey.Server.Controllers.SuperAdmin;

[Route("api/admin/unit-types")]
public sealed class AdminUnitTypesController : SuperAdminControllerBase
{
    private readonly ISender _sender;

    public AdminUnitTypesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType<PagedResult<UnitTypeAdminDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUnitTypes([FromQuery] string? search, [FromQuery] int? page, [FromQuery] int? pageSize, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetUnitTypesAdminQuery(search, page, pageSize), cancellationToken)).ToActionResult();

    [HttpPost]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateUnitTypeCommand command, CancellationToken cancellationToken) =>
        (await _sender.Send(command, cancellationToken)).ToActionResult();

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateUnitTypeCommand command, CancellationToken cancellationToken) =>
        (await _sender.Send(command with { Id = id }, cancellationToken)).ToActionResult();

    /// <summary>Deletes the type when no units use it, otherwise deactivates it; the result says which.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType<LookupDeleteResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        (await _sender.Send(new DeleteUnitTypeCommand(id), cancellationToken)).ToActionResult();
}
