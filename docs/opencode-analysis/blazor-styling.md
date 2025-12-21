# Blazor Styling Inventory - AgUi.IDE.BlazorServer

**Project**: `/src/opencode-dotnet/examples/AgUi.IDE.BlazorServer/`
**CSS Architecture**: Embedded component styles + MudBlazor + CSS variables

## Styling Approach

### CSS Strategy

1. **Embedded `<style>` blocks** - Each .razor file contains its own styles
2. **MudBlazor components** - Using pre-built component library
3. **CSS Variables** - Custom `--opencode-*` variables for theming
4. **No external CSS files** - All styles inline with components

### Component Libraries

- **MudBlazor** - Primary UI component library
- **No Tailwind** - Unlike OpenCode which uses Tailwind
- **No CSS preprocessor** - Plain CSS only

## CSS Variables Defined

### Background Colors

```css
--opencode-bg-base: #131010
--opencode-bg-raised: #1b1818
--opencode-bg-raised-stronger: #1e1a1a
--opencode-bg-hover: rgba(255, 255, 255, 0.05)
```

### Border Colors

```css
--opencode-border-base: #333
--opencode-border-strong: #3d3d3d
```

### Text Colors

```css
--opencode-text-strong: #f5f5f4
--opencode-text-base: #e7e5e4 (also #e6e1dc in some places)
--opencode-text-weak: #a8a29e (also #78716c)
--opencode-text-weaker: #78716c (also #57534e)
```

### Accent Colors

```css
--opencode-accent: #4ade80  /* Green - used for cursor/highlights */
--mud-palette-primary: #7c3aed  /* Purple - MudBlazor primary */
--mud-palette-primary-darken: #6d28d9
```

### State Colors (from MudBlazor)

```css
--mud-palette-info: #2196f3
--mud-palette-success: #4caf50
--mud-palette-warning: #ffa726
--mud-palette-error: #f44336
--mud-palette-grey-default: #9e9e9e
```

## Typography

### Font Families Used

```css
/* Chat/Code */
font-family: 'JetBrains Mono', 'Fira Code', monospace

/* General (inherited from MudBlazor) */
font-family: inherit
```

### Font Sizes

```css
12px - Small text (captions, timestamps, code)
13px - Tool names, labels
14px - Base text size
16px - Dialog titles
```

### Font Weights

```css
500 - Medium (headers, labels)
600 - Semi-bold (headings)
```

## Component-Specific Styles

### ChatInput

```css
.chat-input-form {
    background: var(--opencode-bg-raised-stronger, #1e1a1a);
    border: 1px solid var(--opencode-border-strong, #3d3d3d);
    border-radius: 6px;
}

.chat-input-form.focused {
    border-color: transparent;
    box-shadow: 0 0 0 2px var(--mud-palette-primary, #7c3aed);
}

.chat-textarea {
    padding: 12px 20px;
    font-size: 14px;
    line-height: 1.5;
    min-height: 44px;
}
```

### ChatMessage

```css
.chat-message {
    gap: 12px;
    padding: 12px 16px;
}

.assistant-message {
    background: var(--opencode-bg-raised, #1b1818);
    border-radius: 8px;
}

.message-avatar {
    width: 32px;
    height: 32px;
    border-radius: 50%;
}
```

### Code Blocks

```css
.code-block {
    background: #0d0d0d;
    border-radius: 8px;
}

.code-block code {
    font-family: 'JetBrains Mono', 'Fira Code', monospace;
    font-size: 13px;
    line-height: 1.5;
}
```

### Tool Calls

```css
.tool-call-block {
    background: var(--opencode-bg-base, #131010);
    border-left: 3px solid var(--opencode-border-base, #333);
    border-radius: 4px;
}

.tool-pending { border-left-color: #9e9e9e; }
.tool-running { border-left-color: #2196f3; }
.tool-completed { border-left-color: #4caf50; }
.tool-error { border-left-color: #f44336; }
```

### Diff Viewer

```css
.line-added { background: rgba(74, 222, 128, 0.1); }
.line-removed { background: rgba(248, 113, 113, 0.1); }

.additions { color: #4ade80; }
.deletions { color: #f87171; }
```

## Spacing Patterns

### Padding

- **Small**: 4px, 6px
- **Medium**: 8px, 10px, 12px
- **Large**: 16px, 20px, 24px

### Gaps

- **Tight**: 2px, 4px
- **Normal**: 8px
- **Wide**: 12px, 16px

### Margins

- **mb-1**: 4px (MudBlazor class)
- **mb-2, mb-3, mb-4**: 8px, 12px, 16px

## Border Radius

```css
3px - Small elements (kbd)
4px - Buttons, chips, tool blocks
6px - Input fields, dialogs
8px - Cards, messages, code blocks
```

## Shadows

```css
/* Dialog shadow */
box-shadow: 0 8px 32px rgba(0, 0, 0, 0.4);

/* Dropdown shadow */
box-shadow: 0 4px 12px rgba(0, 0, 0, 0.3);

/* Focus ring */
box-shadow: 0 0 0 2px var(--mud-palette-primary, #7c3aed);
```

## Animations

### Cursor Blink

```css
@keyframes blink {
    0%, 50% { opacity: 1; }
    51%, 100% { opacity: 0; }
}

.streaming-cursor {
    animation: blink 1s infinite;
    color: var(--opencode-accent, #4ade80);
}
```

## Theme Configuration

### Dark Mode Only

The current implementation is **dark mode only**:
- No light mode variant
- No theme switching mechanism
- Hard-coded dark colors

### MudBlazor Theme

MudBlazor theme likely configured in Program.cs with:
- Primary color: #7c3aed (purple)
- Dark background palette

## Issues Identified

### Inconsistent Variable Naming

Some variables have duplicate definitions with different values:
- `--opencode-text-base` vs `--opencode-text-weak` inconsistency
- Some components use raw hex values instead of variables

### Missing Variables

OpenCode variables not defined in Blazor:
- `--background-strong`, `--background-stronger`
- `--surface-*` variables
- `--icon-*` variables
- `--border-selected`
- All syntax highlighting colors
- All semantic state colors

### Hardcoded Colors

Many colors are hardcoded rather than using variables:
- `#0d0d0d` (code block background)
- `rgba(74, 222, 128, 0.1)` (diff add background)
- `#6ba3ff` (hunk header)

## Comparison with OpenCode

| Aspect | OpenCode | Blazor |
|--------|----------|--------|
| CSS Files | Separate per component | Embedded in .razor |
| Variables | 100+ theme variables | ~15 custom variables |
| Color Scales | 12-step color scales | Single colors only |
| Fonts | Geist, Berkeley Mono | JetBrains Mono only |
| Theme Variants | Light + Dark + Paper | Dark only |
| Syntax Colors | 12+ syntax colors | None defined |
| Animations | Keyframes + stagger | Cursor blink only |
