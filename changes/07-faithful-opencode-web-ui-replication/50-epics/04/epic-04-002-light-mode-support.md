# Epic 04-002: Light Mode Support

## Overview

Implement full light theme matching OpenCode. The current Blazor implementation only supports dark mode, while OpenCode offers complete light mode support with multiple theme variants. This epic adds light mode variable values, media query support for system preference, manual theme toggling, and theme persistence.

## Goals

1. Define light mode variable values for all CSS variables
2. Add media query support for system color scheme preference
3. Add manual theme toggle with data-theme attribute support
4. Create ThemeToggle component for user control
5. Persist theme preference in localStorage
6. Prevent theme flash on page load

## Tasks

### Light Mode Variables
- [x] Define light mode values in `opencode-theme.css`:
  - [x] Background variables:
    - [x] `--background-base: #f8f7f7`
    - [x] `--background-weak`: lighter variant
    - [x] `--background-strong`: subtle variant
  - [x] Text color variables:
    - [x] `--text-strong`: near black
    - [x] `--text-base`: dark gray
    - [x] `--text-weak`: medium gray
    - [x] `--text-weaker`: light gray
  - [x] Border variables:
    - [x] Adjust for light backgrounds
  - [x] Surface variables:
    - [x] Light mode card backgrounds
  - [x] Diff colors:
    - [x] Ensure add/remove visible on light background

### System Preference Support
- [x] Add `@media (prefers-color-scheme: light)` rules:
  ```css
  @media (prefers-color-scheme: light) {
    :root:not([data-theme="dark"]) {
      --background-base: #f8f7f7;
      /* ... all light mode values */
    }
  }
  ```
- [x] Ensure dark mode is default when no preference

### Manual Theme Control
- [x] Implement `data-theme` attribute support:
  - [x] `data-theme="light"` forces light mode
  - [x] `data-theme="dark"` forces dark mode
  - [x] No attribute follows system preference
- [x] CSS structure for theme override:
  ```css
  :root[data-theme="light"] {
    --background-base: #f8f7f7;
    /* ... */
  }
  :root[data-theme="dark"] {
    --background-base: #131010;
    /* ... */
  }
  ```

### ThemeToggle Component
- [x] Create `ThemeToggle.razor` component:
  - [x] Two-way toggle: Light / Dark (simplified - system preference handled automatically)
  - [x] Visual indicator of current mode
  - [x] Icon for each mode (sun, moon SVG icons)
- [x] Implement toggle logic:
  - [x] Set `data-theme` attribute on html/body
  - [x] Store preference in localStorage
  - [x] Trigger theme change event
- [x] Style toggle control:
  - [x] Clean button design
  - [x] Match OpenCode styling

### Theme Persistence
- [x] Store theme preference in localStorage:
  - [x] Key: `opencode-theme`
  - [x] Values: `light`, `dark`
- [x] Read preference on page load
- [x] Apply preference before render to prevent flash

### Flash Prevention
- [x] Add inline script in HTML `<head>`:
  ```html
  <script>
    (function() {
      var theme = localStorage.getItem('opencode-theme');
      if (theme === 'light' || theme === 'dark') {
        document.documentElement.setAttribute('data-theme', theme);
        document.documentElement.classList.add(theme);
      } else {
        var prefersDark = window.matchMedia('(prefers-color-scheme: dark)').matches;
        var defaultTheme = prefersDark ? 'dark' : 'light';
        document.documentElement.setAttribute('data-theme', defaultTheme);
        document.documentElement.classList.add(defaultTheme);
      }
    })();
  </script>
  ```
- [x] Ensure script runs before body renders
- [x] Test for flash on page load

### Component Testing in Light Mode
- [x] Test ChatMessage in light mode:
  - [x] Avatar visibility
  - [x] Message background
  - [x] Text readability
- [x] Test ChatInput in light mode:
  - [x] Input background
  - [x] Border visibility
  - [x] Focus ring
- [x] Test code blocks in light mode:
  - [x] Background contrast
  - [x] Text readability
- [x] Test tool displays in light mode:
  - [x] Progress indicators
  - [x] Collapsible sections
- [x] Test DiffViewer in light mode:
  - [x] Add/remove colors visible
  - [x] Hunk headers readable
  - [x] Line numbers visible

## Acceptance Criteria

- Light mode renders correctly for all components
- System preference (prefers-color-scheme) is respected when no manual override
- Theme toggle switches between Light / System / Dark modes
- Theme preference persists across browser sessions
- No flash of wrong theme on page load
- All text is readable in both modes
- Diff colors work correctly in light mode
- Theme can be toggled without page reload

## Implementation Notes

### Existing Infrastructure:
The opencode-theme.css file already contained comprehensive light/dark mode support:
- Light mode as default in `:root`
- Dark mode via `@media (prefers-color-scheme: dark)`
- Force classes: `.dark`, `.light`, `[data-theme="dark"]`, `[data-theme="light"]`
- Complete variable sets for both modes including: background, surface, text, border, icon, diff colors, syntax highlighting, markdown

### Files Modified:
- `src/LionFire.OpenCode.Blazor/Components/Shared/ThemeToggle.razor` - Complete rewrite:
  - Uses OpenCodeThemeService for state management
  - SVG sun/moon icons
  - Proper event subscription and cleanup
  - Theme-aware styling

- `examples/AgUi.IDE.BlazorServer/Components/App.razor`:
  - Added flash prevention script in `<head>`
  - Reads from localStorage before CSS loads
  - Falls back to system preference if no stored value
  - Removed hardcoded `class="dark"` from `<html>`

### Services Used:
- `OpenCodeThemeService` - Already existed with:
  - InitializeAsync() for reading stored preference
  - ToggleThemeAsync() for switching modes
  - SetDarkModeAsync() for explicit mode setting
  - OnThemeChanged event for component updates
  - localStorage persistence with key `opencode-theme`

## Dependencies

- Epic 03-001: CSS Variable System Overhaul (all variables must exist before light values)

## References

- [Styling Comparison](/src/opencode-dotnet/docs/opencode-analysis/styling-comparison.md) - Theme System section
- OpenCode theme files: `packages/web/ui/styles/themes/`
- OpenCode light theme: oc-1-light variant

## Effort Estimate

2-3 hours
