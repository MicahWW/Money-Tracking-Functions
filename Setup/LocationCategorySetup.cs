using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Money.Modules;
using Money.Tables;

namespace Money.Setup
{
    public class LocationCategorySetup
    {
        private readonly ILogger<LocationCategorySetup> _logger;

        public LocationCategorySetup(ILogger<LocationCategorySetup> logger)
        {
            _logger = logger;
        }

        [Function("LocationCategorySetup")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Admin, "put", Route = "setup/location-category")] HttpRequest req)
        {
            LocationCategoryTable.Setup();

            return new OkObjectResult("Location categories setup");
        }
    }
}
