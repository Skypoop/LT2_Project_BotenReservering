namespace ProjectBotenReservering.Core.Interfaces.Services;

public interface ITweetService
{
    Task<string> GenerateCompetitionTweetAsync(string competitionContext);
}