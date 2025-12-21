using Microsoft.Extensions.AI;
using LionFire.OpenCode.Serve.Models;

namespace LionFire.OpenCode.Serve.AgentFramework;

/// <summary>
/// Extension methods for integrating OpenCode with Microsoft.Extensions.AI.
/// </summary>
public static class OpenCodeClientExtensions
{
    /// <summary>
    /// Creates an <see cref="IChatClient"/> from the OpenCode client.
    /// </summary>
    /// <param name="client">The OpenCode client.</param>
    /// <param name="options">Optional configuration options.</param>
    /// <param name="model">Optional model to use for all requests.</param>
    /// <returns>A chat client implementation.</returns>
    /// <example>
    /// <code>
    /// // Using default model
    /// IChatClient chatClient = openCodeClient.AsChatClient();
    ///
    /// // Using a specific model
    /// IChatClient chatClient = openCodeClient.AsChatClient(model: new ModelReference
    /// {
    ///     ProviderId = "chutes",
    ///     ModelId = "openai/gpt-oss-20b"
    /// });
    ///
    /// var response = await chatClient.GetResponseAsync([new ChatMessage(ChatRole.User, "Hello!")]);
    /// </code>
    /// </example>
    public static IChatClient AsChatClient(
        this IOpenCodeClient client,
        OpenCodeChatClientOptions? options = null,
        ModelReference? model = null)
    {
        var effectiveOptions = options ?? new OpenCodeChatClientOptions();
        if (model != null)
        {
            effectiveOptions.Model = model;
            effectiveOptions.ModelId = $"{model.ProviderId}/{model.ModelId}";
        }
        return new OpenCodeChatClient(client, effectiveOptions);
    }

    /// <summary>
    /// Creates an <see cref="IChatClient"/> with a specific session.
    /// </summary>
    /// <param name="client">The OpenCode client.</param>
    /// <param name="sessionId">The session ID to use.</param>
    /// <param name="options">Optional configuration options.</param>
    /// <param name="model">Optional model to use for all requests.</param>
    /// <returns>A chat client implementation bound to the session.</returns>
    public static IChatClient AsChatClient(
        this IOpenCodeClient client,
        string sessionId,
        OpenCodeChatClientOptions? options = null,
        ModelReference? model = null)
    {
        var effectiveOptions = options ?? new OpenCodeChatClientOptions();
        if (model != null)
        {
            effectiveOptions.Model = model;
            effectiveOptions.ModelId = $"{model.ProviderId}/{model.ModelId}";
        }
        var chatClient = new OpenCodeChatClient(client, effectiveOptions)
        {
            SessionId = sessionId
        };
        return chatClient;
    }
}
