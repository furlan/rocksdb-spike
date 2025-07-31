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
    /// <param name="assetId">Asset identifier</param>
    /// <returns>Asset with operational data</returns>
    public async Task<AssetWithOperationalData?> GetAssetWithOperationalDataAsync(string assetId)
    {
        try
        {
            _logger.LogInformation("Retrieving asset with operational data for ID: {AssetId}", assetId);

            // Get asset information
            var asset = await _assetService.GetAssetByIdAsync(assetId);
            if (asset == null)
            {
                _logger.LogWarning("Asset with ID '{AssetId}' not found", assetId);
                return null;
            }

            // Get streams for the asset
            var streams = await _streamService.GetStreamsByAssetIdAsync(assetId);
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
                    
                    // Get operational data from RocksDB using the asset's operational type
                    // if (Enum.TryParse<OperationalTypeEnum>(asset.OperationalType, true, out var operationalTypeEnum))
                    // {
                        var operationalData = await _rocksDbService.GetOperationalDataAsync(
                            assetId, 
                            streamId,
                            stream.Type
                        );
                        streamWithValues.Values = operationalData.ToList();
                    // }
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

            _logger.LogInformation("Successfully retrieved asset with {StreamCount} streams and operational data", streamsWithValues.Count);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset with operational data for ID: {AssetId}", assetId);
            throw;
        }
    }
}
