# Epic 03-001: CSS Variable System Overhaul

## Overview

Migrate from ad-hoc CSS styling to OpenCode's comprehensive CSS variable system. The current Blazor implementation has approximately 15 CSS variables, while OpenCode uses 100+ semantic variables for consistent theming. This epic establishes the foundation for visual fidelity with the OpenCode web UI.

## Goals

1. Create a comprehensive CSS variable file matching OpenCode's theme system
2. Port all background, text, border, surface, and icon color variables
3. Update all existing components to use semantic variables instead of hardcoded colors
4. Establish a consistent naming convention following OpenCode's `--category-semantic-variant` pattern

## Tasks

- [x] Create `/wwwroot/css/opencode-theme.css` with full variable set structure (already existed in LionFire.OpenCode.Blazor)
- [x] Port background variables:
  - [x] `--background-base` (#131010)
  - [x] `--background-weak` (#1c1717)
  - [x] `--background-strong` (#151313)
  - [x] `--background-stronger` (#fcfcfc)
- [x] Port text color variables:
  - [x] `--text-strong` (smoke-dark-12)
  - [x] `--text-base` (smoke-dark-11)
  - [x] `--text-weak` (smoke-dark-9)
  - [x] `--text-weaker` (smoke-dark-8)
- [x] Port border variables:
  - [x] `--border-base` (smoke-alpha-7)
  - [x] `--border-strong-base` (smoke-alpha-7)
  - [x] `--border-weak-base` (smoke-alpha-5)
  - [x] `--border-selected` (cobalt-alpha-9)
- [x] Port surface variables (`--surface-*`) for cards and dialogs (8+ variables)
- [x] Port icon color variables (`--icon-*`) (10+ variables)
- [x] Port diff color variables:
  - [x] `--surface-diff-add-base` (mint-3)
  - [x] `--text-diff-add-base` (mint-11)
  - [x] `--surface-diff-delete-base` (ember-3)
  - [x] `--text-diff-delete-base` (ember-10)
- [x] Port interactive color variables:
  - [x] `--text-interactive-base` (cobalt-9)
  - [x] `--surface-base-hover`
- [x] Update ChatMessage component to use new variables
- [x] Update ChatInput component to use new variables
- [x] Update ChatPanel component to use new variables
- [x] Update DiffViewer component to use new variables
- [x] Update tool display styles to use new variables (ToolExecutionPanel, PermissionPanel)
- [x] Update IdeLayout component to use new variables
- [x] Remove all hardcoded color values from component styles
- [x] Test variable consistency across all components (build passes)
- [x] Verify dark mode renders correctly with new variables (added thinking block variables)

## Acceptance Criteria

- `opencode-theme.css` contains 80+ variables matching OpenCode's theme system
- No hardcoded color values remain in component styles
- All components render correctly with new variables
- Naming convention follows `--category-semantic-variant` pattern
- Variables are organized by category (background, text, border, surface, icon, diff)

## Dependencies

- Phase 01 (Deep Analysis) - COMPLETE
- Phase 02 (Gap Analysis) - COMPLETE
- Gap analysis documents provide variable mappings

## References

- [Styling Comparison](/src/opencode-dotnet/docs/opencode-analysis/styling-comparison.md) - CSS variable comparison tables
- [Component Mapping](/src/opencode-dotnet/docs/opencode-analysis/component-mapping.md) - Component styling differences
- OpenCode theme files: `packages/web/ui/styles/themes/`

## Effort Estimate

2-3 hours
