using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Represents the status of an OpenCode session.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<SessionStatus>))]
public enum SessionStatus
{
    /// <summary>
    /// The session is active and can receive messages.
    /// </summary>
    Active,

    /// <summary>
    /// The session has been aborted.
    /// </summary>
    Aborted,

    /// <summary>
    /// The session has completed normally.
    /// </summary>
    Completed
}

/// <summary>
/// Represents an OpenCode session.
/// </summary>
/// <param name="Id">The unique identifier of the session.</param>
/// <param name="CreatedAt">When the session was created.</param>
/// <param name="UpdatedAt">When the session was last updated.</param>
/// <param name="Status">The current status of the session.</param>
/// <param name="SharedToken">Optional token for sharing the session.</param>
/// <param name="Directory">Optional working directory for the session.</param>
public record Session(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("createdAt")] DateTimeOffset CreatedAt,
    [property: JsonPropertyName("updatedAt")] DateTimeOffset UpdatedAt,
    [property: JsonPropertyName("status")] SessionStatus Status,
    [property: JsonPropertyName("sharedToken")] string? SharedToken = null,
    [property: JsonPropertyName("directory")] string? Directory = null
);

/// <summary>
/// Request to create a new session.
/// </summary>
/// <param name="ParentId">Optional parent session ID to fork from.</param>
/// <param name="Title">Optional title for the session.</param>
public record CreateSessionRequest(
    [property: JsonPropertyName("parentID")] string? ParentId = null,
    [property: JsonPropertyName("title")] string? Title = null
);

/// <summary>
/// Request to fork an existing session.
/// </summary>
/// <param name="MessageId">The message ID to fork from (creates a new session with messages up to this point).</param>
public record ForkSessionRequest(
    [property: JsonPropertyName("messageId")] string MessageId
);
