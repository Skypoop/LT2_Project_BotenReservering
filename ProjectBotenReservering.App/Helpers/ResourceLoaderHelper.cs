using System.Reflection;

namespace ProjectBotenReservering.App.Helpers
{
    public static class ResourceLoaderHelper
    {
        public static async Task<string> LoadEmbeddedResourceAsync(string fileName)
        {
            Assembly assembly = Assembly.GetExecutingAssembly();
            string[] allResources = assembly.GetManifestResourceNames();
            string? resourcePath= allResources.FirstOrDefault(reader => reader.EndsWith(fileName));
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
