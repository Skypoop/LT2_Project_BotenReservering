namespace ProjectBotenReservering.Core.Data.Helpers
{
    public static class StreamHelper
    {
        public static async Task<byte[]> ReadStreamToBytesAsync(Stream stream)
        {
            using MemoryStream ms = new();
            await stream.CopyToAsync(ms);
            return ms.ToArray();
        }
    }
}
