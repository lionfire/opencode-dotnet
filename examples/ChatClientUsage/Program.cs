using LionFire.OpenCode.Serve;
using LionFire.OpenCode.Serve.AgentFramework;
using LionFire.OpenCode.Serve.Exceptions;
using LionFire.OpenCode.Serve.Models;
using Microsoft.Extensions.AI;

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

Console.WriteLine("LionFire.OpenCode.Serve - IChatClient Integration Example");
Console.WriteLine("=".PadRight(55, '='));
Console.WriteLine();

// Create client with options
var clientOptions = new OpenCodeClientOptions
{
    BaseUrl = baseUrl ?? "http://localhost:9876",
    Directory = directory
};

try
{
    // Create OpenCode client
    var openCodeClient = new OpenCodeClient(clientOptions);

    // Handle --list-models flag
    if (listModels)
    {
        Console.WriteLine("Available Providers and Models:");
        Console.WriteLine("-".PadRight(70, '-'));

        var providers = await openCodeClient.ListProvidersAsync(directory);

        foreach (var provider in providers.OrderBy(p => p.Name))
        {
            if (provider.Models == null || provider.Models.Count == 0)
                continue;

            var modelsToShow = freeOnly
                ? provider.Models.Where(m => (m.InputCost ?? 0) == 0 && (m.OutputCost ?? 0) == 0).ToList()
                : provider.Models;

            if (modelsToShow.Count == 0)
                continue;

            Console.WriteLine($"[{provider.Name}] (provider: {provider.Id})");

            foreach (var model in modelsToShow.OrderBy(m => m.Name))
            {
                var costInfo = (model.InputCost ?? 0) == 0 && (model.OutputCost ?? 0) == 0
                    ? "[FREE]"
                    : $"${model.InputCost:F2}/${model.OutputCost:F2}";
                Console.WriteLine($"   {model.Name,-40} {costInfo,-12} (model: {model.Id})");
            }
            Console.WriteLine();
        }

        Console.WriteLine("-".PadRight(70, '-'));
        Console.WriteLine("Usage: ChatClientUsage -p <provider> -m <model>");
        Console.WriteLine("Example: ChatClientUsage -p chutes -m openai/gpt-oss-20b");
        return;
    }

    // Show selected model info
    if (providerId != null && modelId != null)
    {
        Console.WriteLine($"Using model: {providerId}/{modelId}");
    }
    else
    {
        Console.WriteLine("Using default model (use -p and -m to specify a model)");
        Console.WriteLine("Tip: Run with --list-models --free to see free models");
    }
    Console.WriteLine();

    // Create model reference if specified
    ModelReference? modelRef = (providerId != null && modelId != null)
        ? new ModelReference { ProviderId = providerId, ModelId = modelId }
        : null;

    // Example 1: Basic IChatClient usage
    Console.WriteLine("1. Creating IChatClient from OpenCode client...");
    IChatClient chatClient = openCodeClient.AsChatClient(model: modelRef);
    var openCodeChatClient = chatClient as OpenCodeChatClient;
    Console.WriteLine($"   Provider: {openCodeChatClient?.Metadata.ProviderName ?? "OpenCode"}");
    if (modelRef != null)
    {
        Console.WriteLine($"   Model: {modelRef.ProviderId}/{modelRef.ModelId}");
    }
    Console.WriteLine();

    // Example 2: Send a simple message
    Console.WriteLine("2. Sending a message via IChatClient...");
    var messages = new List<ChatMessage>
    {
        new(ChatRole.User, "What is 2 + 2? Reply with just the number.")
    };

    var response = await chatClient.GetResponseAsync(messages);
    Console.WriteLine($"   Response ID: {response.ResponseId}");
    Console.WriteLine($"   Content: {response.Messages[0].Text}");
    Console.WriteLine();

    // Example 3: Multi-turn conversation
    Console.WriteLine("3. Multi-turn conversation...");
    var conversation = new List<ChatMessage>
    {
        new(ChatRole.System, "You are a helpful math tutor. Keep answers brief."),
        new(ChatRole.User, "What is the formula for the area of a circle?")
    };

    var tutorResponse = await chatClient.GetResponseAsync(conversation);
    var tutorText = tutorResponse.Messages[0].Text ?? "";
    Console.WriteLine($"   Tutor: {tutorText.Substring(0, Math.Min(150, tutorText.Length))}...");
    Console.WriteLine();

    // Example 4: Streaming responses
    Console.WriteLine("4. Streaming response (simulated - returns complete response)...");
    Console.Write("   ");

    var streamMessages = new List<ChatMessage>
    {
        new(ChatRole.User, "Count from 1 to 5, putting each number on its own line.")
    };

    // Create a new chat client for streaming (to get fresh session)
    var streamChatClient = openCodeClient.AsChatClient(model: modelRef);

    await foreach (var update in streamChatClient.GetStreamingResponseAsync(streamMessages))
    {
        // Get text content from the update
        foreach (var content in update.Contents)
        {
            if (content is TextContent textContent && !string.IsNullOrEmpty(textContent.Text))
            {
                // Show the response (note: currently returns full response, not true streaming)
                var lines = textContent.Text.Split('\n');
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                        Console.WriteLine(line.Trim());
                }
            }
        }

        if (update.FinishReason == ChatFinishReason.Stop)
        {
            Console.WriteLine("   [Complete]");
        }
    }
    Console.WriteLine();

    // Example 5: Using with specific session and model
    Console.WriteLine("5. Using IChatClient with specific session...");
    var session = await openCodeClient.CreateSessionAsync(directory: directory);

    // Initialize session with model if specified
    if (modelRef != null)
    {
        await openCodeClient.InitializeSessionAsync(session.Id, InitSessionRequest.FromModel(modelRef), directory);
    }

    var sessionChatClient = openCodeClient.AsChatClient(session.Id, model: modelRef);

    var sessionResponse = await sessionChatClient.GetResponseAsync([
        new ChatMessage(ChatRole.User, "Say hello and tell me your name (or model name if you know it).")
    ]);
    var sessionText = sessionResponse.Messages[0].Text ?? "";
    Console.WriteLine($"   Response: {sessionText.Substring(0, Math.Min(100, sessionText.Length))}...");

    // Clean up session
    await openCodeClient.DeleteSessionAsync(session.Id, directory);
    Console.WriteLine("   Session cleaned up.");
    Console.WriteLine();

    // Example 6: Accessing underlying services
    Console.WriteLine("6. Accessing underlying OpenCode client...");
    var underlyingClient = chatClient.GetService(typeof(IOpenCodeClient)) as IOpenCodeClient;
    if (underlyingClient is not null)
    {
        // Demonstrate accessing the underlying client by listing providers
        var providers = await underlyingClient.ListProvidersAsync(directory);
        var freeProviders = providers
            .Where(p => p.Models?.Any(m => (m.InputCost ?? 0) == 0 && (m.OutputCost ?? 0) == 0) == true)
            .Take(5)
            .Select(p => p.Name);
        Console.WriteLine($"   Underlying client type: {underlyingClient.GetType().Name}");
        Console.WriteLine($"   Available providers: {providers.Count}");
        Console.WriteLine($"   Free providers (sample): {string.Join(", ", freeProviders)}");
    }
    else
    {
        Console.WriteLine("   Could not retrieve underlying client");
    }
    Console.WriteLine();

    // Example 7: Quick chat with model selection
    Console.WriteLine("7. Quick chat demonstration...");
    var quickChatClient = openCodeClient.AsChatClient(model: modelRef);
    var quickResponse = await quickChatClient.GetResponseAsync([
        new ChatMessage(ChatRole.User, "What's the capital of France? One word answer.")
    ]);
    Console.WriteLine($"   Q: What's the capital of France?");
    Console.WriteLine($"   A: {quickResponse.Messages[0].Text}");
    Console.WriteLine();

    Console.WriteLine("All examples completed successfully!");
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
}
catch (Exception ex)
{
    Console.WriteLine($"ERROR: {ex.Message}");
}

