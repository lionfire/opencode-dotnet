using LionFire.OpenCode.Serve;

namespace LionFire.OpenCode.Serve.Tests;

public class OpenCodeClientOptionsTests
{
    [Fact]
    public void DefaultValues_AreCorrect()
    {
        var options = new OpenCodeClientOptions();

        options.BaseUrl.Should().Be("http://localhost:9123");
        options.Directory.Should().BeNull();
        options.DefaultTimeout.Should().Be(TimeSpan.FromSeconds(30));
        options.MessageTimeout.Should().Be(TimeSpan.FromMinutes(5));
        options.EnableRetry.Should().BeTrue();
        options.MaxRetryAttempts.Should().Be(3);
        options.RetryDelaySeconds.Should().Be(2);
    }

    [Theory]
    [InlineData("http://localhost:9123")]
    [InlineData("http://localhost:8080")]
    [InlineData("https://localhost:9123")]
    [InlineData("http://127.0.0.1:9123")]
    public void ValidateBaseUrl_ValidUrls_ReturnsTrue(string url)
    {
        var options = new OpenCodeClientOptions { BaseUrl = url };

        options.ValidateBaseUrl().Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("not-a-url")]
    [InlineData("ftp://localhost:9123")]
    [InlineData("localhost:9123")]
    public void ValidateBaseUrl_InvalidUrls_ReturnsFalse(string url)
    {
        var options = new OpenCodeClientOptions { BaseUrl = url };

        options.ValidateBaseUrl().Should().BeFalse();
    }

    [Fact]
    public void Validator_ValidOptions_ReturnsSuccess()
    {
        var options = new OpenCodeClientOptions();
        var validator = new OpenCodeClientOptionsValidator();

        var result = validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }

    [Fact]
    public void Validator_EmptyBaseUrl_ReturnsFail()
    {
        var options = new OpenCodeClientOptions { BaseUrl = "" };
        var validator = new OpenCodeClientOptionsValidator();

        var result = validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(f => f.Contains("BaseUrl"));
    }

    [Fact]
    public void Validator_InvalidBaseUrl_ReturnsFail()
    {
        var options = new OpenCodeClientOptions { BaseUrl = "not-a-url" };
        var validator = new OpenCodeClientOptionsValidator();

        var result = validator.Validate(null, options);

        result.Failed.Should().BeTrue();
    }

    [Fact]
    public void Validator_NegativeTimeout_ReturnsFail()
    {
        var options = new OpenCodeClientOptions { DefaultTimeout = TimeSpan.FromSeconds(-1) };
        var validator = new OpenCodeClientOptionsValidator();

        var result = validator.Validate(null, options);

        result.Failed.Should().BeTrue();
        result.Failures.Should().Contain(f => f.Contains("DefaultTimeout"));
    }

    [Fact]
    public void Validator_NegativeMessageTimeout_ReturnsFail()
    {
        var options = new OpenCodeClientOptions { MessageTimeout = TimeSpan.FromSeconds(-1) };
        var validator = new OpenCodeClientOptionsValidator();

        var result = validator.Validate(null, options);

        result.Failed.Should().BeTrue();
    }

    [Fact]
    public void Validator_NegativeRetryAttempts_ReturnsFail()
    {
        var options = new OpenCodeClientOptions { MaxRetryAttempts = -1 };
        var validator = new OpenCodeClientOptionsValidator();

        var result = validator.Validate(null, options);

        result.Failed.Should().BeTrue();
    }

    [Fact]
    public void Validator_ZeroRetryAttempts_ReturnsSuccess()
    {
        var options = new OpenCodeClientOptions { MaxRetryAttempts = 0 };
        var validator = new OpenCodeClientOptionsValidator();

        var result = validator.Validate(null, options);

        result.Succeeded.Should().BeTrue();
    }
}
