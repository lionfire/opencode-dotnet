# Change 07: Faithful OpenCode Web UI Replication

**Type**: feature
**Created**: 2025-12-12
**Status**: Active
**Related Spec**: None

## Description

We have some of the main components from OpenCode web UI implemented in our Blazor Server IDE sample, but there are inconsistencies and sample data. This change aims to faithfully replicate the OpenCode web UI layout and flow with our own branding.

## Goals

1. **Thorough Analysis** - Deeply analyze the OpenCode web UI structure, components, styling, and interaction patterns
2. **Component Mapping** - Map every OpenCode React component to our Blazor equivalents
3. **Faithful Replication** - Replicate the UI exactly (with branding changes only)
4. **Remove Sample Data** - Replace all mock/sample data with real backend integration

## Scope

### In Scope
- Chat panel layout and message rendering
- Tool call visualization
- Permission request dialogs
- Model/provider selector
- Agent selector
- File explorer (if applicable)
- Diff viewer
- Terminal panel
- Theme/styling consistency
- Keyboard shortcuts
- Loading states and animations

### Out of Scope
- Backend API changes (use existing OpenCode SDK)
- New features not in OpenCode web
- Mobile-specific layouts

## Success Criteria

- [ ] Every visible component matches OpenCode web layout pixel-close
- [ ] All interaction patterns work identically
- [ ] No mock/sample data in production builds
- [ ] Theme variables match OpenCode exactly
- [ ] Smooth animations and transitions

## Analysis Approach

### Phase 1: Deep Dive Analysis
1. Map OpenCode web directory structure
2. Identify all React components and their purposes
3. Document component hierarchy and data flow
4. Extract CSS/styling patterns
5. Identify state management patterns

### Phase 2: Gap Analysis
1. Compare current Blazor components to OpenCode equivalents
2. Document missing components
3. Document styling inconsistencies
4. Identify data flow differences

### Phase 3: Implementation
1. Fix styling inconsistencies
2. Implement missing components
3. Wire up real data
4. Polish animations/transitions

## Notes

Source reference: `/dv/opencode/packages/ui/` (web UI components)
Target project: `/src/opencode-dotnet/examples/AgUi.IDE.BlazorServer/`

## Execution Summary

**Executed**: 2025-12-13
**Phases**: 4
**Epics**: 4 (Phase 1)
**Tasks**: 79 (across all Phase 1 epics)

### Generated Artifacts

- `40-phases/phases.md` - Phase planning overview with 4 phases
- `50-epics/01/epic-01-001-map-opencode-directory-structure.md` - Directory and component inventory
- `50-epics/01/epic-01-002-component-hierarchy-data-flow.md` - Component relationships and state management
- `50-epics/01/epic-01-003-styling-system-extraction.md` - Complete styling and theme documentation
- `50-epics/01/epic-01-004-interaction-patterns-keyboard.md` - UX behaviors and keyboard shortcuts
- `65-status/greenlit.md` - All Phase 1 epics greenlit for implementation
- `65-status/status.hjson` - Status tracking

### Phase 1 Epics (Greenlit)

All 4 Phase 1 epics have been created and greenlit:

1. **Epic 01-001** (1 day): Map OpenCode web directory structure and create component inventory
2. **Epic 01-002** (2 days): Document component hierarchy, relationships, and data flow patterns
3. **Epic 01-003** (2 days): Extract complete styling system including CSS variables, theme, animations
4. **Epic 01-004** (1.5 days): Document all interaction patterns, keyboard shortcuts, and UX behaviors

**Estimated Phase 1 Effort**: 6.5 days

### Documentation Output

All analysis documentation will be created in:
`/src/opencode-dotnet/docs/opencode-analysis/`

This will include:
- Directory structure and component inventory
- Component hierarchy diagrams and data flow
- Complete theme variables and styling reference
- Keyboard shortcuts and interaction patterns
- And more detailed technical documentation

### Next Steps

1. Begin implementing Phase 1 epics (deep analysis of OpenCode web UI)
2. Use `/ax:change:implement 07` to start working on greenlit epics
3. After Phase 1 completes, Phase 2 epics will be created for gap analysis
4. Phases 3 and 4 will follow with implementation and polish
