using System.Text.Json.Serialization;

namespace GitGet.Models;

public sealed class InstalledPackage
{
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    [JsonPropertyName("version")]
    public string? Version { get; init; }

    [JsonPropertyName("installPath")]
    public required string InstallPath { get; init; }

    [JsonPropertyName("installedAt")]
    public DateTimeOffset InstalledAt { get; init; }
}

public sealed class InstalledPackageIndex
{
    [JsonPropertyName("packages")]
    public List<InstalledPackage> Packages { get; init; } = [];
}