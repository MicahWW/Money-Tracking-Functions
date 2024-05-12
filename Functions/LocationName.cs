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
            var table_locationLongToShortName = Environment.GetEnvironmentVariable("table-locationLongToShortName");
            string? query = req.Query["providerName"];

            if(String.IsNullOrEmpty(query))
                cmd.CommandText = $"SELECT * FROM {table_locationLongToShortName}";
            else
                cmd.CommandText = $"SELECT * FROM {table_locationLongToShortName} WHERE provider_name = \"{query}\"";

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
