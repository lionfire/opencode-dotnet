# OpenCode Web UI Styling System

**Source**: `/dv/opencode/packages/ui/src/styles/`
**CSS Architecture**: CSS Variables + Component CSS + Tailwind

## File Structure

| File | Size | Purpose |
|------|------|---------|
| index.css | 2KB | Main entry, imports all files |
| theme.css | 48KB | Theme variables (largest file) |
| colors.css | 18KB | Color palette primitives |
| base.css | 8KB | CSS reset/normalize |
| animations.css | 2KB | Animation keyframes |
| utilities.css | 4KB | Utility classes |
| tailwind/index.css | - | Tailwind-specific styles |

## Theme System

### Font Families

```css
:root {
  --font-family-sans: "Geist", "Geist Fallback";
  --font-family-sans--font-feature-settings: "ss03" 1;

  --font-family-mono: "Berkeley Mono", "Berkeley Mono Fallback";
  --font-family-mono--font-feature-settings: "ss01" 1;
}
```

### Font Sizes

```css
:root {
  --font-size-small: 12px;
  --font-size-base: 14px;
  --font-size-large: 16px;
  --font-size-x-large: 20px;

  --font-weight-regular: 400;
  --font-weight-medium: 500;

  --line-height-large: 20px;
  --line-height-x-large: 24px;
  --line-height-2x-large: 30px;

  --letter-spacing-normal: 0;
  --letter-spacing-tight: -0.16px;
  --letter-spacing-tightest: -0.32px;
}
```

### Spacing

```css
:root {
  --spacing: 0.25rem;  /* Base spacing unit (4px) */
}
```

### Breakpoints

```css
:root {
  --breakpoint-sm: 40rem;   /* 640px */
  --breakpoint-md: 48rem;   /* 768px */
  --breakpoint-lg: 64rem;   /* 1024px */
  --breakpoint-xl: 80rem;   /* 1280px */
  --breakpoint-2xl: 96rem;  /* 1536px */
}
```

### Container Sizes

```css
:root {
  --container-3xs: 16rem;  /* 256px */
  --container-2xs: 18rem;  /* 288px */
  --container-xs: 20rem;   /* 320px */
  --container-sm: 24rem;   /* 384px */
  --container-md: 28rem;   /* 448px */
  --container-lg: 32rem;   /* 512px */
  --container-xl: 36rem;   /* 576px */
  --container-2xl: 42rem;  /* 672px */
  --container-3xl: 48rem;  /* 768px */
  --container-4xl: 56rem;  /* 896px */
  --container-5xl: 64rem;  /* 1024px */
  --container-6xl: 72rem;  /* 1152px */
  --container-7xl: 80rem;  /* 1280px */
}
```

### Border Radius

```css
:root {
  --radius-xs: 0.125rem;  /* 2px */
  --radius-sm: 0.25rem;   /* 4px */
  --radius-md: 0.375rem;  /* 6px */
  --radius-lg: 0.5rem;    /* 8px */
  --radius-xl: 0.625rem;  /* 10px */
}
```

### Shadows

```css
:root {
  --shadow-xs: 0 1px 2px -1px rgba(19, 16, 16, 0.04),
               0 1px 2px 0 rgba(19, 16, 16, 0.06),
               0 1px 3px 0 rgba(19, 16, 16, 0.08);

  --shadow-md: 0 6px 8px -4px rgba(19, 16, 16, 0.12),
               0 4px 3px -2px rgba(19, 16, 16, 0.12),
               0 1px 2px -1px rgba(19, 16, 16, 0.12);

  /* Border shadows (for focus states) */
  --shadow-xs-border: 0 0 0 1px var(--border-base), ...;
  --shadow-xs-border-select: 0 0 0 3px var(--border-weak-selected), ...;
  --shadow-xs-border-focus: ..., 0 0 0 3px var(--border-selected);
}
```

## Color Palette

### Color Scales

Each color has a 12-step scale (1-12) with both solid and alpha variants.

**Neutral Scales**:
- `smoke` - Warm gray (primary neutral)
- `ink` - Cool gray (paper theme)

**Brand/Accent Colors**:
- `yuzu` - Yellow (brand accent)
- `cobalt` - Blue (interactive/links)
- `apple` - Green (success)
- `ember` - Red (critical/error)
- `amber` - Orange (warning)
- `solaris` - Yellow-orange (warning alt)
- `mint` - Teal (diff additions)
- `lilac` - Purple (info)
- `purple` - Purple (agent icons)
- `cyan` - Cyan (agent icons)
- `blue` - Blue (diff hidden)

### Light Mode Colors (Default)

