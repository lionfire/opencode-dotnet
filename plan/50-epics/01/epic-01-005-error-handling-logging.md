---
greenlit: true
---

# Epic 01-005: Error Handling and Logging

**Phase**: 01 - MVP Foundation
**Status**: Implemented in v2.0
**Estimated Effort**: 3-4 days
**Priority**: High

[← Back to Phase 01](../../40-phases/01-mvp-foundation.md)

## Overview

Implement comprehensive exception hierarchy, error message formatting with troubleshooting hints, retry logic with exponential backoff, health check API, and structured logging integration.

## Implementation Tasks

### Exception Hierarchy
- [ ] Create OpencodeException base class
- [ ] OpencodeApiException (API errors with status code)
- [ ] OpencodeNotFoundException (404)
- [ ] OpencodeConflictException (409)
- [ ] OpencodeServerException (5xx)
- [ ] OpencodeConnectionException (can't reach server)
- [ ] OpencodeTimeoutException (request timeout)

### Error Message Quality
- [ ] Include troubleshooting hints in messages
- [ ] "Is opencode serve running?" for connection errors
- [ ] "Try increasing timeout" for timeout errors
- [ ] Include request ID when available
- [ ] Structured error data in exception properties

### Health Check API
- [ ] Implement PingAsync() method
- [ ] Try GET /config or GET /session as lightweight check
- [ ] Return bool or throw with clear message
- [ ] Include server version in success response

### Retry Logic
- [ ] Implement retry policy with exponential backoff
- [ ] Configurable max attempts (default: 3)
- [ ] Configurable base delay (default: 2s)
- [ ] Retry on transient failures (5xx, timeouts)
- [ ] Don't retry on client errors (4xx except 429)
- [ ] Log retry attempts

### Logging Integration
- [ ] ILogger integration via constructor
- [ ] Log HTTP requests at Debug level
- [ ] Log HTTP responses at Debug level
- [ ] Log errors at Error level with full exception
- [ ] Log retries at Warning level
- [ ] Structured logging with correlation IDs
- [ ] Don't log sensitive data (tokens, file content)

## Testing Tasks

- [ ] Test all exception types thrown correctly
- [ ] Test error message quality
- [ ] Test retry logic (mock failing then succeeding)
- [ ] Test health check with server running/not running
- [ ] Test logging output

## Dependencies

**Depends on**: Epic 01-001

## Acceptance Criteria

- [ ] Complete exception hierarchy
- [ ] Health check works reliably
- [ ] Retry logic configurable and functional
- [ ] Error messages include helpful hints
- [ ] Logging doesn't expose sensitive data
- [ ] >85% code coverage for error paths
