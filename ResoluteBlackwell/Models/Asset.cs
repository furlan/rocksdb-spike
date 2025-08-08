using System;

namespace ResoluteBlackwell.Models;

/// <summary>
/// Defines the attributes of an asset.
/// </summary>
public class Asset
{
    /// <summary>
    /// Unique identifier for the asset
    /// </summary>
    public string Id { get; set; } = string.Empty;

    /// <summary>
    /// Human-readable name of the asset
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Physical location of the asset
    /// </summary>
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Type of the asset (e.g., thermostat, light bulb)
    /// </summary>
    public string Type { get; set; } = string.Empty;

    /// <summary>
    /// Classification of the asset (e.g., automation, light)
    /// </summary>
    public string Class { get; set; } = string.Empty;

    /// <summary>
    /// Parent asset identifier (optional)
    /// </summary>
    public string? Parent { get; set; }

    /// <summary>
    /// Operational type for organizing RocksDB data (e.g., utilization, alarm, notification)
    /// </summary>
    public string OperationalType { get; set; } = string.Empty;
}
