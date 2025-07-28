using Microsoft.AspNetCore.Mvc;
using ModestBlackwell.Models;
using ModestBlackwell.Services.Interfaces;

namespace ModestBlackwell.Controllers;

/// <summary>
/// API controller for managing assets
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class AssetsController : ControllerBase
{
    private readonly IAssetService _assetService;
    private readonly ILogger<AssetsController> _logger;

    public AssetsController(IAssetService assetService, ILogger<AssetsController> logger)
    {
        _assetService = assetService ?? throw new ArgumentNullException(nameof(assetService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves all assets
    /// </summary>
    /// <returns>Collection of all assets</returns>
    /// <response code="200">Returns the list of assets</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Asset>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<Asset>>> GetAllAssets()
    {
        try
        {
            _logger.LogInformation("Getting all assets");
            var assets = await _assetService.GetAllAssetsAsync();
            return Ok(assets);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all assets");
            return StatusCode(500, "An error occurred while retrieving assets");
        }
    }

    /// <summary>
    /// Retrieves a specific asset by ID
    /// </summary>
    /// <param name="id">Asset identifier</param>
    /// <returns>The requested asset</returns>
    /// <response code="200">Returns the requested asset</response>
    /// <response code="404">If the asset is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(Asset), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<Asset>> GetAssetById(string id)
    {
        try
        {
            _logger.LogInformation("Getting asset with id: {Id}", id);
            
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Asset ID cannot be empty");
            }

            var asset = await _assetService.GetAssetByIdAsync(id);
            
            if (asset == null)
            {
                return NotFound($"Asset with ID '{id}' not found");
            }

            return Ok(asset);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving asset with id: {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the asset");
        }
    }
}
