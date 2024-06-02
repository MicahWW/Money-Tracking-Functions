namespace Money.Modules
{
    public class HttpRequestTools
    {
        public static async Task<StringReader> ReadBodyAsync(Stream stream, string contentType, int contentLength)
        {
            byte[] streamContentBytes = new byte[contentLength];
            await stream.ReadAsync(streamContentBytes, 0, contentLength);
            string streamContentString = System.Text.Encoding.Default.GetString(streamContentBytes);
            StringReader sr = new StringReader(streamContentString);
            return sr;
        }
    }
}