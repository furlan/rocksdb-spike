using ModestBlackwell.Models;
using ModestBlackwell.Services.Interfaces;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace ModestBlackwell.Services;

/// <summary>
/// Service implementation for managing assets from YAML data source
/// </summary>
public class AssetService : IAssetService
{
    private readonly string _yamlFilePath;
    private readonly ILogger<AssetService> _logger;
    private readonly IDeserializer _yamlDeserializer;

    public AssetService(ILogger<AssetService> logger, IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _yamlFilePath = Path.Combine(
            configuration["DataPath"] ?? "data", 
            "yaml", 
            "assets.yaml"
        );
        
        _yamlDeserializer = new DeserializerBuilder()
            .WithNamingConvention(CamelCaseNamingConvention.Instance)
            .Build();
    }

    /// <summary>
    /// Retrieves all assets from the YAML data source
    /// </summary>
    /// <returns>Collection of assets</returns>
    public async Task<IEnumerable<Asset>> GetAllAssetsAsync()
    {
        try
        {
            _logger.LogInformation("Loading assets from {FilePath}", _yamlFilePath);
            
            if (!File.Exists(_yamlFilePath))
            {
                _logger.LogWarning("Assets file not found at {FilePath}", _yamlFilePath);
                return Enumerable.Empty<Asset>();
            }

            var yamlContent = await File.ReadAllTextAsync(_yamlFilePath);
            var assets = ParseYamlToAssets(yamlContent);
            
            _logger.LogInformation("Successfully loaded {Count} assets", assets.Count());
            return assets;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error loading assets from {FilePath}", _yamlFilePath);
            throw;
        }
    }

    /// <summary>
    /// Retrieves a specific asset by its identifier
    /// </summary>
    /// <param name="id">Asset identifier</param>
    /// <returns>Asset if found, null otherwise</returns>
    public async Task<Asset?> GetAssetByIdAsync(string id)
    {
        try
        {
            var assets = await GetAllAssetsAsync();
            var asset = assets.FirstOrDefault(a => a.Id.Equals(id, StringComparison.OrdinalIgnoreCase));
            
            if (asset == null)
            {
                _logger.LogWarning("Asset with id '{Id}' not found", id);
            }
            
            return asset;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset with id '{Id}'", id);
            throw;
        }
    }

    /// <summary>
    /// Parses YAML content to extract assets
    /// </summary>
    /// <param name="yamlContent">Raw YAML content</param>
    /// <returns>Collection of assets</returns>
    private IEnumerable<Asset> ParseYamlToAssets(string yamlContent)
    {
        var assets = new List<Asset>();
        
        // Split the YAML into documents (each starting with "asset:")
        var documents = yamlContent.Split(new[] { "\nasset:" }, StringSplitOptions.RemoveEmptyEntries);
        
        foreach (var document in documents)
        {
            try
            {
                // Ensure each document starts with "asset:"
                var yamlDoc = document.TrimStart().StartsWith("asset:") ? document : "asset:" + document;
                
                // Parse the YAML document
                var assetData = _yamlDeserializer.Deserialize<Dictionary<string, object>>(yamlDoc);
                
                if (assetData.TryGetValue("asset", out var assetObj) && assetObj is Dictionary<object, object> assetDict)
                {
                    var asset = new Asset
                    {
                        Id = GetStringValue(assetDict, "id"),
                        Name = GetStringValue(assetDict, "name"),
                        Location = GetStringValue(assetDict, "location") ?? GetStringValue(assetDict, "Location"), // Handle case sensitivity
                        Type = GetStringValue(assetDict, "type"),
                        Class = GetStringValue(assetDict, "class"),
                        Parent = GetStringValue(assetDict, "parent")
                    };
                    
                    assets.Add(asset);
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to parse asset document: {Document}", document);
            }
        }
        
        return assets;
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
