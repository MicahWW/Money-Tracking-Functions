using Microsoft.AspNetCore.Http;

namespace Money.Modules
{
    public class FormProcessing
    {
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
                    {
                        return file.OpenReadStream();
                    }
                    else
                    {
                        throw new Exception("File was null");
                    }
                }
                else
                {
                    throw new Exception("File was not part of form");
                }
            }
            else
            {
                throw new Exception("Form has no content");
            }
        }
    }
}