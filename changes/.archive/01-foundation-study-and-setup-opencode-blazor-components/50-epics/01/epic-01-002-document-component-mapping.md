# Epic 01-002: Document Component Mapping

**Change**: 01 - Foundation - Study and Setup OpenCode Blazor Components
**Phase**: 01 - Study & Setup
**Status**: completed
**Effort**: 4h
**Dependencies**: 01-001 (Study OpenCode Web UI)

## Status Overview
- [x] Planning Complete
- [x] Development Started
- [x] Core Features Complete
- [x] Testing Complete
- [x] Documentation Complete

## Overview

Review the SolidJS source code in `/dv/opencode/packages/ui/` and create a comprehensive mapping document that shows how each SolidJS component will translate to Blazor/MudBlazor equivalents.

## User Stories & Implementation Tasks

### Must Have

#### Story 001: Analyze Layout Components

**Effort**: 1h
**Status**: completed

##### Tasks
- [x] 0001: Review `/dv/opencode/packages/ui/src/components/` directory structure
- [x] 0002: Document main layout component (overall page structure)
- [x] 0003: Document sidebar component (file tree container)
- [x] 0004: Document panel system (resizable panels, tabs)
- [x] 0005: Map each to Blazor/MudBlazor equivalent (MudLayout, MudDrawer, etc.)

#### Story 002: Analyze Chat Components

**Effort**: 1.5h
**Status**: completed

##### Tasks
- [x] 0001: Study `session-turn.tsx` (400+ lines) - message rendering
- [x] 0002: Study `message-part.tsx` (450+ lines) - content type handling
- [x] 0003: Study `prompt-input.tsx` (1000+ lines) - input area with features
- [x] 0004: Document props/parameters for each component
- [x] 0005: Map to Blazor components (custom + MudTextField, etc.)

#### Story 003: Analyze IDE Components

**Effort**: 1h
**Status**: completed

##### Tasks
- [x] 0001: Study file tree component - tree structure, icons, expand/collapse
- [x] 0002: Study diff viewer component - unified/split view, line highlighting
- [x] 0003: Study editor tabs component - tab management, close buttons
- [x] 0004: Document interactions and state management patterns
- [x] 0005: Map to Blazor (MudTreeView, Monaco Editor wrapper, MudTabs)

#### Story 004: Create Mapping Document

**Effort**: 1h
**Status**: completed

##### Tasks
- [x] 0001: Create `/src/opencode-dotnet/docs/COMPONENT-MAPPING.md`
- [x] 0002: Document each SolidJS component with Blazor equivalent
- [x] 0003: Note which need custom implementation vs MudBlazor
- [x] 0004: Document SolidJS patterns and their Blazor translations
- [x] 0005: Include code examples for key pattern translations

### Should Have

#### Story 005: Document State Management Patterns

**Effort**: 1h
**Status**: completed

##### Tasks
- [x] 0001: Identify how SolidJS signals are used
- [x] 0002: Document data flow between components
- [x] 0003: Plan Blazor state management approach (Fluxor, Cascading, Services)
- [x] 0004: Document event handling patterns

## Technical Requirements Checklist

- [x] Access to `/dv/opencode/packages/ui/` source code
- [x] Understanding of SolidJS concepts (signals, effects, stores)
- [x] Understanding of Blazor concepts (parameters, cascading values, events)
- [x] MudBlazor documentation reference

## Dependencies & Blockers

- Depends on Epic 01-001 for context on what components do ✓
- Need OpenCode source available at `/dv/opencode/` ✓

## Acceptance Criteria

- [x] COMPONENT-MAPPING.md created with all major components
- [x] Each SolidJS component has a planned Blazor equivalent
- [x] Pattern translations documented with code examples
- [x] Clear identification of custom vs library components needed
- [x] State management approach decided

## Deliverables

- `/src/opencode-dotnet/docs/COMPONENT-MAPPING.md` - Complete component mapping with:
  - 11 major sections
  - Layout, Session, Tool, Primitive component mappings
  - State management patterns
  - Icon mapping table
  - CSS class mapping
  - File structure recommendation
  - Implementation priority phases
