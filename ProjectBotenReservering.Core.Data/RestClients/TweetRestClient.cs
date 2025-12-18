namespace ProjectBotenReservering.Core.Data.RestClients;

using ProjectBotenReservering.Core.Data.Helpers;
using System;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Tweetinvi;
using Tweetinvi.Core.Web;

public class TweetRestClient
{
    private readonly TwitterClient _client;

    public TweetRestClient(string apiKey, string apiSecret, string accessToken, string accessSecret)
    {
        _client = new TwitterClient(apiKey, apiSecret, accessToken, accessSecret);
    }

    public async Task<string> UploadMediaAsync(Stream mediaStream, string fileName)
    {
        byte[] mediaBytes = await StreamHelper.ReadStreamToBytesAsync(mediaStream);
        ITwitterResult result = await PostMediaUploadAsync(mediaBytes, fileName);
        return ExtractMediaIdStringFromResponse(result);
    }

    public async Task<string> PublishTweetAsync(string text, string? mediaId = null)
    {
        string payload = BuildTweetJson(text, mediaId);
        ITwitterResult result = await PostTweetPostAsync(payload);
        return TransformStatusCodeToResponseMessage(result);
    }

    private async Task<ITwitterResult> PostMediaUploadAsync(byte[] mediaBytes, string fileName)
    {
        return await _client.Execute.AdvanceRequestAsync(request =>
        {
            request.Query.Url = "https://upload.twitter.com/1.1/media/upload.json";
            request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;

            MultipartFormDataContent form = new MultipartFormDataContent();
            ByteArrayContent content = new ByteArrayContent(mediaBytes);
            content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
            form.Add(content, "media", fileName);

            request.Query.HttpContent = form;
        });
    }

    private static string ExtractMediaIdStringFromResponse(ITwitterResult result)
    {
        if (!result.Response.IsSuccessStatusCode)
            throw new InvalidOperationException($"Media upload failed ({result.Response.StatusCode}): {result.Content}");

        using JsonDocument doc = JsonDocument.Parse(result.Content);
        JsonElement root = doc.RootElement;
         
        return root.GetProperty("media_id_string").GetString()
               ?? throw new InvalidOperationException("media_id_string not found in response");
    }

    private static string BuildTweetJson(string text, string? mediaId)
    {
        object payload = string.IsNullOrEmpty(mediaId)
            ? new { text }
            : new { text, media = new { media_ids = new[] { mediaId } } };

        return JsonSerializer.Serialize(payload);
    }

    private async Task<ITwitterResult> PostTweetPostAsync(string jsonPayload)
    {
        return await _client.Execute.AdvanceRequestAsync(request =>
        {
            request.Query.Url = "https://api.twitter.com/2/tweets";
            request.Query.HttpMethod = Tweetinvi.Models.HttpMethod.POST;
            request.Query.HttpContent = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        });
    }

    private static string TransformStatusCodeToResponseMessage(ITwitterResult result)
    {
        if (result.Response.IsSuccessStatusCode)
        {
            return "Tweet is succesvol geplaatst!";
        }
        return "Er is iets fout gegaan, excuses!";
    }
}