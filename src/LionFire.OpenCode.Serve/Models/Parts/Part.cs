using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Represents a message part. Uses JsonExtensionData to capture all properties
/// since the API may return the type discriminator in non-first position.
/// </summary>
public record Part
{
    /// <summary>
    /// The unique identifier of the part.
    /// </summary>
    [JsonPropertyName("id")]
    public string Id { get; init; } = string.Empty;

    /// <summary>
    /// The session ID this part belongs to.
    /// </summary>
    [JsonPropertyName("sessionID")]
    public string SessionId { get; init; } = string.Empty;

    /// <summary>
    /// The message ID this part belongs to.
    /// </summary>
    [JsonPropertyName("messageID")]
    public string MessageId { get; init; } = string.Empty;

    /// <summary>
    /// The type of part (text, file, tool, agent, reasoning, etc.).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// The text content (for text and reasoning parts).
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }

    /// <summary>
    /// Tool call ID (for tool parts).
    /// </summary>
    [JsonPropertyName("callID")]
    public string? CallId { get; init; }

    /// <summary>
    /// Tool name (for tool parts).
    /// </summary>
    [JsonPropertyName("tool")]
    public string? Tool { get; init; }

    /// <summary>
    /// Agent name (for agent parts).
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// File MIME type (for file parts).
    /// </summary>
    [JsonPropertyName("mime")]
    public string? Mime { get; init; }

    /// <summary>
    /// File URL (for file parts).
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// Filename (for file parts).
    /// </summary>
    [JsonPropertyName("filename")]
    public string? Filename { get; init; }

    /// <summary>
    /// Timing information for streaming parts.
    /// </summary>
    [JsonPropertyName("time")]
    public PartTime? Time { get; init; }

    /// <summary>
    /// Hash (for patch and snapshot parts).
    /// </summary>
    [JsonPropertyName("hash")]
    public string? Hash { get; init; }

    /// <summary>
    /// Finish reason (for step-finish parts).
    /// </summary>
    [JsonPropertyName("reason")]
    public string? Reason { get; init; }

    /// <summary>
    /// Cost in dollars (for step-finish parts).
    /// </summary>
    [JsonPropertyName("cost")]
    public double? Cost { get; init; }

    /// <summary>
    /// Retry attempt number (for retry parts).
    /// </summary>
    [JsonPropertyName("attempt")]
    public int? Attempt { get; init; }

    /// <summary>
    /// Error message (for retry parts).
    /// </summary>
    [JsonPropertyName("message")]
    public string? ErrorMessage { get; init; }

    /// <summary>
    /// Tool state (for tool parts). Can be a string ("pending", "running", "completed", "error")
    /// or an object with status, input, and raw properties.
    /// </summary>
    [JsonPropertyName("state")]
    public object? State { get; init; }

    /// <summary>
    /// Gets the tool state status as a string, whether state is a string or object.
    /// </summary>
    [JsonIgnore]
    public string? StateStatus
    {
        get
        {
            if (State is string s) return s;
            if (State is System.Text.Json.JsonElement je)
            {
                if (je.ValueKind == System.Text.Json.JsonValueKind.String)
                    return je.GetString();
                if (je.ValueKind == System.Text.Json.JsonValueKind.Object && je.TryGetProperty("status", out var statusProp))
                    return statusProp.GetString();
            }
            return null;
        }
    }

    /// <summary>
    /// Tool input data (for tool parts).
    /// </summary>
    [JsonPropertyName("input")]
    public object? Input { get; init; }

    /// <summary>
    /// Tool output data (for tool parts).
    /// </summary>
    [JsonPropertyName("output")]
    public object? Output { get; init; }

    /// <summary>
    /// Whether this is a synthetic part.
    /// </summary>
    [JsonPropertyName("synthetic")]
    public bool? Synthetic { get; init; }

    /// <summary>
    /// Whether this part should be ignored.
    /// </summary>
    [JsonPropertyName("ignored")]
    public bool? Ignored { get; init; }

    /// <summary>
    /// Raw JSON data for properties not explicitly mapped.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object?>? ExtensionData { get; set; }

    /// <summary>
    /// Gets the text content for this part (convenience property).
    /// Works for text, reasoning, and other text-containing parts.
    /// </summary>
    [JsonIgnore]
    public string? Content => Text;

    /// <summary>
    /// Checks if this part is a text part.
    /// </summary>
    [JsonIgnore]
    public bool IsTextPart => Type == "text";

    /// <summary>
    /// Checks if this part is a tool part.
    /// </summary>
    [JsonIgnore]
    public bool IsToolPart => Type == "tool";

    /// <summary>
    /// Checks if this part is a file part.
    /// </summary>
    [JsonIgnore]
    public bool IsFilePart => Type == "file";

    /// <summary>
    /// Checks if this part is an agent part.
    /// </summary>
    [JsonIgnore]
    public bool IsAgentPart => Type == "agent";

    /// <summary>
    /// Checks if this tool part is completed.
    /// </summary>
    [JsonIgnore]
    public bool IsToolCompleted => Type == "tool" && StateStatus == "completed";

    /// <summary>
    /// Gets the tool output as a string (for completed tool parts).
    /// </summary>
    [JsonIgnore]
    public string? OutputString => Output?.ToString();
}

/// <summary>
/// Input for creating a message part.
/// </summary>
public record PartInput
{
    /// <summary>
    /// The type of part (text, file, agent, subtask).
    /// </summary>
    [JsonPropertyName("type")]
    public string Type { get; init; } = string.Empty;

    /// <summary>
    /// The text content (for text parts).
    /// </summary>
    [JsonPropertyName("text")]
    public string? Text { get; init; }

    /// <summary>
    /// The content (for subtask parts).
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; init; }

    /// <summary>
    /// The agent name (for agent parts).
    /// </summary>
    [JsonPropertyName("name")]
    public string? Name { get; init; }

    /// <summary>
    /// Optional ID for the part.
    /// </summary>
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    /// <summary>
    /// The MIME type of the file (for file parts).
    /// </summary>
    [JsonPropertyName("mime")]
    public string? Mime { get; init; }

    /// <summary>
    /// The filename (for file parts).
    /// </summary>
    [JsonPropertyName("filename")]
    public string? Filename { get; init; }

    /// <summary>
    /// The URL of the file (for file parts).
    /// </summary>
    [JsonPropertyName("url")]
    public string? Url { get; init; }

    /// <summary>
    /// Raw JSON data for properties not explicitly mapped.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, object?>? ExtensionData { get; set; }

    /// <summary>
    /// Creates a text part input.
    /// </summary>
    public static PartInput TextInput(string text) => new() { Type = "text", Text = text };

    /// <summary>
    /// Creates a subtask part input.
    /// </summary>
    public static PartInput SubtaskInput(string content) => new() { Type = "subtask", Content = content };
}

/// <summary>
/// Timing information for a part.
/// </summary>
public record PartTime
{
    /// <summary>
    /// Start timestamp (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("start")]
    public long Start { get; init; }

    /// <summary>
    /// End timestamp (Unix timestamp in milliseconds).
    /// </summary>
    [JsonPropertyName("end")]
    public long? End { get; init; }
}
