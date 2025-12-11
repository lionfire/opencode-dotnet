using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace LionFire.OpenCode.Serve.Extensions;

/// <summary>
/// Extension methods for configuring the OpenCode HttpClient.
/// </summary>
public static class HttpClientBuilderExtensions
{
    /// <summary>
    /// Configures retry behavior using built-in .NET retry mechanisms.
    /// For advanced scenarios, consider using Polly via AddPolicyHandler.
    /// </summary>
    /// <param name="builder">The HttpClient builder.</param>
    /// <param name="configureRetry">Optional delegate to configure retry options.</param>
    /// <returns>The builder for chaining.</returns>
    /// <remarks>
    /// For more advanced resilience patterns (circuit breaker, bulkhead, etc.),
    /// install Microsoft.Extensions.Http.Polly and use AddPolicyHandler.
    /// </remarks>
    /// <example>
    /// Using built-in retry:
    /// <code>
    /// services.AddOpenCodeClient()
    ///     .ConfigureRetry(retry =>
    ///     {
    ///         retry.MaxRetryAttempts = 5;
    ///         retry.RetryDelaySeconds = 3;
    ///     });
    /// </code>
    ///
    /// Using Polly (requires Microsoft.Extensions.Http.Polly):
    /// <code>
    /// services.AddOpenCodeClient()
    ///     .AddPolicyHandler(Policy.Handle&lt;HttpRequestException&gt;()
    ///         .WaitAndRetryAsync(3, i => TimeSpan.FromSeconds(Math.Pow(2, i))));
    /// </code>
    /// </example>
    public static IHttpClientBuilder ConfigureRetry(
        this IHttpClientBuilder builder,
        Action<RetryOptions>? configureRetry = null)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.Services.Configure<RetryOptions>(configureRetry ?? (_ => { }));

        return builder;
    }

    /// <summary>
    /// Sets the default timeout for HTTP operations.
    /// </summary>
    /// <param name="builder">The HttpClient builder.</param>
    /// <param name="timeout">The timeout duration.</param>
    /// <returns>The builder for chaining.</returns>
    public static IHttpClientBuilder WithTimeout(
        this IHttpClientBuilder builder,
        TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(builder);

        builder.ConfigureHttpClient(client => client.Timeout = timeout);

        return builder;
    }
}

/// <summary>
/// Options for configuring retry behavior.
/// </summary>
public class RetryOptions
{
    /// <summary>
    /// Gets or sets the maximum number of retry attempts.
    /// Defaults to 3.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Gets or sets the initial delay between retries in seconds.
    /// Defaults to 2 seconds.
    /// </summary>
    public int RetryDelaySeconds { get; set; } = 2;

    /// <summary>
    /// Gets or sets whether to use exponential backoff.
    /// Defaults to true.
    /// </summary>
    public bool UseExponentialBackoff { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum jitter to add to retry delays in milliseconds.
    /// Set to 0 to disable jitter.
    /// Defaults to 1000ms.
    /// </summary>
    public int MaxJitterMilliseconds { get; set; } = 1000;
}
