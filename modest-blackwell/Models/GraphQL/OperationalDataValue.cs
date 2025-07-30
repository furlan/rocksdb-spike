namespace ModestBlackwell.Models.GraphQL;

/// <summary>
/// Represents a key-value pair for operational data from RocksDB
/// </summary>
public class OperationalDataValue
{
    /// <summary>
    /// The key from RocksDB (e.g., timestamp-based key)
    /// </summary>
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// The value associated with the key
    /// </summary>
    public string Value { get; set; } = string.Empty;
}
