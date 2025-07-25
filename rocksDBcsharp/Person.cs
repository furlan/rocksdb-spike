using System;

namespace rocksDBcsharp;

public class Person
{
    public string name { get; set; }
    public string sex { get; set; }
    public string parent { get; set; }
    public string spouse { get; set; }

    public Person(string name, string sex, string parent, string spouse)
    {
        this.name = name;
        this.sex = sex;
        this.parent = parent;
        this.spouse = spouse;
    }
}
