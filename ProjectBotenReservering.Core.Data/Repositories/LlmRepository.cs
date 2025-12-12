namespace ProjectBotenReservering.Core.Data.Repositories;

using System;
using System.Threading.Tasks;
using RestClients;
using ProjectBotenReservering.Core.Interfaces.Repositories;

public class LlmRepository : ILlmRepository
{
    private readonly LlmRestClient _restClient;

    public LlmRepository()
    {
        _restClient = new LlmRestClient();
    }

    public LlmRepository(string apiKey)
    {
        _restClient = new LlmRestClient(apiKey);
    }

    public LlmRepository(LlmRestClient restClient)
    {
        _restClient = restClient;
    }

    public async Task<string> GenerateContentAsync(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Prompt cannot be empty.", nameof(prompt));
        }

        return await _restClient.GenerateContentAsync(prompt);
    }

    public async Task<string> GenerateContentWithContextAsync(string prompt, string systemInstruction)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Prompt cannot be empty.", nameof(prompt));
        }

        if (string.IsNullOrWhiteSpace(systemInstruction))
        {
            throw new ArgumentException("System instruction cannot be empty.", nameof(systemInstruction));
        }

        return await _restClient.GenerateContentWithSystemInstructionAsync(prompt, systemInstruction);
    }
}

