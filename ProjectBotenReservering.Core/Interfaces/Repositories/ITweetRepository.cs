namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface ITweetRepository
{
    Task<string> PostMediaAsync(Stream file, string fileName);
    Task<string> PostTweetAsync(string tweetContent);
}
