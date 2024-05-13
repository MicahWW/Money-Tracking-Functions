using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Money.Tables;
using Money.Modules;

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
                ItemsTable.InsertItems(
                    CapitalOneTracationRecord.ParseCsv(
                        await FormProcessing.ReadFormFileAsync(req, "file")
                    )
                );
                return new OkObjectResult("done");
            }
            return new BadRequestObjectResult("havn't programmed yet");
        }
    }
}
