// AOT Example - Demonstrates Native AOT compilation with OpenCode SDK
//
// This example showcases the use of source-generated JSON serialization
// for Native AOT compatibility. The SDK uses System.Text.Json source generators
// to enable AOT compilation without reflection-based serialization.
//
// To publish as AOT:
//   dotnet publish -c Release -r win-x64 --self-contained
//   dotnet publish -c Release -r linux-x64 --self-contained
//   dotnet publish -c Release -r osx-x64 --self-contained

using System.Text.Json;
using LionFire.OpenCode.Serve;
using LionFire.OpenCode.Serve.Internal;
using LionFire.OpenCode.Serve.Models;

Console.WriteLine("OpenCode SDK - AOT Compilation Example");
Console.WriteLine("======================================");
Console.WriteLine();

// Demonstrate source-generated JSON serialization
DemonstrateSourceGeneratedSerialization();

// If running with OpenCode server, demonstrate API calls
await DemonstrateApiCallsAsync();

Console.WriteLine();
Console.WriteLine("AOT example completed successfully!");

void DemonstrateSourceGeneratedSerialization()
{
    Console.WriteLine("1. Source-Generated JSON Serialization Demo");
    Console.WriteLine("   -----------------------------------------");

    // Create a sample session
    var session = new Session
    {
        Id = "ses_example123",
        ProjectId = "proj_demo",
        Directory = "/path/to/project",
        Title = "AOT Test Session",
        Version = "1.0.0",
        Time = new SessionTime
        {
            Created = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds(),
            Updated = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()
        }
    };

    // Serialize using source-generated context (AOT-safe) - use TypeInfo for full AOT support
    var json = JsonSerializer.Serialize(session, OpenCodeSerializerContext.Default.Session);
    Console.WriteLine($"   Serialized Session: {json}");

    // Deserialize using source-generated context (AOT-safe) - use TypeInfo for full AOT support
    var deserialized = JsonSerializer.Deserialize(json, OpenCodeSerializerContext.Default.Session);
    Console.WriteLine($"   Deserialized Session ID: {deserialized?.Id}");

    // Create and serialize a message with parts
    var sendRequest = new SendMessageRequest
    {
        Parts = [PartInput.TextInput("Hello from AOT-compiled app!")]
    };

    var requestJson = JsonSerializer.Serialize(sendRequest, OpenCodeSerializerContext.Default.SendMessageRequest);
    Console.WriteLine($"   Serialized SendMessageRequest: {requestJson}");

    Console.WriteLine("   [OK] Source-generated serialization working correctly");
    Console.WriteLine();
}

async Task DemonstrateApiCallsAsync()
{
    Console.WriteLine("2. API Calls Demo (requires running OpenCode server)");
    Console.WriteLine("   --------------------------------------------------");

    // Check if OPENCODE_URL environment variable is set
    var baseUrl = Environment.GetEnvironmentVariable("OPENCODE_URL") ?? "http://localhost:9123";

    try
    {
        // Create client - all JSON operations use source-generated serialization
        var options = new OpenCodeClientOptions
        {
            BaseUrl = baseUrl,
            DefaultTimeout = TimeSpan.FromSeconds(5)
        };

        await using var client = new OpenCodeClient(options);

        // Try to list sessions (will fail if no server is running)
        Console.WriteLine($"   Connecting to OpenCode at {baseUrl}...");
        var sessions = await client.ListSessionsAsync();
        Console.WriteLine($"   Found {sessions.Count} session(s)");

        if (sessions.Count > 0)
        {
            var firstSession = sessions[0];
            Console.WriteLine($"   First session: {firstSession.Title} ({firstSession.Id})");
        }

        // List providers
        var providers = await client.ListProvidersAsync();
        Console.WriteLine($"   Found {providers.Count} provider(s)");

        Console.WriteLine("   [OK] API calls working correctly with AOT");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"   [SKIP] OpenCode server not available: {ex.Message}");
        Console.WriteLine("   (This is expected if OpenCode is not running)");
    }

    Console.WriteLine();
}
