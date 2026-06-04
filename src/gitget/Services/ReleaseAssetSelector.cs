using GitGet.Models;
using GitGet.Utilities;

namespace GitGet.Services;

public static class ReleaseAssetSelector
{
    public static GitHubReleaseAsset? SelectZipAsset(GitHubRelease release, string? assetPattern)
    {
        var candidates = release.Assets
            .Where(IsUserUploadedZip)
            .ToList();

        if (candidates.Count == 0)
        {
            return null;
        }

        if (!string.IsNullOrWhiteSpace(assetPattern))
        {
            return candidates.FirstOrDefault(a => WildcardMatcher.IsMatch(a.Name, assetPattern));
        }

        return candidates[0];
    }

    private static bool IsUserUploadedZip(GitHubReleaseAsset asset)
    {
        if (!asset.Name.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        if (!string.IsNullOrEmpty(asset.Label) &&
            asset.Label.Contains("source code", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        return true;
    }
}