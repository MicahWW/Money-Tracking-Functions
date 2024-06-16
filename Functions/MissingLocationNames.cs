using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Money.Tables;

namespace Money.Functions
{
    public class MissingLocationNames
    {
        private readonly ILogger<MissingLocationNames> _logger;

        public MissingLocationNames(ILogger<MissingLocationNames> logger)
        {
            _logger = logger;
        }

        [Function("MissingLocationNames")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            return new OkObjectResult(LocationNamesTable.MissingLocationNames());
        }
    }
}
