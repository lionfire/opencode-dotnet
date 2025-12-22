using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Represents a message in a session. Uses a unified structure with all properties
/// to handle JSON deserialization without relying on polymorphic type discriminators.
/// </summary>
public record Message
{
    /// <summary>
    /// The unique identifier of the message.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// The session ID this message belongs to.
    /// </summary>
    [JsonPropertyName("sessionID")]
    public string SessionId { get; init; } = string.Empty;

    /// <summary>
    /// The role of the message sender (user or assistant).
    /// </summary>
    [JsonPropertyName("role")]
    public string Role { get; init; } = string.Empty;

    /// <summary>
    /// Timestamps for the message.
    /// </summary>
    [JsonPropertyName("time")]
    public MessageTime? Time { get; init; }

    // User message properties

    /// <summary>
    /// Summary information for the message (user messages).
    /// </summary>
    [JsonPropertyName("summary")]
    public object? Summary { get; init; }

    /// <summary>
    /// The agent to use for the session (user messages).
    /// </summary>
    [JsonPropertyName("agent")]
    public string? Agent { get; init; }

    /// <summary>
    /// The model to use for the session (user messages).
    /// </summary>
    [JsonPropertyName("model")]
    public ModelReference? Model { get; init; }

    /// <summary>
    /// Optional system prompt (user messages).
    /// </summary>
    [JsonPropertyName("system")]
    public string? System { get; init; }

    /// <summary>
    /// Tool permissions for the session (user messages).
    /// </summary>
    [JsonPropertyName("tools")]
    public Dictionary<string, bool>? Tools { get; init; }

    // Assistant message properties

    /// <summary>
    /// Error information if the message failed (assistant messages).
    /// </summary>
    [JsonPropertyName("error")]
    public MessageError? Error { get; init; }

    /// <summary>
    /// The parent message ID that this message is responding to (assistant messages).
    /// </summary>
    [JsonPropertyName("parentID")]
    public string? ParentId { get; init; }

    /// <summary>
    /// The model ID used (assistant messages).
    /// </summary>
    [JsonPropertyName("modelID")]
    public string? ModelId { get; init; }

    /// <summary>
    /// The provider ID used (assistant messages).
    /// </summary>
    [JsonPropertyName("providerID")]
    public string? ProviderId { get; init; }

    /// <summary>
    /// The mode of the message (assistant messages).
    /// </summary>
    [JsonPropertyName("mode")]
    public string? Mode { get; init; }

    /// <summary>
    /// Path information for the message (assistant messages).
    /// </summary>
    [JsonPropertyName("path")]
    public MessagePath? Path { get; init; }

    /// <summary>
    /// Cost in dollars for this message (assistant messages).
    /// </summary>
    [JsonPropertyName("cost")]
    public double? Cost { get; init; }

    /// <summary>
    /// Token usage information (assistant messages).
    /// </summary>
    [JsonPropertyName("tokens")]
    public TokenUsage? Tokens { get; init; }

    /// <summary>
    /// Finish reason for the message (assistant messages).
    /// </summary>
    [JsonPropertyName("finish")]
    public string? Finish { get; init; }

    /// <summary>
    /// Raw JSON data for properties not explicitly mapped.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object?>? ExtensionData { get; set; }

    /// <summary>
    /// Checks if this is a user message.
    /// </summary>
    [JsonIgnore]
    public bool IsUserMessage => Role == "user";

    /// <summary>
    /// Checks if this is an assistant message.
    /// </summary>
    [JsonIgnore]
    public bool IsAssistantMessage => Role == "assistant";
}

/// <summary>
/// Convenience type for creating user messages programmatically.
/// </summary>
public record UserMessage : Message
{
    /// <summary>
    /// Creates a new user message.
    /// </summary>
    public UserMessage()
    {
        Role = "user";
    }
}

/// <summary>
/// Convenience type for creating assistant messages programmatically.
/// </summary>
public record AssistantMessage : Message
{
    /// <summary>
    /// Creates a new assistant message.
    /// </summary>
    public AssistantMessage()
    {
        Role = "assistant";
    }
}

/// <summary>
/// Timestamps for a message.
/// </summary>
public record MessageTime
{
    /// <summary>
    /// When the message was created (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("created")]
    public long Created { get; init; }

    /// <summary>
    /// When the message was completed (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("completed")]
    public long? Completed { get; init; }
}

