# Epic 03-006: Terminal/Output Component

**Phase**: 02 - Core Components
**Status**: Complete
**Priority**: Medium

## Overview

Build the terminal/output component for displaying command output and logs.

## Stories / Tasks

### Data Model

- [ ] **Task 1**: Define TerminalOutput model
  - Content, Timestamp, OutputType
  - OutputType: stdout, stderr, system
  - Optional: ANSI color codes support

- [ ] **Task 2**: Create terminal buffer service
  - Append output lines
  - Clear buffer
  - Scroll to bottom
  - Maximum buffer size limit

### Component Implementation

- [ ] **Task 3**: Create Terminal.razor component
  - Monospace font rendering
  - Auto-scroll to bottom
  - Scroll position preservation
  - Clear button

- [ ] **Task 4**: Create TerminalLine.razor component
  - Stdout styling
  - Stderr styling (red/error)
  - System message styling
  - Timestamp display (optional)

- [ ] **Task 5**: Add ANSI color support (optional)
  - Parse ANSI escape codes
  - Map to CSS classes
  - Support basic colors and bold

### Input (Optional)

- [ ] **Task 6**: Add command input (if needed)
  - Input prompt at bottom
  - Command history
  - Send to backend

### Styling

- [ ] **Task 7**: Style terminal to match OpenCode
  - Dark terminal background
  - Monospace font (JetBrains Mono or similar)
  - Proper scrollbar styling
  - Selection highlighting

## Acceptance Criteria

- [ ] Terminal displays output lines correctly
- [ ] Stderr shows in red/error color
- [ ] Auto-scroll to new content works
- [ ] Manual scroll position is preserved
- [ ] Clear functionality works
- [ ] Styling matches OpenCode design

## Dependencies

- Epic 03-001, 03-002 must be complete

## Notes

This can start simple (just output display) and add input later if needed.

OpenCode terminal reference:
- `/dv/opencode/packages/ui/src/components/terminal/`

Consider XTerm.js via JS interop for full terminal emulation if needed.
