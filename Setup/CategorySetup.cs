using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Money.Tables;

namespace Money.Setup
{
    public class CategorySetup
    {
        private readonly ILogger<CategorySetup> _logger;

        public CategorySetup(ILogger<CategorySetup> logger)
        {
            _logger = logger;
        }

        [Function("CategorySetup")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Admin, "put", Route = "setup/categories")] HttpRequest req)
        {
            CategoriesTable.Setup();

            return new OkObjectResult("categories setup");
        }
    }
}
