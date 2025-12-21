# Epic 01-003: Extract and Document Styling System

**Change**: 07 - Faithful OpenCode Web UI Replication
**Phase**: 01 - Deep Analysis
**Status**: planned
**Effort**: 2 days
**Dependencies**: Epic 01-001 (Directory structure must be known)

## Status Overview

- [ ] Planning Complete
- [ ] Development Started
- [ ] Core Features Complete
- [ ] Testing Complete
- [ ] Documentation Complete

## Overview

Extract and document the complete styling system used in OpenCode web UI, including CSS variables, theme definitions, component-specific styles, layout patterns, and responsive design. This epic ensures we can faithfully replicate the visual appearance in Blazor.

## User Stories & Implementation Tasks

### Must Have

#### Story 001: CSS Variables and Theme System
**Effort**: 4 hours
**Status**: pending

Extract all CSS custom properties (variables) and theme definitions.

##### Tasks
- [ ] 0001: Locate global CSS files in `/dv/opencode/packages/ui/`
- [ ] 0002: Extract all CSS custom properties (--variable-name format)
- [ ] 0003: Categorize variables by purpose (colors, spacing, typography, etc.)
- [ ] 0004: Identify dark/light theme variable differences
- [ ] 0005: Document color palette with hex values
- [ ] 0006: Extract typography variables (font-family, sizes, weights, line-heights)
- [ ] 0007: Document spacing/sizing scale
- [ ] 0008: Create comprehensive theme variables document
- [ ] 0009: Save to `/src/opencode-dotnet/docs/opencode-analysis/theme-variables.md`

#### Story 002: Layout and Spacing Patterns
**Effort**: 4 hours
**Status**: pending

Document the layout system, grid patterns, and spacing conventions.

##### Tasks
- [ ] 0001: Identify CSS Grid or Flexbox layout patterns
- [ ] 0002: Document main application layout structure (header height, sidebar width, etc.)
- [ ] 0003: Extract padding and margin patterns
- [ ] 0004: Document responsive breakpoints (if any)
- [ ] 0005: Identify container max-widths and constraints
- [ ] 0006: Map panel sizing and resizing behavior
- [ ] 0007: Create layout dimensions reference sheet
- [ ] 0008: Add to `/src/opencode-dotnet/docs/opencode-analysis/layout-system.md`

#### Story 003: Component Styling Patterns
**Effort**: 6 hours
**Status**: pending

Extract styling patterns for key UI components.

##### Tasks
- [ ] 0001: Document button styles (primary, secondary, icon buttons, etc.)
- [ ] 0002: Extract input/textarea styling
- [ ] 0003: Document message bubble/card styling
- [ ] 0004: Extract code block styling and syntax highlighting
- [ ] 0005: Document modal/dialog styling
- [ ] 0006: Extract dropdown/select styling
- [ ] 0007: Document scrollbar customization
- [ ] 0008: Extract loading spinner/animation styles
- [ ] 0009: Document tooltip styling
- [ ] 0010: Create component styling reference
- [ ] 0011: Save to `/src/opencode-dotnet/docs/opencode-analysis/component-styles.md`

#### Story 004: Animation and Transition Patterns
**Effort**: 3 hours
**Status**: pending

Document all CSS animations, transitions, and motion design.

##### Tasks
- [ ] 0001: Extract CSS transition properties
- [ ] 0002: Identify CSS animations and keyframes
- [ ] 0003: Document hover state transitions
- [ ] 0004: Extract loading/shimmer animations
- [ ] 0005: Document modal open/close animations
- [ ] 0006: Identify easing functions used
- [ ] 0007: Note animation durations
- [ ] 0008: Add to `/src/opencode-dotnet/docs/opencode-analysis/animations.md`

### Should Have

#### Story 005: Typography System
**Effort**: 2 hours
**Status**: pending

Document the complete typography system.

##### Tasks
- [ ] 0001: Extract font families and fallback stacks
- [ ] 0002: Document heading styles (h1-h6)
- [ ] 0003: Extract body text styles
- [ ] 0004: Document code/monospace font usage
- [ ] 0005: Identify font loading strategy
- [ ] 0006: Add to `/src/opencode-dotnet/docs/opencode-analysis/typography.md`

#### Story 006: Icon System
**Effort**: 2 hours
**Status**: pending

Document icon usage and implementation.

##### Tasks
- [ ] 0001: Identify icon library used (if any - e.g., Heroicons, Font Awesome)
- [ ] 0002: Locate SVG icon files or icon font
- [ ] 0003: Document icon sizing conventions
- [ ] 0004: Extract icon colors and customization patterns
- [ ] 0005: List all unique icons used in the UI
- [ ] 0006: Add to `/src/opencode-dotnet/docs/opencode-analysis/icon-system.md`

### Nice to Have

#### Story 007: CSS Architecture Analysis
**Effort**: 2 hours
**Status**: pending

Understand the CSS organization and methodology.

##### Tasks
- [ ] 0001: Identify CSS methodology (BEM, CSS Modules, Tailwind, CSS-in-JS, etc.)
- [ ] 0002: Document CSS file organization
- [ ] 0003: Note any CSS preprocessor usage (SCSS, PostCSS, etc.)
- [ ] 0004: Identify utility class patterns
- [ ] 0005: Add to `/src/opencode-dotnet/docs/opencode-analysis/css-architecture.md`

## Technical Requirements Checklist

- [ ] All CSS variables documented with actual values
- [ ] Color palette includes hex codes and usage context
- [ ] Typography scale is complete with sizes and weights
- [ ] Layout dimensions are measured and documented
- [ ] Component styles include code snippets where helpful
- [ ] Animation timings and easing functions are noted
- [ ] Documentation is organized for easy reference during Blazor implementation

## Dependencies & Blockers

- **Dependency**: Epic 01-001 should be complete to know where CSS files are located
- Requires access to `/dv/opencode/packages/ui/` for reading CSS/style files

## Acceptance Criteria

- [ ] Complete CSS variables list with categorization (colors, spacing, typography)
- [ ] Color palette documented with at least 20+ color variables
- [ ] Layout system documented with specific measurements
- [ ] Component styles extracted for all major UI elements (buttons, inputs, messages, etc.)
- [ ] Animation/transition patterns documented with durations and easing
- [ ] Typography system fully documented
- [ ] Documentation provides everything needed to replicate styling in Blazor CSS

## Notes

**Critical for Faithful Replication**:
- Exact color values (must match pixel-perfect)
- Spacing values (padding, margin, gaps)
- Border radius values
- Shadow definitions
- Animation timings

**Documentation Target**: `/src/opencode-dotnet/docs/opencode-analysis/`

**Blazor Implementation Note**: We'll likely use CSS files in Blazor that directly mirror these values, so having exact measurements is crucial.
