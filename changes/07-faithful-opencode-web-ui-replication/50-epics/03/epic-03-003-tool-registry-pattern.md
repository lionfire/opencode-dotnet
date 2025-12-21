# Epic 03-003: Tool Registry Pattern

## Overview

Implement specialized tool renderers matching OpenCode's architecture. Currently, the Blazor implementation uses generic tool display with MudBlazor icons. OpenCode has a ToolRegistry pattern with specialized renderers per tool type, providing appropriate icons, layouts, and displays for each tool. This epic restructures tool rendering for better UX and maintainability.

## Goals

1. Create a tool registry interface and service for managing tool renderers
2. Implement specialized renderers for all tool types (read, write, edit, bash, glob, grep, webfetch, todowrite)
3. Create a Collapsible wrapper component for expandable tool details
4. Update ChatMessage to delegate tool rendering to the registry
5. Match OpenCode's tool display visual style

## Tasks

- [x] Create tool registry architecture:
  - [x] Create `IToolRenderer` interface with `CanHandle(toolName)` and `GetIcon/GetSummary` methods
  - [x] Create `ToolRegistry` service to manage and dispatch to renderers
  - [x] Created in LionFire.OpenCode.Blazor/Components/Tools/
- [x] Create `Collapsible.razor` wrapper component:
  - [x] Trigger element (clickable header)
  - [x] Collapsible content area
  - [x] Open/closed state management
  - [x] Smooth CSS transition for arrow rotation
  - [x] Located in LionFire.OpenCode.Blazor/Components/Shared/
- [x] Implement tool renderers (as classes in ToolRegistry.cs):
  - [x] `ReadToolRenderer` - visibility icon, file path display
  - [x] `WriteToolRenderer` - create icon, file path display
  - [x] `EditToolRenderer` - edit icon, file path display
  - [x] `BashToolRenderer` - terminal icon, command/description display
  - [x] `GlobToolRenderer` - search icon, pattern display
  - [x] `GrepToolRenderer` - find_in_page icon, pattern display
  - [x] `WebFetchToolRenderer` - public icon, URL/host display
  - [x] `TodoWriteToolRenderer` - checklist icon
  - [x] `TaskToolRenderer` - assignment icon, description display
  - [x] `GenericToolRenderer` - build icon, fallback for unknown tools
- [x] Create `ToolCall.razor` component:
  - [x] Uses Collapsible for expand/collapse
  - [x] Spinner animation for running state
  - [x] State-based icon coloring
  - [x] Input/Output display in expandable section
- [x] Add appropriate SVG icons per tool type:
  - [x] All Material-style icons as inline SVG
  - [x] State-aware coloring (pending/running/completed/error)
- [ ] Update ChatMessage.razor to use ToolCall component (optional - inline version works)
  - Current inline implementation has been styled with semantic variables
  - ToolCall component available for future use
- [x] Style tool displays to match OpenCode:
  - [x] Updated to use semantic CSS variables
  - [x] Spinner animation for running state
  - [x] Collapsible pattern implemented

## Acceptance Criteria

- Each tool type has a specialized renderer with appropriate icon and layout
- Collapsible component works for expandable content with smooth animation
- ChatMessage delegates all tool rendering to tool registry
- Tool displays match OpenCode visual style (no left border, collapsible, appropriate icons)
- Fallback renderer handles unknown tool types gracefully
- Tool progress uses spinner indicator

## Dependencies

- Epic 03-001: CSS Variable System Overhaul (tool styling uses theme variables)

## References

- [Component Mapping](/src/opencode-dotnet/docs/opencode-analysis/component-mapping.md) - Tool Registry section
- OpenCode BasicTool component: `packages/web/ui/chat/tool/basic-tool.tsx`
- OpenCode tool renderers: `packages/web/ui/chat/tool/`

## Effort Estimate

4-5 hours
