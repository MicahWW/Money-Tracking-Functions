using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Money.Modules;

namespace Money.Setup
{
    public class InsertTempData
    {
        private readonly ILogger<InsertTempData> _logger;

        public InsertTempData(ILogger<InsertTempData> logger)
        {
            _logger = logger;
        }

        [Function("InsertTempData")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Admin, "put", Route = "setup/temp-data")] HttpRequest req)
        {
            Setup();

            return new OkObjectResult("done");
        }

        public static void Setup()
        {
            var categories = new List<string> {
                "No Category",
                "Dining",
                "Internet",
                "Gas/Automotive",
                "Grocery",
                "Phone/Cable",
                "Entertainment",
                "Healthcare",
                "Merchandise",
                "Other",
                "Payment",
                "Other Services"
            };

            var locationNames = new Dictionary<string, string> {
                {"HEB ONLINE #108", "HEB"},
                {"CHIPOTLE 0101", "Chipotle"},
                {"CHIPOTLE 3992", "Chipotle"},
                {"RAISING CANES 0081", "Raising Canes"},
                {"RAISING CANES 0098", "Raising Canes"}
            };

            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand("", conn))
                {
                    var table_categories = SystemVariables.TableCategories;
                    
                    cmd.CommandText = $"SELECT COUNT(*) FROM {table_categories}";
                    object result = cmd.ExecuteScalar();
                    if (!(result != null && Convert.ToInt32(result) > 0))
                    {
                        cmd.CommandText = $"INSERT INTO {table_categories} VALUES (@id, @label)";
                        cmd.Parameters.AddWithValue("@id", 1);
                        cmd.Parameters.AddWithValue("@label", "One");
                        cmd.Prepare();

                        for(int i=0; i<categories.Count; i++)
                        {
                            cmd.Parameters["@id"].Value = i+1;
                            cmd.Parameters["@label"].Value = categories[i];
                            cmd.ExecuteNonQuery();
                        }   
                    }

                    var table_locationLongToShortName = SystemVariables.TableLocationNames;
                    cmd.CommandText = $"Select COUNT(*) FROM {table_locationLongToShortName}";
                    result = cmd.ExecuteScalar();
                    if (!(result != null && Convert.ToInt32(result) > 0))
                    {
                        cmd.CommandText = $"INSERT INTO {table_locationLongToShortName} VALUES (@providerName, @name)";
                        cmd.Parameters.AddWithValue("@providerName", "One");
                        cmd.Parameters.AddWithValue("@name", "One");
                        cmd.Prepare();

                        foreach(KeyValuePair<string, string> entry in locationNames)
                        {
                            cmd.Parameters["@providerName"].Value = entry.Key;
                            cmd.Parameters["@name"].Value = entry.Value;
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}