static void ShowHelp()
{
    Console.WriteLine("LionFire.OpenCode.Serve - IChatClient Integration Example");
    Console.WriteLine();
    Console.WriteLine("Usage: ChatClientUsage [options]");
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
    Console.WriteLine("  ChatClientUsage                                    # Use default model");
    Console.WriteLine("  ChatClientUsage --list-models                      # List all models");
    Console.WriteLine("  ChatClientUsage --list-models --free               # List only free models");
    Console.WriteLine("  ChatClientUsage -p chutes -m openai/gpt-oss-20b    # Use free GPT-OSS model");
    Console.WriteLine("  ChatClientUsage -p opencode -m gpt-5-nano          # Use free GPT-5 Nano model");
    Console.WriteLine("  ChatClientUsage -p groq -m llama-3.1-8b-instant    # Use Groq Llama model");
    Console.WriteLine();
    Console.WriteLine("Popular FREE models (no API key required for some):");
    Console.WriteLine("  chutes    openai/gpt-oss-20b                  GPT-OSS 20B (reasoning, tool use)");
    Console.WriteLine("  chutes    Alibaba-NLP/Tongyi-DeepResearch-30B-A3B  Tongyi DeepResearch");
    Console.WriteLine("  chutes    unsloth/gemma-3-4b-it               Gemma 3 4B (vision)");
    Console.WriteLine("  opencode  gpt-5-nano                          GPT-5 Nano (reasoning)");
    Console.WriteLine();
    Console.WriteLine("Note: Free models may still require provider authentication.");
    Console.WriteLine("      Run: opencode auth login <provider>");
}
