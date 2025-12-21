using System.Runtime.CompilerServices;
using Microsoft.Extensions.AI;
using LionFire.OpenCode.Serve;
using LionFire.OpenCode.Serve.Models;

namespace LionFire.OpenCode.Serve.AgentFramework;

/// <summary>
/// An implementation of <see cref="IChatClient"/> that wraps an OpenCode session.
/// Enables using OpenCode as a chat backend through the Microsoft.Extensions.AI abstractions.
/// </summary>
public class OpenCodeChatClient : IChatClient
{
    private readonly IOpenCodeClient _client;
    private readonly OpenCodeChatClientOptions _options;
    private string? _sessionId;

    /// <summary>
    /// Gets the metadata for this chat client.
    /// </summary>
    public ChatClientMetadata Metadata { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenCodeChatClient"/> class.
    /// </summary>
    /// <param name="client">The OpenCode client.</param>
    /// <param name="options">Optional configuration options.</param>
    public OpenCodeChatClient(IOpenCodeClient client, OpenCodeChatClientOptions? options = null)
    {
        _client = client ?? throw new ArgumentNullException(nameof(client));
        _options = options ?? new OpenCodeChatClientOptions();

        Metadata = new ChatClientMetadata(
            "OpenCode",
            new Uri(_options.BaseUrl ?? OpenCodeClientOptions.DefaultBaseUrl),
            _options.ModelId);
    }

    /// <summary>
    /// Gets or sets the session ID for this chat client.
    /// If not set, a new session will be created on the first message.
    /// </summary>
    public string? SessionId
    {
        get => _sessionId;
        set => _sessionId = value;
    }

    /// <inheritdoc />
    public async Task<ChatResponse> GetResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(chatMessages);

        // Ensure we have a session
        if (_sessionId is null)
        {
            var session = await _client.CreateSessionAsync(directory: _options.Directory, cancellationToken: cancellationToken);
            _sessionId = session.Id;
        }

        // Get the last user message
        var messagesList = chatMessages.ToList();
        var lastMessage = messagesList.LastOrDefault(m => m.Role == ChatRole.User);
        if (lastMessage is null)
        {
            throw new ArgumentException("No user message found in chat messages", nameof(chatMessages));
        }

        // Convert to OpenCode part inputs
        var partInputs = MessageConverter.ToPartInputs(lastMessage);

        // Send and get response
        var request = new SendMessageRequest
        {
            Parts = partInputs.ToList(),
            Model = _options.Model
        };
        var response = await _client.PromptAsync(_sessionId, request, _options.Directory, cancellationToken);

        // Convert back to ChatResponse
        return MessageConverter.ToChatResponse(response);
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ChatResponseUpdate> GetStreamingResponseAsync(
        IEnumerable<ChatMessage> chatMessages,
        ChatOptions? options = null,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        // True streaming is not currently supported - the OpenCode API uses SSE events
        // which would require subscribing to SubscribeToEventsAsync while calling PromptAsyncNonBlocking.
        // For now, fall back to non-streaming and emit the response content as a single update.
        var response = await GetResponseAsync(chatMessages, options, cancellationToken);

        // Yield a single update with the content from the response
        var update = new ChatResponseUpdate
        {
            Role = ChatRole.Assistant,
            FinishReason = response.FinishReason,
            MessageId = response.ResponseId
        };

        // Add the content from the response messages
        if (response.Messages.Count > 0)
        {
            foreach (var message in response.Messages)
            {
                foreach (var content in message.Contents)
                {
                    update.Contents.Add(content);
                }
            }
        }

        yield return update;
    }

    /// <inheritdoc />
    public object? GetService(Type serviceType, object? serviceKey = null)
    {
        if (serviceKey is not null)
        {
            return null;
        }

        if (serviceType == typeof(IOpenCodeClient))
        {
            return _client;
        }

        if (serviceType == typeof(OpenCodeChatClient))
        {
            return this;
        }

        return null;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        // Session cleanup is handled by the user via SessionId management
        // The underlying IOpenCodeClient should be managed by DI or the caller
    }
}

/// <summary>
/// Options for configuring the <see cref="OpenCodeChatClient"/>.
/// </summary>
public class OpenCodeChatClientOptions
{
    /// <summary>
    /// Gets or sets the working directory for sessions.
    /// </summary>
    public string? Directory { get; set; }

    /// <summary>
    /// Gets or sets the model ID to report in metadata.
    /// </summary>
    public string? ModelId { get; set; } = "opencode";

    /// <summary>
    /// Gets or sets the base URL to report in metadata.
    /// </summary>
    public string? BaseUrl { get; set; }

    /// <summary>
    /// Gets or sets whether to create a new session for each conversation.
    /// Defaults to false (reuse session).
    /// </summary>
    public bool CreateSessionPerConversation { get; set; }

    /// <summary>
    /// Gets or sets the model to use for prompts.
    /// If set, this model will be used for all requests from this chat client.
    /// </summary>
    public ModelReference? Model { get; set; }
}
