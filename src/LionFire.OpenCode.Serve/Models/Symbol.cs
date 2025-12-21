using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Represents a code symbol.
/// </summary>
public record Symbol
{
    /// <summary>
    /// The symbol name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// The symbol kind (e.g., "function", "class", "variable").
    /// </summary>
    [JsonPropertyName("kind")]
    public required string Kind { get; init; }

    /// <summary>
    /// The file path containing the symbol.
    /// </summary>
    [JsonPropertyName("path")]
    public required string Path { get; init; }

    /// <summary>
    /// The range where the symbol is defined.
    /// </summary>
    [JsonPropertyName("range")]
    public Range? SymbolRange { get; init; }

    /// <summary>
    /// The container name (e.g., parent class).
    /// </summary>
    [JsonPropertyName("container")]
    public string? Container { get; init; }
}
