using Microsoft.Extensions.AI;
using LionFire.OpenCode.Serve.Models;

namespace LionFire.OpenCode.Serve.AgentFramework;

/// <summary>
/// Utility class for converting between Microsoft.Extensions.AI messages and OpenCode messages.
/// </summary>
public static class MessageConverter
{
    /// <summary>
    /// Converts an OpenCode <see cref="MessageWithParts"/> to a <see cref="ChatMessage"/>.
    /// </summary>
    /// <param name="messageWithParts">The OpenCode message with parts to convert.</param>
    /// <returns>A ChatMessage with equivalent content.</returns>
    public static ChatMessage ToChatMessage(MessageWithParts messageWithParts)
    {
        ArgumentNullException.ThrowIfNull(messageWithParts);

        var chatMessage = new ChatMessage
        {
            Role = ToChatRole(messageWithParts.Message?.Role ?? "assistant")
        };

        if (messageWithParts.Parts is not null)
        {
            foreach (var part in messageWithParts.Parts)
            {
                var content = ToAIContent(part);
                if (content is not null)
                {
                    chatMessage.Contents.Add(content);
                }
            }
        }

        return chatMessage;
    }

    /// <summary>
    /// Converts a <see cref="ChatMessage"/> to OpenCode part inputs.
    /// </summary>
    /// <param name="message">The ChatMessage to convert.</param>
    /// <returns>A list of part inputs representing the content.</returns>
    public static IReadOnlyList<PartInput> ToPartInputs(ChatMessage message)
    {
        ArgumentNullException.ThrowIfNull(message);

        var parts = new List<PartInput>();

        foreach (var content in message.Contents)
        {
            var part = ToPartInput(content);
            if (part is not null)
            {
                parts.Add(part);
            }
        }

        return parts;
    }

    /// <summary>
    /// Converts an OpenCode <see cref="MessageWithParts"/> to a <see cref="ChatResponse"/>.
    /// </summary>
    /// <param name="messageWithParts">The OpenCode message with parts to convert.</param>
    /// <returns>A ChatResponse containing the message.</returns>
    public static ChatResponse ToChatResponse(MessageWithParts messageWithParts)
    {
        ArgumentNullException.ThrowIfNull(messageWithParts);

        var chatMessage = ToChatMessage(messageWithParts);
        var message = messageWithParts.Message;

        return new ChatResponse(chatMessage)
        {
            ResponseId = message?.Id ?? string.Empty,
            CreatedAt = message?.Time is not null
                ? DateTimeOffset.FromUnixTimeMilliseconds(message.Time.Created)
                : DateTimeOffset.UtcNow
        };
    }

    /// <summary>
    /// Converts an OpenCode message role string to a <see cref="ChatRole"/>.
    /// </summary>
    /// <param name="role">The OpenCode message role string.</param>
    /// <returns>The equivalent ChatRole.</returns>
    public static ChatRole ToChatRole(string role)
    {
        return role.ToLowerInvariant() switch
        {
            "user" => ChatRole.User,
            "assistant" => ChatRole.Assistant,
            "system" => ChatRole.System,
            "tool" => ChatRole.Tool,
            _ => ChatRole.Assistant
        };
    }

    /// <summary>
    /// Converts a <see cref="ChatRole"/> to an OpenCode message role string.
    /// </summary>
    /// <param name="role">The ChatRole to convert.</param>
    /// <returns>The equivalent role string.</returns>
    public static string ToMessageRole(ChatRole role)
    {
        if (role == ChatRole.User)
            return "user";
        if (role == ChatRole.Assistant)
            return "assistant";
        if (role == ChatRole.System)
            return "system";
        if (role == ChatRole.Tool)
            return "tool";

        return "user";
    }

    /// <summary>
    /// Converts an OpenCode <see cref="Part"/> to an <see cref="AIContent"/>.
    /// </summary>
    /// <param name="part">The message part to convert.</param>
    /// <returns>The equivalent AIContent, or null if the part type is not supported.</returns>
    public static AIContent? ToAIContent(Part part)
    {
        if (part.IsTextPart)
        {
            return new TextContent(part.Text ?? string.Empty);
        }

        if (part.IsToolCompleted && part.CallId is not null)
        {
            return new FunctionResultContent(part.CallId, part.OutputString);
        }

        return null;
    }

    /// <summary>
    /// Converts an <see cref="AIContent"/> to an OpenCode <see cref="PartInput"/>.
    /// </summary>
    /// <param name="content">The AI content to convert.</param>
    /// <returns>The equivalent PartInput, or null if the content type is not supported.</returns>
    public static PartInput? ToPartInput(AIContent content)
    {
        return content switch
        {
            TextContent textContent => PartInput.TextInput(textContent.Text ?? string.Empty),
            FunctionCallContent => null, // Tool calls are handled differently in OpenCode
            FunctionResultContent => null, // Tool results are handled differently in OpenCode
            _ => PartInput.TextInput(content.ToString() ?? string.Empty)
        };
    }

    /// <summary>
    /// Converts a collection of OpenCode messages to ChatMessages.
    /// </summary>
    /// <param name="messages">The messages to convert.</param>
    /// <returns>A list of converted ChatMessages.</returns>
    public static IReadOnlyList<ChatMessage> ToChatMessages(IEnumerable<MessageWithParts> messages)
    {
        ArgumentNullException.ThrowIfNull(messages);
        return messages.Select(ToChatMessage).ToList();
    }
}
