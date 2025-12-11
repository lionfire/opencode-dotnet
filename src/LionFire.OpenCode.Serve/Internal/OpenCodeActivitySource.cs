using System.Diagnostics;

namespace LionFire.OpenCode.Serve.Internal;

/// <summary>
/// Provides an <see cref="ActivitySource"/> for OpenCode client operations.
/// </summary>
public static class OpenCodeActivitySource
{
    /// <summary>
    /// The name of the activity source.
    /// </summary>
    public const string Name = "LionFire.OpenCode.Serve";

    /// <summary>
    /// The version of the activity source.
    /// </summary>
    public const string Version = "0.1.0";

    /// <summary>
    /// Gets the activity source for OpenCode operations.
    /// </summary>
    public static ActivitySource Source { get; } = new(Name, Version);

    /// <summary>
    /// Activity names for various operations.
    /// </summary>
    public static class Activities
    {
        public const string Ping = "opencode.ping";
        public const string CreateSession = "opencode.session.create";
        public const string GetSession = "opencode.session.get";
        public const string ListSessions = "opencode.session.list";
        public const string DeleteSession = "opencode.session.delete";
        public const string ForkSession = "opencode.session.fork";
        public const string AbortSession = "opencode.session.abort";
        public const string ShareSession = "opencode.session.share";
        public const string SendMessage = "opencode.message.send";
        public const string SendMessageStreaming = "opencode.message.send.streaming";
        public const string GetMessages = "opencode.message.list";
        public const string GetMessage = "opencode.message.get";
        public const string GetTools = "opencode.tool.list";
        public const string GetTool = "opencode.tool.get";
        public const string ApproveTool = "opencode.tool.approve";
        public const string DenyTool = "opencode.tool.deny";
        public const string ListFiles = "opencode.file.list";
        public const string GetFileContent = "opencode.file.get";
        public const string SearchFiles = "opencode.file.search";
        public const string ApplyChanges = "opencode.file.apply";
        public const string ExecuteCommand = "opencode.command.execute";
        public const string GetConfig = "opencode.config.get";
        public const string GetProviders = "opencode.provider.list";
        public const string GetModels = "opencode.model.list";
    }

    /// <summary>
    /// Tag names for activity attributes.
    /// </summary>
    public static class Tags
    {
        public const string SessionId = "opencode.session.id";
        public const string MessageId = "opencode.message.id";
        public const string ToolId = "opencode.tool.id";
        public const string FilePath = "opencode.file.path";
        public const string Command = "opencode.command";
        public const string StatusCode = "http.status_code";
        public const string ErrorType = "error.type";
    }

    /// <summary>
    /// Starts an activity for the specified operation.
    /// </summary>
    /// <param name="operationName">The name of the operation.</param>
    /// <returns>The started activity, or null if tracing is not enabled.</returns>
    public static Activity? StartActivity(string operationName)
    {
        return Source.StartActivity(operationName, ActivityKind.Client);
    }

    /// <summary>
    /// Starts an activity with a session ID tag.
    /// </summary>
    public static Activity? StartSessionActivity(string operationName, string sessionId)
    {
        var activity = StartActivity(operationName);
        activity?.SetTag(Tags.SessionId, sessionId);
        return activity;
    }

    /// <summary>
    /// Starts an activity with session and message ID tags.
    /// </summary>
    public static Activity? StartMessageActivity(string operationName, string sessionId, string? messageId = null)
    {
        var activity = StartSessionActivity(operationName, sessionId);
        if (messageId != null)
        {
            activity?.SetTag(Tags.MessageId, messageId);
        }
        return activity;
    }
}
