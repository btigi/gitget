using GitGet.Services;

namespace GitGet.Commands;

public static class SearchCommand
{
    public static async Task<int> ExecuteAsync(RegistryService registryService, string term)
    {
        try
        {
            var results = await registryService.SearchAsync(term);

            if (results.Count == 0)
            {
                Console.WriteLine($"No packages found matching '{term}'.");
                return 1;
            }

            Console.WriteLine($"{"Name",-30} {"Repo",-30}");
            Console.WriteLine(new string('-', 80));

            foreach (var package in results.OrderBy(p => p.Name, StringComparer.OrdinalIgnoreCase))
            {
                Console.WriteLine($"{package.Name,-30} {package.Repo,-30}");
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }
}