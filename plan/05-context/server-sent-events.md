# Server-Sent Events (SSE)

## Brief Description

Server-Sent Events (SSE) is a standard for servers to push real-time updates to clients over HTTP. It's a simpler alternative to WebSockets for unidirectional server-to-client streaming.

SSE uses the `text/event-stream` MIME type and provides a protocol for sending discrete events with named fields (`event:`, `data:`, `id:`, `retry:`).

## Relevance to Project

OpenCode uses SSE for streaming AI responses in real-time as the model generates tokens. This is critical for providing responsive user experience with AI interactions.

**Why this matters for our project**:
- Core to streaming API implementation
- Enables incremental message updates as AI generates responses
- Provides better UX than waiting for complete response

**Where it's used in our architecture**:
- `SendMessageStreamingAsync()` method uses SSE transport
- IAsyncEnumerable<MessageUpdate> parses SSE stream
- Streaming examples demonstrate SSE consumption

**Impact on implementation**:
- Need robust SSE parser for `text/event-stream` responses
- Must handle connection keep-alive and reconnection
- Requires careful HttpClient configuration for streaming

## Interoperability Points

**Integrates with**:
- HttpClient: HttpClient must be configured to not buffer responses (HttpCompletionOption.ResponseHeadersRead)
- IAsyncEnumerable: SSE events mapped to async enumerable for .NET consumption
- CancellationToken: Cancellation must properly terminate SSE connections

**Data flow**:
1. POST to /session/{id}/message/stream with Accept: text/event-stream
2. Server responds with status 200 and Content-Type: text/event-stream
3. Server sends events in SSE format: `data: {...}\n\n`
4. Client parses events and yields MessageUpdate objects
5. Client closes connection on cancellation or stream completion

**APIs and interfaces**:
- Accept: text/event-stream header signals SSE request
- Stream.ReadAsync() for reading bytes incrementally
- TextReader for line-based parsing

## Considerations and Watch Points

### Technical Considerations

**Complexity factors**:
- SSE protocol parsing requires careful state machine (multi-line data, field parsing)
- Connection management (keep-alive, reconnection on drop)
- Memory efficiency (avoid buffering entire stream)

**Learning curve**:
- SSE is simpler than WebSockets but still requires understanding of streaming HTTP
- .NET doesn't have built-in SSE client (unlike JavaScript EventSource)

**Potential conflicts or challenges**:
- **HttpClient buffering**: By default, HttpClient buffers entire response. Must use HttpCompletionOption.ResponseHeadersRead to stream.
- **Timeouts**: Normal timeouts don't apply to streaming; need different timeout strategy
- **Error handling**: Errors can occur mid-stream; need to handle partial data gracefully

### Best Practices

**For this project specifically**:
- Use HttpCompletionOption.ResponseHeadersRead for streaming requests
- Parse SSE line-by-line, not character-by-character (more efficient)
- Use Memory<T> and Span<T> to avoid string allocations
- Implement connection keep-alive handling (SSE retry field)
- Provide clear error messages when stream is interrupted

**Common patterns**:
- State machine for parsing SSE fields (idle, reading field name, reading field value)
- Buffering lines until double-newline (event terminator)
- JSON deserialization of `data:` field content

### Common Pitfalls

**Watch out for**:
- **Buffering entire response**: Defeats purpose of streaming, causes high memory usage
  - **How to avoid**: Always use HttpCompletionOption.ResponseHeadersRead
- **Not handling multi-line data fields**: SSE allows `data:` to span multiple lines
  - **How to avoid**: Accumulate data lines until event terminator
- **Ignoring retry field**: Server may send `retry:` to suggest reconnection delay
  - **How to avoid**: Parse and respect retry field for reconnection
- **Memory leaks in long streams**: String concatenation can cause allocations
  - **How to avoid**: Use StringBuilder or Memory<char> for accumulation

**Gotchas**:
- SSE events end with double newline (`\n\n`), not single
- Empty lines should be ignored (per spec)
- Comment lines start with `:` and should be ignored
- Field values have leading space after colon trimmed: `data: value` → `"value"`

### Performance Implications

- **Reading strategy**: Reading line-by-line is more efficient than byte-by-byte
- **Buffering**: Minimize string allocations by using spans
- **Parsing overhead**: JSON deserialization of each event adds latency (typically <1ms)

**Optimization opportunities**:
- Pool StringBuilder instances for line accumulation
- Use source-generated JSON for `data:` field deserialization
- Consider ValueTask for async enumeration (less allocation)

### Security Considerations

- **Connection hijacking**: Ensure HTTPS for remote connections (localhost ok for MVP)
- **Resource exhaustion**: Limit max event size to prevent memory attacks
- **Connection limits**: Don't allow unbounded concurrent streams

**Security best practices**:
- Validate event format before parsing
- Enforce max event size (e.g., 1MB)
- Set timeout for idle connections (no events for N seconds)

### Scalability Factors

- **Concurrent streams**: Each stream holds an open connection; limit concurrent streams
- **Memory per stream**: Each stream needs buffer for incomplete events (typically <10KB)
- **Connection pooling**: HttpClient reuses connections, but streaming ties up connection

**Scaling strategies**:
- Limit concurrent streaming operations (e.g., max 10)
- Use cancellation tokens to allow users to abort streams
- Monitor connection count and memory usage

## References

- SSE Specification: https://html.spec.whatwg.org/multipage/server-sent-events.html
- MDN SSE Guide: https://developer.mozilla.org/en-US/docs/Web/API/Server-sent_events
- .NET HttpClient Streaming: https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/http/httpclient-streaming
- Similar implementation: Docker.DotNet streaming (reference for patterns)
