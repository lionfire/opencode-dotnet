---
greenlit: true
---

# Epic 03-003: Thread Management and Serialization

**Phase**: 03 - Agent Framework Integration
**Estimated Effort**: 4-5 days

## Overview

Implement OpencodeAgentThread extending AgentThread with session mapping, serialization support, and bidirectional conversion helpers between Agent Framework threads and OpenCode sessions.

## Tasks

### Core Thread Implementation
- [ ] Create OpencodeAgentThread extending AgentThread
- [ ] Map to OpenCode session internally (SessionId, not IConversation)
- [ ] ThreadId property (string)
- [ ] SessionId property (string?) - underlying OpenCode session ID
- [ ] Messages property (IReadOnlyList<ChatMessage>)
- [ ] Implement Serialize() → JsonElement
- [ ] Create OpencodeAgentThreadState for serialization
- [ ] Test serialization/deserialization round-trip
- [ ] Handle session restoration from serialized state

### Session/Thread Conversion Helpers (Option 4)
- [ ] Create extension method: `session.GetAgentThread()` → AgentThread
  ```csharp
  public static AgentThread GetAgentThread(this Session session, IOpenCodeClient client)
  ```
- [ ] Create extension method: `thread.ToSession(client)` → Session info
  ```csharp
  public static string? GetSessionId(this AgentThread thread)
  ```
- [ ] Handle case where AgentThread is not OpencodeAgentThread (create new session)
- [ ] Bidirectional message history synchronization
- [ ] Test conversion helpers with various thread states

## Acceptance Criteria

- OpencodeAgentThread extends AgentThread
- Maps to OpenCode session via SessionId
- Serialization works correctly
- Deserialization restores state
- Thread survives round-trip
- `session.GetAgentThread()` creates valid AgentThread
- `thread.GetSessionId()` returns correct session ID
- Conversion helpers handle edge cases gracefully
