---
greenlit: true
implementationDone: true
implementationReviewed: true
---

# Epic 03-001: OpencodeAgent Core Implementation

**Phase**: 03 - Agent Framework Integration
**Estimated Effort**: 5-6 days

## Overview

Implement OpencodeAgent class extending Microsoft Agent Framework's AIAgent abstract class with all required methods, plus convenience extension methods for fluent API access.

## Tasks

### Core Agent Implementation
- [x] Add Microsoft.Agents.AI.Abstractions dependency
- [x] Add Microsoft.Extensions.AI dependency
- [x] Create OpencodeAgent class extending AIAgent
- [x] Implement GetNewThread() → returns OpencodeAgentThread
- [x] Implement DeserializeThread(JsonElement) → OpencodeAgentThread
- [x] Implement RunAsync(IEnumerable<ChatMessage>, AgentThread?, AgentRunOptions?)
- [x] Implement RunStreamingAsync() → IAsyncEnumerable<AgentRunResponseUpdate>
- [x] Properties: Id, Name, Description, DisplayName
- [x] GetService<T>() for service resolution
- [x] Constructor accepting IOpenCodeClient
- [x] Constructor accepting IOpenCodeClient + OpencodeAgentOptions

### Extension Methods (Option 4 - Enhanced Client Interfaces)
- [x] Create OpenCodeClientExtensions static class
- [x] Implement `AsAgent()` extension on IOpenCodeClient
  ```csharp
  public static AIAgent AsAgent(this IOpenCodeClient client, string? name = null, string? description = null)
  ```
- [x] Implement `AsAgent(OpencodeAgentOptions)` overload
- [x] Implement `WithMiddleware()` fluent extension for agent wrapping
  ```csharp
  var agent = client.AsAgent().WithMiddleware(new TelemetryMiddleware());
  ```

## Acceptance Criteria

- OpencodeAgent extends AIAgent
- All abstract methods implemented
- Constructs with IOpenCodeClient
- Properties configurable via options
- `client.AsAgent()` extension method works
- Middleware can be added fluently
- Unit tests pass
