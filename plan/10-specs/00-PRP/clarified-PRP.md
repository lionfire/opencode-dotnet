# opencode-dotnet - Clarified Product Requirements Prompt

**Version**: 2.0 (Clarified)
**Date**: 2025-12-09
**Based On**: Original PRP.md + Clarifying Questions + Feature Analysis + Agent Framework Integration Spec

## Overview

opencode-dotnet is a community-maintained .NET SDK for OpenCode, the open-source terminal-first AI coding agent. The SDK provides a fully-typed, async-first, production-ready client for the **OpenCode Server API** (`opencode serve`), enabling .NET developers to programmatically interact with OpenCode sessions and integrate AI-powered coding assistance into their applications.

**Key Distinction**: This SDK targets a **local headless server** (similar to Docker.DotNet), not a cloud API. It communicates with `opencode serve` running on localhost, with no authentication complexity, rate limiting, or multi-tenant concerns.

## Problem Statement

.NET developers who want to programmatically interact with OpenCode lack a native, production-ready SDK. While OpenCode provides a powerful HTTP API via `opencode serve`, there's no official .NET client library to abstract raw HTTP calls into strongly-typed, idiomatic .NET APIs. This forces developers to manually implement HTTP clients, reinventing the wheel with error-prone, untyped approaches.

The opencode-dotnet SDK eliminates this gap, providing the definitive .NET client for OpenCode with excellent developer experience, production-ready reliability, and comprehensive feature coverage.

## Target Audiences

### Primary Audiences

1. **.NET Application Developers** (Initial Completeness: 70% → Projected: 92%)
   - Building applications that need AI-powered coding assistance
   - Integrating OpenCode into existing .NET solutions
   - Creating developer tools, IDEs, or productivity applications
   - **Key Needs**: Strong typing, IntelliSense, error handling, examples, logging

2. **.NET Library/SDK Authors** (Initial Completeness: 80% → Projected: 95%)
   - Building higher-level abstractions on top of OpenCode
   - Creating domain-specific tooling with AI capabilities
   - Developing Visual Studio or VS Code extensions
   - **Key Needs**: Interface-based design, extensibility, AOT support, minimal dependencies

3. **Enterprise Development Teams** (Initial Completeness: 65% → Projected: 88%)
   - Automating code generation workflows
   - Building internal developer platforms
   - Creating CI/CD integrations
   - **Key Needs**: Observability, resilience, configuration validation, security

### Secondary Audiences

4. **Open Source Contributors** (Initial Completeness: 75% → Projected: 85%)
   - Contributing to the SDK itself
   - Extending functionality for specific use cases
   - **Key Needs**: Clean architecture, test infrastructure, contribution guide

5. **Educators and Researchers** (Initial Completeness: 70% → Projected: 80%)
   - Teaching AI-assisted development
   - Researching code generation patterns
   - **Key Needs**: Simple API, session management, examples, reproducibility

## Core Requirements

### Functional Requirements

#### Package and Naming (Clarified)

- **Package Name**: `LionFire.OpenCode.Serve`
  - `LionFire` prefix indicates community/third-party SDK
  - `Serve` suffix explicitly indicates local server API (not cloud)
- **Namespace**: `LionFire.OpenCode.Serve`
- **Main Client Class**: `OpenCodeClient`
- **Interface**: `IOpencodeClient` for testability and mocking

#### SDK Implementation Approach (Clarified)

- **Hand-crafted SDK** with strong .NET idioms (not auto-generated)
- Reference OpenAPI spec for accuracy but prioritize developer experience
- Use proper .NET naming conventions (PascalCase, async suffixes)
- Leverage C# 12 features (records, required properties, primary constructors)
- Create intuitive method overloads for common patterns

#### Client Configuration

- **Base URL**: Default `http://localhost:9123`, configurable
- **Directory Parameter**: Optional for multi-project support
- **Timeout Configuration** (Clarified):
  - Quick operations (list, get, delete): 30 seconds default
  - Message sending (AI responses): 5 minutes default
  - Streaming: No timeout (rely on SSE keep-alive)
  - All configurable via `OpencodeClientOptions`
- **HttpClient Injection**: Support IHttpClientFactory for connection pooling
- **Retry Policy**: Exponential backoff for transient failures (Must-Have)

#### Configuration Priority (Clarified)

Priority order (highest to lowest):
1. Explicit constructor/method parameters (most specific)
2. `IOptions<OpencodeClientOptions>` from DI (configured by host)
3. Default values (`http://localhost:9123`)

**No hidden environment variable lookups** - SDK doesn't read env vars directly. Host handles configuration sources (appsettings, env vars, etc.).

#### Health Check and Server Detection (Clarified - Must-Have)

