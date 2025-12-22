---
greenlit: true
implementationDone: true
implementationReviewed: true
---

# Epic 03-006: Testing and Examples

**Phase**: 03 - Agent Framework Integration
**Estimated Effort**: 4-5 days

## Overview

Comprehensive testing and example projects demonstrating Agent Framework integration, including concrete middleware implementations from Option 4.

## Tasks

### Unit Tests
- [x] Unit tests for OpencodeAgent (mock client)
- [x] Unit tests for message conversion
- [x] Unit tests for thread management
- [x] Unit tests for `AsAgent()` extension method
- [x] Unit tests for session/thread conversion helpers
- [x] Unit tests for unified DI registration

### Integration Tests
- [x] Integration tests with Agent Framework
- [x] Test all workflow patterns (sequential, parallel, conditional)
- [x] Test middleware pipeline execution order
- [x] Test thread persistence across multiple runs

### Middleware Examples (Option 4 - Enterprise Patterns)
- [x] Create `TelemetryMiddleware` example:
  - OpenTelemetry Activity tracking
  - Request/response logging
  - Duration metrics
- [x] Create `CostTrackingMiddleware` example:
  - Token usage tracking
  - Cumulative cost calculation
  - Budget limit enforcement with `BudgetExceededException`
- [x] Create `ApprovalMiddleware` / `CodeApprovalMiddleware` example:
  - Human-in-the-loop approval for generated code
  - Configurable approval triggers (e.g., dangerous operations)
  - `ApprovalRejectedException` when rejected
- [x] Create `ContentFilterMiddleware` example:
  - Input/output content filtering
  - Blocked content detection
  - `ContentFilterException` for policy violations

### Workflow Examples
- [x] Example: Sequential agents (OpenCode + OpenAI)
- [x] Example: Parallel agents (multiple OpenCode instances)
- [x] Example: Multi-turn conversation with thread persistence
- [x] Example: Using `AsAgent()` extension method
- [x] Example: Unified DI registration with middleware:
  ```csharp
  services.AddOpenCodeClientAsAgent(clientOpts, agentOpts)
      .WithMiddleware<TelemetryMiddleware>()
      .WithMiddleware<CostTrackingMiddleware>(opts => opts.MaxBudget = 100m)
      .WithMiddleware<ApprovalMiddleware>();
  ```

### Documentation
- [x] Agent Framework integration guide
- [x] Middleware authoring guide
- [x] Extension methods reference
- [x] Migration guide from direct client usage

## Acceptance Criteria

- >80% test coverage
- All Agent Framework patterns work
- All middleware examples compile and work
- Examples build and run
- Documentation complete
- Integration tests pass
- Middleware can be composed in any order
