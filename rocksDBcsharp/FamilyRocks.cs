using System;
using System.ComponentModel;
using RocksDbSharp;
using System.Text;

namespace rocksDBcsharp;

public class FamilyRocks
{
    protected RocksDb rocksDb;

    public FamilyRocks(string dbName)
    {
        // Create or open a RocksDB database
        var options = new DbOptions()
            .SetCreateIfMissing(true)
            .SetCompression(Compression.Snappy);

        string temp = Path.GetTempPath();
        string path = Environment.ExpandEnvironmentVariables(Path.Combine(temp, dbName));
        Console.WriteLine($"RocksDb data path: {path}");
        rocksDb = RocksDb.Open(options, path);
    }
    public string AddPerson(Person person)
    {
        // Create a JSON object with the person's data
        var personData = new
        {
            name = person.name,
            sex = person.sex,
            parent = person.parent,
            spouse = person.spouse
        };

        // Serialize the object to a JSON string
        string personJson = System.Text.Json.JsonSerializer.Serialize(personData);

        rocksDb.Put(person.name, personJson);
        Console.WriteLine($"Added: {personData.name} ({personData.sex})");

        return person.name;
    }

    public Person? GetPerson(string name)
    {
        string? data = rocksDb.Get(name);
        return data != null ? System.Text.Json.JsonSerializer.Deserialize<Person>(data) : null;
    }

    public void SetPerson(object person)
    {
        // Convert to JsonElement to access the properties dynamically
        var jsonElement = System.Text.Json.JsonSerializer.SerializeToElement(person);

        // Extract the name to use as key
        string key = jsonElement.TryGetProperty("name", out var nameProperty) && nameProperty.ValueKind == System.Text.Json.JsonValueKind.String
            ? nameProperty.GetString() ?? "unknown"
            : "unknown";

        // Serialize the object to store in the database
        var personData = System.Text.Json.JsonSerializer.Serialize(person);

        // Store with name as the key
        rocksDb.Put(key, personData);
    }

    public void ShowPerson(string name)
    {
        var person = GetPerson(name);
        if (person != null)
        {
            Console.WriteLine($"Name: {person.name}");
            Console.WriteLine($"Sex: {person.sex}");
            Console.WriteLine($"Parent: {(person.parent == "" ? "None" : person.parent)}");
            Console.WriteLine($"Spouse: {(person.spouse == "" ? "None" : person.spouse)}");
            Console.WriteLine(new string('-', 20));
        }
    }

    public void DeletePerson(string name)
    {
        // Delete a key
        Console.WriteLine("\n--- Deleting data ---");
        rocksDb.Remove(name);

    }

    public void ShowAll()
    {
        // Iterate through all key-value pairs
        Console.WriteLine("\n--- All stored data ---");
        using var iterator = rocksDb.NewIterator();
        iterator.SeekToFirst();

        while (iterator.Valid())
        {
            string name = Encoding.UTF8.GetString(iterator.Key());
            ShowPerson(name);

            iterator.Next();
        }
    }

    public List<string> FindAllMales()
    {
        List<string> males = new List<string> { };

        // Iterate through all key-value pairs
        using var iterator = rocksDb.NewIterator();
        iterator.SeekToFirst();

        while (iterator.Valid())
        {
            string name = Encoding.UTF8.GetString(iterator.Key());
            var person = GetPerson(name);
            if (person != null)
            {
                if (person.sex == "Male")
                {
                    males.Add(person.name);
                }
            }

            iterator.Next();
        }
        return males;
    }

    public List<string> FindAllChildren(string parent_name)
    {
        List<string> children = new List<string> { };

        // Iterate through all key-value pairs
        using var iterator = rocksDb.NewIterator();
        iterator.SeekToFirst();

        while (iterator.Valid())
        {
            string name = Encoding.UTF8.GetString(iterator.Key());
            var person = GetPerson(name);
            if (person != null)
            {
                if (person.parent == parent_name)
                {
                    children.Add(person.name);
                }
            }

            iterator.Next();
        }
        return children;
    }

    public void ShowRawData()
    {
        // Iterate through all key-value pairs
        using var iterator = rocksDb.NewIterator();
        iterator.SeekToFirst();

        while (iterator.Valid())
        {
            string name = Encoding.UTF8.GetString(iterator.Key());
            string value = Encoding.UTF8.GetString(iterator.Value());
            Console.WriteLine($"{name}: {value}");
            iterator.Next();
        }
    }
}
