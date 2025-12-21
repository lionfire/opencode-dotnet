# OpenCode Blazor Architecture - AG-UI Integration

> **Purpose:** Explain how AG-UI protocol and OpenCode-specific features work together
> **Audience:** Developers building with LionFire.OpenCode.Blazor
> **Created:** 2025-12-11

---

## Overview

The OpenCode Blazor components use a **hybrid architecture** combining:
1. **AG-UI protocol** for standardized chat/agent interaction
2. **OpenCode REST API** for IDE-specific features
3. **WebSocket** for real-time terminal I/O

This document explains why we use multiple communication channels and how they interact.

---

## The Three Communication Channels

```
┌───────────────────────────────────────────────────────────────────┐
│              LionFire.OpenCode.Blazor Components                   │
│                                                                   │
│  ┌─────────────┐  ┌──────────────────┐  ┌──────────────────┐    │
│  │  Chat UI    │  │  IDE Features    │  │   Terminal UI    │    │
│  │             │  │                  │  │                  │    │
│  │ Generic,    │  │ OpenCode-        │  │ OpenCode-        │    │
│  │ portable    │  │ specific         │  │ specific         │    │
│  └─────────────┘  └──────────────────┘  └──────────────────┘    │
└───────────────────────────────────────────────────────────────────┘
         │                    │                      │
         ▼                    ▼                      ▼
┌──────────────┐  ┌─────────────────────┐  ┌──────────────────┐
│   AG-UI      │  │  OpenCode REST API  │  │  WebSocket       │
│   Protocol   │  │                     │  │  (Direct)        │
│              │  │  IOpenCodeClient    │  │                  │
│  • Messages  │  │  • GetSessionDiff   │  │  • PTY I/O       │
│  • Streaming │  │  • ListFiles        │  │  • Terminal      │
│  • Tool calls│  │  • GetFileStatus    │  │    output        │
└──────────────┘  └─────────────────────┘  └──────────────────┘
         │                    │                      │
         └────────────────────┴──────────────────────┘
                              │
                              ▼
                    ┌──────────────────┐
                    │  opencode serve  │
                    │  localhost:9123  │
                    │                  │
                    │  HTTP + WS + SSE │
                    └──────────────────┘
```

---

## Channel 1: AG-UI Protocol (Chat & Agent Interaction)

### What It Handles

✅ **Conversational aspects:**
- User messages → AI responses
- Streaming text (token-by-token)
- Tool calls (Read, Write, Edit, Bash, etc.)
- Tool results and outputs
- Human-in-the-loop approval requests
- Agent run lifecycle (started, finished, error)

### How It Works

**Stack:**
```
OpenCodeChatClient : IChatClient
       ↓ implements
Microsoft.Extensions.AI.IChatClient
       ↓ wrapped by
Microsoft.Agents.AI.AIAgent
       ↓ emits
AG-UI Events (RUN_STARTED, TEXT_MESSAGE_CONTENT, TOOL_CALL_*, etc.)
```

**Code example:**
```csharp
// Create OpenCode as an IChatClient
var openCodeClient = new OpenCodeClient();
var chatClient = new OpenCodeChatClient(openCodeClient, options);

// Wrap as AIAgent (Microsoft Agent Framework)
var agent = chatClient.AsIChatClient().CreateAIAgent(
    name: "OpenCode",
    instructions: "You are an AI coding assistant.");

// Get AG-UI event stream
await foreach (var update in agent.RunStreamingAsync(messages, thread))
{
    // AG-UI events:
    // - TEXT_MESSAGE_CONTENT (streaming text)
    // - TOOL_CALL_START (OpenCode about to use a tool)
    // - TOOL_CALL_RESULT (tool execution result)
    // - RUN_FINISHED (message complete)

    ProcessUpdate(update);
}
```

### Components Using AG-UI

| Component | Package | Purpose |
|-----------|---------|---------|
| `AgentChat` | `LionFire.AgUi.Blazor` | Generic chat (any agent) |
| `SessionTurn` | `LionFire.OpenCode.Blazor` | Message display |
| `MessagePart` | `LionFire.OpenCode.Blazor` | Part renderer |
| `ToolCallViewer` | `LionFire.OpenCode.Blazor` | Tool visualization |

**Key point:** These components work with **any** AG-UI agent, not just OpenCode.

