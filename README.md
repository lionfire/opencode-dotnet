# LionFire.OpenCode.Serve

A .NET SDK for interacting with [OpenCode](https://github.com/opencode-ai/opencode) local headless server (`opencode serve`).

[![NuGet](https://img.shields.io/nuget/v/LionFire.OpenCode.Serve.svg)](https://www.nuget.org/packages/LionFire.OpenCode.Serve)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## Status

Experimental vibe code. See [Plan Status](#plan-status).
- the basic communication with OpenCode appears to be working.

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

### Permission Handling

```csharp
// Respond to a permission request
// Permissions cover bash, edit, webfetch, and other operations
await client.RespondToPermissionAsync(sessionId, permissionId, PermissionResponse.Once);

// Options: PermissionResponse.Once, PermissionResponse.Always, PermissionResponse.Reject
```

### File Operations (Directory-scoped)

```csharp
// List files in a directory
var files = await client.ListFilesAsync(path: ".", directory: "/path/to/project");

// Read file content
var content = await client.ReadFileAsync("/path/to/file.cs", directory: "/path/to/project");

// Get git status for files
var status = await client.GetFileStatusAsync(directory: "/path/to/project");
```

### Find Operations

```csharp
// Search for text in files (ripgrep)
var textMatches = await client.FindTextAsync("TODO", directory: "/path/to/project");

// Search for files by name
var files = await client.FindFilesAsync("*.cs", directory: "/path/to/project");

// Search for symbols (LSP)
var symbols = await client.FindSymbolsAsync("MyClass", directory: "/path/to/project");
```

## API Reference

### IOpenCodeClient

#### Core Session Operations
| Method | Description |
|--------|-------------|
| `CreateSessionAsync` | Create a new OpenCode session |
| `GetSessionAsync` | Get session by ID |
| `ListSessionsAsync` | List all sessions |
| `DeleteSessionAsync` | Delete a session |
| `ForkSessionAsync` | Fork session at message point |
| `ShareSessionAsync` | Share session (get share URL) |
| `UnshareSessionAsync` | Remove session sharing |
| `UpdateSessionAsync` | Update session (title, etc.) |

#### Message Operations
| Method | Description |
|--------|-------------|
| `PromptAsync` | Send message and get AI response |
| `PromptAsyncNonBlocking` | Send message asynchronously (returns immediately) |
| `ListMessagesAsync` | Get all messages in session |
| `GetMessageAsync` | Get specific message by ID |

#### Permission Operations
| Method | Description |
|--------|-------------|
| `RespondToPermissionAsync` | Respond to permission request (once/always/reject) |

#### File Operations (Directory-scoped)
| Method | Description |
|--------|-------------|
| `ListFilesAsync` | List files in directory |
| `ReadFileAsync` | Read file content |
| `GetFileStatusAsync` | Get git status for files |

#### Find Operations
| Method | Description |
|--------|-------------|
| `FindTextAsync` | Search text in files (ripgrep) |
| `FindFilesAsync` | Search files by name |
| `FindSymbolsAsync` | Search symbols (LSP) |

#### Configuration
| Method | Description |
|--------|-------------|
| `GetConfigAsync` | Get server configuration |
| `UpdateConfigAsync` | Update configuration |
| `GetConfiguredProvidersAsync` | Get configured AI providers |
| `GetAllProvidersAsync` | Get all available providers |

#### Advanced Features
| Method | Description |
|--------|-------------|
| `CreatePtyAsync` | Create pseudo-terminal session |
| `ListPtySessionsAsync` | List PTY sessions |
| `SubscribeToEventsAsync` | Subscribe to server events (SSE) |
| `GetVcsInfoAsync` | Get version control info |
| `ListAgentsAsync` | List available agents |

See full interface at `/src/opencode-dotnet/src/LionFire.OpenCode.Serve/IOpenCodeClient.cs` for all 60+ methods.

## Version 2.0 Breaking Changes

The SDK was completely redesigned in v2.0 to match the actual OpenCode serve API specification. Key changes:

### API Paradigm Shift

**v1.0 (Hypothetical)**: Session-scoped operations
```csharp
// OLD - Does not match actual API
var files = await client.GetFilesAsync(sessionId);
await client.ApproveToolAsync(sessionId, toolId);
```

**v2.0 (Actual API)**: Directory-scoped operations
```csharp
// NEW - Matches actual OpenCode serve API
var files = await client.ListFilesAsync(path: ".", directory: "/project");
await client.RespondToPermissionAsync(sessionId, permissionId, PermissionResponse.Once);
```

### Major Changes

1. **File Operations**:
   - `GetFilesAsync(sessionId)` → `ListFilesAsync(path, directory)`
   - `GetFileContentAsync(sessionId, path)` → `ReadFileAsync(path, directory)`
   - Files are directory-scoped, not session-scoped

2. **Permission System**:
   - `ApproveToolAsync(sessionId, toolId)` → `RespondToPermissionAsync(sessionId, permissionId, response)`
   - Permissions are generic (cover bash, edit, webfetch), not tool-specific

3. **Messages**:
   - `SendMessageAsync()` → `PromptAsync()`
   - Return type changed from `Message` to `MessageWithParts`
   - Messages split into `UserMessage` and `AssistantMessage`

4. **Session Model**:
   - Complete restructure with new properties: `ProjectId`, `Title`, `Version`, `Time`, `Summary`
   - Timestamps now use Unix epoch (milliseconds)

5. **Removed Features**:
   - No pagination API (removed `PaginationOptions`)
   - No batch file apply (`ApplyChangesAsync` removed)
   - No `ISessionScope` helper (use manual session cleanup)

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

<!-- START: axi-plan status -->
## Plan Status

- **Specs:** 2
- **Phases:** 4
- **Epics:** 24
- **Progress:** 92.3% (632/685 tasks)
- **Greenlit:** 4
- **Done:** 18
- **Needs Attention:** 0


### Specifications

| ID | Name | Status | Phases |
|:---|:-----|:-------|-------:|
| PRP | Product Requirements Prompt: opencode-dotnet | Draft | 0 |
| 01 | LionFire.OpenCode.Serve.AgentFramework - Technical Specification | Draft | 0 |


### Phases

| Phase | Name | Epics | Completion |
|------:|:-----|------:|-----------:|
| 1 | MVP Foundation | 6 | 100% |
| 2 | Production Hardening | 6 | 96% |
| 3 | Agent Framework Integration | 6 | 84% |
| 4 | Polish and Release | 6 | 50% |


### Epics

| Greenlit | Phase | Epic | Title | Tasks | Completion |
|:--------:|------:|-----:|:------|------:|-----------:|
| ✅ | 1 | 1 | Core Client Infrastructure | 190/190 | 100% |
| ✅ | 1 | 2 | Session Management API | 115/115 | 100% |
| ✅ | 1 | 3 | Message and Streaming API | 65/65 | 100% |
| ✅ | 1 | 4 | File Operations, Permissions, and Command APIs | 26/26 | 100% |
| ✅ | 1 | 5 | Error Handling and Logging | 40/40 | 100% |
| ✅ | 1 | 6 | Testing and Examples | 37/37 | 100% |
|  | 2 | 1 | Source-Generated JSON and AOT | 5/6 | 83% |
| ✅ | 2 | 2 | HttpClientFactory and Connection Management | 10/10 | 100% |
| ✅ | 2 | 3 | Polly Resilience Integration | 8/8 | 100% |
|  | 2 | 4 | OpenTelemetry Observability | 7/8 | 88% |
| ✅ | 2 | 5 | Advanced API Features | 12/12 | 100% |
| ✅ | 2 | 6 | Performance Optimization | 9/9 | 100% |
| ✅ | 3 | 1 | OpencodeAgent Core Implementation | 15/15 | 100% |
| ✅ | 3 | 2 | Message Conversion Layer | 9/9 | 100% |
| ✅ | 3 | 3 | Thread Management and Serialization | 0/14 | 0% |
| ✅ | 3 | 4 | Streaming Integration | 7/7 | 100% |
| ✅ | 3 | 5 | DI Extensions and Builder Pattern | 17/17 | 100% |
| ✅ | 3 | 6 | Testing and Examples | 23/23 | 100% |
| ✅ | 4 | 1 | Documentation Suite | 12/12 | 100% |
| ✅ | 4 | 2 | Example Projects | 13/13 | 100% |
| ✅ | 4 | 3 | NuGet Packaging and Publishing | 12/12 | 100% |
| ✅ | 4 | 4 | Community Infrastructure | 0/13 | 0% |
| ✅ | 4 | 5 | Security and Legal Review | 0/11 | 0% |
| ✅ | 4 | 6 | Launch and Marketing | 0/13 | 0% |
<!-- END: axi-plan status -->

## Related Projects

- [OpenCode](https://github.com/opencode-ai/opencode) - AI-powered coding assistant
- [Microsoft.Extensions.AI](https://www.nuget.org/packages/Microsoft.Extensions.AI.Abstractions) - AI abstractions for .NET

## License

MIT License - see [LICENSE](LICENSE) for details.

## Contributing

Contributions are welcome! Please read our [Contributing Guide](CONTRIBUTING.md) for details.
