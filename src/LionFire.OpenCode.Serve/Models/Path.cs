using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Path information.
/// </summary>
public record PathInfo
{
    /// <summary>
    /// The current working directory.
    /// </summary>
    [JsonPropertyName("cwd")]
    public required string Cwd { get; init; }

    /// <summary>
    /// The root directory.
    /// </summary>
    [JsonPropertyName("root")]
    public required string Root { get; init; }
}
