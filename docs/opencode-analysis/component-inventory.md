# OpenCode Web UI Component Inventory

**Source**: `/dv/opencode/packages/ui/src/components/`
**Framework**: SolidJS
**Total Components**: 39 TSX files

## Component Categories

### Chat/Session Components

Core components for chat functionality.

| Component | File | Lines | Description |
|-----------|------|-------|-------------|
| SessionTurn | session-turn.tsx | 299 | **Main chat turn display** - Shows user message, assistant response, diffs, tool calls |
| MessagePart | message-part.tsx | 440 | **Message content renderer** - Routes parts to appropriate renderers (text, tool, reasoning) |
| MessageProgress | message-progress.tsx | 185 | Progress indicator during response generation |
| MessageNav | message-nav.tsx | 158 | Navigation between messages in a session |
| SessionReview | session-review.tsx | 144 | Review display for completed sessions |
| SessionMessageRail | session-message-rail.tsx | 46 | Sidebar rail for session messages |

### Tool Display Components

Components for rendering tool calls and their outputs.

| Component | File | Lines | Description |
|-----------|------|-------|-------------|
| BasicTool | basic-tool.tsx | 96 | **Base tool component** - Collapsible trigger with icon, title, subtitle |
| DiffChanges | diff-changes.tsx | 78 | Shows +/- line counts for file changes |
| Diff | diff.tsx | 68 | Diff viewer wrapper component |
| DiffSSR | diff-ssr.tsx | 91 | Server-side rendered diff component |

### UI Primitives

Reusable base components (many using @kobalte/core).

| Component | File | Lines | Description |
|-----------|------|-------|-------------|
| Accordion | accordion.tsx | 66 | Expandable sections with items |
| Button | button.tsx | 25 | Standard button with variants |
| Card | card.tsx | 17 | Card container with variants (error, etc.) |
| Checkbox | checkbox.tsx | 44 | Checkbox with label |
| Collapsible | collapsible.tsx | 51 | Collapsible content panel |
| Dialog | dialog.tsx | 53 | Modal dialog with trigger |
| DropdownMenu | dropdown-menu.tsx | 433 | Full dropdown menu with submenus |
| IconButton | icon-button.tsx | 26 | Button containing only an icon |
| Input | input.tsx | 37 | Text input field |
| List | list.tsx | 114 | Virtual list component |
| ResizeHandle | resize-handle.tsx | 80 | Draggable resize handle |
| Select | select.tsx | 107 | Select dropdown |
| SelectDialog | select-dialog.tsx | 89 | Dialog-based selection |
| StickyAccordionHeader | sticky-accordion-header.tsx | 20 | Sticky header for accordion |
| Tabs | tabs.tsx | 68 | Tab navigation |
| Tag | tag.tsx | 17 | Tag/label component |
| Tooltip | tooltip.tsx | 61 | Tooltip on hover |

### Display Components

Presentation-focused components.

| Component | File | Lines | Description |
|-----------|------|-------|-------------|
| Avatar | avatar.tsx | 38 | User/agent avatar display |
| Code | code.tsx | 31 | Inline code display |
| Icon | icon.tsx | 731 | **Icon system** - Large icon set with sprite |
| FileIcon | file-icon.tsx | 523 | File type icon by extension |
| Logo | logo.tsx | 57 | OpenCode logo component |
| Markdown | markdown.tsx | 26 | Markdown-to-HTML renderer |
| ProgressCircle | progress-circle.tsx | 44 | Circular progress indicator |
| ProviderIcon | provider-icon.tsx | 25 | AI provider logo icon |
| Spinner | spinner.tsx | 30 | Loading spinner |
| Typewriter | typewriter.tsx | 39 | Typewriter text animation |

### Utility Components

Helper components.

| Component | File | Lines | Description |
|-----------|------|-------|-------------|
| Favicon | favicon.tsx | 13 | Dynamic favicon setting |
| Font | font.tsx | 35 | Font loading/preload |

## Key Component Details

### SessionTurn (Main Chat Display)

The primary component for rendering a chat conversation turn.

**Location**: `session-turn.tsx`
**Props**:
```typescript
{
  sessionID: string
  messageID: string
  classes?: {
    root?: string
    content?: string
    container?: string
  }
}
```

**Features**:
- Displays user message with all parts
- Shows assistant response with summary
- Accordion for file diffs with inline diff viewer
- Typewriter animation for titles/summaries
- "Show details" / "Hide details" collapsible
- Error card display
- Progress indicator while generating

**Child Components Used**:
- Message, Markdown, Accordion, Collapsible
- DiffChanges, FileIcon, Icon, Card
- MessageProgress, Typewriter, StickyAccordionHeader

### MessagePart (Content Renderer)

Routes message parts to appropriate display components.

**Location**: `message-part.tsx`

**Part Types Handled**:
- `text` - Rendered as Markdown
- `tool` - Routed through ToolRegistry
- `reasoning` - Rendered as Markdown (currently filtered in assistant display)

**Tool Registry**:
Built-in tool renderers:
- `read` - File read with glasses icon
- `list` - Directory listing
- `glob` - Pattern search
- `grep` - Content search
- `webfetch` - Web page fetch
- `task` - Agent task
- `bash` - Shell command
- `edit` - File edit with diff
- `write` - File write
- `todowrite` - Todo list with checkboxes

