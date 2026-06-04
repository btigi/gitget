using System.Text.Json;
using GitGet.Models;
using GitGet.Serialization;

namespace GitGet.Services;

public sealed class RegistryService(HttpClient httpClient, ConfigService configService)
{

    public async Task<IReadOnlyList<PackageManifest>> GetPackagesAsync(CancellationToken cancellationToken = default)
    {
        var localPath = configService.GetLocalPackagesPath();
        if (localPath is not null)
        {
            return LoadPackagesFromFile(localPath);
        }

        var config = configService.LoadConfig();
        var url = configService.GetRegistryPackagesUrl(config);

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.TryAddWithoutValidation("User-Agent", Config.AppSettings.UserAgent);

        using var response = await httpClient.SendAsync(request, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var index = await JsonSerializer.DeserializeAsync(stream, GitGetJsonContext.Default.PackageIndex, cancellationToken) ?? throw new InvalidOperationException("Registry index is empty or invalid.");

        return index.Packages;
    }

    private static IReadOnlyList<PackageManifest> LoadPackagesFromFile(string path)
    {
        var json = File.ReadAllText(path);
        var index = JsonSerializer.Deserialize(json, GitGetJsonContext.Default.PackageIndex) ?? throw new InvalidOperationException($"Registry index is empty or invalid: {path}");

        return index.Packages;
    }

    public async Task<PackageManifest?> FindByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        var packages = await GetPackagesAsync(cancellationToken);
        return packages.FirstOrDefault(p => string.Equals(p.Name, name, StringComparison.OrdinalIgnoreCase));
    }

    public async Task<IReadOnlyList<PackageManifest>> SearchAsync(string term, CancellationToken cancellationToken = default)
    {
        var packages = await GetPackagesAsync(cancellationToken);
        if (string.IsNullOrWhiteSpace(term))
        {
            return packages;
        }

        return packages
            .Where(p =>
                p.Name.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                (p.Description?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false))
            .ToList();
    }
}