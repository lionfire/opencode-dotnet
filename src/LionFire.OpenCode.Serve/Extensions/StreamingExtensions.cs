using LionFire.OpenCode.Serve.Models;

namespace LionFire.OpenCode.Serve.Extensions;

/// <summary>
/// Extension methods for working with streaming responses.
/// </summary>
public static class StreamingExtensions
{
    /// <summary>
    /// Subscribes to a streaming response using callbacks.
    /// Useful for UI frameworks where event-based patterns are preferred.
    /// </summary>
    /// <param name="stream">The async enumerable stream.</param>
    /// <param name="onUpdate">Callback for each update (delta content).</param>
    /// <param name="onComplete">Optional callback when streaming completes.</param>
    /// <param name="onError">Optional callback for errors.</param>
    /// <param name="cancellationToken">A token to cancel the subscription.</param>
    /// <returns>A task that completes when streaming finishes.</returns>
    /// <example>
    /// <code>
    /// await client.SendMessageStreamingAsync(sessionId, "Hello")
    ///     .Subscribe(
    ///         onUpdate: delta => Console.Write(delta),
    ///         onComplete: () => Console.WriteLine("\nDone!"),
    ///         onError: ex => Console.WriteLine($"Error: {ex.Message}"));
    /// </code>
    /// </example>
    public static async Task Subscribe(
        this IAsyncEnumerable<MessageUpdate> stream,
        Action<string> onUpdate,
        Action? onComplete = null,
        Action<Exception>? onError = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(onUpdate);

        try
        {
            await foreach (var update in stream.WithCancellation(cancellationToken))
            {
                if (update.Delta is not null)
                {
                    onUpdate(update.Delta);
                }

                if (update.Done)
                {
                    onComplete?.Invoke();
                    break;
                }
            }
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            // Cancellation is expected, don't treat as error
            throw;
        }
        catch (Exception ex)
        {
            if (onError != null)
            {
                onError(ex);
            }
            else
            {
                throw;
            }
        }
    }

    /// <summary>
    /// Subscribes to a streaming response with detailed progress information.
    /// </summary>
    /// <param name="stream">The async enumerable stream.</param>
    /// <param name="onProgress">Callback for each update with full details.</param>
    /// <param name="cancellationToken">A token to cancel the subscription.</param>
    /// <returns>A task that completes when streaming finishes.</returns>
    public static async Task SubscribeWithProgress(
        this IAsyncEnumerable<MessageUpdate> stream,
        Action<StreamingProgress> onProgress,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);
        ArgumentNullException.ThrowIfNull(onProgress);

        var builder = new System.Text.StringBuilder();
        var updateCount = 0;

        await foreach (var update in stream.WithCancellation(cancellationToken))
        {
            updateCount++;

            if (update.Delta is not null)
            {
                builder.Append(update.Delta);
            }

            var progress = new StreamingProgress
            {
                MessageId = update.MessageId,
                Delta = update.Delta,
                AccumulatedContent = builder.ToString(),
                TokenCount = update.TokenCount,
                UpdateCount = updateCount,
                IsDone = update.Done
            };

            onProgress(progress);
        }
    }

    /// <summary>
    /// Collects all streaming updates into a single string.
    /// </summary>
    /// <param name="stream">The async enumerable stream.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The complete response content.</returns>
    public static async Task<string> ToStringAsync(
        this IAsyncEnumerable<MessageUpdate> stream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var builder = new System.Text.StringBuilder();

        await foreach (var update in stream.WithCancellation(cancellationToken))
        {
            if (update.Delta is not null)
            {
                builder.Append(update.Delta);
            }
        }

        return builder.ToString();
    }

    /// <summary>
    /// Collects all streaming updates into a list of updates.
    /// </summary>
    /// <param name="stream">The async enumerable stream.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of all updates.</returns>
    public static async Task<List<MessageUpdate>> ToListAsync(
        this IAsyncEnumerable<MessageUpdate> stream,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(stream);

        var result = new List<MessageUpdate>();

        await foreach (var update in stream.WithCancellation(cancellationToken))
        {
            result.Add(update);
        }

        return result;
    }
}

/// <summary>
/// Represents progress during streaming.
/// </summary>
public record StreamingProgress
{
    /// <summary>
    /// Gets the message ID being streamed.
    /// </summary>
    public string? MessageId { get; init; }

    /// <summary>
    /// Gets the latest delta content.
    /// </summary>
    public string? Delta { get; init; }

    /// <summary>
    /// Gets all content received so far.
    /// </summary>
    public required string AccumulatedContent { get; init; }

    /// <summary>
    /// Gets the token count if available.
    /// </summary>
    public int? TokenCount { get; init; }

    /// <summary>
    /// Gets the number of updates received.
    /// </summary>
    public int UpdateCount { get; init; }

    /// <summary>
    /// Gets whether streaming is complete.
    /// </summary>
    public bool IsDone { get; init; }
}
