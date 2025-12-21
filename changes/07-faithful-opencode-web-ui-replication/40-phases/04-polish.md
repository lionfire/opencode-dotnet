# Phase 04: Polish

**Source**: Change 07 - Faithful OpenCode Web UI Replication
**Duration**: 8-12 hours estimated
**Focus**: Animations, theme support, syntax highlighting, performance

## Motivation

After Phase 03 implements the core functionality and missing components, Phase 04 adds the finishing touches that elevate the UI from functional to polished. The gap analysis identified several areas where OpenCode provides a more refined experience:

- Rich animation system (typewriter, fade-up, collapsible height)
- Full light mode support with multiple theme variants
- Syntax highlighting for code blocks
- Graceful handling of edge cases (empty, loading, error states)
- Performance optimization for large sessions

These polish items may seem secondary, but they significantly impact perceived quality and user experience.

## Goals and Objectives

1. Port OpenCode's animation system for smooth, professional interactions
2. Implement full light mode support with theme switching
3. Add syntax highlighting to code blocks
4. Handle all edge cases gracefully
5. Optimize performance for large sessions

## Rationale

This phase comes after Implementation (Phase 03) because animations and polish depend on the component structure being finalized. Theme support requires the CSS variable system to be complete. Performance optimization should happen after features are implemented to avoid premature optimization.

## Key Deliverables

**Animation System**:
- Animation keyframes CSS file
- Fade-up stagger for list items
- Collapsible height animations
- Arrow rotation indicators
- Pulse opacity for loading states
- Respect for `prefers-reduced-motion`

**Theme Support**:
- Light mode variable values
- Media query for system preference
- Manual theme toggle support
- Theme persistence in localStorage

**Syntax Highlighting**:
- Syntax token CSS variables
- Highlighting implementation (Shiki, Prism, or server-side)
- Support for common languages

**Edge Cases**:
- Empty state designs
- Loading state indicators
- Error state displays
- Long content handling

**Performance**:
- Virtual scrolling for message list
- Lazy loading strategies
- Render optimization

## Success Criteria

- [ ] All animations match OpenCode's timing and style
- [ ] Light mode is fully functional and visually correct
- [ ] Code blocks have syntax highlighting for 7+ languages
- [ ] Empty, loading, and error states display appropriately
- [ ] Large sessions (100+ messages) perform smoothly
- [ ] Respects user motion and theme preferences

## Dependencies

**Prerequisites**:
- Phase 03 (Implementation) - CSS variables, components must be complete

**Blocks**:
- None - this is the final phase of Change 07

## Suggested Epics

Based on the objectives and deliverables, this phase includes 5 epics:

### Epic 04-001: Animation System
Port OpenCode's animation system for smooth interactions.

**Tasks**:
1. Create `/wwwroot/css/animations.css` with keyframes
2. **Fade Up Stagger Animation**:
   - `@keyframes fadeUp` for list items
   - Staggered delay for sequential appearance
   - Apply to message list on load
3. **Collapsible Height Animation**:
   - Smooth height transitions for expandable sections
   - CSS `max-height` approach or JS-based calculation
   - Apply to Collapsible component
4. **Arrow Rotation Animation**:
   - Rotate indicator (e.g., chevron) for collapsible sections
   - 0 to 90 or 0 to 180 degree rotation
5. **Pulse Opacity Animation**:
   - For loading/thinking indicators
   - Smooth breathing effect
6. Integrate animations into Collapsible component
7. Add animation to message entry (new messages fade in)
8. Add animation preferences:
   - Check `prefers-reduced-motion: reduce`
   - Provide reduced or no-motion alternatives

**Acceptance Criteria**:
- [ ] fadeUp animation works on list items
- [ ] Collapsible height animates smoothly
- [ ] Arrow indicators rotate on toggle
- [ ] Animations disabled when user prefers reduced motion

**Effort**: 2 hours

---

### Epic 04-002: Light Mode Support
Implement full light theme matching OpenCode.

**Tasks**:
1. Define light mode variable values in `opencode-theme.css`:
   - Background: `#f8f7f7` base
   - Text: dark variants
   - Borders: lighter variants
2. Add `@media (prefers-color-scheme: light)` rules
3. Add `data-theme` attribute support:
   - `data-theme="light"` forces light mode
   - `data-theme="dark"` forces dark mode
   - No attribute follows system preference
4. Create `ThemeToggle.razor` component:
   - Three-way toggle: Light / System / Dark
   - Visual indicator of current mode
5. Update all background variables for light mode
6. Update all text color variables for light mode
7. Update all border variables for light mode
8. Test all components in light mode:
   - ChatMessage
   - ChatInput
   - Code blocks
   - Tool displays
   - DiffViewer
9. Verify diff colors work in light mode (may need adjustment)
10. Store theme preference in localStorage
11. Apply preference on page load (before render to avoid flash)

**Acceptance Criteria**:
- [ ] Light mode renders correctly for all components
- [ ] Theme toggle switches between modes
- [ ] Preference persists across sessions
- [ ] No flash of wrong theme on load

**Effort**: 2-3 hours

---

