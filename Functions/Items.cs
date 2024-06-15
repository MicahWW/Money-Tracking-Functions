using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Money.Modules;
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
            switch(req.Method)
            {
                case "GET":
                    return new OkObjectResult(ItemsTable.GetItems());
                case "POST":
                    if (req.ContentType == null)
                        return new ErrorResponse("ContentType is null", 515);
                    if (req.ContentLength == null)
                        return new ErrorResponse("ContentLength is null", 515);

                    await ItemsTable.UploadData(req.Body, req.ContentType, (int)req.ContentLength);
                    return new OkObjectResult("yay");
                default:
                    return new ErrorResponse($"{req.Method} has not been implemented", 515);
            }
        }
    }
}
