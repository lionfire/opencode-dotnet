---
greenlit: true
implementationDone: true
implementationReviewed: true
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
- [x] Create OpencodeException base class
- [x] OpencodeApiException (API errors with status code)
- [x] OpencodeNotFoundException (404)
- [x] OpencodeConflictException (409)
- [x] OpencodeServerException (5xx)
- [x] OpencodeConnectionException (can't reach server)
- [x] OpencodeTimeoutException (request timeout)

### Error Message Quality
- [x] Include troubleshooting hints in messages
- [x] "Is opencode serve running?" for connection errors
- [x] "Try increasing timeout" for timeout errors
- [x] Include request ID when available
- [x] Structured error data in exception properties

### Health Check API
- [x] Implement PingAsync() method
- [x] Try GET /config or GET /session as lightweight check
- [x] Return bool or throw with clear message
- [x] Include server version in success response

### Retry Logic
- [x] Implement retry policy with exponential backoff
- [x] Configurable max attempts (default: 3)
- [x] Configurable base delay (default: 2s)
- [x] Retry on transient failures (5xx, timeouts)
- [x] Don't retry on client errors (4xx except 429)
- [x] Log retry attempts

### Logging Integration
- [x] ILogger integration via constructor
- [x] Log HTTP requests at Debug level
- [x] Log HTTP responses at Debug level
- [x] Log errors at Error level with full exception
- [x] Log retries at Warning level
- [x] Structured logging with correlation IDs
- [x] Don't log sensitive data (tokens, file content)

## Testing Tasks

- [x] Test all exception types thrown correctly
- [x] Test error message quality
- [x] Test retry logic (mock failing then succeeding)
- [x] Test health check with server running/not running
- [x] Test logging output

## Dependencies

**Depends on**: Epic 01-001

## Acceptance Criteria

- [x] Complete exception hierarchy
- [x] Health check works reliably
- [x] Retry logic configurable and functional
- [x] Error messages include helpful hints
- [x] Logging doesn't expose sensitive data
- [x] >85% code coverage for error paths
