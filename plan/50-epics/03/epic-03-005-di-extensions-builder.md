---
greenlit: true
---

# Epic 03-005: DI Extensions and Builder Pattern

**Phase**: 03 - Agent Framework Integration
**Estimated Effort**: 3-4 days

## Overview

Create dependency injection extensions and fluent builder for OpencodeAgent configuration, including unified registration that configures both client and agent in a single call.

## Tasks

### Core DI Extensions
- [ ] Create ServiceCollectionExtensions for Agent Framework
- [ ] AddOpencodeAgent(IServiceCollection) extension
- [ ] AddOpencodeAgentFactory() for named agents
- [ ] Integration with existing AddOpenCodeClient()
- [ ] IOptions<OpencodeAgentOptions> support

### Unified Registration (Option 4 - Enhanced Client Interfaces)
- [ ] Implement `AddOpenCodeClientAsAgent()` - single registration for both:
  ```csharp
  services.AddOpenCodeClientAsAgent(
      clientOptions => { clientOptions.BaseUrl = "http://localhost:9123"; },
      agentOptions => {
          agentOptions.Name = "CodeExpert";
          agentOptions.EnableTelemetry = true;
      });
  ```
- [ ] Register IOpenCodeClient as singleton
- [ ] Register AIAgent as singleton (using AsAgent() internally)
- [ ] Both resolvable from same container:
  ```csharp
  var client = sp.GetRequiredService<IOpenCodeClient>();  // Direct client
  var agent = sp.GetRequiredService<AIAgent>();           // Agent wrapper
  ```

### Builder Pattern
- [ ] Create OpencodeAgentBuilder fluent API
- [ ] Configure agent options via builder
- [ ] Add middleware registration to builder:
  ```csharp
  services.AddOpenCodeClientAsAgent(...)
      .WithMiddleware<TelemetryMiddleware>()
      .WithMiddleware<CostTrackingMiddleware>();
  ```
- [ ] Support keyed services for multiple agents

### Testing
- [ ] Test DI registration patterns
- [ ] Test unified registration resolves both interfaces
- [ ] Test middleware registration via DI
- [ ] Test keyed services for named agents

## Acceptance Criteria

- AddOpencodeAgent() works
- AddOpenCodeClientAsAgent() registers both client and agent
- IOptions configuration supported
- Builder pattern functional
- Middleware registrable via DI
- Both IOpenCodeClient and AIAgent resolvable from same registration
- ASP.NET Core example works
