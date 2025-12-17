namespace ProjectBotenReservering.Core.Data.RestClients;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

public class TweetRestClient
{
    private const string BASE_URL = "https://api.x.com/2/";
    private const string TWEETS_ENDPOINT = "tweets";

    private static readonly JsonSerializerOptions _serializerOptions = new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
    };

    private readonly HttpClient _httpClient;
    private readonly string _bearerToken;

    public TweetRestClient(string bearerToken, HttpClient? httpClient = null)
    {
        //todo we need a lifecycle httpclient since thats clean 
        _httpClient = httpClient ?? CreateDefaultHttpClient();
        _bearerToken = bearerToken;
    }

    public async Task<string> PostTweetAsync(string tweetContent, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(tweetContent))
        {
            throw new ArgumentException("Tweet content cannot be empty.", nameof(tweetContent));
        }

        TweetRequest payload = new TweetRequest
        {
            Text = tweetContent
        };

        string serializedPayload = JsonSerializer.Serialize(payload, _serializerOptions);

        using HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, TWEETS_ENDPOINT);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _bearerToken);
        request.Content = new StringContent(serializedPayload, Encoding.UTF8, "application/json");

        HttpResponseMessage response = await _httpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
        string responsePayload = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine(response.StatusCode);
            Console.WriteLine(response.ToString());
            throw new InvalidOperationException($"Api call failed {response.ToString()}");
            // imlementeer exponenoiteel backoff


        }

        return responsePayload;
    }

    private static HttpClient CreateDefaultHttpClient()
    {
        HttpClient httpClient = new HttpClient
        {
            BaseAddress = new Uri(BASE_URL, UriKind.Absolute)
        };

        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        return httpClient;
    }

    private sealed class TweetRequest
    {
        public string? Text { get; set; }
    }
}
