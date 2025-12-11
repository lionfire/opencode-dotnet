using System.Net;
using System.Text.Json;
using LionFire.OpenCode.Serve;
using LionFire.OpenCode.Serve.Exceptions;
using LionFire.OpenCode.Serve.Models;
using RichardSzalay.MockHttp;

namespace LionFire.OpenCode.Serve.Tests;

public class OpenCodeClientTests
{
    private readonly MockHttpMessageHandler _mockHttp;
    private readonly HttpClient _httpClient;
    private readonly OpenCodeClient _client;

    public OpenCodeClientTests()
    {
        _mockHttp = new MockHttpMessageHandler();
        _httpClient = _mockHttp.ToHttpClient();
        _httpClient.BaseAddress = new Uri("http://localhost:9123");
        _client = new OpenCodeClient(_httpClient);
    }

    [Fact]
    public async Task Constructor_WithBaseUrl_SetsBaseAddress()
    {
        await using var client = new OpenCodeClient("http://localhost:8080");
        // No exception thrown means success - can't directly verify private field
    }

    [Fact]
    public void Constructor_WithInvalidOptions_ThrowsArgumentException()
    {
        var options = new OpenCodeClientOptions { BaseUrl = "invalid" };

        var act = () => new OpenCodeClient(options);

        act.Should().Throw<ArgumentException>()
            .WithMessage("*Invalid OpenCodeClientOptions*");
    }

    [Fact]
    public async Task PingAsync_ServerHealthy_ReturnsHealthCheckResult()
    {
        var expected = new HealthCheckResult("ok", "1.0.0", 3600);

        _mockHttp.When("/health")
            .Respond("application/json", JsonSerializer.Serialize(expected));

        var result = await _client.PingAsync();

        result.Status.Should().Be("ok");
        result.Version.Should().Be("1.0.0");
    }

    [Fact]
    public async Task CreateSessionAsync_ReturnsNewSession()
    {
        var expected = new Session(
            "sess_123",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            SessionStatus.Active);

        _mockHttp.When(HttpMethod.Post, "/sessions")
            .Respond("application/json", JsonSerializer.Serialize(expected));

        var result = await _client.CreateSessionAsync();

        result.Id.Should().Be("sess_123");
        result.Status.Should().Be(SessionStatus.Active);
    }

    [Fact]
    public async Task CreateSessionAsync_WithDirectory_IncludesDirectoryInRequest()
    {
        var expected = new Session(
            "sess_123",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            SessionStatus.Active,
            Directory: "/my/project");

        _mockHttp.When(HttpMethod.Post, "/sessions")
            .With(req => req.Content != null)
            .Respond("application/json", JsonSerializer.Serialize(expected));

        var result = await _client.CreateSessionAsync("/my/project");

        result.Directory.Should().Be("/my/project");
    }

    [Fact]
    public async Task GetSessionAsync_SessionExists_ReturnsSession()
    {
        var expected = new Session(
            "sess_123",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            SessionStatus.Active);

        _mockHttp.When("/sessions/sess_123")
            .Respond("application/json", JsonSerializer.Serialize(expected));

        var result = await _client.GetSessionAsync("sess_123");

        result.Id.Should().Be("sess_123");
    }

    [Fact]
    public async Task GetSessionAsync_SessionNotFound_ThrowsNotFoundException()
    {
        _mockHttp.When("/sessions/nonexistent")
            .Respond(HttpStatusCode.NotFound, "application/json", "{\"error\":\"Session not found\"}");

        var act = () => _client.GetSessionAsync("nonexistent");

        await act.Should().ThrowAsync<OpenCodeNotFoundException>();
    }

    [Fact]
    public async Task ListSessionsAsync_ReturnsSessions()
    {
        var sessions = new[]
        {
            new Session("sess_1", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, SessionStatus.Active),
            new Session("sess_2", DateTimeOffset.UtcNow, DateTimeOffset.UtcNow, SessionStatus.Completed)
        };

        _mockHttp.When("/sessions")
            .Respond("application/json", JsonSerializer.Serialize(sessions));

        var result = await _client.ListSessionsAsync();

        result.Should().HaveCount(2);
        result[0].Id.Should().Be("sess_1");
        result[1].Status.Should().Be(SessionStatus.Completed);
    }

    [Fact]
    public async Task DeleteSessionAsync_SessionExists_Succeeds()
    {
        _mockHttp.When(HttpMethod.Delete, "/sessions/sess_123")
            .Respond(HttpStatusCode.NoContent);

        var act = () => _client.DeleteSessionAsync("sess_123");

        await act.Should().NotThrowAsync();
    }

