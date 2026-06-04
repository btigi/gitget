using System.CommandLine;
using GitGet.Commands;
using GitGet.Services;

var configService = new ConfigService();
using var httpClient = new HttpClient();
var registryService = new RegistryService(httpClient, configService);
var gitHubReleaseService = new GitHubReleaseService(httpClient);
var installService = new InstallService(configService, registryService, gitHubReleaseService);

var rootCommand = new RootCommand("gitget - install portable Windows apps from GitHub releases");

var searchCommand = new Command("search", "Search the package registry for matching apps");
var searchTermArgument = new Argument<string>("term", "App name or description to search for");
searchCommand.AddArgument(searchTermArgument);
searchCommand.SetHandler(
    (string term) => SearchCommand.ExecuteAsync(registryService, term),
    searchTermArgument);
rootCommand.AddCommand(searchCommand);

var installCommand = new Command("install", "Download and install the latest release of an app");
var installNameArgument = new Argument<string>("name", "Exact package name from the registry");
installCommand.AddArgument(installNameArgument);
installCommand.SetHandler(
    (string name) => InstallCommand.ExecuteAsync(installService, name),
    installNameArgument);
rootCommand.AddCommand(installCommand);

var uninstallCommand = new Command("uninstall", "Remove an installed app from disk");
var uninstallNameArgument = new Argument<string>("name", "Package name to uninstall");
uninstallCommand.AddArgument(uninstallNameArgument);
uninstallCommand.SetHandler(
    (string name) => UninstallCommand.ExecuteAsync(installService, name),
    uninstallNameArgument);
rootCommand.AddCommand(uninstallCommand);

return await rootCommand.InvokeAsync(args);