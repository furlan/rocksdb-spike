namespace WBStrucOut.Models;

public class EmployeeQuery
{
    public string Operation { get; set; }
    public string Entity { get; set; }
    public Dictionary<string, Dictionary<string, string>> Filters { get; set; }
}
