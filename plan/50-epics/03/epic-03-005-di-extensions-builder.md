---
greenlit: true
implementationDone: true
implementationReviewed: true
---

# Epic 03-005: DI Extensions and Builder Pattern

**Phase**: 03 - Agent Framework Integration
**Estimated Effort**: 3-4 days

## Overview

Create dependency injection extensions and fluent builder for OpencodeAgent configuration, including unified registration that configures both client and agent in a single call.

## Tasks

### Core DI Extensions
- [x] Create ServiceCollectionExtensions for Agent Framework
- [x] AddOpencodeAgent(IServiceCollection) extension
- [x] AddOpencodeAgentFactory() for named agents
- [x] Integration with existing AddOpenCodeClient()
- [x] IOptions<OpencodeAgentOptions> support

### Unified Registration (Option 4 - Enhanced Client Interfaces)
- [x] Implement `AddOpenCodeClientAsAgent()` - single registration for both:
  ```csharp
  services.AddOpenCodeClientAsAgent(
      clientOptions => { clientOptions.BaseUrl = "http://localhost:9123"; },
      agentOptions => {
          agentOptions.Name = "CodeExpert";
          agentOptions.EnableTelemetry = true;
      });
  ```
- [x] Register IOpenCodeClient as singleton
- [x] Register AIAgent as singleton (using AsAgent() internally)
- [x] Both resolvable from same container:
  ```csharp
  var client = sp.GetRequiredService<IOpenCodeClient>();  // Direct client
  var agent = sp.GetRequiredService<AIAgent>();           // Agent wrapper
  ```

### Builder Pattern
- [x] Create OpencodeAgentBuilder fluent API
- [x] Configure agent options via builder
- [x] Add middleware registration to builder:
  ```csharp
  services.AddOpenCodeClientAsAgent(...)
      .WithMiddleware<TelemetryMiddleware>()
      .WithMiddleware<CostTrackingMiddleware>();
  ```
- [x] Support keyed services for multiple agents

### Testing
- [x] Test DI registration patterns
- [x] Test unified registration resolves both interfaces
- [x] Test middleware registration via DI
- [x] Test keyed services for named agents

## Acceptance Criteria

- AddOpencodeAgent() works
- AddOpenCodeClientAsAgent() registers both client and agent
- IOptions configuration supported
- Builder pattern functional
- Middleware registrable via DI
- Both IOpenCodeClient and AIAgent resolvable from same registration
- ASP.NET Core example works
