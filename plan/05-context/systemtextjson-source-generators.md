# System.Text.Json Source Generators

## Brief Description

System.Text.Json source generators are compile-time code generation tools that create optimized JSON serialization code for .NET types. They eliminate runtime reflection, enable Native AOT compilation, and improve startup performance.

## Relevance to Project

**Why this matters for our project**:
- Enables Native AOT scenarios (Phase 2 requirement)
- Improves startup time by eliminating reflection
- Reduces memory allocation during serialization
- Required for .NET trimming and AOT deployment

**Where it's used in our architecture**:
- All DTOs (Session, Message, Tool, etc.) will use source-generated serialization
- JsonSerializerContext provides metadata at compile-time
- Phase 2 migration from reflection-based to source-generated

**Impact on implementation**:
- Must add [JsonSerializable] attributes to context class
- DTOs must be partial classes if using source generation
- Compilation time increases slightly (generates code)

## Interoperability Points

**Integrates with**:
- System.Text.Json: Uses same APIs, but with generated code instead of reflection
- Native AOT: Required for AOT compilation (reflection doesn't work in AOT)
- Trimming: Allows aggressive assembly trimming

**Data flow**:
1. Compile-time: Source generator analyzes types
2. Generate: Creates serialization code
3. Runtime: Uses generated code (no reflection)

## Considerations and Watch Points

### Best Practices

**For this project specifically**:
- Create OpencodeJsonContext : JsonSerializerContext
- Mark all DTOs with [JsonSerializable(typeof(Session))], etc.
- Use [JsonSourceGenerationOptions] for global settings
- Test both reflection and source-generated paths in Phase 1-2 transition

**Common patterns**:
```csharp
[JsonSerializable(typeof(Session))]
[JsonSerializable(typeof(Message))]
[JsonSourceGenerationOptions(WriteIndented = false,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
internal partial class OpencodeJsonContext : JsonSerializerContext
{
}
```

### Common Pitfalls

- **Forgetting to register types**: Each DTO needs [JsonSerializable]
- **Polymorphism challenges**: MessagePart hierarchy needs special handling
- **Breaking changes**: Migration from reflection can expose serialization bugs

## References

- Source Generators Overview: https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation
- Native AOT: https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/
