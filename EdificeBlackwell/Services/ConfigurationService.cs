using Microsoft.Extensions.Configuration;

namespace EdificeBlackwell.Services;

/// <summary>
/// Service for handling application configuration from environment variables
/// </summary>
public class ConfigurationService
{
    private readonly IConfiguration _configuration;

    public ConfigurationService()
    {
        var builder = new ConfigurationBuilder()
            .AddEnvironmentVariables();

        _configuration = builder.Build();
    }

    /// <summary>
    /// Azure OpenAI endpoint URL
    /// </summary>
    public string AzureOpenAiEndpoint => 
        _configuration["AZURE_OPENAI_ENDPOINT"] ?? "https://ff-openai-gpt-4.openai.azure.com/";

    /// <summary>
    /// Azure OpenAI API key
    /// </summary>
    public string AzureOpenAiApiKey => 
        _configuration["AZURE_OPENAI_API_KEY"] ?? 
        throw new InvalidOperationException("AZURE_OPENAI_API_KEY environment variable is required");

    /// <summary>
    /// Azure OpenAI model deployment ID
    /// </summary>
    public string AzureOpenAiModelId => 
        _configuration["AZURE_OPENAI_MODEL_ID"] ?? "gpt-4o_deployment";

    /// <summary>
    /// Validate that all required configuration is present
    /// </summary>
    public void ValidateConfiguration()
    {
        if (string.IsNullOrEmpty(AzureOpenAiApiKey))
        {
            throw new InvalidOperationException(
                "AZURE_OPENAI_API_KEY environment variable is required. " +
                "Please set it before running the application.");
        }

        Console.WriteLine($"Configuration loaded:");
        Console.WriteLine($"  Azure OpenAI Endpoint: {AzureOpenAiEndpoint}");
        Console.WriteLine($"  Azure OpenAI Model: {AzureOpenAiModelId}");
        Console.WriteLine($"  Azure OpenAI API Key: [CONFIGURED]");
        Console.WriteLine();
    }
}
