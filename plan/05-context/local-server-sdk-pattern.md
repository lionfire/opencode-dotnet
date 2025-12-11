# Local Server SDK Pattern

## Brief Description

The Local Server SDK Pattern is an architectural approach for .NET client libraries that communicate with a local headless server process (typically running on localhost). This pattern is exemplified by Docker.DotNet (Docker daemon) and OmniSharp clients (language servers).

Unlike cloud API SDKs (Stripe, Twilio), local server SDKs assume single-user, single-machine scenarios with no authentication, rate limiting, or multi-tenancy concerns.

## Relevance to Project

opencode-dotnet follows this pattern because OpenCode runs as a local server via `opencode serve`, not as a cloud service.

**Why this matters for our project**:
- Fundamentally different design constraints than cloud SDKs
- Simplifies authentication (none needed)
- Changes error handling focus (process lifecycle vs network errors)
- Impacts configuration (localhost default, no API keys)

**Where it's used in our architecture**:
- Base URL defaults to http://localhost:9123
- No authentication layer or API key management
- Health check verifies local process is running
- Error messages focus on "Is opencode serve running?"

**Impact on implementation**:
- No OAuth, API keys, or credential management
- No rate limiting or quota tracking
- Process lifecycle awareness (server may not be running)
- IPC-style communication patterns (efficient localhost)

## Interoperability Points

**Integrates with**:
- OpenCode server process: SDK communicates with locally-running `opencode serve`
- HttpClient: Uses localhost connections (different performance profile than internet)
- Session persistence: OpenCode uses SQLite locally, SDK references sessions by ID

**Data flow**:
1. User starts `opencode serve` process (separate from SDK)
2. SDK sends HTTP request to localhost:9123
3. OpenCode processes request and returns response
4. SDK parses response and returns typed objects

**APIs and interfaces**:
- Standard HTTP/REST API, but on localhost
- No authentication headers
- Sessions persist in OpenCode's local SQLite database

## Considerations and Watch Points

### Technical Considerations

**Complexity factors**:
- Process lifecycle management: SDK doesn't control server process
- Detecting if server is running (no standard health endpoint initially)
- Localhost connection failures are different than internet failures

**Learning curve**:
- Developers used to cloud SDKs may expect API keys
- Need to understand two-process architecture

**Potential conflicts or challenges**:
- **Server not running**: Most common error case, need excellent diagnostics
  - **How to handle**: Health check on construction, clear error messages
- **Port conflicts**: localhost:9123 might be taken by another process
  - **How to handle**: Allow configurable port, detect port conflicts
- **Multiple SDK instances**: Multiple clients to same server is fine, but port is shared
  - **How to handle**: HttpClient connection pooling, no global state in SDK

### Best Practices

**For this project specifically**:
- Default to http://localhost:9123 (OpenCode default)
- Provide health check method (PingAsync) to verify server availability
- Error messages should guide users: "Is opencode serve running?"
- Don't implement features assuming multiple servers (single-instance focus)
- Optimize for localhost performance (low latency, high throughput)

**Common patterns (from Docker.DotNet)**:
- Named pipes or Unix sockets for IPC (Docker.DotNet uses this on Linux)
- Localhost HTTP for cross-platform compatibility (our choice)
- Configuration primarily about connection, not authentication
- No retry on connection refused (server simply isn't running)

### Common Pitfalls

**Watch out for**:
- **Assuming server is always available**: Unlike cloud services with high uptime
  - **How to avoid**: Check health before operations, fail fast with clear guidance
- **Over-engineering authentication**: No auth needed for localhost
  - **How to avoid**: Keep security model simple, document assumptions
- **Ignoring process lifecycle**: Server can be stopped, restarted
  - **How to avoid**: Handle connection errors gracefully, document server requirements
- **Cloud SDK patterns**: Rate limiting, retries, exponential backoff less critical
  - **How to avoid**: Simpler retry logic, focus on transient failures not quotas

**Gotchas**:
- localhost can resolve to 127.0.0.1 or ::1 (IPv4 vs IPv6)
- Windows firewall may block localhost connections
- Session IDs from OpenCode persist across server restarts (SQLite)

### Performance Implications

- **Localhost latency**: Typically <1ms vs 50-200ms for internet APIs
- **Connection pooling**: Still beneficial but less critical than cloud APIs
- **Throughput**: Localhost can handle much higher request rates

**Optimization opportunities**:
- Connection keep-alive highly effective on localhost
- Can be more aggressive with concurrent requests
- Less need for caching (responses are fast)

### Security Considerations

- **Localhost-only binding**: OpenCode should only bind to 127.0.0.1, not 0.0.0.0
- **No credential leakage risk**: No API keys or tokens to leak
- **Process trust**: SDK trusts the local OpenCode process (same user)

**Security best practices**:
- Document that OpenCode should not expose to network
- If remote connections needed, require TLS
- Don't log sensitive file contents or commands

### Scalability Factors

- **Single-user, single-machine**: Not designed for distributed scenarios
- **Concurrent operations**: Limited by local machine resources
- **Session count**: OpenCode SQLite can handle many sessions

**Scaling strategies**:
- Vertical scaling only (more CPU/RAM on local machine)
- Multiple projects via `directory` parameter
- No horizontal scaling (can't load-balance across servers)

## References

- Docker.DotNet: https://github.com/dotnet/Docker.DotNet (excellent reference implementation)
- OmniSharp clients: https://github.com/OmniSharp (language server protocol)
- Language Server Protocol: https://microsoft.github.io/language-server-protocol/ (similar IPC pattern)
- Related Context: [opencode-architecture.md](opencode-architecture.md)
