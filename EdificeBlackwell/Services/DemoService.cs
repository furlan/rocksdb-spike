using EdificeBlackwell.Services;
using EdificeBlackwell.Models;

namespace EdificeBlackwell;

/// <summary>
/// Demo class to showcase the GraphQL query generation functionality
/// without requiring Azure AI credentials
/// </summary>
public class DemoService
{
    private readonly GraphQLQueryService _graphQLService;

    public DemoService()
    {
        _graphQLService = new GraphQLQueryService();
    }

    /// <summary>
    /// Run demo scenarios showing how the application works
    /// </summary>
    public void RunDemo()
    {
        Console.WriteLine("=== EdificeBlackwell Demo Mode ===");
        Console.WriteLine("Demonstrating GraphQL query generation with sample intents");
        Console.WriteLine();

        var demoScenarios = new[]
        {
            new
            {
                Query = "List the alarms for living room",
                Intent = new QueryIntent
                {
                    Location = "living room",
                    OperationalDataType = "alarm",
                    IsRelevant = true,
                    Context = "User wants to see alarm data for living room"
                }
            },
            new
            {
                Query = "Show me temperature sensors in the kitchen",
                Intent = new QueryIntent
                {
                    Location = "kitchen",
                    OperationalDataType = "utilization",
                    IsRelevant = true,
                    Context = "User wants to see temperature sensor readings for kitchen"
                }
            },
            new
            {
                Query = "What notifications do I have?",
                Intent = new QueryIntent
                {
                    Location = null,
                    OperationalDataType = "notification",
                    IsRelevant = true,
                    Context = "User wants to see all notifications across all locations"
                }
            },
            new
            {
                Query = "Show me sensor readings for bathroom",
                Intent = new QueryIntent
                {
                    Location = "bathroom",
                    OperationalDataType = "utilization",
                    IsRelevant = true,
                    Context = "User wants to see utilization data for bathroom"
                }
            },
            new
            {
                Query = "What's the weather like today?",
                Intent = new QueryIntent
                {
                    Location = null,
                    OperationalDataType = null,
                    IsRelevant = false,
                    Context = "Query not related to home automation data"
                }
            }
        };

        foreach (var scenario in demoScenarios)
        {
            ProcessDemoScenario(scenario.Query, scenario.Intent);
            Console.WriteLine();
        }

        Console.WriteLine("Demo completed! In production mode, these intents would be extracted using Azure AI.");
    }

    private void ProcessDemoScenario(string userQuery, QueryIntent intent)
    {
        Console.WriteLine($"🔤 User Query: '{userQuery}'");
        Console.WriteLine();

        Console.WriteLine("📋 Extracted Intent:");
        Console.WriteLine($"  Location: {intent.Location ?? "Not specified"}");
        Console.WriteLine($"  Operational Data Type: {intent.OperationalDataType ?? "Not specified"}");
        Console.WriteLine($"  Relevant: {intent.IsRelevant}");
        Console.WriteLine($"  Context: {intent.Context}");
        Console.WriteLine();

        if (!intent.IsRelevant)
        {
            Console.WriteLine("❌ Query is not relevant for home automation data access.");
            return;
        }

        Console.WriteLine("✅ Generated GraphQL Query:");
        Console.WriteLine(new string('-', 40));
        
        var graphQLQuery = _graphQLService.GenerateQuery(intent);
        var formattedQuery = GraphQLQueryService.FormatQuery(graphQLQuery);
        Console.WriteLine(formattedQuery);
        
        Console.WriteLine(new string('-', 40));
    }
}
