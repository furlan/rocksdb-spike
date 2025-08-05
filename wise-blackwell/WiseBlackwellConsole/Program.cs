using Microsoft.Extensions.DependencyInjection;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;

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

// Add a plugin
kernel.Plugins.AddFromType<ModestBlackwellApiPlugin>("ModestBlackwellApi");

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

    // Get response from the AI
    var result = await chatCompletionService.GetChatMessageContentAsync(
        history,
        executionSettings: openAIPromptExecutionSettings,
        kernel: kernel
    );

    Console.WriteLine("Wise-Blackwell > " + result);

    history.AddMessage(result.Role, result.Content ?? string.Empty);
} while (userInput is not "bye");