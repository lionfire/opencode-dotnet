using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Request to execute a slash command.
/// </summary>
/// <param name="Command">The command to execute (e.g., "/help", "/clear").</param>
/// <param name="Arguments">Optional arguments for the command.</param>
public record ExecuteCommandRequest(
    [property: JsonPropertyName("command")] string Command,
    [property: JsonPropertyName("arguments")] string? Arguments = null
);

/// <summary>
/// Result from executing a command.
/// </summary>
/// <param name="Output">The standard output from the command.</param>
/// <param name="Error">Any error output from the command.</param>
/// <param name="ExitCode">The exit code (0 for success).</param>
public record CommandResult(
    [property: JsonPropertyName("output")] string? Output,
    [property: JsonPropertyName("error")] string? Error,
    [property: JsonPropertyName("exitCode")] int ExitCode
);
