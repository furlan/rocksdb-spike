using ModestBlackwell.Models;

namespace ModestBlackwell.Services.Interfaces;

/// <summary>
/// Service interface for managing data streams
/// </summary>
public interface IStreamService
{
    /// <summary>
    /// Retrieves all data streams from the YAML data source
    /// </summary>
    /// <returns>Collection of data streams</returns>
    Task<IEnumerable<DataStream>> GetAllStreamsAsync();

    /// <summary>
    /// Retrieves a specific data stream by its identifier
    /// </summary>
    /// <param name="id">Data stream identifier</param>
    /// <returns>Data stream if found, null otherwise</returns>
    Task<DataStream?> GetStreamByIdAsync(string id);

    /// <summary>
    /// Retrieves all data streams that belong to a specific asset
    /// </summary>
    /// <param name="assetId">Asset identifier</param>
    /// <returns>Collection of data streams for the asset</returns>
    Task<IEnumerable<DataStream>> GetStreamsByAssetIdAsync(string assetId);
}
