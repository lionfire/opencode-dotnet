# Epic 02-001: Component-by-Component Comparison

## Overview

Compare each component in the current Blazor implementation against the OpenCode web UI to identify gaps and differences.

## Goals

1. Document existing Blazor components and their state
2. Map Blazor components to OpenCode equivalents
3. Identify missing components
4. Document structural differences
5. Document functional differences

## Deliverables

### 1. Blazor Component Inventory

Create `/src/opencode-dotnet/docs/opencode-analysis/blazor-components.md`:
- List all current Blazor components in AgUi.IDE.BlazorServer
- Document their purpose and functionality
- Note any hardcoded/mock data usage

### 2. Component Mapping Matrix

Create `/src/opencode-dotnet/docs/opencode-analysis/component-mapping.md`:
- Side-by-side comparison table
- Blazor component → OpenCode equivalent
- Status (complete, partial, missing)
- Notes on differences

### 3. Missing Components List

Document components that exist in OpenCode but not in Blazor:
- Priority level (critical, important, nice-to-have)
- Complexity estimate
- Dependencies

### 4. Structural Differences

For each mapped component, document:
- HTML structure differences
- Child component differences
- Props/parameters differences
- Event handling differences

## Tasks

- [x] Inventory AgUi.IDE.BlazorServer components
- [x] Map each Blazor component to OpenCode equivalent
- [x] Identify missing components
- [x] Document structural differences for each component
- [x] Create priority recommendations

## Acceptance Criteria

- Complete inventory of Blazor components
- Mapping matrix covering all OpenCode components from Phase 01 analysis
- Clear list of missing components with priority
- Actionable recommendations for Phase 03

## Dependencies

- Phase 01 analysis complete (component-inventory.md, component-hierarchy.md)

## References

- `/src/opencode-dotnet/docs/opencode-analysis/component-inventory.md`
- `/src/opencode-dotnet/docs/opencode-analysis/component-hierarchy.md`
- `/src/opencode-dotnet/examples/AgUi.IDE.BlazorServer/`
