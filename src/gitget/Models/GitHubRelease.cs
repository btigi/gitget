using System.Text.Json.Serialization;

namespace GitGet.Models;

public sealed class GitHubRelease
{
    [JsonPropertyName("tag_name")]
    public string? TagName { get; init; }

    [JsonPropertyName("assets")]
    public List<GitHubReleaseAsset> Assets { get; init; } = [];
}

public sealed class GitHubReleaseAsset
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("label")]
    public string? Label { get; init; }

    [JsonPropertyName("browser_download_url")]
    public required string BrowserDownloadUrl { get; init; }
}