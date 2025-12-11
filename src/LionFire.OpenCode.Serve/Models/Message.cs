using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Represents the role of a message in a conversation.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<MessageRole>))]
public enum MessageRole
{
    /// <summary>
    /// System message providing context or instructions.
    /// </summary>
    System,

    /// <summary>
    /// Message from the user.
    /// </summary>
    User,

    /// <summary>
    /// Message from the AI assistant.
    /// </summary>
    Assistant,

    /// <summary>
    /// Message from a tool execution.
    /// </summary>
    Tool
}

/// <summary>
/// Base class for message parts.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(TextPart), "text")]
[JsonDerivedType(typeof(FilePart), "file")]
[JsonDerivedType(typeof(AgentPart), "agent")]
[JsonDerivedType(typeof(ToolUsePart), "tool_use")]
[JsonDerivedType(typeof(ToolResultPart), "tool_result")]
public abstract record MessagePart;

/// <summary>
/// A text content part of a message.
/// </summary>
public record TextPart : MessagePart
{
    /// <summary>
    /// Gets or sets the text content.
    /// </summary>
    [JsonPropertyName("text")]
    public required string Text { get; init; }

    /// <summary>
    /// Creates a new TextPart with the specified text.
    /// </summary>
    /// <param name="text">The text content.</param>
    [SetsRequiredMembers]
    public TextPart(string text) => Text = text;

    /// <summary>
    /// Creates a new TextPart. Required for JSON deserialization.
    /// </summary>
    public TextPart() { }
}

/// <summary>
/// A file reference part of a message.
/// </summary>
public record FilePart : MessagePart
{
    /// <summary>
    /// Gets or sets the path to the file.
    /// </summary>
    [JsonPropertyName("filePath")]
    public required string FilePath { get; init; }

    /// <summary>
    /// Gets or sets the optional file content if inline.
    /// </summary>
    [JsonPropertyName("content")]
    public string? Content { get; init; }

    /// <summary>
    /// Creates a new FilePart with the specified path and optional content.
    /// </summary>
    /// <param name="filePath">The path to the file.</param>
    /// <param name="content">Optional file content if inline.</param>
    [SetsRequiredMembers]
    public FilePart(string filePath, string? content = null)
    {
        FilePath = filePath;
        Content = content;
    }

    /// <summary>
    /// Creates a new FilePart. Required for JSON deserialization.
    /// </summary>
    public FilePart() { }
}

/// <summary>
/// An agent-specific content part.
/// </summary>
public record AgentPart : MessagePart
{
    /// <summary>
    /// Gets or sets the agent identifier.
    /// </summary>
    [JsonPropertyName("agentId")]
    public required string AgentId { get; init; }

    /// <summary>
    /// Gets or sets the content from the agent.
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; init; }

    /// <summary>
    /// Creates a new AgentPart with the specified agent ID and content.
    /// </summary>
    /// <param name="agentId">The agent identifier.</param>
    /// <param name="content">The content from the agent.</param>
    [SetsRequiredMembers]
    public AgentPart(string agentId, string content)
    {
        AgentId = agentId;
        Content = content;
    }

    /// <summary>
    /// Creates a new AgentPart. Required for JSON deserialization.
    /// </summary>
    public AgentPart() { }
}

/// <summary>
/// A tool use request part.
/// </summary>
public record ToolUsePart : MessagePart
{
    /// <summary>
    /// Gets or sets the ID of the tool being used.
    /// </summary>
    [JsonPropertyName("toolId")]
    public required string ToolId { get; init; }

    /// <summary>
    /// Gets or sets the name of the tool.
    /// </summary>
    [JsonPropertyName("toolName")]
    public required string ToolName { get; init; }

    /// <summary>
    /// Gets or sets the input parameters for the tool.
    /// </summary>
    [JsonPropertyName("input")]
    public object? Input { get; init; }

    /// <summary>
    /// Creates a new ToolUsePart with the specified parameters.
    /// </summary>
    /// <param name="toolId">The ID of the tool being used.</param>
    /// <param name="toolName">The name of the tool.</param>
    /// <param name="input">Optional input parameters for the tool.</param>
    [SetsRequiredMembers]
    public ToolUsePart(string toolId, string toolName, object? input = null)
    {
        ToolId = toolId;
        ToolName = toolName;
        Input = input;
    }

    /// <summary>
    /// Creates a new ToolUsePart. Required for JSON deserialization.
    /// </summary>
    public ToolUsePart() { }
}

/// <summary>
/// A tool execution result part.
/// </summary>
public record ToolResultPart : MessagePart
{
    /// <summary>
    /// Gets or sets the ID of the tool that was used.
    /// </summary>
    [JsonPropertyName("toolId")]
    public required string ToolId { get; init; }

