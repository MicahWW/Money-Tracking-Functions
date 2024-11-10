using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Money.Modules;

namespace Money.Functions
{
    public class Health
    {
        private readonly ILogger<Health> _logger;
        private readonly HealthCheckService _healthCheck;

        public Health(ILogger<Health> logger, HealthCheckService healthCheck)
        {
            _logger = logger;
            _healthCheck = healthCheck;
        }

        [Function("Health")]
        public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get")] HttpRequest req)
        {
            var output = new Dictionary<dynamic, dynamic>
            {
                { "healthStatus", (await _healthCheck.CheckHealthAsync()).Status.ToString() },
                { "systemVariablesStatus", SystemVariables.CheckAll() ? "Healthy" : "Unhealthy" }
            };

            try
            {
                using (var conn = DatabaseConnection.CreateConnection())
                {
                    // do nothing, just testing connection
                }
                output.Add("databaseConnection", "Healthy");
            }
            catch (DatabaseConnection.DatabaseConnectionException ex)
            {
                output.Add("databaseConnection", "Unhealthy");
                output.Add("mySqlError", new Dictionary<string, dynamic> 
                {
                    { "MySQLErrorNumber", ex.MySQLErrorNumber },
                    { "MySQLErrorMessage", ex.MySQLErrorMessage }
                });
            }

            foreach (var pair in output)
            {
                if (pair.Value is string )
                {
                    if (pair.Value == "Unhealthy")
                        return new ErrorResponse(output, 503);
                }
            }
            return new OkObjectResult(output);
        }
    }
}