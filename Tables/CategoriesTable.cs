using Money.Modules;
using MySql.Data.MySqlClient;

namespace Money.Tables
{
    public class CategoriesTable
    {
        public class CategoriesRecord
        {
            public int id { get; set; }
            public string label { get; set; }

            public CategoriesRecord(int id, string label)
            {
                this.id = id;
                this.label = label;
            }
        }

        public static List<CategoriesRecord> GetCategories(string? query = null)
        {
            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand("", conn))
                {
                    cmd.CommandText = $"SELECT * FROM {SystemVariables.TableCategories}";
                    if (!string.IsNullOrEmpty(query))
                        cmd.CommandText += $" WHERE id = {query}";

                    var rdr = cmd.ExecuteReader();

                    var result = new List<CategoriesRecord>();
                    while (rdr.Read())
                        result.Add(new CategoriesRecord((int)rdr[0], (string)rdr[1]));

                    return result;
                }
            }
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

            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand("", conn))
                {
                    var table_categories = SystemVariables.TableCategories;

                    cmd.CommandText =
                        $"CREATE TABLE IF NOT EXISTS {table_categories} (" +
                        "  id int NOT NULL," +
                        "  label varchar(255) NOT NULL," +
                        "  PRIMARY KEY (id)" +
                        ")";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = $"SELECT COUNT(*) FROM {table_categories}";
                    object result = cmd.ExecuteScalar();
                    if (!(result != null && Convert.ToInt32(result) > 0))
                    {
                        cmd.CommandText = $"INSERT INTO {table_categories} VALUES (@id, @label)";
                        cmd.Parameters.AddWithValue("@id", 1);
                        cmd.Parameters.AddWithValue("@label", "One");
                        cmd.Prepare();

                        for (int i = 0; i < categories.Count; i++)
                        {
                            cmd.Parameters["@id"].Value = i + 1;
                            cmd.Parameters["@label"].Value = categories[i];
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
    }
}