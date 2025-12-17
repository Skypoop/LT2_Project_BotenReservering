namespace ProjectBotenReservering.Core.Data.Repositories;

using System;
using System.Threading.Tasks;
using ProjectBotenReservering.Core.Data.RestClients;
using ProjectBotenReservering.Core.Interfaces.Repositories;

public class TweetRepository: ITweetRepository
{
    private readonly TweetRestClient _restClient;
    public TweetRepository(TweetRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public async Task<string> PostTweetAsync(string tweetContent)
    {
        if (string.IsNullOrWhiteSpace(tweetContent))
        {
            throw new ArgumentException("Prompt cannot be empty.", nameof(tweetContent));
        }

        return await _restClient.PostTweetAsync(tweetContent);
    }

    public async Task<int> PostMediaAsync(string file)
    {
        throw new NotImplementedException();
    }
}

