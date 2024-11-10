using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using YamlDotNet.Serialization;

namespace Money.Function
{
    public class OpenApiFile
    {
        private readonly ILogger<OpenApiFile> _logger;

        public OpenApiFile(ILogger<OpenApiFile> logger)
        {
            _logger = logger;
        }

        [Function("OpenApiFile")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Admin, "get", Route = "openapi.json")] HttpRequest req)
        {
            string fileContents = "";
            try
            {
                string? line;
                var reader = new StreamReader("openapi.yml");
                line = reader.ReadLine();
                while (line != null)
                {
                    fileContents += line + "\n";
                    line = reader.ReadLine();
                }
                reader.Close();
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return new BadRequestObjectResult(e.Message);
            }

            var deserializer = new Deserializer();
            var data = deserializer.Deserialize(fileContents);

            return new OkObjectResult(data);
        }
    }
}