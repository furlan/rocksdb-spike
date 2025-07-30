using ModestBlackwell.Models.GraphQL;
using ModestBlackwell.Services.Interfaces;
using RocksDbSharp;
using System.Text;

namespace ModestBlackwell.Services;

/// <summary>
/// Service implementation for RocksDB operational data operations
/// </summary>
public class RocksDbService : IRocksDbService, IDisposable
{
    private readonly string _dbPath;
    private readonly ILogger<RocksDbService> _logger;
    private RocksDb? _db;
    private readonly Dictionary<string, ColumnFamilyHandle> _columnFamilies = new();
    private readonly object _lock = new();
    private bool _initialized = false;
    private bool _disposed = false;

    public RocksDbService(ILogger<RocksDbService> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _dbPath = System.IO.Path.Combine(
            configuration["DataPath"] ?? "data", 
            "rocksdb", 
            "operational"
        );
    }

    /// <summary>
    /// Initializes the RocksDB database with column families
    /// </summary>
    public Task InitializeDatabaseAsync()
    {
        try
        {
            _logger.LogInformation("Initializing RocksDB at {Path}", _dbPath);
            
            Directory.CreateDirectory(_dbPath);
            
            // First, try to open the database with just the default column family
            // to check if it exists and create it if necessary
            var simpleOptions = new DbOptions().SetCreateIfMissing(true);
            
            // Check if the database already exists
            bool dbExists = Directory.Exists(_dbPath) && Directory.GetFiles(_dbPath).Length > 0;
            
            if (!dbExists)
            {
                // Create the database with just the default column family first
                using (var tempDb = RocksDb.Open(simpleOptions, _dbPath))
                {
                    _logger.LogInformation("Created new RocksDB database");
                }
            }
            
            // Now try to open with all column families
            var options = new DbOptions()
                .SetCreateIfMissing(true)
                .SetCreateMissingColumnFamilies(true);
                
            var columnFamilies = new ColumnFamilies
            {
                //{ "default", new ColumnFamilyOptions() },
                { "alarm", new ColumnFamilyOptions() },
                { "notification", new ColumnFamilyOptions() },
                { "utilization", new ColumnFamilyOptions() }
            };

            _db = RocksDb.Open(options, _dbPath, columnFamilies);
            
            _columnFamilies["alarm"] = _db.GetColumnFamily("alarm");
            _columnFamilies["notification"] = _db.GetColumnFamily("notification");
            _columnFamilies["utilization"] = _db.GetColumnFamily("utilization");
            
            _initialized = true;
            _logger.LogInformation("RocksDB initialized successfully");
            
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to initialize RocksDB at {Path}", _dbPath);
            throw;
        }
    }

    /// <summary>
    /// Retrieves operational data for a specific asset and stream
    /// </summary>
    public async Task<IEnumerable<OperationalDataValue>> GetOperationalDataAsync(string assetId, string streamId, OperationalTypeEnum operationalType)
    {
        if (!_initialized || _db == null)
        {
            _logger.LogWarning("RocksDB not initialized - returning empty operational data for asset {AssetId}, stream {StreamId}", assetId, streamId);
            return new List<OperationalDataValue>();
        }

        return await Task.Run(() =>
        {
            try
            {
                var columnFamilyName = operationalType.ToString().ToLowerInvariant();
                if (!_columnFamilies.TryGetValue(columnFamilyName, out var columnFamily))
                {
                    _logger.LogWarning("Column family {ColumnFamily} not found for operational type {OperationalType}", columnFamilyName, operationalType);
                    return new List<OperationalDataValue>();
                }

                var results = new List<OperationalDataValue>();
                var keyPrefix = $"{assetId}{streamId}";
                
                // Use iterator to scan for keys with the specified prefix
                var readOptions = new ReadOptions();
                using var iterator = _db.NewIterator(readOptions: readOptions, cf: columnFamily);
                var prefixBytes = Encoding.UTF8.GetBytes(keyPrefix);
                iterator.Seek(prefixBytes);
                var cf = _db.GetColumnFamily("utilization");
                using var i = _db.NewIterator(readOptions: readOptions, cf: cf);
                i.SeekToFirst();
                Console.WriteLine(_db.Get("NT01T0220250725T103258Z", cf:cf));

                while (i.Valid())
                {
                    Console.WriteLine(i.StringKey());
                    i.Next();
                }
                
                while (iterator.Valid())
                {
                    var keyBytes = iterator.Key();
                    var keyString = Encoding.UTF8.GetString(keyBytes);

                    // Check if the key starts with our prefix
                    if (!keyString.StartsWith(keyPrefix))
                        break;

                    var valueBytes = iterator.Value();
                    var valueString = Encoding.UTF8.GetString(valueBytes);

                    // Parse timestamp from key (assuming format: assetId.streamId.timestamp)
                    var keyParts = keyString.Split('.');
                    if (keyParts.Length >= 3)
                    {
                        results.Add(new OperationalDataValue
                        {
                            Key = keyParts.Length >= 3 ? keyParts[2] : keyString, // Use timestamp part or full key
                            Value = valueString
                        });
                    }

                    iterator.Next();
                }
                
                return results.OrderBy(x => x.Key).AsEnumerable();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to retrieve operational data for asset {AssetId}, stream {StreamId}", assetId, streamId);
                return new List<OperationalDataValue>();
            }
        });
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _db?.Dispose();
            // Note: ColumnFamilyHandle doesn't need explicit disposal in RocksDbSharp
            _columnFamilies.Clear();
            _disposed = true;
            _logger.LogInformation("RocksDB service disposed");
        }
    }
}
