using ModestBlackwell.Models.GraphQL;
using ModestBlackwell.Services.Interfaces;

namespace ModestBlackwell.GraphQL;

/// <summary>
/// GraphQL query root
/// </summary>
public class Query
{
    /// <summary>
    /// Retrieves asset with operational data
    /// </summary>
    /// <param name="graphQLService">GraphQL service</param>
    /// <param name="id">Asset identifier (optional)</param>
    /// <param name="location">Filter by asset location (optional)</param>
    /// <returns>Asset with operational data</returns>
    public async Task<AssetWithOperationalData?> GetAssetAsync(
        [Service] IGraphQLService graphQLService,
        string? id = null,
        string? location = null)
    {
        return await graphQLService.GetAssetWithOperationalDataAsync(id, location);
    }

    /// <summary>
    /// Retrieves all assets with their operational data
    /// </summary>
    /// <param name="assetService">Asset service</param>
    /// <param name="graphQLService">GraphQL service</param>
    /// <param name="location">Filter by asset location (optional)</param>
    /// <returns>Collection of assets with operational data</returns>
    public async Task<IEnumerable<AssetWithOperationalData>> GetAssetsAsync(
        [Service] IAssetService assetService,
        [Service] IGraphQLService graphQLService,
        string? location = null)
    {
        IEnumerable<Models.Asset> assets;
        
        if (!string.IsNullOrEmpty(location))
        {
            assets = await assetService.GetAssetsByLocationAsync(location);
        }
        else
        {
            assets = await assetService.GetAllAssetsAsync();
        }
        
        var results = new List<AssetWithOperationalData>();

        foreach (var asset in assets)
        {
            var assetWithData = await graphQLService.GetAssetWithOperationalDataAsync(asset.Id);
            if (assetWithData != null)
            {
                results.Add(assetWithData);
            }
        }

        return results;
    }
}
