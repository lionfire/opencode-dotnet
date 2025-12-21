# OpenCode Web UI Data Flow

**Source**: `/dv/opencode/packages/ui/`
**Framework**: SolidJS

## State Management Architecture

OpenCode UI uses **SolidJS context** for state management rather than Redux or Zustand. The architecture is:

1. **DataProvider** - Central store for all application data
2. **Context consumers** - Components access data via `useData()` hook
3. **Reactive primitives** - `createMemo`, `createSignal`, `createResource` for derived state

## Core Data Store Structure

```typescript
// src/context/data.tsx
type Data = {
  session: Session[]                              // All sessions
  session_status: { [sessionID: string]: SessionStatus }   // Status per session
  session_diff: { [sessionID: string]: FileDiff[] }        // Diffs per session
  message: { [sessionID: string]: Message[] }              // Messages per session
  part: { [messageID: string]: Part[] }                    // Parts per message
}
```

This is provided via `DataProvider` which wraps the application:

```typescript
const { use: useData, provider: DataProvider } = createSimpleContext({
  name: "Data",
  init: (props: { data: Data; directory: string }) => {
    return { store: props.data, directory: props.directory }
  },
})
```

## Data Flow Patterns

### 1. Session → Messages → Parts

The data hierarchy flows as:

```
Session (sessionID)
  └── Messages (message.parentID links to user message)
        ├── UserMessage (parts stored separately)
        └── AssistantMessage (parts stored separately)
              └── Parts (stored in data.store.part[messageID])
                    ├── TextPart
                    ├── ToolPart
                    └── ReasoningPart
```

### 2. Component Data Access Pattern

Components access data reactively using `createMemo`:

```typescript
// In SessionTurn component
const data = useData()

// Reactive computed values
const messages = createMemo(() =>
  props.sessionID ? (data.store.message[props.sessionID] ?? []) : []
)

const userMessages = createMemo(() =>
  messages()
    .filter((m) => m.role === "user")
    .sort((a, b) => b.id.localeCompare(a.id))
)

const assistantMessages = createMemo(() =>
  messages()?.filter((m) => m.role === "assistant" && m.parentID == userMsg().id)
)

const parts = createMemo(() => data.store.part[userMsg().id])
```

### 3. Status-Based Rendering

Session status determines what's displayed:

```typescript
const status = createMemo(() =>
  data.store.session_status[props.sessionID] ?? { type: "idle" }
)

const working = createMemo(() => status()?.type !== "idle")
const messageWorking = createMemo(() =>
  msg().id === lastUserMessage()?.id && working()
)

// Renders MessageProgress when working, details when done
<Switch>
  <Match when={!completed()}>
    <MessageProgress assistantMessages={assistantMessages} done={!messageWorking()} />
  </Match>
  <Match when={completed() && hasToolPart()}>
    {/* Show/Hide details collapsible */}
  </Match>
</Switch>
```

## Key Data Flow Diagrams

### Sending a Message

```
User Input → [Not shown in UI package]
         ↓
Backend Processing → SSE Stream
         ↓
Data Store Update:
  - data.store.message[sessionID] += new message
  - data.store.session_status[sessionID] = { type: "generating" }
         ↓
Reactive Update:
  - messages() memo recalculates
  - working() becomes true
  - MessageProgress renders
         ↓
Parts Stream:
  - data.store.part[messageID] += new part
  - Part components render incrementally
         ↓
Completion:
  - data.store.session_status[sessionID] = { type: "idle" }
  - completed() becomes true (with delay)
  - Summary section renders
```

### Tool Call Display

```
ToolPart arrives in data.store.part[messageID]
         ↓
AssistantMessageDisplay filters parts:
  - Excludes "reasoning" type
  - Excludes "todoread" tool
         ↓
Part component renders:
  - Looks up PART_MAPPING["tool"]
  - ToolPartDisplay component handles
         ↓
ToolPartDisplay:
  - Checks part.state.status
  - If "error": render error Card
  - Otherwise: lookup ToolRegistry.render(tool.name)
         ↓
Tool Renderer (e.g., "bash"):
  - Receives props: input, metadata, output, tool, hideDetails
  - Returns BasicTool with appropriate icon and content
```

### Diff Rendering Flow

```
Summary includes diffs: msg().summary?.diffs
         ↓
Accordion renders For each diff:
  - Accordion.Item with value={diff.file}
  - DiffChanges shows +/- counts
         ↓
Accordion.Content renders:
  - Uses Dynamic component with useDiffComponent()
  - Passes before/after content with checksums
         ↓
DiffComponent (injected via context):
  - Renders precision diff view
```

## Markdown Rendering Flow

```
Text content (from part.text or summary)
         ↓
Markdown component receives text prop
         ↓
useMarked() gets configured marked instance
         ↓
createResource processes text:
  - strip() removes outer tags
  - marked.parse(markdown) converts to HTML
         ↓
innerHTML={html()} renders result
```

The marked instance is configured with Shiki highlighting:

```typescript
marked.use(
  markedShiki({
    async highlight(code, lang) {
      const highlighter = await getSharedHighlighter({ themes: ["OpenCode"] })
      return highlighter.codeToHtml(code, { lang, theme: "OpenCode" })
    },
  })
)
```

## Filtered List Pattern

For searchable lists (model selection, etc.):

```typescript
const list = useFilteredList({
  items: allItems,              // Static array or async function
  key: (item) => item.id,       // Unique key extractor
  filterKeys: ["name", "id"],   // Fields to search
  current: selectedItem,        // Currently selected
  groupBy: (item) => item.group, // Optional grouping
  onSelect: (item) => { ... },  // Selection handler
})

// Returns:
// - list.grouped() - grouped/filtered items
// - list.filter() - current filter text
// - list.flat() - flat list of filtered items
// - list.active() - currently active item key
// - list.onKeyDown - keyboard handler
// - list.onInput - filter input handler
```

## Animation State Management

For typewriter animations, state is tracked in module-level Maps:

```typescript
// Persists across re-renders
const titleAnimationState = new Map<string, "empty" | "animating" | "done">()

// Logic:
// - "empty": First saw with no value, will animate when value arrives
// - "animating": Currently animating
// - "done": Already animated or first saw with value

const animateTitle = createMemo(() => {
  const state = titleAnimationState.get(msg().id)
  const title = msg().summary?.title

  if (state === "animating") return true
  if (state === "empty" && title) {
    titleAnimationState.set(msg().id, "animating")
    return true
  }
  return false
})
```

## Props vs Context Decision

The codebase follows these patterns:

| Data Type | Delivery | Reason |
|-----------|----------|--------|
| Session/Message data | Context | Global, frequently accessed |
| Component-specific state | Props | Localized, component-specific |
| Configuration | Context | One-time setup (Marked, Diff) |
| UI state (expanded, active) | Local signals | Component-scoped |
| Animation state | Module-level Map | Persist across re-renders |

## Sanitization

The `sanitize` prop flows through for path sanitization:

```typescript
// Creates sanitizer regex from directory
const sanitizer = createMemo(() =>
  data.directory ? new RegExp(`${data.directory}/`, "g") : undefined
)

// Passed down to Message components
<Message message={msg()} parts={parts()} sanitize={sanitizer()} />

// Applied in Part rendering
const part = createMemo(() => sanitizePart(unwrap(props.part), props.sanitize))
```

This removes the working directory from file paths for cleaner display.
