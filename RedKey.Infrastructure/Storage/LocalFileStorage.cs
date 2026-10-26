using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Extensions.Options;
using RedKey.Application.Common.Errors;
using RedKey.Application.Common.Models;
using RedKey.Application.Contracts.Storage;

namespace RedKey.Infrastructure.Storage;

/// <summary>Stores uploaded images on the server disk (served under <see cref="StorageOptions.RequestPath"/>).</summary>
internal sealed partial class LocalFileStorage : IFileStorage
{
    private readonly StorageOptions _options;
    private readonly string _root;

    public LocalFileStorage(IOptions<StorageOptions> options)
    {
        _options = options.Value;
        _root = Path.GetFullPath(_options.RootPath);
        Directory.CreateDirectory(_root);
    }

    public async Task<Result<string>> SaveBase64ImageAsync(
        string folder,
        string base64,
        bool allowSvg = false,
        CancellationToken cancellationToken = default)
    {
        var payload = StripDataUrl(base64);
        var buffer = new byte[payload.Length * 3 / 4 + 3];

        if (!Convert.TryFromBase64String(payload, buffer, out var length))
            return Result<string>.Failure(ErrorCodes.Units.InvalidImage);

        if (length > _options.MaxImageBytes)
            return Result<string>.Failure(ErrorCodes.Units.ImageTooLarge);

        var bytes = buffer.AsMemory(0, length);
        var extension = DetectExtension(bytes.Span, allowSvg);
        if (extension is null)
            return Result<string>.Failure(ErrorCodes.Units.InvalidImage);

        var directory = Path.Combine(_root, folder);
        Directory.CreateDirectory(directory);

        var fileName = $"{Guid.NewGuid():N}.{extension}";
        await File.WriteAllBytesAsync(Path.Combine(directory, fileName), bytes.ToArray(), cancellationToken);

        return Result<string>.Success($"{_options.RequestPath}/{folder}/{fileName}");
    }

    public bool IsStoredUrl(string? value) =>
        value is not null && value.StartsWith(_options.RequestPath + "/", StringComparison.OrdinalIgnoreCase);

    public void Delete(string? url)
    {
        if (!IsStoredUrl(url))
            return;

        var relative = url![(_options.RequestPath.Length + 1)..].Replace('/', Path.DirectorySeparatorChar);
        var path = Path.GetFullPath(Path.Combine(_root, relative));

        // Never leave the uploads folder.
        if (path.StartsWith(_root, StringComparison.OrdinalIgnoreCase) && File.Exists(path))
            File.Delete(path);
    }

    private static string StripDataUrl(string value)
    {
        var comma = value.IndexOf(',');
        return value.StartsWith("data:", StringComparison.OrdinalIgnoreCase) && comma > 0 ? value[(comma + 1)..] : value;
    }

    /// <summary>Detects the real file type from its first bytes (the client-declared type is ignored).</summary>
    private static string? DetectExtension(ReadOnlySpan<byte> bytes, bool allowSvg)
    {
        if (bytes.Length > 3 && bytes[0] == 0xFF && bytes[1] == 0xD8 && bytes[2] == 0xFF)
            return "jpg";
        if (bytes.Length > 8 && bytes[0] == 0x89 && bytes[1] == 0x50 && bytes[2] == 0x4E && bytes[3] == 0x47)
            return "png";
        if (bytes.Length > 12 && bytes[..4].SequenceEqual("RIFF"u8) && bytes[8..12].SequenceEqual("WEBP"u8))
            return "webp";

        if (allowSvg)
        {
            var text = Encoding.UTF8.GetString(bytes[..Math.Min(bytes.Length, 64 * 1024)]);
            // SVG icons only, without scripts / event handlers.
            if (text.Contains("<svg", StringComparison.OrdinalIgnoreCase) && !UnsafeSvg().IsMatch(text))
                return "svg";
        }

        return null;
    }

    [GeneratedRegex(@"<script|javascript:|\son\w+\s*=|<foreignObject", RegexOptions.IgnoreCase)]
    private static partial Regex UnsafeSvg();
}
