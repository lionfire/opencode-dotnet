# Epic 03-004: Missing Display Components

## Overview

Implement critical missing UI components identified in the gap analysis. The OpenCode web UI includes several display components that significantly enhance the user experience but are not present in the current Blazor implementation: Typewriter (animated text), FileIcon (file-type icons), ProviderIcon (AI provider logos), and Logo (OpenCode branding).

## Goals

1. Implement Typewriter component for animated text display
2. Implement FileIcon component with file extension mapping
3. Implement ProviderIcon component for AI provider logos
4. Implement Logo component for OpenCode branding

## Tasks

### Typewriter Component
- [x] Create `Typewriter.razor` component:
  - [x] Character-by-character text animation
  - [x] Configurable typing speed (default ~30ms per character)
  - [x] Support for starting/stopping animation
  - [x] Callback for animation completion
- [x] Implement animation logic:
  - [x] Use `Task.Delay` for timing
  - [x] Handle text updates mid-animation (cancellation)
  - [x] Support cursor blink effect
- [x] Add CSS styling:
  - [x] Cursor styling (blinking line)
  - [x] Smooth text appearance
- [x] Respect `prefers-reduced-motion` (instant display option)
- [ ] Integrate with assistant message summaries (optional - depends on UI needs)

### FileIcon Component
- [x] Enhanced `FileIcon.razor` component:
  - [x] Input: filename or extension
  - [x] Output: appropriate SVG icon
  - [x] Size parameter support (Tiny, Small, Medium, Large)
- [x] Create file extension to icon mapping (20+ file types):
  - [x] TypeScript (.ts, .tsx) - TypeScript icon
  - [x] JavaScript (.js, .jsx, .mjs, .cjs) - JavaScript icon
  - [x] C# (.cs) - C# icon
  - [x] Razor (.razor, .cshtml) - Razor icon
  - [x] Markdown (.md, .mdx) - Markdown icon
  - [x] JSON (.json, .jsonc) - JSON icon
  - [x] YAML (.yml, .yaml) - YAML icon
  - [x] Python (.py) - Python icon
  - [x] Rust (.rs) - Rust icon
  - [x] Go (.go) - Go icon
  - [x] Java (.java) - Java icon
  - [x] HTML (.html, .htm) - HTML icon
  - [x] CSS (.css, .scss, .sass, .less) - CSS icon
  - [x] Shell (.sh, .bash, .zsh, .ps1) - Shell icon
  - [x] Config files (.config, .env, .ini, .toml) - Config icon
  - [x] XML (.xml, .xaml, .csproj, .sln) - XML icon
  - [x] Image files (.png, .jpg, .svg, .webp, .ico) - Image icon
  - [x] SQL (.sql) - SQL icon
  - [x] Docker (Dockerfile) - Docker icon
  - [x] Git (.gitignore, .gitattributes) - Git icon
- [x] Add folder icon support
- [x] Add fallback icon for unknown types
- [x] Use custom SVGs (not MudBlazor icons)

### ProviderIcon Component
- [x] Enhanced `ProviderIcon.razor` component:
  - [x] Input: provider name (anthropic, openai, etc.)
  - [x] Output: provider logo SVG
  - [x] Size parameter support (Tiny, Small, Medium, Large)
  - [x] Optional label display
- [x] Add provider logo SVGs:
  - [x] Anthropic logo
  - [x] OpenAI logo
  - [x] Google/Gemini logo
  - [x] DeepSeek logo
  - [x] Mistral logo
  - [x] Cohere logo
  - [x] Together AI logo
  - [x] Local/Ollama icon
  - [x] Azure logo
  - [x] AWS/Bedrock logo
- [x] Add fallback generic AI icon

### Logo Component
- [x] Create `OpenCodeLogo.razor` component:
  - [x] OpenCode branding SVG (stylized code brackets)
  - [x] Support size parameter (Tiny to XLarge)
  - [x] Support light/dark mode variants via CSS variables
  - [x] Icon-only and Full variants
  - [x] Optional text display
- [x] Add CSS variables for logo colors in theme

## Acceptance Criteria

- [x] Typewriter component animates text at configurable speed
- [x] Typewriter respects `prefers-reduced-motion` user preference
- [x] FileIcon shows correct icon for 10+ common file types (20+ implemented)
- [x] FileIcon has appropriate fallback for unknown types
- [x] ProviderIcon displays for at least Anthropic and OpenAI (10 providers)
- [x] ProviderIcon has fallback for unknown providers
- [x] Logo component renders OpenCode branding
- [x] All components support size/scale parameters

## Implementation Notes

### Files Created/Modified:
- `Components/Shared/Typewriter.razor` - New animated text component
- `Components/Files/FileIcon.razor` - Enhanced with SVG icons
- `Components/Files/FileIconSize.cs` - Size enum
- `Components/Shared/ProviderIcon.razor` - Enhanced with SVG logos
- `Components/Shared/ProviderIconSize.cs` - Size enum
- `Components/Shared/OpenCodeLogo.razor` - New logo component
- `Components/Shared/LogoEnums.cs` - Size and variant enums
- `wwwroot/css/opencode-theme.css` - Added logo and folder CSS variables

## Dependencies

- Epic 03-001: CSS Variable System Overhaul (components use theme variables)
- Epic 03-002: Font Integration (Typewriter uses monospace font for cursor)

## References

- [Component Mapping](/src/opencode-dotnet/docs/opencode-analysis/component-mapping.md) - Missing Components section
- OpenCode Typewriter: search in `packages/web/ui/`
- OpenCode FileIcon: `packages/web/ui/file-icon.tsx`
- OpenCode ProviderIcon: `packages/web/ui/provider-icon.tsx`

## Effort Estimate

3-4 hours
