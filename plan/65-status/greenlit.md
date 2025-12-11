# Greenlit Epics - Ready for Implementation

Epics in this file are approved and ready for active development.

# Epics

## Phase 01: MVP Foundation

- 01-001
- 01-002
- 01-003
- 01-004
- 01-005
- 01-006

## Phase 02: Production Hardening

- 02-001
- 02-002
- 02-003
- 02-004
- 02-005
- 02-006

## Phase 03: Agent Framework Integration

- 03-001
- 03-002
- 03-003
- 03-004
- 03-005
- 03-006

## Phase 04: Polish and Release

- 04-001
- 04-002
- 04-003
- 04-004
- 04-005
- 04-006

# Advisory Notices

## Implementation Order

The epics are organized by phase and listed in dependency order within each phase.

**Recommended implementation sequence:**

### Phase 01: MVP Foundation (Week 1-3)
1. **First**: Epic 01-001 (Core Client Infrastructure) - foundational, blocks all others
2. **Second**: Epics 01-002, 01-003, 01-004, 01-005 can run in parallel after 01-001 completes
3. **Third**: Epic 01-006 (Testing & Examples) - requires other Phase 01 epics complete

### Phase 02: Production Hardening (Week 4-6)
1. Epic 02-001 (Source-Generated JSON & AOT) - foundational for Phase 2
2. Epics 02-002, 02-003, 02-004 can run in parallel
3. Epic 02-005 (Advanced API Features) - requires core APIs complete
4. Epic 02-006 (Performance Optimization) - final polish for Phase 2

### Phase 03: Agent Framework Integration (Week 7-9)
1. Epic 03-001 (OpCodeAgent Core) - foundational for agent framework
2. Epic 03-002 (Message Conversion) - can start in parallel with 03-001
3. Epic 03-003 (Thread Management) - requires 03-001
4. Epic 03-004 (Streaming Integration) - requires 03-001 and 03-002
5. Epic 03-005 (DI Extensions & Builder) - integration layer
6. Epic 03-006 (Testing & Examples) - requires Phase 3 complete

### Phase 04: Polish and Release (Week 10-12)
1. Epic 04-001 (Documentation Suite) - can start early
2. Epic 04-002 (Example Projects) - requires core functionality complete
3. Epic 04-003 (NuGet Packaging) - technical setup
4. Epic 04-004 (Community Infrastructure) - can run in parallel with docs
5. Epic 04-005 (Security & Legal Review) - requires everything else
6. Epic 04-006 (Launch & Marketing) - final step

## Parallel Opportunities

Epics that can be worked on simultaneously:

**After 01-001 completes:**
- 01-002 (Session Management)
- 01-003 (Message Streaming)
- 01-004 (Tool & File Operations)
- 01-005 (Error Handling & Logging)

**Within Phase 02:**
- 02-002 (HttpClientFactory)
- 02-003 (Polly Resilience)
- 02-004 (OpenTelemetry)

**Within Phase 03:**
- 03-001 (OpCodeAgent Core)
- 03-002 (Message Conversion)

**Within Phase 04:**
- 04-001 (Documentation)
- 04-004 (Community Infrastructure)

## Critical Path

**MVP Critical Path:**
01-001 → 01-002/01-003/01-004 → 01-006 → Phase 1 Complete

**Production Critical Path:**
02-001 → 02-002/02-003/02-004 → 02-005 → 02-006 → Phase 2 Complete

**Agent Framework Critical Path:**
03-001 → 03-003 → 03-004 → 03-005 → 03-006 → Phase 3 Complete

**Release Critical Path:**
04-001/04-002 → 04-003 → 04-005 → 04-006 → Release

## Notes

- **Total Epics**: 24 greenlit across 4 phases
- **Estimated Duration**: 12 weeks (sequential), 8-10 weeks with parallelization
- **Last Updated**: 2025-12-09

**Priority Focus:**
- Phase 01: Epic 01-001 is absolutely critical - it's the foundation for everything
- Phase 02: Epic 02-001 (AOT support) is important for .NET ecosystem credibility
- Phase 03: Epic 03-001 (OpCodeAgent Core) unlocks the high-level agent framework
- Phase 04: Epic 04-005 (Security Review) gates release - don't skip

**Key Dependencies:**
- Phase 02 requires Phase 01 complete (core SDK must work)
- Phase 03 requires Phase 01 complete (builds on core SDK)
- Phase 04 requires Phase 02 and 03 complete (can't document/package incomplete features)

**Risk Areas:**
- Epic 03-004 (Streaming Integration) is complex - may need extra time
- Epic 04-005 (Security Review) could reveal issues requiring rework
- Testing epics (01-006, 03-006) are critical for quality - don't rush
