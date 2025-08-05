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

    /// <summary>
    /// Gets filtered operational type by name
    /// </summary>
    /// <param name="name">Optional filter by operational type name</param>
    /// <returns>Filtered operational type or current instance if no filter</returns>
    public OperationalType GetType(string? name = null)
    {
        if (string.IsNullOrEmpty(name))
        {
            return this;
        }

        // Filter streams by the requested operational type
        var filteredStreams = Streams.Where(s => 
            s.Type.Equals(name, StringComparison.OrdinalIgnoreCase)).ToList();

        return new OperationalType
        {
            Name = name,
            Streams = filteredStreams
        };
    }
}
