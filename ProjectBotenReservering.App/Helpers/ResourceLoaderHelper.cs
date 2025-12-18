using System.Reflection;
using ProjectBotenReservering.Core.Interfaces.Helpers;

namespace ProjectBotenReservering.App.Helpers
{
    public class ResourceLoaderHelper : IResourceLoader
    {
        public async Task<string> LoadEmbeddedResourceAsync(string fileName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] allResources = assembly.GetManifestResourceNames();
            string? resourcePath = allResources.FirstOrDefault(reader => reader.EndsWith(fileName));
            if (resourcePath == null)
            {
                return string.Empty;
            }
            using Stream? stream = assembly.GetManifestResourceStream(resourcePath);
            if (stream == null)
            {
                return string.Empty;
            }
            using StreamReader reader = new(stream);
            return await reader.ReadToEndAsync();
        }
    }
}
