using MediatR;
using Microsoft.AspNetCore.Mvc;
using RedKey.Application.Common.Models;
using RedKey.Application.Dtos.Lookups;
using RedKey.Application.Features.Cities.CreateCity;
using RedKey.Application.Features.Cities.DeleteCity;
using RedKey.Application.Features.Cities.GetCitiesAdmin;
using RedKey.Application.Features.Cities.UpdateCity;
using RedKey.Server.Extensions;

namespace RedKey.Server.Controllers.SuperAdmin;

/// <summary>Cities (super admin); their areas are managed by <see cref="AdminAreasController"/>.</summary>
[Route("api/admin/cities")]
public sealed class AdminCitiesController : SuperAdminControllerBase
{
    private readonly ISender _sender;

    public AdminCitiesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType<PagedResult<CityAdminDto>>(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCities([FromQuery] string? search, [FromQuery] int? page, [FromQuery] int? pageSize, CancellationToken cancellationToken) =>
        (await _sender.Send(new GetCitiesAdminQuery(search, page, pageSize), cancellationToken)).ToActionResult();

    [HttpPost]
    [ProducesResponseType<int>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateCityCommand command, CancellationToken cancellationToken) =>
        (await _sender.Send(command, cancellationToken)).ToActionResult();

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status400BadRequest)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateCityCommand command, CancellationToken cancellationToken) =>
        (await _sender.Send(command with { Id = id }, cancellationToken)).ToActionResult();

    /// <summary>Deletes the city and its areas when no units use it, otherwise deactivates it; the result says which.</summary>
    [HttpDelete("{id:int}")]
    [ProducesResponseType<LookupDeleteResultDto>(StatusCodes.Status200OK)]
    [ProducesResponseType<ProblemDetails>(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken) =>
        (await _sender.Send(new DeleteCityCommand(id), cancellationToken)).ToActionResult();
}
