using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Represents the permission level for a tool.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<ToolPermission>))]
public enum ToolPermission
{
    /// <summary>
    /// Tool usage is allowed without approval.
    /// </summary>
    Allowed,

    /// <summary>
    /// Tool usage is denied.
    /// </summary>
    Denied,

    /// <summary>
    /// Tool usage requires explicit approval for each use.
    /// </summary>
    RequiresApproval
}

/// <summary>
/// Represents a tool available in OpenCode.
/// </summary>
/// <param name="Id">The unique identifier of the tool.</param>
/// <param name="Name">The display name of the tool.</param>
/// <param name="Description">A description of what the tool does.</param>
/// <param name="Parameters">JSON schema describing the tool's parameters.</param>
/// <param name="RequiresApproval">Whether the tool requires user approval before execution.</param>
public record Tool(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("description")] string? Description,
    [property: JsonPropertyName("parameters")] object? Parameters,
    [property: JsonPropertyName("requiresApproval")] bool RequiresApproval = false
);

/// <summary>
/// Request to approve a tool execution.
/// </summary>
/// <param name="ToolId">The ID of the tool to approve.</param>
/// <param name="Approved">Whether the tool use is approved.</param>
public record ApproveToolRequest(
    [property: JsonPropertyName("toolId")] string ToolId,
    [property: JsonPropertyName("approved")] bool Approved = true
);

/// <summary>
/// Request to update tool permissions.
/// </summary>
/// <param name="Permission">The new permission level for the tool.</param>
public record UpdateToolPermissionRequest(
    [property: JsonPropertyName("permission")] ToolPermission Permission
);

/// <summary>
/// Represents a pending tool execution that requires approval.
/// </summary>
/// <param name="Id">The unique ID of this pending execution.</param>
/// <param name="SessionId">The session requesting the tool execution.</param>
/// <param name="ToolId">The ID of the tool to be executed.</param>
/// <param name="ToolName">The name of the tool.</param>
/// <param name="Input">The input parameters for the tool.</param>
/// <param name="CreatedAt">When the approval request was created.</param>
public record PendingToolExecution(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("sessionId")] string SessionId,
    [property: JsonPropertyName("toolId")] string ToolId,
    [property: JsonPropertyName("toolName")] string ToolName,
    [property: JsonPropertyName("input")] object? Input,
    [property: JsonPropertyName("createdAt")] DateTimeOffset CreatedAt
);
