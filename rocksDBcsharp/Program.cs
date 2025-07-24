using System.Text;
using RocksDbSharp;

Console.WriteLine("RocksDB Simple Example");

// Create or open a RocksDB database
var options = new DbOptions()
    .SetCreateIfMissing(true)
    .SetCompression(Compression.Snappy);

string dbPath = "my_rocksdb";

using var db = RocksDb.Open(options, dbPath);

Console.WriteLine("Database opened successfully!");

// Put some key-value pairs
Console.WriteLine("\n--- Storing data ---");
db.Put("name", "John Doe");
db.Put("age", "30");
db.Put("city", "New York");
db.Put("occupation", "Software Engineer");

Console.WriteLine("Data stored successfully!");

// Get values by keys
Console.WriteLine("\n--- Retrieving data ---");
string name = db.Get("name");
string age = db.Get("age");
string city = db.Get("city");
string occupation = db.Get("occupation");

Console.WriteLine($"Name: {name}");
Console.WriteLine($"Age: {age}");
Console.WriteLine($"City: {city}");
Console.WriteLine($"Occupation: {occupation}");

// Check if a key exists
Console.WriteLine("\n--- Key existence check ---");
string nonExistentKey = db.Get("salary");
Console.WriteLine($"Salary (non-existent): {(nonExistentKey ?? "Key not found")}");

// Iterate through all key-value pairs
Console.WriteLine("\n--- All stored data ---");
using var iterator = db.NewIterator();
iterator.SeekToFirst();

while (iterator.Valid())
{
    string key = Encoding.UTF8.GetString(iterator.Key());
    string value = Encoding.UTF8.GetString(iterator.Value());
    Console.WriteLine($"{key}: {value}");
    iterator.Next();
}

// Delete a key
Console.WriteLine("\n--- Deleting data ---");
db.Remove("age");
Console.WriteLine("Age key deleted");

// Try to get deleted key
string deletedAge = db.Get("age");
Console.WriteLine($"Age after deletion: {(deletedAge ?? "Key not found")}");

// Batch operations
Console.WriteLine("\n--- Batch operations ---");
using var batch = new WriteBatch();
batch.Put("batch_key1", "batch_value1");
batch.Put("batch_key2", "batch_value2");
batch.Put("batch_key3", "batch_value3");
db.Write(batch);

Console.WriteLine("Batch operations completed!");

// Display final state
Console.WriteLine("\n--- Final database state ---");
using var finalIterator = db.NewIterator();
finalIterator.SeekToFirst();

while (finalIterator.Valid())
{
    string key = Encoding.UTF8.GetString(finalIterator.Key());
    string value = Encoding.UTF8.GetString(finalIterator.Value());
    Console.WriteLine($"{key}: {value}");
    finalIterator.Next();
}

Console.WriteLine("\nRocksDB example completed!");
