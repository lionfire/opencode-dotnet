---
greenlit: true
---

# Epic 02-002: HttpClientFactory and Connection Management

**Phase**: 02 - Production Hardening
**Estimated Effort**: 2-3 days

## Overview

Integrate with IHttpClientFactory for proper HttpClient lifecycle management and connection pooling.

## Tasks

- [ ] Add Microsoft.Extensions.Http dependency
- [ ] Create AddOpencodeClient() DI extension
- [ ] Support IHttpClientFactory in client construction
- [ ] Configure HttpClient with default settings (timeout, headers)
- [ ] Document DI registration patterns
- [ ] Test connection pooling behavior

## Acceptance Criteria

- AddOpencodeClient() extension works with IServiceCollection
- HttpClient managed by factory
- Connection pooling verified
- ASP.NET Core example works
