using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using System.Text.Json;
using EdificeBlackwell.Models;

namespace EdificeBlackwell.Services;

/// <summary>
/// Service for handling AI-powered intent extraction and GraphQL query generation
/// </summary>
public class AiService
{
    private readonly Kernel _kernel;
    private readonly IChatCompletionService _chatService;
    private const string FUNCTION_NAME = "ExtractGraphQLIntent";

    public AiService(string azureOpenAiEndpoint, string azureOpenAiApiKey, string azureOpenAiModelId)
    {
        // Create kernel with Azure OpenAI configuration
        var builder = Kernel.CreateBuilder()
            .AddAzureOpenAIChatCompletion(
                deploymentName: azureOpenAiModelId,
                endpoint: azureOpenAiEndpoint,
                apiKey: azureOpenAiApiKey);

        _kernel = builder.Build();
        _chatService = _kernel.GetRequiredService<IChatCompletionService>();
    }

    /// <summary>
    /// Extract intent from user query using structured output
    /// </summary>
    public async Task<QueryIntent> ExtractIntentAsync(string userQuery)
    {
        try
        {
            // Load the prompt template
            var promptTemplate = await LoadPromptTemplateAsync();
            
            // Create the prompt
            var prompt = promptTemplate.Replace("{{$input}}", userQuery);

            // Configure execution settings for structured output
            var executionSettings = new OpenAIPromptExecutionSettings
            {
                ResponseFormat = "json_object",
                Temperature = 0.1,
                MaxTokens = 500
            };

            // Execute the prompt
            var result = await _chatService.GetChatMessageContentAsync(
                prompt, 
                executionSettings);

            // Parse the JSON response
            var intentJson = result.Content ?? "{}";
            var intent = JsonSerializer.Deserialize<QueryIntent>(intentJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return intent ?? new QueryIntent { IsRelevant = false };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error extracting intent: {ex.Message}");
            return new QueryIntent { IsRelevant = false, Context = $"Error: {ex.Message}" };
        }
    }

    /// <summary>
    /// Load the prompt template from YAML file
    /// </summary>
    private async Task<string> LoadPromptTemplateAsync()
    {
        var promptPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Prompts", "ExtractGraphQLIntent.yaml");
        
        if (!File.Exists(promptPath))
        {
            throw new FileNotFoundException($"Prompt template not found at: {promptPath}");
        }

        var yamlContent = await File.ReadAllTextAsync(promptPath);
        
        // Extract template content from YAML
        var lines = yamlContent.Split('\n');
        var templateStartIndex = Array.FindIndex(lines, line => line.Trim().StartsWith("template:"));
        
        if (templateStartIndex == -1)
        {
            throw new InvalidOperationException("Template section not found in YAML file");
        }

        // Extract template content (everything after "template: |")
        var templateLines = lines.Skip(templateStartIndex + 1)
            .Where(line => line.StartsWith("  ") || string.IsNullOrWhiteSpace(line))
            .Select(line => line.Length >= 2 ? line[2..] : line)
            .ToArray();

        return string.Join('\n', templateLines).Trim();
    }
}
