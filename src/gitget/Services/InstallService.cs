using System.IO.Compression;
using GitGet.Models;

namespace GitGet.Services;

public sealed class InstallService(ConfigService configService, RegistryService registryService, GitHubReleaseService gitHubReleaseService)
{
    public async Task InstallAsync(string packageName, CancellationToken cancellationToken = default)
    {
        var package = await registryService.FindByNameAsync(packageName, cancellationToken) ?? throw new InvalidOperationException($"Package '{packageName}' was not found in the registry.");

        var config = configService.LoadConfig();
        var installPath = configService.GetInstallPath(config, package.Name);

        if (Directory.Exists(installPath))
        {
            throw new InvalidOperationException($"Package '{package.Name}' is already installed at '{installPath}'. Uninstall it first.");
        }

        var (release, asset) = await gitHubReleaseService.GetLatestZipAssetAsync(package, cancellationToken);

        var tempDir = Path.Combine(configService.AppDataDirectory, "downloads");
        Directory.CreateDirectory(tempDir);
        var zipPath = Path.Combine(tempDir, $"{package.Name}-{asset.Name}");

        try
        {
            Console.WriteLine($"Downloading {asset.Name} from {package.Repo}...");
            await gitHubReleaseService.DownloadAssetAsync(asset, zipPath, cancellationToken);

            Console.WriteLine($"Extracting to {installPath}...");
            Directory.CreateDirectory(installPath);
            ZipFile.ExtractToDirectory(zipPath, installPath, overwriteFiles: true);

            var installed = configService.LoadInstalled();
            installed.Packages.RemoveAll(p => string.Equals(p.Name, package.Name, StringComparison.OrdinalIgnoreCase));
            installed.Packages.Add(new InstalledPackage
            {
                Name = package.Name,
                Version = release.TagName,
                InstallPath = installPath,
                InstalledAt = DateTimeOffset.UtcNow
            });
            configService.SaveInstalled(installed);

            Console.WriteLine($"Installed '{package.Name}' ({release.TagName}) to {installPath}");
        }
        finally
        {
            if (File.Exists(zipPath))
            {
                File.Delete(zipPath);
            }
        }
    }

    public Task UninstallAsync(string packageName, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var installed = configService.LoadInstalled();
        var entry = installed.Packages.FirstOrDefault(p => string.Equals(p.Name, packageName, StringComparison.OrdinalIgnoreCase));

        if (entry is null)
        {
            var config = configService.LoadConfig();
            var fallbackPath = configService.GetInstallPath(config, packageName);
            if (!Directory.Exists(fallbackPath))
            {
                throw new InvalidOperationException($"Package '{packageName}' is not installed.");
            }

            Directory.Delete(fallbackPath, recursive: true);
            Console.WriteLine($"Removed '{packageName}' from {fallbackPath}");
            return Task.CompletedTask;
        }

        if (Directory.Exists(entry.InstallPath))
        {
            Directory.Delete(entry.InstallPath, recursive: true);
        }

        installed.Packages.RemoveAll(p => string.Equals(p.Name, packageName, StringComparison.OrdinalIgnoreCase));
        configService.SaveInstalled(installed);

        Console.WriteLine($"Uninstalled '{packageName}' from {entry.InstallPath}");
        return Task.CompletedTask;
    }
}