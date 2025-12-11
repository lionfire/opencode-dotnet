# LionFire.OpenCode.Serve.AgentFramework - Technical Specification

**Status**: Draft
**Version**: 1.2.0
**Author**: LionFire.OpenCode Team
**Date**: 2025-12-06
**Related PRP**: [PRP.md](../../000-PRP/10-spec/PRP.md)

## Overview

### Purpose

This specification defines the integration of LionFire.OpenCode with Microsoft Agent Framework, enabling LionFire.OpenCode clients to participate in Agent Framework workflows as first-class AI agents. The integration creates an `OpencodeAgent` class that extends the Agent Framework's `AIAgent` abstract base class, allowing LionFire.OpenCode to work seamlessly with Agent Framework's orchestration, middleware, and multi-agent patterns.

### Scope

**In scope:**
- `OpencodeAgent` class implementing `Microsoft.Agents.AI.AIAgent`
- `OpencodeAgentThread` class extending `AgentThread` for conversation state
- Bidirectional message conversion between Agent Framework and LionFire.OpenCode formats
- Thread serialization/deserialization for persistence
- Streaming support via `IAsyncEnumerable<AgentRunResponseUpdate>`
- Dependency injection extensions for registration
- Tool integration between LionFire.OpenCode tools and Agent Framework
- Middleware support for cross-cutting concerns

**Out of scope:**
- Agent Framework workflow engine modifications
- New LionFire.OpenCode API features
- Multi-agent orchestration implementation (uses existing Agent Framework capabilities)
- GUI or visualization components

### Audience

- **SDK Developers**: Implementing the integration layer
- **Library Consumers**: Using LionFire.OpenCode within Agent Framework workflows
- **Enterprise Teams**: Building multi-agent systems with LionFire.OpenCode participation
- **Integration Architects**: Designing agent-based applications

## Background

### Context

Microsoft Agent Framework provides a unified abstraction for AI agents across different providers (OpenAI, Anthropic, Azure AI, etc.). The framework defines standard interfaces for agent execution, conversation threading, streaming responses, and middleware pipelines. By implementing these interfaces, LionFire.OpenCode can participate in Agent Framework scenarios without requiring custom integration code.

The Agent Framework is built on `Microsoft.Extensions.AI` abstractions and follows modern .NET patterns including:
- Abstract base classes for extension (`AIAgent`, `AgentThread`)
- `IAsyncEnumerable<T>` for streaming
- JSON serialization for thread persistence
- Service provider integration for dependency injection
- Decorator pattern for middleware (`DelegatingAIAgent`)

### Problem Statement

OpenCode exposes a local headless server via `opencode serve` with a REST API for session management, prompts, and tool execution. Users who want to combine OpenCode with other AI providers in Microsoft Agent Framework multi-agent workflows must manually handle:
- Message format conversion between Agent Framework's `ChatMessage` and OpenCode's session/message API
- Session state synchronization with OpenCode's SQLite persistence
- Streaming coordination via Server-Sent Events (SSE)
- Tool invocation mapping between frameworks
- Process lifecycle management (ensuring `opencode serve` is running)

This integration eliminates these manual efforts by providing a native Agent Framework implementation that wraps the local OpenCode server.

### Goals and Objectives

1. **Native Agent Framework Participation**: Enable LionFire.OpenCode to work directly in Agent Framework workflows
2. **Full Feature Parity**: Support all LionFire.OpenCode capabilities through the Agent Framework interface
3. **Seamless Integration**: Minimal code changes for existing LionFire.OpenCode users to adopt Agent Framework
4. **Streaming Excellence**: Maintain LionFire.OpenCode's streaming performance through Agent Framework
5. **Tool Interoperability**: Map LionFire.OpenCode tools to Agent Framework tool format
6. **Middleware Compatibility**: Enable Agent Framework middleware for LionFire.OpenCode operations

### Success Criteria

- OpencodeAgent can participate in all Agent Framework workflow patterns (sequential, parallel, conditional)
- Message conversion maintains full fidelity (no data loss)
- Streaming performance within 5% of native LionFire.OpenCode performance
- Thread state can be persisted and restored across application restarts
- All existing LionFire.OpenCode telemetry functions through Agent Framework
- Unit test coverage exceeds 80%

## Requirements

### Functional Requirements

#### FR1: OpencodeAgent Implementation

**Priority**: Must Have

**Description**: Create `OpencodeAgent` class that extends `AIAgent` and wraps `IOpencodeClient`.

