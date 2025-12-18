namespace ProjectBotenReservering.Core.Interfaces.RestClients;

public interface ILlmRestClient
{
    Task<string> GenerateContentAsync(string prompt, string? model = null);
    Task<string> GenerateContentWithSystemInstructionAsync(
        string prompt,
        string systemInstruction,
        string? model = null);
}