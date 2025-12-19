using ProjectBotenReservering.Core.Exceptions;
using ProjectBotenReservering.Core.Interfaces.Helpers;
using ProjectBotenReservering.Core.Interfaces.Repositories;
using ProjectBotenReservering.Core.Interfaces.Services;

namespace ProjectBotenReservering.Core.Services;

public class TweetService : ITweetService
{
    private readonly ILlmService _llmService;
    private readonly IPromptHelper _promptHelper;
    private readonly ITweetRepository _tweetRepository;

    public TweetService(ILlmService llmService, IPromptHelper promptHelper, ITweetRepository tweetRepository)
    {
        _llmService = llmService;
        _promptHelper = promptHelper;
        _tweetRepository = tweetRepository;
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
    private async Task<string> TweetExceptionHandler(Func<Task<bool>> action)
    {
        try
        {
            bool success = await action();
            return success ? "Gelukt!" : "Er is iets mis gegaan...";
        }
        catch (Exception ex)
        {
            throw new TweetPostException("Foutmelding...", ex);
        }
    }

    public async Task<string> PublishTweetAsync(string tweetContent)
    {
        return await TweetExceptionHandler(() => _tweetRepository.PublishTweetAsync(tweetContent));
    }
    public async Task<string> PublishTweetWithMediaAsync(string tweetContent, byte[] fileBytes, string fileName)
    {
        return await TweetExceptionHandler(async () =>
        {
            string mediaKey = await _tweetRepository.UploadMediaAsync(new MemoryStream(fileBytes), fileName);
            return await _tweetRepository.PublishTweetAsync(tweetContent, mediaKey);
        });
    }
}