# Change 07: Faithful OpenCode Web UI Replication - Phases

## Overview

This document outlines the implementation phases for faithfully replicating the OpenCode web UI.

**Source**: Change 07 gap analysis documents
**Total Phases**: 4

## Phase Summary

| Phase | Name | Goal | Duration | Epics |
|-------|------|------|----------|-------|
| 01 | Deep Analysis | Thoroughly analyze OpenCode web structure | Complete | 4 |
| 02 | Gap Analysis | Compare and document differences | Complete | 2 |
| 03 | Implementation | Fix inconsistencies and add missing pieces | 15-18 hours | 6 |
| 04 | Polish | Animations, transitions, edge cases | 8-12 hours | 5 |

---

## Phase 01: Deep Analysis (COMPLETE)

**Goal**: Thoroughly understand the OpenCode web UI architecture, components, styling, and data flow.

**Deliverables**:
1. Complete component inventory from `/dv/opencode/packages/ui/`
2. Component hierarchy diagram
3. CSS/styling system documentation
4. State management patterns documentation
5. Event/interaction patterns documentation

**Epics**:
- 01-001: Map OpenCode web directory and component structure
- 01-002: Document component hierarchy and data flow
- 01-003: Extract and document styling system
- 01-004: Document interaction patterns and keyboard shortcuts

---

## Phase 02: Gap Analysis (COMPLETE)

**Goal**: Identify all differences between current Blazor implementation and OpenCode web.

**Deliverables**:
1. Side-by-side comparison document (`/docs/opencode-analysis/component-mapping.md`)
2. Missing components list (prioritized in component-mapping.md)
3. Styling differences list (`/docs/opencode-analysis/styling-comparison.md`)
4. Data flow differences list (in component-mapping.md)

**Epics**:
- 02-001: Component-by-component comparison
- 02-002: Styling and theme comparison

---

## Phase 03: Implementation

**Goal**: Implement fixes and missing pieces to achieve faithful replication.

**Duration**: 15-18 hours estimated
**Focus**: Core UX, feature parity, visual consistency

**Deliverables**:
1. Comprehensive CSS variable system matching OpenCode
2. Font integration (Geist, Berkeley Mono)
3. Tool registry pattern with specialized renderers
4. Missing components (Typewriter, FileIcon, ProviderIcon)
5. Session management features (SessionReview, MessageNav)
6. Updated component styling to match OpenCode exactly

**Epics**:

### 03-001: CSS Variable System Overhaul
Migrate from ad-hoc CSS to OpenCode's comprehensive variable system.

**Tasks**:
1. Create `/wwwroot/css/opencode-theme.css` with full variable set
2. Port all background variables (`--background-base`, `--background-weak`, etc.)
3. Port all text color variables (`--text-strong`, `--text-base`, etc.)
4. Port all border variables (`--border-base`, `--border-strong`, etc.)
5. Port surface variables (`--surface-*`) for cards and dialogs
6. Port icon color variables (`--icon-*`)
7. Update all components to use new variables (remove hardcoded colors)
8. Test variable consistency across all components

**Effort**: 2-3 hours

### 03-002: Font Integration
Add OpenCode's font families for visual fidelity.

**Tasks**:
1. Add Geist font family (sans-serif) - acquire or use open alternative
2. Add Berkeley Mono font family (monospace) - or use JetBrains Mono as close match
3. Create `@font-face` declarations in fonts.css
4. Define `--font-sans` and `--font-mono` variables
5. Apply fonts to body and code elements
6. Verify font rendering across components
7. Add font loading strategy (prevent FOUT/FOIT)

**Effort**: 1-2 hours

### 03-003: Tool Registry Pattern
Implement specialized tool renderers matching OpenCode's architecture.

**Tasks**:
1. Create `IToolRenderer` interface with `CanHandle()` and `Render()` methods
2. Create `ToolRegistry` service to manage tool renderers
3. Implement `ReadToolRenderer` - glasses icon, file path display
4. Implement `WriteToolRenderer` - diff display with +/- counts
5. Implement `EditToolRenderer` - diff display with change details
6. Implement `BashToolRenderer` - terminal output styling
7. Implement `GlobToolRenderer` - file list display
8. Implement `GrepToolRenderer` - search results with line numbers
9. Implement `WebFetchToolRenderer` - URL preview
10. Implement `TodoWriteToolRenderer` - checkbox list display
11. Create `Collapsible` wrapper component for tool details
12. Update `ChatMessage` to use tool registry instead of inline logic
13. Add appropriate icons per tool type

**Effort**: 4-5 hours

### 03-004: Missing Display Components
Implement critical missing UI components.

**Tasks**:
1. **Typewriter Component**:
   - Create `Typewriter.razor` with character-by-character animation
   - Add configurable typing speed
   - Support for starting/stopping animation
   - Use for assistant message summaries
2. **FileIcon Component**:
   - Create `FileIcon.razor` with file extension mapping
   - Map common extensions to appropriate icons (ts, js, cs, md, json, etc.)
   - Support for folder icons
   - Fallback icon for unknown types
3. **ProviderIcon Component**:
   - Create `ProviderIcon.razor` for AI provider logos
   - Support Anthropic, OpenAI, and other providers
   - SVG-based for scalability
4. **Logo Component**:
   - Create `OpenCodeLogo.razor` with OpenCode branding SVG
   - Support light/dark variants

**Effort**: 3-4 hours

### 03-005: Session Management Features
Add session review and navigation features.

