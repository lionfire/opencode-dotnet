# Change 04: Blazor WASM Minimal Sample

**Type**: feature
**Created**: 2025-12-12
**Status**: Active
**Related Spec**: None

## Description

Create AgUi.Chat.BlazorWasm with Client/Server split, test offline mode and reconnection

## Scope

- [ ] Create AgUi.Chat.BlazorWasm client project
- [ ] Create AgUi.Chat.BlazorWasm server backend project
- [ ] Implement minimal chat UI in WASM client
- [ ] Implement offline mode detection
- [ ] Implement reconnection logic
- [ ] Test offline functionality
- [ ] Test reconnection after network outage
- [ ] Verify WASM bundle size is reasonable

## Success Criteria

- [ ] AgUi.Chat.BlazorWasm client builds and loads in browser
- [ ] Chat interface works while connected
- [ ] Offline mode detected and displayed
- [ ] Queued messages sent after reconnection
- [ ] No console errors during use
- [ ] Bundle size reasonable (< 5MB uncompressed)

## Notes

References:
- Blazor WASM docs: https://learn.microsoft.com/en-us/aspnet/core/blazor/hosting-models
- Service workers for offline: https://learn.microsoft.com/en-us/aspnet/core/blazor/progressive-web-app
- Testing offline: Browser DevTools Network throttling
