using LionFire.OpenCode.Serve;
using LionFire.OpenCode.Serve.Exceptions;
using LionFire.OpenCode.Serve.Models;

// Parse command line arguments
string? directory = null;
string? baseUrl = null;
string? providerId = null;
string? modelId = null;
bool listModels = false;
bool freeOnly = false;

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
        case "-p":
        case "--provider":
            if (i + 1 < args.Length)
                providerId = args[++i];
            break;
        case "-m":
        case "--model":
            if (i + 1 < args.Length)
                modelId = args[++i];
            break;
        case "-l":
        case "--list-models":
            listModels = true;
            break;
        case "--free":
            freeOnly = true;
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
    try
    {
        var config = await client.GetConfigAsync();
        Console.WriteLine("   Server is responding");
        if (config.TryGetValue("version", out var versionObj))
        {
            Console.WriteLine($"   Version: {versionObj}");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"   Failed to connect: {ex.Message}");
        return;
    }
    Console.WriteLine();

    // Example 2: List Providers and Models
    Console.WriteLine("2. Listing available providers and models...");
    var providers = await client.ListProvidersAsync(directory);
    Console.WriteLine($"   Found {providers.Count} providers");

    if (listModels)
    {
        Console.WriteLine();
        Console.WriteLine("   Available Models:");
        Console.WriteLine("   " + "-".PadRight(70, '-'));

        foreach (var provider in providers.OrderBy(p => p.Name))
        {
            if (provider.Models == null || provider.Models.Count == 0)
                continue;

            var modelsToShow = freeOnly
                ? provider.Models.Where(m => (m.InputCost ?? 0) == 0 && (m.OutputCost ?? 0) == 0).ToList()
                : provider.Models;

            if (modelsToShow.Count == 0)
                continue;

            Console.WriteLine($"   [{provider.Name}] (provider: {provider.Id})");

            foreach (var model in modelsToShow.OrderBy(m => m.Name))
            {
                var costInfo = (model.InputCost ?? 0) == 0 && (model.OutputCost ?? 0) == 0
                    ? "[FREE]"
                    : $"${model.InputCost:F2}/${model.OutputCost:F2}";
                Console.WriteLine($"      {model.Name,-40} {costInfo,-12} (model: {model.Id})");
            }
            Console.WriteLine();
        }

        Console.WriteLine("   " + "-".PadRight(70, '-'));
        Console.WriteLine("   Usage: BasicUsage -p <provider> -m <model>");
        Console.WriteLine("   Example: BasicUsage -p chutes -m openai/gpt-oss-20b");
        Console.WriteLine();
        return;
    }
    else
    {
        // Show summary of providers with free models
        var freeProviders = providers
            .Where(p => p.Models?.Any(m => (m.InputCost ?? 0) == 0 && (m.OutputCost ?? 0) == 0) == true)
            .ToList();
        if (freeProviders.Count > 0)
        {
            Console.WriteLine($"   Providers with FREE models: {string.Join(", ", freeProviders.Select(p => p.Id))}");
            Console.WriteLine("   Run with --list-models --free to see all free models");
        }
    }
    Console.WriteLine();

    // Example 3: Create Session with optional model
    Console.WriteLine("3. Creating session...");
    var session = await client.CreateSessionAsync(directory: directory);
    Console.WriteLine($"   Created session: {session.Id}");
    Console.WriteLine($"   Session directory: {session.Directory ?? "(server default)"}");

    // Initialize session with model if specified
    if (providerId != null && modelId != null)
    {
        Console.WriteLine($"   Initializing with model: {providerId}/{modelId}");
        await client.InitializeSessionAsync(session.Id, InitSessionRequest.FromModel(new ModelReference
        {
            ProviderId = providerId,
            ModelId = modelId
        }), directory);
        Console.WriteLine("   Model configured!");
    }
    else
    {
        Console.WriteLine("   Using default model (specify -p and -m to use a specific model)");
    }

    // Clean up the session
    await client.DeleteSessionAsync(session.Id, directory: directory);
    Console.WriteLine("   Session cleaned up.");
    Console.WriteLine();

    // Example 4: List sessions
    Console.WriteLine("4. Listing sessions...");
    var sessions = await client.ListSessionsAsync(directory: directory);
    Console.WriteLine($"   Found {sessions.Count} sessions");
    foreach (var sess in sessions.Take(3))
    {
        var createdDate = DateTimeOffset.FromUnixTimeMilliseconds(sess.Time.Created);
        Console.WriteLine($"   - {sess.Id} (created: {createdDate:g})");
    }
    if (sessions.Count > 3)
    {
        Console.WriteLine($"   ... and {sessions.Count - 3} more");
    }
    Console.WriteLine();

    Console.WriteLine("All examples completed successfully!");
    Console.WriteLine();

    if (providerId == null || modelId == null)
    {
        Console.WriteLine("Tip: Use --list-models to see available models");
        Console.WriteLine("     Use --free to filter to only free models");
        Console.WriteLine("     Use -p <provider> -m <model> to select a specific model");
    }
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
    Console.WriteLine("  -p, --provider <id>     Provider ID (e.g., chutes, opencode, groq)");
    Console.WriteLine("  -m, --model <id>        Model ID (e.g., openai/gpt-oss-20b, gpt-5-nano)");
    Console.WriteLine("  -l, --list-models       List all available providers and models");
    Console.WriteLine("  --free                  Only show free models (use with --list-models)");
    Console.WriteLine("  -h, --help              Show this help message");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  BasicUsage                                    # Basic usage with default model");
    Console.WriteLine("  BasicUsage --list-models                      # List all models");
    Console.WriteLine("  BasicUsage --list-models --free               # List only free models");
    Console.WriteLine("  BasicUsage -p chutes -m openai/gpt-oss-20b    # Use free GPT-OSS model");
    Console.WriteLine("  BasicUsage -p opencode -m gpt-5-nano          # Use free GPT-5 Nano model");
    Console.WriteLine("  BasicUsage -p groq -m llama-3.1-8b-instant    # Use Groq Llama model");
    Console.WriteLine();
    Console.WriteLine("Popular FREE models:");
    Console.WriteLine("  chutes    openai/gpt-oss-20b                  GPT-OSS 20B (reasoning, tool use)");
    Console.WriteLine("  chutes    Alibaba-NLP/Tongyi-DeepResearch-30B-A3B  Tongyi DeepResearch");
    Console.WriteLine("  chutes    unsloth/gemma-3-4b-it               Gemma 3 4B (vision)");
    Console.WriteLine("  opencode  gpt-5-nano                          GPT-5 Nano (reasoning)");
}
