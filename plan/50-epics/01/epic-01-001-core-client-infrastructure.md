---
greenlit: true
---

# Epic 01-001: Core Client Infrastructure

**Phase**: 01 - MVP Foundation
**Status**: Implemented in v2.0
**Estimated Effort**: 1 week
**Priority**: Critical (blocks all other epics)

[← Back to Phase 01](../../40-phases/01-mvp-foundation.md)

## Overview

Establish the foundational infrastructure for the SDK including HTTP client, configuration, base DTOs, interfaces, and project structure. This epic provides the core abstractions and plumbing that all other functionality builds upon.

## Motivation

Before implementing any API endpoints, we need a solid foundation with proper configuration, HTTP handling, type definitions, and testability hooks. This infrastructure determines the architecture and patterns for the entire SDK.

## Status Overview

- [ ] Planning complete
- [ ] Design approved
- [ ] Development in progress
- [ ] Code review complete
- [ ] Testing complete
- [ ] Documentation complete

## Technical Requirements

### Project Structure
- [ ] Create `LionFire.OpenCode.Serve` project (.NET 8+ class library)
- [ ] Configure project properties (nullable enabled, implicit usings, C# 12)
- [ ] Add core dependencies (System.Text.Json, Microsoft.Extensions.Options, Microsoft.Extensions.Logging.Abstractions)
- [ ] Set up namespace structure (`LionFire.OpenCode.Serve`, `.Models`, `.Exceptions`)

### Configuration System
- [ ] Define `OpencodeClientOptions` class with properties:
  - [ ] BaseUrl (string, default: "http://localhost:9123")
  - [ ] Directory (string?, optional)
  - [ ] DefaultTimeout (TimeSpan, default: 30s)
  - [ ] MessageTimeout (TimeSpan, default: 5min)
  - [ ] EnableRetry (bool, default: true)
  - [ ] MaxRetryAttempts (int, default: 3)
  - [ ] RetryDelaySeconds (int, default: 2)
- [ ] Implement options validation (ValidateOnStart)
- [ ] Support constructor-based configuration
- [ ] Support IOptions<OpencodeClientOptions> pattern

### Core Interfaces
- [ ] Define `IOpencodeClient` interface with method signatures for:
  - [ ] Session management (CreateSessionAsync, GetSessionAsync, etc.)
  - [ ] Message operations (SendMessageAsync, GetMessagesAsync, etc.)
  - [ ] Streaming (SendMessageStreamingAsync with IAsyncEnumerable)
  - [ ] Tool operations (GetToolsAsync, ApproveToolAsync, etc.)
  - [ ] File operations (ListFilesAsync, GetFileContentAsync, etc.)
  - [ ] Command operations (ExecuteCommandAsync)
  - [ ] Configuration (GetConfigAsync, GetProvidersAsync, GetModelsAsync)
  - [ ] Health check (PingAsync)
  - [ ] Dispose pattern (IAsyncDisposable)

### Client Implementation
- [ ] Implement `OpenCodeClient` class implementing `IOpencodeClient`
- [ ] Constructor accepting HttpClient (for injection)
- [ ] Constructor accepting OpencodeClientOptions
- [ ] Internal HttpClient field with proper lifecycle management
- [ ] Base URL resolution from options
- [ ] Request building helpers (BuildRequest method)
- [ ] Response parsing helpers (ParseResponse<T> method)
- [ ] Cancellation token propagation throughout
- [ ] IAsyncDisposable implementation (dispose HttpClient if owned)

### Data Transfer Objects (DTOs)
- [ ] Define core DTOs as records:
  - [ ] Session (Id, CreatedAt, UpdatedAt, Status, SharedToken?, Directory?)
  - [ ] SessionStatus enum (Active, Aborted, Completed)
  - [ ] Message (Id, SessionId, Role, Parts, CreatedAt, TokenCount?)
  - [ ] MessageRole enum (System, User, Assistant, Tool)
  - [ ] MessagePart base (abstract record or discriminated union)
  - [ ] TextPart : MessagePart (Text string)
  - [ ] FilePart : MessagePart (FilePath string, Content string?)
  - [ ] AgentPart : MessagePart (AgentId string, Content string)
  - [ ] Tool (Id, Name, Description, Parameters, RequiresApproval)
  - [ ] ToolPermission (Allowed, Denied, RequiresApproval)
  - [ ] FileInfo (Path, Size, IsDirectory, LastModified)
  - [ ] CommandResult (Output, Error, ExitCode)
  - [ ] OpencodeConfig (Version, Providers, DefaultProvider)
  - [ ] Provider (Id, Name, Models, Available)
  - [ ] Model (Id, Name, Provider, MaxTokens, SupportsFunctions)
- [ ] Add JSON property name attributes for API compatibility
- [ ] Implement required/optional property handling
- [ ] Add nullable annotations throughout

### HTTP Helpers
- [ ] Implement `HttpClientExtensions` class with:
  - [ ] GET request helper (GetAsync<T>)
  - [ ] POST request helper (PostAsync<T>)
  - [ ] DELETE request helper (DeleteAsync)
  - [ ] PUT request helper (PutAsync<T>)
  - [ ] PATCH request helper (PatchAsync<T>)
  - [ ] SSE streaming helper (GetServerSentEventsAsync)
- [ ] Request serialization (JSON with System.Text.Json)
- [ ] Response deserialization with error checking
- [ ] Content-Type header handling ("application/json")
- [ ] Accept header handling ("application/json", "text/event-stream")

### JSON Serialization
- [ ] Define `JsonSerializerOptionsProvider` with default options:
  - [ ] PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  - [ ] DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
  - [ ] WriteIndented = false (compact)
  - [ ] PropertyNameCaseInsensitive = true (lenient parsing)
- [ ] Custom converters if needed (MessagePart polymorphism)
- [ ] Enum string conversion for readability

## Implementation Tasks

### 1. Project Setup
- [ ] Create new .NET 8 class library project
  - [ ] Set TargetFramework to net8.0
  - [ ] Enable nullable reference types
  - [ ] Enable implicit usings
  - [ ] Set LangVersion to 12
- [ ] Add PackageReference for Microsoft.Extensions.Options (v8.0.0+)
- [ ] Add PackageReference for Microsoft.Extensions.Logging.Abstractions (v8.0.0+)
- [ ] Create directory structure:
  - [ ] `/Models` (DTOs and domain models)
  - [ ] `/Exceptions` (exception hierarchy)
  - [ ] `/Extensions` (extension methods)
  - [ ] `/Internal` (internal helpers)

### 2. Configuration Implementation
- [ ] Create `OpencodeClientOptions.cs` in root namespace
  - [ ] Add all configuration properties with XML docs
  - [ ] Add default values as constants
  - [ ] Implement validation logic (ValidateBaseUrl method)
- [ ] Create `OpencodeClientOptionsValidator.cs` implementing `IValidateOptions<T>`
  - [ ] Validate BaseUrl is valid URI
  - [ ] Validate timeouts are positive
  - [ ] Validate retry settings are reasonable

### 3. Core DTOs Implementation
- [ ] Create `Session.cs` record with all properties
- [ ] Create `Message.cs` record with MessageRole enum
- [ ] Create `MessagePart.cs` hierarchy (TextPart, FilePart, AgentPart)
  - [ ] Use record base class or discriminated union pattern
  - [ ] Add JSON polymorphic serialization attributes
- [ ] Create `Tool.cs` and `ToolPermission.cs`
- [ ] Create `FileInfo.cs` (rename to avoid conflict with System.IO.FileInfo)
- [ ] Create `OpencodeConfig.cs`, `Provider.cs`, `Model.cs`
- [ ] Create `CommandResult.cs`
- [ ] Add XML documentation to all public types
- [ ] Add JSON property name attributes ([JsonPropertyName])

### 4. Interface Definition
- [ ] Create `IOpencodeClient.cs` interface
  - [ ] Add all method signatures from requirements
  - [ ] Use Task<T> for async operations
  - [ ] Use IAsyncEnumerable<T> for streaming
  - [ ] Include CancellationToken parameters (default)
  - [ ] Inherit from IAsyncDisposable
- [ ] Add comprehensive XML documentation to interface
  - [ ] Document parameters, return values, exceptions
  - [ ] Include code examples for complex methods

### 5. Client Implementation
- [ ] Create `OpenCodeClient.cs` implementing `IOpencodeClient`
  - [ ] Private readonly HttpClient field
  - [ ] Private readonly OpencodeClientOptions field
  - [ ] Private readonly ILogger? field (optional)
  - [ ] Constructor(HttpClient, OpencodeClientOptions, ILogger?)
  - [ ] Constructor(string baseUrl) - convenience constructor
- [ ] Implement stub methods for all interface members
  - [ ] Throw NotImplementedException for now
  - [ ] Add TODO comments for implementation in other epics
- [ ] Implement helper methods:
  - [ ] BuildRequestUri(string path, IDictionary<string, string>? query)
  - [ ] SendRequestAsync<T>(HttpMethod, string, object?, CancellationToken)
  - [ ] HandleErrorResponseAsync(HttpResponseMessage)
- [ ] Implement IAsyncDisposable
  - [ ] Dispose HttpClient if owned
  - [ ] Dispose any other disposable resources

### 6. JSON Serialization Setup
- [ ] Create `JsonSerializerOptionsProvider.cs` static class
  - [ ] Public static property `Default` returning configured options
- [ ] Test JSON serialization with sample DTOs
  - [ ] Serialize Session object
  - [ ] Deserialize Session JSON
  - [ ] Test MessagePart polymorphism serialization
- [ ] Create custom JsonConverter if needed for MessagePart

### 7. HTTP Helpers Implementation
- [ ] Create `HttpClientExtensions.cs` in `/Extensions`
  - [ ] GetJsonAsync<T>(this HttpClient, string, CancellationToken)
  - [ ] PostJsonAsync<T>(this HttpClient, string, object, CancellationToken)
  - [ ] DeleteAsync(this HttpClient, string, CancellationToken)
  - [ ] PutJsonAsync<T>(this HttpClient, string, object, CancellationToken)
- [ ] Error handling in extension methods
  - [ ] Check response.IsSuccessStatusCode
  - [ ] Parse error response body
  - [ ] Throw appropriate exception (defer to Epic 01-005)
- [ ] Log HTTP requests/responses (if ILogger provided)

## Testing Tasks

### Unit Tests
- [ ] Create `LionFire.OpenCode.Serve.Tests` project
- [ ] Add reference to xUnit, FluentAssertions, NSubstitute
- [ ] Test OpencodeClientOptions validation
  - [ ] Valid configuration passes
  - [ ] Invalid BaseUrl throws
  - [ ] Invalid timeouts throw
- [ ] Test OpenCodeClient construction
  - [ ] Construct with HttpClient succeeds
  - [ ] Construct with options succeeds
  - [ ] Construct with invalid options throws
- [ ] Test JSON serialization
  - [ ] Serialize/deserialize Session
  - [ ] Serialize/deserialize Message with all MessagePart types
  - [ ] Serialize/deserialize all DTOs
  - [ ] Handle null values correctly
- [ ] Test helper methods (mock HttpClient)
  - [ ] BuildRequestUri constructs correct URI
  - [ ] Query parameters encoded correctly

### Integration Tests
- [ ] Create `OpenCodeTestFixture` for integration tests
  - [ ] Start/stop OpenCode server if installed
  - [ ] Provide HttpClient configured for test server
  - [ ] Clean up test sessions after tests
- [ ] Test actual HTTP connection to real server
  - [ ] Client can reach localhost:9123
  - [ ] GET /config works
  - [ ] Error handling works for invalid endpoints

## Documentation Tasks

- [ ] Add XML documentation to all public types and members
- [ ] Create `README.md` for project (brief, defer full docs to Phase 4)
- [ ] Add code examples in XML docs for complex types
- [ ] Document configuration options with examples

## Dependencies & Blockers

**No blockers** - This is the foundational epic.

**Blocks**: All other Phase 1 epics depend on this.

## Acceptance Criteria

- [ ] `LionFire.OpenCode.Serve` project builds without warnings
- [ ] All core DTOs defined and documented
- [ ] `IOpencodeClient` interface complete with XML docs
- [ ] `OpenCodeClient` class implements interface (stubs ok)
- [ ] `OpencodeClientOptions` with validation works
- [ ] JSON serialization for all DTOs works correctly
- [ ] HTTP helper methods functional
- [ ] Unit tests pass with >70% coverage of this epic's code
- [ ] Integration test can connect to OpenCode server
- [ ] No nullable warnings or errors

## Notes

- Use records for DTOs to get structural equality and immutability
- Consider using `required` modifier for mandatory properties
- Ensure all async methods have `Async` suffix
- Follow .NET naming conventions strictly
- Keep internal helpers in `/Internal` namespace to avoid polluting public API
- Consider using source-generated JSON in Phase 2, but use reflection-based for MVP
