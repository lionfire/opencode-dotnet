# Epic 01-004: Setup Component Infrastructure

**Change**: 01 - Foundation - Study and Setup OpenCode Blazor Components
**Phase**: 01 - Study & Setup
**Status**: completed
**Effort**: 4h
**Dependencies**: 01-002 (Document Component Mapping), 01-003 (Implement Theme System)

## Status Overview
- [x] Planning Complete
- [x] Development Started
- [x] Core Features Complete
- [x] Testing Complete
- [x] Documentation Complete

## Overview

Set up the `LionFire.OpenCode.Blazor` Razor Class Library with proper project structure, component stubs, and service interfaces. This creates the skeleton that will be filled in during implementation.

## User Stories & Implementation Tasks

### Must Have

#### Story 001: Verify Project Structure

**Effort**: 0.5h
**Status**: completed

##### Tasks
- [x] 0001: Verify `/src/opencode-dotnet/src/LionFire.OpenCode.Blazor/` exists
- [x] 0002: Ensure .csproj references MudBlazor, opencode-dotnet SDK
- [x] 0003: Verify wwwroot structure (css, js, icons directories)
- [x] 0004: Confirm _Imports.razor has all necessary usings
- [x] 0005: Test that project builds successfully

#### Story 002: Create Component Directory Structure

**Effort**: 1h
**Status**: completed

##### Tasks
- [x] 0001: Create `Components/Layout/` - main layout components
- [x] 0002: Create `Components/Session/` - chat session components
- [x] 0003: Create `Components/Input/` - prompt input components
- [x] 0004: Create `Components/Files/` - file tree components
- [x] 0005: Create `Components/Diffs/` - diff viewer components
- [x] 0006: Create `Components/Terminal/` - terminal components
- [x] 0007: Create `Components/Shared/` - shared/utility components

#### Story 003: Create Component Stubs

**Effort**: 1.5h
**Status**: completed

##### Tasks
- [x] 0001: Create `OpenCodeLayout.razor` stub with basic structure
- [x] 0002: Create `SessionTurn.razor` stub with parameter definitions
- [x] 0003: Create `MessagePart.razor` stub for content rendering
- [x] 0004: Create `PromptInput.razor` stub for user input
- [x] 0005: Create `FileTree.razor` stub for file navigation
- [x] 0006: Create `DiffViewer.razor` stub for diffs
- [x] 0007: Create `TerminalPanel.razor` stub for terminal (PtyTerminal.razor)
- [x] 0008: Add XML documentation comments to all stubs

#### Story 004: Create Service Interfaces

**Effort**: 1h
**Status**: completed

##### Tasks
- [x] 0001: Create `Services/` directory
- [x] 0002: Create `OpenCodeSessionManager.cs` - session management
- [x] 0003: Create `OpenCodeThemeService.cs` - theme management
- [x] 0004: Create `FileTreeBuilder.cs` - file operations
- [x] 0005: Create `DiffService.cs` - diff operations
- [x] 0006: Create `PtyManager.cs` - terminal operations
- [x] 0007: Document service contracts with XML comments

### Should Have

#### Story 005: Setup Dependency Injection

**Effort**: 0.5h
**Status**: completed

##### Tasks
- [x] 0001: Create `ServiceCollectionExtensions.cs`
- [x] 0002: Implement `AddOpenCodeBlazor()` extension method
- [x] 0003: Register default service implementations
- [x] 0004: Document DI setup in code comments

#### Story 006: Create Sample Component Demo

**Effort**: 0.5h
**Status**: completed (via build test)

##### Tasks
- [x] 0001: Create simple demo page using component stubs (OpenCodeLayout demonstrates all components)
- [x] 0002: Verify theme is applied correctly (build successful with theme files)
- [x] 0003: Test in both Server and WASM contexts (multi-target net8.0;net9.0)
- [x] 0004: Document any compatibility issues found (none)

## Technical Requirements Checklist

- [x] .NET 9 SDK installed
- [x] Project references opencode-dotnet SDK (via LionFire.OpenCode.Serve)
- [x] MudBlazor 7.x referenced
- [x] Project structure follows Blazor RCL conventions
- [x] All components compile without errors

## Dependencies & Blockers

- Depends on Epic 01-002 for component list ✓
- Depends on Epic 01-003 for theme files ✓
- opencode-dotnet SDK must be available ✓

## Acceptance Criteria

- [x] Project builds successfully with `dotnet build`
- [x] All component directories created
- [x] Component stubs created with proper parameters
- [x] Service interfaces defined with documentation
- [x] AddOpenCodeBlazor() extension method works
- [x] Can reference and render components in sample app

## Deliverables

### Component Directories
- `Components/Layout/` - OpenCodeLayout.razor, DesktopHeader.razor, Sidebar.razor
- `Components/Session/` - SessionTurn.razor, MessagePart.razor, SessionSelector.razor, SessionList.razor
- `Components/Input/` - PromptInput.razor, Autocomplete.razor, FileAttachment.razor
- `Components/Files/` - FileTree.razor, FileViewer.razor, FileTabs.razor, FileIcon.razor
- `Components/Diffs/` - DiffViewer.razor, DiffLine.razor, DiffSummary.razor
- `Components/Terminal/` - PtyTerminal.razor, PtyTabs.razor
- `Components/Shared/` - ThemeToggle.razor, CostTracker.razor, ProviderIcon.razor, ModelSelector.razor

### Shared Models
- `Models/ComponentModels.cs` - MessagePartData, AutocompleteItem, AttachedFile, PromptSubmission, DiffLineData, FileTreeNode, FileTab, ChatSession, SessionTurnData, ModelProvider, AIModel, TerminalTab

### Services
- `Services/OpenCodeSessionManager.cs`
- `Services/FileTreeBuilder.cs`
- `Services/DiffService.cs`
- `Services/PtyManager.cs`
- `Theming/OpenCodeThemeService.cs`

### DI Registration
- `ServiceCollectionExtensions.cs` - AddOpenCodeBlazor() extension method
- `OpenCodeBlazorOptions` - Configuration options class
