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
            var healthStatus = await _healthCheck.CheckHealthAsync();
            string systemVariablesStatus;
            if (SystemVariables.CheckAll())
                systemVariablesStatus = "Healthy";
            else
                systemVariablesStatus = "Unhealthy";

            var output = new Dictionary<dynamic, dynamic>
            {
                { "healthStatus", healthStatus.Status.ToString() },
                { "systemVariablesStatus", systemVariablesStatus }
            };

            if (systemVariablesStatus == "Healthy" && healthStatus.Status == HealthStatus.Healthy)
                return new OkObjectResult(output);
            else
                return new ErrorResponse(output, 503);
        }
    }
}