---

## Channel 2: OpenCode REST API (IDE Features)

### What It Handles

✅ **IDE-specific features:**
- Session diffs (`GET /session/{id}/diff`)
- File browser (`GET /file?path=...`)
- File status (`GET /file/status` - modified/added/deleted)
- Session list (`GET /session`)
- Session management (create, delete, fork, share)
- MCP server management (`GET /mcp/status`)
- Symbol search (`GET /find/symbol`)
- Configuration (`GET /config`)
- Provider/model info (`GET /providers`, `/models`)

### How It Works

**Direct SDK calls:**
```csharp
// IOpenCodeClient from LionFire.OpenCode.Serve
var client = new OpenCodeClient();

// Get session diffs (NOT via AG-UI)
var diffs = await client.GetSessionDiffAsync(sessionId);

// Browse files (NOT via AG-UI)
var files = await client.ListFilesAsync(path: "/src", directory: workingDir);

// Get file status (NOT via AG-UI)
var status = await client.GetFileStatusAsync(directory: workingDir);
// Returns: { modified: [...], added: [...], deleted: [...] }
```

### Why Not Through AG-UI?

**AG-UI is designed for agent communication, not IDE features:**
- ❌ File browsing is not part of AG-UI spec
- ❌ Diff retrieval is not part of AG-UI spec
- ❌ Session management is OpenCode-specific
- ❌ MCP servers are OpenCode-specific

**Using direct API provides:**
- ✅ Type-safe OpenCode models
- ✅ Rich feature set (67+ endpoints)
- ✅ Synchronous queries when needed
- ✅ No protocol overhead

### Components Using OpenCode API

| Component | API Used | Purpose |
|-----------|----------|---------|
| `DiffViewer` | `GetSessionDiffAsync()` | Show file changes |
| `FileTree` | `ListFilesAsync()` | Browse workspace |
| `FileViewer` | `ReadFileAsync()` | Display file content |
| `SessionList` | `ListSessionsAsync()` | Session history |
| `SessionSelector` | `GetSessionAsync()` | Session details |
| `McpServerPanel` | `GetMcpStatusAsync()` | MCP management |
| `FileStatusBar` | `GetFileStatusAsync()` | Show git status |

---

## Channel 3: WebSocket (Terminal I/O)

### What It Handles

✅ **Real-time terminal:**
- PTY (pseudo-terminal) creation
- Terminal input/output streaming
- Terminal resize events
- Multiple terminal tabs
- Buffer persistence

### How It Works

**WebSocket connection:**
```csharp
// Create PTY via REST API
var pty = await client.CreatePtyAsync(new CreatePtyRequest
{
    Cols = 80,
    Rows = 24
}, directory: workingDir);

// Connect to PTY via WebSocket (NOT via AG-UI)
await foreach (var output in client.ConnectToPtyAsync(pty.Id, directory: workingDir))
{
    // Real-time terminal output
    // Render in xterm.js
    await JSRuntime.InvokeVoidAsync("xterm.write", output);
}

// Send input to PTY
await client.SendPtyInputAsync(pty.Id, "ls -la\n", directory: workingDir);
```

**Why WebSocket?**
- ⚡ Low latency (< 10ms)
- 🔁 Bidirectional (input/output)
- 📊 Binary data support
- 🎯 Standard terminal protocol

**Why not AG-UI?**
- AG-UI is unidirectional (agent → client)
- No binary data support
- Not designed for real-time I/O
- Terminal needs immediate feedback

### Components Using WebSocket

| Component | Purpose |
|-----------|---------|
| `PtyTerminal` | Full terminal emulator |
| `PtyTabs` | Multiple terminal management |

---

## Coordination Between Channels

### Scenario: User Asks Agent to Run Tests

**Step-by-step flow:**

