# Epic 02-002: Permission Event Handling

**Phase**: 02 - Permission Request Support
**Status**: Draft
**Priority**: High
**Effort**: 2h

## Description

Handle `PermissionUpdatedEvent` in the SSE event loop to detect permission requests from OpenCode. This epic connects the incoming permission events to the state management system and triggers UI notifications.

## Objectives

1. Detect `PermissionUpdatedEvent` in the SSE event stream
2. Extract permission details from event properties
3. Add detected permissions to state service
4. Trigger UI notification when permission is requested
5. Track permission lifecycle (requested -> responded)

## Stories

### Story 1: Handle PermissionUpdatedEvent in SSE Loop

- [ ] Add case for `PermissionUpdatedEvent` in event handling switch/pattern
- [ ] Extract `Properties.Permission` from the event
- [ ] Validate permission belongs to current session
- [ ] Log permission request for debugging

**Files**:
- `samples/AgUi.Chat.BlazorServer/Services/OpenCodeChatService.cs`

### Story 2: Convert SDK Permission to UI Model

- [ ] Map `Permission` (SDK) to `PermissionRequest` (UI model)
- [ ] Convert pattern to display-friendly string
- [ ] Extract relevant metadata for display
- [ ] Handle null/missing fields gracefully

**Files**:
- `samples/AgUi.Chat.BlazorServer/Services/OpenCodeChatService.cs`
- `samples/AgUi.Chat.BlazorServer/Models/PermissionModels.cs`

### Story 3: Integrate with Permission State Service

- [ ] Inject `IPermissionStateService` into OpenCodeChatService
- [ ] Call state service when permission detected
- [ ] Update state when `PermissionRepliedEvent` received
- [ ] Handle permission expiry/cancellation

**Files**:
- `samples/AgUi.Chat.BlazorServer/Services/OpenCodeChatService.cs`

### Story 4: Handle PermissionRepliedEvent

- [ ] Add case for `PermissionRepliedEvent` in event handling
- [ ] Update permission state to Approved/Denied
- [ ] Remove from pending, add to history
- [ ] Log reply for debugging

**Files**:
- `samples/AgUi.Chat.BlazorServer/Services/OpenCodeChatService.cs`

## Technical Notes

- `PermissionUpdatedEvent` structure:
  ```csharp
  public record PermissionUpdatedEvent : Event
  {
      [JsonPropertyName("properties")]
      public PermissionUpdatedProperties? Properties { get; init; }
  }

  public record PermissionUpdatedProperties
  {
      [JsonPropertyName("sessionID")]
      public string? SessionId { get; init; }

      [JsonPropertyName("permission")]
      public Permission? Permission { get; init; }
  }
  ```

- Permission model has these key fields:
  - `Id` - unique identifier for responding
  - `Type` - permission type (e.g., "file.write", "command.execute")
  - `Pattern` - what's being accessed (file path, command, etc.)
  - `Title` - human-readable description
  - `Metadata` - additional context (tool inputs, etc.)
  - `SessionId`, `MessageId`, `CallId` - context IDs

## Dependencies

- **Requires**: Epic 02-001 (permission state service)
- **Blocks**: Epic 02-003 (UI needs events to display)

## Acceptance Criteria

- [ ] `PermissionUpdatedEvent` detected in SSE stream
- [ ] Permission details extracted correctly
- [ ] State service updated with new permission
- [ ] `PermissionRepliedEvent` updates permission state
- [ ] Logging shows permission lifecycle
- [ ] No impact on existing message streaming

## References

- `src/LionFire.OpenCode.Serve/Models/Events/Event.cs:275-306` - Permission event types
- `src/LionFire.OpenCode.Serve/Models/Permission.cs` - Permission model
