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
        static protected string[] cfNames = new string[] { "alarm", "notification", "utilization" };
        static protected RocksDb? db;

        static void Main(string[] args)
        {
            OpenRocksDb(System.IO.Path.Combine("../data/rocksdb", "operational"));
            bool shouldSeedData = args.Length > 0 && 
                (args[0].Equals("seed", StringComparison.OrdinalIgnoreCase) || 
                 args[0].Equals("--seed", StringComparison.OrdinalIgnoreCase) ||
                 args[0].Equals("-s", StringComparison.OrdinalIgnoreCase));
            
            if (shouldSeedData)
            {
                Console.WriteLine("Seeding sample data...");
                SeedSampleData();
            }
            else
            {
                Console.WriteLine("Skipping seed. Use 'seed', '--seed', or '-s' argument to seed sample data.");
            }
            ReadPrefixLines("utilization", "NT01T02");
        }

        static void ReadPrefixLines(string columnFamilyName, string prefix)
        {
            var cf = db.GetColumnFamily(columnFamilyName);
            var readOptions = new ReadOptions();
            using var iterator = db.NewIterator(readOptions: readOptions, cf: cf);

            Console.WriteLine(new string('=', 20));
            Console.WriteLine($"Key-value for column family '{columnFamilyName}' and prefix '{prefix}':");
            Console.WriteLine(new string('-', 20));

            GC.Collect();
            GC.WaitForPendingFinalizers();
            var prefixBytes = Encoding.UTF8.GetBytes(prefix);
            iterator.Seek(prefixBytes);
            while (iterator.Valid())
            {
                // string key = Encoding.UTF8.GetString(iterator.Key());
                string key = iterator.StringKey();
                // if (key[..prefix.Length] != prefix) break;
                if (!iterator.StringKey().StartsWith(prefix))
                    break;

                // string value = db.Get(key, cf: cf);
                Console.WriteLine($"{key}: {iterator.StringValue()}");
                iterator.Next();
            }
            Console.WriteLine($"---End of prefix {prefix}---");
        }

        static void OpenRocksDb(string dbPath)
        {
            string path = Environment.ExpandEnvironmentVariables(dbPath);

            var options = new DbOptions()
                .SetCreateIfMissing(true)
                .SetCreateMissingColumnFamilies(true);

            var columnFamilies = new ColumnFamilies
            {
                { "alarm", new ColumnFamilyOptions() },
                { "notification", new ColumnFamilyOptions() },
                { "utilization", new ColumnFamilyOptions() },
            };

            db = RocksDb.Open(options, path, columnFamilies);
        }

        static void SeedSampleData()
        {

            // Create the Column families
            foreach (var cfName in cfNames)
            {

                // Add operational data
                var columnFamily = db.GetColumnFamily(cfName);

                // Load data from file
                string dataPath = System.IO.Path.Combine("./load", cfName + "-data-load.txt");

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