```
1. USER TYPES: "Run the tests in the terminal"
   ↓
   PromptInput.razor
   ↓

2. SEND VIA AG-UI:
   AIAgent.RunStreamingAsync(message: "Run the tests...")
   ↓ AG-UI protocol

3. AGENT RESPONDS (via AG-UI events):
   TEXT_MESSAGE_START
   TEXT_MESSAGE_CONTENT: "I'll run the tests using the Bash tool."
   TOOL_CALL_START: { toolName: "Bash", args: { command: "dotnet test" } }
   TOOL_CALL_RESULT: { output: "Tests passed: 42/42" }
   TEXT_MESSAGE_CONTENT: "All 42 tests passed!"
   RUN_FINISHED
   ↓
   SessionTurn.razor displays this

4. COMPONENT FETCHES IDE DATA (via OpenCode API):
   OnMessageComplete() → GetSessionDiffAsync()
   ↓ HTTP GET
   Returns: No file changes (test run only)

5. USER CLICKS TERMINAL TAB (via WebSocket):
   PtyTerminal.OnShow() → CreatePtyAsync()
   ↓ HTTP POST
   Returns: Pty { id: "abc123" }
   ↓ WebSocket connect
   ConnectToPtyAsync("abc123")
   ↓
   Terminal shows: "Tests passed: 42/42"
```

---

## Component Implementation Patterns

### Pattern 1: Pure AG-UI (Generic Component)

**Use when:** Component works with any agent

```razor
@* AgentChat.razor - In LionFire.AgUi.Blazor *@
@inject IAgentClientFactory ClientFactory

<div class="agent-chat">
    <MessageList Messages="@_messages" />
    <PromptInput OnSend="@SendMessage" />
</div>

@code {
    [Parameter] public string AgentName { get; set; } = "opencode";

    private async Task SendMessage(string text)
    {
        var agent = await ClientFactory.GetAgentAsync(AgentName);

        // ONLY AG-UI protocol
        await foreach (var update in agent.RunStreamingAsync(messages, thread))
        {
            // Process TEXT_MESSAGE_CONTENT, TOOL_CALL_*, etc.
            ProcessAgUiUpdate(update);
        }
    }

    // No OpenCode-specific API calls!
    // This component is portable to Goose, Claude Code, etc.
}
```

### Pattern 2: Hybrid (Chat + IDE Features)

**Use when:** Component enhances chat with OpenCode features

```razor
@* OpenCodeChat.razor - In LionFire.OpenCode.Blazor *@
@inject IOpenCodeClient OpenCodeClient
@inherits AgentChat

<div class="opencode-chat">
    <!-- Generic chat (from base class) -->
    @if (ShowChat)
    {
        <MessageList Messages="@Messages" />
    }

    <!-- OpenCode-specific: Diffs -->
    @if (ShowDiffs)
    {
        <DiffViewer SessionId="@CurrentSessionId" Diffs="@_diffs" />
    }

    <!-- Generic input (from base class) -->
    <PromptInput OnSend="@SendMessage" />
</div>

@code {
    [Parameter] public bool ShowDiffs { get; set; } = true;
    [Parameter] public string? CurrentSessionId { get; set; }

    private List<FileDiff> _diffs = new();

    protected override async Task OnMessageComplete(AgentRunResponseUpdate final)
    {
        // Base class handled AG-UI message
        await base.OnMessageComplete(final);

        // Now fetch OpenCode-specific data
        if (ShowDiffs && CurrentSessionId != null)
        {
            _diffs = await OpenCodeClient.GetSessionDiffAsync(CurrentSessionId);
            StateHasChanged();
        }
    }
}
```

### Pattern 3: Pure OpenCode API (IDE Component)

**Use when:** Component has no chat/agent interaction

```razor
@* FileTree.razor - In LionFire.OpenCode.Blazor *@
@inject IOpenCodeClient OpenCodeClient

<MudTreeView T="FileNode" Items="@_rootNodes">
    <ItemTemplate>
        <MudTreeViewItem @bind-Expanded="@context.IsExpanded"
                         Icon="@GetIcon(context)"
                         OnClick="@(() => OnFileClick.InvokeAsync(context))">
            <Content>
                <FileIcon Path="@context.Path" />
                <span>@context.Name</span>
                @if (context.IsModified)
                {
                    <span class="modified-indicator">●</span>
                }
            </Content>
        </MudTreeViewItem>
    </ItemTemplate>
</MudTreeView>

@code {
    [Parameter] public string Directory { get; set; } = "/";
    [Parameter] public EventCallback<FileNode> OnFileClick { get; set; }

    private List<FileNode> _rootNodes = new();

    protected override async Task OnInitializedAsync()
    {
        // Direct OpenCode API call (NO AG-UI)
        var files = await OpenCodeClient.ListFilesAsync(
            path: Directory,
            directory: Directory);

        _rootNodes = BuildTree(files);
    }

    // NO AG-UI involvement - pure OpenCode API
}
```

