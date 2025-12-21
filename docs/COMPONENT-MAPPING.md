# OpenCode Component Mapping: SolidJS → Blazor

> **Date**: 2025-12-12
> **Purpose**: Map OpenCode's SolidJS components to Blazor/MudBlazor equivalents
> **Status**: Complete

---

## 1. Layout Components

### Main Layout

| SolidJS Component | Location | Blazor Equivalent | Notes |
|-------------------|----------|-------------------|-------|
| `Layout` | `desktop/src/pages/layout.tsx` | `OpenCodeLayout.razor` | Custom component using MudLayout |
| Main content area | - | `MudMainContent` | With custom CSS |
| Sidebar | - | `MudDrawer` | Collapsible file tree |
| Header | - | `MudAppBar` | Optional, may not be needed |

**Blazor Implementation**:
```razor
@* OpenCodeLayout.razor *@
<MudLayout>
    <MudDrawer @bind-Open="_sidebarOpen" Variant="DrawerVariant.Persistent">
        <FileTree />
    </MudDrawer>
    <MudMainContent>
        <div class="opencode-main">
            @ChildContent
        </div>
    </MudMainContent>
</MudLayout>
```

---

## 2. Chat/Session Components

### SessionTurn

| SolidJS | File | Blazor | Package |
|---------|------|--------|---------|
| `SessionTurn` | `ui/src/components/session-turn.tsx` | `SessionTurn.razor` | LionFire.OpenCode.Blazor |

**Props Mapping**:
```csharp
// Blazor parameters
[Parameter] public string SessionId { get; set; }
[Parameter] public string MessageId { get; set; }
[Parameter] public string? RootClass { get; set; }
[Parameter] public string? ContentClass { get; set; }
[Parameter] public string? ContainerClass { get; set; }
[Parameter] public RenderFragment? ChildContent { get; set; }
```

**Key Sub-elements**:
- Title with typewriter animation
- User message display
- Summary section with markdown
- File diff accordion
- Progress indicator
- "Show details" collapsible

### MessagePart

| SolidJS | File | Blazor | Package |
|---------|------|--------|---------|
| `Message` | `ui/src/components/message-part.tsx` | `MessageDisplay.razor` | LionFire.OpenCode.Blazor |
| `Part` | same | `MessagePart.razor` | LionFire.OpenCode.Blazor |
| `UserMessageDisplay` | same | `UserMessage.razor` | LionFire.OpenCode.Blazor |
| `AssistantMessageDisplay` | same | `AssistantMessage.razor` | LionFire.OpenCode.Blazor |

**Part Type Registry** (Blazor equivalent):
```csharp
public interface IPartRenderer
{
    bool CanRender(string partType);
    RenderFragment Render(PartData part, MessageData message);
}

public class PartRendererRegistry
{
    private readonly Dictionary<string, IPartRenderer> _renderers = new();

    public void Register(string partType, IPartRenderer renderer)
        => _renderers[partType] = renderer;

    public IPartRenderer? GetRenderer(string partType)
        => _renderers.GetValueOrDefault(partType);
}
```

### PromptInput

| SolidJS | File | Blazor | Package |
|---------|------|--------|---------|
| `PromptInput` | `desktop/src/components/prompt-input.tsx` | `PromptInput.razor` | LionFire.OpenCode.Blazor |

**Complex Component** - Needs custom implementation:
- ContentEditable div (not textarea) for file pill support
- @ mention autocomplete
- Model selector
- Agent selector
- Send/Cancel button

**Blazor Approach**:
```razor
<div class="prompt-input-container">
    <div class="autocomplete-popup" style="display: @(_showAutocomplete ? "block" : "none")">
        <!-- File suggestions -->
    </div>

    <div @ref="_editorRef"
         contenteditable="true"
         @oninput="HandleInput"
         @onkeydown="HandleKeyDown"
         class="prompt-editor">
    </div>

    <div class="prompt-toolbar">
        <MudSelect T="string" @bind-Value="_selectedAgent" Variant="Variant.Text">
            @foreach (var agent in Agents)
            {
                <MudSelectItem Value="@agent.Name">@agent.Name</MudSelectItem>
            }
        </MudSelect>

        <MudButton Variant="Variant.Text" OnClick="OpenModelSelector">
            @_selectedModel?.Name
        </MudButton>

        <MudIconButton Icon="@Icons.Material.Filled.Send" OnClick="Submit" />
    </div>
</div>
```

---

