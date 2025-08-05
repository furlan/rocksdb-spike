using ModestBlackwell.Models;
using ModestBlackwell.Models.GraphQL;
using ModestBlackwell.Services.Interfaces;

namespace ModestBlackwell.Services;

/// <summary>
/// Service implementation for GraphQL operations
/// </summary>
public class GraphQLService : IGraphQLService
{
    private readonly IAssetService _assetService;
    private readonly IStreamService _streamService;
    private readonly IRocksDbService _rocksDbService;
    private readonly ILogger<GraphQLService> _logger;

    public GraphQLService(
        IAssetService assetService,
        IStreamService streamService,
        IRocksDbService rocksDbService,
        ILogger<GraphQLService> logger)
    {
        _assetService = assetService ?? throw new ArgumentNullException(nameof(assetService));
        _streamService = streamService ?? throw new ArgumentNullException(nameof(streamService));
        _rocksDbService = rocksDbService ?? throw new ArgumentNullException(nameof(rocksDbService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves asset with operational data
    /// </summary>
    /// <param name="assetId">Asset identifier (optional)</param>
    /// <param name="location">Filter by asset location (optional)</param>
    /// <returns>Asset with operational data</returns>
    public async Task<AssetWithOperationalData?> GetAssetWithOperationalDataAsync(string? assetId = null, string? location = null)
    {
        try
        {
            _logger.LogInformation("Retrieving asset with operational data - AssetId: {AssetId}, Location: {Location}", 
                assetId ?? "null", location ?? "null");

            Asset? asset = null;

            // Get asset based on provided filters
            if (!string.IsNullOrEmpty(assetId))
            {
                asset = await _assetService.GetAssetByIdAsync(assetId);
                if (asset == null)
                {
                    _logger.LogWarning("Asset with ID '{AssetId}' not found", assetId);
                    return null;
                }

                // If location filter is also provided, check if asset matches
                if (!string.IsNullOrEmpty(location) && 
                    !asset.Location.Equals(location, StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogWarning("Asset '{AssetId}' does not match location filter '{Location}'", assetId, location);
                    return null;
                }
            }
            else if (!string.IsNullOrEmpty(location))
            {
                // Get first asset by location if no specific asset ID provided
                var assetsByLocation = await _assetService.GetAssetsByLocationAsync(location);
                asset = assetsByLocation.FirstOrDefault();
                
                if (asset == null)
                {
                    _logger.LogWarning("No assets found for location '{Location}'", location);
                    return null;
                }
            }
            else
            {
                // If no filters provided, get first available asset
                var allAssets = await _assetService.GetAllAssetsAsync();
                asset = allAssets.FirstOrDefault();
                
                if (asset == null)
                {
                    _logger.LogWarning("No assets found");
                    return null;
                }
            }

            // Get streams for the asset
            var streams = await _streamService.GetStreamsByAssetIdAsync(asset.Id);
            var streamsWithValues = new List<StreamWithValues>();

            // For each stream, get operational data from RocksDB
            foreach (var stream in streams)
            {
                var streamWithValues = new StreamWithValues
                {
                    Id = stream.Id,
                    Name = stream.Name,
                    AssetId = stream.AssetId,
                    Uom = stream.Uom,
                    Type = stream.Type
                };

                // Extract stream ID from the full stream ID (e.g., "NT01.T02" -> "T02")
                var streamIdParts = stream.Id.Split('.');
                if (streamIdParts.Length >= 2)
                {
                    var streamId = streamIdParts[1];
                    
                    // Get operational data from RocksDB using the stream's type
                    var operationalData = await _rocksDbService.GetOperationalDataAsync(
                        asset.Id, 
                        streamId,
                        stream.Type
                    );
                    streamWithValues.Values = operationalData.ToList();
                }

                streamsWithValues.Add(streamWithValues);
            }

            var result = new AssetWithOperationalData
            {
                Asset = asset,
                Type = new OperationalType
                {
                    Name = asset.OperationalType,
                    Streams = streamsWithValues
                }
            };

            _logger.LogInformation("Successfully retrieved asset '{AssetId}' with {StreamCount} streams and operational data", 
                asset.Id, streamsWithValues.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset with operational data - AssetId: {AssetId}, Location: {Location}", 
                assetId ?? "null", location ?? "null");
            throw;
        }
    }
}