/// <summary>
/// Model information for a message.
/// </summary>
public record MessageModel
{
    /// <summary>
    /// The provider ID.
    /// </summary>
    [JsonPropertyName("providerID")]
    public string? ProviderId { get; init; }

    /// <summary>
    /// The model ID.
    /// </summary>
    [JsonPropertyName("modelID")]
    public string? ModelId { get; init; }
}

/// <summary>
/// Summary for a user message.
/// </summary>
public record UserMessageSummary
{
    /// <summary>
    /// Title of the message.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// Body of the message.
    /// </summary>
    [JsonPropertyName("body")]
    public string? Body { get; init; }

    /// <summary>
    /// File diffs in the message.
    /// </summary>
    [JsonPropertyName("diffs")]
    public List<FileDiff>? Diffs { get; init; }
}

/// <summary>
/// Path information for an assistant message.
/// </summary>
public record MessagePath
{
    /// <summary>
    /// Current working directory.
    /// </summary>
    [JsonPropertyName("cwd")]
    public string Cwd { get; init; } = string.Empty;

    /// <summary>
    /// Root directory.
    /// </summary>
    [JsonPropertyName("root")]
    public string Root { get; init; } = string.Empty;
}

/// <summary>
/// Token usage for a message.
/// </summary>
public record TokenUsage
{
    /// <summary>
    /// Number of input tokens.
    /// </summary>
    [JsonPropertyName("input")]
    public int Input { get; init; }

    /// <summary>
    /// Number of output tokens.
    /// </summary>
    [JsonPropertyName("output")]
    public int Output { get; init; }

    /// <summary>
    /// Number of reasoning tokens.
    /// </summary>
    [JsonPropertyName("reasoning")]
    public int Reasoning { get; init; }

    /// <summary>
    /// Cache usage information.
    /// </summary>
    [JsonPropertyName("cache")]
    public CacheUsage? Cache { get; init; }
}

/// <summary>
/// Cache usage information.
/// </summary>
public record CacheUsage
{
    /// <summary>
    /// Number of cache read tokens.
    /// </summary>
    [JsonPropertyName("read")]
    public int Read { get; init; }

    /// <summary>
    /// Number of cache write tokens.
    /// </summary>
    [JsonPropertyName("write")]
    public int Write { get; init; }
}

/// <summary>
/// Reference to a model.
/// </summary>
public record ModelReference
{
    /// <summary>
    /// The provider ID.
    /// </summary>
    [JsonPropertyName("providerID")]
    public string ProviderId { get; init; } = string.Empty;

    /// <summary>
    /// The model ID.
    /// </summary>
    [JsonPropertyName("modelID")]
    public string ModelId { get; init; } = string.Empty;
}

/// <summary>
/// Error information for a message. Uses a unified structure for JSON deserialization.
/// </summary>
public record MessageError
{
    /// <summary>
    /// The type of error (provider_auth, unknown, message_output_length, message_aborted, api).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// The error message.
    /// </summary>
    [JsonPropertyName("message")]
    public string ErrorMessage { get; init; } = string.Empty;

    /// <summary>
    /// The provider ID that failed authentication (for provider_auth errors).
    /// </summary>
    [JsonPropertyName("providerID")]
    public string? ProviderId { get; init; }

    /// <summary>
    /// Raw JSON data for properties not explicitly mapped.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object?>? ExtensionData { get; set; }
}

/// <summary>
/// Wrapper for message with parts (used in some API responses).
/// The API returns "info" for the message data.
/// </summary>
public record MessageWithParts
{
    /// <summary>
    /// The message info (returned as "info" in API responses).
    /// </summary>
    [JsonPropertyName("info")]
    public Message? Message { get; init; }

    /// <summary>
    /// The parts of the message.
    /// </summary>
    [JsonPropertyName("parts")]
    public List<Part>? Parts { get; init; }
}

/// <summary>
/// Request to send a message to a session.
/// </summary>
public record SendMessageRequest
{
    /// <summary>
    /// The message parts to send.
    /// </summary>
    [JsonPropertyName("parts")]
    public List<PartInput> Parts { get; init; } = new();

    /// <summary>
    /// Optional agent to use.
    /// </summary>
    [JsonPropertyName("agent")]
    public string? Agent { get; init; }

    /// <summary>
    /// Optional model to use.
    /// </summary>
    [JsonPropertyName("model")]
    public ModelReference? Model { get; init; }
}
