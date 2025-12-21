using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// MCP (Model Context Protocol) server status.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "status")]
[JsonDerivedType(typeof(McpStatusConnected), "connected")]
[JsonDerivedType(typeof(McpStatusDisabled), "disabled")]
[JsonDerivedType(typeof(McpStatusFailed), "failed")]
[JsonDerivedType(typeof(McpStatusNeedsAuth), "needs_auth")]
[JsonDerivedType(typeof(McpStatusNeedsClientRegistration), "needs_client_registration")]
public abstract record McpStatus
{
    /// <summary>
    /// The MCP server name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// The status type.
    /// </summary>
    [JsonPropertyName("status")]
    public required string Status { get; init; }
}

/// <summary>
/// MCP server is connected.
/// </summary>
public record McpStatusConnected : McpStatus
{
    /// <summary>
    /// Available tools.
    /// </summary>
    [JsonPropertyName("tools")]
    public List<string>? Tools { get; init; }

    /// <summary>
    /// Available resources.
    /// </summary>
    [JsonPropertyName("resources")]
    public List<string>? Resources { get; init; }

    /// <summary>
    /// Available prompts.
    /// </summary>
    [JsonPropertyName("prompts")]
    public List<string>? Prompts { get; init; }
}

/// <summary>
/// MCP server is disabled.
/// </summary>
public record McpStatusDisabled : McpStatus
{
}

/// <summary>
/// MCP server connection failed.
/// </summary>
public record McpStatusFailed : McpStatus
{
    /// <summary>
    /// The error message.
    /// </summary>
    [JsonPropertyName("error")]
    public required string Error { get; init; }
}

/// <summary>
/// MCP server needs authentication.
/// </summary>
public record McpStatusNeedsAuth : McpStatus
{
    /// <summary>
    /// The OAuth authorization URL.
    /// </summary>
    [JsonPropertyName("authUrl")]
    public required string AuthUrl { get; init; }
}

/// <summary>
/// MCP server needs client registration.
/// </summary>
public record McpStatusNeedsClientRegistration : McpStatus
{
}

/// <summary>
/// MCP server configuration.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(McpLocalConfig), "local")]
[JsonDerivedType(typeof(McpRemoteConfig), "remote")]
[JsonDerivedType(typeof(McpOAuthConfig), "oauth")]
public abstract record McpConfig
{
    /// <summary>
    /// The configuration type.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }
}

/// <summary>
/// Local MCP server configuration.
/// </summary>
public record McpLocalConfig : McpConfig
{
    /// <summary>
    /// The command to run.
    /// </summary>
    [JsonPropertyName("command")]
    public required string Command { get; init; }

    /// <summary>
    /// Command arguments.
    /// </summary>
    [JsonPropertyName("args")]
    public List<string>? Args { get; init; }

    /// <summary>
    /// Environment variables.
    /// </summary>
    [JsonPropertyName("env")]
    public Dictionary<string, string>? Env { get; init; }
}

/// <summary>
/// Remote MCP server configuration.
/// </summary>
public record McpRemoteConfig : McpConfig
{
    /// <summary>
    /// The server URL.
    /// </summary>
    [JsonPropertyName("url")]
    public required string Url { get; init; }

    /// <summary>
    /// Optional API key.
    /// </summary>
    [JsonPropertyName("apiKey")]
    public string? ApiKey { get; init; }
}

/// <summary>
/// OAuth MCP server configuration.
/// </summary>
public record McpOAuthConfig : McpConfig
{
    /// <summary>
    /// The OAuth URL.
    /// </summary>
    [JsonPropertyName("url")]
    public required string Url { get; init; }

    /// <summary>
    /// OAuth client ID.
    /// </summary>
    [JsonPropertyName("clientId")]
    public required string ClientId { get; init; }
}

/// <summary>
/// Request to add an MCP server.
/// </summary>
public record AddMcpServerRequest
{
    /// <summary>
    /// The server name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// The server configuration.
    /// </summary>
    [JsonPropertyName("config")]
    public required McpConfig Config { get; init; }
}
