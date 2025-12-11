# Clarifying Questions - Round 1

Note: if you wish to accept the proposed answer, you can leave it as is and the proposed answer will be assumed to be your answer.

## Question 1: OpenAPI Spec Integration

**Question**: Should the SDK generation leverage the OpenAPI 3.1 spec available at `/doc` for automated code generation, or should we hand-craft all models and client methods for better control and .NET idioms?

**Why this matters**: This decision affects the development approach, maintainability, and how we handle API evolution. OpenAPI generators can automate DTO creation but may produce non-idiomatic .NET code. Hand-crafted code gives full control but requires manual updates when the API changes.

**Proposed answer**: Hand-craft the SDK with strong .NET idioms and conventions. While we'll reference the OpenAPI spec for accuracy, we'll prioritize developer experience over automation. This allows us to:
- Use proper .NET naming conventions (PascalCase, async suffixes)
- Leverage C# 12 features (records, required properties)
- Provide better XML documentation
- Create intuitive method overloads
- Ensure consistent patterns across all endpoints

**Alternatives to consider**:
- Option A: Use NSwag or OpenAPI Generator for initial scaffolding, then heavily customize
- Option B: Full automated generation with post-processing scripts
- Option C: Hybrid - generate DTOs but hand-craft client interface

---

## Question 2: Health Check and Server Detection

**Question**: How should the SDK handle detection of whether the OpenCode server is running? Should there be an explicit health check endpoint, or should we rely on catching connection errors on first use?

**Why this matters**: This affects the user experience when the server isn't running. A good error message early can save debugging time.

**Proposed answer**: Implement a `PingAsync()` or `GetHealthAsync()` method that attempts a lightweight request (like `GET /config` or `GET /session`) to verify the server is responsive. Provide clear exception messages when connection fails with helpful hints (e.g., "OpenCode server not responding at http://localhost:9123. Is `opencode serve` running?").

**Alternatives to consider**:
- Option A: Lazy connection - only error when first real API call is made
- Option B: Eager validation - validate connection in constructor/factory method
- Option C: Add a dedicated `/health` endpoint to OpenCode API (requires upstream change)

---

## Question 3: Streaming Implementation Strategy

**Question**: For streaming responses (SSE from OpenCode), should we expose `IAsyncEnumerable<MessagePart>` for incremental updates, or provide callback/event-based patterns, or both?

**Why this matters**: This affects how developers consume streaming responses and impacts the API's ergonomics for different scenarios (async enumeration vs. reactive patterns).

**Proposed answer**: Provide `IAsyncEnumerable<MessageUpdate>` as the primary API for streaming, with optional extension methods for event-based patterns. IAsyncEnumerable is idiomatic for modern .NET and works well with `await foreach`. For developers who prefer events, we can provide a `.Subscribe()` extension method or wrapper.

**Alternatives to consider**:
- Option A: Event-based only (e.g., `client.MessageReceived += handler`)
- Option B: IObservable<T> (Rx.NET pattern)
- Option C: Callback-based (e.g., `SendMessageAsync(onUpdate: part => { })`)

---

## Question 4: Session Lifecycle Management

**Question**: Should the SDK provide high-level abstractions for session lifecycle (e.g., `using` statement with IDisposable that auto-deletes sessions), or keep it low-level where developers explicitly manage sessions?

**Why this matters**: This affects API complexity and developer experience. High-level abstractions are convenient but may hide important details. Low-level control gives flexibility but requires more boilerplate.

**Proposed answer**: Provide both. Low-level API for explicit control (`CreateSessionAsync`, `DeleteSessionAsync`), plus optional higher-level helpers:
```csharp
// High-level: auto-cleanup
await using var session = await client.CreateSessionScope();
// session is automatically deleted when disposed

// Low-level: explicit control
var session = await client.CreateSessionAsync();
// ... use session ...
await client.DeleteSessionAsync(session.Id);
```

**Alternatives to consider**:
- Option A: Low-level only - keep SDK minimal
- Option B: High-level only - always auto-manage sessions
- Option C: Separate "SessionScope" class that wraps the client

---

## Question 5: Error Categorization

**Question**: How granular should exception types be? Should we have specific exceptions for different HTTP status codes (404, 500, 503) or API-specific errors (SessionNotFound, SessionAlreadyAborted)?

**Why this matters**: This affects how developers handle errors and whether they can differentiate between different failure modes programmatically.

