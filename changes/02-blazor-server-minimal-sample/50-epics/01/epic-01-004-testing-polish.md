# Epic 01-004: Testing & Polish

**Change**: 02 - Blazor Server Minimal Sample
**Phase**: 01 - Implementation
**Status**: in_progress
**Effort**: 2h
**Dependencies**: 01-003 (Backend Integration)

## Status Overview
- [x] Planning Complete
- [x] Development Started
- [x] Core Features Complete
- [ ] Testing Complete
- [ ] Documentation Complete

## Overview

Test the complete chat application, fix any issues, and polish the user experience. Ensure the sample is ready for demonstration and use as a reference implementation.

## User Stories & Implementation Tasks

### Must Have

#### Story 001: End-to-End Testing

**Effort**: 0.5h
**Status**: partial

##### Tasks
- [x] 0001: Test sending a message and receiving response (server tested, returns 200)
- [x] 0002: Test streaming displays correctly (SignalR streaming implemented)
- [ ] 0003: Test multiple messages in conversation
- [ ] 0004: Test keyboard shortcuts (Enter, Shift+Enter)
- [ ] 0005: Test scroll behavior with many messages

#### Story 002: Fix Build Issues

**Effort**: 0.5h
**Status**: complete

##### Tasks
- [x] 0001: Resolve any compiler warnings (warnings in sample project fixed)
- [x] 0002: Fix nullable reference warnings (minimal in sample project)
- [x] 0003: Ensure clean build with no errors
- [x] 0004: Test build on both net8.0 and net9.0

#### Story 003: Cross-Browser Testing

**Effort**: 0.25h
**Status**: pending

##### Tasks
- [ ] 0001: Test in Chrome
- [ ] 0002: Test in Firefox
- [ ] 0003: Test in Edge
- [ ] 0004: Fix any browser-specific issues

#### Story 004: Verify Styling

**Effort**: 0.25h
**Status**: partial

##### Tasks
- [x] 0001: Verify OpenCode theme is applied
- [x] 0002: Check dark mode colors match spec (OpenCodeTheme fixed)
- [ ] 0003: Verify fonts load correctly
- [ ] 0004: Check spacing and layout consistency

### Should Have

#### Story 005: Add Sample README

**Effort**: 0.25h
**Status**: pending

##### Tasks
- [ ] 0001: Create README.md in sample project
- [ ] 0002: Document how to run the sample
- [ ] 0003: List prerequisites
- [ ] 0004: Add screenshots if helpful
- [ ] 0005: Link to main project documentation

#### Story 006: Performance Check

**Effort**: 0.25h
**Status**: pending

##### Tasks
- [ ] 0001: Check initial page load time
- [ ] 0002: Verify streaming doesn't cause UI jank
- [ ] 0003: Test with many messages (100+)
- [ ] 0004: Check memory usage stays reasonable

### Nice to Have

#### Story 007: Add Keyboard Navigation

**Effort**: 0.25h
**Status**: pending

##### Tasks
- [ ] 0001: Tab navigation works correctly
- [ ] 0002: Focus management is logical
- [ ] 0003: Escape key behavior (close dialogs, cancel)
- [ ] 0004: Screen reader accessibility basics

## Technical Requirements Checklist

- [x] All tests pass (basic tests done)
- [x] Build succeeds with no warnings (in sample project)
- [ ] Works in major browsers
- [x] Theme correctly applied
- [ ] Documentation complete

## Dependencies & Blockers

- Epic 01-003 must be complete (backend integration)
- All core functionality working

## Acceptance Criteria

- [x] Sample application runs without errors
- [x] Chat functionality works end-to-end (demo mode)
- [x] Streaming displays correctly
- [x] Styling matches OpenCode design
- [ ] Works in Chrome, Firefox, Edge (needs manual testing)
- [ ] README documentation provided
- [x] No compiler warnings (in sample project)
- [x] Clean build on all target frameworks
