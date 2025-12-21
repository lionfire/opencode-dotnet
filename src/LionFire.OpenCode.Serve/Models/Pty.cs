using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Represents a pseudo-terminal (PTY) instance.
/// </summary>
public record Pty
{
    /// <summary>
    /// The unique identifier of the PTY (pattern: ^pty.*).
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The title of the PTY.
    /// </summary>
    [JsonPropertyName("title")]
    public required string Title { get; init; }

    /// <summary>
    /// The command being executed.
    /// </summary>
    [JsonPropertyName("command")]
    public required string Command { get; init; }

    /// <summary>
    /// Arguments for the command.
    /// </summary>
    [JsonPropertyName("args")]
    public required List<string> Args { get; init; }

    /// <summary>
    /// The current working directory.
    /// </summary>
    [JsonPropertyName("cwd")]
    public required string Cwd { get; init; }

    /// <summary>
    /// The status of the PTY.
    /// </summary>
    [JsonPropertyName("status")]
    public required PtyStatus Status { get; init; }

    /// <summary>
    /// The process ID.
    /// </summary>
    [JsonPropertyName("pid")]
    public required int Pid { get; init; }
}

/// <summary>
/// Status of a PTY.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<PtyStatus>))]
public enum PtyStatus
{
    /// <summary>
    /// The PTY is currently running.
    /// </summary>
    Running,

    /// <summary>
    /// The PTY has exited.
    /// </summary>
    Exited
}

/// <summary>
/// Request to create a new PTY.
/// </summary>
public record CreatePtyRequest
{
    /// <summary>
    /// Optional title for the PTY.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }

    /// <summary>
    /// The command to execute.
    /// </summary>
    [JsonPropertyName("command")]
    public required string Command { get; init; }

    /// <summary>
    /// Arguments for the command.
    /// </summary>
    [JsonPropertyName("args")]
    public List<string>? Args { get; init; }

    /// <summary>
    /// The working directory.
    /// </summary>
    [JsonPropertyName("cwd")]
    public string? Cwd { get; init; }

    /// <summary>
    /// Environment variables.
    /// </summary>
    [JsonPropertyName("env")]
    public Dictionary<string, string>? Env { get; init; }
}

/// <summary>
/// Request to update a PTY.
/// </summary>
public record UpdatePtyRequest
{
    /// <summary>
    /// New title for the PTY.
    /// </summary>
    [JsonPropertyName("title")]
    public string? Title { get; init; }
}
