using RedKey.Application.Common.Models;

namespace RedKey.Application.Contracts.Storage;

public interface IFileStorage
{
    /// <summary>Saves a base64 (or data-URL) image and returns its public URL (e.g. /uploads/units/{id}/x.webp).</summary>
    Task<Result<string>> SaveBase64ImageAsync(string folder, string base64, bool allowSvg = false, CancellationToken cancellationToken = default);

    /// <summary>True when the value is a URL produced by this storage (an image that is already saved).</summary>
    bool IsStoredUrl(string? value);

    void Delete(string? url);
}
