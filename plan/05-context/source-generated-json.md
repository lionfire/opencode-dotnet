# Context: Source-Generated JSON in .NET

## Overview

Using System.Text.Json source generators for AOT-compatible, high-performance JSON serialization.

## Why Source Generators?

### Benefits
1. **No Reflection** - Works with AOT/trimming
2. **Faster Startup** - No runtime code generation
3. **Better Performance** - Pre-compiled serialization
4. **Smaller Binaries** - Only generates needed code

## Basic Setup

### JsonSerializerContext
```csharp
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull)]
[JsonSerializable(typeof(GenerateCodeRequest))]
[JsonSerializable(typeof(GenerateCodeResponse))]
[JsonSerializable(typeof(ErrorResponse))]
[JsonSerializable(typeof(CodeChunk))]
internal partial class OpencodeJsonContext : JsonSerializerContext
{
}
```

### Usage
```csharp
// Serialization
var json = JsonSerializer.Serialize(request, OpencodeJsonContext.Default.GenerateCodeRequest);

// Deserialization
var response = JsonSerializer.Deserialize(json, OpencodeJsonContext.Default.GenerateCodeResponse);
```

## Project Configuration

### csproj Settings
```xml
<PropertyGroup>
    <EnableTrimAnalyzer>true</EnableTrimAnalyzer>
    <IsAotCompatible>true</IsAotCompatible>
</PropertyGroup>
```

## Common Patterns

### Enums as Strings
```csharp
[JsonConverter(typeof(JsonStringEnumConverter<ProgrammingLanguage>))]
public enum ProgrammingLanguage
{
    [JsonStringEnumMemberName("csharp")]
    CSharp,

    [JsonStringEnumMemberName("javascript")]
    JavaScript,

    // ...
}
```

### Records with Required Properties
```csharp
public record GenerateCodeRequest
{
    [JsonPropertyName("prompt")]
    public required string Prompt { get; init; }

    [JsonPropertyName("language")]
    public ProgrammingLanguage? Language { get; init; }

    [JsonPropertyName("model")]
    public string? Model { get; init; }
}
```

### Response with Nested Types
```csharp
public record GenerateCodeResponse
{
    [JsonPropertyName("code")]
    public required string Code { get; init; }

    [JsonPropertyName("usage")]
    public TokenUsage? Usage { get; init; }
}

public record TokenUsage
{
    [JsonPropertyName("prompt_tokens")]
    public int PromptTokens { get; init; }

    [JsonPropertyName("completion_tokens")]
    public int CompletionTokens { get; init; }

    [JsonPropertyName("total_tokens")]
    public int TotalTokens { get; init; }
}
```

## Error Response Parsing

### Error Model
```csharp
public record ErrorResponse
{
    [JsonPropertyName("error")]
    public required ApiError Error { get; init; }
}

public record ApiError
{
    [JsonPropertyName("code")]
    public string? Code { get; init; }

    [JsonPropertyName("message")]
    public required string Message { get; init; }

    [JsonPropertyName("details")]
    public JsonElement? Details { get; init; }
}
```

### Safe Parsing
```csharp
public static ErrorResponse? TryParseError(string json)
{
    try
    {
        return JsonSerializer.Deserialize(json, OpencodeJsonContext.Default.ErrorResponse);
    }
    catch (JsonException)
    {
        return null;
    }
}
```

## HttpClient Integration

### Sending JSON
```csharp
public async Task<HttpResponseMessage> SendJsonAsync<T>(
    HttpMethod method,
    string path,
    T content,
    JsonTypeInfo<T> typeInfo,
    CancellationToken cancellationToken)
{
    var request = new HttpRequestMessage(method, path)
    {
        Content = JsonContent.Create(content, typeInfo)
    };

    return await _httpClient.SendAsync(request, cancellationToken);
}
```

### Reading JSON Response
```csharp
public async Task<T> ReadJsonAsync<T>(
    HttpResponseMessage response,
    JsonTypeInfo<T> typeInfo,
    CancellationToken cancellationToken)
{
    await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
    return await JsonSerializer.DeserializeAsync(stream, typeInfo, cancellationToken)
        ?? throw new InvalidOperationException("Deserialization returned null");
}
```

## Testing

### Roundtrip Test
```csharp
[Fact]
public void Request_ShouldRoundtrip()
{
    var request = new GenerateCodeRequest
    {
        Prompt = "Write hello world",
        Language = ProgrammingLanguage.CSharp
    };

    var json = JsonSerializer.Serialize(request, OpencodeJsonContext.Default.GenerateCodeRequest);
    var deserialized = JsonSerializer.Deserialize(json, OpencodeJsonContext.Default.GenerateCodeRequest);

    deserialized.Should().BeEquivalentTo(request);
}
```

## References

- [System.Text.Json Source Generation](https://learn.microsoft.com/en-us/dotnet/standard/serialization/system-text-json/source-generation)
- [AOT Compatibility](https://learn.microsoft.com/en-us/dotnet/core/deploying/native-aot/)
- [JsonSerializerContext](https://learn.microsoft.com/en-us/dotnet/api/system.text.json.serialization.jsonserializercontext)
