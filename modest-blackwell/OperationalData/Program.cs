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
            var db = SeedSampleData(System.IO.Path.Combine("../data/rocksdb", "operational"));
            ReadAllLines(db, "utilization");
            ReadAllLines(db, "alarm");
            ReadAllLines(db, "notification");
        }

        static void ReadAllLines(RocksDb db, string columnFamilyName)
        {
            var cf = db.GetColumnFamily(columnFamilyName);
            var readOptions = new ReadOptions();
            using var iterator = db.NewIterator(readOptions: readOptions, cf: cf);
            iterator.SeekToFirst();

            Console.WriteLine(new string('=', 20));
            Console.WriteLine($"Key-value for column family '{columnFamilyName}':");
            Console.WriteLine(new string('-', 20));

            while (iterator.Valid())
            {
                string key = Encoding.UTF8.GetString(iterator.Key());
                string value = db.Get(key, cf: cf);
                Console.WriteLine($"{key}: {value}");
                iterator.Next();
            }
        }

        static RocksDb SeedSampleData(string dbPath)
        {
            string path = Environment.ExpandEnvironmentVariables(dbPath);

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

            var db = RocksDb.Open(options, path, columnFamilies);

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
            return db;
        }
    }
}
