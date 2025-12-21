using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Represents a range in a file.
/// </summary>
public record Range
{
    /// <summary>
    /// Start line number (0-indexed).
    /// </summary>
    [JsonPropertyName("start")]
    public required int Start { get; init; }

    /// <summary>
    /// End line number (0-indexed).
    /// </summary>
    [JsonPropertyName("end")]
    public required int End { get; init; }
}
