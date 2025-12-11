using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Options for paginating results.
/// </summary>
public record PaginationOptions
{
    /// <summary>
    /// Gets or sets the number of items to skip.
    /// </summary>
    public int? Skip { get; init; }

    /// <summary>
    /// Gets or sets the maximum number of items to return.
    /// </summary>
    public int? Take { get; init; }

    /// <summary>
    /// Creates pagination options for skipping and taking.
    /// </summary>
    /// <param name="skip">Number of items to skip.</param>
    /// <param name="take">Maximum number of items to return.</param>
    /// <returns>A new <see cref="PaginationOptions"/> instance.</returns>
    public static PaginationOptions Create(int skip, int take) => new() { Skip = skip, Take = take };

    /// <summary>
    /// Creates pagination options for the specified page (1-indexed).
    /// </summary>
    /// <param name="page">The page number (1-indexed).</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <returns>A new <see cref="PaginationOptions"/> instance.</returns>
    public static PaginationOptions Page(int page, int pageSize = 20)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(page, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(pageSize, 1);
        return new() { Skip = (page - 1) * pageSize, Take = pageSize };
    }
}

/// <summary>
/// A paginated result set.
/// </summary>
/// <typeparam name="T">The type of items in the result.</typeparam>
public record PaginatedResult<T>
{
    /// <summary>
    /// Gets or sets the items in this page.
    /// </summary>
    [JsonPropertyName("items")]
    public required IReadOnlyList<T> Items { get; init; }

    /// <summary>
    /// Gets or sets the total count of items across all pages.
    /// </summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; init; }

    /// <summary>
    /// Gets or sets the number of items skipped.
    /// </summary>
    [JsonPropertyName("skip")]
    public int Skip { get; init; }

    /// <summary>
    /// Gets or sets the maximum number of items requested.
    /// </summary>
    [JsonPropertyName("take")]
    public int? Take { get; init; }

    /// <summary>
    /// Gets whether there are more items available.
    /// </summary>
    [JsonIgnore]
    public bool HasMore => Skip + Items.Count < TotalCount;

    /// <summary>
    /// Gets the current page number (1-indexed).
    /// </summary>
    /// <param name="pageSize">The page size used.</param>
    /// <returns>The current page number.</returns>
    public int GetPage(int pageSize) => pageSize > 0 ? (Skip / pageSize) + 1 : 1;
}

/// <summary>
/// Options for filtering sessions.
/// </summary>
public record SessionFilterOptions
{
    /// <summary>
    /// Gets or sets the status to filter by.
    /// </summary>
    public SessionStatus? Status { get; init; }

    /// <summary>
    /// Gets or sets the minimum creation date.
    /// </summary>
    public DateTimeOffset? CreatedAfter { get; init; }

    /// <summary>
    /// Gets or sets the maximum creation date.
    /// </summary>
    public DateTimeOffset? CreatedBefore { get; init; }

    /// <summary>
    /// Gets or sets the directory to filter by.
    /// </summary>
    public string? Directory { get; init; }

    /// <summary>
    /// Converts to query string parameters.
    /// </summary>
    internal Dictionary<string, string> ToQueryParameters()
    {
        var parameters = new Dictionary<string, string>();

        if (Status.HasValue)
            parameters["status"] = Status.Value.ToString().ToLowerInvariant();

        if (CreatedAfter.HasValue)
            parameters["createdAfter"] = CreatedAfter.Value.ToString("O");

        if (CreatedBefore.HasValue)
            parameters["createdBefore"] = CreatedBefore.Value.ToString("O");

        if (!string.IsNullOrEmpty(Directory))
            parameters["directory"] = Directory;

        return parameters;
    }
}