### Epic 04-003: Syntax Highlighting
Add code syntax highlighting matching OpenCode's style.

**Tasks**:
1. Define `--syntax-*` CSS variables for all token types:
   - `--syntax-keyword` - control flow, declarations
   - `--syntax-string` - string literals
   - `--syntax-number` - numeric literals
   - `--syntax-comment` - comments
   - `--syntax-function` - function names
   - `--syntax-variable` - variable names
   - `--syntax-type` - type names
   - `--syntax-operator` - operators
   - `--syntax-punctuation` - brackets, semicolons
2. Evaluate and choose approach:
   - **Option A: Shiki via WASM** - highest fidelity, VS Code themes, complex setup
   - **Option B: Prism.js** - good quality, easier setup, well-documented
   - **Option C: Highlight.js** - simpler, good language support
   - **Option D: Server-side with Markdig** - ColorCode or similar extension
3. Implement chosen approach:
   - If client-side (Prism/Highlight.js): Add JS library, integrate with Markdig output
   - If server-side: Configure Markdig extension, render highlighted HTML
4. Support common languages:
   - TypeScript / JavaScript
   - C#
   - Python
   - JSON
   - Markdown
   - Bash / Shell
   - YAML
5. Match OpenCode's color palette for tokens
6. Handle inline code styling (may just use background, no highlighting)
7. Test with various code samples from real sessions

**Acceptance Criteria**:
- [ ] Code blocks have syntax highlighting
- [ ] 7+ languages supported
- [ ] Colors match OpenCode's theme
- [ ] Works in both light and dark modes

**Effort**: 4-6 hours

---

### Epic 04-004: Edge Case Handling
Handle all edge cases for production quality.

**Tasks**:
1. **Empty States**:
   - Create `EmptyState.razor` component with icon, title, description
   - No messages state for chat: "Start a conversation"
   - No sessions state for session list: "No sessions yet"
   - No search results: "No results found"
2. **Loading States**:
   - Initial session loading: Skeleton or spinner
   - Message streaming progress: Typing indicator
   - Tool execution progress: Spinner with tool name
   - Session list loading: Skeleton cards
3. **Error States**:
   - Create `ErrorState.razor` component with retry option
   - Connection error: "Connection lost. Retry?"
   - API error: Display error message with details toggle
   - Tool execution error: Show in tool display with error styling
4. **Long Content Handling**:
   - Very long messages: "Show more" button for truncation
   - Very long file names: Text overflow ellipsis with tooltip
   - Large diff displays: Collapse by default, expand on demand
   - Very long code blocks: Max height with scroll
5. Add graceful degradation:
   - Missing avatar: Default icon
   - Missing provider icon: Generic AI icon
   - Missing file icon: Generic file icon

**Acceptance Criteria**:
- [ ] Empty states display helpful messages
- [ ] Loading states indicate progress
- [ ] Error states allow retry
- [ ] Long content is handled gracefully

**Effort**: 2-3 hours

---

### Epic 04-005: Performance Optimization
Optimize for smooth performance with large sessions.

**Tasks**:
1. **Virtual Scrolling**:
   - Implement `<Virtualize>` for message list
   - ItemSize estimation for dynamic heights
   - Placeholder component for loading items
   - Handle dynamic message heights (may need JS measurement)
   - Maintain scroll position on new messages (scroll to bottom)
2. **Lazy Loading**:
   - Lazy load older messages (paginate on scroll up)
   - Lazy load large diff content (expand on demand)
   - Lazy load file icons (if using icon sprite)
3. **Render Optimization**:
   - Add `ShouldRender()` to components where appropriate
   - Minimize state changes that trigger re-renders
   - Use `@key` directive for list items
   - Profile with browser dev tools
   - Fix identified performance bottlenecks
4. **Asset Optimization**:
   - Ensure CSS is minified in production
   - Optimize font loading (font-display: swap)
   - Consider CSS splitting by component (if bundle gets large)
   - Lazy load syntax highlighting JS (if using client-side)

**Acceptance Criteria**:
- [ ] Virtual scrolling works for 100+ messages
- [ ] Scroll performance is smooth (60fps)
- [ ] Initial load time is acceptable (<2s)
- [ ] Memory usage stays reasonable

**Effort**: 2-3 hours

---

## Risks and Mitigations

- **Risk**: Syntax highlighting adds significant JS bundle size
  - **Mitigation**: Use server-side highlighting or lazy-load client library

- **Risk**: Virtual scrolling with dynamic heights is complex
  - **Mitigation**: Start with estimated heights, refine if needed

- **Risk**: Light mode may reveal styling issues
  - **Mitigation**: Test thoroughly, fix issues as found

- **Risk**: Animation preferences may not be detected correctly
  - **Mitigation**: Default to reduced motion if detection fails

- **Risk**: Theme flash on page load
  - **Mitigation**: Apply theme in `<head>` before body renders

## Related Documents

- [Styling Comparison](/src/opencode-dotnet/docs/opencode-analysis/styling-comparison.md)
- [Phase 03: Implementation](./03-implementation.md)
- [Phase Overview](./phases.md)
