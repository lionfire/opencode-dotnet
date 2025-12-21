# Change 02: Blazor Server Minimal Sample - Phases

## Overview

This document outlines the implementation phases for the Blazor Server Minimal Sample.

**Change Type**: feature
**Complexity**: moderate
**Total Phases**: 2

## Phase Summary

| Phase | Name | Goal | Epics | Status |
|-------|------|------|-------|--------|
| 01 | Implementation | Create functional Blazor Server chat sample | 4 | Completed |
| 02 | Permission Support | Enable human-in-the-middle permission handling for tool execution | 5 | Planned |

---

## Phase 01: Implementation

**Goal**: Create a minimal but functional Blazor Server chat application that demonstrates OpenCode + AG-UI integration with streaming support.

**Deliverables**:
1. AgUi.Chat.BlazorServer project with proper structure
2. Minimal chat UI using OpenCode Blazor components
3. OpenCode backend integration
4. Working streaming functionality
5. SignalR real-time updates

**Success Criteria**:
- Project builds and runs without errors
- Chat interface displays and accepts input
- Messages stream properly from backend
- Styling matches OpenCode design system
- Real-time updates via SignalR work correctly

**Dependencies**: Change 01 (Foundation - Theme System & Component Infrastructure)

---

## Epic Summary

| Epic | Title | Effort | Dependencies |
|------|-------|--------|--------------|
| 01-001 | Project Setup | 2h | None |
| 01-002 | Chat UI Implementation | 3h | 01-001 |
| 01-003 | Backend Integration | 3h | 01-002 |
| 01-004 | Testing & Polish | 2h | 01-003 |

---

## Phase 02: Permission Support

**Goal**: Enable human-in-the-middle permission handling for tool execution, allowing users to approve/deny OpenCode tool requests and see tool execution results.

**Deliverables**:
1. Permission state management in OpenCodeChatService
2. Permission event handling from SSE stream
3. MudBlazor permission dialog UI component
4. Permission response API integration
5. Tool execution status display in chat UI

**Success Criteria**:
- Users can ask tool-based questions (e.g., "list files")
- Permission dialogs appear for tool requests
- Users can approve or deny permissions
- Tool execution results appear in chat
- Multiple permissions handled correctly (queue)
- "Remember this decision" option works
- Tool status visible in chat UI

**Dependencies**: Phase 01 (completed)

**Details**: See [02-permission-support.md](02-permission-support.md)

---

## Epic Summary

### Phase 01 Epics

| Epic | Title | Effort | Dependencies |
|------|-------|--------|--------------|
| 01-001 | Project Setup | 2h | None |
| 01-002 | Chat UI Implementation | 3h | 01-001 |
| 01-003 | Backend Integration | 3h | 01-002 |
| 01-004 | Testing & Polish | 2h | 01-003 |

### Phase 02 Epics

| Epic | Title | Effort | Dependencies |
|------|-------|--------|--------------|
| 02-001 | Permission State Management | 2h | Phase 01 |
| 02-002 | Permission Event Handling | 2h | 02-001 |
| 02-003 | Permission UI Component | 2h | 02-002 |
| 02-004 | Permission API Integration | 1.5h | 02-003 |
| 02-005 | Tool Status Display | 2h | 02-004 |
