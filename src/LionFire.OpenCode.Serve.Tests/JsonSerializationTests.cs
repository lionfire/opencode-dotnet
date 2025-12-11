using System.Text.Json;
using LionFire.OpenCode.Serve.Internal;
using LionFire.OpenCode.Serve.Models;

namespace LionFire.OpenCode.Serve.Tests;

public class JsonSerializationTests
{
    [Fact]
    public void Session_RoundTrip_PreservesValues()
    {
        var original = new Session(
            "sess_123",
            new DateTimeOffset(2024, 1, 15, 10, 30, 0, TimeSpan.Zero),
            new DateTimeOffset(2024, 1, 15, 11, 0, 0, TimeSpan.Zero),
            SessionStatus.Active,
            "token123",
            "/my/project");

        var json = JsonSerializer.Serialize(original, JsonOptions.Default);
        var deserialized = JsonSerializer.Deserialize<Session>(json, JsonOptions.Default);

        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be("sess_123");
        deserialized.Status.Should().Be(SessionStatus.Active);
        deserialized.SharedToken.Should().Be("token123");
        deserialized.Directory.Should().Be("/my/project");
    }

    [Fact]
    public void Message_WithTextPart_RoundTrip()
    {
        var original = new Message(
            "msg_456",
            "sess_123",
            MessageRole.User,
            new MessagePart[] { new TextPart("Hello, world!") },
            DateTimeOffset.UtcNow);

        var json = JsonSerializer.Serialize(original, JsonOptions.Default);
        var deserialized = JsonSerializer.Deserialize<Message>(json, JsonOptions.Default);

        deserialized.Should().NotBeNull();
        deserialized!.Parts.Should().ContainSingle();
        deserialized.Parts[0].Should().BeOfType<TextPart>()
            .Which.Text.Should().Be("Hello, world!");
    }

    [Fact]
    public void Message_WithFilePart_RoundTrip()
    {
        var original = new Message(
            "msg_456",
            "sess_123",
            MessageRole.User,
            new MessagePart[] { new FilePart("/path/to/file.cs", "file content here") },
            DateTimeOffset.UtcNow);

        var json = JsonSerializer.Serialize(original, JsonOptions.Default);
        var deserialized = JsonSerializer.Deserialize<Message>(json, JsonOptions.Default);

        deserialized.Should().NotBeNull();
        deserialized!.Parts[0].Should().BeOfType<FilePart>()
            .Which.FilePath.Should().Be("/path/to/file.cs");
    }

    [Fact]
    public void Message_WithAgentPart_RoundTrip()
    {
        var original = new Message(
            "msg_456",
            "sess_123",
            MessageRole.Assistant,
            new MessagePart[] { new AgentPart("claude", "Agent content") },
            DateTimeOffset.UtcNow);

        var json = JsonSerializer.Serialize(original, JsonOptions.Default);
        var deserialized = JsonSerializer.Deserialize<Message>(json, JsonOptions.Default);

        deserialized.Should().NotBeNull();
        deserialized!.Parts[0].Should().BeOfType<AgentPart>()
            .Which.AgentId.Should().Be("claude");
    }

    [Fact]
    public void Message_WithToolUsePart_RoundTrip()
    {
        var original = new Message(
            "msg_456",
            "sess_123",
            MessageRole.Assistant,
            new MessagePart[] { new ToolUsePart("tool_1", "bash", new { command = "ls -la" }) },
            DateTimeOffset.UtcNow);

        var json = JsonSerializer.Serialize(original, JsonOptions.Default);
        var deserialized = JsonSerializer.Deserialize<Message>(json, JsonOptions.Default);

        deserialized.Should().NotBeNull();
        deserialized!.Parts[0].Should().BeOfType<ToolUsePart>();
        var toolUse = (ToolUsePart)deserialized.Parts[0];
        toolUse.ToolName.Should().Be("bash");
    }

    [Fact]
    public void Message_WithToolResultPart_RoundTrip()
    {
        var original = new Message(
            "msg_456",
            "sess_123",
            MessageRole.Tool,
            new MessagePart[] { new ToolResultPart("tool_1", "Command output here", false) },
            DateTimeOffset.UtcNow);

        var json = JsonSerializer.Serialize(original, JsonOptions.Default);
        var deserialized = JsonSerializer.Deserialize<Message>(json, JsonOptions.Default);

        deserialized.Should().NotBeNull();
        deserialized!.Parts[0].Should().BeOfType<ToolResultPart>()
            .Which.Output.Should().Be("Command output here");
    }

