---
greenlit: true
---

# Epic 02-003: Polly Resilience Integration

**Phase**: 02 - Production Hardening
**Estimated Effort**: 4-5 days

## Overview

Integrate Polly for advanced resilience patterns: circuit breaker, advanced retry, timeout, bulkhead.

## Tasks

- [ ] Add Polly dependency (optional package or built-in)
- [ ] Implement circuit breaker policy for repeated failures
- [ ] Enhanced retry policy with jitter
- [ ] Per-operation timeout policies
- [ ] Add OpencodeClientOptions resilience settings
- [ ] Test circuit breaker behavior (open, half-open, closed)
- [ ] Document resilience configuration
- [ ] Add example demonstrating resilience patterns

## Acceptance Criteria

- Circuit breaker prevents cascading failures
- Retry with jitter reduces thundering herd
- Timeout policies configurable per operation
- >85% coverage of resilience code paths
