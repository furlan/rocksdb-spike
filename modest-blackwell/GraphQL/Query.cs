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
    /// <param name="id">Asset identifier</param>
    /// <param name="graphQLService">GraphQL service</param>
    /// <returns>Asset with operational data</returns>
    public async Task<AssetWithOperationalData?> GetAssetAsync(
        string id,
        [Service] IGraphQLService graphQLService)
    {
        return await graphQLService.GetAssetWithOperationalDataAsync(id);
    }

    /// <summary>
    /// Retrieves all assets with their operational data
    /// </summary>
    /// <param name="assetService">Asset service</param>
    /// <param name="graphQLService">GraphQL service</param>
    /// <returns>Collection of assets with operational data</returns>
    public async Task<IEnumerable<AssetWithOperationalData>> GetAssetsAsync(
        [Service] IAssetService assetService,
        [Service] IGraphQLService graphQLService)
    {
        var assets = await assetService.GetAllAssetsAsync();
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
