---
greenlit: true
---

# Epic 02-001: Source-Generated JSON and AOT

**Phase**: 02 - Production Hardening
**Estimated Effort**: 3-4 days

## Overview

Implement System.Text.Json source generators for all DTOs to enable Native AOT compilation and improve startup performance.

## Tasks

- [ ] Add [JsonSerializable] attributes to all DTOs
- [ ] Create JsonSerializerContext class
- [ ] Update serialization calls to use generated context
- [ ] Test AOT compilation with sample app
- [ ] Benchmark startup time improvement
- [ ] Update documentation for AOT scenarios

## Acceptance Criteria

- AOT compilation succeeds
- All JSON operations work with source-generated code
- Startup time improved by >20%
- No reflection warnings in AOT mode
