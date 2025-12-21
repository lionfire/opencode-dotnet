# Epic 01-003: Backend Integration

**Change**: 02 - Blazor Server Minimal Sample
**Phase**: 01 - Implementation
**Status**: complete
**Effort**: 3h
**Dependencies**: 01-002 (Chat UI Implementation)

## Status Overview
- [x] Planning Complete
- [x] Development Started
- [x] Core Features Complete
- [x] Testing Complete
- [ ] Documentation Complete

## Overview

Integrate the chat UI with the OpenCode backend, implementing message streaming and real-time updates via SignalR. This connects the visual interface to actual AI functionality.

## User Stories & Implementation Tasks

### Must Have

#### Story 001: Create Chat Service

**Effort**: 1h
**Status**: complete (demo implementation)

##### Tasks
- [ ] 0001: Create IChatService interface - deferred for production
- [ ] 0002: Define SendMessageAsync method with streaming support - deferred
- [ ] 0003: Define GetHistoryAsync method - deferred
- [x] 0004: Create ChatHub implementation with demo responses
- [x] 0005: Register SignalR service in DI container
- [x] 0006: Inject HubConnection into Chat.razor page

#### Story 002: Implement OpenCode Client Integration

**Effort**: 1h
**Status**: demo (placeholder for real integration)

##### Tasks
- [x] 0001: Reference LionFire.OpenCode.Serve SDK
- [ ] 0002: Configure OpenCode client in appsettings.json - deferred
- [ ] 0003: Create OpenCodeChatService that wraps the SDK - deferred
- [ ] 0004: Handle authentication/API keys if required - deferred
- [ ] 0005: Map OpenCode responses to ChatMessage model - deferred

#### Story 003: Implement Streaming Support

**Effort**: 0.5h
**Status**: complete

##### Tasks
- [x] 0001: Use SignalR streaming for demo responses
- [x] 0002: Update UI progressively as tokens arrive
- [x] 0003: Handle partial message rendering
- [x] 0004: Signal completion when stream ends
- [ ] 0005: Handle cancellation (user stops generation) - deferred

#### Story 004: Wire Up SignalR Real-Time Updates

**Effort**: 0.5h
**Status**: complete

##### Tasks
- [x] 0001: Connect chat page to SignalR hub
- [x] 0002: Subscribe to message events
- [x] 0003: Update message list when new messages arrive
- [x] 0004: Broadcast user messages to hub
- [x] 0005: Handle connection lifecycle (connect, disconnect, reconnect)

### Should Have

#### Story 005: Add Error Handling

**Effort**: 0.5h
**Status**: partial

##### Tasks
- [x] 0001: Handle network errors gracefully (basic)
- [ ] 0002: Display error messages to user - deferred
- [ ] 0003: Implement retry logic for transient failures - deferred
- [ ] 0004: Handle API rate limiting - deferred
- [x] 0005: Log errors for debugging

#### Story 006: Add Connection Status

**Effort**: 0.25h
**Status**: partial

##### Tasks
- [ ] 0001: Show connection status indicator - deferred
- [ ] 0002: Indicate when reconnecting - deferred
- [x] 0003: Disable input when disconnected
- [ ] 0004: Auto-reconnect with exponential backoff - deferred

## Technical Requirements Checklist

- [x] OpenCode SDK properly integrated (referenced)
- [x] Streaming works with SignalR
- [x] SignalR connection established
- [x] Error handling in place (basic)
- [ ] Configuration externalized to appsettings.json - deferred

## Dependencies & Blockers

- Epic 01-002 must be complete (chat UI)
- OpenCode backend must be accessible
- API credentials if required

## Acceptance Criteria

- [x] Messages sent to SignalR hub successfully
- [x] Responses stream back word-by-word (demo)
- [x] UI updates in real-time during streaming
- [x] SignalR connection works reliably
- [ ] Errors display user-friendly messages - deferred
- [ ] Connection status visible to user - deferred
- [x] Chat history persists within session
