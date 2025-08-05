using ModestBlackwell.Models.GraphQL;

namespace ModestBlackwell.Models.GraphQL;

/// <summary>
/// GraphQL response model for asset with operational data
/// </summary>
public class AssetWithOperationalData
{
    /// <summary>
    /// Asset information
    /// </summary>
    public Asset Asset { get; set; } = new();

    /// <summary>
    /// Operational type data associated with the asset
    /// </summary>
    public OperationalType Type { get; set; } = new();

    /// <summary>
    /// Gets operational type filtered by name
    /// </summary>
    /// <param name="name">Optional filter by operational type name</param>
    /// <returns>Filtered operational type</returns>
    public OperationalType GetType(string? name = null)
    {
        return Type.GetType(name);
    }
}
