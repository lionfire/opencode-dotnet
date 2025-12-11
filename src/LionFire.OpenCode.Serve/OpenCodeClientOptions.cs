using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Options;

namespace LionFire.OpenCode.Serve;

/// <summary>
/// Configuration options for <see cref="OpenCodeClient"/>.
/// </summary>
public class OpenCodeClientOptions
{
    /// <summary>
    /// Default base URL for the OpenCode server.
    /// </summary>
    public const string DefaultBaseUrl = "http://localhost:9123";

    /// <summary>
    /// Default timeout for quick operations (list, get, delete).
    /// </summary>
    public static readonly TimeSpan DefaultQuickTimeout = TimeSpan.FromSeconds(30);

    /// <summary>
    /// Default timeout for message operations (AI responses).
    /// </summary>
    public static readonly TimeSpan DefaultMessageTimeout = TimeSpan.FromMinutes(5);

    /// <summary>
    /// Default maximum number of retry attempts for transient failures.
    /// </summary>
    public const int DefaultMaxRetryAttempts = 3;

    /// <summary>
    /// Default delay between retry attempts in seconds.
    /// </summary>
    public const int DefaultRetryDelaySeconds = 2;

    /// <summary>
    /// Gets or sets the base URL of the OpenCode server.
    /// Defaults to <c>http://localhost:9123</c>.
    /// </summary>
    public string BaseUrl { get; set; } = DefaultBaseUrl;

    /// <summary>
    /// Gets or sets the optional working directory for the OpenCode server.
    /// When set, this directory is used as the context for file operations.
    /// </summary>
    public string? Directory { get; set; }

    /// <summary>
    /// Gets or sets the default timeout for quick operations (list, get, delete).
    /// Defaults to 30 seconds.
    /// </summary>
    public TimeSpan DefaultTimeout { get; set; } = DefaultQuickTimeout;

    /// <summary>
    /// Gets or sets the timeout for message operations that involve AI responses.
    /// Defaults to 5 minutes.
    /// </summary>
    public TimeSpan MessageTimeout { get; set; } = DefaultMessageTimeout;

    /// <summary>
    /// Gets or sets whether to enable automatic retry for transient failures.
    /// Defaults to <c>true</c>.
    /// </summary>
    public bool EnableRetry { get; set; } = true;

    /// <summary>
    /// Gets or sets the maximum number of retry attempts for transient failures.
    /// Defaults to 3.
    /// </summary>
    public int MaxRetryAttempts { get; set; } = DefaultMaxRetryAttempts;

    /// <summary>
    /// Gets or sets the delay between retry attempts in seconds.
    /// Uses exponential backoff starting from this value.
    /// Defaults to 2 seconds.
    /// </summary>
    public int RetryDelaySeconds { get; set; } = DefaultRetryDelaySeconds;

    /// <summary>
    /// Gets or sets whether to enable OpenTelemetry tracing.
    /// Defaults to true (tracing is enabled if there are listeners).
    /// </summary>
    public bool EnableTelemetry { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to validate options on startup.
    /// Defaults to true.
    /// </summary>
    public bool ValidateOnStart { get; set; } = true;

    /// <summary>
    /// Validates that the <see cref="BaseUrl"/> is a valid URI.
    /// </summary>
    /// <returns><c>true</c> if the base URL is valid; otherwise, <c>false</c>.</returns>
    public bool ValidateBaseUrl()
    {
        return Uri.TryCreate(BaseUrl, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps);
    }
}

/// <summary>
/// Validates <see cref="OpenCodeClientOptions"/> on startup.
/// </summary>
public class OpenCodeClientOptionsValidator : IValidateOptions<OpenCodeClientOptions>
{
    /// <inheritdoc />
    public ValidateOptionsResult Validate(string? name, OpenCodeClientOptions options)
    {
        var failures = new List<string>();

        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            failures.Add("BaseUrl cannot be null or empty.");
        }
        else if (!options.ValidateBaseUrl())
        {
            failures.Add($"BaseUrl '{options.BaseUrl}' is not a valid HTTP or HTTPS URI.");
        }

        if (options.DefaultTimeout <= TimeSpan.Zero)
        {
            failures.Add("DefaultTimeout must be greater than zero.");
        }

        if (options.MessageTimeout <= TimeSpan.Zero)
        {
            failures.Add("MessageTimeout must be greater than zero.");
        }

        if (options.MaxRetryAttempts < 0)
        {
            failures.Add("MaxRetryAttempts cannot be negative.");
        }

        if (options.RetryDelaySeconds < 0)
        {
            failures.Add("RetryDelaySeconds cannot be negative.");
        }

        return failures.Count > 0
            ? ValidateOptionsResult.Fail(failures)
            : ValidateOptionsResult.Success;
    }
}
