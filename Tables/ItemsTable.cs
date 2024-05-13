using Money.Modules;
using MySql.Data.MySqlClient;

namespace Money.Tables
{
    public class ItemsTable
    {
        public class ItemsRecord
        {
            public int id { get; set; }
            public string location { get; set; }
            public decimal amount { get; set; }
            public int category_id { get; set; }
            public DateOnly transaction_date { get; set; }

            public ItemsRecord(int id, string location, decimal amount, int category_id, string transaction_date)
            {
                this.id = id;
                this.location = location;
                this.amount = amount;
                this.category_id = category_id;
                this.transaction_date = DateOnly.Parse(transaction_date);
            }
        }

        public static List<ItemsRecord> GetItems()
        {
            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    cmd.CommandText = $"SELECT * FROM {Environment.GetEnvironmentVariable("table-expenseItems")}";

                    var rdr = cmd.ExecuteReader();

                    var result = new List<ItemsRecord>();
                    while(rdr.Read())
                        result.Add(new ItemsRecord((int)rdr[0], (string)rdr[1], (decimal)rdr[2], (int)rdr[3], (string)rdr[4]));

                    return result;
                }
            }
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
    }
}