using ModestBlackwell.Models.GraphQL;

namespace ModestBlackwell.Services.Interfaces;

/// <summary>
/// Service interface for RocksDB operational data operations
/// </summary>
public interface IRocksDbService
{
    /// <summary>
    /// Retrieves operational data for a specific stream using prefix iterator
    /// </summary>
    /// <param name="assetId">Asset identifier</param>
    /// <param name="streamId">Stream identifier</param>
    /// <param name="operationalType">Type of operational data (utilization, alarm, notification)</param>
    /// <returns>Collection of operational data values</returns>
    Task<IEnumerable<OperationalDataValue>> GetOperationalDataAsync(string assetId, string streamId, OperationalTypeEnum operationalType);

    /// <summary>
    /// Initializes the RocksDB database with column families
    /// </summary>
    Task InitializeDatabaseAsync();
}
