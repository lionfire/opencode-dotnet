# Epic 04-001: Animation System

## Overview

Port OpenCode's animation system for smooth, professional interactions. The current Blazor implementation lacks most animations present in OpenCode: fade-up stagger for list items, collapsible height animations, arrow rotation indicators, and pulse opacity for loading states. This epic adds these animations while respecting user preferences for reduced motion.

## Goals

1. Create CSS keyframes for all OpenCode animations
2. Implement fade-up stagger animation for list items
3. Implement collapsible height animation for expandable sections
4. Implement arrow rotation animation for collapsible indicators
5. Implement pulse opacity animation for loading states
6. Respect `prefers-reduced-motion` user preference

## Tasks

### Animation CSS Setup
- [x] Create `/wwwroot/css/animations.css` with keyframes
- [x] Add import to main CSS bundle

### Fade Up Stagger Animation
- [x] Define `@keyframes fadeUp`:
  ```css
  @keyframes fadeUp {
    from { opacity: 0; transform: translateY(5px); }
    to { opacity: 1; transform: translateY(0); }
  }
  ```
- [x] Create utility class `.animate-fade-up`:
  - [x] Animation timing (200-300ms)
  - [x] Ease-out timing function
- [x] Implement staggered delay for list items:
  - [x] CSS custom property for delay index
  - [x] Calculate delay: `calc(var(--stagger-index) * 50ms)`
- [x] Apply to message list on load:
  - [x] Set stagger index on each message
  - [x] Trigger animation on initial render
- [x] Apply to tool result lists

### Collapsible Height Animation
- [x] Implement smooth height transitions:
  - [x] CSS `max-height` approach with large value
  - [x] Or JS-based height calculation for precision
- [x] Define `.collapsible-content` styles:
  - [x] Overflow hidden during animation
  - [x] Transition on max-height (300ms ease)
- [x] Apply to Collapsible component:
  - [x] Integrate with component from Epic 03-003
  - [x] Handle dynamic content height
- [x] Handle content that changes height:
  - [x] Re-calculate on content update

### Arrow Rotation Animation
- [x] Define arrow rotation transform:
  - [x] Default state: 0 degrees (pointing right or down)
  - [x] Expanded state: 90 or 180 degrees
- [x] Create `.arrow-indicator` styles:
  - [x] Transition on transform (200ms ease)
  - [x] Transform-origin center
- [x] Apply to Collapsible triggers:
  - [x] Chevron icon rotation
  - [x] Smooth rotation on toggle

### Pulse Opacity Animation
- [x] Define `@keyframes pulse-opacity`:
  ```css
  @keyframes pulse-opacity {
    0%, 100% { opacity: 0.4; }
    50% { opacity: 1; }
  }
  ```
- [x] Create utility class `.animate-pulse`:
  - [x] Animation duration (1.5s)
  - [x] Infinite iteration
  - [x] Ease-in-out timing
- [x] Apply to loading/thinking indicators:
  - [x] Thinking dots
  - [x] Loading spinners
  - [x] Tool execution states

### Integration with Components
- [x] Add animation to message entry:
  - [x] New messages fade in
  - [x] Scroll into view with animation
- [x] Add animation to tool results:
  - [x] Tool output fades in on completion
- [x] Add animation to error states:
  - [x] Subtle shake or fade for errors

### Reduced Motion Support
- [x] Add `prefers-reduced-motion` media query:
  ```css
  @media (prefers-reduced-motion: reduce) {
    *, *::before, *::after {
      animation-duration: 0.01ms !important;
      animation-iteration-count: 1 !important;
      transition-duration: 0.01ms !important;
    }
  }
  ```
- [x] Provide reduced-motion alternatives:
  - [x] Instant display instead of fade
  - [x] No stagger delay
  - [x] Simple expand/collapse without animation
- [x] Test with reduced motion preference enabled

## Acceptance Criteria

- fadeUp animation works on list items with staggered delays
- Collapsible height animates smoothly (no jumps or flickers)
- Arrow indicators rotate smoothly on toggle (200ms)
- Pulse animation works for loading states
- Message entry has smooth fade-in animation
- Animations are disabled when user prefers reduced motion
- No animation jank or performance issues

## Implementation Notes

### Files Created:
- `src/LionFire.OpenCode.Blazor/wwwroot/css/animations.css` - Complete animation system with:
  - Keyframes: fadeUp, fadeIn, pulseOpacity, spin, shake, blink, scaleIn, slideDown
  - Utility classes: .animate-fade-up, .animate-pulse, .animate-spin, etc.
  - Stagger animation support (.stagger-item with CSS custom property)
  - Collapsible height animation (.collapsible-animate)
  - Arrow rotation (.chevron-indicator)
  - Thinking dots indicator
  - Skeleton loading animation
  - Full reduced motion support via @media query

### Files Modified:
- `src/LionFire.OpenCode.Blazor/wwwroot/css/opencode-base.css` - Added import for animations.css
- `src/LionFire.OpenCode.Blazor/Components/Shared/Collapsible.razor` - Added chevron-indicator and collapsible-animate classes
- `examples/AgUi.IDE.BlazorServer/Components/Shared/Chat/ChatMessage.razor`:
  - Added stagger animation via CSS custom property --stagger-index
  - Added message-enter and tool-result-enter animation classes
  - Added thinking-dots indicator with pulse animation
  - Added reduced motion support in scoped styles
  - Added StaggerIndex parameter
- `examples/AgUi.IDE.BlazorServer/Components/Shared/Chat/ChatPanel.razor` - Passes stagger index to ChatMessage

## Dependencies

- Epic 03-001: CSS Variable System Overhaul (animation timing may use variables)
- Epic 03-003: Tool Registry Pattern (Collapsible component for height animation)

## References

- [Styling Comparison](/src/opencode-dotnet/docs/opencode-analysis/styling-comparison.md) - Animation System section
- OpenCode animation files: search for keyframes in `packages/web/ui/styles/`

## Effort Estimate

2 hours
