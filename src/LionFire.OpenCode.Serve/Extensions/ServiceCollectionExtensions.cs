using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;

namespace LionFire.OpenCode.Serve.Extensions;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to register OpenCode client services.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// The name used for the HttpClient in IHttpClientFactory.
    /// </summary>
    public const string HttpClientName = "OpenCodeClient";

    /// <summary>
    /// Adds the OpenCode client to the service collection with default options.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to further configure the HttpClient.</returns>
    public static IHttpClientBuilder AddOpenCodeClient(this IServiceCollection services)
    {
        return services.AddOpenCodeClient(_ => { });
    }

    /// <summary>
    /// Adds the OpenCode client to the service collection with configured options.
    /// Uses <see cref="IHttpClientFactory"/> for proper HttpClient lifecycle management.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="configureOptions">A delegate to configure the <see cref="OpenCodeClientOptions"/>.</param>
    /// <returns>An <see cref="IHttpClientBuilder"/> that can be used to further configure the HttpClient.</returns>
    /// <example>
    /// <code>
    /// services.AddOpenCodeClient(options =>
    /// {
    ///     options.BaseUrl = "http://localhost:9000";
    ///     options.MessageTimeout = TimeSpan.FromMinutes(10);
    /// })
    /// .AddPolicyHandler(myRetryPolicy); // Optional Polly integration
    /// </code>
    /// </example>
    public static IHttpClientBuilder AddOpenCodeClient(
        this IServiceCollection services,
        Action<OpenCodeClientOptions> configureOptions)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configureOptions);

        services.Configure(configureOptions);
        services.TryAddSingleton<IValidateOptions<OpenCodeClientOptions>, OpenCodeClientOptionsValidator>();

        // Register the OpenCodeClient using HttpClientFactory
        var builder = services.AddHttpClient<IOpenCodeClient, OpenCodeClient>(HttpClientName, (sp, client) =>
        {
            var options = sp.GetRequiredService<IOptions<OpenCodeClientOptions>>().Value;
            client.BaseAddress = new Uri(options.BaseUrl);
            client.Timeout = options.DefaultTimeout;
            client.DefaultRequestHeaders.Accept.ParseAdd("application/json");
        });

        return builder;
    }

    /// <summary>
    /// Adds the OpenCode client using an existing HttpClient instance.
    /// Use this when you want full control over the HttpClient lifecycle.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="httpClient">The pre-configured HttpClient to use.</param>
    /// <param name="configureOptions">Optional delegate to configure options.</param>
    /// <returns>The service collection for chaining.</returns>
    public static IServiceCollection AddOpenCodeClient(
        this IServiceCollection services,
        HttpClient httpClient,
        Action<OpenCodeClientOptions>? configureOptions = null)
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(httpClient);

        if (configureOptions != null)
        {
            services.Configure(configureOptions);
        }
        else
        {
            services.Configure<OpenCodeClientOptions>(_ => { });
        }

        services.TryAddSingleton<IValidateOptions<OpenCodeClientOptions>, OpenCodeClientOptionsValidator>();
        services.TryAddSingleton<IOpenCodeClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<OpenCodeClientOptions>>().Value;
            var logger = sp.GetService<Microsoft.Extensions.Logging.ILogger<OpenCodeClient>>();
            return new OpenCodeClient(httpClient, options, logger);
        });

        return services;
    }
}
