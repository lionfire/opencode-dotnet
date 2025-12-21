# OpenCode Web UI Study Notes

> **Date**: 2025-12-12
> **Purpose**: Document observations from studying the OpenCode web UI source code
> **Source**: `/dv/opencode/packages/ui/` and `/dv/opencode/packages/desktop/`

---

## 1. Technology Stack

### Frontend Framework
- **SolidJS** (not React!) - Fine-grained reactivity with signals
- **Vite** - Build tool and dev server
- **Tailwind CSS v4** - Utility-first CSS framework
- **Kobalte** - Headless UI components for SolidJS (similar to Radix for React)

### Key Libraries
- `@solidjs/router` - Routing
- `solid-js/store` - State management
- `@opencode-ai/sdk/v2` - OpenCode SDK types
- `ghostty-web` - Terminal emulator (likely)

### Fonts
- **Geist** - Sans-serif for UI text
- **Berkeley Mono** - Monospace for code

---

## 2. Application Architecture

### Package Structure
```
packages/
├── ui/           # Reusable UI components (library)
│   └── src/
│       ├── components/    # All UI components
│       ├── context/       # React-style context providers
│       ├── hooks/         # Custom hooks
│       └── styles/        # CSS files
├── desktop/      # Desktop application (electron-like)
│   └── src/
│       ├── components/    # App-specific components
│       ├── context/       # App-specific contexts
│       ├── pages/         # Page components
│       └── hooks/         # App-specific hooks
└── web/          # Web-specific entry point
```

### Key Design Patterns

1. **Component-based architecture** with clear separation
2. **Context-based state management** (useData, useSession, useLocal)
3. **Custom hooks** for reusable logic (useFilteredList, useProviders)
4. **Reactive signals** for fine-grained updates
5. **Slot-based styling** using `data-slot` attributes

---

## 3. Layout Structure

### Main Layout (`layout.tsx`)
```
┌──────────────────────────────────────────────────────────────┐
│                        Header / Title Bar                     │
├────────────┬─────────────────────────────────────────────────┤
│            │                                                  │
│  Sidebar   │              Main Content Area                   │
│  (Files)   │                                                  │
│            │  ┌─────────────────────────────────────────┐    │
│            │  │         Session/Chat Area               │    │
│            │  │                                         │    │
│            │  │  - Session turns (messages)             │    │
│            │  │  - Tool visualizations                  │    │
│            │  │  - Diff viewers                         │    │
│            │  │                                         │    │
│            │  └─────────────────────────────────────────┘    │
│            │                                                  │
│            │  ┌─────────────────────────────────────────┐    │
│            │  │         Prompt Input Area               │    │
│            │  │  - Text input with file pills           │    │
│            │  │  - Model selector                       │    │
│            │  │  - Agent selector                       │    │
│            │  │  - Send button                          │    │
│            │  └─────────────────────────────────────────┘    │
├────────────┴─────────────────────────────────────────────────┤
│                      Terminal Panel (collapsible)             │
└──────────────────────────────────────────────────────────────┘
```

---

## 4. Key Components Analysis

### SessionTurn (`session-turn.tsx`) - ~300 lines

**Purpose**: Renders a complete user message + assistant response turn

**Key Features**:
- Animation state tracking for typewriter effects
- Title rendering with optional animation
- Summary section with markdown
- Accordion for file diffs
- Progress indicator during generation
- Collapsible "Show details" for tool calls
- Error display

**Props**:
```typescript
interface SessionTurnProps {
  sessionID: string
  messageID: string
  classes?: {
    root?: string
    content?: string
    container?: string
  }
}
```

**Data Flow**:
- Gets data from `useData()` context
- Gets diff component from `useDiffComponent()` context
- Filters messages by sessionID and role
- Tracks animation state in module-level Maps

### MessagePart (`message-part.tsx`) - ~440 lines

**Purpose**: Renders individual message parts (text, tools, reasoning)

**Key Features**:
- Part type registry (`PART_MAPPING`)
- Tool registry for custom tool visualizations
- Built-in tool renderers: read, list, glob, grep, webfetch, task, bash, edit, write, todowrite

**Tool Visualization Pattern**:
```typescript
ToolRegistry.register({
  name: "bash",
  render(props) {
    return (
      <BasicTool icon="console" trigger={{ title: "Shell", subtitle: props.input.description }}>
        <Markdown text={`\`\`\`command\n$ ${props.input.command}\n\`\`\``} />
      </BasicTool>
    )
  },
})
```

### PromptInput (`prompt-input.tsx`) - ~730 lines

**Purpose**: Rich text input with file attachments and model selection

**Key Features**:
- ContentEditable-based input (not textarea!)
- File pill insertion with `@` prefix autocomplete
- Cursor position tracking
- Model/agent selection dropdowns
- Rotating placeholder suggestions
- Keyboard shortcuts (Enter to send, Escape to cancel)
- File attachment handling

**Input Structure**:
```typescript
type ContentPart =
  | { type: "text"; content: string; start: number; end: number }
  | { type: "file"; path: string; content: string; start: number; end: number }
