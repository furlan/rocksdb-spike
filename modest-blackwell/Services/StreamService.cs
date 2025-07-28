using ModestBlackwell.Models;
using ModestBlackwell.Services.Interfaces;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ModestBlackwell.Services;

/// <summary>
/// Service implementation for managing data streams from YAML data source
/// </summary>
public class StreamService : IStreamService
{
    private readonly string _yamlFilePath;
    private readonly ILogger<StreamService> _logger;
    private readonly IDeserializer _yamlDeserializer;

    public StreamService(ILogger<StreamService> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _yamlFilePath = Path.Combine(
            configuration["DataPath"] ?? "data", 
            "yaml", 
            "streams.yaml"
        );
        
        _yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
    }

    /// <summary>
    /// Retrieves all data streams from the YAML data source
    /// </summary>
    /// <returns>Collection of data streams</returns>
    public async Task<IEnumerable<DataStream>> GetAllStreamsAsync()
    {
        try
        {
            _logger.LogInformation("Loading data streams from {FilePath}", _yamlFilePath);
            
            if (!File.Exists(_yamlFilePath))
            {
                _logger.LogWarning("Data streams file not found at {FilePath}", _yamlFilePath);
                return Enumerable.Empty<DataStream>();
            }

            var yamlContent = await File.ReadAllTextAsync(_yamlFilePath);
            var streams = ParseYamlToStreams(yamlContent);
            
            _logger.LogInformation("Successfully loaded {Count} data streams", streams.Count());
            return streams;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading data streams from {FilePath}", _yamlFilePath);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific data stream by its identifier
    /// </summary>
    /// <param name="id">Data stream identifier</param>
    /// <returns>Data stream if found, null otherwise</returns>
    public async Task<DataStream?> GetStreamByIdAsync(string id)
    {
        try
        {
            var streams = await GetAllStreamsAsync();
            var stream = streams.FirstOrDefault(s => s.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            
            if (stream == null)
            {
                _logger.LogWarning("Data stream with id '{Id}' not found", id);
            }
            
            return stream;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving data stream with id '{Id}'", id);
            throw;
        }
    }

    /// <summary>
    /// Retrieves all data streams that belong to a specific asset
    /// </summary>
    /// <param name="assetId">Asset identifier</param>
    /// <returns>Collection of data streams for the asset</returns>
    public async Task<IEnumerable<DataStream>> GetStreamsByAssetIdAsync(string assetId)
    {
        try
        {
            var streams = await GetAllStreamsAsync();
            var assetStreams = streams.Where(s => s.AssetId.Equals(assetId, StringComparison.OrdinalIgnoreCase));
            
            _logger.LogInformation("Found {Count} data streams for asset '{AssetId}'", assetStreams.Count(), assetId);
            return assetStreams;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving data streams for asset '{AssetId}'", assetId);
            throw;
        }
    }

    /// <summary>
    /// Parses YAML content to extract data streams
    /// </summary>
    /// <param name="yamlContent">Raw YAML content</param>
    /// <returns>Collection of data streams</returns>
    private IEnumerable<DataStream> ParseYamlToStreams(string yamlContent)
    {
        var streams = new List<DataStream>();
        
        // Split the YAML into documents (each starting with "stream:")
        var documents = yamlContent.Split(new[] { "\nstream:" }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var document in documents)
        {
            try
            {
                // Ensure each document starts with "stream:"
                var yamlDoc = document.TrimStart().StartsWith("stream:") ? document : "stream:" + document;
                
                // Parse the YAML document
                var streamData = _yamlDeserializer.Deserialize<Dictionary<string, object>>(yamlDoc);
                
                if (streamData.TryGetValue("stream", out var streamObj) && streamObj is Dictionary<object, object> streamDict)
                {
                    var stream = new DataStream
                    {
                        Id = GetStringValue(streamDict, "id"),
                        Name = GetStringValue(streamDict, "name"),
                        AssetId = GetStringValue(streamDict, "assetId"),
                        Uom = GetStringValue(streamDict, "uom")
                    };
                    
                    streams.Add(stream);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse data stream document: {Document}", document);
            }
        }
        
        return streams;
    }

    /// <summary>
    /// Safely extracts string value from dictionary
    /// </summary>
    /// <param name="dictionary">Source dictionary</param>
    /// <param name="key">Key to extract</param>
    /// <returns>String value or empty string if not found</returns>
    private static string GetStringValue(Dictionary<object, object> dictionary, string key)
    {
        return dictionary.TryGetValue(key, out var value) ? value?.ToString() ?? string.Empty : string.Empty;
    }
}
