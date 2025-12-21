# OpenCode Web UI - Analysis for Blazor Replication

> **Source:** `/dv/opencode/packages/desktop/` (SolidJS implementation)
> **Goal:** Faithfully replicate in Blazor with MudBlazor
> **Created:** 2025-12-11

## Executive Summary

OpenCode's `opencode web` is a **sophisticated SolidJS-based desktop web application** with multi-panel layout, real-time collaboration, terminal integration, and rich code editing features.

**Our mission:** Replicate this UI faithfully in Blazor for `AgUi.IDE.*` samples.

---

## Technology Stack (Original)

| Layer | Technology |
|-------|------------|
| **Framework** | SolidJS (reactive, fine-grained reactivity) |
| **Build** | Vite |
| **UI Components** | @kobalte/core (headless components) + custom |
| **Styling** | Custom CSS + Tailwind utilities |
| **Terminal** | ghostty-web (WebAssembly terminal emulator) |
| **Markdown** | marked + marked-shiki |
| **Syntax Highlighting** | shiki |
| **State** | solid-js/store (reactive stores) |
| **Routing** | @solidjs/router |
| **Backend** | Hono.js server (REST + WebSocket + SSE) |

---

## UI Layout (To Replicate)

### Main Desktop Layout

```
┌────────────────────────────────────────────────────────────────┐
│ HEADER (12px height)                                           │
│ [Project Selector ▼] [Session Selector ▼] [...] [Terminal ⚡]  │
├────────┬───────────────────────────────────────────────────────┤
│        │                                                        │
│ SIDE   │  MAIN CONTENT AREA                                    │
│ BAR    │  ┌──────────────────────────────────────────────────┐ │
│        │  │ CHAT MESSAGES (scrollable)                       │ │
│ 48-300 │  │                                                  │ │
│ px     │  │ • User message                                   │ │
│ width  │  │ • Assistant response (streaming)                │ │
│        │  │ • Tool calls (collapsible)                       │ │
│ File   │  │ • Diffs (syntax highlighted)                     │ │
│ Tree   │  │                                                  │ │
│ (drag  │  └──────────────────────────────────────────────────┘ │
│ drop)  │  ┌──────────────────────────────────────────────────┐ │
│        │  │ FILE TABS (draggable)                            │ │
│ Resiz  │  │ [App.cs x] [README.md x] [+]                     │ │
│ able   │  ├──────────────────────────────────────────────────┤ │
│        │  │ FILE VIEWER/EDITOR                               │ │
│        │  │ (syntax highlighted, diff view)                  │ │
│        │  └──────────────────────────────────────────────────┘ │
│        │  ┌──────────────────────────────────────────────────┐ │
│        │  │ PROMPT INPUT (with autocomplete)                 │ │
│        │  │ Type @ for file refs, / for commands             │ │
│        │  └──────────────────────────────────────────────────┘ │
├────────┼───────────────────────────────────────────────────────┤
│        │  TERMINAL (toggleable, resizable)                     │
│        │  Multiple tabs, WebSocket-based, ghostty renderer    │
└────────┴───────────────────────────────────────────────────────┘
```

### Responsive Behavior

- **< 768px:** Sidebar collapses to overlay
- **768-1024px:** Compact layout
- **> 1024px:** Full multi-panel experience

---

## Key Components to Replicate

### 1. Session Turn (`session-turn.tsx`)

**What it does:**
- Displays one user-assistant exchange
- Collapsible sections for tool calls
- Syntax highlighted code blocks
- File diffs with line numbers
- Cost and token usage display

**Blazor equivalent:**
```razor
@* SessionTurn.razor *@
<div class="session-turn" data-role="@Message.Role">
    <div class="turn-header">
        <Avatar Role="@Message.Role" />
        <Timestamp Value="@Message.Timestamp" />
        <Show Condition="@(Message.Role == "assistant")">
            <TokenUsage Input="@Message.TokensInput" Output="@Message.TokensOutput" />
        </Show>
    </div>

    <div class="turn-parts">
        <For Items="@Message.Parts">
            <MessagePart Part="@context" />
        </For>
    </div>
</div>
```

