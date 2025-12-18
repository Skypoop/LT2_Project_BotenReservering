namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface ILlmService
{
    Task<string> GenerateTextAsync(string prompt);
    Task<string> GenerateTextWithContextAsync(string prompt, string context);
}

