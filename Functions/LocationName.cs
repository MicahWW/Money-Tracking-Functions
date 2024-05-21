using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Money.Tables;
using Money.Modules;

namespace Money.Functions
{
    public class LocationName
    {
        private readonly ILogger<LocationName> _logger;

        public LocationName(ILogger<LocationName> logger)
        {
            _logger = logger;
        }

        [Function("LocationName")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "location-name")] HttpRequest req)
        {
            if (req.Method == "GET")
                return new OkObjectResult(LocationNamesTable.GetLocationNames(req.Query["providerName"]));
            else if (req.Method == "POST")
            {
                try
                {
                    LocationNamesTable.InsertItems(
                        LocationNameRecord.ParseCsv(
                            await FormProcessing.ReadFormFileAsync(req, "file")
                        )
                    );
                    return new OkObjectResult("done");
                }
                catch (FormProcessing.FormProcessingException ex)
                {
                    return new ErrorResponse(ex.Message, 515);
                }
            }
            return new BadRequestObjectResult("havn't programmed yet");
        }
    }
}
