using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Money.Tables;

namespace Money.Functions.Setup
{
    public class LocationNamesSetup
    {
        private readonly ILogger<LocationNamesSetup> _logger;

        public LocationNamesSetup(ILogger<LocationNamesSetup> logger)
        {
            _logger = logger;
        }

        [Function("LocationNamesSetup")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "setup/location-name")] HttpRequest req)
        {
            LocationNamesTable.Setup();

            return new OkObjectResult("");
        }
    }
}
