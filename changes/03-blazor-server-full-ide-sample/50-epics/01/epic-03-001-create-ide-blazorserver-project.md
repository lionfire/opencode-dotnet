# Epic 03-001: Create IDE BlazorServer Project

**Phase**: 01 - Project Setup
**Status**: Complete
**Priority**: High

## Overview

Create the AgUi.IDE.BlazorServer project with proper structure, dependencies, and initial configuration.

## Stories / Tasks

### Setup & Configuration

- [x] **Task 1**: Create new Blazor Server project `AgUi.IDE.BlazorServer`
  - Use `dotnet new blazor --interactivity Server`
  - Target net8.0 and net9.0
  - Add to solution file

- [x] **Task 2**: Add project dependencies
  - MudBlazor for UI components
  - LionFire.OpenCode.Blazor for theme/utilities
  - LionFire.OpenCode.Serve for backend client
  - Optional: syntax highlighting library

- [x] **Task 3**: Configure Program.cs
  - Add MudBlazor services
  - Add OpenCode Blazor services
  - Configure OpenCode client with base URL
  - Add SignalR for real-time updates

- [x] **Task 4**: Set up appsettings.json
  - OpenCode base URL configuration
  - Demo settings (MockBackendEnabled)
  - Logging configuration

### Project Structure

- [x] **Task 5**: Create component folder structure
  ```
  Components/
    Layout/
      MainLayout.razor
      IdeLayout.razor
    Pages/
      Home.razor
      Ide.razor
    Shared/
      FileTree/
      Chat/
      DiffViewer/
      Terminal/
  ```

- [x] **Task 6**: Set up Routes.razor and App.razor
  - Configure routing
  - Set default layout to MainLayout

- [x] **Task 7**: Configure theme and CSS
  - Import OpenCode theme from LionFire.OpenCode.Blazor
  - Add custom IDE-specific styles
  - Configure dark mode as default

## Acceptance Criteria

- [x] Project builds successfully for both net8.0 and net9.0
- [x] All dependencies resolve correctly
- [x] Basic page loads at `/` and `/ide`
- [x] MudBlazor components render properly
- [x] OpenCode theme applied (dark mode)
- [x] Project structure follows conventions

## Dependencies

- LionFire.OpenCode.Blazor must be available
- LionFire.OpenCode.Serve must be available

## Notes

Reference the AgUi.Chat.BlazorServer project for patterns:
- `/src/opencode-dotnet/samples/AgUi.Chat.BlazorServer/`

## Completion

**Completed**: 2025-12-12
**Implementation Notes**:
- Project added to solution file
- Four-pane IDE layout created (file tree, chat, diff viewer, terminal)
- Services created: IdeStateService, FileTreeService, OpenCodeLauncher
- All tasks implemented with placeholder components for future phases
