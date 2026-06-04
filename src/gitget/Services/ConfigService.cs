using System.Text.Json;
using GitGet.Config;
using GitGet.Models;
using GitGet.Serialization;

namespace GitGet.Services;

public sealed class ConfigService
{

    public string AppDataDirectory { get; } =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), AppSettings.AppDataFolderName);

    public string ConfigFilePath => Path.Combine(AppDataDirectory, AppSettings.ConfigFileName);
    public string InstalledFilePath => Path.Combine(AppDataDirectory, AppSettings.InstalledFileName);

    public GitGetConfig LoadConfig()
    {
        EnsureAppDataDirectory();

        if (!File.Exists(ConfigFilePath))
        {
            var defaults = new GitGetConfig();
            SaveConfig(defaults);
            return defaults;
        }

        var json = File.ReadAllText(ConfigFilePath);
        return JsonSerializer.Deserialize(json, GitGetJsonContext.Default.GitGetConfig) ?? new GitGetConfig();
    }

    public void SaveConfig(GitGetConfig config)
    {
        EnsureAppDataDirectory();
        var json = JsonSerializer.Serialize(config, GitGetJsonContext.Default.GitGetConfig);
        File.WriteAllText(ConfigFilePath, json);
    }

    public string? GetLocalPackagesPath()
    {
        var path = Path.Combine(AppContext.BaseDirectory, AppSettings.LocalPackagesFileName);
        return File.Exists(path) ? path : null;
    }

    public string GetRegistryPackagesUrl(GitGetConfig config) =>
        $"https://raw.githubusercontent.com/{config.RegistryOwner}/{config.RegistryRepo}/{config.RegistryBranch}/{AppSettings.PackagesIndexFileName}";

    public string GetInstallPath(GitGetConfig config, string packageName) =>
        Path.Combine(config.InstallRoot, packageName);

    public InstalledPackageIndex LoadInstalled()
    {
        EnsureAppDataDirectory();

        if (!File.Exists(InstalledFilePath))
        {
            return new InstalledPackageIndex();
        }

        var json = File.ReadAllText(InstalledFilePath);
        return JsonSerializer.Deserialize(json, GitGetJsonContext.Default.InstalledPackageIndex)
            ?? new InstalledPackageIndex();
    }

    public void SaveInstalled(InstalledPackageIndex index)
    {
        EnsureAppDataDirectory();
        var json = JsonSerializer.Serialize(index, GitGetJsonContext.Default.InstalledPackageIndex);
        File.WriteAllText(InstalledFilePath, json);
    }

    private void EnsureAppDataDirectory()
    {
        Directory.CreateDirectory(AppDataDirectory);
    }
}