**Acceptance Criteria**:
- Implements all abstract methods: `GetNewThread()`, `DeserializeThread()`, `RunAsync()`, `RunStreamingAsync()`
- Supports both string and `ChatMessage` input overloads
- Returns `AgentRunResponse` with properly populated messages
- Exposes `Id`, `Name`, `Description`, and `DisplayName` properties
- Provides `GetService<T>()` for accessing wrapped services

#### FR2: Message Conversion

**Priority**: Must Have

**Description**: Bidirectional conversion between `Microsoft.Extensions.AI.ChatMessage` and OpenCode's session message format (multi-part messages with `TextPart`, `FilePart`, `AgentPart`).

**Acceptance Criteria**:
- Converts all message roles: System, User, Assistant, Tool
- Preserves message content including code blocks
- Maintains metadata and additional properties
- Handles tool calls and tool results
- Supports multimodal content (text, code)

#### FR3: Thread Management

**Priority**: Must Have

**Description**: Create `OpencodeAgentThread` class that maintains conversation state, mapping to OpenCode sessions.

**Acceptance Criteria**:
- Extends `AgentThread` base class
- Maps to/from OpenCode session internally (session ID, message history)
- Supports JSON serialization via `Serialize()`
- Supports deserialization via `DeserializeThread()`
- Maintains message history across multiple `RunAsync()` calls
- Provides access to underlying session ID for advanced scenarios
- Leverages OpenCode's built-in SQLite persistence for session state

#### FR4: Streaming Support

**Priority**: Must Have

**Description**: Implement streaming responses via `RunStreamingAsync()`.

**Acceptance Criteria**:
- Returns `IAsyncEnumerable<AgentRunResponseUpdate>`
- Maps LionFire.OpenCode streaming chunks to Agent Framework updates
- Supports cancellation via `CancellationToken`
- Provides real-time progress updates
- Updates thread state as streaming completes

#### FR5: Dependency Injection

**Priority**: Should Have

**Description**: Extension methods for registering OpencodeAgent with DI container.

**Acceptance Criteria**:
- `AddOpencodeAgent()` extension method on `IServiceCollection`
- `AddOpencodeAgentFactory()` for named agent registration
- Integration with existing `AddOpencodeClient()` registrations
- Support for configuration via `IOptions<OpencodeAgentOptions>`

#### FR6: Tool Integration

**Priority**: Should Have

**Description**: Map LionFire.OpenCode tools to Agent Framework tool format.

**Acceptance Criteria**:
- Convert `LionFire.OpenCode.Tools.Tool` to `Microsoft.Extensions.AI.AITool`
- Handle tool invocation requests in `RunAsync()`
- Return tool results in Agent Framework format
- Support async tool execution

#### FR7: Middleware Support

**Priority**: Should Have

**Description**: Enable Agent Framework middleware for LionFire.OpenCode operations.

**Acceptance Criteria**:
- OpencodeAgent works with `DelegatingAIAgent` pattern
- Supports telemetry middleware
- Supports logging middleware
- Supports approval/human-in-the-loop middleware
- Maintains compatibility with LionFire.OpenCode's existing telemetry

#### FR8: Enhanced Client Interfaces (Option 4)

**Priority**: Should Have

**Description**: Provide convenience extension methods on `IOpenCodeClient` for fluent Agent Framework integration without requiring explicit wrapper construction.

**Acceptance Criteria**:

**Extension Methods:**
- `AsAgent()` extension on `IOpenCodeClient`:
  ```csharp
  var agent = client.AsAgent(name: "CodeExpert", description: "Code generation specialist");
  ```
- `AsAgent(OpencodeAgentOptions)` overload for full configuration
- `WithMiddleware()` fluent extension for composing middleware:
  ```csharp
  var agent = client.AsAgent()
      .WithMiddleware(new TelemetryMiddleware())
      .WithMiddleware(new CostTrackingMiddleware());
  ```

**Session/Thread Conversion:**
- `session.GetAgentThread()` extension to wrap OpenCode session as AgentThread
- `thread.GetSessionId()` extension to retrieve underlying session ID
- Bidirectional conversion preserves message history

**Unified DI Registration:**
- `AddOpenCodeClientAsAgent()` registers both `IOpenCodeClient` and `AIAgent`:
  ```csharp
  services.AddOpenCodeClientAsAgent(
      clientOptions => { clientOptions.BaseUrl = "http://localhost:9123"; },
      agentOptions => { agentOptions.Name = "CodeExpert"; });

  // Both resolvable:
  var client = sp.GetRequiredService<IOpenCodeClient>();
  var agent = sp.GetRequiredService<AIAgent>();
  ```

