using GitGet.Services;

namespace GitGet.Commands;

public static class UninstallCommand
{
    public static async Task<int> ExecuteAsync(InstallService installService, string name)
    {
        try
        {
            await installService.UninstallAsync(name);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }
}