## 3. Tool Visualization Components

### BasicTool (Generic Tool Container)

| SolidJS | Blazor | MudBlazor Base |
|---------|--------|----------------|
| `BasicTool` | `ToolCard.razor` | MudCard or custom |

**Implementation**:
```razor
@* ToolCard.razor *@
<div class="tool-card">
    <div class="tool-header" @onclick="ToggleExpand">
        <MudIcon Icon="@Icon" Size="Size.Small" />
        <div class="tool-title">@Title</div>
        <div class="tool-subtitle">@Subtitle</div>
        @if (Actions != null)
        {
            <div class="tool-actions">@Actions</div>
        }
        <MudIcon Icon="@(Expanded ? Icons.Material.Filled.ExpandLess : Icons.Material.Filled.ExpandMore)" />
    </div>
    @if (Expanded && ChildContent != null)
    {
        <div class="tool-content">
            @ChildContent
        </div>
    }
</div>
```

### Specific Tool Renderers

| Tool Name | SolidJS Render | Blazor Component |
|-----------|----------------|------------------|
| `read` | Icon: glasses | `ReadToolCard.razor` |
| `list` | Icon: bullet-list | `ListToolCard.razor` |
| `glob` | Icon: magnifying-glass-menu | `GlobToolCard.razor` |
| `grep` | Icon: magnifying-glass-menu | `GrepToolCard.razor` |
| `webfetch` | Icon: window-cursor | `WebFetchToolCard.razor` |
| `task` | Icon: task | `TaskToolCard.razor` |
| `bash` | Icon: console | `BashToolCard.razor` |
| `edit` | Icon: code-lines | `EditToolCard.razor` |
| `write` | Icon: code-lines | `WriteToolCard.razor` |
| `todowrite` | Icon: checklist | `TodoWriteToolCard.razor` |

**Bash Tool Example**:
```razor
@* BashToolCard.razor *@
<ToolCard Icon="@Icons.Material.Filled.Terminal" Title="Shell" Subtitle="@Description">
    <pre class="bash-output">
        <code>$ @Command</code>
        @if (!string.IsNullOrEmpty(Output))
        {
            <br />@Output
        }
    </pre>
</ToolCard>
```

---

## 4. UI Primitive Components

### Direct MudBlazor Mappings

| OpenCode Component | MudBlazor Component | Notes |
|-------------------|---------------------|-------|
| `Accordion` | `MudExpansionPanels` | Multiple panels |
| `Accordion.Item` | `MudExpansionPanel` | Single panel |
| `Button` | `MudButton` | Various variants |
| `Card` | `MudCard` | With variants (error, etc.) |
| `Checkbox` | `MudCheckBox` | - |
| `Dialog` | `MudDialog` | Modal dialogs |
| `Icon` | `MudIcon` | Need icon mapping |
| `IconButton` | `MudIconButton` | - |
| `Input` | `MudTextField` | - |
| `List` | `MudList` | - |
| `Select` | `MudSelect` | - |
| `Tag` | `MudChip` | Small labels |
| `Tooltip` | `MudTooltip` | - |

### Custom Components Needed

| OpenCode Component | Blazor Implementation | Notes |
|-------------------|----------------------|-------|
| `Collapsible` | `Collapsible.razor` | Custom or MudExpansionPanel |
| `FileIcon` | `FileIcon.razor` | Map file extensions to icons |
| `Markdown` | `MarkdownRenderer.razor` | Use Markdig library |
| `Typewriter` | `Typewriter.razor` | Character-by-character animation |
| `SelectDialog` | `SelectDialog.razor` | Search + select dialog |
| `StickyAccordionHeader` | CSS-based | Position: sticky |

---

## 5. File/IDE Components

### FileTree

| SolidJS | Blazor | MudBlazor Base |
|---------|--------|----------------|
| File tree sidebar | `FileTree.razor` | `MudTreeView` |

```razor
<MudTreeView T="FileNode" Items="_files" Hover="true" SelectedValueChanged="OnFileSelect">
    <ItemTemplate>
        <MudTreeViewItem Value="@context" Icon="@GetFileIcon(context)" Text="@context.Name">
            <ChildContent>
                @if (context.Children != null)
                {
                    @foreach (var child in context.Children)
                    {
                        <MudTreeViewItem Value="@child" Icon="@GetFileIcon(child)" Text="@child.Name" />
                    }
                }
            </ChildContent>
        </MudTreeViewItem>
    </ItemTemplate>
</MudTreeView>
```

