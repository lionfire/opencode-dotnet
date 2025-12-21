# OpenCode Web UI Dependencies

**Source**: `/dv/opencode/packages/ui/package.json`

## Runtime Dependencies

### UI Framework

| Package | Purpose |
|---------|---------|
| `solid-js` | Reactive UI framework (alternative to React) |
| `@kobalte/core` | Accessible UI component primitives |
| `solid-list` | List rendering utilities |
| `virtua` | Virtual scrolling for performance |

### OpenCode Integration

| Package | Purpose |
|---------|---------|
| `@opencode-ai/sdk` | OpenCode backend SDK (workspace dependency) |
| `@opencode-ai/util` | Shared utility functions (workspace dependency) |

### Content Rendering

| Package | Purpose |
|---------|---------|
| `marked` (16.2.0) | Markdown parser |
| `marked-shiki` (1.2.1) | Shiki integration for marked |
| `shiki` (3.9.2) | Syntax highlighter |
| `@shikijs/transformers` (3.9.2) | Shiki transformers |
| `@pierre/precision-diffs` | High-quality diff rendering |

### Utilities

| Package | Purpose |
|---------|---------|
| `remeda` | Functional utility library |
| `fuzzysort` | Fuzzy search/sort |
| `luxon` | Date/time handling |

### TypeScript Support

| Package | Purpose |
|---------|---------|
| `@typescript/native-preview` | TypeScript native preview features |

## Dev Dependencies

### Build Tools

| Package | Purpose |
|---------|---------|
| `vite` | Build tool and dev server |
| `vite-plugin-solid` | SolidJS Vite plugin |
| `vite-plugin-icons-spritesheet` (3.0.1) | Icon sprite generation |

### CSS

| Package | Purpose |
|---------|---------|
| `tailwindcss` | CSS framework |
| `@tailwindcss/vite` | Tailwind Vite plugin |

### TypeScript

| Package | Purpose |
|---------|---------|
| `typescript` | TypeScript compiler |
| `@types/bun` | Bun runtime types |
| `@tsconfig/node22` | Node 22 tsconfig base |

## Dependency Analysis for Blazor Replication

### Must Find Equivalents

| SolidJS Package | Blazor Equivalent |
|-----------------|-------------------|
| `solid-js` | Native Blazor + Razor Components |
| `@kobalte/core` | MudBlazor or Radzen or native HTML |
| `virtua` | Blazor virtualization |

### Can Reuse Concepts

| Package | Blazor Approach |
|---------|-----------------|
| `marked` | Markdig NuGet package |
| Syntax highlighting | Prism.js or highlight.js via JS interop, or Markdig extensions |
| `luxon` | Luxon.NET or TimeZoneConverter |
| `fuzzysort` | FuzzySharp or native LINQ |

### Need Custom Implementation

| Feature | Approach |
|---------|----------|
| Diff rendering | Third-party diff library or custom |
| Icon sprites | SVG embed or icon font |
| CSS variables | Direct port of CSS |

## Package Versions Note

The project uses a pnpm workspace with `catalog:` version references, meaning exact versions are defined in the root workspace configuration.

## Key Technical Decisions

1. **SolidJS over React**: Fine-grained reactivity, better performance for frequent updates
2. **@kobalte/core**: Accessible primitives similar to Radix UI for React
3. **Tailwind CSS**: Utility-first CSS with CSS variables for theming
4. **Marked + Shiki**: High-quality markdown with syntax highlighting
5. **Pierre Precision Diffs**: Premium diff rendering experience
