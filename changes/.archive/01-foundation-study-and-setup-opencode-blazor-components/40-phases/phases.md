# Change 01: Foundation - Study and Setup OpenCode Blazor Components - Phases

## Overview

This document outlines the implementation phases for the foundation work needed before building OpenCode Blazor components.

**Change Type**: feature
**Complexity**: moderate
**Total Phases**: 1

## Phase Summary

| Phase | Name | Goal | Epics |
|-------|------|------|-------|
| 01 | Study & Setup | Understand OpenCode UI and prepare component infrastructure | 4 |

---

## Phase 01: Study & Setup

**Goal**: Gain complete understanding of OpenCode's web UI architecture and set up the foundational infrastructure for Blazor component development.

**Deliverables**:
1. Documented analysis of OpenCode's SolidJS component architecture
2. Component mapping document (SolidJS → Blazor equivalents)
3. Theme system with CSS variables matching OpenCode's design
4. Component structure in `LionFire.OpenCode.Blazor` ready for implementation

**Success Criteria**:
- Can describe every major component in OpenCode's UI
- Theme CSS variables match OpenCode's color scheme and spacing
- Component directory structure created with placeholder files
- Ready to implement actual components

**Dependencies**: None

---

## Epics in Phase 01

| Epic | Title | Effort | Status |
|------|-------|--------|--------|
| 01-001 | Study OpenCode Web UI | 4h | planned |
| 01-002 | Document Component Mapping | 4h | planned |
| 01-003 | Implement Theme System | 4h | planned |
| 01-004 | Setup Component Infrastructure | 4h | planned |

---

## Implementation Notes

### Study Approach
1. Run `opencode web --port 4096` to see live UI
2. Navigate through all features: chat, file tree, diffs, terminal
3. Use browser DevTools to inspect component structure
4. Review source at `/dv/opencode/packages/ui/src/components/`

### Key Components to Study
- Layout: Main layout, sidebar, panels
- Chat: Session turns, message parts, input area
- IDE: File tree, diff viewer, editor tabs
- Terminal: PTY integration, output handling

### Theme System
OpenCode uses a consistent design language with:
- Dark mode by default
- Monospace fonts for code
- Subtle borders and shadows
- Consistent spacing scale