**Middleware Examples:**
- `TelemetryMiddleware` - OpenTelemetry Activity tracking
- `CostTrackingMiddleware` - Token/cost tracking with budget limits
- `ApprovalMiddleware` - Human-in-the-loop approval for generated code
- `ContentFilterMiddleware` - Input/output content policy enforcement

**Reference**: See `/docs/agent-framework/integration-possibilities/04-enhanced-client-interfaces.md` for detailed design.

### Non-Functional Requirements

#### Performance

- **Message Conversion Overhead**: < 1ms per message conversion
- **Thread Serialization**: < 10ms for 100-message thread
- **Memory Usage**: < 10% overhead compared to native LionFire.OpenCode
- **Streaming Latency**: < 5ms additional latency per chunk

#### Scalability

- Support concurrent agent executions (limited by LionFire.OpenCode rate limits)
- Thread-safe operations for multi-threaded environments
- Efficient memory usage for long-running conversations

#### Security

- No exposure of API keys through Agent Framework interfaces
- Maintain LionFire.OpenCode's existing security model
- Secure thread serialization (no sensitive data in plain text)

#### Reliability

- Graceful handling of LionFire.OpenCode API errors
- Proper cleanup on cancellation
- Thread state consistency on partial failures

## Architecture and Design

### System Architecture

```
+--------------------------------------------------+
|           Application / Workflow Layer            |
|  (WorkflowHostAgent, Multi-Agent Orchestration)   |
+--------------------------------------------------+
                        |
                        v
+--------------------------------------------------+
|            Microsoft Agent Framework              |
|  AIAgent, AgentThread, AgentRunResponse          |
+--------------------------------------------------+
                        |
                        v
+--------------------------------------------------+
|          LionFire.OpenCode.AgentFramework               |
|                                                   |
|  +---------------+    +----------------------+   |
|  | OpencodeAgent |    | OpencodeAgentThread  |   |
|  +---------------+    +----------------------+   |
|         |                       |                |
|         v                       v                |
|  +---------------+    +----------------------+   |
|  |MessageConverter|   | ThreadStateManager   |   |
|  +---------------+    +----------------------+   |
|                                                   |
+--------------------------------------------------+
                        |
                        v
+--------------------------------------------------+
|                 LionFire.OpenCode                        |
|  IOpencodeClient, IConversation, ToolHandler    |
+--------------------------------------------------+
                        |
                        v
+--------------------------------------------------+
|              OpenCode REST API                    |
+--------------------------------------------------+
```

### Components

#### 1. OpencodeAgent

**Purpose**: Main agent implementation that bridges Agent Framework and LionFire.OpenCode.

**Responsibilities**:
- Implement `AIAgent` abstract methods
- Manage agent identity (Id, Name, Description)
- Coordinate message conversion and execution
- Handle streaming responses
- Provide service resolution via `GetService<T>()`

**Interfaces**:
- Extends: `Microsoft.Agents.AI.AIAgent`
- Uses: `IOpencodeClient`, `MessageConverter`, `OpencodeAgentThread`

#### 2. OpencodeAgentThread

**Purpose**: Maintain conversation state compatible with Agent Framework.

**Responsibilities**:
- Store conversation history
- Manage underlying `IConversation`
- Support serialization/deserialization
- Provide message access

**Interfaces**:
- Extends: `Microsoft.Agents.AI.AgentThread`
- Wraps: `LionFire.OpenCode.Conversations.IConversation`

#### 3. MessageConverter

**Purpose**: Bidirectional message format conversion.

**Responsibilities**:
- Convert `ChatMessage` to `ConversationMessage`
- Convert `ConversationMessage` to `ChatMessage`
- Handle role mapping
- Preserve content and metadata

**Interfaces**:
- Static utility class with conversion methods

#### 4. OpencodeAgentOptions

**Purpose**: Configuration for OpencodeAgent instances.

**Responsibilities**:
- Agent identity configuration (Name, Description)
- Default model settings
- Tool configuration
- Telemetry options

#### 5. ServiceCollectionExtensions

**Purpose**: DI registration helpers.

**Responsibilities**:
- Register OpencodeAgent as `AIAgent`
- Support named agents
- Configure options via `IOptions<T>`

#### 6. OpencodeAgentBuilder

**Purpose**: Fluent configuration API for building agents.

**Responsibilities**:
- Configure agent options
- Add middleware
- Set up tools
- Build final agent instance

### Data Model

#### OpencodeAgentThreadState

