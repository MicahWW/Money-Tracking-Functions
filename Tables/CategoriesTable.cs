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

        public static List<CategoriesRecord> GetCategories()
        {
            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand("", conn))
                {
                    cmd.CommandText = $"SELECT * FROM {SystemVariables.TableCategories}";

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
                }
            }
        }
    }
}