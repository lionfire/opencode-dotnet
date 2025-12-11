---
greenlit: true
---

# Epic 03-004: Streaming Integration

**Phase**: 03 - Agent Framework Integration
**Estimated Effort**: 3-4 days

## Overview

Implement streaming responses via Agent Framework's IAsyncEnumerable<AgentRunResponseUpdate> pattern.

## Tasks

- [ ] Implement RunStreamingAsync() in OpencodeAgent
- [ ] Convert OpenCode streaming chunks to AgentRunResponseUpdate
- [ ] Map streaming events (message, error, done)
- [ ] Update thread state as streaming completes
- [ ] Handle cancellation
- [ ] Test streaming performance (within 5% of native)
- [ ] Test error handling in streams

## Acceptance Criteria

- Streaming returns IAsyncEnumerable<AgentRunResponseUpdate>
- Real-time updates delivered
- Thread updated after streaming
- Cancellation works
- Performance <5% overhead