```csharp
internal sealed class OpencodeAgentThreadState
{
    // Unique identifier for the thread
    public string? ThreadId { get; set; }

    // Underlying conversation ID from LionFire.OpenCode
    public string? ConversationId { get; set; }

    // Serialized conversation options
    public ConversationOptions? Options { get; set; }

    // Serialized message history
    public List<SerializedMessage>? Messages { get; set; }

    // Additional metadata
    public Dictionary<string, object>? Metadata { get; set; }
}

internal sealed class SerializedMessage
{
    public string? Id { get; set; }
    public MessageRole Role { get; set; }
    public string? Content { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public int? TokenCount { get; set; }
    public List<SerializedToolCall>? ToolCalls { get; set; }
    public string? ToolCallId { get; set; }
}
```

### API Specifications

See [api/OpencodeAgent.md](api/OpencodeAgent.md) for detailed API documentation.

#### Core Types

```csharp
namespace LionFire.OpenCode.AgentFramework;

/// <summary>
/// An AI agent implementation that wraps IOpencodeClient for use with
/// Microsoft Agent Framework.
/// </summary>
public sealed class OpencodeAgent : AIAgent
{
    // Constructors
    public OpencodeAgent(IOpencodeClient client);
    public OpencodeAgent(IOpencodeClient client, OpencodeAgentOptions options);
    public OpencodeAgent(IOpencodeClient client, string? name, string? description);

    // Properties
    public override string Id { get; }
    public override string? Name { get; }
    public override string? Description { get; }
    public IOpencodeClient Client { get; }

    // Abstract method implementations
    public override AgentThread GetNewThread();
    public override AgentThread DeserializeThread(
        JsonElement serializedThread,
        JsonSerializerOptions? jsonSerializerOptions = null);
    public override Task<AgentRunResponse> RunAsync(
        IEnumerable<ChatMessage> messages,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default);
    public override IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(
        IEnumerable<ChatMessage> messages,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default);
}

/// <summary>
/// Thread implementation for OpencodeAgent that wraps IConversation.
/// </summary>
public sealed class OpencodeAgentThread : AgentThread
{
    // Properties
    public string ThreadId { get; }
    public IConversation? Conversation { get; }
    public IReadOnlyList<ChatMessage> Messages { get; }

    // Methods
    public override JsonElement Serialize(JsonSerializerOptions? jsonSerializerOptions = null);
}

/// <summary>
/// Configuration options for OpencodeAgent.
/// </summary>
public sealed class OpencodeAgentOptions
{
    public string? Id { get; set; }
    public string? Name { get; set; }
    public string? Description { get; set; }
    public string? DefaultModel { get; set; }
    public string? SystemPrompt { get; set; }
    public IList<AITool>? Tools { get; set; }
    public bool EnableTelemetry { get; set; } = true;
}
```

### Integration Points

#### 1. LionFire.OpenCode Client

- Uses `IOpencodeClient` for all API operations
- Creates `IConversation` for multi-turn interactions
- Leverages existing streaming infrastructure

#### 2. Agent Framework

- Extends `AIAgent` abstract class
- Implements required abstract methods
- Compatible with `DelegatingAIAgent` middleware
- Works with `AIAgentBuilder` pipeline

#### 3. Dependency Injection

- Integrates with `Microsoft.Extensions.DependencyInjection`
- Uses `IOptions<T>` pattern for configuration
- Supports keyed services for named agents

## Implementation Details

### Technology Stack

- **Target Framework**: .NET 8.0
- **Primary Dependencies**:
  - `Microsoft.Agents.AI.Abstractions` (Agent Framework)
  - `LionFire.OpenCode` (existing LionFire.OpenCode client)
  - `Microsoft.Extensions.AI` (common AI abstractions)
- **Optional Dependencies**:
  - `Microsoft.Extensions.DependencyInjection.Abstractions`
  - `Microsoft.Extensions.Options`

### Key Algorithms

#### Message Conversion Algorithm

```
FUNCTION ConvertToConversationMessage(chatMessage: ChatMessage) -> ConversationMessage
    role = MapChatRole(chatMessage.Role)
    content = ExtractTextContent(chatMessage.Contents)

    message = new ConversationMessage(role, content)
    message.Metadata = ConvertAdditionalProperties(chatMessage.AdditionalProperties)

    IF chatMessage.Contents contains FunctionCallContent THEN
        message.ToolCalls = ConvertToolCalls(chatMessage.Contents)
    END IF

    IF chatMessage.Contents contains FunctionResultContent THEN
        message.ToolCallId = ExtractToolCallId(chatMessage.Contents)
    END IF

    RETURN message
END FUNCTION

FUNCTION MapChatRole(chatRole: ChatRole) -> MessageRole
    SWITCH chatRole
        CASE ChatRole.System: RETURN MessageRole.System
        CASE ChatRole.User: RETURN MessageRole.User
        CASE ChatRole.Assistant: RETURN MessageRole.Assistant
        CASE ChatRole.Tool: RETURN MessageRole.Tool
        DEFAULT: THROW InvalidOperationException
    END SWITCH
END FUNCTION
```

