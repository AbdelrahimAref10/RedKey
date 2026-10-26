namespace RedKey.Infrastructure.Storage;

public sealed class StorageOptions
{
    public const string SectionName = "Storage";

    /// <summary>Disk folder of the uploads (default: {ContentRoot}/uploads — outside wwwroot so the build-time static assets never shadow it).</summary>
    public string RootPath { get; set; } = string.Empty;

    /// <summary>Public URL prefix of the uploads.</summary>
    public string RequestPath { get; set; } = "/uploads";

    public int MaxImageBytes { get; set; } = 5 * 1024 * 1024;
}
