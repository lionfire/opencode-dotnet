# Epic 03-007: IDE Main Layout Integration

**Phase**: 03 - Layout & Integration
**Status**: Complete
**Priority**: High

## Overview

Compose all components into the main IDE layout and establish component communication.

## Stories / Tasks

### Layout Composition

- [ ] **Task 1**: Create Ide.razor page
  - @page "/ide" route
  - Use IdeLayout for structure
  - Inject all required services

- [ ] **Task 2**: Integrate FileTree into left panel
  - Size file tree container
  - Handle file selection events
  - Show selected file indicator

- [ ] **Task 3**: Integrate Chat into center panel
  - Chat as primary center content
  - Tab system for multiple chats (optional)
  - Maximize/restore functionality

- [ ] **Task 4**: Integrate DiffViewer into right panel
  - Show diffs from assistant responses
  - File change preview
  - Accept/reject controls (optional)

- [ ] **Task 5**: Integrate Terminal into bottom panel
  - Command output display
  - Build/run output
  - Resizable height

### Component Communication

- [ ] **Task 6**: Set up state management
  - Create IdeState service (scoped)
  - Selected file path
  - Active panel states
  - Current session info

- [ ] **Task 7**: Implement cross-component events
  - File selected -> show in chat context
  - Chat response -> show diffs
  - Command executed -> show in terminal

- [ ] **Task 8**: Add panel coordination
  - Clicking file shows it in context
  - Diffs can be opened in diff viewer
  - Terminal shows relevant output

## Acceptance Criteria

- [ ] IDE page displays all four pane areas
- [ ] Components communicate properly
- [ ] State is shared correctly
- [ ] Layout is responsive and resizable
- [ ] Navigation between files works
- [ ] Overall UX feels cohesive

## Dependencies

- All Phase 02 epics must be complete

## Notes

Consider using:
- Cascading parameters for state
- Event callbacks for communication
- MediatR for complex event routing (optional)
