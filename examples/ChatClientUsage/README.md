# ChatClientUsage Example

> **Status: Working** - This example runs successfully against a live `opencode serve` instance.

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

## Sample Output

```
LionFire.OpenCode.Serve - IChatClient Integration Example
=======================================================

Using default model (use -p and -m to specify a model)
Tip: Run with --list-models --free to see free models

1. Creating IChatClient from OpenCode client...
   Provider: OpenCode

2. Sending a message via IChatClient...
   Response ID: msg_b3eb2fb93001ub7QpkrTvLP6Vd
   Content: 4

3. Multi-turn conversation...
   Tutor: The formula for the area of a circle is:
   $ A = \pi r^2 $
   Where A is the Area, π (Pi) is approximately 3.14159, r is the rad...

4. Streaming response...

5. Using IChatClient with specific session...
   Response: Hello, I am opencode, an interactive CLI agent specializing in software engineering tasks....
   Session cleaned up.

6. Accessing underlying OpenCode client...

7. Quick chat demonstration...
   Q: What's the capital of France?
   A: Paris

All examples completed successfully!
```

## Related Examples

- **BasicUsage** - Direct OpenCode client usage
- **DependencyInjection** - Using IChatClient with ASP.NET Core DI
