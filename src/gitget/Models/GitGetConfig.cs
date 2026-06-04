using System.Text.Json.Serialization;

namespace GitGet.Models;

public sealed class GitGetConfig
{
    [JsonPropertyName("registryOwner")]
    public string RegistryOwner { get; set; } = Config.AppSettings.DefaultRegistryOwner;

    [JsonPropertyName("registryRepo")]
    public string RegistryRepo { get; set; } = Config.AppSettings.DefaultRegistryRepo;

    [JsonPropertyName("registryBranch")]
    public string RegistryBranch { get; set; } = Config.AppSettings.DefaultRegistryBranch;

    [JsonPropertyName("installRoot")]
    public string InstallRoot { get; set; } = Config.AppSettings.DefaultInstallRoot;
}