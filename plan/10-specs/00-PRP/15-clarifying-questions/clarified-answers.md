# Clarified Answers

This document contains the final answers to all clarifying questions from all rounds.

## SDK Generation and Architecture

### Q: Should the SDK generation leverage the OpenAPI 3.1 spec available at `/doc` for automated code generation, or should we hand-craft all models and client methods for better control and .NET idioms?

**A**: Hand-craft the SDK with strong .NET idioms and conventions. While we'll reference the OpenAPI spec for accuracy, we'll prioritize developer experience over automation. This allows us to:
- Use proper .NET naming conventions (PascalCase, async suffixes)
- Leverage C# 12 features (records, required properties)
- Provide better XML documentation
- Create intuitive method overloads
- Ensure consistent patterns across all endpoints

## Server Health and Detection

### Q: How should the SDK handle detection of whether the OpenCode server is running? Should there be an explicit health check endpoint, or should we rely on catching connection errors on first use?

**A**: Implement a `PingAsync()` or `GetHealthAsync()` method that attempts a lightweight request (like `GET /config` or `GET /session`) to verify the server is responsive. Provide clear exception messages when connection fails with helpful hints (e.g., "OpenCode server not responding at http://localhost:9123. Is `opencode serve` running?").

## Streaming API Design

### Q: For streaming responses (SSE from OpenCode), should we expose `IAsyncEnumerable<MessagePart>` for incremental updates, or provide callback/event-based patterns, or both?

**A**: Provide `IAsyncEnumerable<MessageUpdate>` as the primary API for streaming, with optional extension methods for event-based patterns. IAsyncEnumerable is idiomatic for modern .NET and works well with `await foreach`. For developers who prefer events, we can provide a `.Subscribe()` extension method or wrapper.

## Session Lifecycle Management

### Q: Should the SDK provide high-level abstractions for session lifecycle (e.g., `using` statement with IDisposable that auto-deletes sessions), or keep it low-level where developers explicitly manage sessions?

**A**: Provide both. Low-level API for explicit control (`CreateSessionAsync`, `DeleteSessionAsync`), plus optional higher-level helpers:
```csharp
// High-level: auto-cleanup
await using var session = await client.CreateSessionScope();
// session is automatically deleted when disposed

// Low-level: explicit control
var session = await client.CreateSessionAsync();
// ... use session ...
await client.DeleteSessionAsync(session.Id);
```

## Error Handling and Exception Types

### Q: How granular should exception types be? Should we have specific exceptions for different HTTP status codes (404, 500, 503) or API-specific errors (SessionNotFound, SessionAlreadyAborted)?

**A**: Create a hierarchy of specific exceptions:
- `OpencodeException` (base)
  - `OpencodeApiException` (API returned error response)
    - `OpencodeNotFoundException` (404 - session/message not found)
    - `OpencodeConflictException` (409 - operation conflict)
    - `OpencodeServerException` (5xx - server errors)
  - `OpencodeConnectionException` (cannot reach server)
  - `OpencodeTimeoutException` (request timeout)

This allows developers to catch specific errors while still having a base type for general error handling.

## Request Timeout Configuration

### Q: What should the default timeout be for API requests, considering some operations (AI responses) can take 30+ seconds?

**A**: Use different defaults for different operation types:
- Quick operations (list, get, delete): 30 seconds
- Message sending (AI responses): 5 minutes (300 seconds)
- Streaming: No timeout (rely on SSE keep-alive)
- All configurable via `OpencodeClientOptions`

Provide clear timeout exceptions with guidance on increasing timeout for slow operations.

## Framework Targeting

### Q: Should we multi-target to support older .NET versions (net6.0, net7.0) or strictly target .NET 8+ as stated in the PRP?

**A**: Target .NET 8+ only as stated in PRP. This is a new SDK for a new tool, so we can start with modern .NET. This allows us to:
- Use C# 12 features freely
- Leverage latest BCL improvements
- Avoid #ifdef complexity
- Keep package size smaller

If community demand exists for older frameworks, we can add them later.

## Configuration Sources and Priority

### Q: When multiple configuration sources exist (constructor args, IOptions, appsettings.json), what should the priority order be?

**A**: Keep the SDK simple - don't read environment variables directly. Let the host handle configuration sources:

Priority order (highest to lowest):
1. Explicit constructor/method parameters (most specific)
2. IOptions<OpencodeClientOptions> from DI (however the host configured it - appsettings, env vars, etc.)
3. Default values (http://localhost:9123)

The SDK should not have hidden environment variable lookups. If users want env vars, they wire it up in their host:
```csharp
// User explicitly binds config (which may include env vars via AddEnvironmentVariables())
builder.Services.AddOpencodeClient(builder.Configuration.GetSection("OpenCode"));

// Or user explicitly reads env var themselves
builder.Services.AddOpencodeClient(options =>
{
    options.BaseUrl = Environment.GetEnvironmentVariable("OPENCODE_URL")
        ?? "http://localhost:9123";
});
```

This keeps the SDK simpler, more testable, and follows the principle that the SDK shouldn't have opinions about *where* configuration comes from - that's the host's responsibility.

## Package and Namespace Naming

### Q: Should the NuGet package be named `OpencodeAI`, `Opencode.DotNet`, `OpenCode.NET`, or something else?

**A**: `LionFire.OpenCode.Serve` as the package name. This:
- `LionFire` prefix makes it clear this is a community/third-party SDK, not official
- `OpenCode` identifies the product it integrates with
- `Serve` suffix explicitly indicates this is for the `opencode serve` local headless server API, not a cloud/internet API
- Creates clear namespace: `using LionFire.OpenCode.Serve;`

Main client class: `OpenCodeClient` (clean, ergonomic - namespace already disambiguates)

```csharp
using LionFire.OpenCode.Serve;

var client = new OpenCodeClient("http://localhost:9123");
```

If a hypothetical internet/cloud API SDK is ever needed, it could be `OpenCodeApiClient` or live in a different namespace.

## Testing Strategy

### Q: For integration tests, should we require a real OpenCode server running, use mocked HTTP responses, or provide a test server/fixture?

**A**: Hybrid approach:
- Unit tests: Mock HttpClient with MockHttpMessageHandler (fast, no dependencies)
- Integration tests: Require real OpenCode server with clear setup docs
- Provide `OpenCodeTestServer` fixture that automatically starts/stops server if installed
- CI pipeline runs both with containerized OpenCode instance

**Important cost consideration**: Integration tests should use free or local AI providers/models to avoid burning money during test runs. Document recommended test configurations:
- Local models (e.g., Ollama with small models)
- Free tier providers where available
- Mock/stub responses for most tests, real AI only for smoke tests