### DiffViewer

| SolidJS | Blazor | Notes |
|---------|--------|-------|
| Dynamic diff component | `DiffViewer.razor` | Monaco Editor or custom |

**Options**:
1. **Monaco Editor** - Full diff support, but heavy
2. **BlazorMonaco** - Blazor wrapper for Monaco
3. **Custom** - Simple line-by-line with highlighting

```razor
@* Using BlazorMonaco *@
<MonacoDiffEditor @ref="_diffEditor"
                  OriginalValue="@Before"
                  ModifiedValue="@After"
                  ConstructionOptions="DiffEditorConstructionOptions" />
```

### TerminalPanel

| SolidJS | Blazor | Notes |
|---------|--------|-------|
| Terminal (ghostty-web) | `TerminalPanel.razor` | XTerm.js via JS interop |

```razor
@inject IJSRuntime JS

<div @ref="_terminalContainer" class="terminal-container"></div>

@code {
    private ElementReference _terminalContainer;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await JS.InvokeVoidAsync("opencode.initTerminal", _terminalContainer);
        }
    }
}
```

---

## 6. State Management Mapping

### SolidJS Contexts → Blazor Services

| SolidJS Context | Blazor Service | Scope |
|-----------------|----------------|-------|
| `useData()` | `IOpenCodeDataService` | Scoped |
| `useSession()` | `ISessionService` | Scoped |
| `useLocal()` | `ILocalSettingsService` | Scoped |
| `useLayout()` | `ILayoutService` | Scoped |
| `useSDK()` | `IOpenCodeClient` (from SDK) | Scoped |
| `useSync()` | `ISyncService` | Scoped |

**Service Registration**:
```csharp
services.AddScoped<IOpenCodeDataService, OpenCodeDataService>();
services.AddScoped<ISessionService, SessionService>();
services.AddScoped<ILocalSettingsService, LocalSettingsService>();
services.AddScoped<ILayoutService, LayoutService>();
```

### Reactive Patterns

| SolidJS | Blazor |
|---------|--------|
| `createSignal()` | Private field + `StateHasChanged()` |
| `createMemo()` | Computed property |
| `createEffect()` | `OnParametersSet()` or event subscription |
| `For each=` | `@foreach` |
| `Show when=` | `@if` |
| `Switch/Match` | `@switch` or `@if/else if` |
| `createStore()` | Fluxor state or service with events |

**Example Translation**:
```typescript
// SolidJS
const [count, setCount] = createSignal(0);
const doubled = createMemo(() => count() * 2);
```

```csharp
// Blazor
private int _count = 0;
private int Count
{
    get => _count;
    set { _count = value; StateHasChanged(); }
}
private int Doubled => Count * 2;
```

---

## 7. Icon Mapping

OpenCode uses custom icons. Map to Material Icons or custom SVGs:

| OpenCode Icon | Material Icon | Notes |
|---------------|---------------|-------|
| `glasses` | `Visibility` | Read tool |
| `bullet-list` | `List` | List tool |
| `magnifying-glass-menu` | `Search` | Glob/Grep tool |
| `window-cursor` | `Language` | Webfetch |
| `task` | `Assignment` | Task tool |
| `console` | `Terminal` | Bash tool |
| `code-lines` | `Code` | Edit/Write tool |
| `checklist` | `Checklist` | Todo tool |
| `chevron-down` | `ExpandMore` | - |
| `chevron-grabber-vertical` | `DragHandle` | - |
| `circle-ban-sign` | `Block` | Error |
| `plus-small` | `Add` | - |
| `enter` | `KeyboardReturn` | - |
| `stop` | `Stop` | - |
| `arrow-up` | `ArrowUpward` | Send |
| `square-arrow-top-right` | `OpenInNew` | External link |

---

## 8. CSS Class Mapping

### Tailwind → Custom CSS

OpenCode uses Tailwind. For Blazor, create equivalent custom classes:

