using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Money.Modules;

namespace Money.Setup
{
    public class ItemSetup
    {
        private readonly ILogger<ItemSetup> _logger;

        public ItemSetup(ILogger<ItemSetup> logger)
        {
            _logger = logger;
        }

        public static void Setup()
        {

            var conn = DatabaseConnection.CreateConnection();
            var cmd = new MySqlCommand();
            cmd.Connection = conn;
            var table_expenseItems = Environment.GetEnvironmentVariable("table-expenseItems");
            var table_categories = Environment.GetEnvironmentVariable("table-categories");

            cmd.CommandText =
                $"CREATE TABLE IF NOT EXISTS {table_expenseItems} (" + 
                "  id int NOT NULL AUTO_INCREMENT," +
                "  location varchar(255) NOT NULL," +
                "  amount decimal(15, 2) NOT NULL," +
                "  category_id int NOT NULL," +
                "  transaction_date date NOT NULL," +
                "  PRIMARY KEY (id)," +
                $"  FOREIGN KEY (category_id) REFERENCES {table_categories}(id)" +
                ")";

            cmd.ExecuteNonQuery();
        }

        [Function("ItemSetup")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Admin, "put", Route = "setup/item")] HttpRequest req)
        {
            Setup();

            return new OkObjectResult("Item table setup");
        }
    }
}