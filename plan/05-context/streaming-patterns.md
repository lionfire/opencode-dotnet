# Context: Streaming Patterns in .NET

## Overview

Patterns for implementing efficient streaming in .NET for AI API responses.

## IAsyncEnumerable Pattern

### Basic Implementation
```csharp
public async IAsyncEnumerable<CodeChunk> StreamCodeAsync(
    GenerateCodeRequest request,
    [EnumeratorCancellation] CancellationToken cancellationToken = default)
{
    using var response = await _httpClient.SendAsync(
        CreateRequest(request),
        HttpCompletionOption.ResponseHeadersRead,
        cancellationToken);

    await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
    using var reader = new StreamReader(stream);

    while (!reader.EndOfStream)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var line = await reader.ReadLineAsync(cancellationToken);
        if (TryParseChunk(line, out var chunk))
            yield return chunk;
    }
}
```

### Key Points
- Use `HttpCompletionOption.ResponseHeadersRead` to start streaming immediately
- Use `[EnumeratorCancellation]` attribute for proper cancellation
- Dispose resources with `using`/`await using`

## Server-Sent Events (SSE)

### Format
```
data: {"code": "public class ", "index": 0}

data: {"code": "MyClass", "index": 1}

data: [DONE]
```

### Parser
```csharp
internal class SseParser
{
    public bool TryParse(string line, out CodeChunk? chunk)
    {
        chunk = null;

        if (string.IsNullOrEmpty(line))
            return false;

        if (!line.StartsWith("data: "))
            return false;

        var data = line[6..];

        if (data == "[DONE]")
            return false;

        chunk = JsonSerializer.Deserialize<CodeChunk>(data);
        return chunk != null;
    }
}
```

## Newline-Delimited JSON (NDJSON)

### Format
```
{"code": "public class ", "index": 0}
{"code": "MyClass", "index": 1}
{"done": true}
```

### Parser
```csharp
internal class NdjsonParser
{
    public bool TryParse(string line, out CodeChunk? chunk)
    {
        chunk = null;

        if (string.IsNullOrWhiteSpace(line))
            return false;

        var obj = JsonSerializer.Deserialize<JsonElement>(line);

        if (obj.TryGetProperty("done", out var done) && done.GetBoolean())
            return false;

        chunk = JsonSerializer.Deserialize<CodeChunk>(line);
        return chunk != null;
    }
}
```

## Memory Efficiency

### Avoid Buffering
```csharp
// Bad - buffers entire response
var content = await response.Content.ReadAsStringAsync();

// Good - streams directly
await using var stream = await response.Content.ReadAsStreamAsync();
```

### Use ArrayPool for Buffers
```csharp
var buffer = ArrayPool<byte>.Shared.Rent(4096);
try
{
    // Use buffer
}
finally
{
    ArrayPool<byte>.Shared.Return(buffer);
}
```

## Extension Methods

### Collecting Full Response
```csharp
public static async Task<GenerateCodeResponse> ToFullResponseAsync(
    this IAsyncEnumerable<CodeChunk> stream,
    CancellationToken cancellationToken = default)
{
    var builder = new StringBuilder();
    CodeChunk? last = null;

    await foreach (var chunk in stream.WithCancellation(cancellationToken))
    {
        builder.Append(chunk.Code);
        last = chunk;
    }

    return new GenerateCodeResponse
    {
        Code = builder.ToString(),
        // Copy other properties from last chunk
    };
}
```

## Testing Streaming

### Mock Streaming Response
```csharp
public HttpResponseMessage CreateStreamingResponse(params string[] chunks)
{
    var content = string.Join("\n\n", chunks.Select(c => $"data: {c}"));
    content += "\n\ndata: [DONE]\n\n";

    return new HttpResponseMessage(HttpStatusCode.OK)
    {
        Content = new StringContent(content, Encoding.UTF8, "text/event-stream")
    };
}
```

## References

- [IAsyncEnumerable Documentation](https://learn.microsoft.com/en-us/dotnet/api/system.collections.generic.iasyncenumerable-1)
- [Server-Sent Events Spec](https://html.spec.whatwg.org/multipage/server-sent-events.html)
- [NDJSON Spec](https://github.com/ndjson/ndjson-spec)
