namespace EdificeBlackwell.Models;

/// <summary>
/// Represents an operational data type configuration
/// </summary>
public class OperationalType
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> Synonyms { get; set; } = new List<string>();
}

/// <summary>
/// Root configuration for operational types
/// </summary>
public class OperationalTypesConfig
{
    public List<OperationalTypeWrapper> OperationalTypes { get; set; } = new List<OperationalTypeWrapper>();
}

/// <summary>
/// Wrapper for YAML structure
/// </summary>
public class OperationalTypeWrapper
{
    public OperationalType OperationalType { get; set; } = new OperationalType();
}
