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
}
