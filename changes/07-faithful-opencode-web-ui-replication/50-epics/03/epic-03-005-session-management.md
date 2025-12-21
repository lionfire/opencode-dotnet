# Epic 03-005: Session Management Features

## Overview

Add session review and navigation features for feature parity with OpenCode. The current Blazor implementation lacks session summary views and message navigation controls that help users understand and navigate long conversations. This epic adds SessionReview, MessageNav, and StickyHeader components.

## Goals

1. Implement SessionReview component for session summary display
2. Implement MessageNav component for message navigation
3. Implement StickyHeader for maintaining context during scroll
4. Integrate navigation features into ChatPanel

## Tasks

### SessionReview Component
- [x] Create `SessionReview.razor` component:
  - [x] Display session title
  - [x] Display session duration
  - [x] Display message count (user/assistant)
  - [x] Display file changes summary:
    - [x] Files read count
    - [x] Files written count
    - [x] Files edited count
  - [x] Display tool usage summary:
    - [x] Bash commands executed
    - [x] Searches performed
    - [x] Web fetches made
  - [x] Display token usage (optional)
- [x] Add styling:
  - [x] Card-like container with theme variables
  - [x] Stats in grid layout
  - [x] SVG icons for each stat type
- [ ] Calculate statistics from session data (requires integration with session service)

### MessageNav Component
- [x] Create `MessageNav.razor` component:
  - [x] Previous message button
  - [x] Next message button
  - [x] Jump to top button
  - [x] Jump to bottom button
  - [x] Message index display (e.g., "3 of 15")
- [x] Implement navigation logic:
  - [x] Track current message index
  - [x] EventCallback for navigation
  - [x] Handle edge cases (first/last message)
- [x] Style navigation controls:
  - [x] Compact button group
  - [x] Disabled state for edge cases
  - [x] Current position indicator
- [ ] Add keyboard shortcuts (optional future enhancement)
- [ ] Integration with scroll-to-message (requires integration with ChatPanel)

### StickyHeader Component (JS Interop)
- [x] Create `StickyHeader.razor` component:
  - [x] Sticky positioning during scroll
  - [x] State callback when sticking/unsticking
  - [x] Smooth transition visual effects
- [x] Implement using Intersection Observer:
  - [x] Create `/wwwroot/js/sticky-header.js` with observer logic
  - [x] Add JS interop to component
  - [x] Handle cleanup on dispose
- [x] Fallback behavior:
  - [x] Graceful degradation if JS not available
- [x] Style sticky header:
  - [x] Background with blur effect when stuck
  - [x] Shadow to indicate floating state
  - [x] Dark mode shadow adjustment

### ChatPanel Integration
- [ ] Add MessageNav to ChatPanel (optional - depends on ChatPanel refactor)
- [ ] Add SessionReview access (optional - depends on ChatPanel refactor)
- [ ] Apply StickyHeader to message groups (optional - depends on ChatPanel refactor)

## Implementation Notes

### Files Created:
- `Components/Session/SessionReview.razor` - Session statistics display
- `Components/Session/MessageNav.razor` - Message navigation controls
- `Components/Shared/StickyHeader.razor` - Sticky header with JS interop
- `wwwroot/js/sticky-header.js` - Intersection Observer implementation

### Key Features:
- SessionReview uses CSS Grid for responsive stat layout
- MessageNav provides accessible navigation with disabled states
- StickyHeader uses IntersectionObserver for efficient scroll detection
- All components use semantic CSS variables from the theme system

## Acceptance Criteria

- [x] SessionReview displays session statistics layout
- [x] SessionReview shows file and tool usage breakdown
- [x] MessageNav allows navigation between messages (prev/next/top/bottom)
- [x] MessageNav shows current position indicator
- [x] StickyHeader detects scroll state via IntersectionObserver
- [x] All components styled with theme variables
- [ ] ChatPanel integrates all navigation controls (optional - future work)

## Dependencies

- Epic 03-001: CSS Variable System Overhaul (components use theme variables)
- Epic 03-003: Tool Registry Pattern (tool usage stats from registry)

## References

- [Component Mapping](/src/opencode-dotnet/docs/opencode-analysis/component-mapping.md) - Session components section
- OpenCode SessionReview: search in `packages/web/ui/`
- OpenCode MessageNav: search in `packages/web/ui/`

## Effort Estimate

3-4 hours
