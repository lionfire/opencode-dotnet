---
greenlit: true
---

# Epic 01-006: Testing and Examples

**Phase**: 01 - MVP Foundation
**Status**: Implemented in v2.0
**Estimated Effort**: 3-4 days
**Priority**: High

[← Back to Phase 01](../../40-phases/01-mvp-foundation.md)

## Overview

Create comprehensive test suite (unit + integration), example projects demonstrating core functionality, and basic documentation.

## Implementation Tasks

### Test Infrastructure
- [ ] xUnit test project setup
- [ ] MockHttpMessageHandler for unit tests
- [ ] OpenCodeTestFixture for integration tests
- [ ] Test helper utilities
- [ ] CI configuration (GitHub Actions or similar)

### Unit Tests
- [ ] Achieve >70% overall code coverage
- [ ] Test all DTOs serialization/deserialization
- [ ] Test all client methods with mocked HTTP
- [ ] Test error handling paths
- [ ] Test configuration validation
- [ ] Test retry logic

### Integration Tests
- [ ] Require real OpenCode server (document setup)
- [ ] Test session lifecycle
- [ ] Test message send/receive
- [ ] Test streaming
- [ ] Test tool and file operations
- [ ] Use free/local models to avoid costs

### Example Project
- [ ] Create LionFire.OpenCode.Serve.Examples console app
- [ ] Example 1: Basic session and message
- [ ] Example 2: Streaming response
- [ ] Example 3: Session scope pattern
- [ ] Example 4: Tool approval workflow
- [ ] Example 5: File operations
- [ ] README with running instructions

### Documentation
- [ ] Project README.md with quick start
- [ ] XML docs for all public APIs
- [ ] CONTRIBUTING.md (basic for Phase 1)
- [ ] Document testing requirements

## Testing Tasks

- [ ] Run all tests in CI
- [ ] Verify examples build and run
- [ ] Code review for test quality

## Dependencies

**Depends on**: All other Phase 1 epics

## Acceptance Criteria

- [ ] >70% code coverage in unit tests
- [ ] Integration tests pass against OpenCode server
- [ ] Example project builds and runs successfully
- [ ] README provides 30-second quick start
- [ ] CI pipeline runs tests automatically
- [ ] All tests pass consistently
