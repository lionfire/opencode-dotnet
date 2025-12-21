# Epic 01-002: Document Component Hierarchy and Data Flow

**Change**: 07 - Faithful OpenCode Web UI Replication
**Phase**: 01 - Deep Analysis
**Status**: planned
**Effort**: 2 days
**Dependencies**: Epic 01-001 (Component inventory must be complete)

## Status Overview

- [ ] Planning Complete
- [ ] Development Started
- [ ] Core Features Complete
- [ ] Testing Complete
- [ ] Documentation Complete

## Overview

Analyze the component relationships, parent-child hierarchies, and data flow patterns in the OpenCode web UI. This epic builds on the component inventory from Epic 01-001 to understand how components interact, how data flows through the application, and how state is managed.

## User Stories & Implementation Tasks

### Must Have

#### Story 001: Main Application Layout Hierarchy
**Effort**: 4 hours
**Status**: pending

Document the top-level application structure and main layout components.

##### Tasks
- [ ] 0001: Identify the root App component and its location
- [ ] 0002: Map the main layout components (header, sidebar, main content area, panels)
- [ ] 0003: Document the chat interface component hierarchy
- [ ] 0004: Identify panel components (tools, files, terminal, etc.)
- [ ] 0005: Create hierarchy diagram showing parent-child relationships
- [ ] 0006: Document in `/src/opencode-dotnet/docs/opencode-analysis/layout-hierarchy.md`

#### Story 002: Chat Component Hierarchy
**Effort**: 6 hours
**Status**: pending

Deep dive into chat-related components and their relationships.

##### Tasks
- [ ] 0001: Identify the main chat container component
- [ ] 0002: Document message list component and message item components
- [ ] 0003: Map input/textarea components for user messages
- [ ] 0004: Identify tool call visualization components
- [ ] 0005: Document permission request dialog components
- [ ] 0006: Map typing indicators and loading states
- [ ] 0007: Document code block and syntax highlighting components
- [ ] 0008: Create detailed chat component tree diagram
- [ ] 0009: Add to `/src/opencode-dotnet/docs/opencode-analysis/chat-components.md`

#### Story 003: Data Flow Analysis
**Effort**: 6 hours
**Status**: pending

Understand how data flows through the application - props, events, state management.

##### Tasks
- [ ] 0001: Identify state management approach (Context, Redux, Zustand, etc.)
- [ ] 0002: Document global state vs local component state
- [ ] 0003: Map prop drilling patterns (where props are passed through multiple levels)
- [ ] 0004: Identify event handlers and callback patterns
- [ ] 0005: Document how chat messages flow from backend to UI
- [ ] 0006: Document how tool calls are initiated and responses handled
- [ ] 0007: Identify any custom hooks for state/data management
- [ ] 0008: Create data flow diagrams for key user interactions
- [ ] 0009: Document in `/src/opencode-dotnet/docs/opencode-analysis/data-flow.md`

### Should Have

#### Story 004: Provider/Model Selector Component Analysis
**Effort**: 3 hours
**Status**: pending

Analyze the model and provider selection UI components.

##### Tasks
- [ ] 0001: Locate provider/model selector component
- [ ] 0002: Document dropdown/modal UI patterns
- [ ] 0003: Map how available models/providers are fetched
- [ ] 0004: Document selection state management
- [ ] 0005: Identify how selection affects chat behavior
- [ ] 0006: Add to `/src/opencode-dotnet/docs/opencode-analysis/selector-components.md`

#### Story 005: Side Panel Components Hierarchy
**Effort**: 4 hours
**Status**: pending

Document file explorer, terminal, and other side panel components.

##### Tasks
- [ ] 0001: Identify file explorer component structure (if present)
- [ ] 0002: Document terminal panel components
- [ ] 0003: Map diff viewer components
- [ ] 0004: Identify settings/configuration panels
- [ ] 0005: Document panel switching and visibility logic
- [ ] 0006: Add to `/src/opencode-dotnet/docs/opencode-analysis/panel-components.md`

### Nice to Have

#### Story 006: Custom Hooks Documentation
**Effort**: 2 hours
**Status**: pending

Catalog and document custom React hooks used throughout the application.

##### Tasks
- [ ] 0001: Search for custom hook files (usually prefixed with `use`)
- [ ] 0002: Document hook purposes and parameters
- [ ] 0003: Identify hooks for API calls, state, effects, etc.
- [ ] 0004: Map which components use which hooks
- [ ] 0005: Add to `/src/opencode-dotnet/docs/opencode-analysis/custom-hooks.md`

## Technical Requirements Checklist

- [ ] Component hierarchy diagrams are clear and visual
- [ ] Data flow is documented with concrete examples
- [ ] State management patterns are identified and explained
- [ ] All major UI sections have documented component trees
- [ ] Documentation includes code snippets where helpful

## Dependencies & Blockers

- **Dependency**: Epic 01-001 must be complete (component inventory needed)
- Requires access to `/dv/opencode/packages/ui/src/` for code reading

## Acceptance Criteria

- [ ] Main application layout hierarchy documented with diagram
- [ ] Chat component tree is complete with all subcomponents
- [ ] Data flow patterns documented for key interactions (sending message, tool call, permission request)
- [ ] State management approach clearly identified and explained
- [ ] At least 3 data flow diagrams created for key user flows
- [ ] Documentation provides clear understanding for Blazor component design

## Notes

**Focus Areas**:
1. How messages are rendered (most critical for chat UI)
2. How tool calls are visualized
3. How permissions are requested and handled
4. Component composition patterns

**Documentation Target**: `/src/opencode-dotnet/docs/opencode-analysis/`

This epic is critical for understanding how to structure our Blazor components to match the OpenCode architecture.
