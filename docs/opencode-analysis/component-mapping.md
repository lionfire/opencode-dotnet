# Component Mapping Matrix

Side-by-side comparison of OpenCode web UI components and Blazor implementations.

## Status Legend

- **Complete** - Full feature parity
- **Partial** - Basic functionality, missing features
- **Minimal** - Very basic implementation
- **Missing** - Not implemented

---

## Chat/Session Components

| OpenCode Component | Blazor Equivalent | Status | Notes |
|-------------------|-------------------|--------|-------|
| SessionTurn | ChatMessage | **Partial** | Missing: typewriter animation, file diffs accordion, summary display, message rail |
| MessagePart | (inline in ChatMessage) | **Partial** | Text/code/thinking/tool types handled, but no tool registry pattern |
| MessageProgress | MudProgressLinear | **Minimal** | Basic progress bar vs dedicated progress component |
| MessageNav | - | **Missing** | No message navigation |
| SessionReview | - | **Missing** | No session review/summary view |
| SessionMessageRail | - | **Missing** | No message sidebar rail |

### Key Differences

**SessionTurn vs ChatMessage**:
| Feature | OpenCode | Blazor |
|---------|----------|--------|
| User message display | Full part rendering | Simple content display |
| Assistant summary | Typewriter-animated summary with title | None |
| File diffs | Accordion with inline diff viewer | Separate DiffViewer panel |
| Tool calls | Per-tool specialized renderers | Generic tool display |
| Collapsible details | "Show details/Hide details" toggle | Always visible |
| Error display | Styled Card component | Plain error text |

---

## Tool Display Components

| OpenCode Component | Blazor Equivalent | Status | Notes |
|-------------------|-------------------|--------|-------|
| BasicTool | (inline in ChatMessage) | **Minimal** | Basic tool display, no Collapsible wrapper |
| DiffChanges | DiffViewer.diff-stats | **Partial** | Shows +/- counts, different styling |
| Diff | DiffViewer | **Partial** | Basic diff display, not using precision-diffs |
| DiffSSR | - | **Missing** | No SSR diff component |

### Tool Registry

OpenCode has a **ToolRegistry** pattern with specialized renderers per tool:

| Tool | OpenCode Renderer | Blazor Handling |
|------|------------------|-----------------|
| read | BasicTool + glasses icon | Generic icon (Visibility) |
| write | BasicTool + diff display | Generic icon (Create) |
| edit | BasicTool + diff display | Generic icon (Edit) |
| bash | BasicTool + terminal output | Generic icon (Terminal) |
| glob | BasicTool + file list | Generic icon (Search) |
| grep | BasicTool + results | Generic icon (FindInPage) |
| webfetch | BasicTool + preview | Generic icon (Public) |
| task | BasicTool + task output | Generic icon (Assignment) |
| todowrite | Checkbox list display | Generic icon (Checklist) |

**Priority**: Implement tool registry pattern with specialized renderers.

---

## UI Primitives

| OpenCode Component | Blazor Equivalent | Status | Notes |
|-------------------|-------------------|--------|-------|
| Accordion | MudExpansionPanels | **N/A** | Could use MudBlazor |
| Button | MudButton | **N/A** | Using MudBlazor |
| Card | (custom div) | **Minimal** | Basic styling only |
| Checkbox | MudCheckBox | **N/A** | Using MudBlazor |
| Collapsible | (custom implementation) | **Partial** | In ChatMessage for thinking/tools |
| Dialog | (custom modal) | **Partial** | Model dialog exists, different from Kobalte Dialog |
| DropdownMenu | (custom dropdown) | **Partial** | Agent menu is basic dropdown |
| IconButton | MudIconButton | **N/A** | Using MudBlazor |
| Input | (native textarea) | **Complete** | ChatInput textarea |
| List | (native foreach) | **Minimal** | No virtual scrolling |
| ResizeHandle | (custom divs) | **Minimal** | Handlers exist but not functional |
| Select | (custom dropdown) | **Partial** | Provider selection, not Kobalte Select |
| SelectDialog | (model dialog) | **Partial** | Works but structure differs |
| StickyAccordionHeader | - | **Missing** | No sticky header behavior |
| Tabs | MudTabs | **N/A** | Could use MudBlazor |
| Tag | MudChip | **N/A** | Using MudBlazor |
| Tooltip | MudTooltip | **N/A** | Using MudBlazor |

---

## Display Components