**Tasks**:
1. **SessionReview Component**:
   - Create `SessionReview.razor` for session summary view
   - Display session title, duration, message count
   - Show file changes summary
   - Show tool usage summary
2. **MessageNav Component**:
   - Create `MessageNav.razor` for message navigation
   - Previous/next message buttons
   - Jump to top/bottom
   - Message index display (e.g., "3 of 15")
3. **StickyAccordionHeader** (JS Interop):
   - Create sticky header behavior for long message threads
   - Keep current message header visible during scroll
4. Update `ChatPanel` to integrate new navigation features

**Effort**: 3-4 hours

### 03-006: Component Styling Alignment
Update existing component styling to match OpenCode exactly.

**Tasks**:
1. **ChatMessage Refinements**:
   - Change avatar from circle to rounded square (OpenCode style)
   - Reduce avatar size from 32px to 24px
   - Adjust message gap from 12px to 8px
   - Add "Show details/Hide details" toggle for assistant messages
2. **ChatInput Refinements**:
   - Update focus ring color to match `--border-selected`
   - Ensure padding matches OpenCode (12px 16px)
3. **Code Block Refinements**:
   - Remove or minimize header prominence
   - Use surface-base variable for background
   - Prepare for syntax highlighting (next phase)
4. **Tool Call Display Refinements**:
   - Remove left border style
   - Use Collapsible pattern with trigger
   - Switch from MudProgressLinear to spinner for progress
5. **DiffViewer Refinements**:
   - Match hunk header styling exactly
   - Verify line number column styling

**Effort**: 2-3 hours

---

## Phase 04: Polish

**Goal**: Add final touches for production-quality UI.

**Duration**: 8-12 hours estimated
**Focus**: Animations, theme support, syntax highlighting, performance

**Deliverables**:
1. Animation system with all OpenCode animations
2. Full light mode support
3. Syntax highlighting for code blocks
4. Edge case handling (empty states, errors, loading)
5. Performance optimization (virtual scrolling)

**Epics**:

### 04-001: Animation System
Port OpenCode's animation system for smooth interactions.

**Tasks**:
1. Create `/wwwroot/css/animations.css` with keyframes
2. **Fade Up Stagger Animation**:
   - `@keyframes fadeUp` for list items
   - Staggered delay for sequential appearance
3. **Collapsible Height Animation**:
   - Smooth height transitions for expandable sections
   - CSS or JS-based height animation
4. **Arrow Rotation Animation**:
   - Rotate indicator for collapsible sections
5. **Pulse Opacity Animation**:
   - For loading/thinking indicators
6. Integrate animations into Collapsible component
7. Add animation to message entry
8. Add animation preferences (respect `prefers-reduced-motion`)

**Effort**: 2 hours

### 04-002: Light Mode Support
Implement full light theme matching OpenCode.

**Tasks**:
1. Define light mode variable values in `opencode-theme.css`
2. Add `@media (prefers-color-scheme: light)` rules
3. Add `data-theme` attribute support for manual toggle
4. Create theme toggle component
5. Update all background variables for light mode
6. Update all text color variables for light mode
7. Update all border variables for light mode
8. Test all components in light mode
9. Verify diff colors work in light mode
10. Store theme preference in localStorage

**Effort**: 2-3 hours

### 04-003: Syntax Highlighting
Add code syntax highlighting matching OpenCode's style.

**Tasks**:
1. Define `--syntax-*` CSS variables for all token types
2. Evaluate approach:
   - Option A: Shiki via WASM (higher fidelity, more complex)
   - Option B: Prism.js or Highlight.js (simpler, good enough)
   - Option C: Server-side rendering with Markdig extension
3. Implement chosen approach
4. Support common languages: TypeScript, JavaScript, C#, Python, JSON, Markdown, Bash
5. Match OpenCode's color palette for tokens
6. Handle inline code styling
7. Test with various code samples

**Effort**: 4-6 hours

### 04-004: Edge Case Handling
Handle all edge cases for production quality.

**Tasks**:
1. **Empty States**:
   - No messages state for chat
   - No sessions state for session list
   - No search results state
2. **Loading States**:
   - Initial session loading
   - Message streaming progress
   - Tool execution progress
3. **Error States**:
   - Connection error display
   - API error display
   - Tool execution error display
4. **Long Content Handling**:
   - Very long messages (truncation/expansion)
   - Very long file names (ellipsis)
   - Large diff displays
5. Add graceful degradation for missing data

**Effort**: 2-3 hours

### 04-005: Performance Optimization
Optimize for smooth performance with large sessions.

**Tasks**:
1. **Virtual Scrolling**:
   - Implement `<Virtualize>` for message list
   - Handle dynamic message heights
   - Maintain scroll position on new messages
2. **Lazy Loading**:
   - Lazy load older messages
   - Lazy load large diff content
3. **Render Optimization**:
   - Minimize unnecessary re-renders
   - Use `ShouldRender()` where appropriate
   - Profile and fix performance bottlenecks
4. **Asset Optimization**:
   - Ensure CSS is minified
   - Optimize font loading
   - Consider CSS splitting

**Effort**: 2-3 hours

---

## Phase Links

- [Phase 03: Implementation](./03-implementation.md)
- [Phase 04: Polish](./04-polish.md)

---

## Next Steps

1. Greenlight Phase 03 epics with `/ax:epic:greenlight`
2. Implement Phase 03 with `/ax:change:implement 07`
3. Review and test Phase 03 deliverables
4. Proceed to Phase 04 for polish