```

---

## 5. Theme System

### CSS Custom Properties (650+ variables!)

**Color Palettes** (from `colors.css`):
- `--smoke-*` - Neutral grays (warm tint)
- `--ink-*` - Neutral grays (cool tint)
- `--yuzu-*` - Yellow/lime accent
- `--cobalt-*` - Blue accent (interactive)
- `--apple-*` - Green (success)
- `--ember-*` - Red/orange (error/critical)
- `--solaris-*` - Yellow (warning)
- `--lilac-*` - Purple (info)
- `--coral-*` - Pink/coral
- `--mint-*` - Light green (diff add)
- `--blue-*` - Blue

Each palette has:
- `-dark-1` through `-dark-12` (dark mode shades)
- `-light-1` through `-light-12` (light mode shades)
- `-dark-alpha-*` (with transparency)
- `-light-alpha-*` (with transparency)

### Semantic Variables (from `theme.css`)

**Backgrounds**:
- `--background-base` - Main background
- `--background-weak` - Slightly different bg
- `--background-strong` - Emphasized bg
- `--surface-*` - Component surfaces (base, hover, active, raised, etc.)

**Text**:
- `--text-base` - Normal text
- `--text-weak` - De-emphasized text
- `--text-weaker` - Very subtle text
- `--text-strong` - Bold/important text
- `--text-interactive-*` - Clickable text
- `--text-on-*` - Text on colored backgrounds

**Borders**:
- `--border-base`, `--border-weak`, `--border-strong`
- `--border-interactive-*`
- `--border-success/warning/critical/info-*`

**Icons**:
- `--icon-base`, `--icon-weak`, `--icon-strong`
- `--icon-interactive-*`
- `--icon-success/warning/critical/info-*`

**Syntax Highlighting**:
- `--syntax-comment`, `--syntax-string`, `--syntax-keyword`
- `--syntax-primitive`, `--syntax-variable`, `--syntax-property`
- `--syntax-type`, `--syntax-constant`, `--syntax-punctuation`

**Diff Colors**:
- `--surface-diff-add-*`, `--text-diff-add-*`, `--icon-diff-add-*`
- `--surface-diff-delete-*`, `--text-diff-delete-*`, `--icon-diff-delete-*`
- `--surface-diff-hidden-*` (collapsed/context lines)

### Typography Variables (from `theme.css`)

```css
--font-family-sans: "Geist", "Geist Fallback";
--font-family-mono: "Berkeley Mono", "Berkeley Mono Fallback";

--font-size-small: 12px;
--font-size-base: 14px;
--font-size-large: 16px;
--font-size-x-large: 20px;

--font-weight-regular: 400;
--font-weight-medium: 500;

--line-height-large: 20px;
--line-height-x-large: 24px;
--line-height-2x-large: 30px;

--spacing: 0.25rem;  /* 4px base unit */
```

### Theme Variants

1. **OC-1 (default)** - Warm gray (smoke) palette
   - Light mode by default
   - Dark mode via `@media (prefers-color-scheme: dark)`

2. **OC-2-paper** - Cool gray (ink) palette
   - Applied via `html[data-theme="oc-2-paper"]`

---

## 6. Styling Patterns

### Data Attributes for Slots
Components use `data-slot` attributes for styling targets:
```tsx
<div data-slot="session-turn-message-header">
<div data-slot="session-turn-message-title">
```

CSS can target these:
```css
[data-slot="session-turn-message-title"] { ... }
```

### Tailwind Classes
Heavy use of Tailwind utilities:
```tsx
<div class="relative size-full flex flex-col gap-3">
<div class="w-full flex items-center justify-between rounded-md">
```

### Component Variants
Components often have variant props:
```tsx
<Card variant="error">
<Button variant="ghost">
<Collapsible variant="ghost">
```

---

## 7. State Management

### Context Providers

1. **useData()** - Global data store
   - `data.store.message[sessionID]` - Messages by session
   - `data.store.part[messageID]` - Parts by message
   - `data.store.session_status[sessionID]` - Session status
   - `data.directory` - Current working directory

2. **useSession()** - Current session state
   - `session.id` - Current session ID
   - `session.prompt` - Prompt state (current, dirty, set)
   - `session.layout` - Tab management
   - `session.messages` - Message management
   - `session.working()` - Is session currently processing

3. **useLocal()** - Local/persisted state
   - `local.agent` - Selected agent
   - `local.model` - Selected model
   - `local.file` - File browser state

4. **useLayout()** - UI layout state
   - `layout.dialog` - Dialog open/close management

### Reactive Patterns

SolidJS signals and memos for reactivity:
```typescript
const [placeholder, setPlaceholder] = createSignal(0)
const messages = createMemo(() => data.store.message[props.sessionID] ?? [])

createEffect(() => {
  // React to changes
})
```

---

## 8. Communication Patterns

### SDK Client
```typescript
const sdk = useSDK()

