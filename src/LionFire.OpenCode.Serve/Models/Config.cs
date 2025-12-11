using System.Text.Json;
using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Represents the OpenCode server configuration.
/// </summary>
/// <param name="Username">The current username.</param>
/// <param name="Version">The version of the OpenCode server (if available).</param>
/// <param name="Providers">The available AI providers.</param>
/// <param name="DefaultProvider">The default provider ID.</param>
public record OpenCodeConfig(
    [property: JsonPropertyName("username")] string? Username = null,
    [property: JsonPropertyName("version")] string? Version = null,
    [property: JsonPropertyName("providers")] IReadOnlyList<Provider>? Providers = null,
    [property: JsonPropertyName("defaultProvider")] string? DefaultProvider = null
)
{
    /// <summary>
    /// Additional properties not explicitly mapped.
    /// </summary>
    [JsonExtensionData]
    public Dictionary<string, JsonElement>? ExtensionData { get; init; }
}

/// <summary>
/// Represents an AI provider configuration.
/// </summary>
/// <param name="Id">The unique identifier of the provider.</param>
/// <param name="Name">The display name of the provider.</param>
/// <param name="Models">The models available from this provider.</param>
/// <param name="Available">Whether the provider is currently available.</param>
public record Provider(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("models")] IReadOnlyList<Model>? Models = null,
    [property: JsonPropertyName("available")] bool Available = true
);

/// <summary>
/// Represents an AI model.
/// </summary>
/// <param name="Id">The unique identifier of the model.</param>
/// <param name="Name">The display name of the model.</param>
/// <param name="Provider">The provider ID this model belongs to.</param>
/// <param name="MaxTokens">The maximum context window size in tokens.</param>
/// <param name="SupportsFunctions">Whether the model supports function calling.</param>
public record Model(
    [property: JsonPropertyName("id")] string Id,
    [property: JsonPropertyName("name")] string Name,
    [property: JsonPropertyName("provider")] string? Provider = null,
    [property: JsonPropertyName("maxTokens")] int? MaxTokens = null,
    [property: JsonPropertyName("supportsFunctions")] bool SupportsFunctions = false
);

/// <summary>
/// Result from a health check (ping) operation.
/// </summary>
/// <param name="Status">The server status.</param>
/// <param name="Version">The server version.</param>
/// <param name="Uptime">The server uptime in seconds.</param>
public record HealthCheckResult(
    [property: JsonPropertyName("status")] string Status,
    [property: JsonPropertyName("version")] string? Version = null,
    [property: JsonPropertyName("uptime")] long? Uptime = null
);
