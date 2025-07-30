namespace ModestBlackwell.Models.GraphQL;

/// <summary>
/// Represents a stream with its operational data values
/// </summary>
public class StreamWithValues
{
    /// <summary>
    /// Stream identifier
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Stream name
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Asset identifier that this stream belongs to
    /// </summary>
    public string AssetId { get; set; } = string.Empty;

    /// <summary>
    /// Unit of measurement
    /// </summary>
    public string Uom { get; set; } = string.Empty;

    /// <summary>
    /// Operational data values for this stream
    /// </summary>
    public List<OperationalDataValue> Values { get; set; } = new();
}
