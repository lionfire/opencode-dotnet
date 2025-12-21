# Epic 03-005: Diff Viewer Component

**Phase**: 02 - Core Components
**Status**: Complete
**Priority**: Medium

## Overview

Build the diff viewer component for displaying file changes with syntax highlighting.

## Stories / Tasks

### Data Model

- [ ] **Task 1**: Define FileDiff model
  - FileName, OldContent, NewContent
  - DiffHunks collection
  - LineChanges (added, removed, unchanged)
  - FilePath for navigation

- [ ] **Task 2**: Define DiffHunk model
  - OldStart, OldCount, NewStart, NewCount
  - Lines with change type indicators
  - Context lines

### Component Implementation

- [ ] **Task 3**: Create DiffViewer.razor component
  - Side-by-side or unified view option
  - File header with path display
  - Expand/collapse hunks
  - Line number display

- [ ] **Task 4**: Create DiffLine.razor component
  - Added line styling (green)
  - Removed line styling (red)
  - Unchanged line styling
  - Line number gutter

- [ ] **Task 5**: Add syntax highlighting
  - Integrate syntax highlighter library
  - Language detection from file extension
  - Preserve highlighting in diff context

### Navigation

- [ ] **Task 6**: Implement diff navigation
  - Jump to next/previous change
  - File selector for multi-file diffs
  - Keyboard shortcuts (optional)

### Styling

- [ ] **Task 7**: Style diff viewer to match OpenCode
  - Dark theme colors for diff
  - Proper line spacing
  - Gutter styling
  - Selection highlighting

## Acceptance Criteria

- [ ] Diffs display with proper color coding
- [ ] Line numbers show correctly
- [ ] Syntax highlighting works for common languages
- [ ] Side-by-side and unified views available
- [ ] Navigation between changes works
- [ ] Styling matches OpenCode design

## Dependencies

- Epic 03-001, 03-002 must be complete

## Notes

Consider libraries:
- DiffPlex for diff generation in .NET
- Prism.js or similar for syntax highlighting (via JS interop)
- Or use BlazorMonaco for full editor capabilities

OpenCode diff reference:
- `/dv/opencode/packages/ui/src/components/diff/`
