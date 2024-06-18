using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Money.Tables;

namespace Money.Functions
{
    public class AllTables
    {
        private readonly ILogger<AllTables> _logger;

        public AllTables(ILogger<AllTables> logger)
        {
            _logger = logger;
        }

        [Function("AllTables")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "all-tables")] HttpRequest req)
        {
            return new OkObjectResult(GetAllTables());
        }

        public static Dictionary<string, dynamic> GetAllTables()
        {
            return new Dictionary<string, dynamic>
            {
                { "categories", CategoriesTable.GetCategories() },
                { "items", ItemsTable.GetItems() },
                { "locationCategories", LocationCategoryTable.GetLocationCategories() },
                { "locationnames", LocationNamesTable.GetLocationNames() }
            };
        }
    }
}