#### Streaming Response Algorithm

```
ASYNC FUNCTION RunStreamingAsync(messages, thread, options, cancellationToken)
    thread = EnsureThread(thread)

    // Convert input messages
    FOR EACH message IN messages
        conversationMessage = MessageConverter.ToConversationMessage(message)
        thread.Conversation.AddMessage(conversationMessage)
    END FOR

    // Get last user message for streaming
    userMessage = GetLastUserMessage(messages)

    // Stream from LionFire.OpenCode
    ASYNC FOR chunk IN thread.Conversation.StreamMessageAsync(userMessage.Text, cancellationToken)
        update = new AgentRunResponseUpdate
        update.AgentId = this.Id
        update.Role = ChatRole.Assistant
        update.Contents.Add(new TextContent(chunk.Text))
        update.AuthorName = this.Name

        YIELD RETURN update
    END FOR
END FUNCTION
```

### Data Flow

```
User Request
     |
     v
+------------------+
| RunAsync/        |
| RunStreamingAsync|
+------------------+
     |
     v
+------------------+
| Message          |
| Conversion       |
| (ChatMessage ->  |
| ConversationMsg) |
+------------------+
     |
     v
+------------------+
| Thread/          |
| Conversation     |
| Management       |
+------------------+
     |
     v
+------------------+
| IOpencodeClient  |
| Execution        |
+------------------+
     |
     v
+------------------+
| Response         |
| Conversion       |
| (ConversationMsg |
| -> ChatMessage)  |
+------------------+
     |
     v
+------------------+
| AgentRunResponse/|
| ResponseUpdate   |
+------------------+
     |
     v
User Response
```

### Error Handling

**Error Types**:
- `OpencodeConnectionException`: Server not running or unreachable
- `OpencodeApiException`: API errors from OpenCode server
- `OpencodeSessionNotFoundException`: Session no longer exists
- `InvalidOperationException`: Invalid state or configuration
- `ArgumentException`: Invalid input parameters

Note: Unlike cloud API SDKs, there are no authentication or rate limit exceptions since this is a local server.

**Logging Strategy**:
- Log all API calls at Debug level
- Log errors at Error level with full exception details
- Log message conversion at Trace level
- Use structured logging with correlation IDs

**User-Facing Errors**:
- Wrap LionFire.OpenCode exceptions in Agent Framework response format
- Provide clear error messages with actionable guidance
- Include request ID for support correlation

### Configuration

**Configuration Parameters**:

| Parameter | Type | Description | Default |
|-----------|------|-------------|---------|
| `Name` | `string?` | Agent display name | `"LionFire.OpenCode"` |
| `Description` | `string?` | Agent description | `null` |
| `DefaultModel` | `string?` | Default model for requests | From client options |
| `SystemPrompt` | `string?` | Default system instructions | `null` |
| `EnableTelemetry` | `bool` | Enable telemetry | `true` |
| `StreamingBufferSize` | `int` | Streaming buffer size | `1024` |

**Configuration Sources**:
- Constructor parameters
- `OpencodeAgentOptions` object
- `IOptions<OpencodeAgentOptions>` via DI
- Fluent builder API

## Testing Strategy

### Unit Testing

**Coverage Target**: >80%

**Key Test Cases**:
1. Message conversion for all role types
2. Thread creation and serialization
3. Response mapping from LionFire.OpenCode to Agent Framework
4. Error handling and exception translation
5. Options validation and merging
6. Tool conversion and invocation

**Mocking Strategy**:
- Mock `IOpencodeClient` for isolated unit tests
- Use test doubles for `IConversation`
- Create message fixtures for conversion tests

### Integration Testing

**Test Scenarios**:
1. End-to-end agent execution with mock API
2. Thread persistence and restoration
3. Streaming with real LionFire.OpenCode client (integration environment)
4. Multi-turn conversations
5. Tool use scenarios

**Test Environment**:
- Integration tests use mock HTTP responses
- E2E tests run against OpenCode staging environment
- CI/CD pipeline runs integration tests

### End-to-End Testing