    /// <summary>
    /// Gets or sets the output from the tool execution.
    /// </summary>
    [JsonPropertyName("output")]
    public string? Output { get; init; }

    /// <summary>
    /// Gets or sets whether the tool execution resulted in an error.
    /// </summary>
    [JsonPropertyName("isError")]
    public bool IsError { get; init; }

    /// <summary>
    /// Creates a new ToolResultPart with the specified parameters.
    /// </summary>
    /// <param name="toolId">The ID of the tool that was used.</param>
    /// <param name="output">The output from the tool execution.</param>
    /// <param name="isError">Whether the tool execution resulted in an error.</param>
    [SetsRequiredMembers]
    public ToolResultPart(string toolId, string? output, bool isError = false)
    {
        ToolId = toolId;
        Output = output;
        IsError = isError;
    }

    /// <summary>
    /// Creates a new ToolResultPart. Required for JSON deserialization.
    /// </summary>
    public ToolResultPart() { }
}

/// <summary>
/// Represents a message in an OpenCode session.
/// </summary>
public record Message
{
    /// <summary>
    /// Gets or sets the unique identifier of the message.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// Gets or sets the session this message belongs to.
    /// </summary>
    [JsonPropertyName("sessionId")]
    public required string SessionId { get; init; }

    /// <summary>
    /// Gets or sets the role of the message sender.
    /// </summary>
    [JsonPropertyName("role")]
    public required MessageRole Role { get; init; }

    /// <summary>
    /// Gets or sets the parts that make up this message.
    /// </summary>
    [JsonPropertyName("parts")]
    public required IReadOnlyList<MessagePart> Parts { get; init; }

    /// <summary>
    /// Gets or sets when the message was created.
    /// </summary>
    [JsonPropertyName("createdAt")]
    public required DateTimeOffset CreatedAt { get; init; }

    /// <summary>
    /// Gets or sets the optional token count for this message.
    /// </summary>
    [JsonPropertyName("tokenCount")]
    public int? TokenCount { get; init; }

    /// <summary>
    /// Creates a new Message with the specified parameters.
    /// </summary>
    [SetsRequiredMembers]
    public Message(string id, string sessionId, MessageRole role, IReadOnlyList<MessagePart> parts, DateTimeOffset createdAt, int? tokenCount = null)
    {
        Id = id;
        SessionId = sessionId;
        Role = role;
        Parts = parts;
        CreatedAt = createdAt;
        TokenCount = tokenCount;
    }

    /// <summary>
    /// Creates a new Message. Required for JSON deserialization.
    /// </summary>
    public Message() { }
}

/// <summary>
/// Request to send a message to a session.
/// </summary>
public record SendMessageRequest
{
    /// <summary>
    /// Gets or sets the message parts to send.
    /// </summary>
    [JsonPropertyName("parts")]
    public required IReadOnlyList<MessagePart> Parts { get; init; }

    /// <summary>
    /// Creates a new SendMessageRequest with the specified parts.
    /// </summary>
    /// <param name="parts">The message parts to send.</param>
    [SetsRequiredMembers]
    public SendMessageRequest(IReadOnlyList<MessagePart> parts) => Parts = parts;

    /// <summary>
    /// Creates a new SendMessageRequest. Required for JSON deserialization.
    /// </summary>
    public SendMessageRequest() { }
}

/// <summary>
/// Represents an incremental update during message streaming.
/// </summary>
public record MessageUpdate
{
    /// <summary>
    /// Gets or sets the ID of the message being streamed.
    /// </summary>
    [JsonPropertyName("messageId")]
    public string? MessageId { get; init; }

    /// <summary>
    /// Gets or sets the incremental content update.
    /// </summary>
    [JsonPropertyName("delta")]
    public string? Delta { get; init; }

    /// <summary>
    /// Gets or sets whether the message is complete.
    /// </summary>
    [JsonPropertyName("done")]
    public bool Done { get; init; }

    /// <summary>
    /// Gets or sets the running token count if available.
    /// </summary>
    [JsonPropertyName("tokenCount")]
    public int? TokenCount { get; init; }

    /// <summary>
    /// Creates a new MessageUpdate with the specified parameters.
    /// </summary>
    public MessageUpdate(string? messageId, string? delta, bool done = false, int? tokenCount = null)
    {
        MessageId = messageId;
        Delta = delta;
        Done = done;
        TokenCount = tokenCount;
    }

    /// <summary>
    /// Creates a new MessageUpdate. Required for JSON deserialization.
    /// </summary>
    public MessageUpdate() { }
}