- **Method**: `PingAsync()` or `GetHealthAsync()` for lightweight server check
- **Clear Error Messages**: When connection fails, provide helpful hints:
  - "OpenCode server not responding at http://localhost:9123. Is `opencode serve` running?"
- **Process Lifecycle Awareness**: SDK detects if server isn't running with actionable guidance

#### Session Management API

- **Low-Level API** (explicit control):
  - `CreateSessionAsync()` - Create a new session
  - `GetSessionAsync(id)` - Get session details
  - `ListSessionsAsync()` - List all sessions
  - `DeleteSessionAsync(id)` - Delete a session
  - `ForkSessionAsync(id, messageId)` - Fork at specific message
  - `AbortSessionAsync(id)` - Abort running session
  - `ShareSessionAsync(id)` / `UnshareSessionAsync(id)` - Share/unshare

- **High-Level API** (Clarified - Must-Have):
  - `CreateSessionScope()` → `IAsyncDisposable` with auto-cleanup
  - Session automatically deleted when disposed
  - Example:
    ```csharp
    await using var session = await client.CreateSessionScope();
    // session auto-deleted on disposal
    ```

#### Message/Prompt API

- `SendMessageAsync(sessionId, parts)` - Send message to session
- `GetMessagesAsync(sessionId)` - Get all messages
- `GetMessageAsync(sessionId, messageId)` - Get specific message
- **Multi-part message support**:
  - `TextPart` - Text content
  - `FilePart` - File references
  - `AgentPart` - Agent-specific content

#### Streaming API (Clarified)

- **Primary API**: `IAsyncEnumerable<MessageUpdate>` for incremental updates
  - Idiomatic for modern .NET
  - Works with `await foreach`
- **Optional Extension**: `.Subscribe()` extension method for event-based patterns
- **Memory-Efficient**: No buffering of entire responses
- **SSE Implementation**: Handle Server-Sent Events correctly

#### Tool Management API

- `GetToolsAsync()` - List available tools
- `GetToolAsync(id)` - Get tool details
- `ApproveToolAsync(sessionId, toolId)` - Approve tool use
- `UpdateToolPermissionsAsync(sessionId, toolId, permissions)` - Update permissions

#### File Operations API

- `ListFilesAsync(sessionId, path)` - List files in directory
- `GetFileContentAsync(sessionId, path)` - Get file content
- `SearchFilesAsync(sessionId, query)` - Search files
- `ApplyChangesAsync(sessionId, changes)` - Apply file modifications

#### Command API

- `ExecuteCommandAsync(sessionId, command)` - Execute slash command
- Support for all OpenCode slash commands

#### Configuration API

- `GetConfigAsync()` - Get OpenCode configuration
- `GetProvidersAsync()` - List AI providers
- `GetModelsAsync()` - List available models

### Error Handling (Clarified - Must-Have)

**Exception Hierarchy**:
```
OpencodeException (base)
├── OpencodeApiException (API returned error response)
│   ├── OpencodeNotFoundException (404 - session/message not found)
│   ├── OpencodeConflictException (409 - operation conflict)
│   └── OpencodeServerException (5xx - server errors)
├── OpencodeConnectionException (cannot reach server)
└── OpencodeTimeoutException (request timeout)
```

**Error Message Quality** (Must-Have):
- Detailed error messages with troubleshooting hints
- Actionable guidance (e.g., "Is opencode serve running?", "Try increasing timeout")
- Include request ID for support correlation
- Structured logging integration

### Microsoft Agent Framework Integration (Major Feature)

**Status**: Existing spec at `/src/opencode-dotnet/plan/10-specs/01-agent-framework-integration/`

**Purpose**: Enable OpencodeAI to participate in Microsoft Agent Framework workflows as a first-class AI agent.

**Key Components**:
- `OpencodeAgent` class extending `AIAgent` abstract base class
- `OpencodeAgentThread` class extending `AgentThread` for conversation state
- Bidirectional message conversion between Agent Framework and OpencodeAI formats
- Thread serialization/deserialization for persistence
- Streaming support via `IAsyncEnumerable<AgentRunResponseUpdate>`
- Dependency injection extensions for registration
- Tool integration between OpencodeAI and Agent Framework
- Middleware support for cross-cutting concerns

**Scope**:
- Native Agent Framework participation in all workflow patterns
- Full feature parity with native OpencodeAI capabilities
- Streaming performance within 5% of native
- Thread state persistence and restoration
- Telemetry and observability integration

**Benefits**:
- Enables multi-agent workflows combining OpencodeAI with other AI providers
- Seamless integration with Agent Framework middleware and orchestration
- Consistent programming model across different AI agents
- Enterprise-ready with telemetry, logging, and resilience patterns

### Non-Functional Requirements

#### Performance

