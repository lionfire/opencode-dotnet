# Microsoft Agent Framework

## Brief Description

Microsoft Agent Framework is a unified abstraction for building multi-agent AI systems that work across different AI providers (OpenAI, Anthropic, Azure AI, etc.). It provides standard interfaces for agent execution, conversation threading, streaming, and orchestration.

The framework is built on Microsoft.Extensions.AI abstractions and enables developers to create complex workflows combining multiple AI agents.

## Relevance to Project

Phase 3 of opencode-dotnet implements Agent Framework integration, allowing OpenCode to participate in multi-agent workflows.

**Why this matters for our project**:
- Enables OpenCode to work alongside other AI agents (OpenAI, Claude, etc.)
- Provides standardized interfaces for enterprise integration
- Allows use of Agent Framework's orchestration, middleware, and telemetry
- Opens up multi-agent use cases

**Where it's used in our architecture**:
- OpencodeAgent class extends AIAgent
- OpencodeAgentThread extends AgentThread
- Message conversion between ChatMessage and OpenCode formats
- Streaming via IAsyncEnumerable<AgentRunResponseUpdate>

**Impact on implementation**:
- Requires additional NuGet packages (Microsoft.Agents.AI.Abstractions)
- Need bidirectional message format conversion
- Thread serialization for persistence
- Performance overhead from abstraction layer (target: <5%)

## Interoperability Points

**Integrates with**:
- IOpencodeClient: OpencodeAgent wraps our client internally
- Other AI agents: Can be combined in workflows (sequential, parallel, conditional)
- Middleware: Telemetry, logging, approval, rate limiting
- Workflow orchestration: WorkflowHostAgent, conditional routing

**Data flow**:
1. App calls OpencodeAgent.RunAsync(messages, thread, options)
2. OpencodeAgent converts ChatMessage[] to OpenCode format
3. Calls underlying IOpencodeClient
4. Converts response back to ChatMessage format
5. Returns AgentRunResponse to framework

## Considerations and Watch Points

### Best Practices

**For this project specifically**:
- Keep OpencodeAgent as thin wrapper (delegate to IOpencodeClient)
- Maintain full fidelity in message conversion (no data loss)
- Leverage OpenCode's SQLite persistence for thread state
- Expose session ID for advanced scenarios

**Common patterns**:
- Decorator pattern for middleware (DelegatingAIAgent)
- Factory pattern for agent creation (IAgentBuilder)
- Options pattern for configuration (OpencodeAgentOptions)

### Common Pitfalls

- **Message conversion data loss**: Ensure round-trip conversion preserves all data
- **Thread state divergence**: Keep AgentThread in sync with OpenCode session
- **Performance overhead**: Conversion adds latency; minimize allocations

## References

- Agent Framework Docs: https://learn.microsoft.com/agent-framework/
- Existing Spec: [/src/opencode-dotnet/plan/10-specs/01-agent-framework-integration/spec.md](../10-specs/01-agent-framework-integration/spec.md)
- Microsoft.Extensions.AI: https://www.nuget.org/packages/Microsoft.Extensions.AI
