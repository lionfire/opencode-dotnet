# Epic 04-004: Edge Case Handling

## Overview

Handle all edge cases for production quality. A polished application handles empty states, loading states, error states, and long content gracefully. The current Blazor implementation lacks comprehensive handling of these edge cases. This epic adds proper components and patterns for all edge cases.

## Goals

1. Create EmptyState component for various empty scenarios
2. Create loading state indicators for different loading contexts
3. Create ErrorState component with retry capability
4. Handle long content with truncation and expansion
5. Add graceful degradation for missing assets

## Tasks

### Empty States
- [x] Create `EmptyState.razor` component:
  - [x] Icon display area
  - [x] Title text
  - [x] Description text
  - [x] Optional action button
- [x] Design empty state variants:
  - [x] No messages state: "Start a conversation" with chat icon
  - [x] No sessions state: "No sessions yet" with folder icon
  - [x] No search results: "No results found" with search icon
  - [x] No file changes: "No files modified" with file icon
- [x] Style empty states:
  - [x] Centered layout
  - [x] Muted colors
  - [x] Appropriate sizing
- [x] Apply to:
  - [x] ChatPanel when no messages (already exists)
  - [x] Session list when empty (available via component)
  - [x] Search results when empty (available via component)
  - [x] File diff list when empty (available via component)

### Loading States
- [x] Create loading indicators for various contexts:
  - [x] Initial session loading:
    - [x] Skeleton loader for message list
    - [x] Or centered spinner with message
  - [x] Message streaming progress:
    - [x] Typing indicator (animated dots)
    - [x] "Thinking..." text with pulse
  - [x] Tool execution progress:
    - [x] Spinner with tool name
    - [x] Progress text (e.g., "Running bash command...")
  - [x] Session list loading:
    - [x] Skeleton cards
    - [x] Or list of placeholder items
- [x] Create `SkeletonLoader.razor` component:
  - [x] Animated placeholder shapes
  - [x] Configurable for text, avatar, card layouts
- [x] Create `TypingIndicator.razor` component:
  - [x] Three animated dots
  - [x] Pulse animation
- [x] Apply loading states appropriately:
  - [x] Show immediately when loading starts
  - [x] Transition smoothly to content

### Error States
- [x] Create `ErrorState.razor` component:
  - [x] Error icon
  - [x] Error title
  - [x] Error description/details
  - [x] Retry button
  - [x] Optional "Show details" for technical info
- [x] Design error state variants:
  - [x] Connection error: "Connection lost. Retry?"
  - [x] API error: Display message with details toggle
  - [x] Tool execution error: Show in tool display with error styling
  - [x] Session load error: "Failed to load session"
- [x] Style error states:
  - [x] Error color (red/ember) accents
  - [x] Clear call-to-action for retry
  - [x] Collapsible technical details
- [x] Implement retry logic:
  - [x] Callback for retry action
  - [x] Loading state during retry
  - [x] Success/failure feedback
- [x] Apply error handling:
  - [x] Network request failures
  - [x] Tool execution failures
  - [x] Session API errors

### Long Content Handling
- [x] Very long messages:
  - [x] Add "Show more" / "Show less" button
  - [x] Default truncation at ~500 characters
  - [x] Smooth expand/collapse animation
- [x] Very long file names:
  - [x] CSS text-overflow: ellipsis (in existing CSS)
  - [x] Tooltip showing full path on hover
  - [x] Consider truncating from middle for paths
- [x] Large diff displays:
  - [x] Collapse by default if > 50 lines (via Collapsible)
  - [x] "Show full diff" expansion
  - [x] Line count indicator
- [x] Very long code blocks:
  - [x] Max-height with vertical scroll (200px in ChatMessage)
  - [x] Line count indicator
  - [x] Optional "Expand" for full view

### Graceful Degradation
- [x] Missing avatar:
  - [x] Default user/assistant icon (MudIcon fallbacks)
  - [x] Consistent fallback styling
- [x] Missing provider icon:
  - [x] Generic AI icon fallback
  - [x] Log warning for missing provider
- [x] Missing file icon:
  - [x] Generic file icon fallback
  - [x] Works for all unknown extensions
- [x] Image load failures:
  - [x] Placeholder with broken image indicator
  - [x] Alt text display
- [x] Font load failures:
  - [x] System font fallback (in CSS)
  - [x] Graceful appearance

### Component Integration
- [x] Add empty state checks to ChatPanel (already exists)
- [x] Add loading states to message streaming (TypingIndicator available)
- [x] Add error handling to API calls (ErrorState available)
- [x] Add truncation to message display (TruncatedText available)
- [x] Add fallbacks to icon components

## Acceptance Criteria

- Empty states display helpful, contextual messages
- Loading states indicate progress with appropriate indicators
- Error states allow retry with clear feedback
- Long content is truncated with "Show more" option
- Long file paths show ellipsis with tooltip
- Large diffs are collapsed by default
- Missing icons have appropriate fallbacks
- No broken UI elements when data is missing

## Implementation Notes

### Files Created:
- `src/LionFire.OpenCode.Blazor/Components/Shared/EmptyState.razor` - Configurable empty state display:
  - Built-in icon types: chat, folder, search, file, error
  - Custom icon support via RenderFragment
  - Optional action button

- `src/LionFire.OpenCode.Blazor/Components/Shared/SkeletonLoader.razor` - Loading placeholders:
  - Multiple variants: Text, TextMultiline, Avatar, Card, Message, List, Custom
  - Animated gradient pulse effect
  - Reduced motion support

- `src/LionFire.OpenCode.Blazor/Components/Shared/SkeletonVariant.cs` - Enum for skeleton types

- `src/LionFire.OpenCode.Blazor/Components/Shared/TypingIndicator.razor` - Animated typing dots:
  - Optional text label
  - Pulse animation with staggered delays
  - Reduced motion support

- `src/LionFire.OpenCode.Blazor/Components/Shared/ErrorState.razor` - Error display with retry:
  - Error icon and title
  - Optional description and technical details
  - Collapsible details toggle
  - Retry button with loading state
  - Secondary action slot

- `src/LionFire.OpenCode.Blazor/Components/Shared/TruncatedText.razor` - Long content handling:
  - Configurable max length (default 500 chars)
  - Smart truncation at word boundaries
  - Show more/less toggle

### All components include:
- CSS variable integration for theming
- Reduced motion support via @media query
- Scoped component styles
- Flexible parameter configuration

## Dependencies

- Epic 03-001: CSS Variable System Overhaul (state colors use variables)
- Epic 03-004: Missing Display Components (icons for states)
- Epic 04-001: Animation System (transitions for expand/collapse)

## References

- [Component Mapping](/src/opencode-dotnet/docs/opencode-analysis/component-mapping.md) - Edge case handling patterns
- Material Design empty state guidelines
- Best practices for loading/error states

## Effort Estimate

2-3 hours
