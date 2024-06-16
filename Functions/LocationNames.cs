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
            switch (req.Method)
            {
                case "GET":
                    if (!string.IsNullOrEmpty(req.Query["providerName"]))
                        return new OkObjectResult(LocationNamesTable.GetLocationNames(req.Query["providerName"]));
                    else
                        return new OkObjectResult(LocationNamesTable.GetLocationNames());
                case "POST":
                    try
                    {
                        if (req.ContentType == null)
                            return new ErrorResponse("ContentType is null", 515);
                        if (req.ContentLength == null)
                            return new ErrorResponse("ContentLength is null", 515);

                        if (string.Compare("true", req.Query["overwrite"], true) == 0)
                            return new OkObjectResult(await LocationNamesTable.UploadData(req.Body, req.ContentType, (int)req.ContentLength, true));
                        else
                            return new OkObjectResult(await LocationNamesTable.UploadData(req.Body, req.ContentType, (int)req.ContentLength, false));
                    }
                    catch (JsonException ex)
                    {
                        return new ErrorResponse($"JSON parse error on line {ex.LineNumber}", 515);
                    }
                default:
                    return new ErrorResponse($"{req.Method} has not been implemented", 515);
            }
        }
    }
}
