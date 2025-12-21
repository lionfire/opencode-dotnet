# Styling System Comparison: OpenCode vs Blazor

Detailed comparison of CSS styling systems between OpenCode web UI and the Blazor implementation.

## CSS Variable Comparison

### Background Colors

| Purpose | OpenCode Variable | OpenCode Value (Dark) | Blazor Variable | Blazor Value | Match |
|---------|------------------|----------------------|-----------------|--------------|-------|
| Base background | `--background-base` | `#131010` | `--opencode-bg-base` | `#131010` | Yes |
| Raised surface | `--background-weak` | `#1c1717` | `--opencode-bg-raised` | `#1b1818` | Close |
| Strong surface | `--background-strong` | `#151313` | - | - | Missing |
| Stronger surface | `--background-stronger` | `#fcfcfc` | `--opencode-bg-raised-stronger` | `#1e1a1a` | Different |
| Hover state | `--surface-base-hover` | `#0500000f` | `--opencode-bg-hover` | `rgba(255,255,255,0.05)` | Different |

### Text Colors

| Purpose | OpenCode Variable | OpenCode Value (Dark) | Blazor Variable | Blazor Value | Match |
|---------|------------------|----------------------|-----------------|--------------|-------|
| Strong text | `--text-strong` | smoke-dark-12 | `--opencode-text-strong` | `#f5f5f4` | Close |
| Base text | `--text-base` | smoke-dark-11 | `--opencode-text-base` | `#e7e5e4` | Close |
| Weak text | `--text-weak` | smoke-dark-9 | `--opencode-text-weak` | `#a8a29e` | Close |
| Weaker text | `--text-weaker` | smoke-dark-8 | `--opencode-text-weaker` | `#78716c` | Close |

### Border Colors

| Purpose | OpenCode Variable | OpenCode Value | Blazor Variable | Blazor Value | Match |
|---------|------------------|----------------|-----------------|--------------|-------|
| Base border | `--border-base` | smoke-alpha-7 | `--opencode-border-base` | `#333` | Close |
| Strong border | `--border-strong-base` | smoke-alpha-7 | `--opencode-border-strong` | `#3d3d3d` | Close |
| Weak border | `--border-weak-base` | smoke-alpha-5 | - | - | Missing |
| Selected border | `--border-selected` | cobalt-alpha-9 | - | - | Missing |

### Interactive Colors

| Purpose | OpenCode Variable | OpenCode Value | Blazor Variable | Blazor Value | Match |
|---------|------------------|----------------|-----------------|--------------|-------|
| Primary accent | `--text-interactive-base` | cobalt-9 | `--mud-palette-primary` | `#7c3aed` | Different |
| Primary hover | - | - | `--mud-palette-primary-darken` | `#6d28d9` | N/A |
| Success | `--text-on-success-base` | apple-10 | (MudBlazor) | `#4caf50` | Similar |
| Error | `--text-on-critical-base` | ember-10 | (MudBlazor) | `#f44336` | Similar |
| Warning | `--icon-warning-base` | amber-7 | (MudBlazor) | `#ffa726` | Similar |

### Diff Colors

| Purpose | OpenCode Variable | OpenCode Value | Blazor Equivalent | Match |
|---------|------------------|----------------|-------------------|-------|
| Add background | `--surface-diff-add-base` | mint-3 | `rgba(74,222,128,0.1)` | Close |
| Add text | `--text-diff-add-base` | mint-11 | `#4ade80` | Close |
| Delete background | `--surface-diff-delete-base` | ember-3 | `rgba(248,113,113,0.1)` | Close |
| Delete text | `--text-diff-delete-base` | ember-10 | `#f87171` | Close |

### Missing in Blazor

| OpenCode Variable Category | Count | Notes |
|---------------------------|-------|-------|
| Surface variables | 8+ | `--surface-*` for cards, dialogs |
| Icon variables | 10+ | `--icon-*` for icon colors |
| Syntax variables | 12+ | `--syntax-*` for code highlighting |
| Markdown variables | 8+ | `--markdown-*` for rendered markdown |
| Agent icon colors | 4 | `--icon-agent-*` |

---

## Typography Comparison

### Font Families

| Purpose | OpenCode | Blazor | Status |
|---------|----------|--------|--------|
| Sans-serif | Geist | (inherit) | Different |
| Monospace | Berkeley Mono | JetBrains Mono, Fira Code | Different |
| Secondary | TX-02 | - | Missing |

**Font Loading**: OpenCode uses custom font loader component; Blazor uses system fonts.

### Font Sizes

| Size | OpenCode | Blazor | Match |
|------|----------|--------|-------|
| Small | 12px | 12px | Yes |
| Base | 14px | 14px | Yes |
| Large | 16px | 16px | Yes |
| X-Large | 20px | - | Missing |

### Font Weights

| Weight | OpenCode | Blazor | Match |
|--------|----------|--------|-------|
| Regular | 400 | 400 | Yes |
| Medium | 500 | 500 | Yes |
| Semi-bold | - | 600 | Extra |

### Line Heights

| Purpose | OpenCode | Blazor | Match |
|---------|----------|--------|-------|
| Code | 1.5 | 1.5 | Yes |
| Text | 20px-30px | 1.6 | Different units |

---

## Spacing Comparison

### Base Unit

| System | Value | Pattern |
|--------|-------|---------|
| OpenCode | 4px (`--spacing: 0.25rem`) | Multiples: 4, 8, 12, 16, 20, 24 |
| Blazor | Various | Ad-hoc: 4, 6, 8, 10, 12, 16, 20, 24 |

### Common Spacing Values

