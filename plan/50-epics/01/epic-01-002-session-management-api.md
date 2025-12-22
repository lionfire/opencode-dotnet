---
greenlit: true
implementationDone: true
implementationReviewed: true
---

# Epic 01-002: Session Management API

**Phase**: 01 - MVP Foundation
**Status**: Implemented in v2.0
**Estimated Effort**: 3-4 days
**Priority**: High (required for core functionality)

[← Back to Phase 01](../../40-phases/01-mvp-foundation.md)

## Overview

Implement all session management endpoints of the OpenCode API including create, get, list, delete, fork, abort, and share operations. Also implement the session scope pattern with IAsyncDisposable for automatic cleanup.

## Motivation

Sessions are the fundamental unit of interaction with OpenCode. All conversations happen within sessions, so robust session management is essential for the SDK to be useful.

## Status Overview

- [x] Planning complete
- [x] Design approved
- [x] Development in progress
- [x] Code review complete
- [x] Testing complete
- [x] Documentation complete

## Technical Requirements

### API Endpoints to Implement
- [x] POST /session - Create session
- [x] GET /session/{id} - Get session details
- [x] GET /session - List all sessions
- [x] DELETE /session/{id} - Delete session
- [x] POST /session/{id}/fork - Fork session at message
- [x] POST /session/{id}/abort - Abort running session
- [x] POST /session/{id}/share - Share session (generate token)
- [x] DELETE /session/{id}/share - Unshare session (revoke token)

### Request/Response Models
- [x] CreateSessionRequest (directory?, provider?, model?, systemPrompt?)
- [x] SessionListResponse (sessions: Session[])
- [x] ForkSessionRequest (messageId: string)
- [x] ShareSessionResponse (sharedToken: string, shareUrl: string)

### Session Scope Pattern
- [x] Create `SessionScope` class implementing IAsyncDisposable
- [x] Constructor accepts IOpencodeClient and Session
- [x] Dispose calls DeleteSessionAsync automatically
- [x] Extension method CreateSessionScope() on IOpencodeClient

## Implementation Tasks

### 1. Session CRUD Operations
- [x] Implement CreateSessionAsync(CreateSessionRequest?, CancellationToken)
  - [x] POST to /session with optional request body
  - [x] Return Session DTO
  - [x] Handle creation errors (server full, invalid config, etc.)
- [x] Implement GetSessionAsync(string id, CancellationToken)
  - [x] GET from /session/{id}
  - [x] Return Session or throw NotFoundException
- [x] Implement ListSessionsAsync(CancellationToken)
  - [x] GET from /session
  - [x] Return IReadOnlyList<Session>
  - [x] Handle empty list case
- [x] Implement DeleteSessionAsync(string id, CancellationToken)
  - [x] DELETE to /session/{id}
  - [x] Handle 404 (session not found)
  - [x] Handle 409 (session still running)

### 2. Advanced Session Operations
- [x] Implement ForkSessionAsync(string sessionId, string messageId, CancellationToken)
  - [x] POST to /session/{sessionId}/fork with ForkSessionRequest
  - [x] Return new Session (forked)
  - [x] Validate messageId exists in original session
- [x] Implement AbortSessionAsync(string sessionId, CancellationToken)
  - [x] POST to /session/{sessionId}/abort
  - [x] Handle session not running case
  - [x] Return confirmation or throw
- [x] Implement ShareSessionAsync(string sessionId, CancellationToken)
  - [x] POST to /session/{sessionId}/share
  - [x] Return ShareSessionResponse with token and URL
- [x] Implement UnshareSessionAsync(string sessionId, CancellationToken)
  - [x] DELETE to /session/{sessionId}/share
  - [x] Revoke shared token
  - [x] Handle not-shared case

### 3. Session Scope Implementation
- [x] Create `SessionScope.cs` class
  - [x] Constructor(IOpencodeClient client, Session session)
  - [x] Public Session property (get-only)
  - [x] Implement IAsyncDisposable
  - [x] DisposeAsync calls client.DeleteSessionAsync(session.Id)
  - [x] Handle disposal errors gracefully (log but don't throw)
- [x] Create extension method CreateSessionScope() in OpenCodeClientExtensions
  - [x] Calls CreateSessionAsync
  - [x] Wraps result in SessionScope
  - [x] Returns ValueTask<SessionScope>

### 4. Error Handling
- [x] Map HTTP status codes to exceptions:
  - [x] 404 → OpencodeNotFoundException("Session not found")
  - [x] 409 → OpencodeConflictException("Session already aborted" or "Session still running")
  - [x] 500/503 → OpencodeServerException("Server error")
- [x] Include session ID in exception messages
- [x] Add troubleshooting hints in error messages

## Testing Tasks

### Unit Tests
- [x] Test CreateSessionAsync with mock HttpClient
  - [x] Success case returns Session
  - [x] Optional parameters sent correctly
  - [x] Handles server errors
- [x] Test GetSessionAsync
  - [x] Success case returns Session
  - [x] 404 throws OpencodeNotFoundException
- [x] Test ListSessionsAsync
  - [x] Returns list of sessions
  - [x] Handles empty list
- [x] Test DeleteSessionAsync
  - [x] Success case completes
  - [x] 404 handled appropriately
- [x] Test ForkSessionAsync
  - [x] Returns new forked session
  - [x] Validates parameters
- [x] Test AbortSessionAsync
  - [x] Aborts running session
  - [x] Handles not-running case
- [x] Test ShareSessionAsync/UnshareSessionAsync
  - [x] Share returns token
  - [x] Unshare revokes token
- [x] Test SessionScope disposal
  - [x] Disposes session correctly
  - [x] Handles errors during disposal

### Integration Tests
- [x] Test full session lifecycle against real server
  - [x] Create session
  - [x] Get session details
  - [x] Delete session
- [x] Test session scope pattern
  - [x] using/await using disposes correctly
  - [x] Session deleted after scope
- [x] Test fork, abort, share operations
  - [x] Fork creates new session
  - [x] Abort stops running session
  - [x] Share generates valid token

## Documentation Tasks

- [x] XML docs for all session methods
- [x] Code example for basic session usage
- [x] Code example for session scope pattern
- [x] Document error cases and how to handle

## Dependencies & Blockers

**Depends on**: Epic 01-001 (Core Client Infrastructure)

## Acceptance Criteria

- [x] All 8 session API endpoints implemented
- [x] SessionScope class with IAsyncDisposable works
- [x] CreateSessionScope() extension method functional
- [x] All unit tests pass (>80% coverage)
- [x] Integration tests pass against real server
- [x] Error handling provides clear messages
- [x] XML documentation complete

## Notes

- Session IDs are strings (not GUIDs necessarily)
- Consider rate limiting for session creation (future phase)
- SessionScope should log disposal errors but not throw (prevent crashes in finalizers)
- Shared tokens are security-sensitive - document carefully
