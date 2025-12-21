# OpenCode Blazor Samples - Implementation Plan

> **Status:** Ready to Implement
> **Created:** 2025-12-11
> **Architecture:** Samples use pre-built components from LionFire.OpenCode.Blazor

## Overview

This plan describes **5 sample projects** demonstrating OpenCode integration with AG-UI Blazor components across all Blazor hosting models.

---

## Sample Projects

### 1. AgUi.Chat.BlazorServer (Minimal Web)

**Path:** `examples/AgUi.Chat.BlazorServer/`
**Type:** Blazor Server Web App
**Purpose:** Simplest possible example (< 30 lines)
**Dependencies:** LionFire.AgUi.Blazor, OpenCode SDK

Shows basic OpenCode + generic AG-UI chat integration.

### 2. AgUi.IDE.BlazorServer (Full Web)

**Path:** `examples/AgUi.IDE.BlazorServer/`
**Type:** Blazor Server Web App
**Purpose:** **Faithful replication of `opencode web` UI in Blazor**
**Reference:** `/dv/opencode/packages/desktop/` (SolidJS implementation)
**Dependencies:** LionFire.OpenCode.Blazor, MudBlazor

Multi-panel IDE experience with:
- File explorer
- Code editor with diff viewer
- AI chat assistant
- Terminal emulator
- All using pre-built components

### 3. AgUi.Chat.BlazorWasm (Minimal SPA)

**Path:** `examples/AgUi.Chat.BlazorWasm/`
**Type:** Blazor WASM (Client/Server split)
**Purpose:** Minimal SPA with offline support
**Dependencies:** LionFire.AgUi.Blazor.Wasm, OpenCode SDK

Shows WASM variant of chat.

### 4. AgUi.IDE.BlazorWasm (Full SPA)

**Path:** `examples/AgUi.IDE.BlazorWasm/`
**Type:** Blazor WASM (Client/Server split)
**Purpose:** Full IDE as single-page app
**Dependencies:** LionFire.OpenCode.Blazor, MudBlazor

Proves component portability across hosting models.

### 5. AgUi.IDE.Desktop (Native App)

**Path:** `examples/AgUi.IDE.Desktop/`
**Type:** .NET MAUI with Blazor WebView
**Purpose:** Native Windows/Mac desktop application
**Platforms:** Windows, MacCatalyst
**Dependencies:** LionFire.OpenCode.Blazor, MAUI, WebView2

Demonstrates reusing same components in desktop context.

---

## Architecture

### Component Model

All samples consume pre-built components from `LionFire.OpenCode.Blazor`:

```
LionFire.OpenCode.Blazor (RCL)
    ├── IDEView.razor
    ├── ChatPanel.razor
    ├── FilesPanel.razor
    ├── DiffPanel.razor
    ├── TerminalPanel.razor
    └── SessionTurn.razor
        ↑ used by all samples
```

### Dependency Flow

```
All Samples
    ↓ depend on
LionFire.OpenCode.Blazor (OpenCode-specific UI)
    ↓ depends on both
    ├── LionFire.AgUi.Blazor (generic AG-UI chat)
    └── LionFire.OpenCode.Serve (SDK)
```

### No More Shared.Components

The `Shared.Components` library has been moved to `src/LionFire.OpenCode.Blazor/`. Samples no longer need local copies:

**Before:**
```
examples/Shared.Components/ (RCL)
  ↑ used by all samples
```

**After:**
```
src/LionFire.OpenCode.Blazor/ (in opencode-dotnet/src/)
  ↑ referenced as NuGet package or project reference
  ↑ used by all samples
```

Benefits:
- Single source of truth
- Easier to maintain and update
- Pre-built, tested components
- Samples focus on wiring/configuration only

---

## Getting Started

### Prerequisites

```bash
# 1. Ensure opencode is installed
opencode --version

# 2. Start opencode serve
opencode serve --port 9123

# 3. Ensure ag-ui-blazor is built
cd /src/ag-ui-blazor
dotnet build
```

### Study the Original UI

