# Epic 03-004: Chat Interface Component

**Phase**: 02 - Core Components
**Status**: Complete
**Priority**: High

## Overview

Build the chat interface component for AI conversations, including message display and streaming support.

## Stories / Tasks

### Message Display

- [ ] **Task 1**: Define ChatMessage model
  - Content, Timestamp, IsUser properties
  - IsStreaming flag for in-progress messages
  - MessageParts for rich content (text, code, etc.)

- [ ] **Task 2**: Create ChatMessageList.razor component
  - Scrollable message container
  - Auto-scroll to latest message
  - Message grouping by sender
  - Timestamp display

- [ ] **Task 3**: Create ChatMessage.razor component
  - User vs Assistant styling
  - Avatar/icon display
  - Markdown rendering for assistant messages
  - Code block syntax highlighting

### Input Area

- [ ] **Task 4**: Create ChatInput.razor component
  - Text input with Enter-to-send
  - Multi-line support (Shift+Enter for newline)
  - Send button
  - Disabled state during response

- [ ] **Task 5**: Add input enhancements
  - Command suggestions (optional)
  - File attachment support (optional)
  - Input history (up/down arrows, optional)

### Streaming Support

- [ ] **Task 6**: Implement streaming message display
  - Progressive content rendering
  - Typing indicator
  - Cancel button during streaming

- [ ] **Task 7**: Connect to chat service
  - Use IChatService interface
  - Handle streaming async enumerable
  - Error display in chat

### Styling

- [ ] **Task 8**: Style chat to match OpenCode
  - Message bubbles with proper colors
  - Consistent spacing and typography
  - Code blocks with dark theme
  - Loading/streaming indicators

## Acceptance Criteria

- [ ] Messages display correctly for user and assistant
- [ ] Streaming messages show progressive content
- [ ] Input handles Enter and Shift+Enter properly
- [ ] Auto-scroll works as expected
- [ ] Markdown and code blocks render correctly
- [ ] Styling matches OpenCode design

## Dependencies

- Epic 03-001, 03-002 must be complete
- IChatService interface from AgUi.Chat.BlazorServer

## Notes

Reference existing chat implementation:
- `/src/opencode-dotnet/samples/AgUi.Chat.BlazorServer/Components/Pages/Chat.razor`

Consider using Markdig for Markdown rendering.