- Message conversion: < 1ms per message
- Thread serialization: < 10ms for 100 messages
- Streaming latency: < 5ms additional per chunk
- Memory: < 10% overhead vs raw HTTP calls
- First token latency: < 50ms overhead

#### Scalability

- Support concurrent operations (limited by OpenCode rate limits)
- Thread-safe operations for multi-threaded environments
- Efficient memory usage for long-running conversations
- Optional client-side rate limiting

#### Security

- **No Authentication Complexity**: Localhost-only pattern (no API keys)
- Bind to localhost only (OpenCode default)
- Input validation on deserialization
- No PII logging by default
- Secure thread serialization

#### Reliability

- Graceful handling of API errors
- Proper cleanup on cancellation
- Thread state consistency on partial failures
- Retry policy with exponential backoff
- Circuit breaker for repeated failures (Should-Have)

#### Testing Strategy (Clarified - Hybrid Approach)

**Unit Tests**:
- Mock HttpClient with MockHttpMessageHandler
- Fast, no dependencies
- Target >80% code coverage

**Integration Tests**:
- Require real OpenCode server with clear setup docs
- Provide `OpenCodeTestServer` fixture (auto-start/stop if installed)
- **Use free or local AI models** to avoid costs:
  - Local models (Ollama with small models)
  - Free tier providers where available
  - Mock/stub responses for most tests
  - Real AI only for smoke tests

**CI/CD**:
- Run both unit and integration tests
- Containerized OpenCode instance for CI

## Extended Features

### High Priority (Must-Have Inferred Features)

1. **Robust Connection Health Check** (already covered above)
2. **Detailed Error Messages with Troubleshooting Hints** (already covered above)
3. **Example Projects and Templates**
   - Basic session usage
   - Streaming responses
   - DI integration
   - ASP.NET Core usage
   - Agent Framework integration
4. **HttpClient Factory Integration** (already covered above)
5. **Session Scope with Automatic Cleanup** (already covered above)
6. **Retry Policy with Exponential Backoff** (already covered above)
7. **Structured Logging Integration** (ILogger for diagnostics)

### Medium Priority (Should-Have Features)

1. **Source-Generated JSON Serialization** - Enable AOT, improve performance
2. **Polly Integration for Resilience** - Circuit breaker, timeout policies
3. **OpenTelemetry Integration** - Traces, metrics for observability
4. **Configuration Validation** - Fail-fast with clear errors on startup
5. **Message History Pagination** - Efficient retrieval for large sessions
6. **Session Query and Filtering** - Filter by date, status, tags
7. **Streaming Progress Callbacks** - Alternative to IAsyncEnumerable for UI frameworks
8. **Timeout Per-Operation Override** - Method-level timeout control

### Nice-to-Have Features

1. **Session Templates** - Pre-configured for common workflows
2. **Batch Operations** - Multiple messages/sessions efficiently
3. **Session Export/Import** - JSON/Markdown for backup/migration
4. **Semantic Versioning Detection** - Adapt to OpenCode API changes
5. **Message Search** - Find previous conversations/code
6. **Rate Limiting (Local Throttling)** - Protect against runaway loops
7. **Streaming Reconnection** - Auto-reconnect if connection drops
8. **Response Caching** - Cache idempotent operations

### Future Considerations

1. **WebSocket Support** - Alternative transport for bidirectional communication
2. **Model Selection Helpers** - Choose models by capabilities/cost
3. **Token Usage Tracking** - Cost analysis per session/message
4. **Response Streaming with Partial Parsing** - Extract code blocks incrementally
5. **Code Execution Sandbox** - Safely execute AI-generated code
6. **Multi-Instance Support** - Manage multiple OpenCode servers
7. **Diff and Merge Helpers** - Apply code changes from AI responses
8. **Session Analytics** - Success rates, token usage, response times

## Known Challenges

### High Effort Challenges

1. **Streaming Implementation (SSE)** - Proper SSE handling in .NET requires careful HttpClient configuration
2. **Production Hardening** - Comprehensive resilience, observability, error handling for enterprise use
3. **Testing Infrastructure** - Reliable CI with OpenCode server, cost-effective test strategies
4. **API Stability** - OpenCode is evolving, need version adaptation strategy
5. **Agent Framework Integration** - Complex message conversion, streaming coordination, thread management

### Medium Effort Challenges

1. **Error Message Quality** - Crafting helpful, actionable messages for all scenarios
2. **Session Lifecycle** - Ensuring proper cleanup to avoid leaks
3. **Timeout Handling** - Different operations need different timeouts
4. **Performance Optimization** - Source-generated JSON, efficient streaming, minimal allocations
5. **Documentation** - Enterprise deployment guides, troubleshooting runbooks

## Success Criteria

