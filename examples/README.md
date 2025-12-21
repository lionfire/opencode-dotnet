# OpenCode Blazor Samples

> **Goal:** Demonstrate OpenCode .NET SDK with AG-UI integration across all Blazor hosting models
> **Special Focus:** Faithful replication of `opencode web` UI in Blazor

---

## Documentation Index

| Document | Purpose |
|----------|---------|
| [PLAN.md](./PLAN.md) | Master plan - sample overview and build order |
| [OPENCODE-WEB-UI-ANALYSIS.md](./OPENCODE-WEB-UI-ANALYSIS.md) | Analysis of original SolidJS UI |
| [UI-REPLICATION-GUIDE.md](./UI-REPLICATION-GUIDE.md) | How to replicate SolidJS → Blazor |
| [../docs/ARCHITECTURE.md](../docs/ARCHITECTURE.md) | **How AG-UI and OpenCode APIs work together** ⭐ |

---

## Sample Projects

### Blazor Server (Web)

**[AgUi.Chat.BlazorServer/](./AgUi.Chat.BlazorServer/)** Minimal
- Simplest possible example (< 30 lines)
- Generic AG-UI chat
- Proves OpenCode SDK + ag-ui-blazor works

**[AgUi.IDE.BlazorServer/](./AgUi.IDE.BlazorServer/)** Full
- **Replicates `opencode web` UI**
- Multi-panel layout: file tree, chat, diffs, terminal
- All OpenCode features exposed
- Uses pre-built components from `LionFire.OpenCode.Blazor`

### Blazor WASM (SPA)

**[AgUi.Chat.BlazorWasm/](./AgUi.Chat.BlazorWasm/)** Minimal
- Client/Server split
- Offline support
- Minimal chat interface

**[AgUi.IDE.BlazorWasm/](./AgUi.IDE.BlazorWasm/)** Full
- Full IDE as single-page app
- Same features as Server variant
- Proves component portability
- Uses pre-built components from `LionFire.OpenCode.Blazor`

### Blazor Hybrid (Desktop)

**[AgUi.IDE.Desktop/](./AgUi.IDE.Desktop/)** Native App
- .NET MAUI with Blazor WebView
- Native Windows/Mac application
- Bundles `opencode serve` with app
- Uses same components as web versions!

---

## Quick Start

### Prerequisites

```bash
# 1. Ensure opencode is installed
opencode --version

# 2. Start opencode serve (required for all samples)
opencode serve --port 9123

# 3. Ensure ag-ui-blazor is built
cd /src/ag-ui-blazor
dotnet build
```

### Build Order

```bash
cd /src/opencode-dotnet/examples

# 1. Minimal sample
cd AgUi.Chat.BlazorServer
dotnet run

# 2. Full IDE
cd ../AgUi.IDE.BlazorServer
dotnet run

# 3. WASM variants (when ready)
# 4. Desktop app (when ready)
```

---

## UI Replication Philosophy

**"Faithful, not slavish"**

We aim to:
- Match the **visual design** (colors, spacing, layout)
- Match the **user experience** (interactions, animations)
- Match the **feature set** (diffs, terminal, file browser)

But we:
- Use Blazor patterns (not direct SolidJS translation)
- Use pre-built components from `LionFire.OpenCode.Blazor` (not custom from scratch)
- Optimize for Blazor Server (direct streaming)

**Result:** Feels like OpenCode web, built the Blazor way.

---

## Architecture

### Component Reuse Strategy

All UI components are provided by `LionFire.OpenCode.Blazor`, which samples consume:

```
┌────────────────────────────────────────┐
│   LionFire.OpenCode.Blazor             │
│  (OpenCode-specific components)        │
│                                        │
│  • IDEView, SessionTurn, MessagePart  │
│  • DiffViewer, FileTree                │
│  • PromptInput, PtyTerminal            │
└────────────────────────────────────────┘
              ↑ used by
     ┌────────┼────────┬─────────┐
     │        │        │         │
     ▼        ▼        ▼         ▼
 Server    Server    WASM     Desktop
 (min)     (full)   (full)   (native)
```

### Dependency Flow

```
All Samples
  ↓
LionFire.OpenCode.Blazor (OpenCode-specific components)
  ↓
├── LionFire.AgUi.Blazor (Generic AG-UI)
└── LionFire.OpenCode.Serve (SDK)
```

---

## Key Differences from Old Structure

### What Changed

- **Removed:** Shared.Components library (moved to src/)
- **Now used:** `LionFire.OpenCode.Blazor` package with pre-built components
- **Why:** Centralized component development, easier to maintain and update

### Migration Notes

All samples now reference `LionFire.OpenCode.Blazor` instead of `Shared.Components`:
- Cleaner dependency structure
- Single source of truth for UI components
- Pre-built, tested components ready to use
- Just wire up configuration and services

---

## Development Tips

### Running Locally

```bash
# Terminal 1: OpenCode serve
opencode serve --port 9123

# Terminal 2: Sample app
cd examples/AgUi.IDE.BlazorServer
dotnet watch

# Opens browser to http://localhost:5000
```

### Comparing with Original

```bash
# Terminal 3: Original OpenCode web
opencode web --port 4096

# Now you have both:
# - Original SolidJS: http://localhost:4096
# - Blazor replica: http://localhost:5000
# Compare side-by-side!
```

---

## Progress Tracking

Each sample has a `TODO.md` with hierarchical checklist:

```bash
# Check overall progress
find . -name "TODO.md" -exec echo {} \; -exec grep -c "- \[ \]" {} \;

# See what's done
find . -name "TODO.md" -exec grep "- \[x\]" {} \;
```

---

## Success Metrics

### Per Sample

- [ ] Builds without errors
- [ ] Runs on first try
- [ ] Connects to opencode serve
- [ ] UI matches screenshots
- [ ] All features work

### Overall

- [ ] 5/5 samples complete (excluding BasicUsage/ChatClientUsage)
- [ ] Visual fidelity > 95%
- [ ] Feature parity 100%
- [ ] Performance comparable to original
- [ ] Desktop app distributable (MSI/DMG)

---

## Contributing

When implementing:
1. Study the original SolidJS component in `/dv/opencode`
2. Read [UI-REPLICATION-GUIDE.md](./UI-REPLICATION-GUIDE.md)
3. Follow the TODO.md checklist for each sample
4. Test against running `opencode web` for visual comparison
5. Update TODO.md as you complete tasks

---

**Let's build the best Blazor UI for OpenCode!**
