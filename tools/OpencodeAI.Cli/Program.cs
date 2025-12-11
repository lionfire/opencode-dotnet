// Simple CLI for testing OpencodeAI SDK
// Usage: dotnet run -- "your prompt here" [--model claude-3-5-sonnet] [--base-url http://localhost:8080]

using System.CommandLine;
using OpencodeAI;
using OpencodeAI.Models.Requests;

var promptArg = new Argument<string>("prompt", "The prompt to send to the API");
var modelOption = new Option<string?>("--model", () => null, "Model to use (e.g., claude-3-5-sonnet-20241022)");
var baseUrlOption = new Option<string?>("--base-url", () => null, "Base URL for the API (default: from env or https://api.opencode.ai/v1)");
var apiKeyOption = new Option<string?>("--api-key", () => null, "API key (default: from OPENCODE_API_KEY env var)");
var languageOption = new Option<string>("--language", () => "csharp", "Programming language for code generation");
var streamOption = new Option<bool>("--stream", () => false, "Use streaming mode");

var rootCommand = new RootCommand("OpencodeAI CLI - Test the .NET SDK")
{
    promptArg,
    modelOption,
    baseUrlOption,
    apiKeyOption,
    languageOption,
    streamOption,
};

rootCommand.SetHandler(async (prompt, model, baseUrl, apiKey, language, stream) =>
{
    try
    {
        // Build options
        var options = new OpencodeClientOptions();

        // Apply API key
        if (!string.IsNullOrEmpty(apiKey))
        {
            options.ApiKey = apiKey;
        }
        else
        {
            var envKey = Environment.GetEnvironmentVariable("OPENCODE_API_KEY");
            if (string.IsNullOrEmpty(envKey))
            {
                Console.Error.WriteLine("Error: No API key provided. Use --api-key or set OPENCODE_API_KEY environment variable.");
                Environment.Exit(1);
            }
            options.ApiKey = envKey;
        }

        // Apply base URL
        if (!string.IsNullOrEmpty(baseUrl))
        {
            options.BaseUrl = baseUrl;
        }
        else
        {
            var envUrl = Environment.GetEnvironmentVariable("OPENCODE_BASE_URL");
            if (!string.IsNullOrEmpty(envUrl))
            {
                options.BaseUrl = envUrl;
            }
        }

        // Apply model
        if (!string.IsNullOrEmpty(model))
        {
            options.DefaultModel = model;
        }

        // Disable API key format validation since we might use different key formats
        options.ValidateApiKeyFormat = false;

        Console.WriteLine($"Connecting to: {options.BaseUrl}");
        Console.WriteLine($"Model: {options.DefaultModel ?? "(default)"}");
        Console.WriteLine($"Language: {language}");
        Console.WriteLine();

        using var client = new OpencodeClient(options);

        var request = new GenerateCodeRequest
        {
            Prompt = prompt,
            Language = language,
            MaxTokens = 2048,
        };

        if (stream)
        {
            Console.WriteLine("Response (streaming):");
            Console.WriteLine("---------------------");

            await foreach (var chunk in client.StreamCodeAsync(request))
            {
                Console.Write(chunk.Code);
            }
            Console.WriteLine();
        }
        else
        {
            Console.WriteLine("Generating response...");
            var response = await client.GenerateCodeAsync(request);

            Console.WriteLine();
            Console.WriteLine("Response:");
            Console.WriteLine("---------");
            Console.WriteLine(response.Code);

            if (response.Usage != null)
            {
                Console.WriteLine();
                Console.WriteLine($"Tokens - Prompt: {response.Usage.PromptTokens}, Completion: {response.Usage.CompletionTokens}, Total: {response.Usage.TotalTokens}");
            }

            if (!string.IsNullOrEmpty(response.Model))
            {
                Console.WriteLine($"Model: {response.Model}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
        if (ex.InnerException != null)
        {
            Console.Error.WriteLine($"Inner: {ex.InnerException.Message}");
        }
        Environment.Exit(1);
    }
}, promptArg, modelOption, baseUrlOption, apiKeyOption, languageOption, streamOption);

return await rootCommand.InvokeAsync(args);
