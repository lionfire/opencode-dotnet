using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using LionFire.OpenCode.Serve.Exceptions;
using LionFire.OpenCode.Serve.Extensions;
using LionFire.OpenCode.Serve.Internal;
using LionFire.OpenCode.Serve.Models;

namespace LionFire.OpenCode.Serve;

/// <summary>
/// Client for communicating with an OpenCode server.
/// </summary>
/// <remarks>
/// <para>
/// This client provides access to all OpenCode server operations including session management,
/// message handling, tool operations, file access, and configuration retrieval.
/// </para>
/// <para>
/// The client is thread-safe and can be used for concurrent operations.
/// For best performance in DI scenarios, use IHttpClientFactory to manage HttpClient lifetime.
/// </para>
/// </remarks>
/// <example>
/// Basic usage:
/// <code>
/// await using var client = new OpenCodeClient();
/// var session = await client.CreateSessionAsync();
/// var response = await client.SendMessageAsync(session.Id, "Hello, OpenCode!");
/// Console.WriteLine(response.Parts.OfType&lt;TextPart&gt;().First().Text);
/// </code>
/// </example>
public sealed class OpenCodeClient : IOpenCodeClient
{
    private readonly HttpClient _httpClient;
    private readonly OpenCodeClientOptions _options;
    private readonly ILogger _logger;
    private readonly bool _ownsHttpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeClient"/> class with default options.
    /// </summary>
    /// <remarks>
    /// Uses the default base URL <c>http://localhost:9123</c>.
    /// </remarks>
    public OpenCodeClient() : this(new OpenCodeClientOptions())
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeClient"/> class with a custom base URL.
    /// </summary>
    /// <param name="baseUrl">The base URL of the OpenCode server.</param>
    public OpenCodeClient(string baseUrl) : this(new OpenCodeClientOptions { BaseUrl = baseUrl })
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeClient"/> class with options.
    /// </summary>
    /// <param name="options">The client configuration options.</param>
    /// <param name="logger">Optional logger for diagnostics.</param>
    public OpenCodeClient(OpenCodeClientOptions options, ILogger<OpenCodeClient>? logger = null)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? NullLogger<OpenCodeClient>.Instance;