**User Flows to Test**:
1. Simple prompt -> response flow
2. Multi-turn conversation with context
3. Streaming response consumption
4. Thread serialization and resumption
5. Error recovery scenarios

**Test Data**:
- Predefined prompt/response pairs
- Edge case inputs (empty, very long, special characters)
- Various code language samples

### Performance Testing

**Load Testing**:
- 10 concurrent agents with 100 requests each
- Sustained throughput measurement
- Memory usage under load

**Stress Testing**:
- Maximum concurrent connections
- Recovery after API throttling
- Large message handling

**Benchmarks**:
- Message conversion: <1ms for typical messages
- Serialization: <10ms for 100-message thread
- First token latency: <50ms overhead

## Deployment and Operations

### Deployment Strategy

**Deployment Method**: NuGet package

**Package Structure**:
```
LionFire.OpenCode.AgentFramework/
  - LionFire.OpenCode.AgentFramework.nupkg
    - lib/net8.0/
      - LionFire.OpenCode.AgentFramework.dll
      - LionFire.OpenCode.AgentFramework.xml (XML docs)
    - README.md
    - LICENSE
```

**Rollout Plan**:
1. Alpha release for internal testing
2. Beta release with selected partners
3. Public preview with feedback collection
4. GA release with full documentation

**Rollback Plan**:
- NuGet packages are immutable; publish patch version
- Maintain backward compatibility within major version
- Deprecation notices for breaking changes

### Monitoring and Observability

**Metrics to Track**:
- `opencode.agent.requests.total`: Total agent run requests
- `opencode.agent.requests.duration`: Request duration histogram
- `opencode.agent.streaming.chunks`: Streaming chunks delivered
- `opencode.agent.errors.total`: Error count by type

**Logging**:
- Structured JSON logging via `ILogger`
- Correlation ID propagation
- Request/response logging at Debug level

**Alerting**:
- High error rate alerts (>1% of requests)
- Latency degradation alerts (>500ms P95)
- Rate limit approaching alerts

**Dashboards**:
- Agent usage dashboard
- Error rate and latency trends
- Streaming performance metrics

### Operational Considerations

**Maintenance Windows**: N/A (client library)

**Backup and Recovery**: Thread state serialization enables state backup

**Disaster Recovery**: N/A (stateless client library)

## Dependencies

### Internal Dependencies

- **LionFire.OpenCode** (>= 1.0.0): Core client library
  - Required for `IOpencodeClient`, `IConversation`, models

### External Dependencies

- **Microsoft.Agents.AI.Abstractions** (>= 0.1.0): Agent Framework base types
  - Required for `AIAgent`, `AgentThread`, response types
- **Microsoft.Extensions.AI** (>= 9.0.0): Common AI abstractions
  - Required for `ChatMessage`, `ChatRole`, content types
- **Microsoft.Extensions.DependencyInjection.Abstractions** (>= 8.0.0): DI abstractions
  - Optional for DI extension methods
- **Microsoft.Extensions.Options** (>= 8.0.0): Options pattern
  - Optional for configuration

### Breaking Changes

This is a new package with no backward compatibility concerns for initial release.

## Security Considerations

### Authentication and Authorization

- No API keys required (local server model)
- Authentication is implicit via localhost access
- Server runs on user's machine with user's credentials
- No sensitive credentials to manage or protect

### Data Protection

- Thread serialization does not include sensitive data
- Message content treated as potentially sensitive
- No PII logging by default
- TLS required for API communication (inherited from client)

### Vulnerabilities and Mitigations

| Vulnerability | Mitigation |
|--------------|------------|
| Unauthorized local access | Bind to localhost only (OpenCode default behavior) |
| Thread injection | Validate serialized thread format |
| Message tampering | Input validation on deserialization |
| Memory exhaustion | Bounded message history |
| Server not running | Health check and clear error messages |

### Compliance

- GDPR: No personal data stored; conversation history is user-controlled
- SOC2: Follows secure development practices

## Performance Considerations

### Performance Requirements

- Message conversion: <1ms per message
- Thread serialization: <10ms for 100 messages
- Streaming overhead: <5ms per chunk
- Memory: <10% overhead vs native LionFire.OpenCode

### Optimization Strategies

1. **Object Pooling**: Pool message converters and builders
2. **Lazy Loading**: Delay conversation creation until first message
3. **Streaming Buffering**: Optimize buffer sizes for throughput
4. **JSON Optimization**: Use source-generated JSON serialization

### Bottlenecks and Solutions

