# Epic 02-001: Permission State Management

**Phase**: 02 - Permission Request Support
**Status**: Draft
**Priority**: High
**Effort**: 2h

## Description

Add permission tracking infrastructure to the OpenCodeChatService and related components. This epic establishes the foundation for permission handling by creating state management for pending and responded permissions.

## Objectives

1. Create permission state models for UI consumption
2. Track pending permissions (awaiting user response)
3. Track responded permissions (for history/reference)
4. Expose permission state changes via events or callbacks
5. Integrate with existing chat service architecture

## Stories

### Story 1: Create Permission State Models

- [ ] Create `PermissionRequest` record for UI consumption
  - Id, Type, Title, Pattern (display-friendly), Metadata, Timestamp
- [ ] Create `PermissionState` enum (Pending, Approved, Denied, Expired)
- [ ] Create `PermissionStateChangedEventArgs` for notifications
- [ ] Add XML documentation for all public types

**Files**:
- `samples/AgUi.Chat.BlazorServer/Models/PermissionModels.cs` (new)

### Story 2: Add Permission Tracking to Chat Service

- [ ] Add `IObservable<PermissionRequest>` or event for permission requests
- [ ] Add `Dictionary<string, PermissionRequest>` for pending permissions
- [ ] Add `List<PermissionRequest>` for permission history (optional)
- [ ] Add method to get current pending permissions
- [ ] Add method to clear/reset permissions on session change

**Files**:
- `samples/AgUi.Chat.BlazorServer/Services/OpenCodeChatService.cs`

### Story 3: Create Permission State Service

- [ ] Create `IPermissionStateService` interface
- [ ] Implement `PermissionStateService` with state tracking
- [ ] Add DI registration in Program.cs
- [ ] Wire up to OpenCodeChatService for permission events

**Files**:
- `samples/AgUi.Chat.BlazorServer/Services/IPermissionStateService.cs` (new)
- `samples/AgUi.Chat.BlazorServer/Services/PermissionStateService.cs` (new)
- `samples/AgUi.Chat.BlazorServer/Program.cs`

### Story 4: Unit Tests for Permission State

- [ ] Test permission state transitions
- [ ] Test pending/responded tracking
- [ ] Test event notification delivery
- [ ] Test cleanup on session change

**Files**:
- `samples/AgUi.Chat.BlazorServer.Tests/PermissionStateServiceTests.cs` (new, optional)

## Technical Notes

- Use existing `Permission` model from `LionFire.OpenCode.Serve.Models`
- Consider using `System.Reactive` for IObservable if reactive patterns needed
- Alternatively use standard .NET events for simpler implementation
- State service should be scoped to Blazor circuit (per-user)

## Dependencies

- **Requires**: Phase 01 completion (OpenCodeChatService exists)
- **Blocks**: Epic 02-002 (needs state management to store detected permissions)

## Acceptance Criteria

- [ ] Permission state models compile and are documented
- [ ] State service tracks pending permissions correctly
- [ ] Events/observables fire when permissions are added/updated
- [ ] Service is registered in DI container
- [ ] Cleanup works on session change/disconnect

## References

- `src/LionFire.OpenCode.Serve/Models/Permission.cs` - SDK permission model
- `samples/AgUi.Chat.BlazorServer/Services/OpenCodeChatService.cs` - Current chat service
