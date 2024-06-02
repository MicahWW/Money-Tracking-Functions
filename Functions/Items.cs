using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Money.Tables;

namespace Money.Functions
{
    public class Items
    {
        private readonly ILogger<Items> _logger;

        public Items(ILogger<Items> logger)
        {
            _logger = logger;
        }

        [Function("Items")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "items")] HttpRequest req)
        {
            if (req.Method == "GET")
                return new OkObjectResult(ItemsTable.GetItems());
            else if (req.Method == "POST")
            {
                if (req.ContentType != null && req.ContentLength != null)
                {
                    await ItemsTable.UploadData(req.Body, req.ContentType, (int)req.ContentLength);
                    return new OkObjectResult("yay");
                }
            }
            return new BadRequestObjectResult("havn't programmed yet");
        }
    }
}
