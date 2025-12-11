# Project Context Documentation

This directory contains context documents about technologies, domains, and concepts relevant to the opencode-dotnet project.

## Purpose

These documents serve as reference material throughout the project lifecycle, helping team members understand:
- Technologies and frameworks used
- Domain-specific knowledge (local server patterns, AI SDK design)
- Integration points and dependencies
- Important considerations and best practices

## Context Documents

### Technologies
- [Server-Sent Events (SSE)](server-sent-events.md) - Streaming protocol for AI responses
- [System.Text.Json Source Generators](systemtextjson-source-generators.md) - AOT-compatible JSON serialization
- [IHttpClientFactory](ihttpclientfactory.md) - Proper HttpClient lifecycle management
- [Microsoft Agent Framework](microsoft-agent-framework.md) - Multi-agent orchestration
- [Polly Resilience](polly-resilience.md) - Advanced retry and circuit breaker patterns
- [OpenTelemetry](opentelemetry.md) - Distributed tracing and metrics

### Patterns and Practices
- [Local Server SDK Pattern](local-server-sdk-pattern.md) - Docker.DotNet-style design
- [Async/Await Best Practices](async-await-best-practices.md) - Modern .NET async patterns
- [Session-Based API Design](session-based-api-design.md) - Stateful conversation patterns

### OpenCode Domain
- [OpenCode Architecture](opencode-architecture.md) - Understanding OpenCode server
- [OpenCode REST API](opencode-rest-api.md) - API structure and endpoints