### 2. Message Part (`message-part.tsx`)

**What it does:**
- Renders different part types:
  - `text` - Markdown with syntax highlighting
  - `tool` - Tool invocation with args/result
  - `code` - Code block with language
  - `diff` - File diff visualization
  - `error` - Error messages
  - `file` - File attachments

**Blazor equivalent:**
```razor
@* MessagePart.razor *@
@switch (Part.Type)
{
    case "text":
        <MarkdownRenderer Content="@Part.Text" />
        break;

    case "tool":
        <ToolCallViewer
            Tool="@Part.Tool"
            Input="@Part.Input"
            Output="@Part.Output"
            Status="@Part.Status" />
        break;

    case "diff":
        <DiffViewer Changes="@Part.Changes" />
        break;

    // ... etc
}
```

### 3. Diff Viewer (`diff-changes.tsx`, `diff.tsx`)

**What it does:**
- Shows unified diffs with syntax highlighting
- Line numbers
- +/- indicators
- Collapsible file sections
- Filename with file icon

**Uses:** `@pierre/precision-diffs` for diff parsing

**Blazor equivalent:**
```razor
@* DiffViewer.razor *@
<MudExpansionPanels MultiExpansion="true">
    <For Items="@Diffs">
        <MudExpansionPanel>
            <TitleContent>
                <div class="diff-file-header">
                    <FileIcon Path="@context.Path" />
                    <span>@context.Path</span>
                    <span class="diff-stats">+@context.Additions -@context.Deletions</span>
                </div>
            </TitleContent>
            <ChildContent>
                <CodeBlock
                    Language="diff"
                    Code="@context.UnifiedDiff"
                    ShowLineNumbers="true" />
            </ChildContent>
        </MudExpansionPanel>
    </For>
</MudExpansionPanels>
```

### 4. File Tree (`file-tree.tsx`)

**Features:**
- Hierarchical tree view
- Expand/collapse folders
- File icons by type
- Drag-drop to chat
- Modified file indicators
- Click to open in editor

**Blazor equivalent:**
```razor
@* FileTree.razor *@
<MudTreeView T="FileNode" Items="@RootNodes">
    <ItemTemplate>
        <MudTreeViewItem @bind-Expanded="@context.IsExpanded"
                         Icon="@GetFileIcon(context)"
                         OnClick="@(() => OpenFile(context))">
            <Content>
                <div class="file-node" draggable="true">
                    <FileIcon Path="@context.Path" />
                    <span class="file-name">@context.Name</span>
                    <Show Condition="@context.IsModified">
                        <span class="modified-indicator">●</span>
                    </Show>
                </div>
            </Content>
        </MudTreeViewItem>
    </ItemTemplate>
</MudTreeView>
```

### 5. Prompt Input (`prompt-input.tsx`)

**Features:**
- Multi-line textarea with auto-resize
- `@` autocomplete for file references
- `/` autocomplete for commands
- Keyboard shortcuts (Cmd+Enter to send)
- File attachment via drag-drop
- Model selector dropdown

**Blazor equivalent:**
```razor
@* PromptInput.razor *@
<div class="prompt-input-container">
    <div class="prompt-toolbar">
        <ModelSelector @bind-Selected="@SelectedModel" />
        <IconButton Icon="@Icons.Material.Filled.AttachFile" OnClick="@AttachFile" />
    </div>

    <MudTextField @bind-Value="@InputText"
                  Lines="@CalculatedLines"
                  Variant="Variant.Outlined"
                  Placeholder="Type @ for files, / for commands..."
                  OnKeyDown="@HandleKeyDown"
                  Immediate="true" />

    <Show Condition="@ShowAutocomplete">
        <AutocompletePopup
            Items="@AutocompleteItems"
            OnSelect="@InsertCompletion" />
    </Show>

    <div class="prompt-actions">
        <MudButton OnClick="@SendMessage"
                   Disabled="@IsSending"
                   Color="Color.Primary">
            Send (Ctrl+Enter)
        </MudButton>
    </div>
</div>
```

