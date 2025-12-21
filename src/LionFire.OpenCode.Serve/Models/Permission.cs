using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Represents a permission request.
/// </summary>
public record Permission
{
    /// <summary>
    /// The unique identifier of the permission.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The type of permission (e.g., "file.write", "command.execute").
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// The pattern or patterns being requested (e.g., file path, command name).
    /// </summary>
    [JsonPropertyName("pattern")]
    public required PermissionPattern Pattern { get; init; }

    /// <summary>
    /// The session ID this permission belongs to.
    /// </summary>
    [JsonPropertyName("sessionID")]
    public required string SessionId { get; init; }

    /// <summary>
    /// The message ID this permission belongs to.
    /// </summary>
    [JsonPropertyName("messageID")]
    public required string MessageId { get; init; }

    /// <summary>
    /// The call ID if this is for a tool call.
    /// </summary>
    [JsonPropertyName("callID")]
    public string? CallId { get; init; }

    /// <summary>
    /// The title of the permission request.
    /// </summary>
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    /// <summary>
    /// Metadata for the permission.
    /// </summary>
    [JsonPropertyName("metadata")]
    public required Dictionary<string, object> Metadata { get; init; }

    /// <summary>
    /// Timestamps for the permission.
    /// </summary>
    [JsonPropertyName("time")]
    public required PermissionTime Time { get; init; }
}

/// <summary>
/// Permission pattern (can be a single string or array of strings).
/// </summary>
public abstract record PermissionPattern
{
}

/// <summary>
/// Single permission pattern.
/// </summary>
public record SinglePermissionPattern : PermissionPattern
{
    /// <summary>
    /// The pattern string.
    /// </summary>
    [JsonIgnore]
    public required string Pattern { get; init; }
}

/// <summary>
/// Multiple permission patterns.
/// </summary>
public record MultiplePermissionPattern : PermissionPattern
{
    /// <summary>
    /// The pattern strings.
    /// </summary>
    [JsonIgnore]
    public required List<string> Patterns { get; init; }
}

/// <summary>
/// Timestamps for a permission.
/// </summary>
public record PermissionTime
{
    /// <summary>
    /// When the permission was created (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("created")]
    public required long Created { get; init; }
}

/// <summary>
/// Request to respond to a permission.
/// </summary>
public record PermissionResponse
{
    /// <summary>
    /// Whether to allow the permission.
    /// </summary>
    [JsonPropertyName("allow")]
    public required bool Allow { get; init; }

    /// <summary>
    /// Whether to remember this decision.
    /// </summary>
    [JsonPropertyName("remember")]
    public bool? Remember { get; init; }
}
