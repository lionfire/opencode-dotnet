using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Represents a diff for a file.
/// </summary>
public record FileDiff
{
    /// <summary>
    /// The file path.
    /// </summary>
    [JsonPropertyName("file")]
    public required string File { get; init; }

    /// <summary>
    /// The hash before the change.
    /// </summary>
    [JsonPropertyName("before")]
    public required string Before { get; init; }

    /// <summary>
    /// The hash after the change.
    /// </summary>
    [JsonPropertyName("after")]
    public required string After { get; init; }

    /// <summary>
    /// Number of lines added.
    /// </summary>
    [JsonPropertyName("additions")]
    public required int Additions { get; init; }

    /// <summary>
    /// Number of lines deleted.
    /// </summary>
    [JsonPropertyName("deletions")]
    public required int Deletions { get; init; }
}
