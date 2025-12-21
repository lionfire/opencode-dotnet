# Change 06: Blazor Hybrid Desktop App (MAUI)

**Type**: feature
**Created**: 2025-12-12
**Status**: Active
**Related Spec**: None

## Description

Create AgUi.IDE.Desktop as native Windows/Mac app with MAUI, bundle opencode serve, build installers

## Scope

- [ ] Create .NET MAUI Blazor Hybrid project (AgUi.IDE.Desktop)
- [ ] Integrate Blazor components into MAUI shell
- [ ] Bundle OpenCode serve into application
- [ ] Implement OpenCode process management
- [ ] Create Windows installer (.msi)
- [ ] Create Mac installer (.dmg)
- [ ] Configure app signing and distribution
- [ ] Test on Windows and Mac
- [ ] Create user documentation

## Success Criteria

- [ ] MAUI project builds for Windows and Mac
- [ ] Blazor components render in MAUI desktop app
- [ ] OpenCode server starts automatically on app launch
- [ ] Full IDE functionality works in desktop app
- [ ] Windows installer creates working application
- [ ] Mac installer creates working application
- [ ] App can be signed and distributed
- [ ] No console errors during use

## Notes

References:
- .NET MAUI docs: https://learn.microsoft.com/en-us/dotnet/maui/
- Blazor Hybrid: https://learn.microsoft.com/en-us/aspnet/core/blazor/hybrid/
- MAUI packaging: https://learn.microsoft.com/en-us/dotnet/maui/deployment/
- OpenCode bundling: Embed binary in resources
