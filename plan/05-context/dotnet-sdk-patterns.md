# Context: .NET SDK Development Patterns

## Overview

Best practices and patterns for building high-quality .NET SDKs, drawn from established libraries like Stripe, Twilio, and Anthropic SDKs.

## Project Structure

### Recommended Layout
```
src/
├── OpencodeAI/                    # Main SDK library
│   ├── OpencodeClient.cs          # Primary entry point
│   ├── IOpencodeClient.cs         # Interface for testability
│   ├── OpencodeClientOptions.cs   # Configuration
│   ├── Models/                    # Request/Response DTOs
│   ├── Exceptions/                # Custom exceptions
│   ├── Extensions/                # DI extensions
│   └── Internal/                  # Implementation details
├── OpencodeAI.Tests/              # Unit tests
└── OpencodeAI.IntegrationTests/   # Integration tests

examples/
├── BasicUsage/
├── AspNetCoreIntegration/
└── StreamingExample/
```

## API Design Principles

### 1. Async by Default
- All I/O operations are async
- Return `Task<T>` or `IAsyncEnumerable<T>`
- Always accept `CancellationToken`

```csharp
public interface IOpencodeClient
{
    Task<CodeResponse> GenerateCodeAsync(
        GenerateCodeRequest request,
        CancellationToken cancellationToken = default);
}
```

### 2. Strongly Typed
- All request/response types are explicit
- Use enums for known values
- Use records for immutable DTOs

```csharp
public record GenerateCodeRequest
{
    public required string Prompt { get; init; }
    public ProgrammingLanguage? Language { get; init; }
}
```

### 3. Interface-Based Design
- Define `IOpencodeClient` interface
- Enables mocking in tests
- Supports decoration pattern

### 4. Minimal Dependencies
- Only essential packages
- Avoid dependency conflicts
- Use built-in .NET features

## Configuration Patterns

### Multiple Configuration Sources
```csharp
// Direct construction
var client = new OpencodeClient("api_key");

// Options pattern
var client = new OpencodeClient(options);

// Environment variables
var client = OpencodeClient.FromEnvironment();

// DI integration
services.AddOpencodeClient(o => o.ApiKey = "...");
```

### IOptions Integration
```csharp
public class OpencodeClientOptions
{
    public string? ApiKey { get; set; }
    public Uri BaseUrl { get; set; } = new("https://api.opencode.ai/v1");
    public TimeSpan Timeout { get; set; } = TimeSpan.FromMinutes(2);
}
```

## Error Handling

### Exception Hierarchy
```
OpencodeException (base)
├── OpencodeApiException (API errors)
│   ├── OpencodeAuthenticationException (401/403)
│   └── OpencodeRateLimitException (429)
├── OpencodeValidationException (client-side)
└── OpencodeTimeoutException (timeouts)
```

### Rich Error Information
```csharp
public class OpencodeApiException : OpencodeException
{
    public int StatusCode { get; }
    public string? ErrorCode { get; }
    public string? RequestId { get; }
}
```

## Testing Patterns

### MockHttpMessageHandler
```csharp
public class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly Queue<HttpResponseMessage> _responses = new();

    public void Enqueue(HttpResponseMessage response)
        => _responses.Enqueue(response);

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
        => Task.FromResult(_responses.Dequeue());
}
```

### Factory Pattern for Testing
```csharp
public interface IOpencodeClientFactory
{
    IOpencodeClient CreateClient(string name);
}
```

## Performance Considerations

### Connection Management
- Use `HttpClientFactory` in DI scenarios
- Reuse HttpClient instances
- Configure DNS refresh (PooledConnectionLifetime)

### JSON Serialization
- Use source generators (System.Text.Json)
- Avoid allocations in hot paths
- Use Utf8JsonReader for streaming

```csharp
[JsonSerializable(typeof(GenerateCodeRequest))]
[JsonSerializable(typeof(GenerateCodeResponse))]
internal partial class OpencodeJsonContext : JsonSerializerContext
{
}
```

## References

- [Microsoft API Guidelines](https://github.com/microsoft/api-guidelines)
- [Stripe .NET SDK](https://github.com/stripe/stripe-dotnet)
- [Anthropic .NET SDK](https://github.com/anthropics/anthropic-sdk-dotnet)
- [HttpClient Best Practices](https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-guidelines)
