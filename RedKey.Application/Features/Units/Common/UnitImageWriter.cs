using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Storage;

namespace RedKey.Application.Features.Units.Common;

/// <summary>
/// Turns the images of the unit form into stored URLs: base64 values are saved, URLs the unit already
/// has are kept. Files written by a failed request are cleaned up by <see cref="Rollback"/>.
/// </summary>
internal sealed class UnitImageWriter
{
    private readonly IFileStorage _storage;
    private readonly string _folder;
    private readonly HashSet<string> _existing;
    private readonly List<string> _written = [];

    public UnitImageWriter(IFileStorage storage, Guid unitId, IEnumerable<string> existingUrls)
    {
        _storage = storage;
        _folder = $"units/{unitId:N}";
        _existing = existingUrls.ToHashSet(StringComparer.OrdinalIgnoreCase);
    }

    public async Task<Result<string>> ResolveAsync(string value, CancellationToken cancellationToken)
    {
        if (_existing.Contains(value))
            return Result<string>.Success(value);

        if (_storage.IsStoredUrl(value))
            return Result<string>.Failure(ErrorCodes.Units.InvalidImage);

        var saved = await _storage.SaveBase64ImageAsync(_folder, value, cancellationToken: cancellationToken);
        if (saved.IsSuccess)
            _written.Add(saved.Value!);
        return saved;
    }

    public async Task<Result<List<string>>> ResolveManyAsync(IEnumerable<string> values, CancellationToken cancellationToken)
    {
        var urls = new List<string>();
        foreach (var value in values)
        {
            var url = await ResolveAsync(value, cancellationToken);
            if (!url.IsSuccess)
                return Result<List<string>>.Failure(url.Error, url.StatusCode);
            urls.Add(url.Value!);
        }
        return Result<List<string>>.Success(urls);
    }

    /// <summary>Deletes files saved during this request (the save failed).</summary>
    public void Rollback()
    {
        foreach (var url in _written)
            _storage.Delete(url);
    }

    /// <summary>Deletes previous images that are not used any more (after a successful save).</summary>
    public void DeleteUnused(IEnumerable<string> keptUrls)
    {
        var kept = keptUrls.ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var url in _existing.Where(u => !kept.Contains(u)))
            _storage.Delete(url);
    }
}
