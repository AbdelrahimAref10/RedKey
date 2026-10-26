namespace RedKey.Application.Common.Models;

public sealed class PagedResult<T>
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 50;

    public required IReadOnlyList<T> Items { get; init; }
    public required int Page { get; init; }
    public required int PageSize { get; init; }
    public required int TotalCount { get; init; }
    public int TotalPages => PageSize == 0 ? 0 : (int)Math.Ceiling(TotalCount / (double)PageSize);
}

/// <summary>Normalises the page / page size coming from the client.</summary>
public readonly record struct Paging(int Page, int PageSize)
{
    public static Paging From(int? page, int? pageSize) => new(
        Math.Max(1, page ?? 1),
        Math.Clamp(pageSize ?? PagedResult<object>.DefaultPageSize, 1, PagedResult<object>.MaxPageSize));

    public int Skip => (Page - 1) * PageSize;
}
