using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;
using EdificeBlackwell.Models;

namespace EdificeBlackwell.Services;

/// <summary>
/// Service for loading and managing operational data types from configuration
/// </summary>
public class OperationalDataService
{
    private readonly List<OperationalType> _operationalTypes;
    private const string OPERATIONAL_DATA_FILE = "operationals.yaml";

    public OperationalDataService()
    {
        _operationalTypes = LoadOperationalTypes();
    }

    /// <summary>
    /// Get all available operational data types
    /// </summary>
    public List<OperationalType> GetOperationalTypes()
    {
        return _operationalTypes.ToList();
    }

    /// <summary>
    /// Get operational type names as a formatted string for prompts
    /// </summary>
    public string GetOperationalTypesForPrompt()
    {
        var result = new List<string>();
        
        foreach (var opType in _operationalTypes)
        {
            var synonymsText = opType.Synonyms.Count > 0 
                ? $" (synonyms: {string.Join(", ", opType.Synonyms)})" 
                : "";
            
            result.Add($"- \"{opType.Name}\" - {opType.Description}{synonymsText}");
        }
        
        return string.Join("\n     ", result);
    }

    /// <summary>
    /// Find operational type by name or synonym
    /// </summary>
    public OperationalType? FindOperationalType(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return null;

        var normalizedInput = input.Trim().ToLowerInvariant();

        // First, try exact match by name
        var exactMatch = _operationalTypes.FirstOrDefault(ot => 
            ot.Name.Equals(normalizedInput, StringComparison.OrdinalIgnoreCase));
        
        if (exactMatch != null)
            return exactMatch;

        // Then try synonym match
        return _operationalTypes.FirstOrDefault(ot => 
            ot.Synonyms.Any(synonym => 
                synonym.Equals(normalizedInput, StringComparison.OrdinalIgnoreCase)));
    }

    /// <summary>
    /// Load operational types from YAML configuration file
    /// </summary>
    private List<OperationalType> LoadOperationalTypes()
    {
        try
        {
            var dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", OPERATIONAL_DATA_FILE);
            
            if (!File.Exists(dataPath))
            {
                throw new FileNotFoundException($"Operational data file not found at: {dataPath}");
            }

            var yamlContent = File.ReadAllText(dataPath);
            
            var deserializer = new DeserializerBuilder()
                .WithNamingConvention(UnderscoredNamingConvention.Instance)
                .Build();

            var config = deserializer.Deserialize<OperationalTypesConfig>(yamlContent);
            
            return config.OperationalTypes
                .Select(wrapper => wrapper.OperationalType)
                .ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading operational data types: {ex.Message}");
            
            // Return default operational types as fallback
            return GetDefaultOperationalTypes();
        }
    }

    /// <summary>
    /// Get default operational types if configuration file is not available
    /// </summary>
    private List<OperationalType> GetDefaultOperationalTypes()
    {
        return new List<OperationalType>
        {
            new OperationalType
            {
                Id = "01",
                Name = "notification",
                Description = "Messages, informational alerts, and general notifications",
                Synonyms = new List<string> { "message", "info", "notice", "alert" }
            },
            new OperationalType
            {
                Id = "02",
                Name = "alarm",
                Description = "Critical alerts, warnings, and emergency notifications",
                Synonyms = new List<string> { "alert", "warning", "critical", "emergency" }
            },
            new OperationalType
            {
                Id = "03",
                Name = "utilization",
                Description = "Sensor readings, measurements, temperature data, and usage information",
                Synonyms = new List<string> { "sensor", "reading", "temperature", "measurement", "usage", "data", "sensors readings", "actual temperature" }
            }
        };
    }
}
