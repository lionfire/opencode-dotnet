# Blazor Component Inventory - AgUi.IDE.BlazorServer

**Project**: `/src/opencode-dotnet/examples/AgUi.IDE.BlazorServer/`
**Framework**: Blazor Server with MudBlazor
**Total Components**: 16 Razor files

## Component Categories

### Layout Components

| Component | File | Description |
|-----------|------|-------------|
| App | App.razor | Root application component |
| Routes | Routes.razor | Router configuration |
| MainLayout | Layout/MainLayout.razor | Main layout wrapper |
| IdeLayout | Layout/IdeLayout.razor | **IDE-style multi-pane layout** - File tree, chat, changes, terminal |

### Pages

| Component | File | Description |
|-----------|------|-------------|
| Home | Pages/Home.razor | Landing page |
| IDE | Pages/IDE.razor | IDE page combining all panels |

### Chat Components

| Component | File | Description |
|-----------|------|-------------|
| ChatPanel | Shared/Chat/ChatPanel.razor | **Main chat container** - Message list + input |
| ChatInput | Shared/Chat/ChatInput.razor | **Rich input area** - Agent selector, model dialog |
| ChatMessage | Shared/Chat/ChatMessage.razor | **Message display** - Text, code, tools, thinking |

### Tool & Permission Components

| Component | File | Description |
|-----------|------|-------------|
| ToolExecutionPanel | Shared/Tools/ToolExecutionPanel.razor | Active tool execution display |
| PermissionPanel | Shared/Permissions/PermissionPanel.razor | Permission request UI |

### Utility Components

| Component | File | Description |
|-----------|------|-------------|
| DiffViewer | Shared/DiffViewer/DiffViewer.razor | File diff display |
| FileTree | Shared/FileTree/FileTree.razor | Directory tree navigation |
| FileTreeNodeView | Shared/FileTree/FileTreeNodeView.razor | Individual tree node |
| TerminalPanel | Shared/Terminal/TerminalPanel.razor | Terminal output display |

## Key Component Details

### IdeLayout (Main Layout)

Multi-panel IDE layout with resizable sections.

**Features**:
- Left sidebar: File tree (collapsible)
- Center: Chat panel
- Right: Changes/diff viewer (collapsible)
- Bottom: Terminal (collapsible)
- Resize handles (not fully implemented)

**Render Fragments**: LeftPanel, CenterPanel, RightPanel, BottomPanel

### ChatInput (Input Area)

Full-featured chat input with toolbars.

**Features**:
- Textarea with placeholder
- Agent selector dropdown
- Model/provider selection dialog
- Send button with keyboard support (Enter to send)
- Focus ring styling

**State**:
- `_inputText` - Current input
- `_showModelDialog` - Dialog visibility
- `_showAgentMenu` - Menu visibility
- `_selectedProviderId` - Provider filter

### ChatMessage (Message Display)

Renders individual chat messages with multiple part types.

**Part Types Supported**:
- `Text` - Markdown rendered with Markdig
- `CodeBlock` - Syntax-highlighted code with copy button
- `Thinking` - Collapsible reasoning display
- `ToolCall` - Tool execution display

**Features**:
- Avatar with role-based icon (Person/SmartToy)
- Timestamp display
- Streaming cursor animation
- Collapsible tool calls with input/output

### ToolExecutionPanel

Shows currently active tool executions.

**Features**:
- Lists active tools (up to 5)
- Shows tool state (pending, running, completed, error)
- Permission-waiting indicator
- Progress bar for running tools

### PermissionPanel

Handles permission requests.

**Features**:
- Lists pending permissions
- Shows permission type and pattern
- Three response buttons: Allow Once, Always Allow, Deny
- Keyboard hints (Enter, A, D)

### DiffViewer

Displays file changes.

**Features**:
- File tab selection
- Line-by-line diff display
- Addition/deletion markers
- Hunk headers
- Sample diff on init

## Missing Features Compared to OpenCode

### Not Implemented

1. **No Session Management** - No session list/history
2. **No Typewriter Animation** - Text appears instantly
3. **No Message Navigation** - No MessageNav equivalent
4. **No Virtual Scrolling** - Lists not virtualized
5. **No Tool Registry** - Tools hardcoded in ChatMessage
6. **Limited Tool Displays** - No specialized per-tool UI
7. **No Session Review** - No review/summary view
8. **No Sticky Headers** - No StickyAccordionHeader
9. **Limited Keyboard Navigation** - Basic Enter/Escape only
10. **No Icon Sprite System** - Uses MudBlazor icons

### Partial Implementations

1. **Model Dialog** - Works but styling differs
2. **Tool Call Display** - Basic compared to OpenCode's per-tool renderers
3. **Diff Viewer** - Simpler than @pierre/precision-diffs
4. **Markdown** - Uses Markdig, no Shiki highlighting

## Data Sources

### Hardcoded/Mock Data

1. **ChatInput placeholder** - Hardcoded example text
2. **DiffViewer** - Uses sample diff on initialization
3. **FileTree** - Connects to FileTreeService (may return mock)
4. **Agent list** - From IdeStateService (populated from API)
5. **Provider/Model list** - From IdeStateService (populated from API)

### Real Data Integration

1. **ChatService** - Messages, tool executions
2. **PermissionService** - Permission requests
3. **IdeStateService** - Session state, providers, models
4. **OpenCodeIdeService** - API integration

## Styling Approach

### CSS Strategy

- Component-scoped `<style>` blocks
- CSS custom properties (variables)
- MudBlazor components for common UI

### Variables Used

```css
--opencode-bg-base: #131010
--opencode-bg-raised: #1b1818
--opencode-bg-raised-stronger: #1e1a1a
--opencode-border-base: #333
--opencode-border-strong: #3d3d3d
--opencode-text-strong: #f5f5f4
--opencode-text-base: #e7e5e4
--opencode-text-weak: #a8a29e
--opencode-text-weaker: #78716c
--opencode-accent: #4ade80
--mud-palette-primary: #7c3aed
```

## Component Dependencies

### MudBlazor Usage

| Component | MudBlazor Used |
|-----------|----------------|
| ChatPanel | MudIcon, MudText |
| ChatInput | (custom HTML only) |
| ChatMessage | MudIcon, MudIconButton, MudSpacer, MudProgressLinear |
| ToolExecutionPanel | MudIcon, MudChip, MudSpacer, MudProgressLinear, MudText |
| PermissionPanel | MudIcon, MudButton, MudText |
| DiffViewer | MudIcon, MudChip, MudText, MudSpacer, MudProgressCircular |
| FileTree | MudProgressCircular, MudText |
| IdeLayout | MudIcon, MudIconButton, MudStack, MudChip |