### BasicTool (Tool Display Base)

Base component for tool displays.

**Location**: `basic-tool.tsx`

**Props**:
```typescript
{
  icon: IconProps["name"]   // Icon to display
  trigger: TriggerTitle | JSX.Element
  children?: JSX.Element    // Expandable content
  hideDetails?: boolean
}

type TriggerTitle = {
  title: string
  titleClass?: string
  subtitle?: string
  subtitleClass?: string
  args?: string[]
  argsClass?: string
  action?: JSX.Element
}
```

**Structure**:
- Collapsible wrapper
- Icon + title/subtitle in trigger
- Optional arrow for expandable content
- Expandable content area

### Icon (Icon System)

Large icon component with extensive icon set.

**Location**: `icon.tsx` (731 lines)
**Icon Source**: SVG sprite sheet

**Sizes**:
- `tiny`, `small`, `medium`, `large`

**Notable Icons** (partial list):
- Navigation: chevron-*, arrow-*, caret-*
- Actions: plus, minus, check, x-close
- Objects: file, folder, document, code
- UI: settings, search, filter, menu
- Status: warning, error, info, success

### DropdownMenu (Complex Menu)

Full-featured dropdown menu system.

**Location**: `dropdown-menu.tsx` (433 lines)

**Sub-components**:
- DropdownMenu.Trigger
- DropdownMenu.Portal
- DropdownMenu.Content
- DropdownMenu.Arrow
- DropdownMenu.Separator
- DropdownMenu.Item
- DropdownMenu.Label
- DropdownMenu.Sub, DropdownMenu.SubTrigger, DropdownMenu.SubContent
- DropdownMenu.RadioGroup, DropdownMenu.RadioItem
- DropdownMenu.CheckboxItem

## Data Flow Patterns

### Context Usage

Components access data through SolidJS context:

```typescript
// In components
const data = useData()
const messages = data.store.message[sessionID]
const parts = data.store.part[messageID]
```

### Props Patterns

1. **Slot-based styling**: `data-slot="name"` for CSS targeting
2. **Component-level styling**: `data-component="name"`
3. **State attributes**: `data-state="open|closed"`
4. **Class merging**: `classList` prop for conditional classes

### Component Registration

Tools are registered dynamically:

```typescript
ToolRegistry.register({
  name: "toolname",
  render(props) {
    return <BasicTool icon="icon" trigger={{...}} />
  }
})
```

## CSS Companion Files

Every TSX component has a corresponding CSS file:

| Component | CSS File | Size |
|-----------|----------|------|
| session-turn.tsx | session-turn.css | 6.6KB |
| message-part.tsx | message-part.css | 3.3KB |
| dropdown-menu.tsx | dropdown-menu.css | 2.9KB |
| tabs.tsx | tabs.css | 4.9KB |
| accordion.tsx | accordion.css | 3.9KB |
| button.tsx | button.css | 3.8KB |
| icon-button.tsx | icon-button.css | 3.5KB |
| checkbox.tsx | checkbox.css | 3.4KB |
| dialog.tsx | dialog.css | 3.1KB |
| list.tsx | list.css | 3.1KB |
| select.tsx | select.css | 2.7KB |
| message-nav.tsx | message-nav.css | 2.6KB |
| collapsible.tsx | collapsible.css | 2.2KB |
| basic-tool.tsx | basic-tool.css | 1.9KB |
| tooltip.tsx | tooltip.css | 1.5KB |
| markdown.tsx | markdown.css | 1.5KB |
| message-progress.tsx | message-progress.css | 1.3KB |
| diff-changes.tsx | diff-changes.css | 1.2KB |
| session-message-rail.tsx | session-message-rail.css | 1.0KB |
| tag.tsx | tag.css | 1.0KB |
| avatar.tsx | avatar.css | 1.0KB |
| session-review.tsx | session-review.css | 2.4KB |
| diff.tsx | diff.css | 0.9KB |
| card.tsx | card.css | 0.9KB |
| select-dialog.tsx | select-dialog.css | 0.9KB |
| input.tsx | input.css | 0.6KB |
| icon.tsx | icon.css | 0.5KB |
| resize-handle.tsx | resize-handle.css | 0.4KB |
| progress-circle.tsx | progress-circle.css | 0.3KB |
| spinner.tsx | spinner.css | 0.1KB |
| typewriter.tsx | typewriter.css | 0.2KB |
| logo.tsx | logo.css | 0.1KB |
| file-icon.tsx | file-icon.css | 0.1KB |
| provider-icon.tsx | provider-icon.css | 0.1KB |
| code.tsx | code.css | 0.05KB |

## External Dependencies

### @kobalte/core
Provides accessible primitives for:
- Accordion, Checkbox, Collapsible
- Dialog, DropdownMenu, Select
- Tabs, Tooltip

### marked + marked-shiki
Markdown rendering with syntax highlighting.

### @pierre/precision-diffs
High-quality diff rendering for file changes.

### virtua
Virtual scrolling for long lists.
