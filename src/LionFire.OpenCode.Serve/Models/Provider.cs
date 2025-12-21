using System.Text.Json.Serialization;

namespace LionFire.OpenCode.Serve.Models;

/// <summary>
/// Response wrapper for the /provider endpoint.
/// </summary>
public record ProviderListResponse
{
    /// <summary>
    /// All available providers.
    /// </summary>
    [JsonPropertyName("all")]
    public List<Provider>? All { get; init; }
}

/// <summary>
/// Represents a model provider.
/// </summary>
public record Provider
{
    /// <summary>
    /// The provider identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The provider name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// Available models from this provider (as dictionary keyed by model ID).
    /// </summary>
    [JsonPropertyName("models")]
    public Dictionary<string, Model>? ModelsDict { get; init; }

    /// <summary>
    /// Gets the models as a list for convenience.
    /// </summary>
    [JsonIgnore]
    public List<Model>? Models => ModelsDict?.Values.ToList();

    /// <summary>
    /// Provider configuration.
    /// </summary>
    [JsonPropertyName("config")]
    public ProviderConfig? Config { get; init; }
}

/// <summary>
/// Provider configuration.
/// </summary>
public record ProviderConfig
{
    /// <summary>
    /// API base URL.
    /// </summary>
    [JsonPropertyName("baseUrl")]
    public string? BaseUrl { get; init; }

    /// <summary>
    /// API key.
    /// </summary>
    [JsonPropertyName("apiKey")]
    public string? ApiKey { get; init; }
}

/// <summary>
/// Represents a model.
/// </summary>
public record Model
{
    /// <summary>
    /// The model identifier.
    /// </summary>
    [JsonPropertyName("id")]
    public required string Id { get; init; }

    /// <summary>
    /// The model name.
    /// </summary>
    [JsonPropertyName("name")]
    public required string Name { get; init; }

    /// <summary>
    /// The provider ID.
    /// </summary>
    [JsonPropertyName("providerID")]
    public string? ProviderId { get; init; }

    /// <summary>
    /// Model status (e.g., "active", "deprecated").
    /// </summary>
    [JsonPropertyName("status")]
    public string? Status { get; init; }

    /// <summary>
    /// Model cost information.
    /// </summary>
    [JsonPropertyName("cost")]
    public ModelCost? Cost { get; init; }

    /// <summary>
    /// Model limits (context window, output).
    /// </summary>
    [JsonPropertyName("limit")]
    public ModelLimit? Limit { get; init; }

    /// <summary>
    /// Cost per million input tokens (convenience property).
    /// </summary>
    [JsonIgnore]
    public double? InputCost => Cost?.Input;

    /// <summary>
    /// Cost per million output tokens (convenience property).
    /// </summary>
    [JsonIgnore]
    public double? OutputCost => Cost?.Output;

    /// <summary>
    /// Maximum context window size (convenience property).
    /// </summary>
    [JsonIgnore]
    public int? ContextWindow => Limit?.Context;
}

/// <summary>
/// Model cost information.
/// </summary>
public record ModelCost
{
    /// <summary>
    /// Cost per million input tokens.
    /// </summary>
    [JsonPropertyName("input")]
    public double Input { get; init; }

    /// <summary>
    /// Cost per million output tokens.
    /// </summary>
    [JsonPropertyName("output")]
    public double Output { get; init; }

    /// <summary>
    /// Cache cost information.
    /// </summary>
    [JsonPropertyName("cache")]
    public ModelCacheCost? Cache { get; init; }
}

/// <summary>
/// Model cache cost information.
/// </summary>
public record ModelCacheCost
{
    /// <summary>
    /// Cost for cache reads.
    /// </summary>
    [JsonPropertyName("read")]
    public double Read { get; init; }

    /// <summary>
    /// Cost for cache writes.
    /// </summary>
    [JsonPropertyName("write")]
    public double Write { get; init; }
}

/// <summary>
/// Model limits.
/// </summary>
public record ModelLimit
{
    /// <summary>
    /// Maximum context window size in tokens.
    /// </summary>
    [JsonPropertyName("context")]
    public int Context { get; init; }

    /// <summary>
    /// Maximum output size in tokens.
    /// </summary>
    [JsonPropertyName("output")]
    public int Output { get; init; }
}

/// <summary>
/// Provider authentication information.
/// </summary>
public record ProviderAuth
{
    /// <summary>
    /// The provider ID.
    /// </summary>
    [JsonPropertyName("providerID")]
    public required string ProviderId { get; init; }

    /// <summary>
    /// Authentication method information.
    /// </summary>
    [JsonPropertyName("method")]
    public ProviderAuthMethod? Method { get; init; }
}

/// <summary>
/// Provider authentication method.
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ProviderAuthMethodApiKey), "api_key")]
[JsonDerivedType(typeof(ProviderAuthMethodOAuth), "oauth")]
public abstract record ProviderAuthMethod
{
    /// <summary>
    /// The authentication method type.
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }
}

/// <summary>
/// API key authentication method.
/// </summary>
public record ProviderAuthMethodApiKey : ProviderAuthMethod
{
}

/// <summary>
/// OAuth authentication method.
/// </summary>
public record ProviderAuthMethodOAuth : ProviderAuthMethod
{
    /// <summary>
    /// OAuth authorization URL.
    /// </summary>
    [JsonPropertyName("authorizationUrl")]
    public string? AuthorizationUrl { get; init; }
}
