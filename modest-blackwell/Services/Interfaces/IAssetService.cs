using ModestBlackwell.Models;

namespace ModestBlackwell.Services.Interfaces;

/// <summary>
/// Service interface for managing assets
/// </summary>
public interface IAssetService
{
    /// <summary>
    /// Retrieves all assets from the YAML data source
    /// </summary>
    /// <returns>Collection of assets</returns>
    Task<IEnumerable<Asset>> GetAllAssetsAsync();

    /// <summary>
    /// Retrieves a specific asset by its identifier
    /// </summary>
    /// <param name="id">Asset identifier</param>
    /// <returns>Asset if found, null otherwise</returns>
    Task<Asset?> GetAssetByIdAsync(string id);
}
