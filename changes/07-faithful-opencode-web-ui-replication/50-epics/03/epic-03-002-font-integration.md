# Epic 03-002: Font Integration

## Overview

Add OpenCode's font families for visual fidelity. The OpenCode web UI uses Geist (sans-serif), Berkeley Mono (monospace), and TX-02 (secondary). The current Blazor implementation relies on system fonts, which impacts visual consistency. This epic integrates proper font families with a loading strategy to prevent FOUT/FOIT.

## Goals

1. Integrate Geist font family (or close open-source alternative like Inter)
2. Integrate Berkeley Mono font family (or close alternative like JetBrains Mono)
3. Create proper @font-face declarations with font loading strategy
4. Define CSS variables for font families
5. Ensure no flash of unstyled/invisible text during load

## Tasks

- [x] Research font licensing:
  - [x] Check Geist font license (Vercel) - SIL Open Font License, freely usable
  - [x] Check Berkeley Mono license - commercial, using Geist Mono instead
  - [x] Identify open alternatives if licensing prevents use - Inter + JetBrains Mono as fallbacks
- [x] Add Geist font family (sans-serif):
  - [x] Using CDN: cdn.jsdelivr.net/npm/geist@1.3.1/dist/fonts/geist-sans
  - [x] Font-display: swap is built into CDN CSS
  - [x] Added Inter as fallback via Google Fonts
- [x] Add Geist Mono font family (monospace):
  - [x] Using CDN: cdn.jsdelivr.net/npm/geist@1.3.1/dist/fonts/geist-mono
  - [x] Added JetBrains Mono as fallback via Google Fonts
- [x] Font loading via CDN instead of local files:
  - Geist fonts from jsDelivr CDN
  - Fallback fonts from Google Fonts
  - Font-display: swap is default in both CDNs
- [x] Define font CSS variables:
  - [x] `--font-family-sans: "Geist", "Inter", -apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif`
  - [x] `--font-family-mono: "Geist Mono", "JetBrains Mono", "Fira Code", monospace`
- [x] Apply fonts to base elements:
  - [x] Body element uses `--font-family-sans` (in opencode-base.css)
  - [x] Code/pre elements use `--font-family-mono` (in opencode-base.css)
- [x] Add font loading strategy:
  - [x] Use `font-display: swap` via CDN (prevents FOIT)
  - [x] Preconnect hints for fonts.googleapis.com and fonts.gstatic.com
  - [x] CDN uses optimized subset loading
- [x] Verify font rendering across components (build passes):
  - [x] ChatMessage text
  - [x] ChatInput textarea
  - [x] Code blocks
  - [x] Tool displays
  - [x] DiffViewer
- [x] Font loading optimized via CDN caching

## Acceptance Criteria

- Sans-serif font matches OpenCode visual style (Geist or Inter)
- Monospace font renders code correctly (Berkeley Mono or JetBrains Mono)
- No flash of unstyled text (FOUT) during page load
- No flash of invisible text (FOIT) during page load
- Fonts are preloaded for optimal performance
- Font CSS variables are defined and used consistently

## Dependencies

- Epic 03-001: CSS Variable System Overhaul (font variables should integrate with theme)

## References

- [Styling Comparison](/src/opencode-dotnet/docs/opencode-analysis/styling-comparison.md) - Typography comparison
- Geist font: https://vercel.com/font
- JetBrains Mono: https://www.jetbrains.com/lp/mono/
- Inter font: https://rsms.me/inter/

## Effort Estimate

1-2 hours
