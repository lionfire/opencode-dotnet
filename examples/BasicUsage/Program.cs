using LionFire.OpenCode.Serve;
using LionFire.OpenCode.Serve.Exceptions;
using LionFire.OpenCode.Serve.Models;

// Parse command line arguments
string? directory = null;
string? baseUrl = null;

for (int i = 0; i < args.Length; i++)
{
    switch (args[i])
    {
        case "-d":
        case "--directory":
            if (i + 1 < args.Length)
                directory = args[++i];
            break;
        case "-u":
        case "--url":
            if (i + 1 < args.Length)
                baseUrl = args[++i];
            break;
        case "-h":
        case "--help":
            ShowHelp();
            return;
    }
}

Console.WriteLine("LionFire.OpenCode.Serve SDK - Basic Usage Example");
Console.WriteLine("=".PadRight(50, '='));
Console.WriteLine();

if (directory is not null)
{
    Console.WriteLine($"Working directory: {directory}");
}
if (baseUrl is not null)
{
    Console.WriteLine($"Server URL: {baseUrl}");
}
if (directory is not null || baseUrl is not null)
{
    Console.WriteLine();
}

// Create client with options
var options = new OpenCodeClientOptions
{
    BaseUrl = baseUrl ?? "http://localhost:9123",
    Directory = directory
};
await using var client = new OpenCodeClient(options);

try
{
    // Example 1: Health Check
    Console.WriteLine("1. Checking server health...");
    var health = await client.PingAsync();
    Console.WriteLine($"   Server status: {health.Status}");
    Console.WriteLine($"   Version: {health.Version ?? "unknown"}");
    Console.WriteLine();

    // Example 2: Get Configuration
    Console.WriteLine("2. Getting server configuration...");
    var config = await client.GetConfigAsync();
    Console.WriteLine($"   OpenCode version: {config.Version}");
    Console.WriteLine($"   Default provider: {config.DefaultProvider ?? "not set"}");
    Console.WriteLine();

    // Example 3: Create Session with directory
    Console.WriteLine("3. Creating session with directory...");
    await using (var scope = await client.CreateSessionScopeAsync(directory))
    {
        Console.WriteLine($"   Created session: {scope.SessionId}");
        Console.WriteLine($"   Session directory: {scope.Session.Directory ?? "(server default)"}");
    }
    Console.WriteLine("   Session cleaned up.");
    Console.WriteLine();

    // Example 4: List sessions
    Console.WriteLine("4. Listing sessions...");
    var sessions = await client.ListSessionsAsync();
    Console.WriteLine($"   Found {sessions.Count} sessions");
    foreach (var sess in sessions.Take(3))
    {
        Console.WriteLine($"   - {sess.Id} (created: {sess.CreatedAt:g})");
    }
    if (sessions.Count > 3)
    {
        Console.WriteLine($"   ... and {sessions.Count - 3} more");
    }
    Console.WriteLine();

    Console.WriteLine("All examples completed successfully!");
    Console.WriteLine();
    Console.WriteLine("Note: Message sending examples require a configured LLM provider.");
    Console.WriteLine("      Run: opencode auth login anthropic (or openai, etc.)");
}
catch (OpenCodeConnectionException ex)
{
    Console.WriteLine($"ERROR: Could not connect to OpenCode server.");
    Console.WriteLine($"       {ex.Message}");
    Console.WriteLine();
    Console.WriteLine("Make sure the OpenCode server is running:");
    Console.WriteLine("  opencode serve --port 9123");
}
catch (OpenCodeApiException ex)
{
    Console.WriteLine($"ERROR: API error (HTTP {(int)ex.StatusCode})");
    Console.WriteLine($"       {ex.Message}");
    if (ex.RequestId != null)
    {
        Console.WriteLine($"       Request ID: {ex.RequestId}");
    }
}
catch (OpenCodeException ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
}

static void ShowHelp()
{
    Console.WriteLine("LionFire.OpenCode.Serve SDK - Basic Usage Example");
    Console.WriteLine();
    Console.WriteLine("Usage: BasicUsage [options]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  -d, --directory <path>  Working directory for sessions");
    Console.WriteLine("  -u, --url <url>         OpenCode server URL (default: http://localhost:9123)");
    Console.WriteLine("  -h, --help              Show this help message");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  BasicUsage");
    Console.WriteLine("  BasicUsage -d /path/to/project");
    Console.WriteLine("  BasicUsage -d C:\\MyProject -u http://localhost:8080");
}
