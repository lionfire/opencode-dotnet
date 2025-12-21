# Phase 03: Implementation

**Source**: Change 07 - Faithful OpenCode Web UI Replication
**Duration**: 15-18 hours estimated
**Focus**: Core UX, feature parity, visual consistency

## Motivation

Phases 01 (Deep Analysis) and 02 (Gap Analysis) have identified significant differences between the current Blazor implementation and the OpenCode web UI. This phase focuses on implementing the critical fixes and missing components needed to achieve faithful replication.

The gap analysis revealed:
- CSS variable system is incomplete (~15 variables vs 100+ in OpenCode)
- Missing font families (Geist, Berkeley Mono)
- No tool registry pattern (generic tool display vs specialized renderers)
- Several missing components (Typewriter, FileIcon, ProviderIcon, etc.)
- Session management features not implemented
- Various styling inconsistencies

## Goals and Objectives

1. Establish a comprehensive CSS variable system matching OpenCode
2. Integrate proper font families for visual fidelity
3. Implement tool registry pattern for specialized tool rendering
4. Build missing display components critical for UX
5. Add session management features for feature parity
6. Align existing component styling with OpenCode exactly

## Rationale

This phase is ordered to build foundations first (CSS variables, fonts) before implementing components that depend on them. The tool registry is a structural change that will improve maintainability and enable proper tool displays. Missing components are prioritized by their impact on the user experience.

## Key Deliverables

**Styling Foundation**:
- Complete CSS variable system (`opencode-theme.css`)
- Font integration with proper loading strategy
- Updated components using semantic variables

**Component Architecture**:
- Tool registry pattern with specialized renderers
- Collapsible component for tool details

**New Components**:
- Typewriter (for animated text)
- FileIcon (file-type-specific icons)
- ProviderIcon (AI provider logos)
- Logo (OpenCode branding)
- SessionReview (session summary)
- MessageNav (message navigation)

**Updated Components**:
- ChatMessage (avatar, layout, details toggle)
- ChatInput (focus styling, padding)
- Code blocks (styling alignment)
- Tool displays (collapsible, icons)
- DiffViewer (hunk headers, line numbers)

## Success Criteria

- [ ] All CSS variables from OpenCode are defined and used
- [ ] Font families load correctly without FOUT/FOIT
- [ ] Tool registry handles all tool types with specialized renderers
- [ ] Typewriter component animates text correctly
- [ ] FileIcon shows appropriate icons for common file types
- [ ] SessionReview displays session summary information
- [ ] MessageNav allows navigation between messages
- [ ] Visual inspection shows strong similarity to OpenCode web UI

## Dependencies

**Prerequisites**:
- Phase 01 (Deep Analysis) - COMPLETE
- Phase 02 (Gap Analysis) - COMPLETE
- Gap analysis documents available

**Blocks**:
- Phase 04 (Polish) depends on this phase for animation and theme work

## Suggested Epics

Based on the objectives and deliverables, this phase includes 6 epics:

### Epic 03-001: CSS Variable System Overhaul
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

**Acceptance Criteria**:
- [ ] `opencode-theme.css` contains 80+ variables matching OpenCode
- [ ] No hardcoded color values in component styles
- [ ] All components render correctly with new variables

**Effort**: 2-3 hours

---

### Epic 03-002: Font Integration
Add OpenCode's font families for visual fidelity.

**Tasks**:
1. Add Geist font family (sans-serif) - acquire or use open alternative
2. Add Berkeley Mono font family (monospace) - or use JetBrains Mono as close match
3. Create `@font-face` declarations in fonts.css
4. Define `--font-sans` and `--font-mono` variables
5. Apply fonts to body and code elements
6. Verify font rendering across components
7. Add font loading strategy (prevent FOUT/FOIT)

**Acceptance Criteria**:
- [ ] Sans-serif font matches OpenCode visual style
- [ ] Monospace font renders code correctly
- [ ] No flash of unstyled/invisible text during load

**Effort**: 1-2 hours

---

### Epic 03-003: Tool Registry Pattern
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

**Acceptance Criteria**:
- [ ] Each tool type has a specialized renderer
- [ ] Collapsible component works for expandable content
- [ ] ChatMessage delegates to tool registry
- [ ] Tool displays match OpenCode visual style

**Effort**: 4-5 hours

---

### Epic 03-004: Missing Display Components
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

**Acceptance Criteria**:
- [ ] Typewriter animates text at configurable speed
- [ ] FileIcon shows correct icon for 10+ common file types
- [ ] ProviderIcon displays for at least Anthropic and OpenAI
- [ ] Logo component renders OpenCode branding

**Effort**: 3-4 hours

---

### Epic 03-005: Session Management Features
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

**Acceptance Criteria**:
- [ ] SessionReview displays accurate session statistics
- [ ] MessageNav allows navigation between messages
- [ ] StickyAccordionHeader keeps context visible during scroll
- [ ] ChatPanel integrates navigation controls

**Effort**: 3-4 hours

---

### Epic 03-006: Component Styling Alignment
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

**Acceptance Criteria**:
- [ ] ChatMessage avatar is 24px rounded square
- [ ] ChatInput focus ring uses correct color
- [ ] Code blocks use semantic variables
- [ ] Tool calls use Collapsible pattern
- [ ] Visual comparison shows strong match to OpenCode

**Effort**: 2-3 hours

---

## Risks and Mitigations

- **Risk**: Font licensing issues with Geist or Berkeley Mono
  - **Mitigation**: Use open-source alternatives (Inter, JetBrains Mono) if needed

- **Risk**: Tool registry adds complexity to ChatMessage
  - **Mitigation**: Keep interface simple, document patterns clearly

- **Risk**: Typewriter animation performance on large text
  - **Mitigation**: Use requestAnimationFrame, consider chunking

- **Risk**: Sticky header behavior may not work cross-browser
  - **Mitigation**: Use intersection observer with fallback

## Related Documents

- [Component Mapping](/src/opencode-dotnet/docs/opencode-analysis/component-mapping.md)
- [Styling Comparison](/src/opencode-dotnet/docs/opencode-analysis/styling-comparison.md)
- [Phase Overview](./phases.md)
