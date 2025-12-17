using ProjectBotenReservering.Core.Interfaces.Helpers;
using ProjectBotenReservering.Core.Interfaces.Services;

namespace ProjectBotenReservering.Core.Services;
public class TweetService : ITweetService
{
    private readonly ILlmService _llmService;
    private readonly IPromptHelper _promptHelper;

    public TweetService(ILlmService llmService, IPromptHelper promptHelper)
    {
        _llmService = llmService;
        _promptHelper = promptHelper;
    }

    public async Task<string> GenerateCompetitionTweetAsync(string competitionContext)
    {
        string systemPrompt = await _promptHelper.LoadPromptAsync("TweetSystemPrompt.txt");
        string userPromptTemplate = await _promptHelper.LoadPromptAsync("TweetUserPrompt.txt");

        string contextToUse = string.IsNullOrWhiteSpace(competitionContext)
            ? "Geen specifieke wedstrijdinformatie beschikbaar. Verzin een algemene wedstrijd."
            : competitionContext;

        string finalUserPrompt = string.Format(userPromptTemplate, contextToUse);

        return await _llmService.GenerateTextWithContextAsync(finalUserPrompt, systemPrompt);
    }
}