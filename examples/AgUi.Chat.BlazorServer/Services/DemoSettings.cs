namespace AgUi.Chat.BlazorServer.Services;

/// <summary>
/// Configuration settings for demo/development features.
/// </summary>
public class DemoSettings
{
    /// <summary>
    /// Whether the mock backend option is available.
    /// When false, only the real OpenCode backend can be used.
    /// </summary>
    public bool MockBackendEnabled { get; set; } = false;
}
