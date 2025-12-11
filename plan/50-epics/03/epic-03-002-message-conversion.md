---
greenlit: true
---

# Epic 03-002: Message Conversion Layer

**Phase**: 03 - Agent Framework Integration
**Estimated Effort**: 3-4 days

## Overview

Implement bidirectional message conversion between Agent Framework ChatMessage and OpenCode message formats.

## Tasks

- [ ] Create MessageConverter static utility class
- [ ] ToChatMessage(Message) → ChatMessage
- [ ] ToConversationMessage(ChatMessage) → OpenCode Message/Parts
- [ ] Map roles: System, User, Assistant, Tool
- [ ] Preserve message content and metadata
- [ ] Handle tool calls and tool results
- [ ] Handle multimodal content (text, code)
- [ ] Test all message type combinations
- [ ] Test round-trip conversion (no data loss)

## Acceptance Criteria

- Bidirectional conversion works
- All message roles supported
- Tool calls/results map correctly
- No data loss in round-trip
- >90% test coverage
