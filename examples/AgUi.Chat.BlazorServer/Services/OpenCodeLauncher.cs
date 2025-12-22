using System.Diagnostics;
using Microsoft.Extensions.Options;

namespace AgUi.Chat.BlazorServer.Services;

/// <summary>
/// Service for launching and managing the OpenCode serve process.
/// </summary>
public class OpenCodeLauncher : IDisposable
{
    private readonly ILogger<OpenCodeLauncher> _logger;
    private readonly string _baseUrl;
    private Process? _openCodeProcess;

    public OpenCodeLauncher(ILogger<OpenCodeLauncher> logger, IConfiguration configuration)
    {
        _logger = logger;
        _baseUrl = configuration["OpenCode:BaseUrl"] ?? "http://localhost:9123";
    }

    /// <summary>
    /// Gets the OpenCode serve base URL.
    /// </summary>
    public string BaseUrl => _baseUrl;

    /// <summary>
    /// Checks if OpenCode serve is running and accessible.
    /// </summary>
    public async Task<bool> IsRunningAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
            var response = await httpClient.GetAsync($"{_baseUrl}/path", cancellationToken);
            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            _logger.LogDebug(ex, "OpenCode serve not responding at {BaseUrl}", _baseUrl);
            return false;
        }
    }

    /// <summary>
    /// Attempts to start the OpenCode serve process.
    /// </summary>
    /// <returns>True if started successfully or already running.</returns>
    public async Task<StartResult> StartAsync(CancellationToken cancellationToken = default)
    {
        // Check if already running
        if (await IsRunningAsync(cancellationToken))
        {
            _logger.LogInformation("OpenCode serve is already running at {BaseUrl}", _baseUrl);
            return new StartResult(true, "OpenCode is already running");
        }

        try
        {
            // Try to find opencode executable
            var openCodePath = FindOpenCodeExecutable();
            if (openCodePath == null)
            {
                return new StartResult(false, "Could not find 'opencode' command. Please install OpenCode first.");
            }

            _logger.LogInformation("Starting OpenCode serve from {Path}", openCodePath);

            var startInfo = new ProcessStartInfo
            {
                FileName = openCodePath,
                Arguments = "serve",
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };

            _openCodeProcess = Process.Start(startInfo);

            if (_openCodeProcess == null)
            {
                return new StartResult(false, "Failed to start OpenCode process");
            }

            // Wait a bit for it to start up
            for (int i = 0; i < 10; i++)
            {
                await Task.Delay(500, cancellationToken);
                if (await IsRunningAsync(cancellationToken))
                {
                    _logger.LogInformation("OpenCode serve started successfully");
                    return new StartResult(true, "OpenCode started successfully");
                }
            }

            return new StartResult(false, "OpenCode process started but server not responding");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start OpenCode serve");
            return new StartResult(false, $"Failed to start OpenCode: {ex.Message}");
        }
    }

    private string? FindOpenCodeExecutable()
    {
        // Check common locations
        var possiblePaths = new[]
        {
            "opencode",  // In PATH
            "/usr/local/bin/opencode",
            "/usr/bin/opencode",
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), ".local", "bin", "opencode"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "go", "bin", "opencode"),
        };

        foreach (var path in possiblePaths)
        {
            try
            {
                // Try running with --version to check if it exists
                var startInfo = new ProcessStartInfo
                {
                    FileName = path,
                    Arguments = "--version",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = true
                };

                using var process = Process.Start(startInfo);
                if (process != null)
                {
                    process.WaitForExit(2000);
                    if (process.ExitCode == 0)
                    {
                        return path;
                    }
                }
            }
            catch
            {
                // Continue to next path
            }
        }

        return null;
    }

    public void Dispose()
    {
        if (_openCodeProcess != null && !_openCodeProcess.HasExited)
        {
            try
            {
                _openCodeProcess.Kill();
                _openCodeProcess.Dispose();
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to stop OpenCode process");
            }
        }
    }

    public record StartResult(bool Success, string Message);
}