| Bottleneck | Solution |
|-----------|----------|
| Message conversion allocations | Use spans and pooling |
| Thread serialization | Streaming JSON serialization |
| Large message handling | Chunked processing |

## Migration and Compatibility

### Migration Strategy

For existing LionFire.OpenCode client users:

```csharp
// Before: Direct client usage with local server
var client = new OpencodeClient("http://localhost:9123");
var session = await client.CreateSessionAsync();
await client.SendMessageAsync(session.Id, new TextPart { Text = "Hello" });

// After: Agent Framework integration
var client = new OpencodeClient("http://localhost:9123");
var agent = new OpencodeAgent(client);
var thread = agent.GetNewThread();
await agent.RunAsync("Hello", thread);
```

### Backward Compatibility

- Existing `IOpencodeClient` usage unchanged
- Agent Framework integration is additive
- No breaking changes to core LionFire.OpenCode package

### Deprecation Plan

N/A for initial release.

## Open Questions and Decisions

### Open Questions

1. Should `OpencodeAgentThread` expose direct access to `IConversation`?
   - Pro: Enables advanced scenarios
   - Con: Breaks abstraction, potential misuse

2. How to handle tool schema differences between frameworks?
   - Option A: Strict conversion with validation
   - Option B: Best-effort mapping with fallbacks

### Decisions to Be Made

1. **Thread Storage Strategy**:
   - Option A: In-memory only with serialization for persistence
   - Option B: Pluggable storage providers (SQL, Redis, etc.)
   - **Recommendation**: Option A for v1.0, Option B for future

2. **Middleware Integration**:
   - Option A: Full DelegatingAIAgent support
   - Option B: Limited middleware with explicit hooks
   - **Recommendation**: Option A for consistency with Agent Framework

### Assumptions

- Microsoft Agent Framework APIs are stable for the duration of development
- LionFire.OpenCode API backward compatibility is maintained
- Users have existing familiarity with either LionFire.OpenCode or Agent Framework

## Risks and Mitigations

### Technical Risks

| Risk | Impact | Mitigation |
|------|--------|------------|
| Agent Framework API changes | Breaking changes | Pin to specific version, abstract interfaces |
| Performance regression | User experience | Continuous benchmarking, optimization sprints |
| Complex tool mapping | Feature gaps | Phased rollout, clear documentation |

### Schedule Risks

| Risk | Impact | Mitigation |
|------|--------|------------|
| Dependency delays | Delayed release | Parallel development tracks |
| Testing complexity | Extended timeline | Early test automation |

### Resource Risks

| Risk | Impact | Mitigation |
|------|--------|------------|
| Knowledge gaps | Quality issues | Training, pair programming |

## Timeline and Phases

### Phase 1: Core Implementation (2 weeks)

**Duration**: 10 business days

**Deliverables**:
- `OpencodeAgent` class with basic `RunAsync()`
- `OpencodeAgentThread` with in-memory storage
- `MessageConverter` for all message types
- Unit tests for core functionality

### Phase 2: Streaming and Tools (1 week)

**Duration**: 5 business days

**Deliverables**:
- `RunStreamingAsync()` implementation
- Tool conversion and invocation
- Thread serialization/deserialization
- Integration tests

### Phase 3: DI and Middleware (1 week)

**Duration**: 5 business days

**Deliverables**:
- `ServiceCollectionExtensions` for DI
- `OpencodeAgentBuilder` fluent API
- Middleware compatibility testing
- Documentation

### Phase 4: Polish and Release (1 week)

**Duration**: 5 business days

**Deliverables**:
- Performance optimization
- Documentation finalization
- Example projects
- NuGet package publication

## Success Metrics

### Key Performance Indicators (KPIs)

- **Adoption**: 100+ downloads in first month
- **Quality**: Zero critical bugs in first 30 days
- **Performance**: All benchmarks met
- **Coverage**: >80% test coverage

### Validation Criteria

- All Agent Framework workflow patterns work with OpencodeAgent
- Thread state survives serialization round-trip
- Streaming delivers chunks with <5ms overhead
- Middleware pipeline executes correctly
- Multi-agent scenarios function properly

## References

### Related Documents

- [PRP.md](../../PRP.md) - Product Requirements
- [02-opencode-as-tools.md](../../../docs/agent-framework/integration-possibilities/02-opencode-as-tools.md) - Integration Option 2
- [03-hybrid-workflow-architecture.md](../../../docs/agent-framework/integration-possibilities/03-hybrid-workflow-architecture.md) - Integration Option 3
- [04-enhanced-client-interfaces.md](../../../docs/agent-framework/integration-possibilities/04-enhanced-client-interfaces.md) - Integration Option 4

