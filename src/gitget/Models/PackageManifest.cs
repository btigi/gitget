using System.Text.Json.Serialization;

namespace GitGet.Models;

public sealed class PackageManifest
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("description")]
    public string? Description { get; init; }

    [JsonPropertyName("repo")]
    public required string Repo { get; init; }

    [JsonPropertyName("asset")]
    public string? Asset { get; init; }
}

public sealed class PackageIndex
{
    [JsonPropertyName("packages")]
    public List<PackageManifest> Packages { get; init; } = [];
}