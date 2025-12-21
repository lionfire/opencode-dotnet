# OpenCode Web UI → Blazor Replication Guide

> **Goal:** Create a pixel-perfect Blazor version of OpenCode's SolidJS web UI
> **Source:** `/dv/opencode/packages/desktop/` and `/dv/opencode/packages/ui/`
> **Target:** MudBlazor-based Blazor components in `/src/opencode-dotnet/`

---

## Quick Reference: Component Mapping

| OpenCode (SolidJS) | Blazor (MudBlazor) | Location |
|-------------------|-------------------|----------|
| `session-turn.tsx` | `SessionTurn.razor` | `LionFire.OpenCode.Blazor/Components/Session/` |
| `message-part.tsx` | `MessagePart.razor` | `LionFire.OpenCode.Blazor/Components/Session/` |
| `diff-changes.tsx` | `DiffViewer.razor` | `LionFire.OpenCode.Blazor/Components/Diffs/` |
| `file-tree.tsx` | `FileTree.razor` | `LionFire.OpenCode.Blazor/Components/Files/` |
| `prompt-input.tsx` | `PromptInput.razor` | `LionFire.OpenCode.Blazor/Components/Input/` |
| `terminal.tsx` | `PtyTerminal.razor` | `LionFire.OpenCode.Blazor/Components/Terminal/` |
| `layout.tsx` | `OpenCodeLayout.razor` | `LionFire.OpenCode.Blazor/Components/Layout/` |

---

## Visual Fidelity Checklist

### Colors & Theme
- [ ] Light theme colors match (use CSS inspector on opencode web)
- [ ] Dark theme colors match
- [ ] Border colors match
- [ ] Hover states match
- [ ] Focus states match
- [ ] Selection colors match

### Typography
- [ ] IBM Plex Mono font loaded
- [ ] Font sizes match (12px code, 14px body, 16px headers)
- [ ] Line heights match
- [ ] Font weights match

### Spacing
- [ ] Message vertical spacing: 16px
- [ ] Panel gaps: 12px
- [ ] Component padding: 12px
- [ ] Header height: 48px
- [ ] Sidebar width: 48px-300px (resizable)

### Icons
- [ ] File type icons match (use same SVG sprites if possible)
- [ ] Provider icons match
- [ ] UI icons match (collapse, expand, close, etc.)

### Animations
- [ ] Message streaming (typewriter effect)
- [ ] Scroll-to-bottom smooth scrolling
- [ ] Panel resize smooth transitions
- [ ] Tab drag-drop visual feedback
- [ ] Loading spinners match

---

## Implementation Workflow

### For Each Component

1. **Study original:**
   ```bash
   # Open in VS Code
   code /dv/opencode/packages/ui/src/components/session-turn.tsx
   ```

2. **Extract structure:**
   - What props does it accept?
   - What state does it manage?
   - What events does it emit?
   - What child components does it use?

3. **Create Blazor equivalent:**
   - Map SolidJS patterns to Blazor
   - Map Kobalte components to MudBlazor
   - Preserve same CSS classes (for easier styling)

4. **Match styling:**
   - Copy relevant CSS from `.css` file
   - Adapt for Blazor CSS isolation
   - Use MudBlazor theming where appropriate

5. **Test side-by-side:**
   - Run `opencode web`
   - Run Blazor sample
   - Compare visually
   - Iterate until identical

---

## SolidJS → Blazor Pattern Mapping

### Reactivity

**SolidJS:**
```typescript
const [count, setCount] = createSignal(0);
const doubled = createMemo(() => count() * 2);

<div>{doubled()}</div>
```

**Blazor:**
```csharp
private int _count = 0;
private int Doubled => _count * 2;

<div>@Doubled</div>
```

### Conditional Rendering

**SolidJS:**
```typescript
<Show when={condition} fallback={<div>No</div>}>
  <div>Yes</div>
</Show>
```

**Blazor:**
```razor
@if (condition)
{
    <div>Yes</div>
}
else
{
    <div>No</div>
}
```

### List Rendering

**SolidJS:**
```typescript
<For each={items}>
  {(item, index) => <div>{item.name}</div>}
</For>
```

**Blazor:**
```razor
@foreach (var item in items)
{
    <div>@item.Name</div>
}
```

### Event Handling

**SolidJS:**
```typescript
<button onClick={() => handleClick(item)}>Click</button>
```

**Blazor:**
```razor
<button @onclick="@(() => HandleClick(item))">Click</button>
```

### Lifecycle

**SolidJS:**
```typescript
onMount(() => {
  // Runs once on component mount
});

onCleanup(() => {
  // Runs on unmount
});
```

**Blazor:**
```csharp
protected override async Task OnInitializedAsync()
{
    // Runs once on component initialization
}

protected override async Task OnAfterRenderAsync(bool firstRender)
{
    if (firstRender)
    {
        // Runs once after first render
    }
}

public void Dispose()
{
    // Runs on component disposal
}
```

---

## CSS Porting Strategy

### Option A: Direct Port
Copy OpenCode's CSS files, adapt for Blazor CSS isolation:

```css
/* session-turn.razor.css */
::deep [data-component="session-turn"] {
  /* Original OpenCode styles */
}
```

### Option B: MudBlazor Theming
Use MudBlazor's theming system to match colors:

```csharp
var theme = new MudTheme
{
    Palette = new PaletteLight
    {
        Primary = "#..." // Match OpenCode primary
        // ... etc
    }
};
```

### Option C: Hybrid
- Use MudBlazor for component structure
- Add custom CSS for OpenCode-specific styling
- Use CSS variables for colors

**Recommendation:** Option C (hybrid approach)

---

## Testing Strategy

### Visual Regression Testing

1. **Screenshot comparison:**
   - Take screenshots of `opencode web`
   - Take screenshots of Blazor version
   - Use image diff tools

2. **Manual testing:**
   - Run both side-by-side
   - Compare every component
   - Check responsive behavior

3. **Feature parity:**
   - Every feature in OpenCode works in Blazor
   - Same keyboard shortcuts
   - Same drag-drop behavior

---

## Timeline Estimate

| Component | Complexity | Estimate |
|-----------|-----------|----------|
| Layout | Medium | 2 days |
| SessionTurn | High | 3 days |
| MessagePart | High | 3 days |
| DiffViewer | Medium | 2 days |
| FileTree | Medium | 2 days |
| PromptInput | High | 4 days |
| FileTabs | Medium | 2 days |
| Terminal | High | 5 days |
| Polish | Medium | 3 days |

**Total:** ~4 weeks for full-fidelity replication

---

## Success Criteria

**The Blazor version is successful when:**

1. ✅ A user familiar with `opencode web` feels immediately at home
2. ✅ All visual elements match (colors, fonts, spacing, icons)
3. ✅ All features work (chat, files, diffs, terminal)
4. ✅ Performance is comparable (smooth streaming, no jank)
5. ✅ Works in Server, WASM, and Hybrid (desktop) modes

---

*When in doubt, defer to the original OpenCode UI design.*
