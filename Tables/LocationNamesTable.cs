using Money.Modules;
using MySql.Data.MySqlClient;
using Microsoft.VisualBasic.FileIO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Money.Tables
{
    public class LocationNamesTable
    {
        public class LocationNamesRecord
        {
            [JsonRequired]
            public string provider_name { get; set; }
            [JsonRequired]
            public string name { get; set; }

            public LocationNamesRecord(string provider_name, string name)
            {
                this.provider_name = provider_name;
                this.name = name;
            }
        }

        public static List<LocationNamesRecord> GetLocationNames(string? query = null)
        {
            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand("", conn))
                {
                    var table_locationLongToShortName = SystemVariables.TableLocationNames;
                    cmd.CommandText = $"SELECT * FROM {table_locationLongToShortName}";
                    if (!string.IsNullOrEmpty(query))
                        cmd.CommandText += $" WHERE provider_name = \"{query}\"";

                    var rdr = cmd.ExecuteReader();

                    var result = new List<LocationNamesRecord>();
                    while (rdr.Read())
                        result.Add(new LocationNamesRecord((string)rdr[0], (string)rdr[1]));

                    return result;
                }
            }
        }

        public static Dictionary<string, Dictionary<string, dynamic>> InsertItems(List<LocationNamesRecord> items, bool overwrite=false)
        {
            var result = new Dictionary<string, Dictionary<string, dynamic>>
            {
                { "items_added", new Dictionary<string, dynamic>
                    {
                        { "count", 0 },
                        { "items", new List<LocationNamesRecord>() }
                    }
                },
                {"items_overwritten", new Dictionary<string, dynamic>
                    {
                        { "count", 0 },
                        { "items", new List<LocationNamesRecord>() }
                    }
                },
                {"items_skipped", new Dictionary<string, dynamic>
                    {
                        { "count", 0 },
                        { "items", new List<LocationNamesRecord>() }
                    }
                }
            };

            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand("", conn))
                {
                    var table_locationNames = SystemVariables.TableLocationNames;

                    if (overwrite)
                        cmd.CommandText = "REPLACE ";
                    else
                        cmd.CommandText = "INSERT ";
                    cmd.CommandText +=
                        $"INTO {table_locationNames} " +
                        "  (provider_name, name) " +
                        "VALUES " +
                        "  (@provider_name, @name)";
                    cmd.Parameters.AddWithValue("@provider_name", "one");
                    cmd.Parameters.AddWithValue("@name", "one");
                    cmd.Prepare();

                    for (int i = 0; i < items.Count; i++)
                    {
                        cmd.Parameters["@provider_name"].Value = items[i].provider_name;
                        cmd.Parameters["@name"].Value = items[i].name;
                        try
                        {
                            int affected_rows = cmd.ExecuteNonQuery();
                            if (affected_rows == 1)
                            {
                                result["items_added"]["count"] += 1;
                                result["items_added"]["items"].Add(new LocationNamesRecord(items[i].provider_name, items[i].name));
                            }
                            else if (affected_rows == 2)
                            {
                                result["items_overwritten"]["count"] += 1;
                                result["items_overwritten"]["items"].Add(new LocationNamesRecord(items[i].provider_name, items[i].name));
                            }
                        }
                        catch (MySqlException ex)
                        {
                            // only care about SQL error code 1062 as it means a duplicate entry.
                            // https://dev.mysql.com/doc/mysql-errors/8.0/en/server-error-reference.html#error_er_dup_entry
                            if (ex.Number == 1062)
                            {
                                result["items_skipped"]["count"] += 1;
                                result["items_skipped"]["items"].Add(new LocationNamesRecord(items[i].provider_name, items[i].name));
                            }
                            else
                                throw;
                        }
                    }

                    return result;
                }
            }
        }

        public static async Task<Dictionary<string, Dictionary<string, dynamic>>> UploadData(Stream stream, string contentType, int contentLength, bool overwrite)
        {
            var result = new List<LocationNamesRecord>();
            switch (contentType)
            {
                case "text/csv":
                    StringReader sr = await HttpRequestTools.ReadBodyAsync(stream, contentLength);
                    using (TextFieldParser parser = new TextFieldParser(sr))
                    {
                        parser.TextFieldType = FieldType.Delimited;
                        parser.SetDelimiters(",");
                        // gets rid of headers
                        parser.ReadFields();

                        while (!parser.EndOfData)
                        {
                            string[]? columns = parser.ReadFields();
                            if (columns != null)
                                result.Add(new LocationNamesRecord(columns[0], columns[1]));
                        }
                    }
                    break;
                case "application/json":
                    List<LocationNamesRecord>? deserializedData;
                    /* When deserializing it will throw exceptions when an item
                     * is missing one of the requried parameters defined in by
                     * [JsonRequired]. Error handling should be handled when
                     * calling this function so the proper messaging can be returned.
                     */
                    deserializedData = await JsonSerializer.DeserializeAsync<List<LocationNamesRecord>>(stream);
                    if (deserializedData != null)
                        result.AddRange(deserializedData);
                    break;
            }

            return InsertItems(result, overwrite);
        }

        public static void Setup()
        {
            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand("", conn))
                {
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

        public static List<string> MissingLocationNames()
        {
            var result = new List<string>();
            using (var conn = DatabaseConnection.CreateConnection())
            {
                using (var cmd = new MySqlCommand("", conn))
                {
                    cmd.CommandText = 
                        "SELECT " +
                        "  DISTINCT location " +
                        "FROM " +
                        $"  {SystemVariables.TableExpenseItems} " +
                        "WHERE " +
                        "  location NOT IN (" +
                        "    SELECT " +
                        "      name " +
                        "    FROM " +
                        $"      {SystemVariables.TableLocationNames} " +
                        "  ) " +
                        "ORDER BY " +
                        "  location";
                    var rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                        result.Add((string)rdr[0]);
                }
            }
            return result;
        }
    }
}