namespace ProjectBotenReservering.Core.Data.RestClients;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Core.Web;
using Tweetinvi.Models;

public class TweetRestClient
{
    private readonly HttpClient _httpClient;
    private readonly string _xApiKey;
    private readonly string _xApiSecret;
    private readonly string _xAccessKey;
    private readonly string _xAccessSecret;


    public TweetRestClient(string xApiKey,string xApiSecret,string xAccessKey, string xAccessSecret)
    {
        _xApiKey = xApiKey;
        _xApiSecret = xApiSecret;
        _xAccessKey = xAccessKey;
        _xAccessSecret = xAccessSecret;
    }
    
    public async Task<string> PostTweetAsync(string tweetContent)
    {
        // authenticate
        TwitterClient client = new TwitterClient(_xApiKey, _xApiSecret, _xAccessKey, _xAccessSecret);

         ITwitterResult result = await client.Execute.AdvanceRequestAsync((ITwitterRequest request) =>
        {
            request.Query.Url = "https://api.twitter.com/2/tweets";
            request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;

            
            object payload = new { text = tweetContent };
            string jsonBody = JsonSerializer.Serialize(payload);

            StringContent content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
            request.Query.HttpContent = content;
        });

        if (result.Response.IsSuccessStatusCode)
        {
            Console.WriteLine("Tweet sent successfully!");
            return "true";
        }
        else
        {
            Console.WriteLine($"Error: {result.Response.StatusCode}");
            Console.WriteLine(result.Content);
            return "jghg";
        }
    }


}
