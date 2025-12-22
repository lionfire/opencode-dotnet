using System.Diagnostics;
using System.Net.Http.Json;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using LionFire.OpenCode.Serve.Exceptions;
using LionFire.OpenCode.Serve.Internal;
using LionFire.OpenCode.Serve.Models;
using LionFire.OpenCode.Serve.Models.Events;

namespace LionFire.OpenCode.Serve;

/// <summary>
/// Client for communicating with an OpenCode serve API instance.
/// </summary>
/// <remarks>
/// <para>
/// This client provides access to all OpenCode serve API operations including session management,
/// message handling, PTY operations, MCP servers, file access, and more.
/// </para>
/// <para>
/// The client is thread-safe and can be used for concurrent operations.
/// Based on OpenCode serve API specification version 0.0.3.
/// </para>
/// </remarks>
public sealed class OpenCodeClient : IOpenCodeClient
{
    private readonly HttpClient _httpClient;
    private readonly OpenCodeClientOptions _options;
    private readonly ILogger _logger;
    private readonly bool _ownsHttpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeClient"/> class with default options.
    /// </summary>
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
    public OpenCodeClient(HttpClient httpClient, OpenCodeClientOptions? options = null, ILogger<OpenCodeClient>? logger = null)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _options = options ?? new OpenCodeClientOptions();
        _logger = logger ?? NullLogger<OpenCodeClient>.Instance;
        _ownsHttpClient = false;
    }

    private static void ValidateOptions(OpenCodeClientOptions options)
    {
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
            throw new ArgumentException("BaseUrl cannot be null or empty.", nameof(options));

        if (!Uri.TryCreate(options.BaseUrl, UriKind.Absolute, out _))
            throw new ArgumentException("BaseUrl must be a valid absolute URI.", nameof(options));
    }

    private string BuildUrl(string endpoint, string? directory = null, Dictionary<string, string>? queryParams = null)
    {
        var url = new StringBuilder(endpoint);
        var hasQuery = endpoint.Contains('?');

        if (directory != null)
        {
            url.Append(hasQuery ? "&" : "?");
            url.Append($"directory={Uri.EscapeDataString(directory)}");
            hasQuery = true;
        }

        if (queryParams != null)
        {
            foreach (var param in queryParams)
            {
                url.Append(hasQuery ? "&" : "?");
                url.Append($"{param.Key}={Uri.EscapeDataString(param.Value)}");
                hasQuery = true;
            }
        }

        return url.ToString();
    }

    #region Global Events

    public async IAsyncEnumerable<GlobalEvent> SubscribeToGlobalEventsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await foreach (var ev in StreamServerSentEventsAsync<GlobalEvent>(ApiEndpoints.GlobalEvent, null, cancellationToken))
        {
            yield return ev;
        }
    }

    #endregion

    #region Projects

    public async Task<List<Project>> ListProjectsAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Projects, directory);
        return await GetAsync<List<Project>>(url, cancellationToken) ?? new List<Project>();
    }

    public async Task<Project> GetCurrentProjectAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.ProjectCurrent, directory);
        return await GetAsync<Project>(url, cancellationToken)
            ?? throw new OpenCodeException("No current project found");
    }

    #endregion

    #region Sessions

    public async Task<List<Session>> ListSessionsAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Sessions, directory);
        return await GetAsync<List<Session>>(url, cancellationToken) ?? new List<Session>();
    }

    /// <inheritdoc />
    public async Task<List<Session>> ListSessionsAsync(SessionListOptions options, string? directory = null, CancellationToken cancellationToken = default)
    {
        var queryParams = options.ToQueryParameters();
        var url = BuildUrl(ApiEndpoints.Sessions, directory, queryParams);
        return await GetAsync<List<Session>>(url, cancellationToken) ?? new List<Session>();
    }

    public async Task<Session> CreateSessionAsync(CreateSessionRequest? request = null, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Sessions, directory);
        // API requires an object, even if empty - pass empty object if request is null
        var body = request ?? new CreateSessionRequest();
        return await PostAsync<CreateSessionRequest, Session>(url, body, cancellationToken)
            ?? throw new OpenCodeException("Failed to create session");
    }

    public async Task<SessionStatus> GetSessionStatusAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionStatus, directory);
        return await GetAsync<SessionStatus>(url, cancellationToken)
            ?? throw new OpenCodeException("Failed to get session status");
    }

    public async Task<Session> GetSessionAsync(string sessionId, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Session(sessionId), directory);
        return await GetAsync<Session>(url, cancellationToken)
            ?? throw new OpenCodeNotFoundException($"Session {sessionId} not found");
    }

    public async Task DeleteSessionAsync(string sessionId, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Session(sessionId), directory);
        await DeleteAsync(url, cancellationToken);
    }

    public async Task<Session> UpdateSessionAsync(string sessionId, UpdateSessionRequest request, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Session(sessionId), directory);
        return await PatchAsync<UpdateSessionRequest, Session>(url, request, cancellationToken)
            ?? throw new OpenCodeException($"Failed to update session {sessionId}");
    }

    public async Task AbortSessionAsync(string sessionId, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionAbort(sessionId), directory);
        await PostAsync<object?, object>(url, null, cancellationToken);
    }

    public async Task<List<Session>> GetSessionChildrenAsync(string sessionId, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionChildren(sessionId), directory);
        return await GetAsync<List<Session>>(url, cancellationToken) ?? new List<Session>();
    }

    public async Task ExecuteSessionCommandAsync(string sessionId, ExecuteCommandRequest request, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionCommand(sessionId), directory);
        await PostAsync<ExecuteCommandRequest, object>(url, request, cancellationToken);
    }

    public async Task<List<FileDiff>> GetSessionDiffAsync(string sessionId, string? messageId = null, string? directory = null, CancellationToken cancellationToken = default)
    {
        var queryParams = messageId != null ? new Dictionary<string, string> { ["messageID"] = messageId } : null;
        var url = BuildUrl(ApiEndpoints.SessionDiff(sessionId), directory, queryParams);
        return await GetAsync<List<FileDiff>>(url, cancellationToken) ?? new List<FileDiff>();
    }

    public async Task<Session> ForkSessionAsync(string sessionId, ForkSessionRequest? request = null, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionFork(sessionId), directory);
        return await PostAsync<ForkSessionRequest?, Session>(url, request, cancellationToken)
            ?? throw new OpenCodeException($"Failed to fork session {sessionId}");
    }

    public async Task InitializeSessionAsync(string sessionId, InitSessionRequest request, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionInit(sessionId), directory);
        await PostAsync<InitSessionRequest, object>(url, request, cancellationToken);
    }

    public async Task RevertSessionAsync(string sessionId, RevertSessionRequest request, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionRevert(sessionId), directory);
        await PostAsync<RevertSessionRequest, object>(url, request, cancellationToken);
    }

    public async Task UnrevertSessionAsync(string sessionId, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionUnrevert(sessionId), directory);
        await PostAsync<object?, object>(url, null, cancellationToken);
    }

    public async Task<Session> ShareSessionAsync(string sessionId, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionShare(sessionId), directory);
        return await PostAsync<object?, Session>(url, null, cancellationToken)
            ?? throw new OpenCodeException($"Failed to share session {sessionId}");
    }

    public async Task UnshareSessionAsync(string sessionId, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionUnshare(sessionId), directory);
        await DeleteAsync(url, cancellationToken);
    }

    public async Task ExecuteSessionShellAsync(string sessionId, ExecuteShellRequest request, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionShell(sessionId), directory);
        await PostAsync<ExecuteShellRequest, object>(url, request, cancellationToken);
    }

    public async Task SummarizeSessionAsync(string sessionId, SummarizeSessionRequest? request = null, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionSummarize(sessionId), directory);
        await PostAsync<SummarizeSessionRequest?, object>(url, request, cancellationToken);
    }

    public async Task<List<Todo>> GetSessionTodosAsync(string sessionId, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionTodo(sessionId), directory);
        return await GetAsync<List<Todo>>(url, cancellationToken) ?? new List<Todo>();
    }

    #endregion

    #region Messages

    public async Task<List<MessageWithParts>> ListMessagesAsync(string sessionId, int? limit = null, string? directory = null, CancellationToken cancellationToken = default)
    {
        var queryParams = limit.HasValue ? new Dictionary<string, string> { ["limit"] = limit.Value.ToString() } : null;
        var url = BuildUrl(ApiEndpoints.SessionMessages(sessionId), directory, queryParams);
        return await GetAsync<List<MessageWithParts>>(url, cancellationToken) ?? new List<MessageWithParts>();
    }

    /// <inheritdoc />
    public async Task<List<MessageWithParts>> ListMessagesAsync(
        string sessionId,
        MessageListOptions options,
        string? directory = null,
        CancellationToken cancellationToken = default)
    {
        options.Validate();
        var queryParams = options.ToQueryParameters();
        var url = BuildUrl(ApiEndpoints.SessionMessages(sessionId), directory, queryParams);
        return await GetAsync<List<MessageWithParts>>(url, cancellationToken) ?? new List<MessageWithParts>();
    }

    public async Task<MessageWithParts> PromptAsync(string sessionId, SendMessageRequest request, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionPrompt(sessionId), directory);
        return await PostAsync<SendMessageRequest, MessageWithParts>(url, request, cancellationToken)
            ?? throw new OpenCodeException("Failed to send prompt");
    }

    /// <inheritdoc />
    public async Task<MessageWithParts> PromptAsync(
        string sessionId,
        SendMessageRequest request,
        TimeSpan timeout,
        string? directory = null,
        CancellationToken cancellationToken = default)
    {
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            var url = BuildUrl(ApiEndpoints.SessionPrompt(sessionId), directory);
            return await PostAsync<SendMessageRequest, MessageWithParts>(url, request, linkedCts.Token)
                ?? throw new OpenCodeException("Failed to send prompt");
        }
        catch (OperationCanceledException) when (timeoutCts.IsCancellationRequested && !cancellationToken.IsCancellationRequested)
        {
            throw new TimeoutException($"The prompt operation timed out after {timeout.TotalSeconds:F1} seconds.");
        }
    }

    public async Task PromptAsyncNonBlocking(string sessionId, SendMessageRequest request, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionPromptAsync(sessionId), directory);
        await PostAsync<SendMessageRequest, object>(url, request, cancellationToken);
    }

    public async Task<MessageWithParts> GetMessageAsync(string sessionId, string messageId, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionMessage(sessionId, messageId), directory);
        return await GetAsync<MessageWithParts>(url, cancellationToken)
            ?? throw new OpenCodeNotFoundException($"Message {messageId} not found in session {sessionId}");
    }

    #endregion

    #region Permissions

    public async Task RespondToPermissionAsync(string sessionId, string permissionId, PermissionResponse response, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.SessionPermission(sessionId, permissionId), directory);
        await PostAsync<PermissionResponse, object>(url, response, cancellationToken);
    }

    #endregion

    #region Files

    public async Task<List<FileNode>> ListFilesAsync(string path, string? directory = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string> { ["path"] = path };
        var url = BuildUrl(ApiEndpoints.Files, directory, queryParams);
        return await GetAsync<List<FileNode>>(url, cancellationToken) ?? new List<FileNode>();
    }

    public async Task<FileContent> ReadFileAsync(string path, string? directory = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string> { ["path"] = path };
        var url = BuildUrl(ApiEndpoints.FileContent, directory, queryParams);
        return await GetAsync<FileContent>(url, cancellationToken)
            ?? throw new OpenCodeNotFoundException($"File {path} not found");
    }

    public async Task<FileStatus> GetFileStatusAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.FileStatus, directory);
        return await GetAsync<FileStatus>(url, cancellationToken)
            ?? new FileStatus();
    }

    #endregion

    #region Find

    public async Task<List<string>> FindTextAsync(string query, string? directory = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string> { ["query"] = query };
        var url = BuildUrl(ApiEndpoints.FindText, directory, queryParams);
        return await GetAsync<List<string>>(url, cancellationToken) ?? new List<string>();
    }

    public async Task<List<string>> FindFilesAsync(string query, string? dirs = null, string? directory = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string> { ["query"] = query };
        if (dirs != null)
            queryParams["dirs"] = dirs;
        var url = BuildUrl(ApiEndpoints.FindFiles, directory, queryParams);
        return await GetAsync<List<string>>(url, cancellationToken) ?? new List<string>();
    }

    public async Task<List<Symbol>> FindSymbolsAsync(string query, string? directory = null, CancellationToken cancellationToken = default)
    {
        var queryParams = new Dictionary<string, string> { ["query"] = query };
        var url = BuildUrl(ApiEndpoints.FindSymbols, directory, queryParams);
        return await GetAsync<List<Symbol>>(url, cancellationToken) ?? new List<Symbol>();
    }

    #endregion

    #region PTY (Pseudo-Terminal)

    public async Task<List<Pty>> ListPtysAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Ptys, directory);
        return await GetAsync<List<Pty>>(url, cancellationToken) ?? new List<Pty>();
    }

    public async Task<Pty> CreatePtyAsync(CreatePtyRequest request, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Ptys, directory);
        return await PostAsync<CreatePtyRequest, Pty>(url, request, cancellationToken)
            ?? throw new OpenCodeException("Failed to create PTY");
    }

    public async Task<Pty> GetPtyAsync(string ptyId, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Pty(ptyId), directory);
        return await GetAsync<Pty>(url, cancellationToken)
            ?? throw new OpenCodeNotFoundException($"PTY {ptyId} not found");
    }

    public async Task<Pty> UpdatePtyAsync(string ptyId, UpdatePtyRequest request, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Pty(ptyId), directory);
        return await PutAsync<UpdatePtyRequest, Pty>(url, request, cancellationToken)
            ?? throw new OpenCodeException($"Failed to update PTY {ptyId}");
    }

    public async Task DeletePtyAsync(string ptyId, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Pty(ptyId), directory);
        await DeleteAsync(url, cancellationToken);
    }

    public async IAsyncEnumerable<string> ConnectToPtyAsync(string ptyId, string? directory = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // TODO: This endpoint uses WebSocket for bidirectional communication
        // For now, we'll throw NotImplementedException - full implementation would require WebSocket support
        await Task.CompletedTask.ConfigureAwait(false);
        throw new NotImplementedException("PTY WebSocket connection not yet implemented. Use the /pty/{ptyId}/connect endpoint directly with a WebSocket client.");
#pragma warning disable CS0162 // Unreachable code detected
        yield break;
#pragma warning restore CS0162
    }

    #endregion

    #region MCP (Model Context Protocol)

    public async Task<List<McpStatus>> GetMcpStatusAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Mcp, directory);
        return await GetAsync<List<McpStatus>>(url, cancellationToken) ?? new List<McpStatus>();
    }

    public async Task AddMcpServerAsync(AddMcpServerRequest request, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Mcp, directory);
        await PostAsync<AddMcpServerRequest, object>(url, request, cancellationToken);
    }

    public async Task StartMcpAuthAsync(string name, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.McpAuth(name), directory);
        await PostAsync<object?, object>(url, null, cancellationToken);
    }

    public async Task RemoveMcpAuthAsync(string name, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.McpAuth(name), directory);
        await DeleteAsync(url, cancellationToken);
    }

    public async Task AuthenticateMcpAsync(string name, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.McpAuthAuthenticate(name), directory);
        await PostAsync<object?, object>(url, null, cancellationToken);
    }

    public async Task HandleMcpAuthCallbackAsync(string name, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.McpAuthCallback(name), directory);
        await PostAsync<object?, object>(url, null, cancellationToken);
    }

    public async Task ConnectMcpAsync(string name, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.McpConnect(name), directory);
        await PostAsync<object?, object>(url, null, cancellationToken);
    }

    public async Task DisconnectMcpAsync(string name, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.McpDisconnect(name), directory);
        await PostAsync<object?, object>(url, null, cancellationToken);
    }

    #endregion

    #region Configuration

    public async Task<Dictionary<string, object>> GetConfigAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Config, directory);
        return await GetAsync<Dictionary<string, object>>(url, cancellationToken) ?? new Dictionary<string, object>();
    }

    public async Task UpdateConfigAsync(Dictionary<string, object> config, string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Config, directory);
        await PatchAsync<Dictionary<string, object>, object>(url, config, cancellationToken);
    }

    public async Task<Dictionary<string, object>> GetConfigProvidersAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.ConfigProviders, directory);
        return await GetAsync<Dictionary<string, object>>(url, cancellationToken) ?? new Dictionary<string, object>();
    }

    #endregion

    #region Providers

    public async Task<List<Provider>> ListProvidersAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Providers, directory);
        var response = await GetAsync<ProviderListResponse>(url, cancellationToken);
        return response?.All ?? new List<Provider>();
    }

    public async Task<List<ProviderAuth>> GetProviderAuthAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.ProviderAuth, directory);
        return await GetAsync<List<ProviderAuth>>(url, cancellationToken) ?? new List<ProviderAuth>();
    }

    #endregion

    #region Other Operations

    public async Task<List<Agent>> ListAgentsAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Agents, directory);
        return await GetAsync<List<Agent>>(url, cancellationToken) ?? new List<Agent>();
    }

    public async Task<List<Command>> ListCommandsAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Commands, directory);
        return await GetAsync<List<Command>>(url, cancellationToken) ?? new List<Command>();
    }

    public async Task<LspStatus> GetLspStatusAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Lsp, directory);
        return await GetAsync<LspStatus>(url, cancellationToken)
            ?? new LspStatus();
    }

    public async Task<FormatterStatus> GetFormatterStatusAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Formatter, directory);
        return await GetAsync<FormatterStatus>(url, cancellationToken)
            ?? new FormatterStatus();
    }

    public async Task<PathInfo> GetPathAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Path, directory);
        return await GetAsync<PathInfo>(url, cancellationToken)
            ?? throw new OpenCodeException("Failed to get path information");
    }

    public async Task<VcsInfo> GetVcsInfoAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Vcs, directory);
        return await GetAsync<VcsInfo>(url, cancellationToken)
            ?? throw new OpenCodeException("Failed to get VCS information");
    }

    public async IAsyncEnumerable<Event> SubscribeToEventsAsync(string? directory = null, [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Events, directory);
        await foreach (var ev in StreamServerSentEventsAsync<Event>(url, directory, cancellationToken))
        {
            yield return ev;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Event> SubscribeToEventsAsync(
        IProgress<StreamingProgress> progress,
        string? directory = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.Events, directory);
        await foreach (var ev in StreamServerSentEventsWithProgressAsync<Event>(url, directory, progress, cancellationToken))
        {
            yield return ev;
        }
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<Event> SubscribeToEventsAsync(
        IProgress<StreamingProgress> progress,
        TimeSpan timeout,
        string? directory = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        using var timeoutCts = new CancellationTokenSource(timeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        var url = BuildUrl(ApiEndpoints.Events, directory);
        await foreach (var ev in StreamServerSentEventsWithProgressAsync<Event>(url, directory, progress, linkedCts.Token))
        {
            yield return ev;
        }
    }

    public async Task DisposeInstanceAsync(string? directory = null, CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(ApiEndpoints.InstanceDispose, directory);
        await PostAsync<object?, object>(url, null, cancellationToken);
    }

    #endregion

    #region HTTP Helper Methods

    private async Task<T?> GetAsync<T>(string url, CancellationToken cancellationToken, string? operationName = null)
    {
        var operation = operationName ?? ExtractOperationName(url);
        var stopwatch = _options.EnableTelemetry ? Stopwatch.StartNew() : null;
        using var activity = _options.EnableTelemetry
            ? OpenCodeActivitySource.StartActivity(operation)
            : null;

        try
        {
            if (_options.EnableTelemetry)
            {
                OpenCodeMetrics.Instance.RecordRequestStarted(operation, "GET");
            }

            _logger.LogDebug("GET {Url}", url);

            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            PropagateTraceContext(request, activity);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            activity?.SetTag(OpenCodeActivitySource.Tags.StatusCode, (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponseAsync(response, cancellationToken);
            }

            if (_options.EnableTelemetry && stopwatch != null)
            {
                OpenCodeMetrics.Instance.RecordRequestCompleted(
                    operation, "GET", (int)response.StatusCode, stopwatch.Elapsed.TotalMilliseconds, true);
            }

            return await response.Content.ReadFromJsonAsync<T>(JsonOptions.SourceGenerated, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            RecordError(activity, stopwatch, operation, "GET", ex);
            throw new OpenCodeConnectionException($"Failed to connect to OpenCode server at {_httpClient.BaseAddress}", _httpClient.BaseAddress?.ToString(), ex);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            RecordError(activity, stopwatch, operation, "GET", ex);
            throw OpenCodeTimeoutException.OperationTimedOut(operation, _httpClient.Timeout, ex);
        }
        catch (Exception ex) when (ex is not OpenCodeException)
        {
            RecordError(activity, stopwatch, operation, "GET", ex);
            throw;
        }
    }

    private async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest? request, CancellationToken cancellationToken, string? operationName = null)
    {
        var operation = operationName ?? ExtractOperationName(url);
        var stopwatch = _options.EnableTelemetry ? Stopwatch.StartNew() : null;
        using var activity = _options.EnableTelemetry
            ? OpenCodeActivitySource.StartActivity(operation)
            : null;

        try
        {
            if (_options.EnableTelemetry)
            {
                OpenCodeMetrics.Instance.RecordRequestStarted(operation, "POST");
            }

            _logger.LogDebug("POST {Url}", url);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Post, url);
            httpRequest.Content = JsonContent.Create(request, options: JsonOptions.SourceGenerated);
            PropagateTraceContext(httpRequest, activity);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            activity?.SetTag(OpenCodeActivitySource.Tags.StatusCode, (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponseAsync(response, cancellationToken);
            }

            if (typeof(TResponse) == typeof(object))
            {
                if (_options.EnableTelemetry && stopwatch != null)
                {
                    OpenCodeMetrics.Instance.RecordRequestCompleted(
                        operation, "POST", (int)response.StatusCode, stopwatch.Elapsed.TotalMilliseconds, true);
                }
                return default;
            }

            // Check for empty response or non-JSON content type
            var contentType = response.Content.Headers.ContentType?.MediaType ?? "";
            if (!contentType.Contains("json", StringComparison.OrdinalIgnoreCase))
            {
                var content = await response.Content.ReadAsStringAsync(cancellationToken);
                if (content.Contains("<!doctype html>", StringComparison.OrdinalIgnoreCase) ||
                    content.Contains("<html", StringComparison.OrdinalIgnoreCase))
                {
                    throw new OpenCodeApiException(
                        $"Server returned HTML instead of JSON. This may indicate an invalid endpoint or server error. URL: {url}",
                        response.StatusCode);
                }
                throw new OpenCodeApiException(
                    $"Server returned unexpected content type '{contentType}'. Expected JSON. URL: {url}",
                    response.StatusCode);
            }

            // Check for empty content
            var contentLength = response.Content.Headers.ContentLength;
            if (contentLength == 0)
            {
                // Empty response often means the specified model is not available or requires authentication
                var modelHint = url.Contains("/message")
                    ? " This may indicate the specified model is not available or requires provider authentication. " +
                      "Try: (1) Use the default model without -p/-m flags, or (2) Run 'opencode auth login <provider>'."
                    : "";
                throw new OpenCodeApiException(
                    $"Server returned empty response.{modelHint} URL: {url}",
                    response.StatusCode);
            }

            if (_options.EnableTelemetry && stopwatch != null)
            {
                OpenCodeMetrics.Instance.RecordRequestCompleted(
                    operation, "POST", (int)response.StatusCode, stopwatch.Elapsed.TotalMilliseconds, true);
            }

            return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions.SourceGenerated, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            RecordError(activity, stopwatch, operation, "POST", ex);
            throw new OpenCodeConnectionException($"Failed to connect to OpenCode server at {_httpClient.BaseAddress}", _httpClient.BaseAddress?.ToString(), ex);
        }
        catch (System.Text.Json.JsonException ex)
        {
            RecordError(activity, stopwatch, operation, "POST", ex);
            throw new OpenCodeApiException($"Failed to parse JSON response from server. URL: {url}. Error: {ex.Message}", System.Net.HttpStatusCode.OK, ex);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            RecordError(activity, stopwatch, operation, "POST", ex);
            throw OpenCodeTimeoutException.OperationTimedOut(operation, _httpClient.Timeout, ex);
        }
        catch (Exception ex) when (ex is not OpenCodeException)
        {
            RecordError(activity, stopwatch, operation, "POST", ex);
            throw;
        }
    }

    private async Task<TResponse?> PutAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken, string? operationName = null)
    {
        var operation = operationName ?? ExtractOperationName(url);
        var stopwatch = _options.EnableTelemetry ? Stopwatch.StartNew() : null;
        using var activity = _options.EnableTelemetry
            ? OpenCodeActivitySource.StartActivity(operation)
            : null;

        try
        {
            if (_options.EnableTelemetry)
            {
                OpenCodeMetrics.Instance.RecordRequestStarted(operation, "PUT");
            }

            _logger.LogDebug("PUT {Url}", url);

            using var httpRequest = new HttpRequestMessage(HttpMethod.Put, url);
            httpRequest.Content = JsonContent.Create(request, options: JsonOptions.SourceGenerated);
            PropagateTraceContext(httpRequest, activity);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            activity?.SetTag(OpenCodeActivitySource.Tags.StatusCode, (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponseAsync(response, cancellationToken);
            }

            if (_options.EnableTelemetry && stopwatch != null)
            {
                OpenCodeMetrics.Instance.RecordRequestCompleted(
                    operation, "PUT", (int)response.StatusCode, stopwatch.Elapsed.TotalMilliseconds, true);
            }

            return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions.SourceGenerated, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            RecordError(activity, stopwatch, operation, "PUT", ex);
            throw new OpenCodeConnectionException($"Failed to connect to OpenCode server at {_httpClient.BaseAddress}", _httpClient.BaseAddress?.ToString(), ex);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            RecordError(activity, stopwatch, operation, "PUT", ex);
            throw OpenCodeTimeoutException.OperationTimedOut(operation, _httpClient.Timeout, ex);
        }
        catch (Exception ex) when (ex is not OpenCodeException)
        {
            RecordError(activity, stopwatch, operation, "PUT", ex);
            throw;
        }
    }

    private async Task<TResponse?> PatchAsync<TRequest, TResponse>(string url, TRequest request, CancellationToken cancellationToken, string? operationName = null)
    {
        var operation = operationName ?? ExtractOperationName(url);
        var stopwatch = _options.EnableTelemetry ? Stopwatch.StartNew() : null;
        using var activity = _options.EnableTelemetry
            ? OpenCodeActivitySource.StartActivity(operation)
            : null;

        try
        {
            if (_options.EnableTelemetry)
            {
                OpenCodeMetrics.Instance.RecordRequestStarted(operation, "PATCH");
            }

            _logger.LogDebug("PATCH {Url}", url);
            var content = JsonContent.Create(request, options: JsonOptions.SourceGenerated);
            using var httpRequest = new HttpRequestMessage(HttpMethod.Patch, url) { Content = content };
            PropagateTraceContext(httpRequest, activity);

            var response = await _httpClient.SendAsync(httpRequest, cancellationToken);

            activity?.SetTag(OpenCodeActivitySource.Tags.StatusCode, (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponseAsync(response, cancellationToken);
            }

            if (typeof(TResponse) == typeof(object))
            {
                if (_options.EnableTelemetry && stopwatch != null)
                {
                    OpenCodeMetrics.Instance.RecordRequestCompleted(
                        operation, "PATCH", (int)response.StatusCode, stopwatch.Elapsed.TotalMilliseconds, true);
                }
                return default;
            }

            if (_options.EnableTelemetry && stopwatch != null)
            {
                OpenCodeMetrics.Instance.RecordRequestCompleted(
                    operation, "PATCH", (int)response.StatusCode, stopwatch.Elapsed.TotalMilliseconds, true);
            }

            return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions.SourceGenerated, cancellationToken);
        }
        catch (HttpRequestException ex)
        {
            RecordError(activity, stopwatch, operation, "PATCH", ex);
            throw new OpenCodeConnectionException($"Failed to connect to OpenCode server at {_httpClient.BaseAddress}", _httpClient.BaseAddress?.ToString(), ex);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            RecordError(activity, stopwatch, operation, "PATCH", ex);
            throw OpenCodeTimeoutException.OperationTimedOut(operation, _httpClient.Timeout, ex);
        }
        catch (Exception ex) when (ex is not OpenCodeException)
        {
            RecordError(activity, stopwatch, operation, "PATCH", ex);
            throw;
        }
    }

    private async Task DeleteAsync(string url, CancellationToken cancellationToken, string? operationName = null)
    {
        var operation = operationName ?? ExtractOperationName(url);
        var stopwatch = _options.EnableTelemetry ? Stopwatch.StartNew() : null;
        using var activity = _options.EnableTelemetry
            ? OpenCodeActivitySource.StartActivity(operation)
            : null;

        try
        {
            if (_options.EnableTelemetry)
            {
                OpenCodeMetrics.Instance.RecordRequestStarted(operation, "DELETE");
            }

            _logger.LogDebug("DELETE {Url}", url);

            using var request = new HttpRequestMessage(HttpMethod.Delete, url);
            PropagateTraceContext(request, activity);

            var response = await _httpClient.SendAsync(request, cancellationToken);

            activity?.SetTag(OpenCodeActivitySource.Tags.StatusCode, (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponseAsync(response, cancellationToken);
            }

            if (_options.EnableTelemetry && stopwatch != null)
            {
                OpenCodeMetrics.Instance.RecordRequestCompleted(
                    operation, "DELETE", (int)response.StatusCode, stopwatch.Elapsed.TotalMilliseconds, true);
            }
        }
        catch (HttpRequestException ex)
        {
            RecordError(activity, stopwatch, operation, "DELETE", ex);
            throw new OpenCodeConnectionException($"Failed to connect to OpenCode server at {_httpClient.BaseAddress}", _httpClient.BaseAddress?.ToString(), ex);
        }
        catch (TaskCanceledException ex) when (ex.InnerException is TimeoutException)
        {
            RecordError(activity, stopwatch, operation, "DELETE", ex);
            throw OpenCodeTimeoutException.OperationTimedOut(operation, _httpClient.Timeout, ex);
        }
        catch (Exception ex) when (ex is not OpenCodeException)
        {
            RecordError(activity, stopwatch, operation, "DELETE", ex);
            throw;
        }
    }

    private async IAsyncEnumerable<T> StreamServerSentEventsAsync<T>(string url, string? directory, [EnumeratorCancellation] CancellationToken cancellationToken, string? operationName = null)
    {
        var operation = operationName ?? "opencode.sse.stream";
        var stopwatch = _options.EnableTelemetry ? Stopwatch.StartNew() : null;
        using var activity = _options.EnableTelemetry
            ? OpenCodeActivitySource.StartActivity(operation)
            : null;

        if (_options.EnableTelemetry)
        {
            OpenCodeMetrics.Instance.RecordRequestStarted(operation, "GET");
        }

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.ParseAdd("text/event-stream");
        PropagateTraceContext(request, activity);

        HttpResponseMessage? response = null;
        long chunkCount = 0;
        try
        {
            _logger.LogDebug("GET (SSE) {Url}", url);
            response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            activity?.SetTag(OpenCodeActivitySource.Tags.StatusCode, (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponseAsync(response, cancellationToken);
            }

            using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            using var reader = new StreamReader(stream);

            string? line;
            var eventData = new StringBuilder();

            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                if (cancellationToken.IsCancellationRequested)
                    yield break;

                if (string.IsNullOrWhiteSpace(line))
                {
                    // Empty line indicates end of event
                    if (eventData.Length > 0)
                    {
                        var json = eventData.ToString();
                        _logger.LogDebug("SSE JSON: {Json}", json);

                        if (_options.EnableTelemetry)
                        {
                            chunkCount++;
                            OpenCodeMetrics.Instance.RecordStreamingChunk(operation, directory, json.Length);
                        }

                        var ev = JsonSerializer.Deserialize<T>(json, JsonOptions.SourceGenerated);
                        if (ev != null)
                            yield return ev;
                        eventData.Clear();
                    }
                    continue;
                }

                if (line.StartsWith("data: "))
                {
                    eventData.Append(line.Substring(6));
                }
            }

            if (_options.EnableTelemetry && stopwatch != null)
            {
                activity?.SetTag("opencode.streaming.chunks", chunkCount);
                OpenCodeMetrics.Instance.RecordRequestCompleted(
                    operation, "GET", (int)response.StatusCode, stopwatch.Elapsed.TotalMilliseconds, true);
            }
        }
        finally
        {
            response?.Dispose();
        }
    }

    /// <summary>
    /// Streams server-sent events with progress callback support.
    /// </summary>
    /// <typeparam name="T">The type of events to deserialize.</typeparam>
    /// <param name="url">The URL to stream from.</param>
    /// <param name="directory">Optional directory context.</param>
    /// <param name="progress">Progress callback for streaming updates.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <param name="operationName">Optional operation name for telemetry.</param>
    /// <returns>An async enumerable of events.</returns>
    private async IAsyncEnumerable<T> StreamServerSentEventsWithProgressAsync<T>(
        string url,
        string? directory,
        IProgress<StreamingProgress> progress,
        [EnumeratorCancellation] CancellationToken cancellationToken,
        string? operationName = null)
    {
        var operation = operationName ?? "opencode.sse.stream";
        var stopwatch = Stopwatch.StartNew();
        using var activity = _options.EnableTelemetry
            ? OpenCodeActivitySource.StartActivity(operation)
            : null;

        if (_options.EnableTelemetry)
        {
            OpenCodeMetrics.Instance.RecordRequestStarted(operation, "GET");
        }

        // Report starting
        progress.Report(StreamingProgress.Starting());

        using var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Accept.ParseAdd("text/event-stream");
        PropagateTraceContext(request, activity);

        HttpResponseMessage? response = null;
        int chunkCount = 0;
        long bytesReceived = 0;
        int characterCount = 0;

        // First, establish the connection and handle errors before yielding
        try
        {
            _logger.LogDebug("GET (SSE with progress) {Url}", url);
            response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);

            activity?.SetTag(OpenCodeActivitySource.Tags.StatusCode, (int)response.StatusCode);

            if (!response.IsSuccessStatusCode)
            {
                await HandleErrorResponseAsync(response, cancellationToken);
            }

            // Report connected
            progress.Report(StreamingProgress.Connected());
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            progress.Report(StreamingProgress.Cancelled(chunkCount, stopwatch.Elapsed));
            throw;
        }
        catch (Exception ex)
        {
            progress.Report(StreamingProgress.Failed(ex, chunkCount, stopwatch.Elapsed));
            throw;
        }

        // Now stream the data - we cannot yield inside try-catch, so we handle errors differently
        Stream? stream = null;
        StreamReader? reader = null;
        try
        {
            stream = await response!.Content.ReadAsStreamAsync(cancellationToken);
            reader = new StreamReader(stream);

            string? line;
            var eventData = new StringBuilder();
            string? eventType = null;

            while ((line = await reader.ReadLineAsync(cancellationToken)) != null)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    progress.Report(StreamingProgress.Cancelled(chunkCount, stopwatch.Elapsed));
                    yield break;
                }

                bytesReceived += System.Text.Encoding.UTF8.GetByteCount(line) + 1; // +1 for newline

                if (string.IsNullOrWhiteSpace(line))
                {
                    // Empty line indicates end of event
                    if (eventData.Length > 0)
                    {
                        var json = eventData.ToString();
                        _logger.LogDebug("SSE JSON: {Json}", json);

                        chunkCount++;
                        characterCount += json.Length;

                        if (_options.EnableTelemetry)
                        {
                            OpenCodeMetrics.Instance.RecordStreamingChunk(operation, directory, json.Length);
                        }

                        // Report progress
                        progress.Report(StreamingProgress.ChunkReceived(
                            chunkCount,
                            bytesReceived,
                            characterCount,
                            stopwatch.Elapsed,
                            json.Length > 200 ? json[..200] + "..." : json,
                            eventType));

                        var ev = JsonSerializer.Deserialize<T>(json, JsonOptions.SourceGenerated);
                        if (ev != null)
                            yield return ev;
                        eventData.Clear();
                        eventType = null;
                    }
                    continue;
                }

                if (line.StartsWith("event: "))
                {
                    eventType = line.Substring(7);
                }
                else if (line.StartsWith("data: "))
                {
                    eventData.Append(line.Substring(6));
                }
            }

            // Report completion
            progress.Report(StreamingProgress.Completed(chunkCount, bytesReceived, characterCount, stopwatch.Elapsed));

            if (_options.EnableTelemetry && response != null)
            {
                activity?.SetTag("opencode.streaming.chunks", chunkCount);
                OpenCodeMetrics.Instance.RecordRequestCompleted(
                    operation, "GET", (int)response.StatusCode, stopwatch.Elapsed.TotalMilliseconds, true);
            }
        }
        finally
        {
            reader?.Dispose();
            stream?.Dispose();
            response?.Dispose();
        }
    }

    private async Task HandleErrorResponseAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var statusCode = (int)response.StatusCode;
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (statusCode == 404)
        {
            throw new OpenCodeNotFoundException($"Resource not found: {response.RequestMessage?.RequestUri}");
        }
        else if (statusCode == 400)
        {
            throw new OpenCodeException($"Bad request: {content}");
        }
        else if (statusCode >= 500)
        {
            throw new OpenCodeException($"Server error ({statusCode}): {content}");
        }
        else
        {
            throw new OpenCodeException($"Request failed with status {statusCode}: {content}");
        }
    }

    /// <summary>
    /// Propagates W3C trace context headers to outgoing HTTP requests.
    /// </summary>
    /// <param name="request">The HTTP request message.</param>
    /// <param name="activity">The current activity (may be null).</param>
    private static void PropagateTraceContext(HttpRequestMessage request, Activity? activity)
    {
        if (activity == null) return;

        // W3C Trace Context headers
        // traceparent: version-trace_id-parent_id-trace_flags
        var traceParent = $"00-{activity.TraceId}-{activity.SpanId}-{(activity.Recorded ? "01" : "00")}";
        request.Headers.TryAddWithoutValidation("traceparent", traceParent);

        // tracestate: vendor-specific key-value pairs
        if (!string.IsNullOrEmpty(activity.TraceStateString))
        {
            request.Headers.TryAddWithoutValidation("tracestate", activity.TraceStateString);
        }
    }

    /// <summary>
    /// Records an error to both the activity and metrics.
    /// </summary>
    private void RecordError(Activity? activity, Stopwatch? stopwatch, string operation, string httpMethod, Exception ex)
    {
        activity?.SetStatus(ActivityStatusCode.Error, ex.Message);
        activity?.SetTag(OpenCodeActivitySource.Tags.ErrorType, ex.GetType().Name);

        if (_options.EnableTelemetry)
        {
            OpenCodeMetrics.Instance.RecordError(operation, ex.GetType().Name, httpMethod);
            if (stopwatch != null)
            {
                // Record as failed request with status code 0 for connection errors
                OpenCodeMetrics.Instance.RecordRequestCompleted(
                    operation, httpMethod, 0, stopwatch.Elapsed.TotalMilliseconds, false);
            }
        }
    }

    /// <summary>
    /// Extracts an operation name from the URL path for telemetry purposes.
    /// </summary>
    private static string ExtractOperationName(string url)
    {
        // Extract meaningful operation name from URL
        // e.g., "/session" -> "opencode.session.list"
        // e.g., "/session/{id}" -> "opencode.session.get"
        // e.g., "/session/{id}/message" -> "opencode.message.send"

        var path = url.Split('?')[0].TrimStart('/').ToLowerInvariant();
        var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);

        if (segments.Length == 0)
            return "opencode.unknown";

        // Map common paths to semantic operation names
        var operationBuilder = new StringBuilder("opencode.");

        for (int i = 0; i < segments.Length && i < 3; i++)
        {
            var segment = segments[i];

            // Skip UUIDs and numeric IDs
            if (Guid.TryParse(segment, out _) || int.TryParse(segment, out _))
                continue;

            // Skip very long segments (likely IDs)
            if (segment.Length > 20)
                continue;

            if (operationBuilder.Length > 9) // "opencode."
                operationBuilder.Append('.');

            operationBuilder.Append(segment);
        }

        return operationBuilder.Length > 9 ? operationBuilder.ToString() : "opencode.unknown";
    }

    #endregion

    public async ValueTask DisposeAsync()
    {
        if (_ownsHttpClient)
        {
            _httpClient?.Dispose();
        }

        await Task.CompletedTask;
    }
}
