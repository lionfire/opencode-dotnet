namespace LionFire.OpenCode.Serve.Internal;

/// <summary>
/// Contains the API endpoint paths for the OpenCode server.
/// </summary>
internal static class ApiEndpoints
{
    // Health - OpenCode doesn't have a dedicated health endpoint, so we use /config
    public const string Health = "/config";

    // Sessions
    public const string Sessions = "/session";
    public static string Session(string id) => $"/session/{id}";
    public static string SessionsWithDirectory(string? directory) =>
        string.IsNullOrEmpty(directory) ? Sessions : $"{Sessions}?directory={Uri.EscapeDataString(directory)}";
    public static string SessionFork(string id) => $"/session/{id}/fork";
    public static string SessionAbort(string id) => $"/session/{id}/abort";
    public static string SessionShare(string id) => $"/session/{id}/share";
    public static string SessionUnshare(string id) => $"/session/{id}/unshare";

    // Messages
    public static string Messages(string sessionId) => $"/session/{sessionId}/message";
    public static string Message(string sessionId, string messageId) => $"/session/{sessionId}/message/{messageId}";
    public static string MessagesStream(string sessionId) => $"/session/{sessionId}/event";

    // Tools (experimental endpoints)
    public const string Tools = "/experimental/tool";
    public const string ToolIds = "/experimental/tool/ids";

    // Permissions
    public static string Permission(string sessionId, string permissionId) => $"/session/{sessionId}/permissions/{permissionId}";

    // Files (instance-level, not session-level)
    public static string Files(string? path = null) =>
        string.IsNullOrEmpty(path) ? "/file" : $"/file?path={Uri.EscapeDataString(path)}";
    public static string FileContent(string path) => $"/file/content?path={Uri.EscapeDataString(path)}";
    public static string FindText(string pattern) => $"/find?pattern={Uri.EscapeDataString(pattern)}";
    public static string FindFiles(string query) => $"/find/file?query={Uri.EscapeDataString(query)}";

    // Commands
    public static string Commands(string sessionId) => $"/sessions/{sessionId}/commands";

    // Configuration
    public const string Config = "/config";
    public const string Providers = "/providers";
    public const string Models = "/models";
}
