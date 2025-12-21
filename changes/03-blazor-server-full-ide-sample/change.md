# Change 03: Blazor Server Full IDE Sample

**Type**: feature
**Created**: 2025-12-12
**Status**: Active
**Related Spec**: None

## Description

Create AgUi.IDE.BlazorServer with full OpenCode UI replication - file tree, chat, diffs, terminal

## Scope

- [ ] Create AgUi.IDE.BlazorServer project
- [ ] Implement file tree component
- [ ] Implement chat interface component
- [ ] Implement diff viewer component
- [ ] Implement terminal/output component
- [ ] Create main IDE layout container
- [ ] Integrate all components with OpenCode backend
- [ ] Test multi-pane layout and interactions

## Success Criteria

- [ ] AgUi.IDE.BlazorServer builds without errors
- [ ] Full IDE interface displays all four main panes
- [ ] File tree navigation works properly
- [ ] Chat messages stream correctly
- [ ] Diffs display with syntax highlighting
- [ ] Terminal output renders properly
- [ ] All styling matches OpenCode design

## Notes

References:
- Original IDE UI: `/dv/opencode/packages/desktop/` (SolidJS)
- Layout inspiration: OpenCode web IDE
- Component library: MudBlazor

---

## Execution Summary

**Executed**: 2025-12-12
**Phases**: 4
**Epics**: 10
**Tasks**: ~65

### Phases

| Phase | Name | Epics |
|-------|------|-------|
| 01 | Project Setup | 03-001, 03-002 |
| 02 | Core Components | 03-003, 03-004, 03-005, 03-006 |
| 03 | Layout & Integration | 03-007, 03-008 |
| 04 | Polish | 03-009, 03-010 |

### Generated Artifacts

- `40-phases/phases.md` - Phase planning
- `50-epics/01/epic-03-001-create-ide-blazorserver-project.md`
- `50-epics/01/epic-03-002-configure-layout-system.md`
- `50-epics/02/epic-03-003-file-tree-component.md`
- `50-epics/02/epic-03-004-chat-interface-component.md`
- `50-epics/02/epic-03-005-diff-viewer-component.md`
- `50-epics/02/epic-03-006-terminal-component.md`
- `50-epics/03/epic-03-007-ide-main-layout.md`
- `50-epics/03/epic-03-008-opencode-backend-integration.md`
- `50-epics/04/epic-03-009-styling-polish.md`
- `50-epics/04/epic-03-010-testing-documentation.md`
- `65-status/` - Status tracking

### Greenlit (Ready to Implement)

- 03-001 - Create IDE BlazorServer Project
- 03-002 - Configure Layout System

### Next Steps

1. Review epics in `50-epics/`
2. Start implementation: `/ax:change:implement 03`
