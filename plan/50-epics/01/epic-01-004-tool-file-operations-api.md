---
greenlit: true
---

# Epic 01-004: File Operations, Permissions, and Command APIs

**Phase**: 01 - MVP Foundation
**Status**: Implemented in v2.0
**Estimated Effort**: 4-5 days
**Priority**: High

[← Back to Phase 01](../../40-phases/01-mvp-foundation.md)

> **Note**: This epic was redesigned in v2.0 to match the actual OpenCode serve API. The original plan assumed session-scoped file/tool operations, but the actual API uses directory-scoped file operations and generic permission handling.

## Overview

Implement directory-scoped file operations (list, read, search), generic permission handling, and session command execution APIs to match the actual OpenCode serve API v0.0.3.

## Implementation Tasks

### Permission Management API (Generic, not tool-specific)
- [x] POST /session/{id}/permissions/{permissionId} - RespondToPermissionAsync()
  - Accepts: `{ response: "once" | "always" | "reject" }`
  - Handles permissions for all operations (bash, edit, webfetch, etc.)

### File Operations API (Directory-scoped, not session-scoped)
- [x] GET /file?path={path}&directory={dir} - ListFilesAsync(path, directory)
  - Returns list of FileNode (files and directories)
  - Directory parameter sets instance context

- [x] GET /file/content?path={path}&directory={dir} - ReadFileAsync(path, directory)
  - Returns FileContent with path and content

- [x] GET /file/status?directory={dir} - GetFileStatusAsync(directory)
  - Returns git status for files

### Find Operations API (Text and file search)
- [x] GET /find?pattern={pattern}&directory={dir} - FindTextAsync(pattern, directory)
  - Text search using ripgrep
  - Returns list of matches

- [x] GET /find/file?query={query}&directory={dir} - FindFilesAsync(query, directory)
  - File name search
  - Returns list of file paths

- [x] GET /find/symbol?query={query}&directory={dir} - FindSymbolsAsync(query, directory)
  - LSP workspace symbol search
  - Returns list of symbols

### Session Command API
- [x] POST /session/{id}/command - ExecuteSessionCommandAsync(sessionId, command)
  - Execute slash commands in session context
  - Returns MessageWithParts

- [x] POST /session/{id}/shell - ExecuteSessionShellAsync(sessionId, shellRequest)
  - Execute shell command with AI assistance
  - Returns AssistantMessage

### Configuration API
- [x] GET /config?directory={dir} - GetConfigAsync(directory)
  - Returns Dictionary<string, object> with config

- [x] PATCH /config?directory={dir} - UpdateConfigAsync(config, directory)
  - Update OpenCode configuration

- [x] GET /config/providers?directory={dir} - GetConfiguredProvidersAsync(directory)
  - Returns configured AI providers

- [x] GET /provider?directory={dir} - GetAllProvidersAsync(directory)
  - Returns all available providers

- [x] GET /provider/auth?directory={dir} - GetProviderAuthMethodsAsync(directory)
  - Returns available auth methods per provider

## Testing Tasks

- [x] Unit tests for all endpoints with mocked HTTP
- [x] Integration tests with real server
- [x] Test file operations with various file types
- [x] Test permission workflow with RespondToPermissionAsync
- [x] Test command execution
- [x] Test find operations (text, files, symbols)

## Dependencies

**Depends on**: Epic 01-001, 01-002

## Acceptance Criteria

- [x] All endpoints implemented and tested
- [x] File paths handled correctly (Windows/Linux)
- [x] Permission system works correctly (generic, not tool-specific)
- [x] Command execution returns results
- [x] >75% code coverage
- [x] Directory parameter properly scopes all operations

## v2.0 Implementation Notes

**Key Changes from Original Plan**:
1. **File operations are directory-scoped**, not session-scoped
   - No `/session/{id}/file` endpoints exist
   - Use `/file?directory=X` instead

2. **Permissions are generic**, not tool-specific
   - No `/session/{id}/tool/{id}/approve` endpoint
   - Use `/session/{id}/permissions/{permissionId}` instead
   - Covers all operation types (bash, edit, webfetch, etc.)

3. **Search operations separate from file operations**
   - `/find` for text search (ripgrep)
   - `/find/file` for file name search
   - `/find/symbol` for LSP symbol search

4. **No batch file apply API**
   - File changes happen through AI tool calls in conversations
   - Not a direct SDK operation

**Implementation Location**: `/src/opencode-dotnet/src/LionFire.OpenCode.Serve/OpenCodeClient.cs` (lines 318-380)
