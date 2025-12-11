using LionFire.OpenCode.Serve.Models;

namespace LionFire.OpenCode.Serve;

/// <summary>
/// Interface for the OpenCode client, providing access to all OpenCode server operations.
/// </summary>
/// <remarks>
/// <para>
/// This interface defines the contract for communicating with an OpenCode server
/// running locally via <c>opencode serve</c>. It provides session management,
/// message handling, tool operations, file access, and configuration retrieval.
/// </para>
/// <para>
/// Implementations should be thread-safe and support concurrent operations.
/// </para>
/// </remarks>
/// <example>
/// Basic usage:
/// <code>
/// await using var client = new OpenCodeClient();
/// var session = await client.CreateSessionAsync();
/// var response = await client.SendMessageAsync(session.Id, "Hello, OpenCode!");
/// </code>
/// </example>
public interface IOpenCodeClient : IAsyncDisposable
{
    #region Health Check

    /// <summary>
    /// Checks if the OpenCode server is reachable and healthy.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A <see cref="HealthCheckResult"/> if the server is healthy.</returns>
    /// <exception cref="Exceptions.OpenCodeConnectionException">Thrown when the server cannot be reached.</exception>
    Task<HealthCheckResult> PingAsync(CancellationToken cancellationToken = default);

    #endregion

    #region Session Management