```bash
# 1. Run original SolidJS UI
cd /dv/opencode
opencode web --port 4096

# 2. Review documentation
cd /src/opencode-dotnet/examples
cat OPENCODE-WEB-UI-ANALYSIS.md     # Architecture analysis
cat UI-REPLICATION-GUIDE.md         # Replication guide

# 3. Explore source components
code /dv/opencode/packages/ui/src/components/session-turn.tsx
code /dv/opencode/packages/ui/src/components/message-part.tsx
code /dv/opencode/packages/ui/src/components/prompt-input.tsx
```

---

## Implementation Order

### Week 1: Foundation

1. [ ] Study original OpenCode web UI
2. [ ] Review LionFire.OpenCode.Blazor project structure
3. [ ] Understand component APIs and interfaces
4. [ ] Set up theme/styling system

### Week 2: Blazor Server Minimal

5. [ ] Verify AgUi.Chat.BlazorServer builds
6. [ ] Test OpenCode + generic AG-UI chat
7. [ ] Verify streaming works

### Week 3: Blazor Server Full

8. [ ] Create AgUi.IDE.BlazorServer
9. [ ] Wire up IDEView component
10. [ ] Configure all child panels
11. [ ] Test all features

### Week 4: WASM Minimal

12. [ ] Create AgUi.Chat.BlazorWasm
13. [ ] Test offline mode
14. [ ] Verify reconnection handling

### Week 5: WASM Full

15. [ ] Create AgUi.IDE.BlazorWasm
16. [ ] Reuse components (proves portability)
17. [ ] WASM-specific optimizations

### Week 6: Desktop

18. [ ] Create AgUi.IDE.Desktop (MAUI)
19. [ ] Reuse components (third host type!)
20. [ ] OpenCode process management
21. [ ] Build installers

---

## Work Per Sample

Each sample needs:

### Project Files
- `.csproj` - Project configuration
- Reference `LionFire.OpenCode.Blazor`
- Reference `LionFire.OpenCode.Serve`

### Configuration
- `Program.cs` / `MauiProgram.cs` - Service registration
- OpenCode API client setup
- Project manager setup

### Pages/Components
- Main IDE page or entry point
- Component integration
- Event handling
- State management

### Documentation
- `README.md` - Quick start guide
- `TODO.md` - Task checklist
- Architecture notes (if unique)

---

## Cross-Repository Development

During development, reference LionFire.OpenCode.Blazor as project reference:

```xml
<ItemGroup>
  <!-- Local opencode-dotnet projects -->
  <ProjectReference Include="../../src/LionFire.OpenCode.Serve/..." />
  <ProjectReference Include="../../src/LionFire.OpenCode.Blazor/..." />

  <!-- Cross-repo: ag-ui-blazor during development -->
  <ProjectReference Include="/src/ag-ui-blazor/src/LionFire.AgUi.Blazor.Server/..."
                    Condition="Exists('/src/ag-ui-blazor')" />
</ItemGroup>
```

---

## Key Features by Sample

| Feature | Server Min | Server Full | WASM Min | WASM Full | Desktop |
|---------|-----------|-------------|----------|-----------|---------|
| Basic chat | ✅ | ✅ | ✅ | ✅ | ✅ |
| Session diffs | ❌ | ✅ | ❌ | ✅ | ✅ |
| File browser | ❌ | ✅ | ❌ | ✅ | ✅ |
| PTY terminal | ❌ | ✅ | ❌ | ✅ | ✅ |
| Offline mode | ❌ | ❌ | ✅ | ✅ | ✅ |
| Native dialogs | ❌ | ❌ | ❌ | ❌ | ✅ |

---

## Success Criteria

### Per Sample

Each sample must:
- [ ] Build without errors
- [ ] Have README with prerequisites
- [ ] Have TODO.md with checklist
- [ ] Connect to `opencode serve` successfully
- [ ] Stream responses in real-time
- [ ] Handle errors gracefully

### Overall

- [ ] All 5 samples build without errors
- [ ] Components reused across all samples
- [ ] Desktop app builds for Windows and Mac
- [ ] Documentation complete for all samples
- [ ] Visual fidelity matches original > 95%

---

## Documentation Structure

Each sample gets:
1. **README.md** - Quick start, prerequisites, how to run
2. **TODO.md** - Hierarchical checklist of tasks

Plus:
- **examples/PLAN.md** (this file) - Overall plan
- **examples/README.md** - Index of all samples

---

*Ready to scaffold. Each project will have detailed TODO.md with specific implementation tasks.*
