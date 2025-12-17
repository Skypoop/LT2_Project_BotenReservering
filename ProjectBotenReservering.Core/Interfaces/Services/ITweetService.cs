namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface ITweetService
{
    Task<string> GenerateCompetitionTweetAsync(string competitionContext);
    Task<string> PublishTweetAsync(string tweetContent);
    Task<string> PublishTweetAsync(string tweetContent, Stream file, string fileName);
}