using System.Text.Json.Serialization;
using MediatR;
using RedKey.Application.Common.Models;

namespace RedKey.Application.Features.Areas.UpdateArea;

public sealed record UpdateAreaCommand : IRequest<Result>
{
    [JsonIgnore]
    public int Id { get; init; }

    public required string NameAr { get; init; }
    public required string NameEn { get; init; }
    public int SortOrder { get; init; }
    public bool IsActive { get; init; } = true;
}
