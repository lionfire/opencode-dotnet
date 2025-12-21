using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// LSP (Language Server Protocol) status.
/// </summary>
public record LspStatus
{
    /// <summary>
    /// Active language servers.
    /// </summary>
    [JsonPropertyName("servers")]
    public List<LspServer>? Servers { get; init; }
}

/// <summary>
/// LSP server information.
/// </summary>
public record LspServer
{
    /// <summary>
    /// The language ID.
    /// </summary>
    [JsonPropertyName("language")]
    public required string Language { get; init; }

    /// <summary>
    /// Whether the server is running.
    /// </summary>
    [JsonPropertyName("running")]
    public required bool Running { get; init; }

    /// <summary>
    /// Server capabilities.
    /// </summary>
    [JsonPropertyName("capabilities")]
    public Dictionary<string, object>? Capabilities { get; init; }
}

/// <summary>
/// Formatter status.
/// </summary>
public record FormatterStatus
{
    /// <summary>
    /// Available formatters.
    /// </summary>
    [JsonPropertyName("formatters")]
    public List<FormatterInfo>? Formatters { get; init; }
}

/// <summary>
/// Formatter information.
/// </summary>
public record FormatterInfo
{
    /// <summary>
    /// The language ID.
    /// </summary>
    [JsonPropertyName("language")]
    public required string Language { get; init; }

    /// <summary>
    /// The formatter name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Whether the formatter is available.
    /// </summary>
    [JsonPropertyName("available")]
    public required bool Available { get; init; }
}
