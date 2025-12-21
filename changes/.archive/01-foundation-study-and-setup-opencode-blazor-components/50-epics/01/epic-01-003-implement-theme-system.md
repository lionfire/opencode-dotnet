# Epic 01-003: Implement Theme System

**Change**: 01 - Foundation - Study and Setup OpenCode Blazor Components
**Phase**: 01 - Study & Setup
**Status**: completed
**Effort**: 4h
**Dependencies**: 01-001 (Study OpenCode Web UI)

## Status Overview
- [x] Planning Complete
- [x] Development Started
- [x] Core Features Complete
- [x] Testing Complete
- [x] Documentation Complete

## Overview

Create a CSS theme system that matches OpenCode's visual design. This includes extracting CSS variables from the original UI and implementing them in a Blazor-compatible way that integrates with MudBlazor's theming.

## User Stories & Implementation Tasks

### Must Have

#### Story 001: Extract OpenCode Theme Variables

**Effort**: 1h
**Status**: completed

##### Tasks
- [x] 0001: Inspect computed styles in running OpenCode web UI
- [x] 0002: Extract all CSS custom properties (--variable-name)
- [x] 0003: Document color palette (backgrounds, text, borders, accents)
- [x] 0004: Document typography (fonts, sizes, weights, line-heights)
- [x] 0005: Document spacing scale (margins, paddings, gaps)
- [x] 0006: Document shadows, borders, border-radius values

#### Story 002: Create CSS Variables File

**Effort**: 1h
**Status**: completed

##### Tasks
- [x] 0001: Create `/src/opencode-dotnet/src/LionFire.OpenCode.Blazor/wwwroot/css/opencode-theme.css`
- [x] 0002: Define all color variables matching OpenCode (in opencode-colors.css)
- [x] 0003: Define typography variables
- [x] 0004: Define spacing variables
- [x] 0005: Define component-specific variables (borders, shadows)
- [x] 0006: Add CSS comments documenting each variable group

#### Story 003: Configure MudBlazor Theme

**Effort**: 1.5h
**Status**: completed

##### Tasks
- [x] 0001: Create `OpenCodeTheme.cs` with MudTheme configuration
- [x] 0002: Map OpenCode colors to MudBlazor palette
- [x] 0003: Configure typography to match OpenCode
- [x] 0004: Set up dark mode as default
- [x] 0005: Test theme with sample MudBlazor components (verified no compile errors)
- [x] 0006: Document how to apply theme in Blazor apps (in XML doc comments)

#### Story 004: Create Base Component Styles

**Effort**: 1h
**Status**: completed

##### Tasks
- [x] 0001: Create `opencode-base.css` with component baseline styles
- [x] 0002: Style scrollbars to match OpenCode
- [x] 0003: Style code blocks and pre elements
- [x] 0004: Style buttons and interactive elements
- [x] 0005: Add utility classes for common patterns (in opencode-utilities.css)

### Should Have

#### Story 005: Add Dark/Light Mode Toggle Support

**Effort**: 0.5h
**Status**: completed

##### Tasks
- [x] 0001: Define light mode variable overrides (in opencode-theme.css)
- [x] 0002: Create theme toggle service (OpenCodeThemeService.cs)
- [x] 0003: Persist theme preference in localStorage
- [x] 0004: Ensure all components respect theme changes

## Technical Requirements Checklist

- [x] CSS custom properties browser support (all modern browsers)
- [x] MudBlazor theming documentation reviewed
- [x] Understanding of CSS specificity for overrides
- [x] Color contrast accessibility (WCAG AA minimum)

## Dependencies & Blockers

- Depends on Epic 01-001 for visual reference ✓
- MudBlazor NuGet package must be referenced ✓

## Acceptance Criteria

- [x] theme.css created with all OpenCode colors/typography
- [x] MudBlazor theme configured and applied
- [x] Dark mode is default and looks like OpenCode
- [x] Sample MudBlazor components match OpenCode style
- [x] CSS loads without errors in Blazor apps

## Deliverables

- `/src/opencode-dotnet/src/LionFire.OpenCode.Blazor/wwwroot/css/opencode-colors.css` - 650+ color palette variables
- `/src/opencode-dotnet/src/LionFire.OpenCode.Blazor/wwwroot/css/opencode-theme.css` - Semantic theme variables with dark/light modes
- `/src/opencode-dotnet/src/LionFire.OpenCode.Blazor/wwwroot/css/opencode-base.css` - Global resets and base styles
- `/src/opencode-dotnet/src/LionFire.OpenCode.Blazor/wwwroot/css/opencode-utilities.css` - Tailwind-inspired utility classes
- `/src/opencode-dotnet/src/LionFire.OpenCode.Blazor/Theming/OpenCodeTheme.cs` - MudBlazor theme configuration
- `/src/opencode-dotnet/src/LionFire.OpenCode.Blazor/Theming/OpenCodeThemeService.cs` - Theme state management service
- `/src/opencode-dotnet/src/LionFire.OpenCode.Blazor/Theming/OpenCodeThemeExtensions.cs` - DI registration extensions