### 6. Terminal (`terminal.tsx`)

**Features:**
- Full xterm.js-like terminal emulator
- WebSocket connection to backend PTY
- Multiple terminal tabs
- Resize support
- Buffer persistence
- Copy/paste support

**Uses:** `ghostty-web` (WebAssembly terminal)

**Blazor equivalent:**
```razor
@* Terminal.razor *@
@inject IJSRuntime JS

<div class="terminal-container">
    <MudTabs @bind-ActivePanelIndex="@ActiveTab">
        <For Items="@Terminals">
            <MudTabPanel Text="@context.Title">
                <div @ref="@context.TerminalElement"
                     class="terminal-content"></div>
            </MudTabPanel>
        </For>
        <MudTabPanel>
            <TitleContent>
                <MudIconButton Icon="@Icons.Material.Filled.Add"
                               Size="Size.Small"
                               OnClick="@CreateTerminal" />
            </TitleContent>
        </MudTabPanel>
    </MudTabs>
</div>

@code {
    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            // Initialize xterm.js via JS interop
            await JS.InvokeVoidAsync("initTerminal", TerminalElement);
        }
    }
}
```

---

## Visual Design System (To Match)

### Colors

**Light Theme:**
```css
--surface-primary: hsl(0, 20%, 99%);
--surface-secondary: hsl(0, 15%, 96%);
--text-primary: hsl(0, 9%, 7%);
--text-secondary: hsl(0, 9%, 40%);
--border: hsl(0, 12%, 90%);
```

**Dark Theme:**
```css
--surface-primary: hsl(0, 9%, 7%);
--surface-secondary: hsl(0, 9%, 10%);
--text-primary: hsl(0, 20%, 99%);
--text-secondary: hsl(0, 9%, 60%);
--border: hsl(0, 9%, 15%);
```

### Typography

- **Font:** IBM Plex Mono (monospace for code)
- **Sizes:** 12px (code), 14px (body), 16px (headers)

### Spacing

- **Panel gaps:** 12px
- **Message spacing:** 16px vertical
- **Component padding:** 12px

---

## MudBlazor Component Mapping

| OpenCode Component | MudBlazor Equivalent |
|-------------------|---------------------|
| `dropdown-menu.tsx` | `MudMenu` |
| `tabs.tsx` | `MudTabs` |
| `accordion.tsx` | `MudExpansionPanels` |
| `dialog.tsx` | `MudDialog` |
| `button.tsx` | `MudButton` |
| `select.tsx` | `MudSelect` |
| `checkbox.tsx` | `MudCheckBox` |
| `tooltip.tsx` | `MudTooltip` |
| `input.tsx` | `MudTextField` |
| File tree | `MudTreeView` |
| Terminal | Custom (xterm.js interop) |
| Code editor | Monaco.Blazor or AvaloniaEdit |

---

## Implementation Strategy

### Phase 1: Core Layout Components

Create in `LionFire.OpenCode.Blazor/Components/`:

1. **DesktopLayout.razor** - Main layout matching OpenCode's structure
2. **SessionTurn.razor** - Message display (user/assistant)
3. **MessagePart.razor** - Part renderer (text, tool, diff, code)
4. **DiffViewer.razor** - File diff visualization

### Phase 2: Interactive Components

5. **FileTree.razor** - Sidebar file browser
6. **PromptInput.razor** - Chat input with autocomplete
7. **FileTabs.razor** - Draggable file tab system
8. **ToolCallViewer.razor** - Tool invocation display

### Phase 3: Advanced Features

9. **Terminal.razor** - xterm.js integration
10. **SessionSelector.razor** - Session history/switching
11. **ModelSelector.razor** - Provider/model selection
12. **CostTracker.razor** - Token usage and cost display

### Phase 4: Polish

13. **Theme system** - Light/dark mode matching OpenCode
14. **Keyboard shortcuts** - Cmd+K, Cmd+Enter, etc.
15. **Auto-scroll** - Smart scroll-to-bottom
16. **Drag-drop** - Files into chat

---

## Key Features to Replicate

