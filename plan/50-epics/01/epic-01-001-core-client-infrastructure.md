---
greenlit: true
implementationDone: true
implementationReviewed: true
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

- [x] Planning complete
- [x] Design approved
- [x] Development in progress
- [x] Code review complete
- [x] Testing complete
- [x] Documentation complete

## Technical Requirements

### Project Structure
- [x] Create `LionFire.OpenCode.Serve` project (.NET 8+ class library)
- [x] Configure project properties (nullable enabled, implicit usings, C# 12)
- [x] Add core dependencies (System.Text.Json, Microsoft.Extensions.Options, Microsoft.Extensions.Logging.Abstractions)
- [x] Set up namespace structure (`LionFire.OpenCode.Serve`, `.Models`, `.Exceptions`)

### Configuration System
- [x] Define `OpencodeClientOptions` class with properties:
  - [x] BaseUrl (string, default: "http://localhost:9123")
  - [x] Directory (string?, optional)
  - [x] DefaultTimeout (TimeSpan, default: 30s)
  - [x] MessageTimeout (TimeSpan, default: 5min)
  - [x] EnableRetry (bool, default: true)
  - [x] MaxRetryAttempts (int, default: 3)
  - [x] RetryDelaySeconds (int, default: 2)
- [x] Implement options validation (ValidateOnStart)
- [x] Support constructor-based configuration
- [x] Support IOptions<OpencodeClientOptions> pattern

### Core Interfaces
- [x] Define `IOpencodeClient` interface with method signatures for:
  - [x] Session management (CreateSessionAsync, GetSessionAsync, etc.)
  - [x] Message operations (SendMessageAsync, GetMessagesAsync, etc.)
  - [x] Streaming (SendMessageStreamingAsync with IAsyncEnumerable)
  - [x] Tool operations (GetToolsAsync, ApproveToolAsync, etc.)
  - [x] File operations (ListFilesAsync, GetFileContentAsync, etc.)
  - [x] Command operations (ExecuteCommandAsync)
  - [x] Configuration (GetConfigAsync, GetProvidersAsync, GetModelsAsync)
  - [x] Health check (PingAsync)
  - [x] Dispose pattern (IAsyncDisposable)

### Client Implementation
- [x] Implement `OpenCodeClient` class implementing `IOpencodeClient`
- [x] Constructor accepting HttpClient (for injection)
- [x] Constructor accepting OpencodeClientOptions
- [x] Internal HttpClient field with proper lifecycle management
- [x] Base URL resolution from options
- [x] Request building helpers (BuildRequest method)
- [x] Response parsing helpers (ParseResponse<T> method)
- [x] Cancellation token propagation throughout
- [x] IAsyncDisposable implementation (dispose HttpClient if owned)

### Data Transfer Objects (DTOs)
- [x] Define core DTOs as records:
  - [x] Session (Id, CreatedAt, UpdatedAt, Status, SharedToken?, Directory?)
  - [x] SessionStatus enum (Active, Aborted, Completed)
  - [x] Message (Id, SessionId, Role, Parts, CreatedAt, TokenCount?)
  - [x] MessageRole enum (System, User, Assistant, Tool)
  - [x] MessagePart base (abstract record or discriminated union)
  - [x] TextPart : MessagePart (Text string)
  - [x] FilePart : MessagePart (FilePath string, Content string?)
  - [x] AgentPart : MessagePart (AgentId string, Content string)
  - [x] Tool (Id, Name, Description, Parameters, RequiresApproval)
  - [x] ToolPermission (Allowed, Denied, RequiresApproval)
  - [x] FileInfo (Path, Size, IsDirectory, LastModified)
  - [x] CommandResult (Output, Error, ExitCode)
  - [x] OpencodeConfig (Version, Providers, DefaultProvider)
  - [x] Provider (Id, Name, Models, Available)
  - [x] Model (Id, Name, Provider, MaxTokens, SupportsFunctions)
- [x] Add JSON property name attributes for API compatibility
- [x] Implement required/optional property handling
- [x] Add nullable annotations throughout

### HTTP Helpers
- [x] Implement `HttpClientExtensions` class with:
  - [x] GET request helper (GetAsync<T>)
  - [x] POST request helper (PostAsync<T>)
  - [x] DELETE request helper (DeleteAsync)
  - [x] PUT request helper (PutAsync<T>)
  - [x] PATCH request helper (PatchAsync<T>)
  - [x] SSE streaming helper (GetServerSentEventsAsync)
- [x] Request serialization (JSON with System.Text.Json)
- [x] Response deserialization with error checking
- [x] Content-Type header handling ("application/json")
- [x] Accept header handling ("application/json", "text/event-stream")

### JSON Serialization
- [x] Define `JsonSerializerOptionsProvider` with default options:
  - [x] PropertyNamingPolicy = JsonNamingPolicy.CamelCase
  - [x] DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
  - [x] WriteIndented = false (compact)
  - [x] PropertyNameCaseInsensitive = true (lenient parsing)
- [x] Custom converters if needed (MessagePart polymorphism)
- [x] Enum string conversion for readability

## Implementation Tasks

### 1. Project Setup
- [x] Create new .NET 8 class library project
  - [x] Set TargetFramework to net8.0
  - [x] Enable nullable reference types
  - [x] Enable implicit usings
  - [x] Set LangVersion to 12
- [x] Add PackageReference for Microsoft.Extensions.Options (v8.0.0+)
- [x] Add PackageReference for Microsoft.Extensions.Logging.Abstractions (v8.0.0+)
- [x] Create directory structure:
  - [x] `/Models` (DTOs and domain models)
  - [x] `/Exceptions` (exception hierarchy)
  - [x] `/Extensions` (extension methods)
  - [x] `/Internal` (internal helpers)

### 2. Configuration Implementation
- [x] Create `OpencodeClientOptions.cs` in root namespace
  - [x] Add all configuration properties with XML docs
  - [x] Add default values as constants
  - [x] Implement validation logic (ValidateBaseUrl method)
- [x] Create `OpencodeClientOptionsValidator.cs` implementing `IValidateOptions<T>`
  - [x] Validate BaseUrl is valid URI
  - [x] Validate timeouts are positive
  - [x] Validate retry settings are reasonable

### 3. Core DTOs Implementation
- [x] Create `Session.cs` record with all properties
- [x] Create `Message.cs` record with MessageRole enum
- [x] Create `MessagePart.cs` hierarchy (TextPart, FilePart, AgentPart)
  - [x] Use record base class or discriminated union pattern
  - [x] Add JSON polymorphic serialization attributes
- [x] Create `Tool.cs` and `ToolPermission.cs`
- [x] Create `FileInfo.cs` (rename to avoid conflict with System.IO.FileInfo)
- [x] Create `OpencodeConfig.cs`, `Provider.cs`, `Model.cs`
- [x] Create `CommandResult.cs`
- [x] Add XML documentation to all public types
- [x] Add JSON property name attributes ([JsonPropertyName])

### 4. Interface Definition
- [x] Create `IOpencodeClient.cs` interface
  - [x] Add all method signatures from requirements
  - [x] Use Task<T> for async operations
  - [x] Use IAsyncEnumerable<T> for streaming
  - [x] Include CancellationToken parameters (default)
  - [x] Inherit from IAsyncDisposable
- [x] Add comprehensive XML documentation to interface
  - [x] Document parameters, return values, exceptions
  - [x] Include code examples for complex methods

### 5. Client Implementation
- [x] Create `OpenCodeClient.cs` implementing `IOpencodeClient`
  - [x] Private readonly HttpClient field
  - [x] Private readonly OpencodeClientOptions field
  - [x] Private readonly ILogger? field (optional)
  - [x] Constructor(HttpClient, OpencodeClientOptions, ILogger?)
  - [x] Constructor(string baseUrl) - convenience constructor
- [x] Implement stub methods for all interface members
  - [x] Throw NotImplementedException for now
  - [x] Add TODO comments for implementation in other epics
- [x] Implement helper methods:
  - [x] BuildRequestUri(string path, IDictionary<string, string>? query)
  - [x] SendRequestAsync<T>(HttpMethod, string, object?, CancellationToken)
  - [x] HandleErrorResponseAsync(HttpResponseMessage)
- [x] Implement IAsyncDisposable
  - [x] Dispose HttpClient if owned
  - [x] Dispose any other disposable resources

### 6. JSON Serialization Setup
- [x] Create `JsonSerializerOptionsProvider.cs` static class
  - [x] Public static property `Default` returning configured options
- [x] Test JSON serialization with sample DTOs
  - [x] Serialize Session object
  - [x] Deserialize Session JSON
  - [x] Test MessagePart polymorphism serialization
- [x] Create custom JsonConverter if needed for MessagePart

### 7. HTTP Helpers Implementation
- [x] Create `HttpClientExtensions.cs` in `/Extensions`
  - [x] GetJsonAsync<T>(this HttpClient, string, CancellationToken)
  - [x] PostJsonAsync<T>(this HttpClient, string, object, CancellationToken)
  - [x] DeleteAsync(this HttpClient, string, CancellationToken)
  - [x] PutJsonAsync<T>(this HttpClient, string, object, CancellationToken)
- [x] Error handling in extension methods
  - [x] Check response.IsSuccessStatusCode
  - [x] Parse error response body
  - [x] Throw appropriate exception (defer to Epic 01-005)
- [x] Log HTTP requests/responses (if ILogger provided)

## Testing Tasks

### Unit Tests
- [x] Create `LionFire.OpenCode.Serve.Tests` project
- [x] Add reference to xUnit, FluentAssertions, NSubstitute
- [x] Test OpencodeClientOptions validation
  - [x] Valid configuration passes
  - [x] Invalid BaseUrl throws
  - [x] Invalid timeouts throw
- [x] Test OpenCodeClient construction
  - [x] Construct with HttpClient succeeds
  - [x] Construct with options succeeds
  - [x] Construct with invalid options throws
- [x] Test JSON serialization
  - [x] Serialize/deserialize Session
  - [x] Serialize/deserialize Message with all MessagePart types
  - [x] Serialize/deserialize all DTOs
  - [x] Handle null values correctly
- [x] Test helper methods (mock HttpClient)
  - [x] BuildRequestUri constructs correct URI
  - [x] Query parameters encoded correctly

### Integration Tests
- [x] Create `OpenCodeTestFixture` for integration tests
  - [x] Start/stop OpenCode server if installed
  - [x] Provide HttpClient configured for test server
  - [x] Clean up test sessions after tests
- [x] Test actual HTTP connection to real server
  - [x] Client can reach localhost:9123
  - [x] GET /config works
  - [x] Error handling works for invalid endpoints

## Documentation Tasks

- [x] Add XML documentation to all public types and members
- [x] Create `README.md` for project (brief, defer full docs to Phase 4)
- [x] Add code examples in XML docs for complex types
- [x] Document configuration options with examples

## Dependencies & Blockers

**No blockers** - This is the foundational epic.

**Blocks**: All other Phase 1 epics depend on this.

## Acceptance Criteria

- [x] `LionFire.OpenCode.Serve` project builds without warnings
- [x] All core DTOs defined and documented
- [x] `IOpencodeClient` interface complete with XML docs
- [x] `OpenCodeClient` class implements interface (stubs ok)
- [x] `OpencodeClientOptions` with validation works
- [x] JSON serialization for all DTOs works correctly
- [x] HTTP helper methods functional
- [x] Unit tests pass with >70% coverage of this epic's code
- [x] Integration test can connect to OpenCode server
- [x] No nullable warnings or errors

## Notes

- Use records for DTOs to get structural equality and immutability
- Consider using `required` modifier for mandatory properties
- Ensure all async methods have `Async` suffix
- Follow .NET naming conventions strictly
- Keep internal helpers in `/Internal` namespace to avoid polluting public API
- Consider using source-generated JSON in Phase 2, but use reflection-based for MVP