### Technical Success
- Published to NuGet with stable semantic versioning (SemVer)
- >80% code coverage in automated tests
- Zero critical security vulnerabilities
- Package size < 500KB
- Full XML documentation coverage for public APIs
- Source-generated JSON for AOT compatibility
- All benchmarks met (conversion <1ms, serialization <10ms, streaming <5ms overhead)

### Developer Experience Success
- 30 seconds from NuGet install to first successful API call
- IntelliSense provides useful information for all public APIs
- Clear error messages guide developers to solutions
- Working examples cover all common use cases
- Comprehensive README with quick start guide

### Community Success
- Becomes the de facto .NET SDK for OpenCode
- Active community contributions and feedback
- Referenced in OpenCode documentation as official .NET client
- Used in production by enterprise development teams
- 100+ downloads in first month
- Zero critical bugs in first 30 days

## Target Framework

- **.NET 8+ only** (Clarified)
- No multi-targeting for older frameworks
- Allows use of C# 12 features, latest BCL improvements
- Avoids #ifdef complexity, keeps package smaller
- If community demand exists, can add older frameworks later

## Out of Scope

### Explicitly Not Included

1. **Cloud/Internet API Client** - This SDK is for local `opencode serve` only
2. **OpenCode Installation/Process Management** - SDK assumes server is running
3. **GUI or Visualization Components** - SDK is headless library only
4. **OpenCode API Modifications** - Uses existing API, doesn't change OpenCode
5. **Multi-Agent Orchestration Implementation** - Uses Agent Framework's existing capabilities
6. **New OpenCode Features** - SDK wraps existing API, doesn't add new features

### Deferred to Later Versions

1. **Advanced Resilience Patterns** (Circuit Breaker, Bulkhead) - MVP has basic retry
2. **OpenTelemetry Integration** - Structured logging in MVP, telemetry in Phase 2
3. **Session Templates and Batch Operations** - Nice-to-have, not MVP
4. **WebSocket Transport** - SSE sufficient for MVP
5. **Multi-Instance Support** - Single instance sufficient for MVP

## Dependencies

### Core Dependencies (Minimal)

- **System.Text.Json** - JSON serialization (built-in, no external package)
- **System.Net.Http** - HTTP client (built-in)
- **Microsoft.Extensions.Options** - Configuration pattern (lightweight)
- **Microsoft.Extensions.Logging.Abstractions** - Logging interface (lightweight)

### Optional Dependencies (For Specific Features)

- **Microsoft.Extensions.DependencyInjection.Abstractions** - DI extensions (optional)
- **Microsoft.Extensions.Http** - IHttpClientFactory support (should-have)
- **Polly** - Resilience policies (should-have)
- **Microsoft.Agents.AI.Abstractions** - Agent Framework integration (separate package)
- **Microsoft.Extensions.AI** - Common AI abstractions (Agent Framework)
- **OpenTelemetry packages** - Observability (future)

## Unique Value Proposition

Unlike cloud API SDKs (Stripe, Anthropic, Twilio), opencode-dotnet targets a **local headless server** pattern similar to Docker.DotNet or OmniSharp clients:

1. **No Authentication Complexity** - No API keys, OAuth, or token management
2. **No Rate Limiting** - Localhost connections don't need quota management
3. **Process Lifecycle Focus** - Handles scenarios where server isn't running
4. **Single-User Pattern** - Single-machine, single-user usage (not multi-tenant)
5. **IPC-Style Communication** - Lightweight, efficient localhost patterns
6. **Session Persistence** - Leverages OpenCode's SQLite-persisted sessions

The SDK makes it as easy to use OpenCode from .NET as it is to use Docker from .NET with Docker.DotNet.

## References

### Related Documents

- [Original PRP](./PRP.md) - Initial product requirements
- [Intent Analysis](./intent.md) - Analyzed product goals
- [Clarifying Questions](./15-clarifying-questions/round-01.md) - Design decisions
- [Target Audience Assessment](../../20-scoping/target-audiences-assessment.md) - Completeness analysis
- [Inferred Features](../../20-scoping/inferred-features.md) - Additional features identified
- [Agent Framework Integration Spec](../01-agent-framework-integration/spec.md) - Complete Agent Framework integration

### External References

- OpenCode: https://github.com/opencode-ai/opencode
- OpenCode Server API: `opencode serve --port 9123` → `/doc` endpoint
- Docker.DotNet (similar pattern): https://github.com/dotnet/Docker.DotNet
- Microsoft Agent Framework: https://learn.microsoft.com/agent-framework/
- Microsoft.Extensions.AI: https://www.nuget.org/packages/Microsoft.Extensions.AI

## Revision History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0 | 2025-12-06 | Team | Initial PRP |
| 2.0 | 2025-12-09 | Team | Clarified PRP incorporating: clarifying questions (10 decisions), target audience assessment (73%→90%), inferred features (35 total), Agent Framework integration spec, testing strategy, configuration priorities |
