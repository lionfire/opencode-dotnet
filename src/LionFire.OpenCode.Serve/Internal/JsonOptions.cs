using System.Text.Json;
using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Internal;

/// <summary>
/// Provides default JSON serialization options for the OpenCode client.
/// </summary>
public static class JsonOptions
{
    private static JsonSerializerOptions? _default;
    private static JsonSerializerOptions? _sourceGenerated;

    /// <summary>
    /// Gets the default JSON serializer options for the OpenCode SDK (reflection-based).
    /// </summary>
    public static JsonSerializerOptions Default => _default ??= CreateDefaultOptions();

    /// <summary>
    /// Gets the source-generated JSON serializer options for AOT compatibility.
    /// </summary>
    public static JsonSerializerOptions SourceGenerated => _sourceGenerated ??= CreateSourceGeneratedOptions();

    /// <summary>
    /// Gets the source-generated serializer context for direct use.
    /// </summary>
    public static OpenCodeSerializerContext Context => OpenCodeSerializerContext.Default;

    private static JsonSerializerOptions CreateDefaultOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        return options;
    }

    private static JsonSerializerOptions CreateSourceGeneratedOptions()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false,
            TypeInfoResolver = OpenCodeSerializerContext.Default,
            Converters =
            {
                new JsonStringEnumConverter(JsonNamingPolicy.CamelCase)
            }
        };

        return options;
    }
}