| OpenCode Component | Blazor Equivalent | Status | Notes |
|-------------------|-------------------|--------|-------|
| Avatar | .message-avatar div | **Minimal** | Icon-based, not actual avatar |
| Code | markdown code | **Complete** | Via Markdig rendering |
| Icon | MudIcon | **Partial** | MudBlazor icons vs custom sprite |
| FileIcon | MudIcon (generic) | **Missing** | No file-type-specific icons |
| Logo | - | **Missing** | No OpenCode logo |
| Markdown | Markdig | **Partial** | No Shiki syntax highlighting |
| ProgressCircle | MudProgressCircular | **Complete** | Using MudBlazor |
| ProviderIcon | - | **Missing** | No provider-specific icons |
| Spinner | MudProgressCircular | **Complete** | Using MudBlazor |
| Typewriter | - | **Missing** | No typewriter animation |

---

## Layout Components

| OpenCode Component | Blazor Equivalent | Status | Notes |
|-------------------|-------------------|--------|-------|
| (app layout) | IdeLayout | **Partial** | Multi-pane layout exists |
| (chat input) | ChatInput | **Partial** | Agent/model selectors exist |
| (chat messages) | ChatPanel | **Partial** | Message list exists |

---

## Missing Components (Priority Order)

### Critical (Required for feature parity)

1. **Typewriter** - Text animation for summaries
   - Complexity: Medium
   - Dependencies: None
   - Effect: Major UX improvement

2. **FileIcon** - File type icons
   - Complexity: Medium
   - Dependencies: Icon sprite or mapping
   - Effect: Visual fidelity

3. **Tool Registry Pattern** - Per-tool renderers
   - Complexity: High
   - Dependencies: Component restructure
   - Effect: Better tool displays

4. **SessionReview** - Session summary view
   - Complexity: Medium
   - Dependencies: Session state
   - Effect: Feature completeness

### Important (Significant UX improvement)

5. **StickyAccordionHeader** - Sticky scroll behavior
   - Complexity: Medium (JS interop)
   - Dependencies: Custom component
   - Effect: Better long-list UX

6. **MessageNav** - Message navigation
   - Complexity: Low
   - Dependencies: Session state
   - Effect: Navigation improvement

7. **Virtual List** - Performance for long lists
   - Complexity: High
   - Dependencies: Virtualize component
   - Effect: Performance

8. **ProviderIcon** - Provider logos
   - Complexity: Low
   - Dependencies: Icon assets
   - Effect: Visual polish

### Nice to Have (Polish)

9. **Logo** - OpenCode branding
   - Complexity: Low
   - Dependencies: SVG asset
   - Effect: Branding

10. **SessionMessageRail** - Message sidebar
    - Complexity: Medium
    - Dependencies: Layout change
    - Effect: Navigation

---

## Structural Differences

### Component Organization

| Aspect | OpenCode | Blazor |
|--------|----------|--------|
| CSS | Separate .css files per component | Embedded `<style>` blocks |
| State | SolidJS stores + contexts | Services + events |
| Props | TypeScript interfaces | `[Parameter]` attributes |
| Events | Kobalte-style callbacks | EventCallback<T> |
| Icons | Custom sprite sheet (731 icons) | MudBlazor Material Icons |

### Data Flow

| Aspect | OpenCode | Blazor |
|--------|----------|--------|
| Message data | `data.store.message[id]` | `ChatService.Messages` |
| Part data | `data.store.part[id]` | `Message.Parts` |
| Session status | `data.store.session_status[id]` | `IdeStateService` |
| Tool state | Inline in message parts | `ChatService.ToolExecutions` |

### Reactivity

| Aspect | OpenCode | Blazor |
|--------|----------|--------|
| Updates | SolidJS signals/stores | `StateHasChanged()` |
| Derived | `createMemo()` | Computed properties |
| Effects | `createEffect()` | `OnAfterRenderAsync()` |
| Subscriptions | Store subscriptions | Event handlers |

---

## Recommendations for Phase 03

### Priority 1: Core UX

1. Implement Typewriter component
2. Add FileIcon with file type mapping
3. Create tool registry pattern

### Priority 2: Feature Parity

4. Add SessionReview component
5. Implement MessageNav
6. Add virtual scrolling to message list

### Priority 3: Visual Polish

7. Add ProviderIcon component
8. Implement StickyAccordionHeader
9. Add Logo component
10. Refine tool display renderers
