using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Represents information about a file in the OpenCode context.
/// Named <c>OpenCodeFileInfo</c> to avoid conflict with <see cref="System.IO.FileInfo"/>.
/// </summary>
/// <param name="Path">The relative path of the file.</param>
/// <param name="Size">The size of the file in bytes.</param>
/// <param name="IsDirectory">Whether this entry is a directory.</param>
/// <param name="LastModified">When the file was last modified.</param>
public record OpenCodeFileInfo(
    [property: JsonPropertyName("path")] string Path,
    [property: JsonPropertyName("size")] long Size,
    [property: JsonPropertyName("isDirectory")] bool IsDirectory,
    [property: JsonPropertyName("lastModified")] DateTimeOffset? LastModified = null
);

/// <summary>
/// Represents the content of a file.
/// </summary>
/// <param name="Path">The path of the file.</param>
/// <param name="Content">The content of the file.</param>
/// <param name="Encoding">The encoding used for the content.</param>
public record FileContent(
    [property: JsonPropertyName("path")] string Path,
    [property: JsonPropertyName("content")] string Content,
    [property: JsonPropertyName("encoding")] string Encoding = "utf-8"
);

/// <summary>
/// Request to search for files.
/// </summary>
/// <param name="Query">The search query.</param>
/// <param name="Path">Optional path to search within.</param>
/// <param name="MaxResults">Maximum number of results to return.</param>
public record SearchFilesRequest(
    [property: JsonPropertyName("query")] string Query,
    [property: JsonPropertyName("path")] string? Path = null,
    [property: JsonPropertyName("maxResults")] int? MaxResults = null
);

/// <summary>
/// Result from a file search.
/// </summary>
/// <param name="Files">The files matching the search.</param>
/// <param name="TotalCount">The total number of matches.</param>
public record SearchFilesResult(
    [property: JsonPropertyName("files")] IReadOnlyList<OpenCodeFileInfo> Files,
    [property: JsonPropertyName("totalCount")] int TotalCount
);

/// <summary>
/// Represents a file change to apply.
/// </summary>
/// <param name="Path">The path of the file to change.</param>
/// <param name="Content">The new content for the file.</param>
/// <param name="Operation">The type of operation (create, update, delete).</param>
public record FileChange(
    [property: JsonPropertyName("path")] string Path,
    [property: JsonPropertyName("content")] string? Content,
    [property: JsonPropertyName("operation")] FileOperation Operation = FileOperation.Update
);

/// <summary>
/// The type of file operation.
/// </summary>
[JsonConverter(typeof(JsonStringEnumConverter<FileOperation>))]
public enum FileOperation
{
    /// <summary>
    /// Create a new file.
    /// </summary>
    Create,

    /// <summary>
    /// Update an existing file.
    /// </summary>
    Update,

    /// <summary>
    /// Delete a file.
    /// </summary>
    Delete
}

/// <summary>
/// Request to apply file changes.
/// </summary>
/// <param name="Changes">The changes to apply.</param>
public record ApplyChangesRequest(
    [property: JsonPropertyName("changes")] IReadOnlyList<FileChange> Changes
);

/// <summary>
/// Result from applying file changes.
/// </summary>
/// <param name="Applied">The changes that were applied successfully.</param>
/// <param name="Errors">Any errors that occurred.</param>
public record ApplyChangesResult(
    [property: JsonPropertyName("applied")] IReadOnlyList<string> Applied,
    [property: JsonPropertyName("errors")] IReadOnlyList<FileChangeError>? Errors = null
);

/// <summary>
/// Represents an error applying a file change.
/// </summary>
/// <param name="Path">The path of the file that failed.</param>
/// <param name="Error">The error message.</param>
public record FileChangeError(
    [property: JsonPropertyName("path")] string Path,
    [property: JsonPropertyName("error")] string Error
);
