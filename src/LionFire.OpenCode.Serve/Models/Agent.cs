using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Represents an agent.
/// </summary>
public record Agent
{
    /// <summary>
    /// The agent name (also serves as the identifier).
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// The agent identifier (defaults to Name).
    /// </summary>
    [JsonIgnore]
    public string Id => Name;

    /// <summary>
    /// The agent description.
    /// </summary>
    [JsonPropertyName("description")]
    public string? Description { get; init; }

    /// <summary>
    /// Whether this is a built-in agent.
    /// </summary>
    [JsonPropertyName("builtIn")]
    public bool BuiltIn { get; init; }

    /// <summary>
    /// The agent mode (e.g., "subagent").
    /// </summary>
    [JsonPropertyName("mode")]
    public string? Mode { get; init; }

    /// <summary>
    /// Tool permissions for this agent.
    /// </summary>
    [JsonPropertyName("tools")]
    public Dictionary<string, bool>? Tools { get; init; }

    /// <summary>
    /// Permission settings for the agent.
    /// </summary>
    [JsonPropertyName("permission")]
    public Dictionary<string, object>? Permissions { get; init; }

    /// <summary>
    /// Agent options.
    /// </summary>
    [JsonPropertyName("options")]
    public Dictionary<string, object>? Options { get; init; }

    /// <summary>
    /// The agent configuration.
    /// </summary>
    [JsonPropertyName("config")]
    public AgentConfig? Config { get; init; }
}

/// <summary>
/// Agent configuration.
/// </summary>
public record AgentConfig
{
    /// <summary>
    /// The default model to use.
    /// </summary>
    [JsonPropertyName("model")]
    public ModelReference? Model { get; init; }

    /// <summary>
    /// System prompt for the agent.
    /// </summary>
    [JsonPropertyName("system")]
    public string? System { get; init; }

    /// <summary>
    /// Tool permissions for the agent.
    /// </summary>
    [JsonPropertyName("tools")]
    public Dictionary<string, bool>? Tools { get; init; }
}
