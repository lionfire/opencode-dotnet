# Greenlit Epics

<!-- Add epics ready for implementation -->
<!-- Format: - XX-YYY -->

## Phase 01

- ~~01-001~~ (complete)
- ~~01-002~~ (complete)
- ~~01-003~~ (complete)
- 01-004 (in progress)

## Phase 02: Permission Support

- [ ] 02-001 - Permission State Management (2h)
- [ ] 02-002 - Permission Event Handling (2h, depends on 02-001)
- [ ] 02-003 - Permission UI Component (2h, depends on 02-002)
- [ ] 02-004 - Permission API Integration (1.5h, depends on 02-003)
- [ ] 02-005 - Tool Status Display (2h, depends on 02-004)

### Implementation Order

1. **02-001**: Add permission state to OpenCodeChatService
2. **02-002**: Parse permission events from SSE stream
3. **02-003**: Build MudBlazor permission dialog
4. **02-004**: Connect dialog to permission response API
5. **02-005**: Show tool execution status in chat

**Total Phase 02 Effort**: ~9.5 hours
