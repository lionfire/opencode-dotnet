# Epic 03-003: File Tree Component

**Phase**: 02 - Core Components
**Status**: Complete
**Priority**: High

## Overview

Build the file tree navigation component for browsing project files and folders.

## Stories / Tasks

### Data Model

- [ ] **Task 1**: Define FileTreeNode model
  - Name, Path, IsDirectory properties
  - Children collection for directories
  - Icon property based on file type
  - IsExpanded state

- [ ] **Task 2**: Create file tree service interface
  - GetRootNodes() - list root directory contents
  - GetChildren(path) - lazy-load directory contents
  - FileSelected event
  - DirectoryExpanded event

### Component Implementation

- [ ] **Task 3**: Create FileTree.razor component
  - Recursive tree rendering
  - Expand/collapse functionality
  - File/folder icons (MudBlazor icons)
  - Selection highlighting

- [ ] **Task 4**: Create FileTreeNode.razor component
  - Single node rendering
  - Click handling (select/expand)
  - Context menu support (optional)
  - Drag-drop support (optional)

- [ ] **Task 5**: Implement lazy loading
  - Load children on expand
  - Show loading indicator
  - Handle errors gracefully

### Styling

- [ ] **Task 6**: Style file tree to match OpenCode
  - Proper indentation
  - Icon colors by file type
  - Selection state styling
  - Hover effects

### Integration

- [ ] **Task 7**: Connect to OpenCode backend
  - Use OpenCode path API
  - Map backend response to FileTreeNode
  - Handle path navigation

## Acceptance Criteria

- [ ] File tree displays project structure
- [ ] Directories expand/collapse on click
- [ ] Files can be selected
- [ ] Selection triggers appropriate events
- [ ] Lazy loading works for large directories
- [ ] Styling matches OpenCode design

## Dependencies

- Epic 03-001, 03-002 must be complete

## Notes

OpenCode file tree reference:
- `/dv/opencode/packages/ui/src/components/filetree/`

MudBlazor has MudTreeView component that can be customized.
