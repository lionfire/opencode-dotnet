# Basic Usage Example

This example demonstrates the core functionality of the LionFire.OpenCode.Serve SDK.

## Prerequisites

1. Install OpenCode CLI: https://github.com/opencode-ai/opencode
2. Start the OpenCode server:
   ```bash
   opencode serve --port 9123
   ```

## Running the Example

```bash
dotnet run
```

## What This Example Shows

1. **Health Check** - Verify the server is running
2. **Configuration** - Get server configuration and version
3. **Tools** - List available tools
4. **Session Management** - Create sessions with automatic cleanup
5. **Message Sending** - Send messages and receive responses
6. **Message History** - Retrieve conversation history
7. **Streaming** - Stream responses using `IAsyncEnumerable<T>`

## Code Highlights

### Creating a Client

```csharp
// Default: localhost:9123
await using var client = new OpenCodeClient();

// Custom URL
await using var client = new OpenCodeClient("http://localhost:8080");

// With options
await using var client = new OpenCodeClient(new OpenCodeClientOptions
{
    BaseUrl = "http://localhost:9123",
    MessageTimeout = TimeSpan.FromMinutes(10)
});
```

### Session Scope Pattern

```csharp
// Session is automatically deleted when scope is disposed
await using (var scope = await client.CreateSessionScopeAsync())
{
    var response = await client.SendMessageAsync(scope.SessionId, "Hello!");
}
// Session deleted here
```

### Streaming Responses

```csharp
await foreach (var update in client.SendMessageStreamingAsync(sessionId, "Hello"))
{
    if (update.Delta is not null)
    {
        Console.Write(update.Delta);
    }
}
```

### Error Handling

```csharp
try
{
    var session = await client.GetSessionAsync("invalid-id");
}
catch (OpenCodeNotFoundException ex)
{
    Console.WriteLine($"Session not found: {ex.ResourceId}");
}
catch (OpenCodeConnectionException ex)
{
    Console.WriteLine($"Server not reachable: {ex.BaseUrl}");
}
```
