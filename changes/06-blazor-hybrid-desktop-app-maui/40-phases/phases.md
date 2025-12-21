# Change 06: Blazor Hybrid Desktop App (MAUI) - Phases

## Overview

This document outlines the implementation phases for the Blazor Hybrid desktop app built with MAUI.

## Phase Summary

| Phase | Name | Goal | Epics |
|-------|------|------|-------|
| 01 | Project Setup | Create MAUI Blazor Hybrid project | TBD |
| 02 | Component Integration | Integrate Blazor components into MAUI | TBD |
| 03 | OpenCode Integration | Bundle and manage OpenCode serve | TBD |
| 04 | Packaging & Distribution | Create installers and distribution | TBD |

---

## Phase 01: Project Setup

**Goal**: Create .NET MAUI Blazor Hybrid project structure

**Deliverables**:
1. MAUI Blazor Hybrid project (AgUi.IDE.Desktop)
2. Project configuration for Windows and Mac targets
3. MAUI shell and navigation setup
4. Basic layout structure
5. Build configuration for both platforms

**Success Criteria**:
- Project builds for Windows
- Project builds for Mac
- MAUI shell renders properly
- No build warnings

---

## Phase 02: Component Integration

**Goal**: Integrate Blazor IDE components into MAUI desktop app

**Deliverables**:
1. Blazor components hosted in MAUI
2. File tree component integrated
3. Chat component integrated
4. Diff viewer integrated
5. Terminal/output integrated
6. Resizable pane layout

**Success Criteria**:
- All components display in desktop app
- Components function correctly
- Layout looks polished
- No rendering issues

---

## Phase 03: OpenCode Integration

**Goal**: Bundle OpenCode and manage its lifecycle

**Deliverables**:
1. OpenCode serve binary embedded in app resources
2. OpenCode process manager
3. Automatic startup on app launch
4. Graceful shutdown on app close
5. Process monitoring and restart logic
6. IPC/HTTP communication with OpenCode

**Success Criteria**:
- OpenCode starts automatically
- Connection to OpenCode server works
- IDE can interact with OpenCode
- Process restarts if crashed
- App shuts down cleanly

---

## Phase 04: Packaging & Distribution

**Goal**: Create installers and prepare for distribution

**Deliverables**:
1. Windows installer (.msi) using WiX or similar
2. Mac installer (.dmg)
3. Code signing configuration
4. Auto-update capability
5. User documentation and guides
6. Release notes

**Success Criteria**:
- Windows installer creates working application
- Mac installer creates working application
- Installers are codesigned
- Users can download and install app
- App runs correctly after installation
- All features work in installed version

---

## Next Steps

1. Create epics: `/ax:epic:create 06-001 "Create MAUI Project" --change 06`
2. Greenlight epics
3. Implement with `/ax:change:implement 06`
