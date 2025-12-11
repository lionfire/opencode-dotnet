# opencode-dotnet Implementation Phases

## Overview

The opencode-dotnet project will be implemented in **4 phases**, progressing from MVP foundation to production-ready SDK with Agent Framework integration.

**Total Estimated Duration**: 8-10 weeks

## Phase Progression Strategy

The phasing strategy follows these principles:

1. **Phase 1 (MVP Foundation)**: Deliver core SDK functionality quickly for early validation
   - Focus on essential session and message APIs
   - Basic error handling and streaming
   - Enable "30 seconds to first request" goal
   - Sufficient for building simple applications

2. **Phase 2 (Production Hardening)**: Make SDK production-ready
   - Add resilience, observability, and advanced error handling
   - Source-generated JSON for AOT
   - HttpClient factory integration
   - Enterprise-ready reliability features

3. **Phase 3 (Agent Framework Integration)**: Enable multi-agent workflows
   - Implement Microsoft Agent Framework integration (existing spec)
   - OpencodeAgent and OpencodeAgentThread classes
   - Message conversion and thread management
   - Middleware and DI extensions

4. **Phase 4 (Polish and Release)**: Prepare for public release
   - Comprehensive documentation and examples
   - NuGet packaging and publishing
   - Performance optimization
   - Community onboarding materials

This progression allows early adopters to start using the SDK after Phase 1, while ensuring enterprise teams have production-ready features by Phase 2, and advanced integration scenarios are enabled in Phase 3.

## Phase Definitions

### Phase 01: MVP Foundation

**Goal**: Deliver core SDK functionality that enables developers to create sessions, send messages, and receive responses from OpenCode server.

**Duration**: 2-3 weeks

**Key Deliverables**:
- Core client (`OpenCodeClient` implementing `IOpencodeClient`)
- Session management API (create, get, list, delete, fork, abort, share)
- Message/prompt API with multi-part message support
- Streaming API using IAsyncEnumerable
- Tool management API (list, get, approve, update permissions)
- File operations API (list, get content, search, apply changes)
- Command API (execute slash commands)
- Configuration API (get config, providers, models)
- Basic error handling with exception hierarchy
- Health check and server detection
- Session scope with auto-cleanup (IAsyncDisposable)
- Basic retry logic
- XML documentation for all public APIs
- Example project demonstrating core functionality

**Features Included**:
- All OpenCode REST API endpoints wrapped in typed methods
- Strong typing with records and DTOs
- Async/await patterns with CancellationToken support
- Basic timeout configuration (quick ops: 30s, AI ops: 5min)
- Clear error messages with troubleshooting hints
- ILogger integration for structured logging
- Quick start guide and basic documentation

**Target Audiences**: .NET Application Developers (basic scenarios)

**Success Criteria**:
- All core API methods implemented and tested
- Streaming responses work correctly with SSE
- Health check detects server availability
- Session auto-cleanup works reliably
- 30 seconds from NuGet install to first request achieved
- >70% code coverage in unit tests
- Integration tests pass against real OpenCode server

**Dependencies**: None (foundational phase)

**Risks**:
- **SSE streaming complexity**: Mitigation: Research HttpClient SSE patterns early, test with real server
- **Multi-part message handling**: Mitigation: Study OpenCode API carefully, test all message types
- **Error message quality**: Mitigation: Test common failure scenarios, refine messages iteratively

---

### Phase 02: Production Hardening

**Goal**: Make SDK production-ready with enterprise-grade resilience, observability, and performance optimization.

**Duration**: 2 weeks

**Key Deliverables**:
- Source-generated JSON serialization for AOT compatibility
- IHttpClientFactory integration for connection pooling
- Polly integration for advanced resilience (circuit breaker, timeout policies)
- OpenTelemetry integration (traces, metrics)
- Configuration validation with fail-fast on startup
- Message history pagination for large sessions
- Session query and filtering
- Timeout per-operation override
- Streaming progress callbacks (alternative to IAsyncEnumerable)
- Comprehensive integration test suite
- Performance benchmarks and optimization
- Enterprise deployment guide

**Features Included**:
- Native AOT compatibility via source generators
- Circuit breaker for repeated failures
- Distributed tracing with Activity/trace propagation
- Metrics for request duration, error rates, streaming chunks
- Configuration validation with helpful error messages
- Pagination support for sessions with many messages
- Filter sessions by date, status, tags
- Method-level timeout overrides
- Event-based streaming API for UI frameworks
- Performance testing and optimization
- Production troubleshooting runbook

**Target Audiences**: Enterprise Development Teams, .NET Library/SDK Authors

**Success Criteria**:
- Source-generated JSON works with Native AOT
- Circuit breaker prevents cascading failures
- OpenTelemetry traces visible in APM tools
- Configuration validation catches common errors
- Pagination handles 1000+ message sessions efficiently
- Performance benchmarks meet requirements:
  - Message conversion < 1ms
  - Thread serialization < 10ms for 100 messages
  - Streaming latency < 5ms overhead
- >80% code coverage
- Enterprise teams validate production readiness

**Dependencies**: Phase 1 complete

**Risks**:
- **Source generator complexity**: Mitigation: Use existing patterns from BCL, test thoroughly
- **Polly integration complexity**: Mitigation: Start simple, add advanced policies incrementally
- **Performance optimization**: Mitigation: Establish benchmarks early, profile regularly
- **OpenTelemetry overhead**: Mitigation: Make telemetry optional, measure overhead

---

### Phase 03: Agent Framework Integration

**Goal**: Enable OpenCode participation in Microsoft Agent Framework workflows as a first-class AI agent.

**Duration**: 2-3 weeks

