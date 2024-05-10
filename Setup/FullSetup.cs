using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Money.Setup;

namespace Money.Setup
{
    public class FullSetup
    {
        private readonly ILogger<FullSetup> _logger;

        public FullSetup(ILogger<FullSetup> logger)
        {
            _logger = logger;
        }

        [Function("FullSetup")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Admin, "put", Route = "setup/full")] HttpRequest req)
        {
            DatabaseSetup.Setup();
            CategorySetup.Setup();
            LocationNamesSetup.Setup();
            LocationCategorySetup.Setup();
            
            return new OkObjectResult("Full setup completed!");
        }
    }
}