### External References

- Microsoft Agent Framework: https://learn.microsoft.com/agent-framework/
- Microsoft Agent Framework GitHub: https://github.com/microsoft/agent-framework
- Microsoft.Extensions.AI: https://www.nuget.org/packages/Microsoft.Extensions.AI

### Standards and Best Practices

- .NET Library Design Guidelines: https://docs.microsoft.com/dotnet/standard/design-guidelines/
- Microsoft.Extensions.AI patterns: https://devblogs.microsoft.com/dotnet/introducing-microsoft-extensions-ai/

## Appendices

### Appendix A: Message Role Mapping

| Agent Framework Role | LionFire.OpenCode Role | Notes |
|---------------------|-----------------|-------|
| `ChatRole.System` | `MessageRole.System` | Direct mapping |
| `ChatRole.User` | `MessageRole.User` | Direct mapping |
| `ChatRole.Assistant` | `MessageRole.Assistant` | Direct mapping |
| `ChatRole.Tool` | `MessageRole.Tool` | Tool result messages |

### Appendix B: Example Usage

```csharp
// Basic usage - connect to local OpenCode server
var client = new OpenCodeClient("http://localhost:9123");
var agent = new OpencodeAgent(client, name: "CodeAssistant");

var thread = agent.GetNewThread();
var response = await agent.RunAsync("Write a function to validate email addresses", thread);
Console.WriteLine(response.Text);

// Streaming usage
await foreach (var update in agent.RunStreamingAsync("Explain this code", thread))
{
    Console.Write(update.Text);
}

// With DI (no API key needed - just base URL)
services.AddOpenCodeClient(options => options.BaseUrl = "http://localhost:9123");
services.AddOpencodeAgent(options => options.Name = "CodeAssistant");
```

### Appendix B2: Option 4 - Enhanced Client Interface Examples

```csharp
// Using AsAgent() extension method (Option 4)
var client = new OpenCodeClient("http://localhost:9123");
var agent = client.AsAgent(name: "CodeExpert");

// Fluent middleware composition
var agentWithMiddleware = client.AsAgent()
    .WithMiddleware(new TelemetryMiddleware())
    .WithMiddleware(new CostTrackingMiddleware(maxBudget: 100m))
    .WithMiddleware(new ApprovalMiddleware());

// Unified DI registration (Option 4)
services.AddOpenCodeClientAsAgent(
    clientOptions => { clientOptions.BaseUrl = "http://localhost:9123"; },
    agentOptions => {
        agentOptions.Name = "CodeExpert";
        agentOptions.Description = "Specialized code generation agent";
    });

// Both interfaces resolvable from same registration
var client = sp.GetRequiredService<IOpenCodeClient>();  // Direct client access
var agent = sp.GetRequiredService<AIAgent>();           // Agent Framework access

// Session/Thread conversion helpers
var session = await client.GetSessionAsync(sessionId);
var thread = session.GetAgentThread(client);  // Convert to AgentThread

var sessionId = thread.GetSessionId();  // Get underlying session ID
```

### Appendix C: Thread Serialization Format

```json
{
  "threadId": "abc123",
  "sessionId": "sess_xyz",
  "baseUrl": "http://localhost:9123",
  "messages": [
    {
      "id": "msg_001",
      "role": "User",
      "content": "Write a hello world in C#",
      "createdAt": "2025-12-06T10:00:00Z"
    },
    {
      "id": "msg_002",
      "role": "Assistant",
      "content": "```csharp\nConsole.WriteLine(\"Hello, World!\");\n```",
      "createdAt": "2025-12-06T10:00:01Z"
    }
  ]
}
```

Note: OpenCode sessions persist in SQLite on the server side. The thread serialization captures the session reference for reconnection.

## Revision History

| Version | Date | Author | Changes |
|---------|------|--------|---------|
| 1.0.0 | 2025-12-06 | LionFire.OpenCode Team | Initial draft |
| 1.1.0 | 2025-12-08 | LionFire.OpenCode Team | Updated for local headless server model (removed API key references, updated session-based architecture, added process lifecycle considerations) |
| 1.2.0 | 2025-12-09 | LionFire.OpenCode Team | Added FR8: Enhanced Client Interfaces (Option 4) - extension methods (AsAgent, WithMiddleware), session/thread conversion helpers, unified DI registration, middleware examples |
| 1.3.0 | 2025-12-09 | LionFire.OpenCode Team | Renamed namespace from OpencodeAI to LionFire.OpenCode |
