---
greenlit: true
---

# Epic 01-004: Tool and File Operations API

**Phase**: 01 - MVP Foundation
**Status**: Not Started
**Estimated Effort**: 4-5 days
**Priority**: High

[← Back to Phase 01](../../40-phases/01-mvp-foundation.md)

## Overview

Implement tool management (list, get, approve, permissions) and file operations (list, get content, search, apply changes) APIs. Also implement command execution API.

## Implementation Tasks

### Tool Management API
- [ ] GET /tool - ListToolsAsync()
- [ ] GET /tool/{id} - GetToolAsync(id)
- [ ] POST /session/{id}/tool/{toolId}/approve - ApproveToolAsync()
- [ ] PATCH /session/{id}/tool/{toolId}/permissions - UpdateToolPermissionsAsync()

### File Operations API
- [ ] GET /session/{id}/file?path={path} - ListFilesAsync(sessionId, path)
- [ ] GET /session/{id}/file/content?path={path} - GetFileContentAsync(sessionId, path)
- [ ] POST /session/{id}/file/search - SearchFilesAsync(sessionId, query)
- [ ] POST /session/{id}/file/apply - ApplyChangesAsync(sessionId, changes)

### Command API
- [ ] POST /session/{id}/command - ExecuteCommandAsync(sessionId, command)

### Configuration API
- [ ] GET /config - GetConfigAsync()
- [ ] GET /provider - GetProvidersAsync()
- [ ] GET /model - GetModelsAsync()

## Testing Tasks

- [ ] Unit tests for all endpoints with mocked HTTP
- [ ] Integration tests with real server
- [ ] Test file operations with various file types
- [ ] Test tool approval workflow

## Dependencies

**Depends on**: Epic 01-001, 01-002

## Acceptance Criteria

- [ ] All endpoints implemented and tested
- [ ] File paths handled correctly (Windows/Linux)
- [ ] Tool permissions work correctly
- [ ] Command execution returns results
- [ ] >75% code coverage
