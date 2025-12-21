# OpenCode Web UI Directory Structure

**Source**: `/dv/opencode/packages/ui/`
**Framework**: SolidJS (not React)
**Build Tool**: Vite with Tailwind CSS

## Top-Level Structure

```
/packages/ui/
├── src/                    # Main source code
│   ├── assets/            # Static assets (icons, fonts, images)
│   ├── components/        # UI components (39 TSX files)
│   ├── context/           # SolidJS context providers
│   ├── hooks/             # Custom hooks
│   ├── pierre/            # Precision diffs worker integration
│   └── styles/            # CSS stylesheets
├── script/                # Build scripts
├── package.json           # Dependencies and exports
├── tsconfig.json          # TypeScript configuration
├── vite.config.ts         # Vite build configuration
└── sst-env.d.ts          # SST environment types
```

## Source Directory Breakdown

### `/src/assets/`
Static resources for the UI.

```
assets/
├── favicon/               # Favicon files
├── fonts/                 # Font files
│   ├── geist*.otf/ttf/woff2    # Geist font family (regular, medium, italic, mono)
│   └── tx-02*.otf/ttf/woff2    # TX-02 font family
├── icons/
│   ├── file-types/        # 600+ file type icons (SVG)
│   └── provider/          # 70+ AI provider icons (SVG)
└── images/                # General images
```

**Key Fonts**:
- **Geist**: Primary sans-serif font (regular, medium, italic variants)
- **Geist Mono**: Monospace variant for code
- **TX-02**: Secondary font (bold, italic, regular)

### `/src/components/` (39 Components)
All UI components, each with a corresponding CSS file.

| Component | File | Purpose |
|-----------|------|---------|
| Accordion | accordion.tsx | Expandable/collapsible sections |
| Avatar | avatar.tsx | User/agent avatars |
| BasicTool | basic-tool.tsx | Base tool display component |
| Button | button.tsx | Standard button |
| Card | card.tsx | Card container |
| Checkbox | checkbox.tsx | Checkbox input |
| Code | code.tsx | Code display |
| Collapsible | collapsible.tsx | Collapsible panel |
| Dialog | dialog.tsx | Modal dialog |
| Diff | diff.tsx | Diff viewer wrapper |
| DiffChanges | diff-changes.tsx | File diff change summary |
| DiffSSR | diff-ssr.tsx | Server-side rendered diff |
| DropdownMenu | dropdown-menu.tsx | Dropdown menu |
| Favicon | favicon.tsx | Dynamic favicon |
| FileIcon | file-icon.tsx | File type icon display |
| Font | font.tsx | Font loading component |
| Icon | icon.tsx | General icon display |
| IconButton | icon-button.tsx | Icon-only button |
| Input | input.tsx | Text input |
| List | list.tsx | Virtual list |
| Logo | logo.tsx | OpenCode logo |
| Markdown | markdown.tsx | Markdown renderer |
| MessageNav | message-nav.tsx | Message navigation |
| MessagePart | message-part.tsx | Message content renderer |
| MessageProgress | message-progress.tsx | Progress during response |
| ProgressCircle | progress-circle.tsx | Circular progress indicator |
| ProviderIcon | provider-icon.tsx | AI provider icon |
| ResizeHandle | resize-handle.tsx | Panel resize handle |
| Select | select.tsx | Select dropdown |
| SelectDialog | select-dialog.tsx | Selection dialog |
| SessionMessageRail | session-message-rail.tsx | Session message sidebar |
| SessionReview | session-review.tsx | Session review display |
| SessionTurn | session-turn.tsx | Chat turn display (main) |
| Spinner | spinner.tsx | Loading spinner |
| StickyAccordionHeader | sticky-accordion-header.tsx | Sticky header for accordion |
| Tabs | tabs.tsx | Tab navigation |
| Tag | tag.tsx | Tag/label |
| Tooltip | tooltip.tsx | Tooltip |
| Typewriter | typewriter.tsx | Typewriter text effect |

