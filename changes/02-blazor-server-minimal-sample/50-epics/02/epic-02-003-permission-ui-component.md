# Epic 02-003: Permission UI Component

**Phase**: 02 - Permission Request Support
**Status**: Draft
**Priority**: High
**Effort**: 2h

## Description

Create a MudBlazor dialog component for displaying permission requests to users. The component should clearly show what permission is being requested and allow users to approve or deny with optional "remember" functionality.

## Objectives

1. Create visually clear permission request dialog
2. Display tool name, description, and inputs
3. Provide "Allow" and "Deny" action buttons
4. Add "Remember this decision" option
5. Support multiple simultaneous permission requests (queue)

## Stories

### Story 1: Create PermissionDialog Component

- [ ] Create `PermissionDialog.razor` component
- [ ] Use `MudDialog` for modal display
- [ ] Display permission title prominently
- [ ] Show permission type with icon
- [ ] Display pattern/target (file path, command, etc.)
- [ ] Add close button (X) for dismissal

**Files**:
- `samples/AgUi.Chat.BlazorServer/Components/Dialogs/PermissionDialog.razor` (new)

### Story 2: Add Action Buttons

- [ ] Add "Allow" button (primary, green/success color)
- [ ] Add "Deny" button (secondary, red/error color)
- [ ] Add "Remember this decision" checkbox
- [ ] Style buttons according to OpenCode theme
- [ ] Handle button clicks with callbacks

**Files**:
- `samples/AgUi.Chat.BlazorServer/Components/Dialogs/PermissionDialog.razor`

### Story 3: Display Permission Details

- [ ] Show metadata in collapsible section
- [ ] Format file paths with icons
- [ ] Format command inputs as code blocks
- [ ] Show timestamp of request
- [ ] Add tooltip with full details if truncated

**Files**:
- `samples/AgUi.Chat.BlazorServer/Components/Dialogs/PermissionDialog.razor`

### Story 4: Integrate Dialog with Chat Page

- [ ] Inject `IDialogService` into Chat.razor
- [ ] Subscribe to permission state events
- [ ] Show dialog when permission requested
- [ ] Handle dialog result (allow/deny/remember)
- [ ] Support multiple queued permissions

**Files**:
- `samples/AgUi.Chat.BlazorServer/Components/Pages/Chat.razor`

### Story 5: Add Permission Queue Indicator

- [ ] Show badge/count when multiple permissions pending
- [ ] Allow viewing queue of pending permissions
- [ ] Support "Allow All" / "Deny All" for batch operations
- [ ] Persist queue across dialog dismissals

**Files**:
- `samples/AgUi.Chat.BlazorServer/Components/Pages/Chat.razor`
- `samples/AgUi.Chat.BlazorServer/Components/Shared/PermissionQueueIndicator.razor` (new, optional)

## Technical Notes

- Use `IDialogService.ShowAsync<PermissionDialog>()` for modal display
- Pass `PermissionRequest` as dialog parameter
- Return `PermissionResponse` (allow, remember) from dialog
- Consider non-blocking overlay instead of modal for better UX

Example dialog usage:
```csharp
var parameters = new DialogParameters<PermissionDialog>
{
    { x => x.Permission, permissionRequest }
};
var dialog = await DialogService.ShowAsync<PermissionDialog>("Permission Required", parameters);
var result = await dialog.Result;
if (!result.Canceled && result.Data is PermissionResponse response)
{
    // Handle response
}
```

## Dependencies

- **Requires**: Epic 02-001 (permission models), Epic 02-002 (event notifications)
- **Blocks**: Epic 02-004 (needs UI to capture user response)

## Acceptance Criteria

- [ ] Dialog displays permission details clearly
- [ ] Allow and Deny buttons work correctly
- [ ] Remember checkbox state is captured
- [ ] Dialog integrates with Chat page
- [ ] Multiple permissions handled via queue
- [ ] Styling matches OpenCode theme
- [ ] Dialog is accessible (keyboard navigation, ARIA labels)

## References

- MudBlazor Dialog: https://mudblazor.com/components/dialog
- OpenCode theme: `samples/AgUi.Chat.BlazorServer/OpenCodeTheme.cs`
