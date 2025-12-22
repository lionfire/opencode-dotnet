# AOT Example

This example demonstrates Native AOT (Ahead-of-Time) compilation with the OpenCode SDK.

## Overview

The OpenCode SDK supports Native AOT compilation through the use of System.Text.Json source generators. This enables:

- **Faster startup times** - No JIT compilation needed
- **Smaller binary size** - Unused code is trimmed away
- **Lower memory usage** - Reduced runtime overhead
- **Better deployment** - Single self-contained executable

## How It Works

The SDK uses `OpenCodeSerializerContext` - a source-generated JSON serializer context that includes all DTO types. This eliminates the need for reflection-based JSON serialization, which is incompatible with AOT.

Key implementation details:
- `[JsonSerializable]` attributes on all model types
- `JsonOptions.SourceGenerated` used for all serialization
- No reflection-based type discovery at runtime

## Building and Running

### Standard Build (JIT)

```bash
dotnet build
dotnet run
```

### AOT Publish (Native Binary)

For Windows:
```bash
dotnet publish -c Release -r win-x64 --self-contained
```

For Linux:
```bash
dotnet publish -c Release -r linux-x64 --self-contained
```

For macOS:
```bash
dotnet publish -c Release -r osx-x64 --self-contained
```

The published binary will be in:
`bin/Release/net8.0/{rid}/publish/AotExample(.exe)`

## Verifying AOT Compilation

After publishing, you can verify the binary is AOT-compiled:

1. **Check file size** - AOT binaries are typically 10-50MB due to included runtime
2. **Check startup time** - AOT binaries start nearly instantly
3. **No .deps.json** - Self-contained AOT binaries don't need dependency files
4. **Single executable** - One file contains everything

## Environment Variables

- `OPENCODE_URL` - Set the OpenCode server URL (default: `http://localhost:9123`)

## Troubleshooting

### Trimming Warnings

If you see trimming warnings during publish, they may indicate code that uses reflection and could fail at runtime. The OpenCode SDK is designed to avoid these patterns.

### Missing Types at Runtime

If you encounter `JsonException` about missing types, ensure all DTOs are registered in `OpenCodeSerializerContext`.

## Performance Benefits

Typical improvements with AOT:
- Startup time: 50-100ms vs 500-2000ms (5-20x faster)
- Memory usage: 20-50% reduction
- First request latency: Significantly reduced

## Further Reading

- [.NET Native AOT Deployment](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
- [System.Text.Json Source Generation](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation)
- [Trimming Self-Contained Apps](https://learn.microsoft.com/en-us/dotnet/core/deploying/trimming/trimming-options)
