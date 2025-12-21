using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Represents a file node in the directory tree.
/// </summary>
public record FileNode
{
    /// <summary>
    /// The name of the file or directory.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// The full path.
    /// </summary>
    [JsonPropertyName("path")]
    public required string Path { get; init; }

    /// <summary>
    /// Whether this is a directory.
    /// </summary>
    [JsonPropertyName("isDirectory")]
    public required bool IsDirectory { get; init; }

    /// <summary>
    /// Children nodes if this is a directory.
    /// </summary>
    [JsonPropertyName("children")]
    public List<FileNode>? Children { get; init; }

    /// <summary>
    /// File size in bytes (if not a directory).
    /// </summary>
    [JsonPropertyName("size")]
    public long? Size { get; init; }
}

/// <summary>
/// Represents the content of a file.
/// </summary>
public record FileContent
{
    /// <summary>
    /// The type of content (always "text" for text files).
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }

    /// <summary>
    /// The file content.
    /// </summary>
    [JsonPropertyName("content")]
    public required string Content { get; init; }

    /// <summary>
    /// The diff if available.
    /// </summary>
    [JsonPropertyName("diff")]
    public string? Diff { get; init; }

    /// <summary>
    /// Parsed patch information if available.
    /// </summary>
    [JsonPropertyName("patch")]
    public Patch? Patch { get; init; }

    /// <summary>
    /// The encoding (e.g., "base64" for binary files).
    /// </summary>
    [JsonPropertyName("encoding")]
    public string? Encoding { get; init; }

    /// <summary>
    /// The MIME type for binary files.
    /// </summary>
    [JsonPropertyName("mimeType")]
    public string? MimeType { get; init; }
}

/// <summary>
/// Represents a unified diff patch.
/// </summary>
public record Patch
{
    /// <summary>
    /// The old file name.
    /// </summary>
    [JsonPropertyName("oldFileName")]
    public required string OldFileName { get; init; }

    /// <summary>
    /// The new file name.
    /// </summary>
    [JsonPropertyName("newFileName")]
    public required string NewFileName { get; init; }

    /// <summary>
    /// The old file header.
    /// </summary>
    [JsonPropertyName("oldHeader")]
    public string? OldHeader { get; init; }

    /// <summary>
    /// The new file header.
    /// </summary>
    [JsonPropertyName("newHeader")]
    public string? NewHeader { get; init; }

    /// <summary>
    /// The diff hunks.
    /// </summary>
    [JsonPropertyName("hunks")]
    public required List<PatchHunk> Hunks { get; init; }

    /// <summary>
    /// The index line.
    /// </summary>
    [JsonPropertyName("index")]
    public string? Index { get; init; }
}

/// <summary>
/// Represents a hunk in a diff patch.
/// </summary>
public record PatchHunk
{
    /// <summary>
    /// Start line in the old file.
    /// </summary>
    [JsonPropertyName("oldStart")]
    public required int OldStart { get; init; }

    /// <summary>
    /// Number of lines in the old file.
    /// </summary>
    [JsonPropertyName("oldLines")]
    public required int OldLines { get; init; }

    /// <summary>
    /// Start line in the new file.
    /// </summary>
    [JsonPropertyName("newStart")]
    public required int NewStart { get; init; }

    /// <summary>
    /// Number of lines in the new file.
    /// </summary>
    [JsonPropertyName("newLines")]
    public required int NewLines { get; init; }

    /// <summary>
    /// The lines in the hunk.
    /// </summary>
    [JsonPropertyName("lines")]
    public required List<string> Lines { get; init; }
}

/// <summary>
/// File status information.
/// </summary>
public record FileStatus
{
    /// <summary>
    /// Modified files.
    /// </summary>
    [JsonPropertyName("modified")]
    public List<string>? Modified { get; init; }

    /// <summary>
    /// Added files.
    /// </summary>
    [JsonPropertyName("added")]
    public List<string>? Added { get; init; }

    /// <summary>
    /// Deleted files.
    /// </summary>
    [JsonPropertyName("deleted")]
    public List<string>? Deleted { get; init; }

    /// <summary>
    /// Untracked files.
    /// </summary>
    [JsonPropertyName("untracked")]
    public List<string>? Untracked { get; init; }
}
