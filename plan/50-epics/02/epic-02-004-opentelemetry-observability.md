---
greenlit: true
---

# Epic 02-004: OpenTelemetry Observability

**Phase**: 02 - Production Hardening
**Estimated Effort**: 3-4 days

## Overview

Integrate OpenTelemetry for distributed tracing, metrics, and observability in enterprise environments.

## Tasks

- [ ] Add OpenTelemetry.Api dependency
- [ ] Create ActivitySource for tracing
- [ ] Add traces for all HTTP operations
- [ ] Add metrics (request duration, error rate, streaming chunks)
- [ ] Propagate trace context in headers
- [ ] Add OpencodeClientOptions.EnableTelemetry setting
- [ ] Test with Jaeger or Zipkin
- [ ] Document APM integration

## Acceptance Criteria

- Traces visible in APM tools (Jaeger, Datadog, etc.)
- Metrics exported correctly
- Trace context propagates across calls
- Telemetry overhead <5%
