---
greenlit: true
implementationDone: true
implementationReviewed: true
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

- [x] Planning complete
- [x] Development in progress
- [x] Code review complete
- [x] Testing complete (streaming particularly important)
- [x] Documentation complete

## Technical Requirements

### API Endpoints
- [x] POST /session/{id}/message - Send message (multi-part)
- [x] POST /session/{id}/message/stream - Send message with streaming (SSE)
- [x] GET /session/{id}/message - List messages in session
- [x] GET /session/{id}/message/{messageId} - Get specific message

### DTOs
- [x] SendMessageRequest (parts: MessagePart[])
- [x] MessageUpdate (chunk for streaming - partial message, delta, etc.)
- [x] MessageListResponse (messages: Message[])

### Streaming Infrastructure
- [x] SSE parser for text/event-stream responses
- [x] IAsyncEnumerable<MessageUpdate> implementation
- [x] Chunk buffering and parsing logic
- [x] Connection keep-alive handling
- [x] Cancellation support for long-running streams

## Implementation Tasks

### 1. Message Operations
- [x] Implement SendMessageAsync(sessionId, SendMessageRequest, CancellationToken)
  - [x] Serialize multi-part message correctly
  - [x] Handle different MessagePart types
  - [x] Return completed Message
- [x] Implement GetMessagesAsync(sessionId, CancellationToken)
  - [x] Return ordered list of messages
  - [x] Include all message parts
- [x] Implement GetMessageAsync(sessionId, messageId, CancellationToken)
  - [x] Return specific message
  - [x] Handle not found case

### 2. Streaming Implementation
- [x] Implement SendMessageStreamingAsync(sessionId, SendMessageRequest, CancellationToken)
  - [x] POST to /stream endpoint with Accept: text/event-stream
  - [x] Return IAsyncEnumerable<MessageUpdate>
  - [x] Parse SSE format (data:, event:, id: fields)
  - [x] Handle connection errors and reconnection
  - [x] Propagate cancellation through stream
- [x] Create SSE parser helper
  - [x] ParseServerSentEvent(string line)
  - [x] Handle multi-line data fields
  - [x] Parse JSON payloads in data field
- [x] Buffer management
  - [x] Use Memory<T> and Span<T> for efficiency
  - [x] Avoid string allocations where possible

### 3. Multi-Part Message Support
- [x] TextPart serialization/deserialization
- [x] FilePart handling (path and optional content)
- [x] AgentPart handling
- [x] JSON polymorphic converter for MessagePart hierarchy
- [x] Validation of message part combinations

### 4. Error Handling
- [x] Handle streaming connection drops
- [x] Parse error events in SSE stream
- [x] Map errors to exceptions
- [x] Clean up resources on error

## Testing Tasks

### Unit Tests
- [x] Test SendMessageAsync with different message part types
- [x] Test message retrieval operations
- [x] Test SSE parser with various SSE formats
- [x] Test streaming cancellation
- [x] Test multi-part message serialization

### Integration Tests
- [x] Send text message and receive response
- [x] Send multi-part message (text + file)
- [x] Stream message response and consume all chunks
- [x] Test cancellation during streaming
- [x] Test error handling in streaming

## Dependencies & Blockers

**Depends on**: Epic 01-001 (Core Infrastructure), Epic 01-002 (Sessions)

## Acceptance Criteria

- [x] Send message with all part types works
- [x] Streaming returns IAsyncEnumerable correctly
- [x] SSE parsing handles all event types
- [x] Cancellation stops streaming immediately
- [x] Integration tests pass with real OpenCode server
- [x] >80% code coverage

## Notes

- SSE format: `data: {...}\n\n` (double newline terminates event)
- OpenCode may send different event types (message, error, done)
- Consider memory usage for large file parts
- Streaming timeout should be longer than regular operations
