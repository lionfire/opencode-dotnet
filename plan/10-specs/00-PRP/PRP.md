# Product Requirements Prompt: opencode-dotnet

## Product Vision

**opencode-dotnet** is a community-maintained .NET SDK for [OpenCode](https://github.com/opencode-ai/opencode) - the open-source terminal-first AI coding agent. The SDK provides a fully-typed, async-first, production-ready client for the **OpenCode Server API** (`opencode serve`), enabling .NET developers to programmatically interact with OpenCode sessions, send prompts, receive AI responses, and integrate AI-powered coding assistance into their applications.

The vision is to be the **definitive .NET client** for OpenCode, following quality conventions of established local-server client libraries like `Docker.DotNet` (for Docker daemon) and LSP client libraries (for language servers).

## What is OpenCode?

OpenCode is a **terminal-based AI coding assistant** that:
- Provides an interactive TUI built with Bubble Tea (Go)
- Supports multiple AI providers (OpenAI, Anthropic Claude, Google Gemini, AWS Bedrock, Groq, Azure OpenAI, OpenRouter)
- Offers session management with SQLite persistence
- Includes tool integration allowing AI to execute commands and modify code
- Exposes a **headless HTTP server** via `opencode serve` for programmatic access

### OpenCode Server API (`opencode serve`)

When running `opencode serve --port 9123`, OpenCode exposes a REST API at `http://localhost:9123` with:
- **OpenAPI 3.1 spec** at `/doc`
- Session-based conversation management
- Multi-part messages (text, files, agents)
- Tool execution and permissions
- File operations and search
- MCP (Model Context Protocol) server integration

## Target Audience

### Primary Audiences

1. **.NET Application Developers**
   - Building applications that need AI-powered coding assistance
   - Integrating OpenCode capabilities into existing .NET solutions
   - Creating developer tools, IDEs, or productivity applications

2. **.NET Library/SDK Authors**
   - Building higher-level abstractions on top of OpenCode
   - Creating domain-specific tooling with AI capabilities
   - Developing Visual Studio or VS Code extensions

3. **Enterprise Development Teams**
   - Automating code generation workflows
   - Building internal developer platforms with AI assistance
   - Creating CI/CD integrations for code quality and generation

### Secondary Audiences

4. **Open Source Contributors**
   - Contributing to the SDK itself
   - Extending functionality for specific use cases

5. **Educators and Researchers**
   - Teaching AI-assisted development
   - Researching code generation patterns

## Goals and Objectives

### Primary Goals

1. **Complete API Coverage** - Implement all OpenCode Server API endpoints with full feature parity
2. **Developer Experience** - Provide excellent IntelliSense, XML docs, and intuitive API design
3. **Production Readiness** - Include proper error handling, retries, timeouts, and cancellation support
4. **Modern .NET Patterns** - Use async/await, IAsyncEnumerable for streaming, source-generated JSON

### Success Metrics

- Published to NuGet with stable versioning (SemVer)
- Achieves >80% code coverage in tests
- Zero critical security vulnerabilities
- Comprehensive XML documentation for all public APIs
- Working examples for common use cases

## Core Features (Based on Actual OpenCode API)

### MVP (Must Have)

#### F1: Client Configuration
- Base URL configuration (default: `http://localhost:9123`)
- Optional directory parameter for multi-project support
- Timeout and retry configuration
- HttpClient injection support

#### F2: Session Management API
- `CreateSessionAsync()` - Create a new session
- `GetSessionAsync(id)` - Get session details
- `ListSessionsAsync()` - List all sessions
- `DeleteSessionAsync(id)` - Delete a session
- `ForkSessionAsync(id, messageId)` - Fork a session at a specific message
- `AbortSessionAsync(id)` - Abort a running session
- `ShareSessionAsync(id)` / `UnshareSessionAsync(id)` - Share/unshare sessions

#### F3: Message/Prompt API
- `SendMessageAsync(sessionId, parts)` - Send a message to a session
- `GetMessagesAsync(sessionId)` - Get all messages in a session
- `GetMessageAsync(sessionId, messageId)` - Get a specific message
- Support for multi-part messages:
  - `TextPart` - Text content
  - `FilePart` - File references
  - `AgentPart` - Agent-specific content

#### F4: Strongly-Typed Models
- Request/response DTOs for all API operations
- `Session`, `Message`, `Part` models
- `Provider`, `Model`, `Agent` models
- `Todo`, `Command`, `Tool` models
- Proper nullability annotations

#### F5: Error Handling
- Custom exception types (`OpencodeApiException`, `OpencodeNotFoundException`, etc.)
- Structured error responses with codes and messages
- Automatic retry with exponential backoff for transient failures

#### F6: Cancellation Support
- `CancellationToken` support on all async methods
- Proper cleanup on cancellation

### Post-MVP (Should Have)

#### F7: Streaming/Real-time Updates
- Server-Sent Events (SSE) support for real-time message updates
- `IAsyncEnumerable<T>` for streaming responses
- Progress reporting during long operations

#### F8: Dependency Injection Integration
- `IServiceCollection` extension methods
- `IOptions<OpencodeClientOptions>` pattern
- Named/keyed client registration

#### F9: File Operations API
- `ListFilesAsync(path)` - List files and directories
- `ReadFileAsync(path)` - Read file content
- `GetFileStatusAsync()` - Get file status (git status)

#### F10: Search API
- `FindTextAsync(pattern)` - Search text in files (ripgrep-style)
- `FindFilesAsync(query)` - Find files by name
- `FindSymbolsAsync(query)` - Find workspace symbols (LSP)

#### F11: Command Execution API
- `SendCommandAsync(sessionId, command, args)` - Execute slash commands
- `RunShellAsync(sessionId, command)` - Run shell commands
- `ListCommandsAsync()` - List available commands

### Future (Nice to Have)

#### F12: Tool Management API
- `GetToolIdsAsync()` - List all tool IDs
- `GetToolsAsync(provider, model)` - Get tools for a provider/model
- Permission handling for tool execution

#### F13: Configuration API
- `GetConfigAsync()` - Get current configuration
- `UpdateConfigAsync(config)` - Update configuration
- `GetProvidersAsync()` - List available AI providers

#### F14: Agent Management API
- `ListAgentsAsync()` - List available agents
- Agent selection per request

#### F15: MCP Integration
- `GetMcpStatusAsync()` - Get MCP server status
- MCP server management

#### F16: Telemetry and Observability
- OpenTelemetry integration
- Activity/trace propagation
- Metrics (request count, latency, token usage)

## Technical Considerations

### Target Frameworks
- .NET 8+ (primary target, leveraging latest features)
- No legacy framework support (clean modern codebase)

### Dependencies (Minimal)
- `System.Text.Json` - JSON serialization (source-generated where possible)
- `System.Net.Http` - HTTP client (no external HTTP libraries)
- `Microsoft.Extensions.Options` - Configuration (optional)
- `Microsoft.Extensions.DependencyInjection.Abstractions` - DI (optional)

### Architecture Decisions

1. **No external HTTP library** - Use native `HttpClient` for minimal dependencies
2. **Source-generated JSON** - Use `System.Text.Json` source generators for AOT compatibility
3. **Nullable reference types** - Full nullability annotations throughout
4. **Record types** - Use records for immutable DTOs where appropriate
5. **Interface-based design** - `IOpencodeClient` for testability

### Package Structure
```
OpencodeAI/
├── OpencodeClient.cs           # Main client implementation
├── IOpencodeClient.cs          # Client interface
├── OpencodeClientOptions.cs    # Configuration options
├── Models/
│   ├── Sessions/               # Session-related DTOs
│   ├── Messages/               # Message and Part DTOs
│   ├── Files/                  # File operation DTOs
│   ├── Commands/               # Command DTOs
│   ├── Tools/                  # Tool DTOs
│   └── Config/                 # Configuration DTOs
├── Exceptions/                 # Custom exceptions
├── Streaming/                  # SSE and streaming infrastructure
├── Extensions/                 # DI and configuration extensions
└── Internal/                   # Internal implementation details
```

## OpenCode API Reference

### Base URL
- Local: `http://localhost:{port}` (default port is random, use `--port` to specify)
- OpenAPI spec: `GET /doc`

### Key Endpoints

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/session` | GET | List all sessions |
| `/session` | POST | Create a new session |
| `/session/{id}` | GET | Get session details |
| `/session/{id}` | DELETE | Delete a session |
| `/session/{id}` | PATCH | Update session properties |
| `/session/{id}/message` | GET | List messages in session |
| `/session/{id}/message` | POST | Send a message (prompt) |
| `/session/{id}/message/{msgId}` | GET | Get a specific message |
| `/session/{id}/abort` | POST | Abort a running session |
| `/session/{id}/fork` | POST | Fork session at a message |
| `/session/{id}/share` | POST | Share a session |
| `/session/{id}/command` | POST | Execute a command |
| `/session/{id}/shell` | POST | Run a shell command |
| `/session/{id}/revert` | POST | Revert a message |
| `/session/{id}/todo` | GET | Get session todo list |
| `/config` | GET | Get configuration |
| `/config` | PATCH | Update configuration |
| `/config/providers` | GET | List AI providers |
| `/file` | GET | List files |
| `/file/content` | GET | Read file content |
| `/file/status` | GET | Get file status |
| `/find` | GET | Search text in files |
| `/find/file` | GET | Find files |
| `/find/symbol` | GET | Find symbols |
| `/command` | GET | List commands |
| `/agent` | GET | List agents |
| `/experimental/tool/ids` | GET | List tool IDs |
| `/experimental/tool` | GET | List tools for provider/model |
| `/mcp` | GET | Get MCP status |

### Message Parts (for POST /session/{id}/message)

```json
{
  "parts": [
    { "type": "text", "text": "Write a function to validate emails" },
    { "type": "file", "path": "/path/to/file.cs" },
    { "type": "agent", "agent": "coder" }
  ],
  "model": {
    "providerID": "anthropic",
    "modelID": "claude-sonnet-4-20250514"
  }
}
```

## User Experience

### Quick Start (30 seconds to first request)
```csharp
using OpencodeAI;

// Connect to local OpenCode server
var client = new OpencodeClient("http://localhost:9123");

// Create a session
var session = await client.CreateSessionAsync();

// Send a prompt
var response = await client.SendMessageAsync(session.Id, new TextPart
{
    Text = "Write a function to validate email addresses in C#"
});

// Print the response
foreach (var part in response.Parts)
{
    Console.WriteLine(part.Text);
}
```

### Configuration via appsettings.json
```json
{
  "Opencode": {
    "BaseUrl": "http://localhost:9123",
    "Timeout": "00:05:00",
    "DefaultProvider": "anthropic",
    "DefaultModel": "claude-sonnet-4-20250514"
  }
}
```

### Dependency Injection
```csharp
builder.Services.AddOpencodeClient(options =>
{
    options.BaseUrl = builder.Configuration["Opencode:BaseUrl"];
});
```

## Non-Functional Requirements

### Performance
- First request latency < 100ms (excluding AI response time)
- Memory-efficient streaming (no buffering entire response)
- Connection pooling via HttpClient

### Security
- Support for localhost-only connections (default OpenCode behavior)
- No sensitive data logging
- TLS support for remote connections

### Reliability
- Automatic retry for 5xx errors
- Timeout handling for long-running AI operations
- Graceful handling of aborted sessions

### Maintainability
- >80% test coverage
- XML documentation on all public members
- Changelog maintained with releases

## Constraints and Assumptions

### Constraints
- Must work without admin privileges (standard NuGet install)
- No native/unmanaged dependencies
- Package size < 500KB

### Assumptions
- OpenCode server is running locally via `opencode serve`
- API follows OpenAPI 3.1 spec at `/doc`
- Sessions persist in OpenCode's SQLite database

## Risks and Mitigations

| Risk | Impact | Mitigation |
|------|--------|------------|
| OpenCode API changes | Breaking changes for users | Version pinning, adapter pattern |
| Long AI response times | Timeouts | Configurable timeouts, streaming |
| Session state conflicts | Unexpected behavior | Session locking, clear state management |
| OpenCode not running | SDK unusable | Health check endpoint, clear errors |

## Timeline and Phases

### Phase 1: Foundation
- Project structure and build system
- Core client with session and message APIs
- Initial NuGet package (alpha)

### Phase 2: Core Features
- Complete MVP features (F1-F6)
- Comprehensive test suite
- Documentation and examples
- NuGet release (beta)

### Phase 3: Production Ready
- Streaming support (F7)
- DI integration (F8)
- File and search APIs (F9-F10)
- NuGet release (v1.0.0)

### Phase 4: Advanced Features
- Command execution (F11)
- Tool management (F12)
- Configuration API (F13)
- Telemetry (F16)
- Community feedback incorporation

## Out of Scope

- CLI tool (use official OpenCode CLI)
- GUI applications
- Running/hosting OpenCode server (use `opencode serve`)
- Local model hosting
- IDE plugins (separate projects)

## References

- OpenCode GitHub: https://github.com/opencode-ai/opencode
- OpenCode Server API: `opencode serve` → `http://localhost:{port}/doc`
- Similar SDKs for reference (local server/daemon clients):
  - Docker.DotNet: https://github.com/dotnet/Docker.DotNet - .NET client for local Docker daemon API
  - OmniSharp client libraries - communicating with local language servers
  - Elasticsearch.Net (local mode): https://github.com/elastic/elasticsearch-net - client patterns for local search servers

Note: Unlike cloud API SDKs (Stripe, Anthropic, Twilio), this SDK targets a **local headless server** running on the same machine. This means:
- No authentication/API keys required
- No rate limiting or quota management
- Single-user, single-machine usage pattern
- IPC-style communication rather than cloud API consumption
- Focus on process lifecycle awareness and lightweight local connections

## Revision History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 0.1 | 2025-12-06 | Initial | Initial PRP creation |
| 0.2 | 2025-12-08 | Updated | Complete rewrite based on actual OpenCode API from `opencode serve`. Previous version was based on hypothetical API. |
| 0.3 | 2025-12-08 | Updated | Updated reference SDKs to reflect local server client patterns (Docker.DotNet, OmniSharp, Elasticsearch) instead of cloud API SDKs (Stripe, Anthropic). |
