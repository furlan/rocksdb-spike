using EdificeBlackwell.Models;

namespace EdificeBlackwell.Services;

/// <summary>
/// Service for generating GraphQL queries based on extracted intent
/// </summary>
public class GraphQLQueryService
{
    /// <summary>
    /// Generate GraphQL query based on the extracted intent
    /// </summary>
    public string GenerateQuery(QueryIntent intent)
    {
        if (!intent.IsRelevant)
        {
            return "# Query not relevant for GraphQL generation";
        }

        // Build the asset filter
        var assetFilter = string.IsNullOrEmpty(intent.Location) 
            ? "" 
            : $"(location: \"{NormalizeLocation(intent.Location)}\")";

        // Build the type filter
        var typeFilter = string.IsNullOrEmpty(intent.OperationalDataType) 
            ? "" 
            : $"(name: \"{intent.OperationalDataType}\")";

        // Generate the complete GraphQL query
        var query = $@"query {{
  asset{assetFilter} {{
    type{typeFilter} {{
      name
      streams {{
        id
        name
        uom
        assetId
        values {{
          key
          value
        }}
      }}
    }}
  }}
}}";

        return query;
    }

    /// <summary>
    /// Normalize location string to proper case
    /// </summary>
    private static string NormalizeLocation(string location)
    {
        if (string.IsNullOrWhiteSpace(location))
            return string.Empty;

        // Convert to Title Case for consistency
        var normalized = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ToTitleCase(location.ToLower());
        return normalized;
    }

    /// <summary>
    /// Format the GraphQL query for better readability
    /// </summary>
    public static string FormatQuery(string query)
    {
        if (string.IsNullOrWhiteSpace(query))
            return string.Empty;

        var lines = query.Split('\n');
        var formattedLines = new List<string>();
        var indentLevel = 0;

        foreach (var line in lines)
        {
            var trimmedLine = line.Trim();
            
            if (string.IsNullOrEmpty(trimmedLine))
                continue;

            // Decrease indent for closing braces
            if (trimmedLine.Contains('}'))
                indentLevel = Math.Max(0, indentLevel - 1);

            // Add the line with proper indentation
            formattedLines.Add(new string(' ', indentLevel * 2) + trimmedLine);

            // Increase indent for opening braces
            if (trimmedLine.Contains('{'))
                indentLevel++;
        }

        return string.Join('\n', formattedLines);
    }
}
