---
greenlit: true
implementationDone: true
implementationReviewed: true
---

# Epic 03-002: Message Conversion Layer

**Phase**: 03 - Agent Framework Integration
**Estimated Effort**: 3-4 days

## Overview

Implement bidirectional message conversion between Agent Framework ChatMessage and OpenCode message formats.

## Tasks

- [x] Create MessageConverter static utility class
- [x] ToChatMessage(Message) → ChatMessage
- [x] ToConversationMessage(ChatMessage) → OpenCode Message/Parts
- [x] Map roles: System, User, Assistant, Tool
- [x] Preserve message content and metadata
- [x] Handle tool calls and tool results
- [x] Handle multimodal content (text, code)
- [x] Test all message type combinations
- [x] Test round-trip conversion (no data loss)

## Acceptance Criteria

- Bidirectional conversion works
- All message roles supported
- Tool calls/results map correctly
- No data loss in round-trip
- >90% test coverage