**Proposed answer**: Create a hierarchy of specific exceptions:
- `OpencodeException` (base)
  - `OpencodeApiException` (API returned error response)
    - `OpencodeNotFoundException` (404 - session/message not found)
    - `OpencodeConflictException` (409 - operation conflict)
    - `OpencodeServerException` (5xx - server errors)
  - `OpencodeConnectionException` (cannot reach server)
  - `OpencodeTimeoutException` (request timeout)

This allows developers to catch specific errors while still having a base type for general error handling.

**Alternatives to consider**:
- Option A: Single `OpencodeException` with error codes/properties
- Option B: Mirror HTTP status codes exactly (401, 403, 404, 500, etc.)
- Option C: Domain-specific only (SessionNotFound, MessageNotFound, etc.)

---

## Question 6: Default Timeout Values

**Question**: What should the default timeout be for API requests, considering some operations (AI responses) can take 30+ seconds?

**Why this matters**: Too short and legitimate requests will fail; too long and hung connections waste resources. The right default improves out-of-box experience.

**Proposed answer**: Use different defaults for different operation types:
- Quick operations (list, get, delete): 30 seconds
- Message sending (AI responses): 5 minutes (300 seconds)
- Streaming: No timeout (rely on SSE keep-alive)
- All configurable via `OpencodeClientOptions`

Provide clear timeout exceptions with guidance on increasing timeout for slow operations.

**Alternatives to consider**:
- Option A: Single default (e.g., 2 minutes for everything)
- Option B: No default timeout (infinite unless specified)
- Option C: Adaptive timeout based on historical response times

---

## Question 7: Multi-Target Framework Support

**Question**: Should we multi-target to support older .NET versions (net6.0, net7.0) or strictly target .NET 8+ as stated in the PRP?

**Why this matters**: Multi-targeting increases compatibility but adds complexity and limits use of newer language features. .NET 6 is still in support until November 2024, and .NET 7 until May 2024.

**Proposed answer**: Target .NET 8+ only as stated in PRP. This is a new SDK for a new tool, so we can start with modern .NET. This allows us to:
- Use C# 12 features freely
- Leverage latest BCL improvements
- Avoid #ifdef complexity
- Keep package size smaller

If community demand exists for older frameworks, we can add them later.

**Alternatives to consider**:
- Option A: Multi-target net6.0, net7.0, net8.0
- Option B: Target .NET Standard 2.1 for maximum compatibility
- Option C: Target net8.0 and net9.0 (when released)

---

## Question 8: Configuration Provider Priority

**Question**: When multiple configuration sources exist (constructor args, IOptions, appsettings.json), what should the priority order be?

**Why this matters**: Developers need predictable configuration behavior, especially when troubleshooting why a setting isn't taking effect.

**Proposed answer**: Keep the SDK simple - don't read environment variables directly. Let the host handle configuration sources:

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

**Alternatives to consider**:
- Option A: Only support one configuration method to avoid confusion
- Option B: SDK reads environment variables directly (more magic, less explicit)
- Option C: Merge all sources instead of priority override

---

## Question 9: Package Naming Convention

**Question**: Should the NuGet package be named `OpencodeAI`, `Opencode.DotNet`, `OpenCode.NET`, or something else?

**Why this matters**: Package name affects discoverability, clarity, and namespace conventions. Once published, changing it is disruptive.

**Proposed answer**: `LionFire.OpenCode.Serve` as the package name. This:
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

**Alternatives to consider**:
- Option A: `LionFire.OpenCode` (shorter, but less explicit about local server)
- Option B: `LionFire.OpenCode.Local` (alternative suffix)
- Option C: `OpenCodeServeClient` class name (more explicit but longer)

---

## Question 10: Testing Strategy - Real Server Dependency

**Question**: For integration tests, should we require a real OpenCode server running, use mocked HTTP responses, or provide a test server/fixture?

**Why this matters**: This affects test reliability, speed, and contributor experience. Real server tests are most accurate but require setup. Mocked tests are fast but may miss real-world issues.

**Proposed answer**: Hybrid approach:
- Unit tests: Mock HttpClient with MockHttpMessageHandler (fast, no dependencies)
- Integration tests: Require real OpenCode server with clear setup docs
- Provide `OpenCodeTestServer` fixture that automatically starts/stops server if installed
- CI pipeline runs both with containerized OpenCode instance

**Important cost consideration**: Integration tests should use free or local AI providers/models to avoid burning money during test runs. Document recommended test configurations:
- Local models (e.g., Ollama with small models)
- Free tier providers where available
- Mock/stub responses for most tests, real AI only for smoke tests

**Alternatives to consider**:
- Option A: Unit tests only with mocked responses
- Option B: Integration tests only against real server
- Option C: Create in-memory mock server that simulates OpenCode API
