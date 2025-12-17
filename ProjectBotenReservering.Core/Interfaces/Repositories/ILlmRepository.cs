namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface ILlmRepository
{
    Task<string> GenerateContentAsync(string prompt);
    Task<string> GenerateContentWithContextAsync(string prompt, string systemInstruction);
}

