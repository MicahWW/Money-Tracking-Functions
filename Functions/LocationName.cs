using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Money.Modules;


namespace Money.Function
{
    public class LocationName
    {
        private readonly ILogger<LocationName> _logger;

        public LocationName(ILogger<LocationName> logger)
        {
            _logger = logger;
        }

        [Function("LocationName")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "location-name")] HttpRequest req)
        {
            var conn = DatabaseConnection.CreateConnection();
            var cmd = new MySqlCommand();
            cmd.Connection = conn;

            if(String.IsNullOrEmpty(req.Query["providerName"]))
                cmd.CommandText = $"SELECT * FROM {System.Environment.GetEnvironmentVariable("table-locationLongToShortName")}";
            else
                cmd.CommandText = $"SELECT * FROM {System.Environment.GetEnvironmentVariable("table-locationLongToShortName")}" +
                    $" WHERE provider_name = \"{req.Query["providerName"]}\"";

            var rdr = cmd.ExecuteReader();
            var result = new List<Dictionary<string, string>>();

            while(rdr.Read())
                result.Add(new Dictionary<string, string>() { {(string)rdr[0], (string)rdr[1]} });

            rdr.Close();
            conn.Close();

            return new OkObjectResult(result);
        }
    }
}
