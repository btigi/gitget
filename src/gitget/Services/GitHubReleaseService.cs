using System.Text.Json;
using GitGet.Models;
using GitGet.Serialization;

namespace GitGet.Services;

public sealed class GitHubReleaseService(HttpClient httpClient)
{

    public async Task<(GitHubRelease Release, GitHubReleaseAsset Asset)> GetLatestZipAssetAsync(
        PackageManifest package,
        CancellationToken cancellationToken = default)
    {
        var (owner, repo) = ParseRepo(package.Repo);
        var url = $"https://api.github.com/repos/{owner}/{repo}/releases/latest";

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.TryAddWithoutValidation("User-Agent", Config.AppSettings.UserAgent);
        request.Headers.TryAddWithoutValidation("Accept", "application/vnd.github+json");

        using var response = await httpClient.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new InvalidOperationException($"Failed to fetch latest release for '{package.Repo}' ({(int)response.StatusCode}): {body}");
        }

        await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
        var release = await JsonSerializer.DeserializeAsync(stream, GitGetJsonContext.Default.GitHubRelease, cancellationToken)
            ?? throw new InvalidOperationException($"No release data returned for '{package.Repo}'.");

        var asset = ReleaseAssetSelector.SelectZipAsset(release, package.Asset) ?? throw new InvalidOperationException($"No matching .zip asset found in the latest release for '{package.Repo}'.");

        return (release, asset);
    }

    public async Task DownloadAssetAsync(GitHubReleaseAsset asset, string destinationPath, CancellationToken cancellationToken = default)
    {
        var directory = Path.GetDirectoryName(destinationPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, asset.BrowserDownloadUrl);
        request.Headers.TryAddWithoutValidation("User-Agent", Config.AppSettings.UserAgent);

        using var response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();

        await using var fileStream = File.Create(destinationPath);
        await response.Content.CopyToAsync(fileStream, cancellationToken);
    }

    private static (string Owner, string Repo) ParseRepo(string repo)
    {
        var parts = repo.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            throw new ArgumentException($"Invalid repo format '{repo}'. Expected 'owner/repo'.");
        }

        return (parts[0], parts[1]);
    }
}
