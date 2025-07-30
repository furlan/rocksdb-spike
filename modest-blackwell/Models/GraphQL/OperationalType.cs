namespace ModestBlackwell.Models.GraphQL;

/// <summary>
/// Represents operational type data for GraphQL
/// </summary>
public class OperationalType
{
    /// <summary>
    /// Name of the operational type (e.g., utilization, alarm, notification)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Streams associated with this operational type
    /// </summary>
    public List<StreamWithValues> Streams { get; set; } = new();
}
