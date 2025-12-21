# Epic 01-001: Project Setup

**Change**: 02 - Blazor Server Minimal Sample
**Phase**: 01 - Implementation
**Status**: complete
**Effort**: 2h
**Dependencies**: Change 01 (Foundation - Theme System)

## Status Overview
- [x] Planning Complete
- [x] Development Started
- [x] Core Features Complete
- [x] Testing Complete
- [ ] Documentation Complete

## Overview

Create the AgUi.Chat.BlazorServer project with proper configuration, dependencies, and infrastructure for a minimal Blazor Server chat application.

## User Stories & Implementation Tasks

### Must Have

#### Story 001: Create Blazor Server Project

**Effort**: 0.5h
**Status**: complete

##### Tasks
- [x] 0001: Create new Blazor Server project `AgUi.Chat.BlazorServer`
- [x] 0002: Add project to solution file
- [x] 0003: Configure multi-targeting (net8.0;net9.0)
- [x] 0004: Add project reference to LionFire.OpenCode.Blazor
- [x] 0005: Add project reference to LionFire.OpenCode.Serve

#### Story 002: Configure MudBlazor

**Effort**: 0.5h
**Status**: complete

##### Tasks
- [x] 0001: Add MudBlazor NuGet package reference
- [x] 0002: Register MudBlazor services in Program.cs
- [x] 0003: Add MudBlazor imports to _Imports.razor
- [x] 0004: Configure MudThemeProvider in App.razor or layout
- [x] 0005: Apply OpenCodeTheme to MudThemeProvider

#### Story 003: Configure Theme and CSS

**Effort**: 0.5h
**Status**: complete

##### Tasks
- [x] 0001: Reference OpenCode CSS files in App.razor or index.html
- [x] 0002: Configure static file serving for wwwroot
- [x] 0003: Verify theme CSS loads correctly
- [x] 0004: Test dark mode is applied by default

#### Story 004: Setup SignalR Hub

**Effort**: 0.5h
**Status**: complete

##### Tasks
- [x] 0001: Create ChatHub class for real-time messaging
- [x] 0002: Register SignalR services in Program.cs
- [x] 0003: Map hub endpoint in app configuration
- [x] 0004: Add basic hub methods (SendMessage, JoinChat, LeaveChat)
- [x] 0005: Test hub connection from browser

### Should Have

#### Story 005: Configure Development Settings

**Effort**: 0.25h
**Status**: complete

##### Tasks
- [x] 0001: Configure appsettings.Development.json
- [x] 0002: Set up hot reload for development
- [x] 0003: Configure logging levels
- [x] 0004: Add launch settings for debugging

## Technical Requirements Checklist

- [x] .NET 9 SDK available
- [x] MudBlazor 7.x referenced
- [x] SignalR configured
- [x] OpenCode Blazor components accessible
- [x] Theme CSS loads correctly

## Dependencies & Blockers

- Change 01 must be complete (provides theme system and base components)
- LionFire.OpenCode.Blazor project must build successfully

## Acceptance Criteria

- [x] AgUi.Chat.BlazorServer project builds without errors
- [x] Project runs and displays default page
- [x] MudBlazor components render correctly
- [x] OpenCode theme is applied (dark mode default)
- [x] SignalR hub accepts connections
- [x] CSS and static files served correctly
