using EdificeBlackwell.Services;

namespace EdificeBlackwell;

/// <summary>
/// Edifice Blackwell - AI Assistant for Home Automation Data Access
/// Phase 1: GraphQL Query Generation from Natural Language
/// </summary>
class Program
{
    private static async Task Main(string[] args)
    {
        Console.WriteLine("=== Edifice Blackwell - Home Automation AI Assistant ===");
        Console.WriteLine("Phase 1: Natural Language to GraphQL Query Generation");
        Console.WriteLine();

        // Check if demo mode is requested
        if (args.Length > 0 && args[0].ToLower() == "demo")
        {
            RunDemoMode();
            return;
        }

        // Check if test mode is requested
        if (args.Length > 0 && args[0].ToLower() == "test")
        {
            RunTestMode();
            return;
        }

        try
        {
            // Initialize configuration
            var configService = new ConfigurationService();
            
            // Check if API key is available
            try
            {
                configService.ValidateConfiguration();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine("❌ Configuration Error:");
                Console.WriteLine(ex.Message);
                Console.WriteLine();
                Console.WriteLine("💡 You can run the demo mode to see how the application works:");
                Console.WriteLine("   dotnet run demo");
                Console.WriteLine();
                Console.WriteLine("   Or run tests to verify functionality:");
                Console.WriteLine("   dotnet run test");
                Console.WriteLine();
                Console.WriteLine("   Or set the AZURE_OPENAI_API_KEY environment variable and try again.");
                Environment.Exit(1);
            }

            // Initialize services
            var aiService = new AiService(
                configService.AzureOpenAiEndpoint,
                configService.AzureOpenAiApiKey,
                configService.AzureOpenAiModelId);
            
            var graphQLService = new GraphQLQueryService();

            Console.WriteLine("System ready! Enter your queries about home automation data.");
            Console.WriteLine("Examples:");
            Console.WriteLine("  - 'List the alarms for living room'");
            Console.WriteLine("  - 'Show me temperature sensors in the kitchen'");
            Console.WriteLine("  - 'What notifications do I have?'");
            Console.WriteLine();
            Console.WriteLine("Type 'exit' to quit.");
            Console.WriteLine(new string('=', 60));
            Console.WriteLine();

            // Main interaction loop
            await RunInteractiveMode(aiService, graphQLService);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Fatal error: {ex.Message}");
            Console.WriteLine("Please check your configuration and try again.");
            Environment.Exit(1);
        }
    }

    /// <summary>
    /// Run the application in demo mode with predefined scenarios
    /// </summary>
    private static void RunDemoMode()
    {
        var demoService = new DemoService();
        demoService.RunDemo();
    }

    /// <summary>
    /// Run the application in test mode to verify functionality
    /// </summary>
    private static void RunTestMode()
    {
        Tests.GraphQLQueryServiceTests.RunAllTests();
    }

    /// <summary>
    /// Run the application in interactive mode with AI integration
    /// </summary>
    private static async Task RunInteractiveMode(AiService aiService, GraphQLQueryService graphQLService)
    {
        while (true)
        {
            Console.Write("Query: ");
            var userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput))
                continue;

            if (userInput.Trim().ToLower() == "exit")
            {
                Console.WriteLine("Goodbye!");
                break;
            }

            await ProcessUserQueryAsync(userInput, aiService, graphQLService);
            Console.WriteLine();
        }
    }

    /// <summary>
    /// Process a user query and generate the corresponding GraphQL query
    /// </summary>
    private static async Task ProcessUserQueryAsync(
        string userQuery, 
        AiService aiService, 
        GraphQLQueryService graphQLService)
    {
        try
        {
            Console.WriteLine($"🔤 Processing: '{userQuery}'");
            Console.WriteLine();

            // Extract intent using AI
            Console.WriteLine("🤖 Analyzing query intent...");
            var intent = await aiService.ExtractIntentAsync(userQuery);

            // Display extracted intent
            Console.WriteLine("📋 Extracted Intent:");
            Console.WriteLine($"  Location: {intent.Location ?? "Not specified"}");
            Console.WriteLine($"  Operational Data Type: {intent.OperationalDataType ?? "Not specified"}");
            Console.WriteLine($"  Relevant: {intent.IsRelevant}");
            
            if (!string.IsNullOrEmpty(intent.Context))
            {
                Console.WriteLine($"  Context: {intent.Context}");
            }
            Console.WriteLine();

            if (!intent.IsRelevant)
            {
                Console.WriteLine("❌ Query is not relevant for home automation data access.");
                return;
            }

            // Generate GraphQL query
            Console.WriteLine("🔍 Generating GraphQL query...");
            var graphQLQuery = graphQLService.GenerateQuery(intent);
            var formattedQuery = GraphQLQueryService.FormatQuery(graphQLQuery);

            Console.WriteLine("✅ Generated GraphQL Query:");
            Console.WriteLine(new string('-', 40));
            Console.WriteLine(formattedQuery);
            Console.WriteLine(new string('-', 40));
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error processing query: {ex.Message}");
        }
    }
}
