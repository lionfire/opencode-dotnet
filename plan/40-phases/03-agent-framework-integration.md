# Phase 03: Agent Framework Integration

## Motivation

Enable OpenCode to participate in Microsoft Agent Framework workflows as a first-class AI agent, allowing multi-agent orchestration and integration with other AI providers in unified workflows.

## Goals

- Implement OpencodeAgent extending Agent Framework's AIAgent
- Thread management with serialization/deserialization
- Bidirectional message conversion (Agent Framework ↔ OpenCode)
- Streaming support through Agent Framework
- Tool integration between frameworks
- DI extensions and builder patterns
- Middleware compatibility

## Scope

Based on existing spec: `/src/opencode-dotnet/plan/10-specs/01-agent-framework-integration/spec.md`

**Included**:
- OpencodeAgent and OpencodeAgentThread classes
- Message converter (ChatMessage ↔ ConversationMessage)
- Thread state serialization
- Streaming via IAsyncEnumerable<AgentRunResponseUpdate>
- Tool format mapping
- DI extensions (AddOpencodeAgent)
- OpencodeAgentBuilder fluent API
- Middleware support (DelegatingAIAgent)
- Multi-agent examples

**Deferred**: Advanced workflow patterns (Phase 4 examples)

## Target Duration

2-3 weeks

## Epics

1. [Epic 03-001: OpencodeAgent Core Implementation](../50-epics/03/epic-03-001-opcodeagent-core.md)
2. [Epic 03-002: Message Conversion Layer](../50-epics/03/epic-03-002-message-conversion.md)
3. [Epic 03-003: Thread Management and Serialization](../50-epics/03/epic-03-003-thread-management.md)
4. [Epic 03-004: Streaming Integration](../50-epics/03/epic-03-004-streaming-integration.md)
5. [Epic 03-005: DI Extensions and Builder Pattern](../50-epics/03/epic-03-005-di-extensions-builder.md)
6. [Epic 03-006: Testing and Examples](../50-epics/03/epic-03-006-testing-examples.md)

## Success Criteria

- OpencodeAgent works in all Agent Framework workflow patterns
- Message conversion maintains full fidelity
- Streaming performance within 5% of native OpenCode
- Thread state survives serialization round-trip
- Middleware pipeline executes correctly
- Multi-agent scenarios functional
- >80% test coverage

## Dependencies

Phase 1 complete (uses core SDK)
Existing Agent Framework Integration Spec

## Risks

- Agent Framework API instability
- Complex message conversion edge cases
- Performance regression in abstraction layer
