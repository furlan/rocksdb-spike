using Microsoft.AspNetCore.Mvc;
using ModestBlackwell.Models;
using ModestBlackwell.Services.Interfaces;

namespace ModestBlackwell.Controllers;

/// <summary>
/// API controller for managing data streams
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class StreamsController : ControllerBase
{
    private readonly IStreamService _streamService;
    private readonly ILogger<StreamsController> _logger;

    public StreamsController(IStreamService streamService, ILogger<StreamsController> logger)
    {
        _streamService = streamService ?? throw new ArgumentNullException(nameof(streamService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Retrieves all data streams
    /// </summary>
    /// <returns>Collection of all data streams</returns>
    /// <response code="200">Returns the list of data streams</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<DataStream>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<DataStream>>> GetAllStreams()
    {
        try
        {
            _logger.LogInformation("Getting all data streams");
            var streams = await _streamService.GetAllStreamsAsync();
            return Ok(streams);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving all data streams");
            return StatusCode(500, "An error occurred while retrieving data streams");
        }
    }

    /// <summary>
    /// Retrieves a specific data stream by ID
    /// </summary>
    /// <param name="id">Data stream identifier</param>
    /// <returns>The requested data stream</returns>
    /// <response code="200">Returns the requested data stream</response>
    /// <response code="404">If the data stream is not found</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(DataStream), 200)]
    [ProducesResponseType(404)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<DataStream>> GetStreamById(string id)
    {
        try
        {
            _logger.LogInformation("Getting data stream with id: {Id}", id);
            
            if (string.IsNullOrWhiteSpace(id))
            {
                return BadRequest("Data stream ID cannot be empty");
            }

            var stream = await _streamService.GetStreamByIdAsync(id);
            
            if (stream == null)
            {
                return NotFound($"Data stream with ID '{id}' not found");
            }

            return Ok(stream);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving data stream with id: {Id}", id);
            return StatusCode(500, "An error occurred while retrieving the data stream");
        }
    }

    /// <summary>
    /// Retrieves all data streams for a specific asset
    /// </summary>
    /// <param name="assetId">Asset identifier</param>
    /// <returns>Collection of data streams for the asset</returns>
    /// <response code="200">Returns the list of data streams for the asset</response>
    /// <response code="500">If there was an internal server error</response>
    [HttpGet("by-asset/{assetId}")]
    [ProducesResponseType(typeof(IEnumerable<DataStream>), 200)]
    [ProducesResponseType(500)]
    public async Task<ActionResult<IEnumerable<DataStream>>> GetStreamsByAssetId(string assetId)
    {
        try
        {
            _logger.LogInformation("Getting data streams for asset: {AssetId}", assetId);
            
            if (string.IsNullOrWhiteSpace(assetId))
            {
                return BadRequest("Asset ID cannot be empty");
            }

            var streams = await _streamService.GetStreamsByAssetIdAsync(assetId);
            return Ok(streams);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving data streams for asset: {AssetId}", assetId);
            return StatusCode(500, "An error occurred while retrieving data streams for the asset");
        }
    }
}
