using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Money.Modules;
using Money.Tables;

namespace Money.Functions
{
    public class LocationName
    {
        private readonly ILogger<LocationName> _logger;

        public LocationName(ILogger<LocationName> logger)
        {
            _logger = logger;
        }

        [Function("LocationNames")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", "post", Route = "location-names")] HttpRequest req)
        {
            if (req.Method == "GET")
                return new OkObjectResult(LocationNamesTable.GetLocationNames(req.Query["providerName"]));
            else if (req.Method == "POST")
            {
                try
                {
                    if (req.ContentType != null && req.ContentLength != null)
                    {
                        await LocationNamesTable.UploadData(req.Body, req.ContentType, (int)req.ContentLength);
                        return new OkObjectResult("yay");
                    }
                }
                catch (JsonException ex)
                {
                    return new ErrorResponse($"JSON parse error on line {ex.LineNumber}", 515);
                }
            }
            return new BadRequestObjectResult("havn't programmed yet");
        }
    }
}