| Element | OpenCode | Blazor | Match |
|---------|----------|--------|-------|
| Input padding | 12px 16px | 12px 20px | Close |
| Message padding | 16px | 12px 16px | Close |
| Dialog padding | 16px 20px | 16px 20px | Yes |
| Gap between items | 8px | 8px | Yes |

### Border Radius

| Size | OpenCode | Blazor | Match |
|------|----------|--------|-------|
| XS | 2px | 3px (kbd) | Close |
| SM | 4px | 4px | Yes |
| MD | 6px | 6px | Yes |
| LG | 8px | 8px | Yes |

---

## Component Styling Differences

### Chat Input

| Property | OpenCode | Blazor | Difference |
|----------|----------|--------|------------|
| Background | `--surface-base` | `--opencode-bg-raised-stronger` | Different variable |
| Border | `--border-base` | `--opencode-border-strong` | Different variable |
| Focus ring | `--border-selected` | `--mud-palette-primary` | Different color |
| Border radius | 6px | 6px | Same |

### Chat Message

| Property | OpenCode | Blazor | Difference |
|----------|----------|--------|------------|
| Avatar size | 24px | 32px | Larger in Blazor |
| Avatar shape | Rounded square | Circle | Different shape |
| Message gap | 8px | 12px | Larger in Blazor |
| User background | transparent | transparent | Same |
| Assistant background | surface-base | bg-raised | Similar |

### Code Blocks

| Property | OpenCode | Blazor | Difference |
|----------|----------|--------|------------|
| Background | `--surface-base` | `#0d0d0d` | Hardcoded |
| Font | Berkeley Mono | JetBrains Mono | Different font |
| Header style | Minimal | Has language label | More prominent |
| Syntax colors | Full theme | None | Missing |

### Tool Call Display

| Property | OpenCode (BasicTool) | Blazor | Difference |
|----------|---------------------|--------|------------|
| Layout | Collapsible with trigger | Always visible | Different UX |
| Icon | Custom icon per tool | MudBlazor icons | Different icons |
| Border style | None | Left border 3px | Different style |
| Progress | Spinner | MudProgressLinear | Different indicator |

### Diff Viewer

| Property | OpenCode | Blazor | Difference |
|----------|----------|--------|------------|
| Library | @pierre/precision-diffs | Custom | Different rendering |
| Line numbers | 2 columns | 2 columns | Same |
| Add color | mint scale | rgba(74,222,128,0.1) | Similar |
| Remove color | ember scale | rgba(248,113,113,0.1) | Similar |
| Hunk header | Blue tint | Blue tint | Same |

---

## Theme System Gap Analysis

### Dark/Light Mode

| Feature | OpenCode | Blazor |
|---------|----------|--------|
| Default mode | Auto (prefers-color-scheme) | Dark only |
| Light mode | Full theme | Not implemented |
| Theme switching | data-theme attribute | None |
| Theme variants | 3 (oc-1-light, oc-1-dark, oc-2-paper) | 1 |

### CSS Variable Organization

| Aspect | OpenCode | Blazor |
|--------|----------|--------|
| Variable count | 100+ | ~15 |
| Naming convention | `--category-semantic-variant` | `--opencode-category-variant` |
| Color scales | 12-step per color | Single values |
| Alpha variants | Yes | No |
| Semantic variables | Extensive | Basic |

### Animation System

| Animation | OpenCode | Blazor |
|-----------|----------|--------|
| Cursor blink | Yes | Yes |
| Typewriter | Yes | No |
| Fade up stagger | Yes | No |
| Collapsible height | Yes | No |
| Arrow rotation | Yes | No |
| Pulse opacity | Yes | No |

---

## Migration Recommendations

### Priority 1: Variable System

**Action**: Create comprehensive CSS variable file

```css
/* Create: wwwroot/css/opencode-theme.css */

:root {
  /* Copy OpenCode's full variable set */
  --background-base: #131010;
  --background-weak: #1c1717;
  --background-strong: #151313;
  /* ... */
}

@media (prefers-color-scheme: light) {
  :root {
    --background-base: #f8f7f7;
    /* ... */
  }
}
```

### Priority 2: Typography

**Action**: Add Geist font family

1. Copy font files from OpenCode
2. Add @font-face declarations
3. Update CSS variables
4. Apply to body/components

### Priority 3: Syntax Highlighting

**Action**: Implement Shiki-style code coloring

1. Define `--syntax-*` variables
2. Either use Shiki via WASM or server-side
3. Or implement basic color mapping in Markdig

### Priority 4: Animations

**Action**: Port animation keyframes

```css
/* animations.css */
@keyframes fadeUp {
  from { opacity: 0; transform: translateY(5px); }
  to { opacity: 1; transform: translateY(0); }
}

@keyframes pulse-opacity {
  0%, 100% { opacity: 0; }
  50% { opacity: 1; }
}
```

### Priority 5: Component Refactoring

**Action**: Extract embedded styles to CSS files

1. Create `/wwwroot/css/components/` directory
2. Move `<style>` blocks to separate files
3. Adopt `data-component` / `data-slot` pattern
4. Reference variables consistently

---

## Summary

### Gaps by Severity

**Critical** (visual fidelity impacted):
- No syntax highlighting colors
- Missing font families (Geist)
- No light mode support
- Limited variable system

**Important** (UX impacted):
- Missing animations (typewriter, fade-up)
- No semantic color variables
- Hardcoded colors throughout

**Minor** (polish):
- Slight spacing differences
- Different avatar styling
- Missing agent icon colors

### Estimated Effort

| Task | Effort |
|------|--------|
| CSS variable system | 2-3 hours |
| Font integration | 1 hour |
| Syntax highlighting | 4-6 hours |
| Animation porting | 2 hours |
| Component refactoring | 4-6 hours |
| Light mode support | 2-3 hours |
| **Total** | **15-21 hours** |
