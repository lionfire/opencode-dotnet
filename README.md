# LionFire.OpenCode.Serve

A .NET SDK for interacting with [OpenCode](https://github.com/opencode-ai/opencode) local headless server (`opencode serve`).

[![NuGet](https://img.shields.io/nuget/v/LionFire.OpenCode.Serve.svg)](https://www.nuget.org/packages/LionFire.OpenCode.Serve)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Status

Early WIP / prerelease.  Though the basic communication with OpenCode appears to be working.

## Features

- Full coverage of OpenCode Server API endpoints
- IAsyncEnumerable streaming for real-time responses
- Microsoft.Extensions.AI IChatClient integration
- Dependency injection support with IHttpClientFactory
- Source-generated JSON serialization (AOT-compatible)
- OpenTelemetry tracing support
- Hierarchical exception handling with troubleshooting hints
- Session scopes with automatic cleanup

## Quick Start (30 Seconds)

```bash
# Install the NuGet package
dotnet add package LionFire.OpenCode.Serve
```

```csharp
using LionFire.OpenCode.Serve;

// Create a client
var client = new OpenCodeClient();

// Create a session and send a message
var session = await client.CreateSessionAsync();
var response = await client.SendMessageAsync(session.Id, "Hello, OpenCode!");

Console.WriteLine(response.Parts.OfType<TextPart>().First().Text);
```

## Prerequisites

- .NET 8.0 or later
- OpenCode server running locally (`opencode serve`)

## Installation

```bash
dotnet add package LionFire.OpenCode.Serve
```

For Microsoft.Extensions.AI integration:

```bash
dotnet add package LionFire.OpenCode.Serve.AgentFramework
```

## Usage Examples

### Basic Usage

```csharp
using LionFire.OpenCode.Serve;
using LionFire.OpenCode.Serve.Models;

// Create a client with default options (localhost:9123)
var client = new OpenCodeClient();

// Or with custom options
var client = new OpenCodeClient(new OpenCodeClientOptions
{
    BaseUrl = "http://localhost:9123",
    DefaultTimeout = TimeSpan.FromSeconds(60),
    EnableTelemetry = true
});

// Create a session
var session = await client.CreateSessionAsync("/path/to/project");

// Send a message
var response = await client.SendMessageAsync(session.Id, "Explain this codebase");

// Get text from response
foreach (var part in response.Parts.OfType<TextPart>())
{
    Console.WriteLine(part.Text);
}
```

### Streaming Responses

```csharp
await foreach (var update in client.SendMessageStreamingAsync(sessionId, "Write a test suite"))
{
    if (update.Delta is not null)
    {
        Console.Write(update.Delta);
    }

    if (update.Done)
    {
        Console.WriteLine("\n[Complete]");
    }
}
```

### Using Streaming Extensions

```csharp
using LionFire.OpenCode.Serve.Extensions;

// Subscribe with callbacks
await client.SendMessageStreamingAsync(sessionId, "Hello")
    .Subscribe(
        onUpdate: delta => Console.Write(delta),
        onComplete: () => Console.WriteLine("\n[Done]"),
        onError: ex => Console.Error.WriteLine(ex.Message));

// Collect all text
string fullResponse = await client.SendMessageStreamingAsync(sessionId, "Hello")
    .ToStringAsync();
```

### Session Scopes (Automatic Cleanup)

```csharp
await using (var scope = await client.CreateSessionScopeAsync())
{
    var response = await client.SendMessageAsync(scope.SessionId, "Hello!");
    // Session automatically deleted when scope is disposed
}
```

### Dependency Injection

```csharp
// In Startup or Program.cs
services.AddOpenCodeClient(options =>
{
    options.BaseUrl = "http://localhost:9123";
    options.EnableTelemetry = true;
});

// In your service
public class MyService
{
    private readonly IOpenCodeClient _client;

    public MyService(IOpenCodeClient client)
    {
        _client = client;
    }

    public async Task<string> AskQuestion(string question)
    {
        await using var scope = await _client.CreateSessionScopeAsync();
        var response = await _client.SendMessageAsync(scope.SessionId, question);
        return response.Parts.OfType<TextPart>().First().Text;
    }
}
```

### Microsoft.Extensions.AI Integration

```csharp
using LionFire.OpenCode.Serve.AgentFramework;
using Microsoft.Extensions.AI;

// Create IChatClient from OpenCode
IChatClient chatClient = client.AsChatClient();

// Use the standard AI abstractions
var response = await chatClient.GetResponseAsync([
    new ChatMessage(ChatRole.User, "Hello!")
]);

Console.WriteLine(response.Messages[0].Text);
```

#### With Dependency Injection

```csharp
// Register both client and chat client
services.AddOpenCodeClient(options => options.BaseUrl = "http://localhost:9123");
services.AddOpenCodeChatClient(chatOptions => chatOptions.ModelId = "opencode");

// Use in your code
public class ChatService
{
    private readonly IChatClient _chatClient;

    public ChatService(IChatClient chatClient)
    {
        _chatClient = chatClient;
    }
}
```

### Error Handling

```csharp
try
{
    var session = await client.CreateSessionAsync();
}
catch (OpenCodeConnectionException ex)
{
    Console.WriteLine($"Cannot connect: {ex.Message}");
    Console.WriteLine($"Hint: {ex.TroubleshootingHint}");
    // Hint: "Ensure `opencode serve` is running..."
}
catch (OpenCodeApiException ex)
{
    Console.WriteLine($"API error {ex.StatusCode}: {ex.Message}");
}
catch (OpenCodeTimeoutException ex)
{
    Console.WriteLine($"Timeout: {ex.Message}");
    Console.WriteLine($"Hint: {ex.TroubleshootingHint}");
}
```

### Tool Operations

```csharp
// List available tools
var tools = await client.GetToolsAsync(sessionId);

// Approve/reject tool execution
await client.ApproveToolAsync(sessionId, toolId, approved: true);
```

### File Operations

```csharp
// List files in session context
var files = await client.GetFilesAsync(sessionId);

// Get specific file content
var content = await client.GetFileContentAsync(sessionId, "/path/to/file.cs");
```

### Pagination

```csharp
using LionFire.OpenCode.Serve.Models;

// List sessions with pagination
var options = new PaginationOptions
{
    Limit = 10,
    Offset = 0
};

var sessions = await client.GetSessionsAsync(options);
```

## API Reference

### IOpenCodeClient

| Method | Description |
|--------|-------------|
| `CreateSessionAsync` | Create a new OpenCode session |
| `GetSessionAsync` | Get session by ID |
| `GetSessionsAsync` | List all sessions with pagination |
| `DeleteSessionAsync` | Delete a session |
| `SendMessageAsync` | Send a message and wait for response |
| `SendMessageStreamingAsync` | Send a message and stream response |
| `GetMessagesAsync` | Get messages for a session |
| `GetToolsAsync` | List available tools |
| `ApproveToolAsync` | Approve or reject tool execution |
| `GetFilesAsync` | List files in session context |
| `GetFileContentAsync` | Get file content |
| `GetConfigAsync` | Get server configuration |
| `CreateSessionScopeAsync` | Create auto-disposing session scope |

### Configuration Options

```csharp
var options = new OpenCodeClientOptions
{
    BaseUrl = "http://localhost:9123",      // Server URL
    DefaultTimeout = TimeSpan.FromSeconds(30), // Quick operations
    MessageTimeout = TimeSpan.FromMinutes(5),  // AI operations
    EnableTelemetry = true,                    // OpenTelemetry
    ValidateOnStart = true,                    // Validate connection
    EnableSourceGeneration = true              // AOT JSON
};
```

## Exception Hierarchy

```
OpenCodeException (base)
+-- OpenCodeApiException (HTTP errors with status code)
|   +-- OpenCodeNotFoundException (404)
|   +-- OpenCodeRateLimitException (429)
+-- OpenCodeConnectionException (server not reachable)
+-- OpenCodeTimeoutException (operation timeout)
+-- OpenCodeSerializationException (JSON errors)
```

All exceptions include `TroubleshootingHint` property with actionable guidance.

## AOT Support

The SDK supports Native AOT compilation through source-generated JSON serialization:

```csharp
services.AddOpenCodeClient(options =>
{
    options.EnableSourceGeneration = true;
});
```

## OpenTelemetry

Enable tracing with:

```csharp
services.AddOpenTelemetry()
    .WithTracing(tracing => tracing
        .AddSource("LionFire.OpenCode.Serve"));
```

## Requirements

- .NET 8.0 or later
- OpenCode server running (`opencode serve`)

## Related Projects

- [OpenCode](https://github.com/opencode-ai/opencode) - AI-powered coding assistant
- [Microsoft.Extensions.AI](https://www.nuget.org/packages/Microsoft.Extensions.AI.Abstractions) - AI abstractions for .NET

## License

MIT License - see [LICENSE](LICENSE) for details.

## Contributing

Contributions are welcome! Please read our [Contributing Guide](CONTRIBUTING.md) for details.
