using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.PromptTemplates.Handlebars;
using YamlDotNet.Serialization;

var modelId = Environment.GetEnvironmentVariable("OPENAI_API_MODEL_ID")
    ?? throw new InvalidOperationException("OPENAI_API_MODEL_ID environment variable is not set.");
var endpoint = Environment.GetEnvironmentVariable("OPENAI_API_URL")
    ?? throw new InvalidOperationException("OPENAI_API_URL environment variable is not set.");
var apiKey = Environment.GetEnvironmentVariable("OPENAI_API_KEY")
    ?? throw new InvalidOperationException("OPENAI_API_KEY environment variable is not set.");

// create a kernel with Azure OpenAI chat completion
var builder = Kernel.CreateBuilder().AddAzureOpenAIChatCompletion(modelId, endpoint, apiKey);

Kernel kernel = builder.Build();
var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();

// Load prompt from resource
var handlebarsPromptYaml = EmbeddedResource.Read("ModestBlackwellGraphQlPrompt.yaml");

// Parse YAML to extract the template
var deserializer = new DeserializerBuilder().Build();
var yamlData = deserializer.Deserialize<Dictionary<string, object>>(handlebarsPromptYaml);
var template = yamlData["template"].ToString();

// Create the prompt function from the YAML resource
var templateFactory = new HandlebarsPromptTemplateFactory();
var function = kernel.CreateFunctionFromPrompt(
    template,
    templateFormat: "handlebars",
    promptTemplateFactory: templateFactory
);

// Add a plugin
// kernel.Plugins.AddFromType<ModestBlackwellApiPlugin>("ModestBlackwellApi");
kernel.Plugins.AddFromType<ModestBlackwellGraphQlPlugin>("ModestBlackwellGraphQlPlugin");

// Enable planning
OpenAIPromptExecutionSettings openAIPromptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto()
};

// Create history to store the conversation
var history = new ChatHistory();

// Initialize a back-and-forth chat
string? userInput;
do
{
    // Collect user input
    Console.Write("User > ");
    userInput = Console.ReadLine();

    history.AddUserMessage(userInput ?? string.Empty);

    // Get response from the AI using the Handlebars prompt function
    var promptResult = await function.InvokeAsync(kernel, new() { ["input"] = userInput ?? string.Empty });
    
    // Add the system prompt result to history
    history.AddSystemMessage(promptResult.GetValue<string>() ?? string.Empty);
    
    // Get final response from the AI
    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel
    );

    Console.WriteLine("Wise-Blackwell > " + result);

    history.AddMessage(result.Role, result.Content ?? string.Empty);
} while (userInput is not "bye");