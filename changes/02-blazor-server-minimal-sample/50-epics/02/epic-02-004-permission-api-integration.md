# Epic 02-004: Permission API Integration

**Phase**: 02 - Permission Request Support
**Status**: Draft
**Priority**: High
**Effort**: 1.5h

## Description

Implement the API integration to send permission responses back to OpenCode. This epic connects the UI approval/denial actions to the OpenCode backend via the SDK's `RespondToPermissionAsync` method.

## Objectives

1. Call `RespondToPermissionAsync` when user approves/denies
2. Pass correct sessionId, permissionId, and response payload
3. Handle API errors gracefully (retry, timeout)
4. Resume message streaming after permission response
5. Implement "remember" functionality

## Stories

### Story 1: Implement Permission Response Method

- [ ] Add `RespondToPermissionAsync` method to chat service
- [ ] Accept permissionId, allow (bool), remember (bool)
- [ ] Call `_client.RespondToPermissionAsync()` with correct parameters
- [ ] Update permission state after successful response
- [ ] Return success/failure result

**Files**:
- `samples/AgUi.Chat.BlazorServer/Services/OpenCodeChatService.cs`
- `samples/AgUi.Chat.BlazorServer/Services/IChatService.cs` (add method)

### Story 2: Wire Dialog to API

- [ ] Handle dialog result in Chat.razor
- [ ] Call permission response method with dialog result
- [ ] Show loading indicator during API call
- [ ] Handle success (close dialog, continue)
- [ ] Handle failure (show error, allow retry)

**Files**:
- `samples/AgUi.Chat.BlazorServer/Components/Pages/Chat.razor`

### Story 3: Error Handling and Retry

- [ ] Catch API exceptions in response method
- [ ] Log errors with details
- [ ] Show user-friendly error message
- [ ] Provide retry button in dialog
- [ ] Set timeout for permission responses (optional)

**Files**:
- `samples/AgUi.Chat.BlazorServer/Services/OpenCodeChatService.cs`
- `samples/AgUi.Chat.BlazorServer/Components/Dialogs/PermissionDialog.razor`

### Story 4: Implement Remember Functionality

- [ ] Store remembered decisions in session state
- [ ] Check for remembered decision before showing dialog
- [ ] Auto-respond if pattern matches remembered decision
- [ ] Provide UI to view/clear remembered decisions
- [ ] Scope remembering appropriately (session-level initially)

**Files**:
- `samples/AgUi.Chat.BlazorServer/Services/PermissionStateService.cs`
- `samples/AgUi.Chat.BlazorServer/Components/Pages/Chat.razor`

## Technical Notes

- SDK method signature:
  ```csharp
  Task RespondToPermissionAsync(
      string sessionId,
      string permissionId,
      PermissionResponse response,
      string? directory = null,
      CancellationToken cancellationToken = default);
  ```

- `PermissionResponse` model:
  ```csharp
  public record PermissionResponse
  {
      [JsonPropertyName("allow")]
      public required bool Allow { get; init; }

      [JsonPropertyName("remember")]
      public bool? Remember { get; init; }
  }
  ```

- API endpoint: `POST /session/{sessionId}/permissions/{permissionId}`

## Dependencies

- **Requires**: Epic 02-003 (UI captures user response)
- **Blocks**: Epic 02-005 (tool execution follows permission approval)

## Acceptance Criteria

- [ ] Permission response sent to OpenCode successfully
- [ ] Success/failure status shown to user
- [ ] Retry available on failure
- [ ] Remember functionality works
- [ ] Message streaming continues after approval
- [ ] Denial shows appropriate message
- [ ] No memory leaks from failed requests

## References

- `src/LionFire.OpenCode.Serve/IOpenCodeClient.cs:291-301` - Permission API method
- `src/LionFire.OpenCode.Serve/Models/Permission.cs:111-124` - PermissionResponse model
- `src/LionFire.OpenCode.Serve/Internal/ApiEndpoints.cs:37` - API endpoint path
