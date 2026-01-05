namespace ProjectBotenReservering.Core.Interfaces.RestClients;

public interface ITweetRestClient
{
    Task<string> UploadMediaAsync(Stream mediaStream, string fileName);
    Task<bool> PublishTweetAsync(string text, string? mediaId = null);
}