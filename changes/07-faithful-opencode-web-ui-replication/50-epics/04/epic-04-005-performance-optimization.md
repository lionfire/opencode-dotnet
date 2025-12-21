# Epic 04-005: Performance Optimization

## Overview

Optimize for smooth performance with large sessions. The current Blazor implementation may experience performance degradation with long conversations (100+ messages) due to lack of virtualization and render optimization. This epic adds virtual scrolling, lazy loading, render optimization, and asset optimization for production-quality performance.

## Goals

1. Implement virtual scrolling for message list
2. Add lazy loading for older messages and large content
3. Optimize component rendering to minimize re-renders
4. Optimize asset loading (CSS, fonts, JS)

## Tasks

### Virtual Scrolling
- [x] Implement `<Virtualize>` for message list:
  - [x] Replace foreach with Virtualize component for >50 messages
  - [x] Configure ItemSize for estimated heights (100px)
  - [x] Handle dynamic message heights with generous estimate
- [x] Create placeholder component for loading items:
  - [x] SkeletonLoader component (from Epic 04-004) matches message shape
  - [x] Displayed while items load into view
- [x] Handle dynamic message heights:
  - [x] Messages vary in height (text length, tools, diffs)
  - [x] Using generous estimated height (100px) for simplicity
- [x] Maintain scroll position behavior:
  - [x] Auto-scroll to bottom for new messages via ScrollToBottomAsync
  - [x] JS interop for smooth scrolling
- [x] Configure overscan:
  - [x] OverscanCount=5 for smooth scrolling
  - [x] Balanced between smoothness and performance

### Lazy Loading
- [x] Lazy load older messages:
  - [x] Virtualization only renders visible items (natural lazy loading)
  - [x] VirtualizationThreshold=50 - small lists render normally
- [x] Lazy load large diff content:
  - [x] Collapsible component (from Epic 04-004) handles large diffs
  - [x] Content loads on expand
- [x] Lazy load file icons (if using sprite):
  - [x] Using inline SVGs for critical icons
  - [x] MudBlazor icons for standard ones
- [x] Lazy load syntax highlighting (if client-side):
  - [x] Prism.js loaded with defer attribute
  - [x] Highlighting triggered on component render

### Render Optimization
- [x] Add `ShouldRender()` to components:
  - [x] ChatMessage: only re-render if message data changes
  - [x] ChatInput: only re-render if input/state changes
  - [x] Tool displays integrated in ChatMessage with ShouldRender
- [x] Minimize state changes triggering re-renders:
  - [x] ShouldRender checks previous state
  - [x] Careful use of StateHasChanged()
- [x] Use `@key` directive for list items:
  - [x] Key by message ID in both virtualized and non-virtualized paths
  - [x] Helps Blazor track item identity
  - [x] Reduces unnecessary DOM updates
- [ ] Profile with browser dev tools:
  - [ ] Identify slow renders (manual testing recommended)
  - [ ] Find unnecessary re-renders
  - [ ] Measure component render times
- [x] Fix identified performance bottlenecks:
  - [x] Conditional virtualization based on message count
  - [x] ShouldRender optimization on key components

### Asset Optimization
- [x] Ensure CSS is minified in production:
  - [x] .NET build pipeline handles CSS minification
  - [x] Using CDN for external resources
- [x] Optimize font loading:
  - [x] Use `font-display: swap` on all Google Fonts
  - [x] Preload critical fonts (Geist Sans/Mono)
  - [x] Preconnect to font CDNs
- [x] Consider CSS splitting (if bundle large):
  - [x] Theme CSS separated into logical files
  - [x] Component-scoped CSS in Blazor components
- [x] Lazy load non-critical JS:
  - [x] Prism.js loaded with defer attribute
  - [x] All language plugins deferred

### Performance Testing
- [ ] Test with large session (100+ messages):
  - [ ] Measure initial load time
  - [ ] Measure scroll performance (fps)
  - [ ] Measure memory usage
- [ ] Test with many tool calls:
  - [ ] Multiple diff displays
  - [ ] Many code blocks
  - [ ] Large file trees
- [ ] Test on lower-end devices:
  - [ ] Throttle CPU in dev tools
  - [ ] Test on actual mobile device if possible
- [ ] Document performance benchmarks:
  - [ ] Before/after metrics
  - [ ] Acceptable thresholds

### Memory Management
- [x] Monitor memory usage:
  - [x] Standard Blazor patterns followed
  - [x] Proper Dispose implemented on components
- [x] Dispose resources properly:
  - [x] Unsubscribe from events in Dispose()
  - [x] CancellationTokenSource properly disposed
  - [x] IDisposable implemented on components with subscriptions
- [x] Limit cached items:
  - [x] Virtualization naturally limits rendered DOM elements
  - [x] Static MarkdownPipeline reused across instances

## Acceptance Criteria

- Virtual scrolling works smoothly for 100+ messages
- Initial page load is acceptable (<2s for typical session)
- Scroll performance is smooth (60fps target)
- Memory usage stays reasonable (no runaway growth)
- Older messages load on scroll without blocking UI
- Large diffs load lazily without performance impact
- No unnecessary re-renders detected in profiling
- Assets are optimized for production (minified, compressed)

## Dependencies

- Epic 03-006: Component Styling Alignment (components must be structured first)
- Epic 04-004: Edge Case Handling (loading states for lazy loading)

## References

- Blazor Virtualize documentation: https://learn.microsoft.com/en-us/aspnet/core/blazor/components/virtualization
- Blazor performance best practices: https://learn.microsoft.com/en-us/aspnet/core/blazor/performance
- [Component Mapping](/src/opencode-dotnet/docs/opencode-analysis/component-mapping.md) - Virtual List section

## Implementation Notes

### Files Modified:

**`examples/AgUi.IDE.BlazorServer/Components/Shared/Chat/ChatPanel.razor`**
- Added Virtualize component with conditional activation (>50 messages)
- Created `MessageWithIndex` record for typed virtualization
- Configured ItemSize=100, OverscanCount=5 for smooth scrolling
- Implemented ScrollToBottomAsync with JS interop
- Added @key directive to message items in both paths

**`examples/AgUi.IDE.BlazorServer/Components/Shared/Chat/ChatMessage.razor`**
- Added ShouldRender() to prevent unnecessary re-renders
- Tracks previous message state (content, parts count, streaming)
- Only re-renders when actual message data changes

**`examples/AgUi.IDE.BlazorServer/Components/Shared/Chat/ChatInput.razor`**
- Added ShouldRender() for render optimization
- Tracks IsDisabled and input text changes

**`examples/AgUi.IDE.BlazorServer/Components/App.razor`**
- Added preconnect for cdn.jsdelivr.net
- Added font preloading for Geist Sans and Geist Mono
- Added defer attribute to all Prism.js script tags

### Key Optimizations:
1. **Conditional Virtualization**: Only activates for >50 messages to avoid overhead on small lists
2. **ShouldRender**: ChatMessage compares previous state to prevent wasteful re-renders
3. **Font Preloading**: Critical fonts preloaded for faster initial render
4. **Deferred JS**: Prism.js loads after critical resources
5. **@key Directive**: Helps Blazor efficiently diff list items

## Effort Estimate

2-3 hours