---

## Main Component: OpenCodeLayout

### The Orchestrator Component

`OpenCodeLayout.razor` is the main component that **coordinates all three channels**:

```razor
@inject IOpenCodeClient OpenCodeClient      // Channel 2: OpenCode API
@inject IAgentClientFactory AgentFactory    // Channel 1: AG-UI
@inject IJSRuntime JS                       // Channel 3: WebSocket (via JS)

<MudLayout>
    <MudAppBar Elevation="1">
        <!-- Uses OpenCode API -->
        <SessionSelector Sessions="@_sessions"
                        Selected="@_currentSession"
                        OnSelect="@LoadSession" />

        <MudSpacer />

        <!-- Uses OpenCode API -->
        <ModelSelector Models="@_models"
                      Selected="@_selectedModel"
                      OnSelect="@ChangeModel" />

        <TerminalToggle OnToggle="@ToggleTerminal" />
    </MudAppBar>

    <MudDrawer Open="@_sidebarOpen" Width="@_sidebarWidth" Variant="DrawerVariant.Persistent">
        <!-- Uses OpenCode API -->
        <FileTree Directory="@_currentDirectory"
                 Files="@_files"
                 OnFileClick="@OpenFile"
                 OnFileDragStart="@HandleFileDrag" />
    </MudDrawer>

    <MudMainContent>
        <MudContainer MaxWidth="MaxWidth.False" Class="pa-0">
            <!-- Uses AG-UI Protocol -->
            <div class="chat-container">
                <AgentChat AgentName="opencode"
                          Thread="@_currentThread"
                          OnMessageComplete="@OnMessageComplete"
                          OnToolCall="@HandleToolCall" />
            </div>

            <!-- Uses OpenCode API -->
            <div class="diff-container">
                <DiffViewer SessionId="@_currentSession?.Id"
                           Diffs="@_sessionDiffs"
                           OnAcceptChanges="@AcceptChanges" />
            </div>

            <!-- Uses OpenCode API -->
            <div class="file-tabs-container">
                <FileTabs OpenFiles="@_openFiles"
                         ActiveFile="@_activeFile"
                         OnFileSelect="@SelectFile"
                         OnFileClose="@CloseFile" />
            </div>

            <!-- Uses OpenCode API for content -->
            <div class="file-viewer-container">
                <FileViewer File="@_activeFile"
                           Content="@_fileContent"
                           ShowDiff="@_showDiff" />
            </div>
        </MudContainer>

        <!-- Uses WebSocket (Channel 3) -->
        <MudDrawer Open="@_terminalOpen" Anchor="Anchor.Bottom" Height="300px">
            <PtyTerminal PtyId="@_currentPty"
                        Directory="@_currentDirectory"
                        OnOutput="@HandleTerminalOutput" />
        </MudDrawer>
    </MudMainContent>
</MudLayout>

@code {
    // THREE CHANNELS coordinated here!

    private string? _currentSession;
    private AgentThread _currentThread;
    private List<Session> _sessions = new();
    private List<FileDiff> _sessionDiffs = new();
    private List<FileNode> _files = new();
    private string? _currentPty;

    protected override async Task OnInitializedAsync()
    {
        // Load sessions via OpenCode API (Channel 2)
        _sessions = await OpenCodeClient.ListSessionsAsync();

        // Load files via OpenCode API (Channel 2)
        _files = await OpenCodeClient.ListFilesAsync("/src");

        // Create AG-UI thread (Channel 1)
        var agent = await AgentFactory.GetAgentAsync("opencode");
        _currentThread = agent.GetNewThread();
    }

    private async Task OnMessageComplete(AgentRunResponseUpdate final)
    {
        // Message came via AG-UI (Channel 1)...

        // Now fetch IDE features via OpenCode API (Channel 2):
        _sessionDiffs = await OpenCodeClient.GetSessionDiffAsync(_currentSession);
        _files = await OpenCodeClient.ListFilesAsync("/src");

        StateHasChanged();
    }

    private async Task ToggleTerminal()
    {
        if (_currentPty == null)
        {
            // Create PTY via OpenCode API (Channel 2)
            var pty = await OpenCodeClient.CreatePtyAsync(
                new CreatePtyRequest { Cols = 80, Rows = 24 });

            _currentPty = pty.Id;

            // Connect via WebSocket (Channel 3) happens in PtyTerminal component
        }

        _terminalOpen = !_terminalOpen;
    }
}
```

