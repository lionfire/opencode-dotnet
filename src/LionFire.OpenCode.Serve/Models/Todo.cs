using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Represents a todo item.
/// </summary>
public record Todo
{
    /// <summary>
    /// Unique identifier for the todo item.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Brief description of the task.
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; init; }

    /// <summary>
    /// Current status of the task.
    /// </summary>
    [JsonPropertyName("status")]
    public required string Status { get; init; }

    /// <summary>
    /// Priority level of the task.
    /// </summary>
    [JsonPropertyName("priority")]
    public required string Priority { get; init; }
}
