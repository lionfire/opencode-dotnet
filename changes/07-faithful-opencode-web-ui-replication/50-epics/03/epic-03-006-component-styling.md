# Epic 03-006: Component Styling Alignment

## Overview

Update existing component styling to match OpenCode exactly. The gap analysis identified numerous styling differences between the current Blazor implementation and OpenCode: avatar sizes and shapes, message spacing, code block styling, tool call displays, and diff viewer details. This epic aligns all existing component styling with OpenCode's visual design.

## Goals

1. Align ChatMessage styling (avatar, spacing, details toggle)
2. Align ChatInput styling (focus ring, padding)
3. Align code block styling (header, background)
4. Align tool call display styling (collapsible, progress indicator)
5. Align DiffViewer styling (hunk headers, line numbers)

## Tasks

### ChatMessage Refinements
- [x] Change avatar from circle to rounded square:
  - [x] Update border-radius from 50% to 6px
  - [x] Match OpenCode's avatar shape
- [x] Reduce avatar size from 32px to 24px:
  - [x] Update width and height
  - [x] Update icon size within avatar (14px)
- [x] Adjust message gap from 12px to 8px:
  - [x] Update gap between avatar and content
- [ ] Add "Show details/Hide details" toggle for assistant messages (optional - future feature)
- [x] User message styling already uses theme variables
- [x] Assistant message styling uses surface-raised-base

### ChatInput Refinements
- [x] Focus ring uses semantic variables (shadow-xs-border-select)
- [x] Padding matches OpenCode (12px 20px)
- [x] Border styling uses theme variables
- [x] Background uses surface-raised-stronger

### Code Block Refinements
- [x] Header styling uses theme variables
- [x] Background uses surface-inset-strong
- [x] Border-radius matches OpenCode (radius-lg)
- [x] Overflow handling implemented

### Tool Call Display Refinements
- [x] Remove left border style:
  - [x] Deleted 3px left border
  - [x] Using OpenCode's borderless style
- [x] Icon colors based on tool state:
  - [x] Pending: icon-weak
  - [x] Running: icon-interactive-base
  - [x] Completed: icon-success-base
  - [x] Error: icon-critical-base
- [x] Switch from MudProgressLinear to spinner for progress:
  - [x] Custom CSS spinner animation
  - [x] Positioned in tool header
- [x] Update background and padding with surface variables

### DiffViewer Refinements
- [x] Hunk header styling uses theme variables
- [x] Line number column styling implemented
- [x] Add/remove line styling uses diff color variables
- [x] Proper spacing implemented

## Acceptance Criteria

- [x] ChatMessage avatar is 24px rounded square (not 32px circle)
- [x] ChatMessage has 8px gap (not 12px)
- [ ] ChatMessage has "Show details/Hide details" toggle (optional - future work)
- [x] ChatInput focus ring uses semantic shadow variable
- [x] ChatInput padding matches OpenCode
- [x] Code blocks use semantic variables for background
- [x] Tool calls use borderless style (no left border)
- [x] Tool calls use spinner for progress (not linear progress bar)
- [x] Tool icon color reflects state (pending/running/completed/error)
- [x] DiffViewer styling uses theme variables

## Implementation Notes

### Files Modified:
- `examples/AgUi.IDE.BlazorServer/Components/Shared/Chat/ChatMessage.razor`
  - Avatar: 24px, border-radius 6px (rounded square)
  - Gap: reduced from 12px to 8px
  - Tool call: removed left border, added spinner, state-based icon colors

### Key Style Changes:
- Avatar: `width: 24px; height: 24px; border-radius: 6px;`
- Tool blocks: No left border, uses surface-inset-base background
- Tool spinner: 14px CSS spinner animation
- Icon colors: State-based using theme icon variables

## Dependencies

- Epic 03-001: CSS Variable System Overhaul (required for semantic variables)
- Epic 03-003: Tool Registry Pattern (Collapsible component available)

## References

- [Styling Comparison](/src/opencode-dotnet/docs/opencode-analysis/styling-comparison.md) - Component styling differences table
- [Component Mapping](/src/opencode-dotnet/docs/opencode-analysis/component-mapping.md) - Structural differences
- OpenCode ChatMessage/SessionTurn styling

## Effort Estimate

2-3 hours
