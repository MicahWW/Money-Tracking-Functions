using Microsoft.AspNetCore.Mvc;

namespace Money.Modules
{
    /// <summary>
    /// Through out the project different ObjectResults will need to be returned
    /// to convey error messages to the client. Generally nothing fancy needs to
    /// be returned so this helps shorten the code required to write out.
    /// </summary>
    public class ErrorResponse : ObjectResult
    {
        public ErrorResponse(string message, int code) : base(message)
        {
            this.StatusCode = code;
        }

        public ErrorResponse(Dictionary<dynamic, dynamic> jsonMessage, int code): base(jsonMessage)
        {
            this.StatusCode = code;
        }
    }
}