using System.Text.Json.Serialization;
using LionFire.OpenCode.Serve.Models;

namespace LionFire.OpenCode.Serve.Internal;

/// <summary>
/// Source-generated JSON serializer context for AOT compatibility and improved performance.
/// </summary>
[JsonSourceGenerationOptions(
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = false,
    PropertyNameCaseInsensitive = true)]
[JsonSerializable(typeof(Session))]
[JsonSerializable(typeof(Session[]))]
[JsonSerializable(typeof(List<Session>))]
[JsonSerializable(typeof(CreateSessionRequest))]
[JsonSerializable(typeof(ForkSessionRequest))]
[JsonSerializable(typeof(Message))]
[JsonSerializable(typeof(Message[]))]
[JsonSerializable(typeof(List<Message>))]
[JsonSerializable(typeof(MessagePart))]
[JsonSerializable(typeof(MessagePart[]))]
[JsonSerializable(typeof(List<MessagePart>))]
[JsonSerializable(typeof(TextPart))]
[JsonSerializable(typeof(FilePart))]
[JsonSerializable(typeof(AgentPart))]
[JsonSerializable(typeof(ToolUsePart))]
[JsonSerializable(typeof(ToolResultPart))]
[JsonSerializable(typeof(SendMessageRequest))]
[JsonSerializable(typeof(MessageUpdate))]
[JsonSerializable(typeof(Tool))]
[JsonSerializable(typeof(Tool[]))]
[JsonSerializable(typeof(List<Tool>))]
[JsonSerializable(typeof(ApproveToolRequest))]
[JsonSerializable(typeof(UpdateToolPermissionRequest))]
[JsonSerializable(typeof(PendingToolExecution))]
[JsonSerializable(typeof(OpenCodeFileInfo))]
[JsonSerializable(typeof(OpenCodeFileInfo[]))]
[JsonSerializable(typeof(List<OpenCodeFileInfo>))]
[JsonSerializable(typeof(FileContent))]
[JsonSerializable(typeof(SearchFilesRequest))]
[JsonSerializable(typeof(SearchFilesResult))]
[JsonSerializable(typeof(FileChange))]
[JsonSerializable(typeof(FileChange[]))]
[JsonSerializable(typeof(ApplyChangesRequest))]
[JsonSerializable(typeof(ApplyChangesResult))]
[JsonSerializable(typeof(FileChangeError))]
[JsonSerializable(typeof(OpenCodeConfig))]
[JsonSerializable(typeof(Provider))]
[JsonSerializable(typeof(Provider[]))]
[JsonSerializable(typeof(List<Provider>))]
[JsonSerializable(typeof(Model))]
[JsonSerializable(typeof(Model[]))]
[JsonSerializable(typeof(List<Model>))]
[JsonSerializable(typeof(HealthCheckResult))]
[JsonSerializable(typeof(ExecuteCommandRequest))]
[JsonSerializable(typeof(CommandResult))]
public partial class OpenCodeSerializerContext : JsonSerializerContext
{
}