```css
:root {
  color-scheme: light;

  /* Backgrounds */
  --background-base: #f8f7f7;
  --background-weak: var(--smoke-light-3);      /* #f1f0f0 */
  --background-strong: var(--smoke-light-1);    /* #fdfcfc */
  --background-stronger: #fcfcfc;

  /* Surfaces */
  --surface-base: var(--smoke-light-alpha-2);
  --surface-base-hover: #0500000f;
  --surface-raised-base: var(--smoke-light-alpha-1);
  --surface-strong: #ffffff;

  /* Text */
  --text-base: var(--smoke-light-11);           /* #656363 */
  --text-weak: var(--smoke-light-9);            /* #8e8b8b */
  --text-weaker: var(--smoke-light-8);          /* #bcbbbb */
  --text-strong: var(--smoke-light-12);         /* #211e1e */

  /* Borders */
  --border-base: var(--smoke-light-alpha-7);
  --border-weak-base: var(--smoke-light-alpha-5);
  --border-strong-base: var(--smoke-light-alpha-7);

  /* Icons */
  --icon-base: var(--smoke-light-9);
  --icon-weak-base: var(--smoke-light-7);
  --icon-strong-base: var(--smoke-light-12);

  /* Interactive */
  --text-interactive-base: var(--cobalt-light-9);  /* Blue */
  --border-selected: var(--cobalt-light-alpha-9);
  --surface-interactive-base: var(--cobalt-light-3);
}
```

### Dark Mode Colors

Dark mode is triggered via `@media (prefers-color-scheme: dark)`:

```css
@media (prefers-color-scheme: dark) {
  :root {
    color-scheme: dark;

    /* Backgrounds */
    --background-base: var(--smoke-dark-1);     /* #131010 */
    --background-weak: #1c1717;
    --background-strong: #151313;

    /* Text */
    --text-base: var(--smoke-dark-alpha-11);
    --text-weak: var(--smoke-dark-alpha-9);
    --text-strong: var(--smoke-dark-alpha-12);

    /* Surfaces */
    --surface-base: var(--smoke-dark-alpha-2);
    --surface-strong: var(--smoke-dark-alpha-7);
  }
}
```

### Theme Variants

The system supports named themes via `data-theme` attribute:

- Default: `oc-1-light` / `oc-1-dark` (auto via prefers-color-scheme)
- `oc-2-paper`: Alternate paper theme with ink gray scale

```css
html[data-theme="oc-2-paper"] {
  --background-base: #f7f8f8;
  --background-weak: var(--ink-light-3);
  /* ... */
}
```

## Syntax Highlighting Colors

For code blocks via Shiki:

### Light Mode Syntax

```css
:root {
  --syntax-comment: var(--text-weaker);
  --syntax-string: #007663;        /* Teal */
  --syntax-keyword: var(--text-weak);
  --syntax-primitive: #fb7f51;     /* Orange */
  --syntax-variable: var(--text-strong);
  --syntax-property: #ec6cc8;      /* Pink */
  --syntax-type: #738400;          /* Olive */
  --syntax-constant: #00b2b9;      /* Cyan */
  --syntax-operator: var(--text-weak);
  --syntax-punctuation: var(--text-weaker);
  --syntax-info: #0091a7;
  --syntax-success: var(--apple-light-10);
  --syntax-warning: var(--amber-light-10);
  --syntax-critical: var(--ember-light-9);

  /* Diff */
  --syntax-diff-add: var(--mint-light-11);
  --syntax-diff-delete: var(--ember-light-11);
}
```

### Dark Mode Syntax

```css
@media (prefers-color-scheme: dark) {
  --syntax-string: #00ceb9;        /* Bright teal */
  --syntax-primitive: #ffba92;     /* Light orange */
  --syntax-property: #ff9ae2;      /* Light pink */
  --syntax-type: #ecf58c;          /* Light olive */
  --syntax-constant: #93e9f6;      /* Light cyan */
  --syntax-info: #93e9f6;

  --syntax-diff-add: var(--mint-dark-11);
  --syntax-diff-delete: var(--ember-dark-11);
}
```

## Markdown Colors

For rendered markdown content:

```css
/* Light Mode */
--markdown-heading: #d68c27;       /* Orange */
--markdown-text: #1a1a1a;
--markdown-link: #3b7dd8;          /* Blue */
--markdown-link-text: #318795;     /* Teal */
--markdown-code: #3d9a57;          /* Green */
--markdown-block-quote: #b0851f;   /* Gold */
--markdown-strong: #d68c27;
--markdown-list-item: #3b7dd8;

/* Dark Mode */
--markdown-heading: #9d7cd8;       /* Purple */
--markdown-text: #eeeeee;
--markdown-link: #fab283;          /* Peach */
--markdown-link-text: #56b6c2;     /* Cyan */
--markdown-code: #7fd88f;          /* Green */
--markdown-strong: #f5a742;        /* Orange */
```

## Semantic Color Variables

### State Colors

