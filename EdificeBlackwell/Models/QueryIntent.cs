using System.Text.Json.Serialization;

namespace EdificeBlackwell.Models;

/// <summary>
/// Represents the extracted intent from user query for GraphQL generation
/// </summary>
public class QueryIntent
{
    /// <summary>
    /// The location filter extracted from the user prompt (e.g., "living room", "kitchen")
    /// </summary>
    [JsonPropertyName("location")]
    public string? Location { get; set; }

    /// <summary>
    /// The operational data type filter extracted from the user prompt (e.g., "alarm", "notification", "utilization")
    /// </summary>
    [JsonPropertyName("operationalDataType")]
    public string? OperationalDataType { get; set; }

    /// <summary>
    /// Indicates if the query contains relevant information for GraphQL generation
    /// </summary>
    [JsonPropertyName("isRelevant")]
    public bool IsRelevant { get; set; } = true;

    /// <summary>
    /// Additional context or explanation about the extracted intent
    /// </summary>
    [JsonPropertyName("context")]
    public string? Context { get; set; }
}
