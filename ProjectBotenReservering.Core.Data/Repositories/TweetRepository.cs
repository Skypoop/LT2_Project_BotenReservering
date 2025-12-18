namespace ProjectBotenReservering.Core.Data.Repositories;

using System;
using System.IO;
using System.Threading.Tasks;
using ProjectBotenReservering.Core.Data.RestClients;
using ProjectBotenReservering.Core.Interfaces.Repositories;

public class TweetRepository : ITweetRepository
{
    private readonly TweetRestClient _restClient;

    public TweetRepository(TweetRestClient restClient)
    {
        _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
    }

    public async Task<string> UploadMediaAsync(Stream mediaStream, string fileName)
    {
        if (mediaStream == null) throw new ArgumentException("Media stream cannot be null.", nameof(mediaStream));
        if (string.IsNullOrWhiteSpace(fileName)) throw new ArgumentException("File name cannot be empty.", nameof(fileName));

        return await _restClient.UploadMediaAsync(mediaStream, fileName);
    }

    public async Task<string> PublishTweetAsync(string text, string? mediaId = null)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Tweet text cannot be empty.", nameof(text));

        return await _restClient.PublishTweetAsync(text, mediaId);
    }
}