```css
/* Success (Green) */
--surface-success-base: var(--apple-light-3);
--surface-success-strong: var(--apple-light-9);
--border-success-base: var(--apple-light-6);
--text-on-success-base: var(--apple-light-10);
--icon-success-base: var(--apple-light-7);

/* Warning (Amber/Solaris) */
--surface-warning-base: var(--solaris-light-3);
--surface-warning-strong: var(--solaris-light-9);
--border-warning-base: var(--solaris-light-6);
--icon-warning-base: var(--amber-light-7);

/* Critical/Error (Ember) */
--surface-critical-base: var(--ember-light-3);
--surface-critical-strong: var(--ember-light-9);
--border-critical-base: var(--ember-light-6);
--text-on-critical-base: var(--ember-light-10);
--icon-critical-base: var(--ember-light-10);

/* Info (Lilac) */
--surface-info-base: var(--lilac-light-3);
--surface-info-strong: var(--lilac-light-9);
--border-info-base: var(--lilac-light-6);
```

### Diff Colors

```css
/* Additions (Mint) */
--surface-diff-add-base: var(--mint-light-3);
--surface-diff-add-weak: var(--mint-light-2);
--surface-diff-add-strong: var(--mint-light-5);
--text-diff-add-base: var(--mint-light-11);
--icon-diff-add-base: var(--mint-light-11);

/* Deletions (Ember) */
--surface-diff-delete-base: var(--ember-light-3);
--surface-diff-delete-weak: var(--ember-light-2);
--surface-diff-delete-strong: var(--ember-light-6);
--text-diff-delete-base: var(--ember-light-10);
--icon-diff-delete-base: var(--ember-light-10);

/* Unchanged */
--surface-diff-unchanged-base: #ffffff00;
--surface-diff-skip-base: var(--smoke-light-2);
```

### Agent Icon Colors

```css
--icon-agent-plan-base: var(--purple-light-9);   /* Purple */
--icon-agent-docs-base: var(--amber-light-9);    /* Amber */
--icon-agent-ask-base: var(--cyan-light-9);      /* Cyan */
--icon-agent-build-base: var(--cobalt-light-9);  /* Blue */
```

## Animations

### Keyframes

```css
/* Pulse animation for loading states */
@keyframes pulse-opacity {
  0%, 100% { opacity: 0; }
  50% { opacity: 1; }
}
:root {
  --animate-pulse: pulse-opacity 2s ease-in-out infinite;
}

/* Fade up for staggered text reveal */
@keyframes fadeUp {
  from {
    opacity: 0;
    transform: translateY(5px);
  }
  to {
    opacity: 1;
    transform: translateY(0);
  }
}
```

### Staggered Animation Classes

```css
.fade-up-text {
  animation: fadeUp 0.4s ease-out forwards;
  opacity: 0;
}

/* Staggered delays up to 30 children */
.fade-up-text:nth-child(1) { animation-delay: 0.1s; }
.fade-up-text:nth-child(2) { animation-delay: 0.2s; }
/* ... */
.fade-up-text:nth-child(30) { animation-delay: 3s; }
```

## CSS Reset (base.css)

Key resets applied:

- Box-sizing: border-box on all elements
- Margins/padding: reset to 0
- Font inheritance: inputs, buttons inherit fonts
- List styles: removed by default
- Media elements: block display, responsive sizing
- Form elements: transparent backgrounds, no border-radius

## Component CSS Pattern

Each component has a companion CSS file using:

1. **`data-component`** attribute on root element
2. **`data-slot`** attributes on semantic parts
3. **CSS variables** for theming
4. **State via `data-*`** attributes

Example from `session-turn.css`:

```css
[data-component="session-turn"] {
  /* Root styles */
}

[data-component="session-turn"] [data-slot="session-turn-message-header"] {
  display: flex;
  gap: calc(var(--spacing) * 2);
}

[data-component="session-turn"] [data-slot="session-turn-message-title"] h1 {
  font-size: var(--font-size-x-large);
  font-weight: var(--font-weight-medium);
  color: var(--text-strong);
}
```

## Blazor Replication Notes

### Direct Port

These can be directly copied to Blazor CSS:

1. All CSS variable definitions from theme.css
2. Color palette from colors.css
3. Base reset from base.css
4. Animation keyframes from animations.css

### Adaptation Needed

1. **Component CSS**: Convert `data-slot` patterns to CSS classes or maintain `data-*` approach
2. **Dark mode**: Use CSS media query or Blazor theme service
3. **Tailwind utilities**: Replace with custom utility classes or keep minimal Tailwind

### Font Files

Font files available at `/packages/ui/src/assets/fonts/`:
- geist.woff2, geist.ttf
- geist-mono.woff2, geist-mono.ttf
- geist-medium.otf, geist-italic*.otf
- tx-02*.otf (secondary font)
