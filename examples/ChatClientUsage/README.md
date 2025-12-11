# ChatClientUsage Example

This example demonstrates how to use the LionFire.OpenCode.Serve SDK with Microsoft.Extensions.AI IChatClient interface.

## Prerequisites

- .NET 8.0 SDK
- OpenCode server running (`opencode serve`)

## Running the Example

```bash
# From the examples/ChatClientUsage directory
dotnet run

# Or from the repository root
dotnet run --project examples/ChatClientUsage/ChatClientUsage.csproj
```

## What This Example Demonstrates

1. **Creating IChatClient from OpenCode** - Wrapping the OpenCode client as a standard IChatClient
2. **Simple Message** - Sending a basic message and receiving a response
3. **Multi-turn Conversation** - Maintaining conversation context with system messages
4. **Streaming Responses** - Real-time streaming using IAsyncEnumerable
5. **Session Management** - Using specific sessions with IChatClient
6. **Service Resolution** - Accessing the underlying OpenCode client
7. **Session Scopes** - Using auto-cleanup session scopes with IChatClient

## Key APIs Used

- `openCodeClient.AsChatClient()` - Create IChatClient from OpenCode client
- `chatClient.GetResponseAsync()` - Send messages and get response
- `chatClient.GetStreamingResponseAsync()` - Stream responses
- `scope.AsChatClient()` - Create IChatClient from session scope
- `chatClient.GetService()` - Access underlying services

## Expected Output

```
LionFire.OpenCode.Serve - IChatClient Integration Example
=======================================================

1. Creating IChatClient from OpenCode client...
   Provider: OpenCode

2. Sending a message via IChatClient...
   Response ID: msg_xxx
   Content: 4

3. Multi-turn conversation...
   Tutor: The area of a circle is A = pi * r^2...

4. Streaming response...
   1
   2
   3
   4
   5
   [Stream completed]

5. Using IChatClient with specific session...
   Response: I don't have direct access to see my session ID...
   Session cleaned up.

6. Accessing underlying OpenCode client...
   OpenCode version: 0.x.x

7. Session scope with IChatClient...
   Session ID: session_xxx
   Response: Hello! How can I help you today?...
   Session cleaned up automatically.

All examples completed successfully!
```

## Related Examples

- **BasicUsage** - Direct OpenCode client usage
- **DependencyInjection** - Using IChatClient with ASP.NET Core DI
