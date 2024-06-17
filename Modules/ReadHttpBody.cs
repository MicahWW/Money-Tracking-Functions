namespace Money.Modules
{
    /// <summary>
    /// Tools that help directly with a <c>HttpRequest</c>
    /// </summary>
    public class HttpRequestTools
    {
        /// <summary>
        /// Reads the body of a <c>HttpRequest</c> and returns a format that
        /// works with the CSV parser.
        /// </summary>
        /// <param name="httpBody">
        /// The body parameter of a <c>HttpRequest</c>
        /// </param>
        /// <param name="contentLength">
        /// The length of data inside the body stream (httpBody)
        /// </param>
        /// <returns>
        /// The data in the format of a <c>StringReader</c> which is readable by
        /// the CSV parser.
        /// </returns>
        public static async Task<StringReader> ReadBodyAsync(Stream httpBody, int contentLength)
        {
            byte[] streamContentBytes = new byte[contentLength];
            await httpBody.ReadAsync(streamContentBytes, 0, contentLength);
            string streamContentString = System.Text.Encoding.Default.GetString(streamContentBytes);
            StringReader sr = new StringReader(streamContentString);
            return sr;
        }
    }
}