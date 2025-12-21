---
greenlit: true
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

- [ ] Planning complete
- [ ] Design approved
- [ ] Development in progress
- [ ] Code review complete
- [ ] Testing complete
- [ ] Documentation complete

## Technical Requirements

### API Endpoints to Implement
- [ ] POST /session - Create session
- [ ] GET /session/{id} - Get session details
- [ ] GET /session - List all sessions
- [ ] DELETE /session/{id} - Delete session
- [ ] POST /session/{id}/fork - Fork session at message
- [ ] POST /session/{id}/abort - Abort running session
- [ ] POST /session/{id}/share - Share session (generate token)
- [ ] DELETE /session/{id}/share - Unshare session (revoke token)

### Request/Response Models
- [ ] CreateSessionRequest (directory?, provider?, model?, systemPrompt?)
- [ ] SessionListResponse (sessions: Session[])
- [ ] ForkSessionRequest (messageId: string)
- [ ] ShareSessionResponse (sharedToken: string, shareUrl: string)

### Session Scope Pattern
- [ ] Create `SessionScope` class implementing IAsyncDisposable
- [ ] Constructor accepts IOpencodeClient and Session
- [ ] Dispose calls DeleteSessionAsync automatically
- [ ] Extension method CreateSessionScope() on IOpencodeClient

## Implementation Tasks

### 1. Session CRUD Operations
- [ ] Implement CreateSessionAsync(CreateSessionRequest?, CancellationToken)
  - [ ] POST to /session with optional request body
  - [ ] Return Session DTO
  - [ ] Handle creation errors (server full, invalid config, etc.)
- [ ] Implement GetSessionAsync(string id, CancellationToken)
  - [ ] GET from /session/{id}
  - [ ] Return Session or throw NotFoundException
- [ ] Implement ListSessionsAsync(CancellationToken)
  - [ ] GET from /session
  - [ ] Return IReadOnlyList<Session>
  - [ ] Handle empty list case
- [ ] Implement DeleteSessionAsync(string id, CancellationToken)
  - [ ] DELETE to /session/{id}
  - [ ] Handle 404 (session not found)
  - [ ] Handle 409 (session still running)

### 2. Advanced Session Operations
- [ ] Implement ForkSessionAsync(string sessionId, string messageId, CancellationToken)
  - [ ] POST to /session/{sessionId}/fork with ForkSessionRequest
  - [ ] Return new Session (forked)
  - [ ] Validate messageId exists in original session
- [ ] Implement AbortSessionAsync(string sessionId, CancellationToken)
  - [ ] POST to /session/{sessionId}/abort
  - [ ] Handle session not running case
  - [ ] Return confirmation or throw
- [ ] Implement ShareSessionAsync(string sessionId, CancellationToken)
  - [ ] POST to /session/{sessionId}/share
  - [ ] Return ShareSessionResponse with token and URL
- [ ] Implement UnshareSessionAsync(string sessionId, CancellationToken)
  - [ ] DELETE to /session/{sessionId}/share
  - [ ] Revoke shared token
  - [ ] Handle not-shared case

### 3. Session Scope Implementation
- [ ] Create `SessionScope.cs` class
  - [ ] Constructor(IOpencodeClient client, Session session)
  - [ ] Public Session property (get-only)
  - [ ] Implement IAsyncDisposable
  - [ ] DisposeAsync calls client.DeleteSessionAsync(session.Id)
  - [ ] Handle disposal errors gracefully (log but don't throw)
- [ ] Create extension method CreateSessionScope() in OpenCodeClientExtensions
  - [ ] Calls CreateSessionAsync
  - [ ] Wraps result in SessionScope
  - [ ] Returns ValueTask<SessionScope>

### 4. Error Handling
- [ ] Map HTTP status codes to exceptions:
  - [ ] 404 → OpencodeNotFoundException("Session not found")
  - [ ] 409 → OpencodeConflictException("Session already aborted" or "Session still running")
  - [ ] 500/503 → OpencodeServerException("Server error")
- [ ] Include session ID in exception messages
- [ ] Add troubleshooting hints in error messages

## Testing Tasks

### Unit Tests
- [ ] Test CreateSessionAsync with mock HttpClient
  - [ ] Success case returns Session
  - [ ] Optional parameters sent correctly
  - [ ] Handles server errors
- [ ] Test GetSessionAsync
  - [ ] Success case returns Session
  - [ ] 404 throws OpencodeNotFoundException
- [ ] Test ListSessionsAsync
  - [ ] Returns list of sessions
  - [ ] Handles empty list
- [ ] Test DeleteSessionAsync
  - [ ] Success case completes
  - [ ] 404 handled appropriately
- [ ] Test ForkSessionAsync
  - [ ] Returns new forked session
  - [ ] Validates parameters
- [ ] Test AbortSessionAsync
  - [ ] Aborts running session
  - [ ] Handles not-running case
- [ ] Test ShareSessionAsync/UnshareSessionAsync
  - [ ] Share returns token
  - [ ] Unshare revokes token
- [ ] Test SessionScope disposal
  - [ ] Disposes session correctly
  - [ ] Handles errors during disposal

### Integration Tests
- [ ] Test full session lifecycle against real server
  - [ ] Create session
  - [ ] Get session details
  - [ ] Delete session
- [ ] Test session scope pattern
  - [ ] using/await using disposes correctly
  - [ ] Session deleted after scope
- [ ] Test fork, abort, share operations
  - [ ] Fork creates new session
  - [ ] Abort stops running session
  - [ ] Share generates valid token

## Documentation Tasks

- [ ] XML docs for all session methods
- [ ] Code example for basic session usage
- [ ] Code example for session scope pattern
- [ ] Document error cases and how to handle

## Dependencies & Blockers

**Depends on**: Epic 01-001 (Core Client Infrastructure)

## Acceptance Criteria

- [ ] All 8 session API endpoints implemented
- [ ] SessionScope class with IAsyncDisposable works
- [ ] CreateSessionScope() extension method functional
- [ ] All unit tests pass (>80% coverage)
- [ ] Integration tests pass against real server
- [ ] Error handling provides clear messages
- [ ] XML documentation complete

## Notes

- Session IDs are strings (not GUIDs necessarily)
- Consider rate limiting for session creation (future phase)
- SessionScope should log disposal errors but not throw (prevent crashes in finalizers)
- Shared tokens are security-sensitive - document carefully
