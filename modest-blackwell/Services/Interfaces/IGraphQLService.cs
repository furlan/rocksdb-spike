using ModestBlackwell.Models.GraphQL;

namespace ModestBlackwell.Services.Interfaces;

/// <summary>
/// Service interface for GraphQL operations
/// </summary>
public interface IGraphQLService
{
    /// <summary>
    /// Retrieves asset with operational data
    /// </summary>
    /// <param name="assetId">Asset identifier</param>
    /// <returns>Asset with operational data</returns>
    Task<AssetWithOperationalData?> GetAssetWithOperationalDataAsync(string assetId);
}
