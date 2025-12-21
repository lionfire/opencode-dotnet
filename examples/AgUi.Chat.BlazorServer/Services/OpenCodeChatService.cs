using System.Runtime.CompilerServices;
using System.Text;
using LionFire.OpenCode.Serve;
using LionFire.OpenCode.Serve.Models;
using LionFire.OpenCode.Serve.Models.Events;

namespace AgUi.Chat.BlazorServer.Services;

/// <summary>
/// Chat service that connects to the real OpenCode serve backend.
/// </summary>
public class OpenCodeChatService : IChatService, IAsyncDisposable
{
    private readonly IOpenCodeClient _client;
    private readonly ILogger<OpenCodeChatService> _logger;
    private readonly string? _directory;
    private string? _sessionId;

    public OpenCodeChatService(IOpenCodeClient client, ILogger<OpenCodeChatService> logger, IConfiguration configuration)
    {
        _client = client;
        _logger = logger;
        _directory = configuration["OpenCode:Directory"];
    }

    public string BackendName => "OpenCode";

    public async IAsyncEnumerable<string> SendMessageAsync(
        string message,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("OpenCode: Sending message: {Message}", message);

        // Ensure we have a session
        if (_sessionId == null)
        {
            var session = await _client.CreateSessionAsync(directory: _directory, cancellationToken: cancellationToken);
            _sessionId = session.Id;
            _logger.LogInformation("OpenCode: Created session {SessionId}", _sessionId);
        }

        // Create the message request
        var request = new SendMessageRequest
        {
            Parts = new List<PartInput>
            {
                PartInput.TextInput(message)
            }
        };

        // Track the response text as it streams
        var responseBuilder = new StringBuilder();
        var lastLength = 0;

        // Subscribe to events for this session to get streaming updates
        var eventTask = Task.Run(async () =>
        {
            try
            {
                await foreach (var ev in _client.SubscribeToEventsAsync(_directory, cancellationToken))
                {
                    if (ev is MessagePartUpdatedEvent partEvent)
                    {
                        var part = partEvent.Properties?.Part;
                        // SessionId is inside the part, not at properties level
                        // Filter: only process assistant responses (have Time.Start), not user message echo
                        if (part?.SessionId == _sessionId && part?.Type == "text" && part?.Text != null && part?.Time?.Start != null)
                        {
                            // Update our response text
                            responseBuilder.Clear();
                            responseBuilder.Append(part.Text);
                        }
                    }
                    else if (ev is SessionIdleEvent idleEvent && idleEvent.Properties?.SessionId == _sessionId)
                    {
                        // Session is idle, response is complete
                        break;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelled
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in event subscription");
            }
        }, cancellationToken);

        // Send the message (non-blocking so we can stream events)
        await _client.PromptAsyncNonBlocking(_sessionId, request, _directory, cancellationToken);

        // Stream chunks as they arrive
        while (!eventTask.IsCompleted && !cancellationToken.IsCancellationRequested)
        {
            await Task.Delay(50, cancellationToken);

            var currentLength = responseBuilder.Length;
            if (currentLength > lastLength)
            {
                var newText = responseBuilder.ToString()[lastLength..];
                lastLength = currentLength;
                yield return newText;
            }
        }

        // Yield any remaining text
        if (responseBuilder.Length > lastLength)
        {
            yield return responseBuilder.ToString()[lastLength..];
        }
    }

    public async Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Try to get the path info to check if OpenCode serve is running
            var pathInfo = await _client.GetPathAsync(_directory, cancellationToken);
            return pathInfo != null;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogWarning(ex, "OpenCode serve is not available");
            return false;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking OpenCode availability");
            return false;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync();
    }
}