### `/src/context/` (4 Files)
SolidJS context providers for state management.

| File | Purpose |
|------|---------|
| data.tsx | Main data store (sessions, messages, parts) |
| diff.tsx | Diff component provider |
| helper.tsx | Context creation utilities |
| marked.tsx | Markdown configuration (marked + shiki) |

**Data Store Structure**:
```typescript
type Data = {
  session: Session[]
  session_status: { [sessionID: string]: SessionStatus }
  session_diff: { [sessionID: string]: FileDiff[] }
  message: { [sessionID: string]: Message[] }
  part: { [messageID: string]: Part[] }
}
```

### `/src/hooks/` (1 Hook)

| File | Purpose |
|------|---------|
| use-filtered-list.tsx | Filterable list hook with search |

### `/src/pierre/` (2 Files)
Integration with precision-diffs library for diff rendering.

| File | Purpose |
|------|---------|
| index.ts | Diff rendering initialization |
| worker.ts | Web worker for diff processing |

### `/src/styles/` (6 CSS Files)
Styling system using CSS variables and Tailwind.

| File | Size | Purpose |
|------|------|---------|
| index.css | 2KB | Main entry point, imports other files |
| theme.css | 48KB | Complete theme variables (largest) |
| colors.css | 18KB | Color palette definitions |
| base.css | 8KB | Base element styles |
| utilities.css | 4KB | Utility classes |
| animations.css | 2KB | Animation keyframes |

**Tailwind Integration**:
```
styles/tailwind/
└── index.css            # Tailwind-specific styles
```

## Package Exports

From `package.json`:
```json
{
  "./*": "./src/components/*.tsx",        // Components
  "./pierre": "./src/pierre/index.ts",    // Diff integration
  "./hooks": "./src/hooks/index.ts",      // Hooks
  "./context": "./src/context/index.ts",  // Contexts
  "./context/*": "./src/context/*.tsx",   // Individual contexts
  "./styles": "./src/styles/index.css",   // Styles
  "./styles/tailwind": "./src/styles/tailwind/index.css",
  "./icons/provider": "./src/components/provider-icons/types.ts",
  "./icons/file-type": "./src/components/file-icons/types.ts",
  "./fonts/*": "./src/assets/fonts/*"
}
```

## Build Configuration

**Dependencies** (runtime):
- `solid-js` - UI framework
- `@kobalte/core` - Accessible component primitives
- `@opencode-ai/sdk` - OpenCode SDK
- `@opencode-ai/util` - Utility functions
- `@pierre/precision-diffs` - Diff rendering
- `marked` + `marked-shiki` - Markdown rendering
- `shiki` - Syntax highlighting
- `fuzzysort` - Fuzzy search
- `luxon` - Date/time handling
- `remeda` - Utility functions
- `virtua` - Virtual scrolling
- `solid-list` - List rendering

**Dev Dependencies**:
- `vite` - Build tool
- `tailwindcss` + `@tailwindcss/vite` - CSS framework
- `vite-plugin-icons-spritesheet` - Icon sprite generation
- `vite-plugin-solid` - SolidJS plugin
- `typescript` - Type checking

## Icon Systems

### Provider Icons (70+)
Located in `/src/assets/icons/provider/`

Major providers included:
- anthropic.svg, openai.svg, google.svg
- azure.svg, amazon-bedrock.svg
- deepseek.svg, mistral.svg, cohere.svg
- groq.svg, fireworks-ai.svg, togetherai.svg
- ollama-cloud.svg, huggingface.svg
- And 60+ more...

### File Type Icons (600+)
Located in `/src/assets/icons/file-types/`

Comprehensive coverage including:
- Programming languages (js, ts, py, go, rust, etc.)
- Frameworks (react, vue, angular, svelte, etc.)
- Config files (tsconfig, eslint, prettier, etc.)
- Document types (md, pdf, doc, etc.)
- Image/media types
- CI/CD tools (github, gitlab, jenkins, etc.)
- And many more...