### Must-Have (Core Experience)

- ✅ **Session-based chat** - Multi-turn conversations
- ✅ **Message streaming** - Token-by-token display
- ✅ **Tool call visualization** - Collapsible, syntax highlighted
- ✅ **Diff display** - File changes with line numbers
- ✅ **File browser** - Tree view with icons
- ✅ **Prompt input** - @ autocomplete for files

### Should-Have (Rich Experience)

- ✅ **Terminal integration** - WebSocket PTY
- ✅ **File tabs** - Multiple open files
- ✅ **Syntax highlighting** - Code blocks and diffs
- ✅ **Model selection** - Provider/model dropdown
- ✅ **Cost tracking** - Tokens and cost display
- ✅ **Session history** - Browse past sessions

### Nice-to-Have (Polish)

- ✅ **Keyboard shortcuts** - Power user features
- ✅ **Drag-drop files** - Into chat for context
- ✅ **Theme customization** - Match OpenCode's themes
- ✅ **Auto-scroll** - Smart bottom detection
- ✅ **Copy buttons** - On code blocks

---

## Component Details

### SessionTurn Component

**From OpenCode:** `session-turn.tsx` (14KB, 400+ lines)

**Key features:**
- Role indicator (user/assistant)
- Timestamp display
- Model badge
- Parts list (text, tools, diffs)
- Collapsible sections
- Message actions (copy, regenerate)

**Data structure:**
```typescript
type MessageTurn = {
  id: string;
  role: "user" | "assistant";
  time: { created: number; completed?: number };
  parts: Part[];
  modelID?: string;
  providerID?: string;
  tokens?: { input: number; output: number; reasoning: number };
  cost?: number;
}
```

**Blazor implementation notes:**
- Use `MudCard` for message container
- `MudChip` for model/provider badges
- `MudExpansionPanel` for collapsible tool calls
- Custom CSS to match OpenCode's visual style

### Diff Viewer Component

**From OpenCode:** `diff-changes.tsx` + `diff.tsx`

**Features:**
- Unified diff format
- Syntax highlighting via Shiki
- Line numbers
- +/- indicators with colors
- File path with icon
- Expand/collapse

**Blazor implementation:**
- Parse unified diff format
- Use Shiki .NET port or Monaco for highlighting
- `MudExpansionPanel` for each file
- Custom CSS for diff colors (green for +, red for -)

### Terminal Component

**From OpenCode:** Uses `ghostty-web` (WASM-based terminal)

**Communication:**
- WebSocket: `ws://localhost:4096/pty/{id}/connect`
- Binary protocol for terminal I/O
- Resize events
- Buffer persistence

**Blazor implementation:**
- Use `xterm.js` via JS interop (more mature for web)
- Or: `XtermBlazor` NuGet package
- WebSocket from Blazor to OpenCode PTY endpoint
- Terminal.Gui for TUI variant (separate)

---

## Sample Updates

### AgUi.IDE.BlazorServer

**Updated goal:** Replicate `opencode web` faithfully

**Components to implement:**
```
Components/
├── Layout/
│   └── OpenCodeLayout.razor         # Main layout (sidebar, panels)
├── Session/
│   ├── SessionTurn.razor            # User/assistant exchange
│   ├── MessagePart.razor            # Part renderer
│   └── SessionList.razor            # Session history
├── Files/
│   ├── FileTree.razor               # Sidebar file browser
│   ├── FileTabs.razor               # Tab bar
│   └── FileViewer.razor             # Code viewer
├── Diffs/
│   ├── DiffViewer.razor             # File diff display
│   └── DiffSummary.razor            # Diff statistics
├── Terminal/
│   └── PtyTerminal.razor            # xterm.js wrapper
├── Input/
│   ├── PromptInput.razor            # Chat input
│   └── Autocomplete.razor           # @ and / autocomplete
└── Shared/
    ├── ModelSelector.razor          # Provider/model picker
    ├── CostTracker.razor            # Token/cost display
    └── ThemeToggle.razor            # Light/dark theme
```

### Visual Fidelity Checklist

