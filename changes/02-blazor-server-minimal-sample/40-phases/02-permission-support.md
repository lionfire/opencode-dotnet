# Phase 02: Permission Request Support

**Source**: change - 02-blazor-server-minimal-sample
**Duration**: 6-8 hours
**Focus**: Human-in-the-middle permission handling for tool execution

## Motivation

OpenCode requests permission before executing certain tools (file operations, bash commands, etc.). Without permission handling, tool-based responses appear empty because the model waits indefinitely for permission approval. This phase implements the complete permission request/response flow to enable tool-based interactions.

Currently, when users ask questions like "list files" or "read package.json", the OpenCode backend emits `permission.updated` events (type: "permission.updated") but the chat UI has no mechanism to:
1. Detect these permission requests
2. Display them to the user
3. Allow the user to approve/deny
4. Send the response back to OpenCode

This results in incomplete or empty responses, severely limiting the chat sample's functionality.

## Goals and Objectives

1. **Enable Tool-Based Interactions**: Allow users to ask questions that require tool execution (file operations, bash commands)
2. **Implement Permission UI Flow**: Display permission requests to users in a clear, actionable format
3. **Complete Permission Lifecycle**: Handle the full request -> display -> approve/deny -> execute cycle
4. **Maintain UX Quality**: Ensure permission requests don't disrupt the chat experience
5. **Provide Visibility**: Show users what tools are being executed and their results

## Rationale

This phase addresses the most critical limitation of Phase 1 - the inability to handle tool-based queries. The work is grouped into this phase because:

1. **Dependency Chain**: Permission handling requires all of Phase 1's infrastructure (SSE streaming, session management, event handling)
2. **Feature Cohesion**: All components (state management, event handling, UI, API) work together to deliver one feature
3. **Complexity**: Permission handling involves multiple moving parts that should be implemented together
4. **Testing**: The feature can only be tested when all components are complete

The ordering of epics follows the natural data flow: state management -> event detection -> UI display -> API response -> status feedback.

## Key Deliverables

**Permission State Management**:
- Add permission tracking to OpenCodeChatService
- Store pending permissions (awaiting user response)
- Store responded permissions (for history/reference)
- Expose permission state via events or callbacks

**Permission Event Handling**:
- Handle `PermissionUpdatedEvent` in SSE event loop
- Extract permission details from event properties
- Trigger UI notification when permission is requested
- Track permission lifecycle (requested -> responded)

**Permission UI Component**:
- MudBlazor dialog or card component for permission requests
- Display tool name, description, and inputs (e.g., file paths)
- Provide "Allow" and "Deny" buttons
- Optional "Remember this decision" checkbox
- Support for multiple simultaneous permission requests (queue)

**Permission API Integration**:
- Call `RespondToPermissionAsync` when user approves/denies
- Pass correct sessionId, permissionId, and response payload
- Handle API errors gracefully (retry, timeout)
- Resume message streaming after permission response

**Tool Status Display**:
- Show tool execution status in chat message area
- Display tool name and operation (e.g., "Reading file: package.json")
- Show success/failure indicators
- Render tool outputs appropriately (text, file content, etc.)

## Success Criteria

- [ ] User can ask "list files in current directory" and see results
- [ ] Permission dialog appears when tool execution is requested
- [ ] User can approve or deny permission requests
- [ ] Approved tools execute and results appear in chat
- [ ] Denied tools show appropriate message (permission denied)
- [ ] Multiple permission requests are handled correctly (queue or batch)
- [ ] Tool execution status is visible in the chat UI
- [ ] "Remember this decision" option works (auto-approve future requests)
- [ ] No regressions to Phase 1 streaming functionality
- [ ] Error handling works (permission timeout, API failure)

## Dependencies

**Prerequisites**:
- Phase 01 (Basic Chat Sample) - Must be complete with working SSE streaming and session management

**Blocks**:
- Phase 03 (if planned) - Advanced features like file upload, image display, multi-turn tool conversations
- Full parity with OpenCode web UI tool capabilities

## Suggested Epics

Based on the objectives and deliverables, this phase should include approximately 5 epics:

- Epic 02-001: Permission State Management - Add permission tracking to OpenCodeChatService, store pending/responded permissions, expose state via events
- Epic 02-002: Permission Event Handling - Handle PermissionUpdatedEvent in SSE loop, extract permission details, trigger UI notifications
- Epic 02-003: Permission UI Component - Create MudBlazor dialog for permission requests with approve/deny/remember options
- Epic 02-004: Permission API Integration - Call RespondToPermissionAsync, handle responses, resume streaming after permission granted
- Epic 02-005: Tool Status Display - Show tool execution status in chat UI with operation details and success/failure indicators

**Note**: Actual epic files are generated by `/ax:phase:execute` or the parent execute command.

## Risks and Mitigations

- **Risk**: Permission requests may arrive during active message streaming, causing UI confusion
  - **Mitigation**: Queue permission requests and display them in a non-blocking panel (e.g., sidebar or floating dialog)

- **Risk**: Users may accidentally deny critical permissions, breaking the conversation
  - **Mitigation**: Provide clear messaging about what each permission does, allow users to retry or reset permissions

- **Risk**: Multiple simultaneous permission requests may overwhelm the UI
  - **Mitigation**: Batch or queue permissions, show count indicator, allow batch approval for related permissions

- **Risk**: Permission API calls may fail or timeout
  - **Mitigation**: Implement retry logic with exponential backoff, show error state in UI, allow manual retry

- **Risk**: "Remember this decision" may create unexpected auto-approvals
  - **Mitigation**: Provide UI to view and clear remembered permissions, scope remembering to session or pattern-specific

- **Risk**: Tool outputs may contain sensitive data (file contents, environment variables)
  - **Mitigation**: Sanitize or truncate outputs in UI, provide expand/collapse for long outputs, warn users about sensitive data exposure
