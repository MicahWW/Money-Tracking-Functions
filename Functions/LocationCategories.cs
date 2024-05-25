using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Money.Tables;

namespace Money.Functions
{
    public class LocationCategory
    {
        private readonly ILogger<LocationCategory> _logger;

        public LocationCategory(ILogger<LocationCategory> logger)
        {
            _logger = logger;
        }

        [Function("LocationCategories")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "location-categories")] HttpRequest req)
        {
            return new OkObjectResult(LocationCategoryTable.GetLocationCategories(req.Query["locationName"]));
        }
    }
}
