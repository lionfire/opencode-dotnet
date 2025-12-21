# Epic 02-002: Styling and Theme Comparison

## Overview

Compare the styling system of the current Blazor implementation against OpenCode's styling to identify inconsistencies and gaps.

## Goals

1. Document current Blazor styling approach
2. Compare CSS variable systems
3. Identify color and typography differences
4. Document spacing and layout differences
5. Compare dark/light mode implementation

## Deliverables

### 1. Blazor Styling Inventory

Create `/src/opencode-dotnet/docs/opencode-analysis/blazor-styling.md`:
- Current CSS approach (MudBlazor, custom, etc.)
- CSS variables defined
- Theme configuration
- Component-specific styles

### 2. CSS Variable Comparison

Compare OpenCode's CSS variables to Blazor implementation:
- Background colors
- Text colors
- Border colors
- Syntax highlighting colors
- Spacing values

### 3. Typography Comparison

- Font families in use
- Font sizes
- Font weights
- Line heights

### 4. Component Styling Differences

For each component:
- Visual differences (screenshots if helpful)
- CSS property differences
- Missing styles

### 5. Theme System Gap Analysis

- Dark mode implementation differences
- Theme switching mechanism
- CSS variable organization

## Tasks

- [x] Inventory Blazor CSS and styling approach
- [x] Create CSS variable comparison table
- [x] Document typography differences
- [x] Document component styling differences
- [x] Analyze theme system gaps
- [x] Create migration recommendations

## Acceptance Criteria

- Complete CSS variable mapping
- Typography comparison complete
- Component-by-component styling differences documented
- Actionable recommendations for styling fixes

## Dependencies

- Phase 01 analysis complete (styling-system.md)
- Epic 02-001 component mapping for reference

## References

- `/src/opencode-dotnet/docs/opencode-analysis/styling-system.md`
- `/dv/opencode/packages/ui/src/components/` (CSS files)
- `/dv/opencode/packages/ui/src/app.css`
