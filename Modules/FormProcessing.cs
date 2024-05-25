using Microsoft.AspNetCore.Http;

namespace Money.Modules
{
    public class FormProcessing
    {
        /// <summary>
        /// Reads the file from the given HttpRequest.
        /// </summary>
        /// <param name="req">A HttpRequest with a file passed in with the key
        /// `fileName`.</param>
        /// <param name="fileName">The name of the key holding the passed file.
        /// </param>
        /// <returns>A stream of the file's data.</returns>
        /// <exception cref="FormProcessingException"></exception>
        public static async Task<Stream> ReadFormFileAsync(HttpRequest req, string fileName)
        {
            // checks if any form data is even present
            if (req.HasFormContentType)
            {
                var formdata = await req.ReadFormAsync();
                // checks if a file was sent as part of the request
                if (formdata.Files.Count > 0) 
                {
                    var file = req.Form.Files[fileName];
                    if (file != null)
                        return file.OpenReadStream();
                    else
                        throw new FormProcessingException("File was null");
                }
                else
                    throw new FormProcessingException("File was not part of form");
            }
            else
                throw new FormProcessingException("Form has no content");
        }

        public class FormProcessingException : Exception
        {
            public FormProcessingException(string message) : base(message) { }
        }
    }
}