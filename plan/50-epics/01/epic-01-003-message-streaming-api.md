---
greenlit: true
---

# Epic 01-003: Message and Streaming API

**Phase**: 01 - MVP Foundation
**Status**: Implemented in v2.0
**Estimated Effort**: 1 week
**Priority**: Critical (core functionality)

[← Back to Phase 01](../../40-phases/01-mvp-foundation.md)

## Overview

Implement message sending, retrieval, and streaming APIs with full support for multi-part messages (TextPart, FilePart, AgentPart) and Server-Sent Events (SSE) streaming for real-time AI responses.

## Status Overview

- [ ] Planning complete
- [ ] Development in progress
- [ ] Code review complete
- [ ] Testing complete (streaming particularly important)
- [ ] Documentation complete

## Technical Requirements

### API Endpoints
- [ ] POST /session/{id}/message - Send message (multi-part)
- [ ] POST /session/{id}/message/stream - Send message with streaming (SSE)
- [ ] GET /session/{id}/message - List messages in session
- [ ] GET /session/{id}/message/{messageId} - Get specific message

### DTOs
- [ ] SendMessageRequest (parts: MessagePart[])
- [ ] MessageUpdate (chunk for streaming - partial message, delta, etc.)
- [ ] MessageListResponse (messages: Message[])

### Streaming Infrastructure
- [ ] SSE parser for text/event-stream responses
- [ ] IAsyncEnumerable<MessageUpdate> implementation
- [ ] Chunk buffering and parsing logic
- [ ] Connection keep-alive handling
- [ ] Cancellation support for long-running streams

## Implementation Tasks

### 1. Message Operations
- [ ] Implement SendMessageAsync(sessionId, SendMessageRequest, CancellationToken)
  - [ ] Serialize multi-part message correctly
  - [ ] Handle different MessagePart types
  - [ ] Return completed Message
- [ ] Implement GetMessagesAsync(sessionId, CancellationToken)
  - [ ] Return ordered list of messages
  - [ ] Include all message parts
- [ ] Implement GetMessageAsync(sessionId, messageId, CancellationToken)
  - [ ] Return specific message
  - [ ] Handle not found case

### 2. Streaming Implementation
- [ ] Implement SendMessageStreamingAsync(sessionId, SendMessageRequest, CancellationToken)
  - [ ] POST to /stream endpoint with Accept: text/event-stream
  - [ ] Return IAsyncEnumerable<MessageUpdate>
  - [ ] Parse SSE format (data:, event:, id: fields)
  - [ ] Handle connection errors and reconnection
  - [ ] Propagate cancellation through stream
- [ ] Create SSE parser helper
  - [ ] ParseServerSentEvent(string line)
  - [ ] Handle multi-line data fields
  - [ ] Parse JSON payloads in data field
- [ ] Buffer management
  - [ ] Use Memory<T> and Span<T> for efficiency
  - [ ] Avoid string allocations where possible

### 3. Multi-Part Message Support
- [ ] TextPart serialization/deserialization
- [ ] FilePart handling (path and optional content)
- [ ] AgentPart handling
- [ ] JSON polymorphic converter for MessagePart hierarchy
- [ ] Validation of message part combinations

### 4. Error Handling
- [ ] Handle streaming connection drops
- [ ] Parse error events in SSE stream
- [ ] Map errors to exceptions
- [ ] Clean up resources on error

## Testing Tasks

### Unit Tests
- [ ] Test SendMessageAsync with different message part types
- [ ] Test message retrieval operations
- [ ] Test SSE parser with various SSE formats
- [ ] Test streaming cancellation
- [ ] Test multi-part message serialization

### Integration Tests
- [ ] Send text message and receive response
- [ ] Send multi-part message (text + file)
- [ ] Stream message response and consume all chunks
- [ ] Test cancellation during streaming
- [ ] Test error handling in streaming

## Dependencies & Blockers

**Depends on**: Epic 01-001 (Core Infrastructure), Epic 01-002 (Sessions)

## Acceptance Criteria

- [ ] Send message with all part types works
- [ ] Streaming returns IAsyncEnumerable correctly
- [ ] SSE parsing handles all event types
- [ ] Cancellation stops streaming immediately
- [ ] Integration tests pass with real OpenCode server
- [ ] >80% code coverage

## Notes

- SSE format: `data: {...}\n\n` (double newline terminates event)
- OpenCode may send different event types (message, error, done)
- Consider memory usage for large file parts
- Streaming timeout should be longer than regular operations
