using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Money.Tables;

namespace Money.Functions
{
    public class Categories
    {
        private readonly ILogger<Categories> _logger;

        public Categories(ILogger<Categories> logger)
        {
            _logger = logger;
        }

        [Function("Categories")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "categories")] HttpRequest req)
        {
            return new OkObjectResult(CategoriesTable.GetCategories());
        }
    }
}
