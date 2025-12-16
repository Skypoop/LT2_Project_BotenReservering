using ProjectBotenReservering.Core.Interfaces.Helpers;

namespace ProjectBotenReservering.App.Helpers;

public class PromptHelper : IPromptHelper
{
    public async Task<string> LoadPromptAsync(string fileName)
    {
        try
        {
            using Stream stream = await FileSystem.OpenAppPackageFileAsync($"Prompts/{fileName}");
            using StreamReader reader = new StreamReader(stream);
            return await reader.ReadToEndAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading prompt {fileName}: {ex.Message}");
            return string.Empty;
        }
    }
}