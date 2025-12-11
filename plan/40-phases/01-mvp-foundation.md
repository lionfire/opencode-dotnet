# Phase 01: MVP Foundation

## Motivation

Deliver core SDK functionality that enables developers to create sessions, send messages, and receive responses from OpenCode server. This phase establishes the foundation for all future enhancements and allows early adopters to start building applications.

The MVP must achieve the "30 seconds to first request" goal, providing excellent developer experience with strong typing, clear error messages, and intuitive APIs.

## Goals and Objectives

- Implement complete OpenCode REST API coverage with typed methods
- Enable session-based conversations with streaming support
- Provide health check and server detection
- Include robust error handling with helpful messages
- Support essential features: session auto-cleanup, structured logging
- Create working example demonstrating core functionality
- Achieve >70% code coverage in tests

## Scope

**Included in this phase**:
- Core client infrastructure (OpenCodeClient, IOpencodeClient)
- Session management API (create, get, list, delete, fork, abort, share)
- Message/prompt API with multi-part messages (TextPart, FilePart, AgentPart)
- Streaming API using IAsyncEnumerable<MessageUpdate>
- Tool management API (list, get, approve, update permissions)
- File operations API (list, get content, search, apply changes)
- Command API (slash command execution)
- Configuration API (config, providers, models)
- Health check and server detection
- Exception hierarchy and error handling
- Session scope with IAsyncDisposable auto-cleanup
- Basic retry logic with exponential backoff
- ILogger integration
- XML documentation
- Unit tests and integration tests
- Basic example project

**Deferred to later phases**:
- Source-generated JSON (Phase 2)
- IHttpClientFactory integration (Phase 2)
- Polly resilience policies (Phase 2)
- OpenTelemetry (Phase 2)
- Agent Framework integration (Phase 3)
- Comprehensive examples and documentation (Phase 4)

## Target Duration

2-3 weeks

## Epics in This Phase

1. [Epic 01-001: Core Client Infrastructure](../50-epics/01/epic-01-001-core-client-infrastructure.md)
2. [Epic 01-002: Session Management API](../50-epics/01/epic-01-002-session-management-api.md)
3. [Epic 01-003: Message and Streaming API](../50-epics/01/epic-01-003-message-streaming-api.md)
4. [Epic 01-004: Tool and File Operations API](../50-epics/01/epic-01-004-tool-file-operations-api.md)
5. [Epic 01-005: Error Handling and Logging](../50-epics/01/epic-01-005-error-handling-logging.md)
6. [Epic 01-006: Testing and Examples](../50-epics/01/epic-01-006-testing-examples.md)

## Rationale for Included Epics

### Epic 01-001: Core Client Infrastructure
Foundation for all other functionality. Establishes HTTP client, configuration, DTOs, and interfaces. Must be completed first as all other epics depend on it.

### Epic 01-002: Session Management API
Session management is core to OpenCode's API model. All interactions happen within sessions, so this is essential for MVP functionality.

### Epic 01-003: Message and Streaming API
The primary purpose of the SDK - sending messages and receiving responses. Streaming is critical for good UX with AI responses.

### Epic 01-004: Tool and File Operations API
Completes the core API coverage. Tools and file operations are frequently used features that should be available in MVP.

### Epic 01-005: Error Handling and Logging
Production-quality error handling is essential for good developer experience. Clear error messages save debugging time and improve adoption.

### Epic 01-006: Testing and Examples
Ensures quality and provides learning materials for early adopters. Cannot consider phase complete without tests and working examples.

## Dependencies

**Prerequisites**: None (foundational phase)

**Blocks**: All subsequent phases depend on Phase 1 completion

## Risks and Mitigations

- **Risk**: SSE streaming implementation complexity
  - **Impact**: High - Streaming is core to good UX
  - **Mitigation**: Research HttpClient SSE patterns early, study existing implementations (Docker.DotNet), test extensively with real server

- **Risk**: Multi-part message handling complexity
  - **Impact**: Medium - API uses multi-part messages extensively
  - **Mitigation**: Study OpenCode API spec carefully, test all message type combinations, create fixtures for testing

- **Risk**: Error message quality insufficient
  - **Impact**: Medium - Poor errors hurt developer experience
  - **Mitigation**: Test common failure scenarios, gather feedback from early users, refine messages iteratively

- **Risk**: Scope creep delaying MVP
  - **Impact**: High - Delays all subsequent phases
  - **Mitigation**: Strict scope discipline, defer nice-to-have features to Phase 2/3

## Success Criteria

- ✓ All OpenCode REST API endpoints have typed method implementations
- ✓ Streaming responses work correctly with SSE
- ✓ Health check accurately detects server availability
- ✓ Session auto-cleanup (IAsyncDisposable) works reliably
- ✓ Error messages include troubleshooting hints
- ✓ 30 seconds from NuGet install to first request achieved
- ✓ >70% code coverage in unit tests
- ✓ Integration tests pass against real OpenCode server
- ✓ Example project builds and runs successfully
- ✓ XML documentation complete for all public APIs
- ✓ At least 3 early adopters successfully build applications
