using System.Runtime.CompilerServices;

namespace AgUi.Chat.BlazorServer.Services;

/// <summary>
/// Mock chat service that simulates AI responses for demo purposes.
/// </summary>
public class MockChatService : IChatService
{
    private readonly ILogger<MockChatService> _logger;

    public MockChatService(ILogger<MockChatService> logger)
    {
        _logger = logger;
    }

    public string BackendName => "Mock Backend";

    public async IAsyncEnumerable<string> SendMessageAsync(
        string message,
        [EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Mock: Received message: {Message}", message);

        // Simulate thinking delay
        await Task.Delay(300, cancellationToken);

        var response = GenerateDemoResponse(message);
        var words = response.Split(' ');

        foreach (var word in words)
        {
            if (cancellationToken.IsCancellationRequested)
                yield break;

            yield return word + " ";
            await Task.Delay(40, cancellationToken); // Simulate typing delay
        }
    }

    public Task<bool> IsAvailableAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult(true); // Mock is always available
    }

    private static string GenerateDemoResponse(string userMessage)
    {
        var lowerMessage = userMessage.ToLower();

        if (lowerMessage.Contains("hello") || lowerMessage.Contains("hi"))
        {
            return "Hello! I'm the **mock backend** for the OpenCode Chat demo. " +
                   "I simulate AI responses with streaming to demonstrate the chat interface. " +
                   "Try asking me about code or saying 'help'!";
        }

        if (lowerMessage.Contains("help"))
        {
            return "This is a demo chat application showcasing:\n\n" +
                   "1. **Blazor Server** with interactive components\n" +
                   "2. **MudBlazor** UI framework with OpenCode theming\n" +
                   "3. **SignalR** for real-time communication\n" +
                   "4. **Streaming responses** word-by-word\n\n" +
                   "You can switch to the real OpenCode backend by restarting with `--opencode` or selecting it from the mode dialog.";
        }

        if (lowerMessage.Contains("code") || lowerMessage.Contains("example"))
        {
            return "Here's an example of how streaming works:\n\n" +
                   "```csharp\n" +
                   "await foreach (var chunk in chatService.SendMessageAsync(message))\n" +
                   "{\n" +
                   "    response += chunk;\n" +
                   "    StateHasChanged();\n" +
                   "}\n" +
                   "```\n\n" +
                   "The response is streamed word by word to demonstrate real-time updates.";
        }

        if (lowerMessage.Contains("opencode"))
        {
            return "**OpenCode** is an AI-powered coding assistant. " +
                   "To use the real OpenCode backend instead of this mock:\n\n" +
                   "1. Start `opencode serve` in another terminal\n" +
                   "2. Restart this app with `--opencode`\n" +
                   "3. Or select 'OpenCode' from the mode dialog at startup";
        }

        return $"Thanks for your message: \"{userMessage}\"\n\n" +
               "This is a **mock response** demonstrating the streaming chat interface. " +
               "The actual OpenCode integration would process your request through the AI backend " +
               "and stream back intelligent responses.\n\n" +
               "_Tip: Try saying 'hello', 'help', 'code', or 'opencode'._";
    }
}
