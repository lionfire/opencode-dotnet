namespace AgUi.Chat.BlazorServer.Services;

/// <summary>
/// Interface for chat backend services.
/// </summary>
public interface IChatService
{
    /// <summary>
    /// Gets the display name of this backend.
    /// </summary>
    string BackendName { get; }

    /// <summary>
    /// Sends a message and streams back the response.
    /// </summary>
    /// <param name="message">The user's message</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Async enumerable of response chunks</returns>
    IAsyncEnumerable<string> SendMessageAsync(string message, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if the backend is available/connected.
    /// </summary>
    Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default);
}
