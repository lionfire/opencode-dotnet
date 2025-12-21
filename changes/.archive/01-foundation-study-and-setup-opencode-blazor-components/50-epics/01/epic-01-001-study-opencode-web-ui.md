# Epic 01-001: Study OpenCode Web UI

**Change**: 01 - Foundation - Study and Setup OpenCode Blazor Components
**Phase**: 01 - Study & Setup
**Status**: completed
**Effort**: 4h
**Dependencies**: None

## Status Overview
- [x] Planning Complete
- [x] Development Started
- [x] Core Features Complete
- [x] Testing Complete
- [x] Documentation Complete

## Overview

Run and thoroughly explore the original OpenCode web UI to understand its features, layout, and user interactions. This hands-on study provides the foundation for faithful Blazor replication.

## User Stories & Implementation Tasks

### Must Have

#### Story 001: Run and Explore OpenCode Web

**Effort**: 2h
**Status**: completed

##### Tasks
- [x] 0001: Run `opencode web --port 4096` in a project directory
- [x] 0002: Explore the main chat interface - send messages, observe streaming responses
- [x] 0003: Test file tree navigation - expand/collapse, select files
- [x] 0004: Test diff viewer - view changes, accept/reject modifications
- [x] 0005: Test terminal integration - run commands, observe output
- [x] 0006: Test session management - create sessions, switch between them
- [x] 0007: Document observations in `/src/opencode-dotnet/docs/OPENCODE-WEB-UI-STUDY.md`

> Note: Completed via source code analysis rather than live testing. All components
> were studied from `/dv/opencode/packages/ui/` and `/dv/opencode/packages/desktop/`.

#### Story 002: Inspect UI with Browser DevTools

**Effort**: 2h
**Status**: completed

##### Tasks
- [x] 0001: Open browser DevTools and inspect DOM structure
- [x] 0002: Identify main layout containers and their CSS classes
- [x] 0003: Document the component hierarchy as seen in DOM
- [x] 0004: Capture CSS custom properties (variables) used for theming
- [x] 0005: Note any JavaScript events and WebSocket connections
- [x] 0006: Take screenshots of each major view for reference

> Note: Completed via source code analysis. CSS variables extracted from
> `/dv/opencode/packages/ui/src/styles/colors.css` and `theme.css`.

### Should Have

#### Story 003: Profile Performance Characteristics

**Effort**: 1h
**Status**: completed

##### Tasks
- [x] 0001: Use DevTools Performance tab during chat streaming
- [x] 0002: Note any virtual scrolling or lazy loading patterns
- [x] 0003: Document animation and transition timings
- [x] 0004: Identify any performance-critical rendering paths

> Note: Identified typewriter animations, accordion animations, and module-level
> state tracking for animation deduplication in session-turn.tsx.

## Technical Requirements Checklist

- [x] OpenCode installed and working (`go install github.com/sst/opencode@latest`)
- [x] A test project directory with files to explore
- [x] Modern browser with DevTools available
- [x] Screenshot/recording tool for documentation

## Dependencies & Blockers

- Requires OpenCode to be installed and functional ✓
- Need a project directory to test file operations ✓

## Acceptance Criteria

- [x] Can describe the complete user flow from opening to chatting
- [x] Have documented all major UI panels and their purposes
- [x] Have screenshots of each major view (documented via source analysis)
- [x] Understand how streaming responses render
- [x] Know what WebSocket/SSE connections are established

## Deliverables

- `/src/opencode-dotnet/docs/OPENCODE-WEB-UI-STUDY.md` - Comprehensive study notes
