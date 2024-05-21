using Money.Modules;
using MySql.Data.MySqlClient;

namespace Money.Tables
{
    public class LocationNamesTable
    {
        public class LocationnamesRecord
        {
            public string provider_name { get; set; }
            public string name { get; set; }

            public LocationnamesRecord(string provider_name, string name)
            {
                this.provider_name = provider_name;
                this.name = name;
            }
        }

        public static List<LocationnamesRecord> GetLocationNames(string? query="")
        {
            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    var table_locationLongToShortName = SystemVariables.TableLocationNames;
                    cmd.CommandText = $"SELECT * FROM {table_locationLongToShortName}";
                    if (!string.IsNullOrEmpty(query))
                        cmd.CommandText += $" WHERE provider_name = \"{query}\"";

                    var rdr = cmd.ExecuteReader();

                    var result = new List<LocationnamesRecord>();
                    while(rdr.Read())
                        result.Add(new LocationnamesRecord((string)rdr[0], (string)rdr[1]));

                    return result;
                }
            }
        }

        public static void InsertItems(List<LocationNameRecord> items)
        {
            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;
                    var table_locationNames = SystemVariables.TableLocationNames;

                    // for now this is just meant as a one time upload
                    cmd.CommandText = $"DELETE FROM {table_locationNames}";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = 
                        $"INSERT INTO {table_locationNames} " +
                        "  (provider_name, name) " +
                        "VALUES " +
                        "  (@provider_name, @name)";
                    cmd.Parameters.AddWithValue("@provider_name", "one");
                    cmd.Parameters.AddWithValue("@name", "one");
                    cmd.Prepare();

                    for(int i=0; i<items.Count; i++)
                    {
                        cmd.Parameters["@provider_name"].Value = items[i].ProviderName;
                        cmd.Parameters["@name"].Value = items[i].ShortName;
                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public static void Setup()
        {
            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand())
                {
                    cmd.Connection = conn;

                    var table_locationLongToShortName = SystemVariables.TableLocationNames;

                    cmd.CommandText =
                        $"CREATE TABLE IF NOT EXISTS {table_locationLongToShortName} (" +
                        "  provider_name varchar(255) NOT NULL," +
                        "  name varchar(255) DEFAULT NULL," +
                        "  PRIMARY KEY (provider_name)" +
                        ")";
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}