```css
/* opencode-utilities.css */

/* Flex utilities */
.flex { display: flex; }
.flex-col { flex-direction: column; }
.items-center { align-items: center; }
.justify-between { justify-content: space-between; }
.gap-1 { gap: 0.25rem; }
.gap-2 { gap: 0.5rem; }
.gap-3 { gap: 0.75rem; }

/* Size utilities */
.size-full { width: 100%; height: 100%; }
.size-4 { width: 1rem; height: 1rem; }
.size-5 { width: 1.25rem; height: 1.25rem; }
.w-full { width: 100%; }

/* Text utilities */
.text-14-regular { font-size: 14px; font-weight: 400; }
.text-14-medium { font-size: 14px; font-weight: 500; }
.text-12-regular { font-size: 12px; font-weight: 400; }
.text-text-strong { color: var(--text-strong); }
.text-text-weak { color: var(--text-weak); }

/* Spacing */
.px-2 { padding-left: 0.5rem; padding-right: 0.5rem; }
.py-3 { padding-top: 0.75rem; padding-bottom: 0.75rem; }
.px-5 { padding-left: 1.25rem; padding-right: 1.25rem; }

/* Borders */
.rounded-md { border-radius: 0.375rem; }
.border { border-width: 1px; }
.border-border-base { border-color: var(--border-base); }
```

---

## 9. File Structure

### LionFire.OpenCode.Blazor Package Structure

```
src/LionFire.OpenCode.Blazor/
├── LionFire.OpenCode.Blazor.csproj
├── _Imports.razor
│
├── Components/
│   ├── Layout/
│   │   ├── OpenCodeLayout.razor
│   │   └── OpenCodeLayout.razor.css
│   │
│   ├── Session/
│   │   ├── SessionTurn.razor
│   │   ├── SessionTurn.razor.cs
│   │   ├── MessageDisplay.razor
│   │   ├── MessagePart.razor
│   │   ├── UserMessage.razor
│   │   └── AssistantMessage.razor
│   │
│   ├── Input/
│   │   ├── PromptInput.razor
│   │   ├── PromptInput.razor.cs
│   │   └── FileAutocomplete.razor
│   │
│   ├── Tools/
│   │   ├── ToolCard.razor
│   │   ├── ReadToolCard.razor
│   │   ├── BashToolCard.razor
│   │   ├── EditToolCard.razor
│   │   └── ... (other tools)
│   │
│   ├── Files/
│   │   ├── FileTree.razor
│   │   ├── FileIcon.razor
│   │   └── DiffViewer.razor
│   │
│   ├── Terminal/
│   │   └── TerminalPanel.razor
│   │
│   └── Shared/
│       ├── Typewriter.razor
│       ├── MarkdownRenderer.razor
│       ├── Collapsible.razor
│       └── SelectDialog.razor
│
├── Services/
│   ├── IOpenCodeDataService.cs
│   ├── OpenCodeDataService.cs
│   ├── ISessionService.cs
│   ├── SessionService.cs
│   ├── ILayoutService.cs
│   ├── LayoutService.cs
│   ├── IPartRendererRegistry.cs
│   ├── PartRendererRegistry.cs
│   └── ServiceCollectionExtensions.cs
│
├── Models/
│   ├── SessionTurnData.cs
│   ├── MessageData.cs
│   ├── PartData.cs
│   ├── ToolData.cs
│   └── FileNode.cs
│
├── wwwroot/
│   ├── css/
│   │   ├── opencode-colors.css
│   │   ├── opencode-theme.css
│   │   ├── opencode-base.css
│   │   └── opencode-utilities.css
│   │
│   ├── js/
│   │   ├── opencode-terminal.js
│   │   └── opencode-editor.js
│   │
│   └── icons/
│       └── (custom SVG icons if needed)
│
└── Themes/
    └── OpenCodeTheme.cs
```

---

## 10. Implementation Priority

### Phase 1: Core Components (Week 1-2)
1. Theme system (CSS variables)
2. MudBlazor theme configuration
3. OpenCodeLayout
4. SessionTurn (basic)
5. MessagePart (text only)
6. ToolCard (generic)

### Phase 2: Tools & Input (Week 2-3)
1. All tool card variants
2. PromptInput (basic)
3. Markdown renderer
4. Typewriter animation

### Phase 3: IDE Features (Week 3-4)
1. FileTree
2. DiffViewer
3. PromptInput with @ mentions
4. File attachments

### Phase 4: Polish (Week 4)
1. Terminal integration
2. Animations
3. Dark/light mode toggle
4. Performance optimization

---

## 11. Testing Strategy

### Component Tests
- Each component should have a test file
- Use bUnit for Blazor component testing
- Test parameter binding
- Test event callbacks

### Integration Tests
- Test service interactions
- Test state management
- Test AG-UI protocol integration

### Visual Tests
- Compare screenshots with original OpenCode
- Use Playwright or similar
- Target 95%+ visual fidelity