// Create session
const created = await sdk.client.session.create()

// Send prompt
sdk.client.session.prompt({
  sessionID: existing.id,
  agent: local.agent.current()!.name,
  model: { modelID, providerID },
  parts: [{ type: "text", text }],
})

// Abort current operation
sdk.client.session.abort({ sessionID: session.id! })
```

### Data Sync
Uses `useSync()` context for syncing with backend:
- `sync.absolute(path)` - Convert relative to absolute path

---

## 9. Animation & UX

### Typewriter Effect
Text can animate in character-by-character:
```tsx
<Typewriter as="h1" text={msg().summary?.title} />
```

### Progress Indicators
```tsx
<MessageProgress assistantMessages={assistantMessages} done={!messageWorking()} />
```

### Transitions
Uses CSS transitions and Tailwind animations:
- Fade in/out for summaries
- Accordion expand/collapse
- Button state changes

---

## 10. File Attachments

### @ Mention System
1. User types `@` in prompt input
2. Autocomplete popover shows matching files
3. On selection, file path is inserted as a "pill"
4. Pills are rendered as `<span data-type="file" data-path="...">`
5. On submit, files are converted to attachment parts:
```typescript
{
  type: "file",
  mime: "text/plain",
  url: `file://${absolute}${query}`,
  filename: getFilename(path),
  source: { type: "file", path: absolute }
}
```

---

## 11. Diff Viewer

### Diff Display
- Shows before/after with syntax highlighting
- Uses accordion to expand/collapse per file
- Shows +/- line counts via `<DiffChanges>` component
- Dynamic component injection via `useDiffComponent()` context
- Checksum-based caching for performance

### Diff Data Structure
```typescript
interface Diff {
  file: string
  before: string
  after: string
}
```

---

## 12. Tool Visualization

### Built-in Tools
| Tool | Icon | Display |
|------|------|---------|
| read | glasses | "Read" + filename |
| list | bullet-list | "List" + directory |
| glob | magnifying-glass-menu | "Glob" + pattern |
| grep | magnifying-glass-menu | "Grep" + pattern |
| webfetch | window-cursor | "Webfetch" + URL |
| task | task | Agent type + description |
| bash | console | "Shell" + command + output |
| edit | code-lines | "Edit" + path + diff |
| write | code-lines | "Write" + path |
| todowrite | checklist | "To-dos" + count |

### Custom Tool Registration
```typescript
ToolRegistry.register({
  name: "custom_tool",
  render(props) {
    return <BasicTool icon="..." trigger={{ title: "...", subtitle: "..." }}>
      {/* Content */}
    </BasicTool>
  }
})
```

---

## 13. Key Observations for Blazor Implementation

### Must Replicate
1. **Color system** - All 650+ CSS variables
2. **Dark/light mode** - Media query based
3. **Typewriter animation** - For titles and summaries
4. **Tool visualizations** - Each tool type
5. **File pill system** - @ mentions
6. **Diff accordion** - Expandable file changes
7. **Progress states** - During generation

### SolidJS → Blazor Translations
| SolidJS | Blazor |
|---------|--------|
| `createSignal()` | `@bind` / field |
| `createMemo()` | Computed property / `StateHasChanged` |
| `createEffect()` | `OnParametersSet` / `StateHasChanged` |
| `For each=` | `@foreach` |
| `Show when=` | `@if` |
| `Switch/Match` | `@switch` |
| `useContext()` | Cascading values / DI |
| `createStore()` | Fluxor / State container |

### MudBlazor Mappings
| OpenCode Component | MudBlazor Equivalent |
|-------------------|---------------------|
| Accordion | MudExpansionPanels |
| Button | MudButton |
| Card | MudCard |
| Checkbox | MudCheckBox |
| Collapsible | MudExpansionPanel |
| Dialog | MudDialog |
| Icon | MudIcon |
| IconButton | MudIconButton |
| Input | MudTextField |
| List | MudList |
| Select | MudSelect |
| Tag | MudChip |
| Tooltip | MudTooltip |

### Custom Components Needed
- SessionTurn - No direct equivalent
- MessagePart - No direct equivalent
- PromptInput - Needs custom implementation
- FileTree - MudTreeView base
- DiffViewer - Custom Monaco integration
- TerminalPanel - Custom XTerm.js integration
- BasicTool - Custom card-like component

---

## 14. Screenshots Reference

To capture screenshots for reference:
1. Run `opencode web --port 4096` in a project
2. Open http://localhost:4096
3. Capture:
   - Main chat view (empty)
   - Chat with messages
   - Tool execution in progress
   - Diff accordion expanded
   - File tree open
   - Model selector open
   - Terminal panel

---

## Next Steps

1. Create COMPONENT-MAPPING.md with detailed Blazor equivalents
2. Extract theme CSS variables to theme.css
3. Set up MudBlazor with custom theme matching OpenCode
4. Create component stubs with proper parameters
5. Implement basic layout structure
