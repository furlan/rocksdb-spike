namespace ModestBlackwell.Models;

/// <summary>
/// Defines the attributes of a data stream. A data stream is a measure of utilization of an asset.
/// </summary>
public class DataStream
{
    /// <summary>
    /// Unique identifier for the data stream
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable name of the data stream
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Asset identifier that this data stream belongs to
    /// </summary>
    public string AssetId { get; set; } = string.Empty;

    /// <summary>
    /// Unit of measurement for the data stream values
    /// </summary>
    public string Uom { get; set; } = string.Empty;

    /// <summary>
    /// Operational type
    /// </summary>
    public string Type { get; set; } = string.Empty;
}
