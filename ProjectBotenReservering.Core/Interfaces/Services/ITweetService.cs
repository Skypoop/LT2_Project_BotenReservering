namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface ITweetService
{
    Task<string> GenerateCompetitionTweetAsync(string competitionContext);
    Task<string> PublishTweetAsync(string tweetContent);
    Task<string> PublishTweetWithMediaAsync(string tweetContent, byte[] fileBytes, string fileName);
}