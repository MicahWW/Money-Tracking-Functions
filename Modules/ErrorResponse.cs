using Microsoft.AspNetCore.Mvc;

namespace Money.Modules
{
    public class ErrorResponse : ObjectResult
    {
        public ErrorResponse(string message, int code) : base(message)
        {
            this.StatusCode = code;
        }
    }
}