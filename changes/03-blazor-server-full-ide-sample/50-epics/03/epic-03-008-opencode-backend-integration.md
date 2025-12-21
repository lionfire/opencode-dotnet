# Epic 03-008: OpenCode Backend Integration

**Phase**: 03 - Layout & Integration
**Status**: Complete
**Priority**: High

## Overview

Connect all IDE components to the OpenCode backend for real functionality.

## Stories / Tasks

### Session Management

- [ ] **Task 1**: Implement session lifecycle
  - Create session on IDE load
  - Maintain session across components
  - Handle session expiry/reconnect

- [ ] **Task 2**: Create OpenCodeIdeService
  - Wraps IOpenCodeClient
  - Provides IDE-specific methods
  - Manages session state

### File Operations

- [ ] **Task 3**: Connect file tree to backend
  - Fetch directory listings via OpenCode API
  - Handle file/folder operations
  - Real-time updates (optional)

- [ ] **Task 4**: Implement file content loading
  - Load file on selection
  - Show in appropriate viewer
  - Handle large files gracefully

### Chat Integration

- [ ] **Task 5**: Connect chat to OpenCode session
  - Use existing IChatService pattern
  - Send messages with file context
  - Stream responses

- [ ] **Task 6**: Parse tool calls from responses
  - Detect file modifications
  - Show pending changes
  - Apply/reject functionality

### Real-time Updates

- [ ] **Task 7**: Subscribe to backend events
  - Use OpenCode event subscription
  - Update UI on file changes
  - Update terminal on command output

- [ ] **Task 8**: Handle connection state
  - Show connected/disconnected status
  - Reconnect on connection loss
  - Queue messages during disconnect

## Acceptance Criteria

- [ ] Session persists across IDE usage
- [ ] File tree shows real project structure
- [ ] Chat sends messages to OpenCode
- [ ] Tool calls are detected and displayed
- [ ] Real-time updates work
- [ ] Connection status is visible

## Dependencies

- Epic 03-007 must be complete
- LionFire.OpenCode.Serve client library

## Notes

Reference:
- `/src/opencode-dotnet/src/LionFire.OpenCode.Serve/` for client
- `/src/opencode-dotnet/samples/AgUi.Chat.BlazorServer/` for patterns
