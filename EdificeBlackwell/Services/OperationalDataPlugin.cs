using System.ComponentModel;
using Microsoft.SemanticKernel;
using EdificeBlackwell.Models;

namespace EdificeBlackwell.Services;

/// <summary>
/// Semantic Kernel plugin for operational data functions
/// </summary>
public class OperationalDataPlugin
{
    private readonly OperationalDataService _operationalDataService;

    public OperationalDataPlugin(OperationalDataService operationalDataService)
    {
        _operationalDataService = operationalDataService;
    }

    /// <summary>
    /// Get available operational data types for prompt inclusion
    /// </summary>
    [KernelFunction("GetOperationalTypes")]
    [Description("Retrieves the list of available operational data types with their descriptions and synonyms")]
    public string GetOperationalTypes()
    {
        return _operationalDataService.GetOperationalTypesForPrompt();
    }

    /// <summary>
    /// Find operational type by name or synonym
    /// </summary>
    [KernelFunction("FindOperationalType")]
    [Description("Finds an operational type by name or synonym")]
    public string FindOperationalType([Description("The operational type name or synonym to search for")] string input)
    {
        var operationalType = _operationalDataService.FindOperationalType(input);
        return operationalType?.Name ?? "";
    }
}
