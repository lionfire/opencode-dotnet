---
greenlit: true
---

# Epic 03-001: OpencodeAgent Core Implementation

**Phase**: 03 - Agent Framework Integration
**Estimated Effort**: 5-6 days

## Overview

Implement OpencodeAgent class extending Microsoft Agent Framework's AIAgent abstract class with all required methods, plus convenience extension methods for fluent API access.

## Tasks

### Core Agent Implementation
- [ ] Add Microsoft.Agents.AI.Abstractions dependency
- [ ] Add Microsoft.Extensions.AI dependency
- [ ] Create OpencodeAgent class extending AIAgent
- [ ] Implement GetNewThread() → returns OpencodeAgentThread
- [ ] Implement DeserializeThread(JsonElement) → OpencodeAgentThread
- [ ] Implement RunAsync(IEnumerable<ChatMessage>, AgentThread?, AgentRunOptions?)
- [ ] Implement RunStreamingAsync() → IAsyncEnumerable<AgentRunResponseUpdate>
- [ ] Properties: Id, Name, Description, DisplayName
- [ ] GetService<T>() for service resolution
- [ ] Constructor accepting IOpenCodeClient
- [ ] Constructor accepting IOpenCodeClient + OpencodeAgentOptions

### Extension Methods (Option 4 - Enhanced Client Interfaces)
- [ ] Create OpenCodeClientExtensions static class
- [ ] Implement `AsAgent()` extension on IOpenCodeClient
  ```csharp
  public static AIAgent AsAgent(this IOpenCodeClient client, string? name = null, string? description = null)
  ```
- [ ] Implement `AsAgent(OpencodeAgentOptions)` overload
- [ ] Implement `WithMiddleware()` fluent extension for agent wrapping
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
