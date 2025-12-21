# Epic 02-005: Tool Status Display

**Phase**: 02 - Permission Request Support
**Status**: Draft
**Priority**: Medium
**Effort**: 2h

## Description

Enhance the chat UI to show tool execution status and results. After a permission is granted, users should see what tool is being executed and its results, providing visibility into the AI's actions.

## Objectives

1. Show tool execution status in chat message area
2. Display tool name and operation details
3. Show success/failure indicators
4. Render tool outputs appropriately
5. Distinguish tool messages from regular chat messages

## Stories

### Story 1: Create Tool Status Message Component

- [ ] Create `ToolStatusMessage.razor` component
- [ ] Display tool name with icon
- [ ] Show operation description (e.g., "Reading file: package.json")
- [ ] Add status indicator (pending, running, success, failed)
- [ ] Style differently from regular chat messages

**Files**:
- `samples/AgUi.Chat.BlazorServer/Components/Shared/ToolStatusMessage.razor` (new)

### Story 2: Track Tool Execution in Chat

- [ ] Extend `ChatMessage` class to support tool messages
- [ ] Add `MessageType` enum (User, Assistant, Tool)
- [ ] Add tool-specific properties (ToolName, ToolStatus, ToolOutput)
- [ ] Insert tool messages into chat flow

**Files**:
- `samples/AgUi.Chat.BlazorServer/Components/Pages/Chat.razor`

### Story 3: Handle Tool Part Events

- [ ] Detect tool-related `MessagePartUpdatedEvent`
- [ ] Extract tool information from part data
- [ ] Create tool status messages in chat
- [ ] Update tool status as execution progresses

**Files**:
- `samples/AgUi.Chat.BlazorServer/Services/OpenCodeChatService.cs`

### Story 4: Render Tool Outputs

- [ ] Display text outputs in formatted blocks
- [ ] Show file contents with syntax highlighting (optional)
- [ ] Handle long outputs with expand/collapse
- [ ] Show errors in distinct style
- [ ] Truncate very large outputs with "show more" option

**Files**:
- `samples/AgUi.Chat.BlazorServer/Components/Shared/ToolStatusMessage.razor`
- `samples/AgUi.Chat.BlazorServer/Components/Shared/ToolOutputDisplay.razor` (new)

### Story 5: Add Tool Execution Timeline (Optional)

- [ ] Show timeline of tool executions within a response
- [ ] Display duration of each tool execution
- [ ] Allow expanding individual tool details
- [ ] Collapse completed tools to reduce visual noise

**Files**:
- `samples/AgUi.Chat.BlazorServer/Components/Shared/ToolExecutionTimeline.razor` (new, optional)

## Technical Notes

- Tool execution info comes from `MessagePartUpdatedEvent` with tool-related parts
- Look for parts with `Type` = "tool_use" or similar (verify from OpenCode SSE)
- Tool outputs may be in separate parts following the tool_use part
- Consider using `MudExpansionPanel` for collapsible tool outputs

Example tool message structure:
```csharp
public class ChatMessage
{
    public string Content { get; set; } = "";
    public bool IsUser { get; set; }
    public DateTime Timestamp { get; set; }
    public bool IsStreaming { get; set; }

    // New tool-related properties
    public MessageType Type { get; set; } = MessageType.Text;
    public string? ToolName { get; set; }
    public ToolStatus? ToolStatus { get; set; }
    public string? ToolOutput { get; set; }
}

public enum MessageType { Text, Tool }
public enum ToolStatus { Pending, Running, Success, Failed }
```

## Dependencies

- **Requires**: Epic 02-004 (permission approval triggers tool execution)
- **Blocks**: None (final epic in phase)

## Acceptance Criteria

- [ ] Tool execution shows in chat with distinct styling
- [ ] Tool name and operation visible
- [ ] Status updates as tool progresses
- [ ] Success/failure clearly indicated
- [ ] Tool output rendered appropriately
- [ ] Long outputs are collapsible
- [ ] No performance issues with many tool messages
- [ ] Accessibility maintained (screen reader support)

## References

- `src/LionFire.OpenCode.Serve/Models/Parts/Part.cs` - Part types including tool parts
- MudBlazor ExpansionPanels: https://mudblazor.com/components/expansionpanels
- MudBlazor Cards: https://mudblazor.com/components/card
