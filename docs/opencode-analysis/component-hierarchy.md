# OpenCode Web UI Component Hierarchy

**Source**: `/dv/opencode/packages/ui/`
**Framework**: SolidJS with @kobalte/core primitives

## Architecture Overview

OpenCode UI uses a **component library pattern** where the `@opencode-ai/ui` package provides reusable components that are consumed by the main web application. The UI is:

- **SolidJS-based**: Uses reactive primitives (signals, stores, memos)
- **Context-driven**: Data flows through SolidJS contexts
- **CSS-in-file**: Each component has companion CSS file
- **Slot-based styling**: Uses `data-slot` attributes for CSS targeting

## Provider/Context Hierarchy

The application wraps components in context providers:

```
DataProvider (session, messages, parts, status)
└── DiffComponentProvider (diff rendering component)
    └── MarkedProvider (markdown configuration)
        └── Application Components
```

### DataProvider

Provides the core application state:

```typescript
type Data = {
  session: Session[]
  session_status: { [sessionID: string]: SessionStatus }
  session_diff: { [sessionID: string]: FileDiff[] }
  message: { [sessionID: string]: Message[] }
  part: { [messageID: string]: Part[] }
}
```

Usage: `const data = useData()` in any component.

### DiffComponentProvider

Allows injection of a diff rendering component. The actual diff component is provided at runtime.

Usage: `const diffComponent = useDiffComponent()` to get the diff renderer.

### MarkedProvider

Configures markdown rendering with Shiki syntax highlighting using the custom "OpenCode" theme.

Usage: `const marked = useMarked()` to get the configured marked instance.

## Main Chat Component Tree

```
SessionTurn (main chat turn display)
├── Message (user message)
│   └── UserMessageDisplay
│       └── Text parts joined
│
├── MessageProgress (during generation)
│   └── Progress indicators
│
├── Summary Section (after completion)
│   ├── Markdown (summary text)
│   └── Accordion (file diffs)
│       └── Accordion.Item (per file)
│           ├── StickyAccordionHeader
│           │   └── Accordion.Trigger
│           │       ├── FileIcon
│           │       ├── File path (directory + filename)
│           │       └── DiffChanges (+/- counts)
│           └── Accordion.Content
│               └── DiffComponent (injected)
│
├── Error Card (if error)
│   └── Card variant="error"
│
└── Collapsible Details (if has tools)
    └── Collapsible
        ├── Collapsible.Trigger ("Show/Hide details")
        └── Collapsible.Content
            └── Message (assistant messages)
                └── AssistantMessageDisplay
                    └── Part (per part)
                        ├── TextPartDisplay → Markdown
                        ├── ReasoningPartDisplay → Markdown
                        └── ToolPartDisplay → BasicTool or registered tool
```

## Part Rendering System

The `MessagePart` component uses a registry pattern for extensible rendering:

```
Part (router)
├── PART_MAPPING["text"] → TextPartDisplay → Markdown
├── PART_MAPPING["reasoning"] → ReasoningPartDisplay → Markdown
└── PART_MAPPING["tool"] → ToolPartDisplay
    └── ToolRegistry.render(tool.name)
        ├── "read" → BasicTool (glasses icon)
        ├── "list" → BasicTool (bullet-list icon)
        ├── "glob" → BasicTool (magnifying-glass-menu icon)
        ├── "grep" → BasicTool (magnifying-glass-menu icon)
        ├── "webfetch" → BasicTool (window-cursor icon)
        ├── "task" → BasicTool (task icon)
        ├── "bash" → BasicTool (console icon) + Markdown output
        ├── "edit" → BasicTool (code-lines icon) + DiffComponent
        ├── "write" → BasicTool (code-lines icon)
        ├── "todowrite" → BasicTool (checklist icon) + Checkbox list
        └── * → GenericTool (mcp icon)
```

### Tool Registration

Tools are registered dynamically:

```typescript
ToolRegistry.register({
  name: "toolname",
  render(props: ToolProps) {
    // props.input - tool input parameters
    // props.metadata - additional metadata
    // props.output - tool output (if completed)
    // props.tool - tool name
    // props.hideDetails - whether to hide expandable content
    return <BasicTool icon="..." trigger={{...}}>...</BasicTool>
  }
})
```

## BasicTool Component Structure

All tools use BasicTool as their base:

```
BasicTool
└── Collapsible
    ├── Collapsible.Trigger
    │   └── div[data-component="tool-trigger"]
    │       ├── Icon (tool icon)
    │       ├── Tool info (title, subtitle, args)
    │       └── Collapsible.Arrow (if expandable)
    └── Collapsible.Content (expandable details)
```

## UI Primitive Component Trees

### Accordion

```
Accordion (Kobalte.Accordion)
└── Accordion.Item
    ├── Accordion.Trigger (header)
    └── Accordion.Content (expandable body)
```

### Collapsible

```
Collapsible (Kobalte.Collapsible)
├── Collapsible.Trigger (toggle button)
├── Collapsible.Arrow (directional indicator)
└── Collapsible.Content (expandable body)
```

### Dialog

```
Dialog (Kobalte.Dialog)
├── Dialog.Trigger (open button)
└── Dialog.Portal
    ├── Dialog.Overlay (backdrop)
    └── Dialog.Content
        └── Dialog.Title, etc.
```

### DropdownMenu

```
DropdownMenu (Kobalte.DropdownMenu)
├── DropdownMenu.Trigger
└── DropdownMenu.Portal
    └── DropdownMenu.Content
        ├── DropdownMenu.Arrow
        ├── DropdownMenu.Label
        ├── DropdownMenu.Separator
        ├── DropdownMenu.Item
        ├── DropdownMenu.CheckboxItem
        ├── DropdownMenu.RadioGroup
        │   └── DropdownMenu.RadioItem
        └── DropdownMenu.Sub
            ├── DropdownMenu.SubTrigger
            └── DropdownMenu.SubContent
```

### Select

```
Select (Kobalte.Select)
├── Select.Trigger (as Button)
│   ├── Select.Value
│   └── Select.Icon
└── Select.Portal
    └── Select.Content
        └── Select.Listbox
            ├── Select.Section (group header)
            └── Select.Item
                ├── Select.ItemLabel
                └── Select.ItemIndicator (checkmark)
```

### Tabs

```
Tabs (Kobalte.Tabs)
├── Tabs.List
│   └── Tabs.Trigger (per tab)
│       └── Tabs.Indicator (active tab indicator)
└── Tabs.Content (per tab)
```

## Icon System

```
Icon
└── svg[data-component="icon"]
    └── use[href="sprite.svg#icon-name"]

ProviderIcon (for AI providers)
└── svg[data-component="provider-icon"]
    └── use[href="sprite.svg#provider-name"]

FileIcon (for file types)
└── svg[data-component="file-icon"]
    └── (determined by file extension mapping)
```

## Data Slot Naming Convention

Components use `data-slot` attributes for CSS targeting:

```
data-component="session-turn"        // Root component identifier
data-slot="session-turn-content"     // Semantic slot within component
data-slot="session-turn-message-header"
data-slot="session-turn-message-title"
data-slot="session-turn-summary-section"
...
```

This allows CSS to target specific parts without tight coupling:

```css
[data-component="session-turn"] [data-slot="session-turn-title"] {
  /* styles */
}
```

## State Attributes

Components expose state via data attributes:

```
data-state="open"      // Collapsible/accordion state
data-state="closed"
data-active="true"     // List item active state
data-completed="true"  // Todo completion state
data-diffs="true"      // Has diffs indicator
data-fade="true"       // Animation state
```
