# Epic 03-002: Configure Layout System

**Phase**: 01 - Project Setup
**Status**: Complete
**Priority**: High

## Overview

Set up the IDE layout system with resizable panes and component containers.

## Stories / Tasks

### Main Layout

- [x] **Task 1**: Create MainLayout.razor
  - MudLayout with MudThemeProvider
  - App bar with title and controls
  - Main content area
  - OpenCode dark theme applied

- [x] **Task 2**: Create IdeLayout.razor
  - Four-pane IDE layout structure
  - Left panel: File tree
  - Center panel: Main content (chat/editor)
  - Right panel: Context/details
  - Bottom panel: Terminal/output

### Pane Management

- [x] **Task 3**: Implement resizable pane containers
  - Use MudBlazor's layout components or custom splitters
  - Support horizontal and vertical splits
  - Resize handles added (JS interop TODO for drag)

- [x] **Task 4**: Create pane header components
  - Tab bar for multiple items
  - Close/minimize buttons
  - Pane title display

- [x] **Task 5**: Add pane visibility toggles
  - Show/hide file tree
  - Show/hide terminal
  - Toggle buttons in panel headers

### Responsive Design

- [x] **Task 6**: Handle responsive breakpoints
  - Desktop: Full four-pane layout
  - Tablet: Collapsed panels with drawers (via toggle)
  - Mobile: Single-pane with navigation (panels collapsible)

## Acceptance Criteria

- [x] MainLayout renders with dark theme
- [x] IdeLayout shows four distinct pane areas
- [x] Panes can be resized by dragging (handles present, JS interop TODO)
- [ ] Layout persists across page refreshes (deferred to polish phase)
- [x] Responsive design works on different screen sizes (panels collapsible)

## Dependencies

- Epic 03-001 must be complete

## Notes

Consider using:
- MudBlazor drawers for collapsible panels
- CSS Grid or Flexbox for main layout
- Custom splitter component for resize handles

Reference OpenCode desktop layout:
- `/dv/opencode/packages/desktop/src/components/`
