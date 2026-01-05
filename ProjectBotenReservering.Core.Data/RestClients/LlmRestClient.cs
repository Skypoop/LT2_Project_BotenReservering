namespace ProjectBotenReservering.Core.Data.RestClients;

using System;
using System.Threading.Tasks;
using Google.GenAI;
using Google.GenAI.Types;
using ProjectBotenReservering.Core.Interfaces.RestClients;

public class LlmRestClient: ILlmRestClient
{
    private readonly Client _client;
    private const string DEFAULT_MODEL = "gemini-2.5-flash";

    public LlmRestClient()
    {
        // The client gets the API key from the environment variable `GEMINI_API_KEY`.
        _client = new Client();
    }

    public LlmRestClient(string apiKey)
    {
        // Alternative constructor allowing explicit API key
        _client = new Client(apiKey: apiKey);
    }

    public async Task<string> GenerateContentAsync(string prompt, string? model = null)
    {
        try
        {
            GenerateContentResponse response = await _client.Models.GenerateContentAsync(
                model: model ?? DEFAULT_MODEL,
                contents: prompt
            );

            if (response.Candidates == null || response.Candidates.Count == 0)
            {
                throw new InvalidOperationException("No response candidates returned from the API.");
            }

            Content? content = response.Candidates[0].Content;
            if (content?.Parts == null || content.Parts.Count == 0)
            {
                throw new InvalidOperationException("No content parts returned from the API.");
            }

            return content.Parts[0].Text ?? string.Empty;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to generate content: {ex.Message}", ex);
        }
    }

    public async Task<string> GenerateContentWithSystemInstructionAsync(
        string prompt, 
        string systemInstruction, 
        string? model = null)
    {
        try
        {
            Content systemInstructionContent = new Content
            {
                Parts = new List<Part> { new Part { Text = systemInstruction } }
            };

            GenerateContentConfig config = new GenerateContentConfig
            {
                SystemInstruction = systemInstructionContent
            };

            GenerateContentResponse response = await _client.Models.GenerateContentAsync(
                model: model ?? DEFAULT_MODEL,
                contents: prompt,
                config: config
            );

            if (response.Candidates == null || response.Candidates.Count == 0)
            {
                throw new InvalidOperationException("No response candidates returned from the API.");
            }

            Content? content = response.Candidates[0].Content;
            if (content?.Parts == null || content.Parts.Count == 0)
            {
                throw new InvalidOperationException("No content parts returned from the API.");
            }

            return content.Parts[0].Text ?? string.Empty;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Failed to generate content with system instruction: {ex.Message}", ex);
        }
    }
}