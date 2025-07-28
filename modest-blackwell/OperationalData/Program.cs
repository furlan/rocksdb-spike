using RocksDbSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OperationalData
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = Environment.ExpandEnvironmentVariables(Path.Combine("./data/rocksdb", "operational"));

            var options = new DbOptions()
                .SetCreateIfMissing(true)
                .SetCreateMissingColumnFamilies(true);

            // Create an array of column family names
            string[] cfNames = new string[] { "alarm", "notification", "utilization" };

            var columnFamilies = new ColumnFamilies
            {
                { "alarm", new ColumnFamilyOptions() },
                { "notification", new ColumnFamilyOptions() },
                { "utilization", new ColumnFamilyOptions() },
            };

            // Create the Column families
            foreach (var cfName in cfNames)
            {

                // Add operational data
                using (var db = RocksDb.Open(options, path, columnFamilies))
                {
                    var columnFamily = db.GetColumnFamily(cfName);

                    // Load data from file
                    string dataPath = Path.Combine("./load", cfName + "-data-load.txt");

                    if (File.Exists(dataPath))
                    {
                        Console.WriteLine($"Loading {cfName} data from: {dataPath}");

                        string[] lines = File.ReadAllLines(dataPath);
                        int loadedCount = 0;

                        foreach (string line in lines)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                // Parse key and value separated by ":"
                                string[] parts = line.Split(':', 2); // Split into maximum 2 parts

                                if (parts.Length == 2)
                                {
                                    string key = parts[0].Trim();
                                    string value = parts[1].Trim();

                                    // Put the key-value pair into the column family
                                    db.Put(key, value, cf: columnFamily);
                                    loadedCount++;

                                    Console.WriteLine($"Loaded: {key} -> {value}, on column family {cfName}");
                                }
                                else
                                {
                                    Console.WriteLine($"Warning: Invalid line format: {line}");
                                }
                            }
                        }

                        Console.WriteLine($"Successfully loaded {loadedCount} {cfName} records into RocksDB");

                        // Verify data was stored correctly by reading it back
                        Console.WriteLine("\nVerifying stored data:");
                        foreach (string line in lines)
                        {
                            if (!string.IsNullOrWhiteSpace(line))
                            {
                                string[] parts = line.Split(':', 2);
                                if (parts.Length == 2)
                                {
                                    string key = parts[0].Trim();
                                    string storedValue = db.Get(key, cf: columnFamily);
                                    Console.WriteLine($"Verified: {key} = {storedValue}");
                                }
                            }
                        }
                    }
                    else
                    {
                        Console.WriteLine($"Warning: {cfName} data file not found at: {dataPath}");
                    }
                }

            }

        }
    }
}
