using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using MySql.Data.MySqlClient;
using Money.Modules;

namespace Money.Setup
{
    public class CategorySetup
    {
        private readonly ILogger<CategorySetup> _logger;

        public CategorySetup(ILogger<CategorySetup> logger)
        {
            _logger = logger;
        }

        [Function("CategorySetup")]
        public IActionResult Run([HttpTrigger(AuthorizationLevel.Admin, "put", Route = "setup/categories")] HttpRequest req)
        {
            var categories = new List<string> {"No Category", "Dining", "Internet", "Gas/Automotive", "Grocery", "Phone/Cable", "Entertainment", "Healthcare", "Merchandise", "Other", "Payment", "Other Services"};
            var conn = DatabaseConnection.CreateConnection();
            var cmd = new MySqlCommand();
            cmd.Connection = conn;

            cmd.CommandText = "CREATE TABLE IF NOT EXISTS categories (id int DEFAULT NULL, label varchar(255) DEFAULT NULL)";
            cmd.ExecuteNonQuery();

            cmd.CommandText = "INSERT INTO categories VALUES (@number, @text)";
            cmd.Parameters.AddWithValue("@number", 1);
            cmd.Parameters.AddWithValue("@text", "One");
            cmd.Prepare();


            for(int i=0; i<categories.Count; i++)
            {
                cmd.Parameters["@number"].Value = i+1;
                cmd.Parameters["@text"].Value = categories[i];
                cmd.ExecuteNonQuery();
            }

            return new OkObjectResult("categories setup");
        }
    }
}
