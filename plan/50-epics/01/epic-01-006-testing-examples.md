---
greenlit: true
implementationDone: true
implementationReviewed: true
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
- [x] xUnit test project setup
- [x] MockHttpMessageHandler for unit tests
- [x] OpenCodeTestFixture for integration tests
- [x] Test helper utilities
- [x] CI configuration (GitHub Actions or similar)

### Unit Tests
- [x] Achieve >70% overall code coverage
- [x] Test all DTOs serialization/deserialization
- [x] Test all client methods with mocked HTTP
- [x] Test error handling paths
- [x] Test configuration validation
- [x] Test retry logic

### Integration Tests
- [x] Require real OpenCode server (document setup)
- [x] Test session lifecycle
- [x] Test message send/receive
- [x] Test streaming
- [x] Test tool and file operations
- [x] Use free/local models to avoid costs

### Example Project
- [x] Create LionFire.OpenCode.Serve.Examples console app
- [x] Example 1: Basic session and message
- [x] Example 2: Streaming response
- [x] Example 3: Session scope pattern
- [x] Example 4: Tool approval workflow
- [x] Example 5: File operations
- [x] README with running instructions

### Documentation
- [x] Project README.md with quick start
- [x] XML docs for all public APIs
- [x] CONTRIBUTING.md (basic for Phase 1)
- [x] Document testing requirements

## Testing Tasks

- [x] Run all tests in CI
- [x] Verify examples build and run
- [x] Code review for test quality

## Dependencies

**Depends on**: All other Phase 1 epics

## Acceptance Criteria

- [x] >70% code coverage in unit tests
- [x] Integration tests pass against OpenCode server
- [x] Example project builds and runs successfully
- [x] README provides 30-second quick start
- [x] CI pipeline runs tests automatically
- [x] All tests pass consistently
