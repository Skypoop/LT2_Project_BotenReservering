namespace ProjectBotenReservering.Core.Interfaces.Repositories;

public interface ITweetRepository
{
    Task<string> UploadMediaAsync(Stream mediaStream, string fileName);
    Task<bool> PublishTweetAsync(string text, string? mediaId = null);
}
