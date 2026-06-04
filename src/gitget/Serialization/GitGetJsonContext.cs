using System.Text.Json.Serialization;
using GitGet.Models;

namespace GitGet.Serialization;

[JsonSerializable(typeof(PackageIndex))]
[JsonSerializable(typeof(GitGetConfig))]
[JsonSerializable(typeof(InstalledPackageIndex))]
[JsonSerializable(typeof(GitHubRelease))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true, WriteIndented = true)]
internal partial class GitGetJsonContext : JsonSerializerContext;