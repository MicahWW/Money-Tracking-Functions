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
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "items")] HttpRequest req)
        {
            return new OkObjectResult(ItemsTable.GetItems());
        }
    }
}