    /// <summary>
    /// Creates a new session.
    /// </summary>
    /// <param name="directory">Optional working directory for the session.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created <see cref="Session"/>.</returns>
    Task<Session> CreateSessionAsync(string? directory = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a new session with additional options.
    /// </summary>
    /// <param name="directory">Optional working directory for the session.</param>
    /// <param name="parentId">Optional parent session ID to fork from.</param>
    /// <param name="title">Optional title for the session.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The created <see cref="Session"/>.</returns>
    Task<Session> CreateSessionAsync(string? directory, string? parentId, string? title, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a session by its ID.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The <see cref="Session"/> if found.</returns>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session is not found.</exception>
    Task<Session> GetSessionAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Lists all sessions.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of all <see cref="Session"/> instances.</returns>
    Task<IReadOnlyList<Session>> ListSessionsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a session.
    /// </summary>
    /// <param name="sessionId">The session ID to delete.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session is not found.</exception>
    Task DeleteSessionAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Forks a session at a specific message, creating a new session with messages up to that point.
    /// </summary>
    /// <param name="sessionId">The session ID to fork.</param>
    /// <param name="messageId">The message ID to fork at.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The newly forked <see cref="Session"/>.</returns>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session or message is not found.</exception>
    Task<Session> ForkSessionAsync(string sessionId, string messageId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aborts a running session.
    /// </summary>
    /// <param name="sessionId">The session ID to abort.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session is not found.</exception>
    Task AbortSessionAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Shares a session, generating a shareable token.
    /// </summary>
    /// <param name="sessionId">The session ID to share.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The <see cref="Session"/> with the share token populated.</returns>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session is not found.</exception>
    Task<Session> ShareSessionAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Unshares a session, revoking the share token.
    /// </summary>
    /// <param name="sessionId">The session ID to unshare.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session is not found.</exception>
    Task UnshareSessionAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Creates a session scope that automatically deletes the session when disposed.
    /// </summary>
    /// <param name="directory">Optional working directory for the session.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An <see cref="ISessionScope"/> that manages the session lifecycle.</returns>
    /// <example>
    /// <code>
    /// await using var scope = await client.CreateSessionScopeAsync();
    /// // Use scope.Session...
    /// // Session is automatically deleted when scope is disposed
    /// </code>
    /// </example>
    Task<ISessionScope> CreateSessionScopeAsync(string? directory = null, CancellationToken cancellationToken = default);

    #endregion

    #region Message Operations

    /// <summary>
    /// Sends a message to a session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="parts">The message parts to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The assistant's response <see cref="Message"/>.</returns>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session is not found.</exception>
    Task<Message> SendMessageAsync(string sessionId, IReadOnlyList<MessagePart> parts, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a text message to a session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="text">The text message to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The assistant's response <see cref="Message"/>.</returns>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session is not found.</exception>
    Task<Message> SendMessageAsync(string sessionId, string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a message to a session and streams the response.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="parts">The message parts to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An async enumerable of <see cref="MessageUpdate"/> for incremental updates.</returns>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session is not found.</exception>
    IAsyncEnumerable<MessageUpdate> SendMessageStreamingAsync(string sessionId, IReadOnlyList<MessagePart> parts, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a text message to a session and streams the response.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="text">The text message to send.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>An async enumerable of <see cref="MessageUpdate"/> for incremental updates.</returns>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session is not found.</exception>
    IAsyncEnumerable<MessageUpdate> SendMessageStreamingAsync(string sessionId, string text, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all messages in a session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of all <see cref="Message"/> instances in the session.</returns>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session is not found.</exception>
    Task<IReadOnlyList<Message>> GetMessagesAsync(string sessionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific message from a session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="messageId">The message ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The <see cref="Message"/> if found.</returns>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session or message is not found.</exception>
    Task<Message> GetMessageAsync(string sessionId, string messageId, CancellationToken cancellationToken = default);

    #endregion

    #region Tool Operations

    /// <summary>
    /// Gets all available tools.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of all available <see cref="Tool"/> instances.</returns>
    Task<IReadOnlyList<Tool>> GetToolsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific tool by ID.
    /// </summary>
    /// <param name="toolId">The tool ID.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The <see cref="Tool"/> if found.</returns>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the tool is not found.</exception>
    Task<Tool> GetToolAsync(string toolId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Approves a pending tool execution in a session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="toolId">The tool ID to approve.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session or pending tool is not found.</exception>
    Task ApproveToolAsync(string sessionId, string toolId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Denies a pending tool execution in a session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="toolId">The tool ID to deny.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session or pending tool is not found.</exception>
    Task DenyToolAsync(string sessionId, string toolId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates the permission level for a tool in a session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="toolId">The tool ID.</param>
    /// <param name="permission">The new permission level.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session or tool is not found.</exception>
    Task UpdateToolPermissionsAsync(string sessionId, string toolId, ToolPermission permission, CancellationToken cancellationToken = default);

    #endregion

    #region File Operations

    /// <summary>
    /// Lists files in a directory within a session's context.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="path">The path to list. Defaults to the session's working directory.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of <see cref="OpenCodeFileInfo"/> entries.</returns>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session is not found.</exception>
    Task<IReadOnlyList<OpenCodeFileInfo>> ListFilesAsync(string sessionId, string? path = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the content of a file within a session's context.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="path">The path to the file.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The <see cref="FileContent"/> of the file.</returns>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session or file is not found.</exception>
    Task<FileContent> GetFileContentAsync(string sessionId, string path, CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches for files within a session's context.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="query">The search query.</param>
    /// <param name="path">Optional path to search within.</param>
    /// <param name="maxResults">Maximum number of results to return.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The <see cref="SearchFilesResult"/> containing matching files.</returns>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session is not found.</exception>
    Task<SearchFilesResult> SearchFilesAsync(string sessionId, string query, string? path = null, int? maxResults = null, CancellationToken cancellationToken = default);

    /// <summary>
    /// Applies file changes within a session's context.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="changes">The changes to apply.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The <see cref="ApplyChangesResult"/> indicating which changes were applied.</returns>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session is not found.</exception>
    Task<ApplyChangesResult> ApplyChangesAsync(string sessionId, IReadOnlyList<FileChange> changes, CancellationToken cancellationToken = default);

    #endregion

    #region Command Operations

    /// <summary>
    /// Executes a slash command in a session.
    /// </summary>
    /// <param name="sessionId">The session ID.</param>
    /// <param name="command">The command to execute (e.g., "/help", "/clear").</param>
    /// <param name="arguments">Optional arguments for the command.</param>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The <see cref="CommandResult"/> from the command execution.</returns>
    /// <exception cref="Exceptions.OpenCodeNotFoundException">Thrown when the session is not found.</exception>
    Task<CommandResult> ExecuteCommandAsync(string sessionId, string command, string? arguments = null, CancellationToken cancellationToken = default);

    #endregion

    #region Configuration

    /// <summary>
    /// Gets the OpenCode server configuration.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>The <see cref="OpenCodeConfig"/> of the server.</returns>
    Task<OpenCodeConfig> GetConfigAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available AI providers.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of all <see cref="Provider"/> instances.</returns>
    Task<IReadOnlyList<Provider>> GetProvidersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all available AI models.
    /// </summary>
    /// <param name="cancellationToken">A token to cancel the operation.</param>
    /// <returns>A list of all <see cref="Model"/> instances.</returns>
    Task<IReadOnlyList<Model>> GetModelsAsync(CancellationToken cancellationToken = default);

    #endregion
}

/// <summary>
/// A session scope that automatically deletes the session when disposed.
/// </summary>
public interface ISessionScope : IAsyncDisposable
{
    /// <summary>
    /// Gets the session managed by this scope.
    /// </summary>
    Session Session { get; }

    /// <summary>
    /// Gets the session ID for convenience.
    /// </summary>
    string SessionId => Session.Id;
}