    [Fact]
    public void Message_WithMixedParts_RoundTrip()
    {
        var original = new Message(
            "msg_456",
            "sess_123",
            MessageRole.User,
            new MessagePart[]
            {
                new TextPart("Please look at this file:"),
                new FilePart("/path/to/file.cs")
            },
            DateTimeOffset.UtcNow);

        var json = JsonSerializer.Serialize(original, JsonOptions.Default);
        var deserialized = JsonSerializer.Deserialize<Message>(json, JsonOptions.Default);

        deserialized.Should().NotBeNull();
        deserialized!.Parts.Should().HaveCount(2);
        deserialized.Parts[0].Should().BeOfType<TextPart>();
        deserialized.Parts[1].Should().BeOfType<FilePart>();
    }

    [Fact]
    public void MessageRole_SerializesAsCamelCase()
    {
        var original = new Message(
            "msg_456",
            "sess_123",
            MessageRole.Assistant,
            new MessagePart[] { new TextPart("Test") },
            DateTimeOffset.UtcNow);

        var json = JsonSerializer.Serialize(original, JsonOptions.Default);

        json.Should().Contain("\"assistant\"");
    }

    [Fact]
    public void SessionStatus_SerializesAsCamelCase()
    {
        var original = new Session(
            "sess_123",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            SessionStatus.Completed);

        var json = JsonSerializer.Serialize(original, JsonOptions.Default);

        json.Should().Contain("\"completed\"");
    }

    [Fact]
    public void Tool_RoundTrip()
    {
        var original = new Tool(
            "bash",
            "Bash",
            "Execute bash commands",
            new { type = "object" },
            true);

        var json = JsonSerializer.Serialize(original, JsonOptions.Default);
        var deserialized = JsonSerializer.Deserialize<Tool>(json, JsonOptions.Default);

        deserialized.Should().NotBeNull();
        deserialized!.Id.Should().Be("bash");
        deserialized.RequiresApproval.Should().BeTrue();
    }

    [Fact]
    public void OpenCodeFileInfo_RoundTrip()
    {
        var original = new OpenCodeFileInfo(
            "/src/main.cs",
            1024,
            false,
            DateTimeOffset.UtcNow);

        var json = JsonSerializer.Serialize(original, JsonOptions.Default);
        var deserialized = JsonSerializer.Deserialize<OpenCodeFileInfo>(json, JsonOptions.Default);

        deserialized.Should().NotBeNull();
        deserialized!.Path.Should().Be("/src/main.cs");
        deserialized.Size.Should().Be(1024);
        deserialized.IsDirectory.Should().BeFalse();
    }

    [Fact]
    public void MessageUpdate_RoundTrip()
    {
        var original = new MessageUpdate(
            "msg_123",
            "Hello ",
            false,
            10);

        var json = JsonSerializer.Serialize(original, JsonOptions.Default);
        var deserialized = JsonSerializer.Deserialize<MessageUpdate>(json, JsonOptions.Default);

        deserialized.Should().NotBeNull();
        deserialized!.MessageId.Should().Be("msg_123");
        deserialized.Delta.Should().Be("Hello ");
        deserialized.Done.Should().BeFalse();
    }

    [Fact]
    public void NullValues_AreOmittedFromJson()
    {
        var original = new Session(
            "sess_123",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            SessionStatus.Active,
            null,
            null);

        var json = JsonSerializer.Serialize(original, JsonOptions.Default);

        json.Should().NotContain("sharedToken");
        json.Should().NotContain("directory");
    }

    [Fact]
    public void CamelCase_PropertyNaming()
    {
        var original = new Session(
            "sess_123",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            SessionStatus.Active);

        var json = JsonSerializer.Serialize(original, JsonOptions.Default);

        json.Should().Contain("\"createdAt\"");
        json.Should().Contain("\"updatedAt\"");
        json.Should().NotContain("\"CreatedAt\"");
    }

    [Fact]
    public void Deserialization_CaseInsensitive()
    {
        var json = @"{""Id"":""sess_123"",""CreatedAt"":""2024-01-15T10:00:00Z"",""UpdatedAt"":""2024-01-15T10:00:00Z"",""Status"":""active""}";

        var session = JsonSerializer.Deserialize<Session>(json, JsonOptions.Default);

        session.Should().NotBeNull();
        session!.Id.Should().Be("sess_123");
    }
}