        ValidateOptions(options);

        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(options.BaseUrl),
            Timeout = options.DefaultTimeout
        };
        _httpClient.DefaultRequestHeaders.Accept.ParseAdd("application/json");

        _ownsHttpClient = true;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeClient"/> class with an existing HttpClient.
    /// </summary>
    /// <param name="httpClient">The HttpClient to use for requests.</param>
    /// <param name="options">Optional client configuration options.</param>
    /// <param name="logger">Optional logger for diagnostics.</param>
    /// <remarks>
    /// When using this constructor, the HttpClient is not owned by this instance
    /// and will not be disposed when this client is disposed.
    /// </remarks>
    public OpenCodeClient(HttpClient httpClient, OpenCodeClientOptions? options = null, ILogger<OpenCodeClient>? logger = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? new OpenCodeClientOptions();
        _logger = logger ?? NullLogger<OpenCodeClient>.Instance;
        _ownsHttpClient = false;

        if (options != null)
        {
            ValidateOptions(options);
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeClient"/> class using IOptions pattern.
    /// </summary>
    /// <param name="options">The client configuration options from DI.</param>
    /// <param name="logger">Optional logger for diagnostics.</param>
    public OpenCodeClient(IOptions<OpenCodeClientOptions> options, ILogger<OpenCodeClient>? logger = null)
        : this(options?.Value ?? throw new ArgumentNullException(nameof(options)), logger)
    {
    }

    private static void ValidateOptions(OpenCodeClientOptions options)
    {
        var validator = new OpenCodeClientOptionsValidator();
        var result = validator.Validate(null, options);
        if (result.Failed)
        {
            throw new ArgumentException(
                $"Invalid OpenCodeClientOptions: {string.Join("; ", result.Failures ?? Array.Empty<string>())}",
                nameof(options));
        }
    }

    #region Health Check

    /// <inheritdoc />
    public async Task<HealthCheckResult> PingAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Pinging OpenCode server at {BaseUrl}", _options.BaseUrl);

        try
        {
            // OpenCode doesn't have a dedicated health endpoint, so we use /config
            // and synthesize a health check result from it
            var config = await _httpClient.GetJsonAsync<OpenCodeConfig>(ApiEndpoints.Health, cancellationToken).ConfigureAwait(false);
            return new HealthCheckResult(
                Status: "ok",
                Version: config.Version
            );
        }
        catch (OpenCodeConnectionException)
        {
            throw;
        }
        catch (OpenCodeApiException ex)
        {
            throw OpenCodeConnectionException.ServerNotReachable(_options.BaseUrl, ex);
        }
    }

    #endregion

    #region Session Management

    /// <inheritdoc />
    public Task<Session> CreateSessionAsync(string? directory = null, CancellationToken cancellationToken = default)
        => CreateSessionAsync(directory, parentId: null, title: null, cancellationToken);

    /// <inheritdoc />
    public async Task<Session> CreateSessionAsync(string? directory, string? parentId, string? title, CancellationToken cancellationToken = default)
    {
        var effectiveDirectory = directory ?? _options.Directory;
        _logger.LogDebug("Creating new session with directory: {Directory}, parentId: {ParentId}, title: {Title}",
            effectiveDirectory ?? "(default)", parentId ?? "(none)", title ?? "(none)");

        var url = ApiEndpoints.SessionsWithDirectory(effectiveDirectory);
        var request = new CreateSessionRequest(parentId, title);
        return await _httpClient.PostJsonAsync<Session>(url, request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Session> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);

        _logger.LogDebug("Getting session: {SessionId}", sessionId);
        return await _httpClient.GetJsonAsync<Session>(ApiEndpoints.Session(sessionId), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Session>> ListSessionsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Listing all sessions");
        return await _httpClient.GetJsonAsync<List<Session>>(ApiEndpoints.Sessions, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task DeleteSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);

        _logger.LogDebug("Deleting session: {SessionId}", sessionId);
        await _httpClient.DeleteAsync(ApiEndpoints.Session(sessionId), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Session> ForkSessionAsync(string sessionId, string messageId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);
        ArgumentException.ThrowIfNullOrEmpty(messageId);

        _logger.LogDebug("Forking session {SessionId} at message {MessageId}", sessionId, messageId);

        var request = new ForkSessionRequest(messageId);
        return await _httpClient.PostJsonAsync<Session>(ApiEndpoints.SessionFork(sessionId), request, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task AbortSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);

        _logger.LogDebug("Aborting session: {SessionId}", sessionId);
        await _httpClient.PostJsonAsync(ApiEndpoints.SessionAbort(sessionId), null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Session> ShareSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);

        _logger.LogDebug("Sharing session: {SessionId}", sessionId);
        return await _httpClient.PostJsonAsync<Session>(ApiEndpoints.SessionShare(sessionId), null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task UnshareSessionAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);

        _logger.LogDebug("Unsharing session: {SessionId}", sessionId);
        await _httpClient.PostJsonAsync(ApiEndpoints.SessionUnshare(sessionId), null, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<ISessionScope> CreateSessionScopeAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var session = await CreateSessionAsync(directory, cancellationToken).ConfigureAwait(false);
        return new SessionScope(this, session);
    }

    #endregion

    #region Message Operations

    /// <inheritdoc />
    public async Task<Message> SendMessageAsync(string sessionId, IReadOnlyList<MessagePart> parts, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);
        ArgumentNullException.ThrowIfNull(parts);

        _logger.LogDebug("Sending message to session {SessionId} with {PartCount} parts", sessionId, parts.Count);

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        cts.CancelAfter(_options.MessageTimeout);

        var request = new SendMessageRequest(parts);

        // OpenCode's /session/{id}/message endpoint returns a streaming response
        // We need to read the stream to get the complete message
        var httpRequest = new HttpRequestMessage(HttpMethod.Post, ApiEndpoints.Messages(sessionId))
        {
            Content = JsonContent.Create(request, options: Internal.JsonOptions.Default)
        };

        var response = await _httpClient.SendAsync(httpRequest, HttpCompletionOption.ResponseHeadersRead, cts.Token).ConfigureAwait(false);
        response.EnsureSuccessStatusCode();

        // Read the streamed JSON response
        await using var stream = await response.Content.ReadAsStreamAsync(cts.Token).ConfigureAwait(false);
        var message = await JsonSerializer.DeserializeAsync<Message>(stream, Internal.JsonOptions.Default, cts.Token).ConfigureAwait(false);

        return message ?? throw new OpenCodeException("Server returned null message");
    }

    /// <inheritdoc />
    public Task<Message> SendMessageAsync(string sessionId, string text, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);
        return SendMessageAsync(sessionId, new MessagePart[] { new TextPart(text) }, cancellationToken);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<MessageUpdate> SendMessageStreamingAsync(
        string sessionId,
        IReadOnlyList<MessagePart> parts,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);
        ArgumentNullException.ThrowIfNull(parts);

        _logger.LogDebug("Sending streaming message to session {SessionId} with {PartCount} parts", sessionId, parts.Count);

        var request = new SendMessageRequest(parts);

        await foreach (var update in _httpClient.PostServerSentEventsAsync<MessageUpdate>(
            ApiEndpoints.MessagesStream(sessionId),
            request,
            cancellationToken).ConfigureAwait(false))
        {
            yield return update;
        }
    }

    /// <inheritdoc />
    public IAsyncEnumerable<MessageUpdate> SendMessageStreamingAsync(string sessionId, string text, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(text);
        return SendMessageStreamingAsync(sessionId, new MessagePart[] { new TextPart(text) }, cancellationToken);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Message>> GetMessagesAsync(string sessionId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);

        _logger.LogDebug("Getting messages for session: {SessionId}", sessionId);
        return await _httpClient.GetJsonAsync<List<Message>>(ApiEndpoints.Messages(sessionId), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<Message> GetMessageAsync(string sessionId, string messageId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);
        ArgumentException.ThrowIfNullOrEmpty(messageId);

        _logger.LogDebug("Getting message {MessageId} from session {SessionId}", messageId, sessionId);
        return await _httpClient.GetJsonAsync<Message>(ApiEndpoints.Message(sessionId, messageId), cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Tool Operations

    /// <inheritdoc />
    public Task<IReadOnlyList<Tool>> GetToolsAsync(CancellationToken cancellationToken = default)
    {
        // Note: OpenCode has /experimental/tool endpoint with different structure.
        // This needs redesign to match the actual API.
        throw new NotSupportedException("GetToolsAsync is not yet implemented. OpenCode uses /experimental/tool with different response format.");
    }

    /// <inheritdoc />
    public Task<Tool> GetToolAsync(string toolId, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("GetToolAsync is not supported. Use GetToolsAsync to list all tools.");
    }

    /// <inheritdoc />
    public Task ApproveToolAsync(string sessionId, string toolId, CancellationToken cancellationToken = default)
    {
        // Note: OpenCode uses /session/{id}/permissions/{permissionId} with POST and body { allow: true/false }
        throw new NotSupportedException("ApproveToolAsync needs redesign for OpenCode permissions API.");
    }

    /// <inheritdoc />
    public Task DenyToolAsync(string sessionId, string toolId, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("DenyToolAsync needs redesign for OpenCode permissions API.");
    }

    /// <inheritdoc />
    public Task UpdateToolPermissionsAsync(string sessionId, string toolId, ToolPermission permission, CancellationToken cancellationToken = default)
    {
        throw new NotSupportedException("UpdateToolPermissionsAsync needs redesign for OpenCode permissions API.");
    }

    #endregion

    #region File Operations

    /// <inheritdoc />
    public async Task<IReadOnlyList<OpenCodeFileInfo>> ListFilesAsync(string sessionId, string? path = null, CancellationToken cancellationToken = default)
    {
        // Note: OpenCode file operations are instance-scoped, not session-scoped.
        // The sessionId parameter is kept for API compatibility but not used.
        _logger.LogDebug("Listing files at path: {Path}", path ?? "(root)");

        var uri = ApiEndpoints.Files(path);
        return await _httpClient.GetJsonAsync<List<OpenCodeFileInfo>>(uri, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<FileContent> GetFileContentAsync(string sessionId, string path, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(path);

        // Note: OpenCode file operations are instance-scoped, not session-scoped.
        _logger.LogDebug("Getting file content at path: {Path}", path);
        return await _httpClient.GetJsonAsync<FileContent>(ApiEndpoints.FileContent(path), cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public Task<SearchFilesResult> SearchFilesAsync(string sessionId, string query, string? path = null, int? maxResults = null, CancellationToken cancellationToken = default)
    {
        // Note: OpenCode uses /find and /find/file endpoints which have different response formats.
        // This needs redesign to match the actual API.
        throw new NotSupportedException("SearchFilesAsync is not yet implemented for the OpenCode serve API. Use the /find endpoints directly.");
    }

    /// <inheritdoc />
    public Task<ApplyChangesResult> ApplyChangesAsync(string sessionId, IReadOnlyList<FileChange> changes, CancellationToken cancellationToken = default)
    {
        // Note: OpenCode doesn't have a batch file apply endpoint.
        // File changes are made through tool calls in the AI conversation.
        throw new NotSupportedException("ApplyChangesAsync is not supported by the OpenCode serve API. File changes are made through AI tool calls.");
    }

    #endregion

    #region Command Operations

    /// <inheritdoc />
    public async Task<CommandResult> ExecuteCommandAsync(string sessionId, string command, string? arguments = null, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(sessionId);
        ArgumentException.ThrowIfNullOrEmpty(command);

        _logger.LogDebug("Executing command {Command} for session {SessionId}", command, sessionId);

        var request = new ExecuteCommandRequest(command, arguments);
        return await _httpClient.PostJsonAsync<CommandResult>(ApiEndpoints.Commands(sessionId), request, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Configuration

    /// <inheritdoc />
    public async Task<OpenCodeConfig> GetConfigAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting OpenCode configuration");
        return await _httpClient.GetJsonAsync<OpenCodeConfig>(ApiEndpoints.Config, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Provider>> GetProvidersAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting available providers");
        return await _httpClient.GetJsonAsync<List<Provider>>(ApiEndpoints.Providers, cancellationToken).ConfigureAwait(false);
    }

    /// <inheritdoc />
    public async Task<IReadOnlyList<Model>> GetModelsAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug("Getting available models");
        return await _httpClient.GetJsonAsync<List<Model>>(ApiEndpoints.Models, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region IAsyncDisposable

    /// <inheritdoc />
    public ValueTask DisposeAsync()
    {
        if (_ownsHttpClient)
        {
            _httpClient.Dispose();
        }

        return ValueTask.CompletedTask;
    }

    #endregion

    #region Nested Types

    /// <summary>
    /// Implementation of <see cref="ISessionScope"/> that automatically deletes the session on disposal.
    /// </summary>
    private sealed class SessionScope : ISessionScope
    {
        private readonly OpenCodeClient _client;
        private bool _disposed;

        public SessionScope(OpenCodeClient client, Session session)
        {
            _client = client;
            Session = session;
        }

        public Session Session { get; }

        public async ValueTask DisposeAsync()
        {
            if (_disposed)
                return;

            _disposed = true;

            try
            {
                await _client.DeleteSessionAsync(Session.Id).ConfigureAwait(false);
            }
            catch (OpenCodeNotFoundException)
            {
                // Session already deleted, ignore
            }
            catch (OpenCodeConnectionException)
            {
                // Server not available, can't clean up
            }
        }
    }

    #endregion
}
