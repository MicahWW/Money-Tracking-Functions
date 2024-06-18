using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Money.Modules;
using Money.Tables;

namespace Money.Functions.Setup
{
    public class InsertExamples
    {
        private readonly ILogger<InsertExamples> _logger;

        public InsertExamples(ILogger<InsertExamples> logger)
        {
            _logger = logger;
        }

        [Function("InsertExamples")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Admin, "put", Route = "setup/insert-examples")] HttpRequest req)
        {
            return new OkObjectResult(Setup());
        }

        /// <summary>
        /// Adds some example data to the database
        /// </summary>
        /// <returns>
        /// The state of the tables after calling the function
        /// </returns>
        public static Dictionary<string, dynamic> Setup()
        {
            var categories = CategoriesTable.GetCategories();

            var locationNames = new List<LocationNamesTable.LocationNamesRecord>
            {
                new("HEB ONLINE #108", "HEB"),
                new("CHIPOTLE 0101", "Chipotle"),
                new("CHIPOTLE 3992", "Chipotle"),
                new("Example Store Location 54321", "Example Store"),
                new("Example Store Location 12345", "Example Store")
            };

            var items = new List<ItemsTable.ItemsRecord>
            {
                new("Example Store Location 54321", (decimal)15.89, categories[0].label, DateOnly.Parse("1970-01-01")),
                new("Example Store Location 12345", (decimal)42.09, categories[0].label, DateOnly.Parse("1970-01-01"))
            };
            object itemsCount, locationNamesCount;
            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand("", conn))
                {
                    var table_locationLongToShortName = SystemVariables.TableLocationNames;
                    cmd.CommandText = $"SELECT COUNT(*) FROM {table_locationLongToShortName}";
                    locationNamesCount = cmd.ExecuteScalar();

                    var table_items = SystemVariables.TableExpenseItems;
                    cmd.CommandText = $"SELECT COUNT(*) FROM {table_items}";
                    itemsCount = cmd.ExecuteScalar();
                }
            }

            if (!(locationNamesCount != null && Convert.ToInt32(locationNamesCount) > 0))
                LocationNamesTable.InsertItems(locationNames);
            if (!(itemsCount != null && Convert.ToInt32(itemsCount) > 0))
                ItemsTable.InsertItems(items);
            
            return AllTables.GetAllTables();
        }
    }
}
