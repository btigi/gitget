using GitGet.Services;

namespace GitGet.Commands;

public static class InstallCommand
{
    public static async Task<int> ExecuteAsync(InstallService installService, string name)
    {
        try
        {
            await installService.InstallAsync(name);
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }
}