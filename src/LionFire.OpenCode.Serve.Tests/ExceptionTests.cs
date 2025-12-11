using System.Net;
using LionFire.OpenCode.Serve.Exceptions;

namespace LionFire.OpenCode.Serve.Tests;

public class ExceptionTests
{
    [Fact]
    public void OpenCodeException_WithMessage_SetsMessage()
    {
        var ex = new OpenCodeException("Test error");

        ex.Message.Should().Be("Test error");
    }

    [Fact]
    public void OpenCodeApiException_SetsProperties()
    {
        var ex = new OpenCodeApiException(
            "Not found",
            HttpStatusCode.NotFound,
            "not_found",
            "req_123");

        ex.Message.Should().Be("Not found");
        ex.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ex.ErrorCode.Should().Be("not_found");
        ex.RequestId.Should().Be("req_123");
    }

    [Fact]
    public void OpenCodeNotFoundException_SetsResourceInfo()
    {
        var ex = new OpenCodeNotFoundException(
            "Session not found",
            "session",
            "sess_123",
            "req_456");

        ex.Message.Should().Be("Session not found");
        ex.StatusCode.Should().Be(HttpStatusCode.NotFound);
        ex.ResourceType.Should().Be("session");
        ex.ResourceId.Should().Be("sess_123");
        ex.RequestId.Should().Be("req_456");
    }

    [Fact]
    public void OpenCodeConflictException_SetsProperties()
    {
        var ex = new OpenCodeConflictException("Session still running", "req_789");

        ex.Message.Should().Be("Session still running");
        ex.StatusCode.Should().Be(HttpStatusCode.Conflict);
        ex.ErrorCode.Should().Be("conflict");
    }

    [Fact]
    public void OpenCodeServerException_SetsProperties()
    {
        var ex = new OpenCodeServerException(
            "Internal server error",
            HttpStatusCode.InternalServerError,
            "req_111");

        ex.Message.Should().Be("Internal server error");
        ex.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        ex.ErrorCode.Should().Be("server_error");
    }

    [Fact]
    public void OpenCodeConnectionException_ServerNotReachable_CreatesHelpfulMessage()
    {
        var ex = OpenCodeConnectionException.ServerNotReachable("http://localhost:9123");

        ex.Message.Should().Contain("OpenCode server not responding");
        ex.Message.Should().Contain("http://localhost:9123");
        ex.Message.Should().Contain("opencode serve");
        ex.BaseUrl.Should().Be("http://localhost:9123");
    }

    [Fact]
    public void OpenCodeTimeoutException_OperationTimedOut_CreatesHelpfulMessage()
    {
        var ex = OpenCodeTimeoutException.OperationTimedOut(
            "SendMessage",
            TimeSpan.FromSeconds(30));

        ex.Message.Should().Contain("SendMessage");
        ex.Message.Should().Contain("timed out");
        ex.Message.Should().Contain("30");
        ex.Message.Should().Contain("increasing the timeout");
        ex.Timeout.Should().Be(TimeSpan.FromSeconds(30));
        ex.Operation.Should().Be("SendMessage");
    }

    [Fact]
    public void ExceptionHierarchy_IsCorrect()
    {
        typeof(OpenCodeApiException).Should().BeDerivedFrom<OpenCodeException>();
        typeof(OpenCodeNotFoundException).Should().BeDerivedFrom<OpenCodeApiException>();
        typeof(OpenCodeConflictException).Should().BeDerivedFrom<OpenCodeApiException>();
        typeof(OpenCodeServerException).Should().BeDerivedFrom<OpenCodeApiException>();
        typeof(OpenCodeConnectionException).Should().BeDerivedFrom<OpenCodeException>();
        typeof(OpenCodeTimeoutException).Should().BeDerivedFrom<OpenCodeException>();
    }
}