---

## Why This Hybrid Approach?

### Benefits

**1. Portability**
- Chat components (AG-UI) work with Goose, Claude Code, any agent
- Only OpenCode-specific features locked to OpenCode

**2. Type Safety**
- OpenCode API returns strongly-typed C# models
- No parsing `CUSTOM` events with JSON

**3. Feature Richness**
- OpenCode has 67+ REST endpoints
- Cramming into AG-UI would be messy

**4. Performance**
- Direct API calls for IDE features (no protocol overhead)
- WebSocket for terminal (lowest latency)
- AG-UI for what it's designed for (chat)

**5. Standards Compliance**
- AG-UI spec is respected (no proprietary extensions)
- OpenCode API is fully utilized
- Both protocols used as intended

---

## Service Layer

### OpenCodeSessionManager

Coordinates all three channels:

```csharp
namespace LionFire.OpenCode.Blazor.Services;

public class OpenCodeSessionManager
{
    private readonly AIAgent _agent;                  // AG-UI
    private readonly IOpenCodeClient _openCodeClient; // OpenCode API

    public OpenCodeSessionManager(
        IAgentClientFactory agentFactory,
        IOpenCodeClient openCodeClient)
    {
        _agent = agentFactory.GetAgentAsync("opencode").Result;
        _openCodeClient = openCodeClient;
    }

    public async Task<SessionViewModel> CreateSessionAsync(string directory)
    {
        // 1. Create via OpenCode API (get session ID)
        var session = await _openCodeClient.CreateSessionAsync(
            directory: directory);

        // 2. Create AG-UI thread (for chat)
        var thread = _agent.GetNewThread();

        // 3. Return view model combining both
        return new SessionViewModel
        {
            SessionId = session.Id,      // From OpenCode API
            Thread = thread,              // From AG-UI
            Directory = directory,
            CreatedAt = session.CreatedAt
        };
    }

    public async IAsyncEnumerable<SessionUpdate> SendMessageAsync(
        SessionViewModel session,
        string message,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        // 1. Send message via AG-UI (Channel 1)
        var messages = new[] { new ChatMessage(ChatRole.User, message) };

        await foreach (var update in _agent.RunStreamingAsync(messages, session.Thread))
        {
            // Yield AG-UI updates (streaming text, tool calls)
            yield return new SessionUpdate
            {
                Type = UpdateType.AgUiEvent,
                AgUiUpdate = update
            };
        }

        // 2. After AG-UI message completes, fetch OpenCode data (Channel 2)

        // Get diffs
        var diffs = await _openCodeClient.GetSessionDiffAsync(
            session.SessionId,
            directory: session.Directory);

        yield return new SessionUpdate
        {
            Type = UpdateType.DiffsUpdated,
            Diffs = diffs
        };

        // Get file status
        var status = await _openCodeClient.GetFileStatusAsync(
            directory: session.Directory);

        yield return new SessionUpdate
        {
            Type = UpdateType.FileStatusUpdated,
            FileStatus = status
        };

        // Get updated file list (in case files created/deleted)
        var files = await _openCodeClient.ListFilesAsync(
            path: session.Directory,
            directory: session.Directory);

        yield return new SessionUpdate
        {
            Type = UpdateType.FilesUpdated,
            Files = files
        };
    }
}

public record SessionUpdate
{
    public UpdateType Type { get; init; }
    public AgentRunResponseUpdate? AgUiUpdate { get; init; }
    public List<FileDiff>? Diffs { get; init; }
    public FileStatus? FileStatus { get; init; }
    public List<FileNode>? Files { get; init; }
}

public enum UpdateType
{
    AgUiEvent,           // From AG-UI protocol
    DiffsUpdated,        // From OpenCode API
    FileStatusUpdated,   // From OpenCode API
    FilesUpdated         // From OpenCode API
}
```

---

## Package Boundaries

### LionFire.AgUi.Blazor (Generic)

