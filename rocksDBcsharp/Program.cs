using System.Text;
using rocksDBcsharp;
using RocksDbSharp;

Console.WriteLine("RocksDB Family Tree Example");

var family = new FamilyRocks("family");

// Add parents
var john = new Person("John", "Male", "", "");
var alice = new Person("Alice", "Female", "", "");
family.AddPerson(john);
family.AddPerson(alice);

// Update John's spouse
john = family.GetPerson("John");
john.spouse = "Alice";
family.SetPerson(john);

// Add children of John and Alice
var david = new Person("David", "Male", "John", "");
var joe = new Person("Joe", "Male", "John", "");
family.AddPerson(david);
family.AddPerson(joe);

//Add David's wife
var sarah = new Person("Sarah", "Female", "", "David");
family.AddPerson(sarah);
david.spouse = "Sarah";
family.SetPerson(david);

// Add David and Sarah's child
var michael = new Person("Michael", "Male", "David", "");

// View Family Tree
family.ShowAll();

// Simple queries
var males = family.FindAllMales();
Console.WriteLine("\n--- All Males ---");
foreach (var male in males)
{
    Console.WriteLine($"Name: {male}");
}

var johnChildren = family.FindAllChildren("John");
Console.WriteLine("\n--- John's children ---");
foreach (var child in johnChildren)
{
    Console.WriteLine($"Child name: {child}");
}

var davidChildren = family.FindAllChildren("David");
Console.WriteLine("\n--- David's children ---");
foreach (var child in davidChildren)
{
    Console.WriteLine($"Child name: {child}");
}

// Display final state
Console.WriteLine("\n--- Final database state ---");
family.ShowRawData();

Console.WriteLine("\nRocksDB example completed!");
