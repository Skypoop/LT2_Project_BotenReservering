namespace ProjectBotenReservering.Core.Interfaces.Helpers;

public interface IResourceLoader
{
    Task<string> LoadEmbeddedResourceAsync(string fileName);
}