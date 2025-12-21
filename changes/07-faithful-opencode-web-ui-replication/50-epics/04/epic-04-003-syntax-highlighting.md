# Epic 04-003: Syntax Highlighting

## Overview

Add code syntax highlighting matching OpenCode's style. The current Blazor implementation renders code blocks via Markdig without syntax highlighting. OpenCode uses a full syntax highlighting system with theme-aware token colors. This epic adds syntax highlighting for common programming languages with proper token CSS variables.

## Goals

1. Define syntax token CSS variables for all token types
2. Evaluate and implement a syntax highlighting approach
3. Support common programming languages (7+ languages)
4. Match OpenCode's token color palette
5. Ensure highlighting works in both light and dark modes

## Tasks

### Syntax CSS Variables
- [x] Define `--syntax-*` CSS variables in theme file:
  ```css
  :root {
    --syntax-keyword: #ff79c6;      /* Control flow, declarations */
    --syntax-string: #f1fa8c;       /* String literals */
    --syntax-number: #bd93f9;       /* Numeric literals */
    --syntax-comment: #6272a4;      /* Comments */
    --syntax-function: #50fa7b;     /* Function names */
    --syntax-variable: #f8f8f2;     /* Variable names */
    --syntax-type: #8be9fd;         /* Type names */
    --syntax-operator: #ff79c6;     /* Operators */
    --syntax-punctuation: #f8f8f2;  /* Brackets, semicolons */
    --syntax-property: #66d9ef;     /* Object properties */
    --syntax-class: #8be9fd;        /* Class names */
    --syntax-constant: #bd93f9;     /* Constants */
    --syntax-tag: #ff79c6;          /* HTML/XML tags */
    --syntax-attribute: #50fa7b;    /* HTML/XML attributes */
  }
  ```
- [x] Add light mode variants for all syntax colors

### Evaluate Highlighting Approaches
- [x] **Option A: Prism.js (Recommended)** - SELECTED
  - [x] Pros: Easy setup, good language support, well-documented
  - [x] Cons: Client-side JS, may need theme customization
  - [x] Research Blazor integration patterns
- [x] **Option B: Highlight.js** - Not selected
- [x] **Option C: Shiki via WASM** - Not selected (too complex)
- [x] **Option D: Server-side with Markdig extension** - Not selected
- [x] Document decision and rationale: Prism.js selected for ease of integration, extensive language support, and theme customization via CSS variables

### Implement Chosen Approach

#### If Prism.js:
- [x] Add Prism.js library to project:
  - [x] CDN link (jsdelivr)
  - [x] Include in App.razor
- [x] Create custom Prism theme CSS:
  - [x] Map Prism classes to `--syntax-*` variables
  - [x] Example: `.token.keyword { color: var(--syntax-keyword); }`
- [x] Integrate with Markdig output:
  - [x] Ensure code blocks have language class
  - [x] Call `Prism.highlightAll()` after render
- [x] Add JS interop for highlighting:
  - [x] Call highlight on component render
  - [x] Re-highlight on content update

### Language Support
- [x] Ensure support for common languages:
  - [x] TypeScript / JavaScript (.ts, .tsx, .js, .jsx)
  - [x] C# (.cs)
  - [x] Python (.py)
  - [x] JSON (.json)
  - [x] Markdown (.md)
  - [x] Bash / Shell (.sh, .bash)
  - [x] YAML (.yml, .yaml)
  - [x] HTML (.html)
  - [x] CSS (.css)
  - [x] SQL (.sql)
  - [x] Rust (.rs)
  - [x] Go (.go)
  - [x] Diff (for diffs)
- [x] Test each language with representative samples

### Token Color Matching
- [x] Compare OpenCode token colors with implementation
- [x] Adjust colors to match OpenCode's palette
- [x] Ensure consistency across languages
- [x] Document token color mappings

### Inline Code Handling
- [x] Style inline code (backtick code):
  - [x] Background color only (no highlighting)
  - [x] Simple mono font styling (via markdown-content css)
- [x] Differentiate from code blocks

### Testing
- [x] Test with real code samples from sessions:
  - [x] C# code samples
  - [x] TypeScript samples
  - [x] JSON configuration
  - [x] Shell commands
- [x] Verify colors in dark mode
- [x] Verify colors in light mode
- [x] Test long code blocks
- [x] Test code with unusual tokens

## Acceptance Criteria

- Code blocks have syntax highlighting for recognized languages
- At least 7 common languages are supported
- Token colors match OpenCode's theme
- Highlighting works correctly in both light and dark modes
- Inline code is styled appropriately (distinct from code blocks)
- No significant performance impact from highlighting
- Highlighting updates when theme changes

## Implementation Notes

### Files Created:
- `src/LionFire.OpenCode.Blazor/wwwroot/css/prism-opencode.css` - Custom Prism theme using CSS variables:
  - Maps Prism token classes to --syntax-* variables
  - Language-specific overrides for C#, TypeScript, JSON, YAML, CSS, HTML, etc.
  - Selection styling
  - Diff highlighting support

- `src/LionFire.OpenCode.Blazor/wwwroot/js/prism-highlight.js` - JS interop helper:
  - PrismHighlight.highlightAll() - Highlight all code blocks
  - PrismHighlight.highlightElement() - Highlight specific element
  - PrismHighlight.getLanguageClass() - Language name mapping

### Files Modified:
- `examples/AgUi.IDE.BlazorServer/Components/App.razor`:
  - Added prism-opencode.css link
  - Added Prism.js core from CDN
  - Added language modules for 15 languages (C#, TypeScript, JavaScript, JSX, TSX, Python, JSON, YAML, Bash, SQL, CSS, HTML, Rust, Go, Diff)

- `examples/AgUi.IDE.BlazorServer/Components/Shared/Chat/ChatMessage.razor`:
  - Added OnAfterRenderAsync to call PrismHighlight.highlightAll()
  - Added GetLanguageClass() helper method
  - Updated code block rendering to include language classes
  - Implemented clipboard copy functionality

### Decision Rationale:
Prism.js was selected over alternatives because:
1. Easy CDN integration (no npm/bundler required)
2. Extensive language support (15+ languages included)
3. Theme customization via CSS classes
4. Lightweight (each language component ~1-3KB)
5. Well-documented and widely used

## Dependencies

- Epic 03-001: CSS Variable System Overhaul (syntax variables defined)
- Epic 03-006: Component Styling Alignment (code block structure ready)
- Epic 04-002: Light Mode Support (syntax colors for both modes)

## References

- [Styling Comparison](/src/opencode-dotnet/docs/opencode-analysis/styling-comparison.md) - Syntax Highlighting section
- Prism.js: https://prismjs.com/
- Highlight.js: https://highlightjs.org/
- Shiki: https://shiki.style/

## Effort Estimate

4-6 hours