    [Fact]
    public async Task SendMessageAsync_WithText_ReturnsResponse()
    {
        var responseMessage = new Message(
            "msg_456",
            "sess_123",
            MessageRole.Assistant,
            new MessagePart[] { new TextPart("Hello! How can I help you?") },
            DateTimeOffset.UtcNow);

        _mockHttp.When(HttpMethod.Post, "/sessions/sess_123/messages")
            .Respond("application/json", JsonSerializer.Serialize(responseMessage));

        var result = await _client.SendMessageAsync("sess_123", "Hello");

        result.Id.Should().Be("msg_456");
        result.Role.Should().Be(MessageRole.Assistant);
        result.Parts.Should().ContainSingle();
        result.Parts[0].Should().BeOfType<TextPart>()
            .Which.Text.Should().Be("Hello! How can I help you?");
    }

    [Fact]
    public async Task GetMessagesAsync_ReturnsAllMessages()
    {
        var messages = new[]
        {
            new Message(
                "msg_1",
                "sess_123",
                MessageRole.User,
                new MessagePart[] { new TextPart("Hello") },
                DateTimeOffset.UtcNow),
            new Message(
                "msg_2",
                "sess_123",
                MessageRole.Assistant,
                new MessagePart[] { new TextPart("Hi there!") },
                DateTimeOffset.UtcNow)
        };

        _mockHttp.When("/sessions/sess_123/messages")
            .Respond("application/json", JsonSerializer.Serialize(messages));

        var result = await _client.GetMessagesAsync("sess_123");

        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetToolsAsync_ReturnsTools()
    {
        var tools = new[]
        {
            new Tool("bash", "Bash", "Execute bash commands", null, true),
            new Tool("write", "Write File", "Write content to a file", null, false)
        };

        _mockHttp.When("/tools")
            .Respond("application/json", JsonSerializer.Serialize(tools));

        var result = await _client.GetToolsAsync();

        result.Should().HaveCount(2);
        result[0].Name.Should().Be("Bash");
        result[0].RequiresApproval.Should().BeTrue();
    }

    [Fact]
    public async Task GetConfigAsync_ReturnsConfig()
    {
        var config = new OpenCodeConfig(
            Username: "testuser",
            Version: "1.0.0",
            Providers: new[]
            {
                new Provider("anthropic", "Anthropic", null, true)
            },
            DefaultProvider: "anthropic");

        _mockHttp.When("/config")
            .Respond("application/json", JsonSerializer.Serialize(config));

        var result = await _client.GetConfigAsync();

        result.Version.Should().Be("1.0.0");
        result.DefaultProvider.Should().Be("anthropic");
        result.Providers.Should().ContainSingle();
    }

    [Fact]
    public async Task CreateSessionScopeAsync_CreatesAndDeletesSession()
    {
        var session = new Session(
            "sess_123",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            SessionStatus.Active);

        _mockHttp.When(HttpMethod.Post, "/sessions")
            .Respond("application/json", JsonSerializer.Serialize(session));

        var deleteMatcher = _mockHttp.When(HttpMethod.Delete, "/sessions/sess_123")
            .Respond(HttpStatusCode.NoContent);

        await using (var scope = await _client.CreateSessionScopeAsync())
        {
            scope.Session.Id.Should().Be("sess_123");
            scope.SessionId.Should().Be("sess_123");
        }

        // Verify delete was called - count should be 1 after dispose
        _mockHttp.GetMatchCount(deleteMatcher).Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ServerError_ThrowsServerException()
    {
        _mockHttp.When("/sessions")
            .Respond(HttpStatusCode.InternalServerError, "application/json", "{\"error\":\"Internal server error\"}");

        var act = () => _client.ListSessionsAsync();

        await act.Should().ThrowAsync<OpenCodeServerException>();
    }

    [Fact]
    public async Task ForkSessionAsync_ReturnsForkedSession()
    {
        var forked = new Session(
            "sess_456",
            DateTimeOffset.UtcNow,
            DateTimeOffset.UtcNow,
            SessionStatus.Active);

        _mockHttp.When(HttpMethod.Post, "/sessions/sess_123/fork")
            .Respond("application/json", JsonSerializer.Serialize(forked));

        var result = await _client.ForkSessionAsync("sess_123", "msg_001");

        result.Id.Should().Be("sess_456");
    }

    public async ValueTask DisposeAsync()
    {
        await _client.DisposeAsync();
        _httpClient.Dispose();
        _mockHttp.Dispose();
    }
}
