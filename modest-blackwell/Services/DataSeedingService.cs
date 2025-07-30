using RocksDbSharp;
using System.Text;

namespace ModestBlackwell.Services;

/// <summary>
/// Service for seeding test data into RocksDB
/// </summary>
public class DataSeedingService
{
    private readonly ILogger<DataSeedingService> _logger;
    private readonly IConfiguration _configuration;

    public DataSeedingService(ILogger<DataSeedingService> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    /// <summary>
    /// Seeds test data into RocksDB for demonstration purposes
    /// </summary>
    public void SeedTestData()
    {
        var dbPath = System.IO.Path.Combine(
            _configuration["DataPath"] ?? "data",
            "rocksdb",
            "operational"
        );

        try
        {
            _logger.LogInformation("Seeding test data into RocksDB at {DbPath}", dbPath);

            // Ensure directory exists
            Directory.CreateDirectory(System.IO.Path.GetDirectoryName(dbPath) ?? dbPath);

            var options = new DbOptions()
                .SetCreateIfMissing(true)
                .SetCreateMissingColumnFamilies(true);

            var columnFamilies = new ColumnFamilies
            {
                { "utilization", new ColumnFamilyOptions() },
                { "alarm", new ColumnFamilyOptions() },
                { "notification", new ColumnFamilyOptions() }
            };

            using var db = RocksDb.Open(options, dbPath, columnFamilies);
            
            var utilizationCf = db.GetColumnFamily("utilization");
            var alarmCf = db.GetColumnFamily("alarm");
            var notificationCf = db.GetColumnFamily("notification");

            // Seed utilization data for NT01 (thermostat)
            var utilizationData = new[]
            {
                ("NT01T0220250725T103258Z", "75"),
                ("NT01T0220250725T112658Z", "74"),
                ("NT01T0220250725T123258Z", "73"),
                ("NT01T0220250725T132658Z", "74"),
                ("NT01T0120250725T103258Z", "72"),
                ("NT01T0120250725T112658Z", "73"),
                ("NT01O0120250725T103258Z", "1"),
                ("NT01O0120250725T112658Z", "0"),
            };

            foreach (var (key, value) in utilizationData)
            {
                db.Put(key, value, cf: utilizationCf);
                _logger.LogDebug("Added utilization data: {Key} -> {Value}", key, value);
            }

            // Seed alarm data for LB01 (light bulb)
            var alarmData = new[]
            {
                ("LB01O0120250725T134509Z", "1"),
                ("LB01O0120250723T123200Z", "1"),
                ("LB01O0120250724T090000Z", "0"),
                ("LB01O0120250724T180000Z", "1"),
            };

            foreach (var (key, value) in alarmData)
            {
                db.Put(key, value, cf: alarmCf);
                _logger.LogDebug("Added alarm data: {Key} -> {Value}", key, value);
            }

            // Seed notification data (example)
            var notificationData = new[]
            {
                ("NT01N0120250725T103258Z", "Temperature threshold exceeded"),
                ("LB01N0120250725T134509Z", "Light bulb malfunction detected"),
            };

            foreach (var (key, value) in notificationData)
            {
                db.Put(key, value, cf: notificationCf);
                _logger.LogDebug("Added notification data: {Key} -> {Value}", key, value);
            }

            _logger.LogInformation("Successfully seeded test data into RocksDB");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to seed test data into RocksDB");
            throw;
        }
    }
}