**Key Deliverables**:
- `OpencodeAgent` class extending `AIAgent`
- `OpencodeAgentThread` class extending `AgentThread`
- Message converter (bidirectional between Agent Framework and OpenCode formats)
- Thread state manager (serialization/deserialization)
- Streaming integration (`IAsyncEnumerable<AgentRunResponseUpdate>`)
- Tool integration (map OpenCode tools to Agent Framework format)
- DI extensions (`AddOpencodeAgent`, `AddOpencodeAgentFactory`)
- `OpencodeAgentBuilder` fluent API
- Middleware support (`DelegatingAIAgent` pattern)
- Multi-agent workflow examples
- Agent Framework integration tests
- Agent Framework documentation

**Features Included**:
- Native Agent Framework participation (all workflow patterns)
- Full feature parity with native OpenCode capabilities
- Streaming performance within 5% of native
- Thread persistence and restoration across restarts
- Tool invocation mapping between frameworks
- Agent identity (Id, Name, Description, DisplayName)
- Service resolution via `GetService<T>()`
- Telemetry compatibility with Agent Framework middleware
- Human-in-the-loop middleware support
- Example projects: sequential agents, parallel agents, conditional workflows

**Target Audiences**: .NET Library/SDK Authors, Enterprise Development Teams (multi-agent scenarios)

**Success Criteria**:
- OpencodeAgent works in all Agent Framework workflow patterns
- Message conversion maintains full fidelity (no data loss)
- Streaming performance within 5% of native OpenCode
- Thread state survives serialization round-trip
- All Agent Framework middleware works correctly
- Multi-agent scenarios function properly
- Unit test coverage >80%
- Agent Framework documentation complete

**Dependencies**: Phase 1 complete (uses core SDK), Existing Agent Framework spec

**Risks**:
- **Agent Framework API changes**: Mitigation: Pin to specific version, abstract interfaces
- **Complex message conversion**: Mitigation: Comprehensive test coverage, validation tests
- **Thread state management**: Mitigation: Test serialization extensively, handle edge cases
- **Performance regression**: Mitigation: Continuous benchmarking, optimization sprints

---

### Phase 04: Polish and Release

**Goal**: Prepare SDK for public release with comprehensive documentation, examples, and community onboarding.

**Duration**: 1-2 weeks

**Key Deliverables**:
- Comprehensive README with quick start
- Example projects for all major scenarios:
  - Basic session usage
  - Streaming responses
  - DI integration (ASP.NET Core, console app)
  - Agent Framework integration
  - Multi-agent workflows
  - Error handling patterns
- API reference documentation (XML docs published)
- Contribution guide for open source contributors
- Issue and PR templates
- CI/CD pipeline (build, test, publish)
- NuGet package configuration and publishing
- Performance optimization pass
- Security audit
- License and legal review
- Announcement blog post
- Community communication plan

**Features Included**:
- Production-ready NuGet package
- Complete documentation suite
- Working examples for all audiences
- Contributor onboarding materials
- Automated CI/CD pipeline
- GitHub issue/PR templates
- Code of conduct and governance
- Security policy and reporting
- Community Discord/Slack setup
- Launch announcement and marketing

**Target Audiences**: All audiences (broad availability)

**Success Criteria**:
- NuGet package published with stable version (1.0.0)
- README provides 30-second quick start
- All major use cases have working examples
- Contribution guide enables community participation
- CI/CD pipeline runs tests and publishes automatically
- Zero critical security vulnerabilities
- Legal review complete
- Launch announcement published
- Community channels active
- First community contributions received

**Dependencies**: Phases 1, 2, and 3 complete

**Risks**:
- **Documentation completeness**: Mitigation: Review with external developers, iterate on feedback
- **Example quality**: Mitigation: Test examples with fresh developers, ensure they work out-of-box
- **NuGet publishing issues**: Mitigation: Test publish process to test feed first
- **Community readiness**: Mitigation: Prepare support materials, designate maintainers

---

## Phase Dependencies

```
Phase 01: MVP Foundation (weeks 1-3)
    ↓
Phase 02: Production Hardening (weeks 4-5)
    ↓
Phase 03: Agent Framework Integration (weeks 6-8)
    ↓
Phase 04: Polish and Release (weeks 9-10)
```

**Parallel Workstreams**: Within each phase, some epics can be worked on in parallel:
- Phase 1: Session API and Message API can be developed concurrently
- Phase 2: Source generators and Polly integration are independent
- Phase 3: OpencodeAgent and message converter can be developed separately
- Phase 4: Examples and documentation can be created in parallel

## Milestones

### Milestone 1: Core SDK Functional (End of Phase 1)
- All OpenCode API endpoints accessible
- Basic applications can be built
- Early adopter validation possible

### Milestone 2: Enterprise-Ready (End of Phase 2)
- Production-grade reliability
- Enterprise teams can deploy
- Performance validated

### Milestone 3: Agent Framework Enabled (End of Phase 3)
- Multi-agent workflows possible
- Advanced integration scenarios enabled
- SDK feature-complete

### Milestone 4: Public Release (End of Phase 4)
- NuGet package published
- Documentation complete
- Community onboarding active
- SDK ready for broad adoption

## Success Metrics

**Phase 1 Success**: 10+ early adopters using SDK in projects
**Phase 2 Success**: First enterprise production deployment
**Phase 3 Success**: First multi-agent application built
**Phase 4 Success**: 100+ downloads in first month, zero critical bugs

## Risk Management

**Overall Risk**: OpenCode API evolution during development
**Mitigation**: Close communication with OpenCode maintainers, version pinning, compatibility testing

**Overall Risk**: Community adoption lower than expected
**Mitigation**: Engage OpenCode community early, showcase examples, provide excellent documentation

**Overall Risk**: Performance issues at scale
**Mitigation**: Continuous benchmarking, performance budget, optimization sprints