- [ ] Colors match OpenCode theme
- [ ] Fonts match (IBM Plex Mono)
- [ ] Spacing matches (12px gaps, 16px margins)
- [ ] Icons match (file type icons, provider icons)
- [ ] Animations match (message streaming, typewriter effect)
- [ ] Layout matches (sidebar width, panel heights)
- [ ] Responsive behavior matches

---

## Technical Challenges

### 1. Terminal Emulation

**OpenCode uses:** ghostty-web (WASM)
**Blazor options:**
- xterm.js via JS interop (recommended)
- `XtermBlazor` NuGet package
- Custom terminal renderer (hard!)

**Recommendation:** xterm.js via JS interop

### 2. Syntax Highlighting

**OpenCode uses:** Shiki (VS Code's highlighter)
**Blazor options:**
- Shiki via JS interop
- Monaco Editor (full editor)
- ColorCode (C# library, limited languages)
- Highlight.js via JS interop

**Recommendation:** Monaco.Blazor for full editing, Shiki via JS for code blocks

### 3. Drag-Drop

**OpenCode:** Native HTML5 drag-drop
**Blazor:**
- `MudDropContainer` (MudBlazor)
- Custom JS interop
- Blazor EventHandlers

**Recommendation:** MudBlazor's drag-drop for simplicity

### 4. WebSocket for Terminal

**OpenCode:** Direct WebSocket to PTY endpoint
**Blazor:**
- SignalR hub (preferred in Blazor)
- Direct WebSocket via JS interop
- Custom WebSocket handler

**Recommendation:** SignalR hub that proxies to OpenCode PTY WebSocket

---

## Updated TODO for AgUi.IDE.BlazorServer

### Phase 1: Layout Foundation (Week 1)
- [ ] Create `OpenCodeLayout.razor` matching desktop layout
- [ ] Implement resizable sidebar
- [ ] Implement header with selectors
- [ ] Add theme toggle (light/dark)

### Phase 2: Message Display (Week 2)
- [ ] Implement `SessionTurn.razor`
- [ ] Implement `MessagePart.razor` with all part types
- [ ] Add markdown rendering (marked equivalent)
- [ ] Add syntax highlighting (Shiki or Monaco)

### Phase 3: File System (Week 3)
- [ ] Implement `FileTree.razor` with MudTreeView
- [ ] Implement `FileTabs.razor` with drag-drop
- [ ] Implement `FileViewer.razor`
- [ ] Add file type icons (match OpenCode's)

### Phase 4: Diffs (Week 4)
- [ ] Implement `DiffViewer.razor`
- [ ] Parse unified diff format
- [ ] Syntax highlight diffs
- [ ] Add expand/collapse per file

### Phase 5: Input (Week 5)
- [ ] Implement `PromptInput.razor`
- [ ] Add `@` autocomplete for files
- [ ] Add `/` autocomplete for commands
- [ ] Keyboard shortcuts (Ctrl+Enter)

### Phase 6: Terminal (Week 6)
- [ ] Implement `PtyTerminal.razor`
- [ ] xterm.js JS interop
- [ ] WebSocket connection to OpenCode PTY
- [ ] Multiple terminal tabs

### Phase 7: Polish (Week 7)
- [ ] Match colors exactly (CSS variables)
- [ ] Match animations (typewriter, streaming)
- [ ] Match spacing/layout precisely
- [ ] Add all keyboard shortcuts
- [ ] Cost/token tracking UI

---

## References

**OpenCode Source:**
- Desktop UI: `/dv/opencode/packages/desktop/`
- UI Components: `/dv/opencode/packages/ui/src/components/`
- Server: `/dv/opencode/packages/opencode/src/server/`

**Key Files to Study:**
- `session-turn.tsx` - Message display (400+ lines)
- `message-part.tsx` - Part rendering (450+ lines)
- `prompt-input.tsx` - Input component (1000+ lines!)
- `file-tree.tsx` - File browser
- `terminal.tsx` - Terminal emulator
- `diff-changes.tsx` - Diff visualization

---

*Goal: Blazor version should feel identical to SolidJS version for users.*
