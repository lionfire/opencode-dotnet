# Backlog: Option 2 - OpenCode as Agent Framework Tools (Redesign)

**Status**: Deferred to v1.1+
**Priority**: Low
**Original Document**: `/docs/agent-framework/integration-possibilities/02-opencode-as-tools.md`

## Summary

Option 2 proposes exposing OpenCode operations as `AIFunction` tools that any Microsoft Agent Framework agent can invoke. This allows agents powered by OpenAI, Anthropic, Azure, etc. to delegate tasks to OpenCode.

## Problem

The original document was written for a **cloud API** that had specialized methods:
- `GenerateCodeAsync(request)`
- `ExplainCodeAsync(code, language)`
- `ReviewCodeAsync(code, focus)`
- `SuggestRefactoringAsync(code, goals)`

The **local server API** (`opencode serve`) is fundamentally different:
- Session-based conversations
- Generic message sending (`POST /session/{id}/message`)
- AI decides what to do based on the prompt
- No specialized code endpoints

## Redesign Considerations

### Adapted Tool Concepts

For the local server model, tools would look like:

```csharp
public class OpenCodeTools
{
    [Description("Send a prompt to an OpenCode session")]
    public async Task<string> SendPrompt(
        string prompt,
        string? sessionId = null,
        CancellationToken ct = default)
    {
        // Create or reuse session, send message, return response
    }

    [Description("Search for code patterns in the workspace")]
    public async Task<string> SearchCode(
        string pattern,
        string? path = null,
        CancellationToken ct = default)
    {
        // Use OpenCode's /search/text endpoint
    }

    [Description("List files in the workspace")]
    public async Task<string> ListFiles(
        string path,
        CancellationToken ct = default)
    {
        // Use OpenCode's /file/list endpoint
    }

    [Description("Read a file from the workspace")]
    public async Task<string> ReadFile(
        string path,
        CancellationToken ct = default)
    {
        // Use OpenCode's /file/read endpoint
    }
}
```

### Value Proposition Questions

1. **Double AI Layer**: If another agent (e.g., GPT-4) delegates to OpenCode (which uses Claude), is this efficient?
2. **Latency**: Tool calls add round-trips
3. **Use Cases**: When would you want another LLM deciding to delegate to OpenCode vs. using OpenCode directly?

### Potential Use Cases

1. **Orchestration**: A coordinator agent manages workflow, delegates code tasks to OpenCode
2. **Cost Optimization**: Cheaper model for planning, OpenCode for execution
3. **Multi-Model Workflows**: Different models for different expertise areas

## Decision

**Deferred to v1.1+** because:
1. Core SDK and Agent Framework integration (Option 1 + Option 4) are higher priority
2. Needs redesign for local server model
3. Unclear demand - wait for user feedback

## Next Steps (When Revisiting)

1. Validate use cases with community
2. Redesign tools for session-based API
3. Prototype and benchmark latency
4. Consider if `AsTools()` extension (from Option 4) covers this need
5. Write updated specification

## Related

- Option 1: OpencodeAgent (implemented in Phase 3)
- Option 4: Enhanced Client Interfaces (implemented in Phase 3)
- PRP: `/plan/10-specs/00-PRP/clarified-PRP.md`
