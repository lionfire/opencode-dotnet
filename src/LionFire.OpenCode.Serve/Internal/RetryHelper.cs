using System.Net;
using LionFire.OpenCode.Serve.Exceptions;
using Microsoft.Extensions.Logging;

namespace LionFire.OpenCode.Serve.Internal;

/// <summary>
/// Helper for implementing retry logic with exponential backoff.
/// </summary>
internal static class RetryHelper
{
    /// <summary>
    /// Executes an async operation with retry logic.
    /// </summary>
    /// <typeparam name="T">The return type of the operation.</typeparam>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="options">The client options containing retry settings.</param>
    /// <param name="logger">Optional logger for retry attempts.</param>
    /// <param name="operationName">Name of the operation for logging.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the operation.</returns>
    public static async Task<T> ExecuteWithRetryAsync<T>(
        Func<CancellationToken, Task<T>> operation,
        OpenCodeClientOptions options,
        ILogger? logger,
        string operationName,
        CancellationToken cancellationToken)
    {
        if (!options.EnableRetry || options.MaxRetryAttempts <= 0)
        {
            return await operation(cancellationToken).ConfigureAwait(false);
        }

        var attempts = 0;
        var delay = TimeSpan.FromSeconds(options.RetryDelaySeconds);

        while (true)
        {
            try
            {
                return await operation(cancellationToken).ConfigureAwait(false);
            }
            catch (Exception ex) when (ShouldRetry(ex) && attempts < options.MaxRetryAttempts)
            {
                attempts++;

                logger?.LogWarning(
                    ex,
                    "Operation {OperationName} failed (attempt {Attempt}/{MaxAttempts}). Retrying in {Delay}ms...",
                    operationName,
                    attempts,
                    options.MaxRetryAttempts,
                    delay.TotalMilliseconds);

                await Task.Delay(delay, cancellationToken).ConfigureAwait(false);

                // Exponential backoff
                delay = TimeSpan.FromTicks(delay.Ticks * 2);
            }
        }
    }

    /// <summary>
    /// Executes an async operation with retry logic (no return value).
    /// </summary>
    /// <param name="operation">The operation to execute.</param>
    /// <param name="options">The client options containing retry settings.</param>
    /// <param name="logger">Optional logger for retry attempts.</param>
    /// <param name="operationName">Name of the operation for logging.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public static async Task ExecuteWithRetryAsync(
        Func<CancellationToken, Task> operation,
        OpenCodeClientOptions options,
        ILogger? logger,
        string operationName,
        CancellationToken cancellationToken)
    {
        await ExecuteWithRetryAsync(
            async ct =>
            {
                await operation(ct).ConfigureAwait(false);
                return true;
            },
            options,
            logger,
            operationName,
            cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Determines if an exception is retryable.
    /// </summary>
    private static bool ShouldRetry(Exception ex)
    {
        return ex switch
        {
            // Retry server errors
            OpenCodeServerException => true,

            // Retry timeouts
            OpenCodeTimeoutException => true,

            // Retry connection failures
            OpenCodeConnectionException => true,

            // Retry HTTP 429 (too many requests)
            OpenCodeApiException { StatusCode: HttpStatusCode.TooManyRequests } => true,

            // Don't retry other API errors (4xx client errors)
            OpenCodeApiException => false,

            // Don't retry other OpenCode exceptions
            OpenCodeException => false,

            // Retry transient HTTP exceptions
            HttpRequestException => true,

            // Retry task canceled (timeout)
            TaskCanceledException when ex.InnerException is TimeoutException => true,

            // Don't retry actual cancellation
            _ => false
        };
    }
}
