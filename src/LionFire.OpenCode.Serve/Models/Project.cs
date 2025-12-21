using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Represents a project.
/// </summary>
public record Project
{
    /// <summary>
    /// The unique identifier of the project.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The worktree path.
    /// </summary>
    [JsonPropertyName("worktree")]
    public required string Worktree { get; init; }

    /// <summary>
    /// The VCS directory path (e.g., .git directory).
    /// </summary>
    [JsonPropertyName("vcsDir")]
    public string? VcsDir { get; init; }

    /// <summary>
    /// The version control system type (currently only "git").
    /// </summary>
    [JsonPropertyName("vcs")]
    public string? Vcs { get; init; }

    /// <summary>
    /// Timestamps for the project.
    /// </summary>
    [JsonPropertyName("time")]
    public required ProjectTime Time { get; init; }
}

/// <summary>
/// Timestamps for a project.
/// </summary>
public record ProjectTime
{
    /// <summary>
    /// When the project was created (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("created")]
    public required long Created { get; init; }

    /// <summary>
    /// When the project was last updated (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("updated")]
    public long? Updated { get; init; }

    /// <summary>
    /// When the project was initialized (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("initialized")]
    public long? Initialized { get; init; }
}
