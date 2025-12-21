using System.Text.Json.Serialization;
using LionFire.OpenCode.Serve.Models;

namespace LionFire.OpenCode.Serve.Models.Events;

/// <summary>
/// Base class for all events from OpenCode SSE stream.
/// OpenCode sends events in format: {"type":"event.type","properties":{...}}
/// </summary>
[JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
[JsonDerivedType(typeof(ServerInstanceDisposedEvent), "server.instance.disposed")]
[JsonDerivedType(typeof(ServerConnectedEvent), "server.connected")]
[JsonDerivedType(typeof(SessionCreatedEvent), "session.created")]
[JsonDerivedType(typeof(SessionUpdatedEvent), "session.updated")]
[JsonDerivedType(typeof(SessionDeletedEvent), "session.deleted")]
[JsonDerivedType(typeof(SessionStatusEvent), "session.status")]
[JsonDerivedType(typeof(SessionIdleEvent), "session.idle")]
[JsonDerivedType(typeof(SessionCompactedEvent), "session.compacted")]
[JsonDerivedType(typeof(SessionDiffEvent), "session.diff")]
[JsonDerivedType(typeof(SessionErrorEvent), "session.error")]
[JsonDerivedType(typeof(MessageUpdatedEvent), "message.updated")]
[JsonDerivedType(typeof(MessageRemovedEvent), "message.removed")]
[JsonDerivedType(typeof(MessagePartUpdatedEvent), "message.part.updated")]
[JsonDerivedType(typeof(MessagePartRemovedEvent), "message.part.removed")]
[JsonDerivedType(typeof(PermissionUpdatedEvent), "permission.updated")]
[JsonDerivedType(typeof(PermissionRepliedEvent), "permission.replied")]
[JsonDerivedType(typeof(FileEditedEvent), "file.edited")]
[JsonDerivedType(typeof(FileWatcherUpdatedEvent), "file.watcher.updated")]
[JsonDerivedType(typeof(TodoUpdatedEvent), "todo.updated")]
[JsonDerivedType(typeof(PtyCreatedEvent), "pty.created")]
[JsonDerivedType(typeof(PtyUpdatedEvent), "pty.updated")]
[JsonDerivedType(typeof(PtyExitedEvent), "pty.exited")]
[JsonDerivedType(typeof(PtyDeletedEvent), "pty.deleted")]
[JsonDerivedType(typeof(LspUpdatedEvent), "lsp.updated")]
[JsonDerivedType(typeof(LspClientDiagnosticsEvent), "lsp.client.diagnostics")]
[JsonDerivedType(typeof(CommandExecutedEvent), "command.executed")]
[JsonDerivedType(typeof(VcsBranchUpdatedEvent), "vcs.branch.updated")]
[JsonDerivedType(typeof(InstallationUpdatedEvent), "installation.updated")]
[JsonDerivedType(typeof(InstallationUpdateAvailableEvent), "installation.update-available")]
[JsonDerivedType(typeof(TuiPromptAppendEvent), "tui.prompt.append")]
[JsonDerivedType(typeof(TuiCommandExecuteEvent), "tui.command.execute")]
[JsonDerivedType(typeof(TuiToastShowEvent), "tui.toast.show")]
public abstract record Event
{
    /// <summary>
    /// The event type (populated from JSON discriminator).
    /// </summary>
    [JsonPropertyName("type")]
    public string? Type { get; init; }
}

/// <summary>
/// Base class for global events (no directory context).
/// </summary>
public abstract record GlobalEvent : Event
{
}

// Server events
public record ServerInstanceDisposedEvent : GlobalEvent
{
    [JsonPropertyName("properties")]
    public ServerInstanceDisposedProperties? Properties { get; init; }
}

public record ServerInstanceDisposedProperties { }

public record ServerConnectedEvent : Event
{
    [JsonPropertyName("properties")]
    public ServerConnectedProperties? Properties { get; init; }
}

public record ServerConnectedProperties { }

// Session events
public record SessionCreatedEvent : Event
{
    [JsonPropertyName("properties")]
    public SessionCreatedProperties? Properties { get; init; }
}

public record SessionCreatedProperties
{
    [JsonPropertyName("session")]
    public Session? Session { get; init; }
}

public record SessionUpdatedEvent : Event
{
    [JsonPropertyName("properties")]
    public SessionUpdatedProperties? Properties { get; init; }
}

public record SessionUpdatedProperties
{
    [JsonPropertyName("session")]
    public Session? Session { get; init; }
}

public record SessionDeletedEvent : Event
{
    [JsonPropertyName("properties")]
    public SessionDeletedProperties? Properties { get; init; }
}

public record SessionDeletedProperties
{
    [JsonPropertyName("sessionID")]
    public string? SessionId { get; init; }
}

public record SessionStatusEvent : Event
{
    [JsonPropertyName("properties")]
    public SessionStatusProperties? Properties { get; init; }
}

public record SessionStatusProperties
{
    [JsonPropertyName("sessionID")]
    public string? SessionId { get; init; }

    [JsonPropertyName("status")]
    public SessionStatus? Status { get; init; }
}

public record SessionIdleEvent : Event
{
    [JsonPropertyName("properties")]
    public SessionIdleProperties? Properties { get; init; }
}

public record SessionIdleProperties
{
    [JsonPropertyName("sessionID")]
    public string? SessionId { get; init; }
}

public record SessionCompactedEvent : Event
{
    [JsonPropertyName("properties")]
    public SessionCompactedProperties? Properties { get; init; }
}

public record SessionCompactedProperties
{
    [JsonPropertyName("sessionID")]
    public string? SessionId { get; init; }
}

public record SessionDiffEvent : Event
{
    [JsonPropertyName("properties")]
    public SessionDiffProperties? Properties { get; init; }
}

public record SessionDiffProperties
{
    [JsonPropertyName("sessionID")]
    public string? SessionId { get; init; }

    [JsonPropertyName("diffs")]
    public List<FileDiff>? Diffs { get; init; }
}

public record SessionErrorEvent : Event
{
    [JsonPropertyName("properties")]
    public SessionErrorProperties? Properties { get; init; }
}

public record SessionErrorProperties
{
    [JsonPropertyName("sessionID")]
    public string? SessionId { get; init; }

    [JsonPropertyName("error")]
    public string? Error { get; init; }
}

// Message events - OpenCode sends: {"type":"message.updated","properties":{"info":{...message data...}}}
public record MessageUpdatedEvent : Event
{
    [JsonPropertyName("properties")]
    public MessageUpdatedProperties? Properties { get; init; }
}

public record MessageUpdatedProperties
{
    /// <summary>
    /// Message info from OpenCode (uses "info" not "message")
    /// </summary>
    [JsonPropertyName("info")]
    public MessageInfo? Info { get; init; }
}

/// <summary>
/// Message info as returned by OpenCode SSE events.
/// Uses existing Message model types from Models namespace.
/// </summary>
public record MessageInfo
{
    [JsonPropertyName("id")]
    public string? Id { get; init; }

    [JsonPropertyName("sessionID")]
    public string? SessionId { get; init; }

    [JsonPropertyName("role")]
    public string? Role { get; init; }

    [JsonPropertyName("time")]
    public MessageTime? Time { get; init; }

    [JsonPropertyName("agent")]
    public string? Agent { get; init; }

    [JsonPropertyName("model")]
    public MessageModel? Model { get; init; }
}

public record MessageRemovedEvent : Event
{
    [JsonPropertyName("properties")]
    public MessageRemovedProperties? Properties { get; init; }
}

public record MessageRemovedProperties
{
    [JsonPropertyName("sessionID")]
    public string? SessionId { get; init; }

    [JsonPropertyName("messageID")]
    public string? MessageId { get; init; }
}

public record MessagePartUpdatedEvent : Event
{
    [JsonPropertyName("properties")]
    public MessagePartUpdatedProperties? Properties { get; init; }
}

public record MessagePartUpdatedProperties
{
    [JsonPropertyName("sessionID")]
    public string? SessionId { get; init; }

    [JsonPropertyName("messageID")]
    public string? MessageId { get; init; }

    [JsonPropertyName("part")]
    public Part? Part { get; init; }

    /// <summary>
    /// Delta text for streaming parts (text, reasoning).
    /// This is the incremental text added in this update.
    /// </summary>
    [JsonPropertyName("delta")]
    public string? Delta { get; init; }
}

public record MessagePartRemovedEvent : Event
{
    [JsonPropertyName("properties")]
    public MessagePartRemovedProperties? Properties { get; init; }
}

public record MessagePartRemovedProperties
{
    [JsonPropertyName("sessionID")]
    public string? SessionId { get; init; }

    [JsonPropertyName("messageID")]
    public string? MessageId { get; init; }

    [JsonPropertyName("partID")]
    public string? PartId { get; init; }
}

// Permission events
public record PermissionUpdatedEvent : Event
{
    [JsonPropertyName("properties")]
    public PermissionUpdatedProperties? Properties { get; init; }
}

public record PermissionUpdatedProperties
{
    [JsonPropertyName("sessionID")]
    public string? SessionId { get; init; }

    [JsonPropertyName("permission")]
    public Permission? Permission { get; init; }
}

public record PermissionRepliedEvent : Event
{
    [JsonPropertyName("properties")]
    public PermissionRepliedProperties? Properties { get; init; }
}

public record PermissionRepliedProperties
{
    [JsonPropertyName("sessionID")]
    public string? SessionId { get; init; }

    [JsonPropertyName("permissionID")]
    public string? PermissionId { get; init; }

    [JsonPropertyName("allow")]
    public bool? Allow { get; init; }
}

// File events
public record FileEditedEvent : Event
{
    [JsonPropertyName("properties")]
    public FileEditedProperties? Properties { get; init; }
}

public record FileEditedProperties
{
    [JsonPropertyName("path")]
    public string? Path { get; init; }
}

public record FileWatcherUpdatedEvent : Event
{
    [JsonPropertyName("properties")]
    public FileWatcherUpdatedProperties? Properties { get; init; }
}

public record FileWatcherUpdatedProperties
{
    [JsonPropertyName("files")]
    public List<string>? Files { get; init; }
}

// Todo events
public record TodoUpdatedEvent : Event
{
    [JsonPropertyName("properties")]
    public TodoUpdatedProperties? Properties { get; init; }
}

public record TodoUpdatedProperties
{
    [JsonPropertyName("sessionID")]
    public string? SessionId { get; init; }

    [JsonPropertyName("todos")]
    public List<Todo>? Todos { get; init; }
}

// PTY events
public record PtyCreatedEvent : Event
{
    [JsonPropertyName("properties")]
    public PtyCreatedProperties? Properties { get; init; }
}

public record PtyCreatedProperties
{
    [JsonPropertyName("pty")]
    public Pty? Pty { get; init; }
}

public record PtyUpdatedEvent : Event
{
    [JsonPropertyName("properties")]
    public PtyUpdatedProperties? Properties { get; init; }
}

public record PtyUpdatedProperties
{
    [JsonPropertyName("pty")]
    public Pty? Pty { get; init; }
}

public record PtyExitedEvent : Event
{
    [JsonPropertyName("properties")]
    public PtyExitedProperties? Properties { get; init; }
}

public record PtyExitedProperties
{
    [JsonPropertyName("ptyID")]
    public string? PtyId { get; init; }

    [JsonPropertyName("exitCode")]
    public int? ExitCode { get; init; }
}

public record PtyDeletedEvent : Event
{
    [JsonPropertyName("properties")]
    public PtyDeletedProperties? Properties { get; init; }
}

public record PtyDeletedProperties
{
    [JsonPropertyName("ptyID")]
    public string? PtyId { get; init; }
}

// LSP events
public record LspUpdatedEvent : Event
{
    [JsonPropertyName("properties")]
    public LspUpdatedProperties? Properties { get; init; }
}

public record LspUpdatedProperties
{
    [JsonPropertyName("status")]
    public LspStatus? Status { get; init; }
}

public record LspClientDiagnosticsEvent : Event
{
    [JsonPropertyName("properties")]
    public LspClientDiagnosticsProperties? Properties { get; init; }
}

public record LspClientDiagnosticsProperties
{
    [JsonPropertyName("diagnostics")]
    public Dictionary<string, object>? Diagnostics { get; init; }
}

// Command events
public record CommandExecutedEvent : Event
{
    [JsonPropertyName("properties")]
    public CommandExecutedProperties? Properties { get; init; }
}

public record CommandExecutedProperties
{
    [JsonPropertyName("command")]
    public string? CommandName { get; init; }
}

// VCS events
public record VcsBranchUpdatedEvent : Event
{
    [JsonPropertyName("properties")]
    public VcsBranchUpdatedProperties? Properties { get; init; }
}

public record VcsBranchUpdatedProperties
{
    [JsonPropertyName("branch")]
    public string? Branch { get; init; }
}

// Installation events
public record InstallationUpdatedEvent : GlobalEvent
{
    [JsonPropertyName("properties")]
    public InstallationUpdatedProperties? Properties { get; init; }
}

public record InstallationUpdatedProperties
{
    [JsonPropertyName("version")]
    public string? Version { get; init; }
}

public record InstallationUpdateAvailableEvent : GlobalEvent
{
    [JsonPropertyName("properties")]
    public InstallationUpdateAvailableProperties? Properties { get; init; }
}

public record InstallationUpdateAvailableProperties
{
    [JsonPropertyName("version")]
    public string? Version { get; init; }
}

// TUI events
public record TuiPromptAppendEvent : Event
{
    [JsonPropertyName("properties")]
    public TuiPromptAppendProperties? Properties { get; init; }
}

public record TuiPromptAppendProperties
{
    [JsonPropertyName("text")]
    public string? Text { get; init; }
}

public record TuiCommandExecuteEvent : Event
{
    [JsonPropertyName("properties")]
    public TuiCommandExecuteProperties? Properties { get; init; }
}

public record TuiCommandExecuteProperties
{
    [JsonPropertyName("command")]
    public string? CommandName { get; init; }
}

public record TuiToastShowEvent : Event
{
    [JsonPropertyName("properties")]
    public TuiToastShowProperties? Properties { get; init; }
}

public record TuiToastShowProperties
{
    [JsonPropertyName("message")]
    public string? Message { get; init; }

    [JsonPropertyName("level")]
    public string? Level { get; init; }
}
