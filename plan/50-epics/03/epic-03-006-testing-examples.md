---
greenlit: true
---

# Epic 03-006: Testing and Examples

**Phase**: 03 - Agent Framework Integration
**Estimated Effort**: 4-5 days

## Overview

Comprehensive testing and example projects demonstrating Agent Framework integration, including concrete middleware implementations from Option 4.

## Tasks

### Unit Tests
- [ ] Unit tests for OpencodeAgent (mock client)
- [ ] Unit tests for message conversion
- [ ] Unit tests for thread management
- [ ] Unit tests for `AsAgent()` extension method
- [ ] Unit tests for session/thread conversion helpers
- [ ] Unit tests for unified DI registration

### Integration Tests
- [ ] Integration tests with Agent Framework
- [ ] Test all workflow patterns (sequential, parallel, conditional)
- [ ] Test middleware pipeline execution order
- [ ] Test thread persistence across multiple runs

### Middleware Examples (Option 4 - Enterprise Patterns)
- [ ] Create `TelemetryMiddleware` example:
  - OpenTelemetry Activity tracking
  - Request/response logging
  - Duration metrics
- [ ] Create `CostTrackingMiddleware` example:
  - Token usage tracking
  - Cumulative cost calculation
  - Budget limit enforcement with `BudgetExceededException`
- [ ] Create `ApprovalMiddleware` / `CodeApprovalMiddleware` example:
  - Human-in-the-loop approval for generated code
  - Configurable approval triggers (e.g., dangerous operations)
  - `ApprovalRejectedException` when rejected
- [ ] Create `ContentFilterMiddleware` example:
  - Input/output content filtering
  - Blocked content detection
  - `ContentFilterException` for policy violations

### Workflow Examples
- [ ] Example: Sequential agents (OpenCode + OpenAI)
- [ ] Example: Parallel agents (multiple OpenCode instances)
- [ ] Example: Multi-turn conversation with thread persistence
- [ ] Example: Using `AsAgent()` extension method
- [ ] Example: Unified DI registration with middleware:
  ```csharp
  services.AddOpenCodeClientAsAgent(clientOpts, agentOpts)
      .WithMiddleware<TelemetryMiddleware>()
      .WithMiddleware<CostTrackingMiddleware>(opts => opts.MaxBudget = 100m)
      .WithMiddleware<ApprovalMiddleware>();
  ```

### Documentation
- [ ] Agent Framework integration guide
- [ ] Middleware authoring guide
- [ ] Extension methods reference
- [ ] Migration guide from direct client usage

## Acceptance Criteria

- >80% test coverage
- All Agent Framework patterns work
- All middleware examples compile and work
- Examples build and run
- Documentation complete
- Integration tests pass
- Middleware can be composed in any order