**Dependencies:**
- `Microsoft.Agents.AI`
- `Microsoft.Agents.AI.AGUI`
- `Microsoft.Extensions.AI`

**API surface:**
```csharp
// Only AG-UI abstractions
public interface IAgentClientFactory
{
    Task<AIAgent> GetAgentAsync(string agentName);
}

// Generic components
<AgentChat AgentName="..." />
<ToolCallPanel ToolCalls="..." />
```

**No knowledge of:**
- ❌ OpenCode-specific features
- ❌ File operations
- ❌ Sessions
- ❌ PTY/terminals

### LionFire.OpenCode.Blazor (OpenCode-Specific)

**Dependencies:**
- `LionFire.OpenCode.Serve` (SDK)
- `LionFire.AgUi.Blazor` (for generic chat)
- `MudBlazor`
- `Microsoft.Agents.AI` (for AG-UI integration)

**API surface:**
```csharp
// Both AG-UI and OpenCode
public class OpenCodeSessionManager
{
    // AG-UI for chat
    private readonly AIAgent _agent;

    // OpenCode API for IDE
    private readonly IOpenCodeClient _client;
}

// OpenCode-specific components
<OpenCodeLayout AgentName="opencode" Directory="/src" />
<DiffViewer SessionId="..." />
<FileTree Directory="..." />
<PtyTerminal PtyId="..." />
```

**Has knowledge of:**
- ✅ AG-UI protocol (via dependency)
- ✅ OpenCode-specific features (via SDK)
- ✅ How to coordinate both

---

## Event Flow Diagram

### Complete User Interaction

```
┌──────────────────────────────────────────────────────────────────┐
│ USER: "Add logging to ApiController.cs"                          │
└──────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌──────────────────────────────────────────────────────────────────┐
│ PromptInput.razor                                                 │
│   → Calls: AIAgent.RunStreamingAsync()                           │
└──────────────────────────────────────────────────────────────────┘
                              │ AG-UI Protocol (Channel 1)
                              ▼
┌──────────────────────────────────────────────────────────────────┐
│ opencode serve                                                    │
│   → Processes message                                             │
│   → Calls LLM                                                     │
│   → LLM decides to use Write tool                                │
└──────────────────────────────────────────────────────────────────┘
                              │ AG-UI Events
                              ▼
┌──────────────────────────────────────────────────────────────────┐
│ SessionTurn.razor                                                 │
│   ← TEXT_MESSAGE_START                                           │
│   ← TEXT_MESSAGE_CONTENT: "I'll add logging..."                 │
│   ← TOOL_CALL_START: Write(ApiController.cs)                    │
│   ← TOOL_CALL_RESULT: Success                                   │
│   ← TEXT_MESSAGE_CONTENT: "Added logging to line 42"            │
│   ← RUN_FINISHED                                                 │
└──────────────────────────────────────────────────────────────────┘
                              │
                              ▼
┌──────────────────────────────────────────────────────────────────┐
│ OnMessageComplete() handler                                       │
│   → Triggers OpenCode API calls                                  │
└──────────────────────────────────────────────────────────────────┘
                              │ OpenCode API (Channel 2)
                              ▼
┌──────────────────────────────────────────────────────────────────┐
│ DiffViewer.razor                                                  │
│   → Calls: OpenCodeClient.GetSessionDiffAsync()                 │
│   ← Returns: FileDiff[] with unified diffs                       │
│   → Displays: ApiController.cs changes with syntax highlighting  │
└──────────────────────────────────────────────────────────────────┘
                              │
┌──────────────────────────────────────────────────────────────────┐
│ FileTree.razor                                                    │
│   → Calls: OpenCodeClient.GetFileStatusAsync()                  │
│   ← Returns: Modified files list                                │
│   → Updates: Shows ● indicator on ApiController.cs               │
└──────────────────────────────────────────────────────────────────┘
                              │
                    (User clicks Terminal tab)
                              │ OpenCode API (Channel 2) + WebSocket (Channel 3)
                              ▼
┌──────────────────────────────────────────────────────────────────┐
│ PtyTerminal.razor                                                 │
│   → Calls: OpenCodeClient.CreatePtyAsync()                      │
│   ← Returns: Pty { id: "abc123" }                               │
│   → Connects: WebSocket to /pty/abc123/connect                  │
│   ← Streams: Terminal output in real-time                       │
└──────────────────────────────────────────────────────────────────┘
```

