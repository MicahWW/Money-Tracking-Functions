using Microsoft.AspNetCore.Mvc;

namespace Money.Modules
{
    /// <summary>
    /// Through out the project different ObjectResults will need to be returned
    /// to convey error messages to the client. Gnerally nothing fancy needs to
    /// be returned so this helps shorten the code requried to write out.
    /// </summary>
    public class ErrorResponse : ObjectResult
    {
        public ErrorResponse(string message, int code) : base(message)
        {
            this.StatusCode = code;
        }
    }
}