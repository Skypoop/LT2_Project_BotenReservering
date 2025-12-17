namespace ProjectBotenReservering.Core.Services;

using System;
using System.Threading.Tasks;
using Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;

public class LlmService : ILlmService
{
    private readonly ILlmRepository _llmRepository;

    public LlmService(ILlmRepository llmRepository)
    {
        _llmRepository = llmRepository;
    }

    public async Task<string> GenerateTextAsync(string prompt)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Prompt cannot be null or empty.", nameof(prompt));
        }

        return await _llmRepository.GenerateContentAsync(prompt);
    }

    public async Task<string> GenerateTextWithContextAsync(string prompt, string context)
    {
        if (string.IsNullOrWhiteSpace(prompt))
        {
            throw new ArgumentException("Prompt cannot be null or empty.", nameof(prompt));
        }

        if (string.IsNullOrWhiteSpace(context))
        {
            throw new ArgumentException("Context cannot be null or empty.", nameof(context));
        }

        return await _llmRepository.GenerateContentWithContextAsync(prompt, context);
    }
}
