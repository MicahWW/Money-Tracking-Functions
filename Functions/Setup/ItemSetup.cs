using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Money.Tables;

namespace Money.Setup
{
    public class ItemSetup
    {
        private readonly ILogger<ItemSetup> _logger;

        public ItemSetup(ILogger<ItemSetup> logger)
        {
            _logger = logger;
        }

        [Function("ItemSetup")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Admin, "put", Route = "setup/item")] HttpRequest req)
        {
            ItemsTable.Setup();

            return new OkObjectResult("");
        }
    }
}