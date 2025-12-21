# Epic 01-001: Map OpenCode Web Directory and Component Structure

**Change**: 07 - Faithful OpenCode Web UI Replication
**Phase**: 01 - Deep Analysis
**Status**: planned
**Effort**: 1 day
**Dependencies**: None

## Status Overview

- [ ] Planning Complete
- [ ] Development Started
- [ ] Core Features Complete
- [ ] Testing Complete
- [ ] Documentation Complete

## Overview

Thoroughly explore and document the OpenCode web UI codebase located at `/dv/opencode/packages/ui/`. This epic focuses on understanding the directory structure, identifying all components, and creating a comprehensive inventory that will guide the replication effort.

## User Stories & Implementation Tasks

### Must Have

#### Story 001: Create Directory Structure Map
**Effort**: 4 hours
**Status**: pending

Create a comprehensive map of the `/dv/opencode/packages/ui/` directory structure.

##### Tasks
- [ ] 0001: Navigate to `/dv/opencode/packages/ui/` and list all top-level directories
- [ ] 0002: Document the purpose of each top-level directory (src/, script/, etc.)
- [ ] 0003: Map the `src/` subdirectory structure (components, hooks, utils, etc.)
- [ ] 0004: Identify and document the build configuration files (vite.config.ts, tsconfig.json, package.json)
- [ ] 0005: Create a markdown document: `/src/opencode-dotnet/docs/opencode-analysis/directory-structure.md`

#### Story 002: Component Inventory
**Effort**: 4 hours
**Status**: pending

Identify and catalog every React component in the OpenCode web UI.

##### Tasks
- [ ] 0001: Search for all `.tsx` and `.ts` files in `/dv/opencode/packages/ui/src/`
- [ ] 0002: For each component file, extract the component name and file path
- [ ] 0003: Identify which components are UI components vs utility/hook files
- [ ] 0004: Categorize components by functionality (chat, tools, editor, etc.)
- [ ] 0005: Document component props/interfaces where clearly visible
- [ ] 0006: Create component inventory table in `/src/opencode-dotnet/docs/opencode-analysis/component-inventory.md`
- [ ] 0007: Note any external dependencies used (libraries, frameworks)

### Should Have

#### Story 003: Assets and Resources Catalog
**Effort**: 2 hours
**Status**: pending

Document all assets, icons, images, and static resources used in the UI.

##### Tasks
- [ ] 0001: Locate assets directories (images, icons, fonts)
- [ ] 0002: Document SVG icons and their usage
- [ ] 0003: Identify font files and typography resources
- [ ] 0004: Note any embedded assets in component files
- [ ] 0005: Add assets section to `/src/opencode-dotnet/docs/opencode-analysis/directory-structure.md`

### Nice to Have

#### Story 004: Package Dependencies Analysis
**Effort**: 1 hour
**Status**: pending

Analyze the package.json to understand third-party dependencies.

##### Tasks
- [ ] 0001: Read `/dv/opencode/packages/ui/package.json`
- [ ] 0002: List all runtime dependencies
- [ ] 0003: Identify UI framework dependencies (React, etc.)
- [ ] 0004: Note any state management libraries
- [ ] 0005: Document in `/src/opencode-dotnet/docs/opencode-analysis/dependencies.md`

## Technical Requirements Checklist

- [ ] All documentation files are created in `/src/opencode-dotnet/docs/opencode-analysis/`
- [ ] Directory structure is documented with clear hierarchy
- [ ] Component inventory includes file paths and categorization
- [ ] Assets are cataloged with locations
- [ ] Documentation is clear and useful for the team

## Dependencies & Blockers

- Access to `/dv/opencode/packages/ui/` directory (verified accessible)

## Acceptance Criteria

- [ ] Directory structure document created with complete hierarchy
- [ ] Component inventory table lists all React components with categorization
- [ ] At least 30+ components identified and documented
- [ ] Assets catalog completed
- [ ] All documentation is markdown format and readable
- [ ] Documentation provides clear roadmap for Epic 01-002 (component hierarchy)

## Notes

**Source Directory**: `/dv/opencode/packages/ui/`
**Documentation Target**: `/src/opencode-dotnet/docs/opencode-analysis/`

This is the foundation epic - all subsequent analysis depends on this comprehensive mapping.
