using LionFire.OpenCode.Serve;
using LionFire.OpenCode.Serve.AgentFramework;
using LionFire.OpenCode.Serve.Exceptions;
using Microsoft.Extensions.AI;

Console.WriteLine("LionFire.OpenCode.Serve - IChatClient Integration Example");
Console.WriteLine("=".PadRight(55, '='));
Console.WriteLine();

try
{
    // Create OpenCode client
    var openCodeClient = new OpenCodeClient();

    // Example 1: Basic IChatClient usage
    Console.WriteLine("1. Creating IChatClient from OpenCode client...");
    IChatClient chatClient = openCodeClient.AsChatClient();
    var openCodeChatClient = chatClient as OpenCodeChatClient;
    Console.WriteLine($"   Provider: {openCodeChatClient?.Metadata.ProviderName ?? "OpenCode"}");
    Console.WriteLine();

    // Example 2: Send a simple message
    Console.WriteLine("2. Sending a message via IChatClient...");
    var messages = new List<ChatMessage>
    {
        new(ChatRole.User, "What is 2 + 2? Reply with just the number.")
    };

    var response = await chatClient.GetResponseAsync(messages);
    Console.WriteLine($"   Response ID: {response.ResponseId}");
    Console.WriteLine($"   Content: {response.Messages[0].Text}");
    Console.WriteLine();

    // Example 3: Multi-turn conversation
    Console.WriteLine("3. Multi-turn conversation...");
    var conversation = new List<ChatMessage>
    {
        new(ChatRole.System, "You are a helpful math tutor. Keep answers brief."),
        new(ChatRole.User, "What is the formula for the area of a circle?")
    };

    var tutorResponse = await chatClient.GetResponseAsync(conversation);
    Console.WriteLine($"   Tutor: {tutorResponse.Messages[0].Text?.Substring(0, Math.Min(150, tutorResponse.Messages[0].Text?.Length ?? 0))}...");
    Console.WriteLine();

    // Example 4: Streaming responses
    Console.WriteLine("4. Streaming response...");
    Console.Write("   ");

    var streamMessages = new List<ChatMessage>
    {
        new(ChatRole.User, "Count from 1 to 5, one number per line.")
    };

    // Create a new chat client for streaming (to get fresh session)
    var streamChatClient = openCodeClient.AsChatClient();

    await foreach (var update in streamChatClient.GetStreamingResponseAsync(streamMessages))
    {
        if (!string.IsNullOrEmpty(update.Text))
        {
            Console.Write(update.Text);
        }

        if (update.FinishReason == ChatFinishReason.Stop)
        {
            Console.WriteLine();
            Console.WriteLine("   [Stream completed]");
        }
    }
    Console.WriteLine();

    // Example 5: Using with specific session
    Console.WriteLine("5. Using IChatClient with specific session...");
    var session = await openCodeClient.CreateSessionAsync();
    var sessionChatClient = openCodeClient.AsChatClient(session.Id);

    var sessionResponse = await sessionChatClient.GetResponseAsync([
        new ChatMessage(ChatRole.User, "What's my session ID?")
    ]);
    Console.WriteLine($"   Response: {sessionResponse.Messages[0].Text?.Substring(0, Math.Min(100, sessionResponse.Messages[0].Text?.Length ?? 0))}...");

    // Clean up session
    await openCodeClient.DeleteSessionAsync(session.Id);
    Console.WriteLine("   Session cleaned up.");
    Console.WriteLine();

    // Example 6: Accessing underlying services
    Console.WriteLine("6. Accessing underlying OpenCode client...");
    var underlyingClient = chatClient.GetService(typeof(IOpenCodeClient)) as IOpenCodeClient;
    if (underlyingClient is not null)
    {
        var config = await underlyingClient.GetConfigAsync();
        Console.WriteLine($"   OpenCode version: {config.Version}");
    }
    Console.WriteLine();

    // Example 7: Using session scope with IChatClient
    Console.WriteLine("7. Session scope with IChatClient...");
    await using (var scope = await openCodeClient.CreateSessionScopeAsync())
    {
        var scopeChatClient = scope.AsChatClient(openCodeClient);
        var scopeResponse = await scopeChatClient.GetResponseAsync([
            new ChatMessage(ChatRole.User, "Hello from scoped session!")
        ]);
        Console.WriteLine($"   Session ID: {scope.SessionId}");
        Console.WriteLine($"   Response: {scopeResponse.Messages[0].Text?.Substring(0, Math.Min(80, scopeResponse.Messages[0].Text?.Length ?? 0))}...");
    }
    Console.WriteLine("   Session cleaned up automatically.");
    Console.WriteLine();

    Console.WriteLine("All examples completed successfully!");
}
catch (OpenCodeConnectionException ex)
{
    Console.WriteLine($"ERROR: Could not connect to OpenCode server.");
    Console.WriteLine($"       {ex.Message}");
    Console.WriteLine();
    Console.WriteLine("Make sure the OpenCode server is running:");
    Console.WriteLine("  opencode serve --port 9123");
}
catch (OpenCodeApiException ex)
{
    Console.WriteLine($"ERROR: API error (HTTP {(int)ex.StatusCode})");
    Console.WriteLine($"       {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
}