---

## When to Use Which Channel

### Decision Matrix

| Need | Use | Why |
|------|-----|-----|
| Send chat message | AG-UI | Standard protocol, portable |
| Stream AI response | AG-UI | Real-time text streaming |
| Show tool calls | AG-UI | Standard TOOL_CALL events |
| Get session diffs | OpenCode API | OpenCode-specific feature |
| Browse files | OpenCode API | Rich file metadata |
| Show file changes | OpenCode API | Git integration |
| List sessions | OpenCode API | Session management |
| Terminal I/O | WebSocket | Low latency, bidirectional |
| MCP servers | OpenCode API | OpenCode-specific |
| Symbol search | OpenCode API | LSP integration |

### Rule of Thumb

**Use AG-UI when:**
- ✅ Feature is conversational (messages, chat)
- ✅ Feature should work with other agents
- ✅ Feature is in AG-UI spec (tools, state)

**Use OpenCode API when:**
- ✅ Feature is IDE-specific (files, diffs, sessions)
- ✅ Feature is OpenCode-unique
- ✅ Type-safe models are important

**Use WebSocket when:**
- ✅ Need real-time bidirectional I/O
- ✅ Low latency critical (< 10ms)
- ✅ Binary data (terminal)

---

## Benefits of Hybrid Architecture

### ✅ Best of Both Worlds

| Aspect | Benefit |
|--------|---------|
| **Portability** | Chat components work with Goose, Claude Code, etc. |
| **Feature richness** | Full access to OpenCode's 67+ API endpoints |
| **Type safety** | OpenCode SDK provides C# models |
| **Performance** | Direct API, no protocol translation overhead |
| **Standards** | AG-UI spec respected, no proprietary extensions |

### ✅ Clean Separation of Concerns

```csharp
// Generic chat service (any agent)
public interface IAgentChatService
{
    // Uses AG-UI only
    IAsyncEnumerable<AgentUpdate> StreamAsync(string message);
}

// OpenCode IDE service (OpenCode-specific)
public interface IOpenCodeIdeService
{
    // Uses OpenCode API
    Task<List<FileDiff>> GetDiffsAsync(string sessionId);
    Task<List<FileNode>> GetFilesAsync(string directory);
    Task<Pty> CreateTerminalAsync();
}

// UI components compose both
public class OpenCodeLayout
{
    [Inject] private IAgentChatService ChatService { get; set; }
    [Inject] private IOpenCodeIdeService IdeService { get; set; }
}
```

---

## Interoperability Story

### Same Chat Component, Different Agents

```razor
@* This works with ANY AG-UI agent *@

<!-- OpenCode (with IDE features) -->
<OpenCodeLayout AgentName="opencode" />

<!-- Goose (when we add it) -->
<GooseLayout AgentName="goose" />

<!-- Raw LLM (just chat, no IDE) -->
<AgentChat AgentName="ollama-llama3" />
```

**The `AgentChat` component knows nothing about OpenCode.** It just uses AG-UI protocol.

**The `OpenCodeLayout` component knows about both:**
- Uses `AgentChat` (generic) for the chat part
- Adds `DiffViewer`, `FileTree`, `PtyTerminal` (OpenCode-specific)

---

## Summary

### The Architecture Decision

We use **three communication channels** because they each solve different problems:

1. **AG-UI** - Standardized agent communication (chat, tools)
2. **OpenCode API** - Rich IDE features (files, diffs, sessions)
3. **WebSocket** - Real-time terminal I/O

### The Package Strategy

- **`LionFire.AgUi.Blazor`** - Generic AG-UI components (Channel 1 only)
- **`LionFire.OpenCode.Blazor`** - OpenCode IDE components (All 3 channels)
- **Samples** - Demonstrate how to compose them

### The Value Proposition

**For users:**
- Install one package: `LionFire.OpenCode.Blazor`
- Get everything: Chat + IDE features + Terminal
- Components handle the complexity
- Just compose and configure

**For developers:**
- Clear boundaries between generic and specific
- Type-safe APIs
- Standards-compliant
- Easy to extend

---

*This hybrid architecture gives us portability where it matters (chat) and richness where it's needed (IDE features).*
