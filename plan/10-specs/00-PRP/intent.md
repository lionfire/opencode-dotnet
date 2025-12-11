# Product Intent Analysis

## Core Problem Being Solved

.NET developers who want to programmatically interact with OpenCode (an open-source terminal-first AI coding agent) currently lack a native, production-ready SDK. While OpenCode provides a powerful HTTP API via `opencode serve`, there's no official .NET client library to abstract the raw HTTP calls into strongly-typed, idiomatic .NET APIs. This forces .NET developers to either manually implement HTTP clients (reinventing the wheel) or resort to untyped, error-prone approaches using raw HTTP calls or dynamic objects.

The opencode-dotnet SDK solves this by providing a fully-typed, async-first, production-ready client that follows .NET conventions and best practices, similar to how Docker.DotNet provides a typed client for the Docker daemon API.

## Primary Goals and Objectives

1. **Eliminate HTTP boilerplate** - Abstract away raw HTTP calls, JSON serialization, error handling, and retry logic
2. **Provide excellent developer experience** - IntelliSense, XML documentation, intuitive API design following .NET conventions
3. **Enable production use** - Comprehensive error handling, cancellation support, proper timeouts, and retry policies
4. **Be the definitive .NET client for OpenCode** - Community-maintained, high-quality, well-tested SDK that becomes the standard
5. **Support local server patterns** - Unlike cloud API SDKs, this targets a local headless server with process lifecycle awareness
6. **Maintain minimal dependencies** - Keep package lightweight using native .NET libraries (HttpClient, System.Text.Json)

## User Needs Addressed

- **Application Integration**: Developers building applications that need AI-powered coding assistance can easily integrate OpenCode capabilities
- **Developer Tooling**: SDK/library authors can build higher-level abstractions, VS extensions, or domain-specific tools on top of OpenCode
- **Enterprise Automation**: Teams can automate code generation workflows, build internal developer platforms, or create CI/CD integrations
- **Quick Prototyping**: Minimal code required to get started (30 seconds to first request)
- **Type Safety**: Strong typing prevents runtime errors from API misuse
- **Async/Await Patterns**: Native support for modern async programming patterns in .NET
- **Dependency Injection**: Seamless integration with ASP.NET Core and modern .NET DI containers

## Implicit Requirements

Beyond the explicit features listed in the PRP, several requirements are implied:

1. **Process Lifecycle Awareness**: SDK should detect if OpenCode server is running and provide clear error messages if not
2. **Connection Management**: Efficient HttpClient usage with connection pooling for localhost connections
3. **Session State Management**: Clear handling of session lifecycle, especially for long-running sessions
4. **Streaming Efficiency**: Memory-efficient handling of large AI responses without buffering entire responses
5. **Versioning Strategy**: Handle API version differences gracefully as OpenCode evolves
6. **Documentation Quality**: Working examples for all major use cases, not just XML docs
7. **Testing Strategy**: Both unit tests (mocked) and integration tests (against real OpenCode server)
8. **Backwards Compatibility**: SemVer compliance for stable API evolution
9. **Performance Optimization**: Source-generated JSON for AOT compatibility and performance
10. **Error Diagnostics**: Meaningful error messages that help developers troubleshoot issues quickly

## Success Criteria

### Technical Success
- Published to NuGet with stable semantic versioning
- >80% code coverage in automated tests
- Zero critical security vulnerabilities in dependencies
- Package size < 500KB
- Full XML documentation coverage for public APIs
- Source-generated JSON for AOT compatibility

### Developer Experience Success
- 30 seconds from NuGet install to first successful API call
- IntelliSense provides useful information for all public APIs
- Clear error messages guide developers to solutions
- Working examples cover all common use cases
- Comprehensive README with quick start guide

### Community Success
- Becomes the de facto .NET SDK for OpenCode
- Active community contributions and feedback
- Referenced in OpenCode documentation as official .NET client
- Used in production by enterprise development teams

## Unique Value Proposition

Unlike cloud API SDKs (Stripe, Anthropic, Twilio), opencode-dotnet targets a **local headless server** pattern similar to Docker.DotNet or OmniSharp clients. This means:

1. **No Authentication Complexity**: No API keys, OAuth flows, or token management
2. **No Rate Limiting**: Localhost connections don't need rate limiting or quota management
3. **Process Lifecycle Focus**: SDK needs to handle scenarios where the local server isn't running
4. **Single-User Pattern**: Designed for single-machine, single-user usage rather than multi-tenant cloud scenarios
5. **IPC-Style Communication**: Lightweight, efficient communication patterns optimized for localhost
6. **Session Persistence**: Leverages OpenCode's SQLite-persisted sessions for state management

The SDK bridges the gap between .NET applications and OpenCode's powerful AI coding capabilities, making it as easy to use OpenCode from .NET as it is to use Docker from .NET with Docker.DotNet.

## Key Design Principles

1. **Minimal Dependencies**: Use native .NET libraries to keep package small and reduce dependency conflicts
2. **Modern .NET First**: Target .NET 8+ to leverage latest language features (no legacy framework baggage)
3. **Interface-Based Design**: IOpencodeClient interface for testability and mocking
4. **Immutable DTOs**: Use records for request/response models where appropriate
5. **Nullable Annotations**: Full nullability support throughout for null-safety
6. **Async-First**: All I/O operations use async/await with CancellationToken support
7. **Fail-Fast**: Clear, actionable exceptions rather than silent failures
8. **Progressive Enhancement**: MVP works simply, advanced features available when needed
