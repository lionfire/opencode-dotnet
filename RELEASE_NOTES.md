# Release Notes

## Version 1.0.0 (TBD)

Initial release of the LionFire.OpenCode.Serve SDK.

### Features

#### LionFire.OpenCode.Serve

- **Full OpenCode API Coverage**: Support for all 60+ API endpoints
  - Session management (create, fork, share, revert)
  - Message operations (prompt, list, stream)
  - File operations (list, read, status)
  - PTY operations (create, connect, manage)
  - MCP server integration
  - Configuration management
  - Provider management

- **Modern .NET Design**
  - Async/await throughout
  - `IAsyncEnumerable` for streaming responses
  - Nullable reference types enabled
  - Records for immutable models

- **Dependency Injection**
  - `IOpenCodeClient` interface for testability
  - `AddOpenCodeClient()` extension method
  - Full `IHttpClientFactory` integration

- **Configuration**
  - `OpenCodeClientOptions` for customization
  - Options validation on startup
  - Environment variable support

- **Error Handling**
  - Hierarchical exception types
  - `TroubleshootingHint` on all exceptions
  - Proper HTTP status code mapping

- **Observability**
  - OpenTelemetry tracing support
  - Microsoft.Extensions.Logging integration

- **AOT Compatible**
  - Source-generated JSON serialization
  - No runtime reflection required

#### LionFire.OpenCode.Serve.AgentFramework

- **Microsoft.Extensions.AI Integration**
  - `IChatClient` implementation via `OpenCodeChatClient`
  - Compatible with Semantic Kernel and other AI frameworks
  - Message format conversion utilities

- **Dependency Injection**
  - `AddOpenCodeChatClient()` extension method
  - `AddKeyedOpenCodeChatClient()` for multiple clients

### Breaking Changes

None (initial release).

### Known Issues

- PTY WebSocket connections not yet implemented (placeholder throws `NotImplementedException`)
- True streaming requires event subscription (SSE-based, not direct streaming)

### Dependencies

- .NET 8.0 or later
- Microsoft.Extensions.Http 8.0.0
- Microsoft.Extensions.Logging.Abstractions 8.0.0
- Microsoft.Extensions.Options 8.0.0
- Microsoft.Extensions.AI.Abstractions 9.x (AgentFramework only)

### Migration Guide

For users coming from direct HTTP calls to the OpenCode API, see the [Migration Guide](docs/migration-guide.md).

---

## Future Versions

### Planned for 1.1.0

- PTY WebSocket support
- Enhanced streaming with delta events
- Connection pooling improvements
- Additional configuration options

### Planned for 2.0.0

- Breaking API changes based on community feedback
- Enhanced error handling
- Additional AI framework integrations
