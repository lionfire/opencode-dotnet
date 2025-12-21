using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Version control system information.
/// </summary>
public record VcsInfo
{
    /// <summary>
    /// The current branch name.
    /// </summary>
    [JsonPropertyName("branch")]
    public required string Branch { get; init; }
}
