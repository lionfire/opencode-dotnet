// Error Handling and Resilience Example
// Demonstrates proper exception handling patterns and Polly resilience integration

using LionFire.OpenCode.Serve;
using LionFire.OpenCode.Serve.Exceptions;
using LionFire.OpenCode.Serve.Extensions;
using LionFire.OpenCode.Serve.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Polly.CircuitBreaker;

Console.WriteLine("LionFire.OpenCode.Serve - Error Handling & Resilience Example");
Console.WriteLine("==============================================================");
Console.WriteLine();

// Example 1: Connection error handling
Console.WriteLine("Example 1: Connection Error (wrong URL)");
Console.WriteLine("----------------------------------------");
try
{
    // Try to connect to a non-existent server
    var badClient = new OpenCodeClient(new OpenCodeClientOptions
    {
        BaseUrl = "http://localhost:9999"  // Wrong port
    });

    await badClient.ListSessionsAsync();
}
catch (OpenCodeConnectionException ex)
{
    Console.WriteLine($"  Exception Type: {ex.GetType().Name}");
    Console.WriteLine($"  Message: {ex.Message}");
    Console.WriteLine($"  Server URL: {ex.BaseUrl}");
}
Console.WriteLine();

// Example 2: Not found error handling
Console.WriteLine("Example 2: Not Found Error (invalid session ID)");
Console.WriteLine("------------------------------------------------");
try
{
    await using var client = new OpenCodeClient();
    await client.GetSessionAsync("ses_nonexistent_id");
}
catch (OpenCodeNotFoundException ex)
{
    Console.WriteLine($"  Exception Type: {ex.GetType().Name}");
    Console.WriteLine($"  Message: {ex.Message}");
    Console.WriteLine($"  Status Code: {ex.StatusCode}");
}
catch (OpenCodeConnectionException)
{
    Console.WriteLine("  (Skipped - OpenCode server not running)");
}
Console.WriteLine();

// Example 3: Polly Resilience with Dependency Injection
Console.WriteLine("Example 3: Polly Resilience Policies with DI");
Console.WriteLine("---------------------------------------------");
await DemonstratePollyResilienceAsync();
Console.WriteLine();

// Example 4: Retry pattern with exponential backoff and jitter
Console.WriteLine("Example 4: Manual Retry Pattern (without Polly)");
Console.WriteLine("------------------------------------------------");
try
{
    var result = await ExecuteWithRetryAsync(
        operation: async () =>
        {
            await using var client = new OpenCodeClient();
            return await client.ListProjectsAsync();
        },
        maxAttempts: 3);

    Console.WriteLine($"  Success! Found {result.Count} project(s)");
}
catch (OpenCodeConnectionException)
{
    Console.WriteLine("  (Failed after retries - OpenCode server not running)");
}
Console.WriteLine();

// Example 5: Graceful degradation
Console.WriteLine("Example 5: Graceful Degradation");
Console.WriteLine("--------------------------------");
var response = await TryGetAIResponseAsync("What is 2+2?");
Console.WriteLine($"  Response: {response}");
Console.WriteLine();

// Example 6: Circuit breaker behavior demonstration
Console.WriteLine("Example 6: Circuit Breaker Pattern");
Console.WriteLine("-----------------------------------");
await DemonstrateCircuitBreakerAsync();
Console.WriteLine();

// Example 7: Comprehensive error handling
Console.WriteLine("Example 7: Comprehensive Error Handler");
Console.WriteLine("---------------------------------------");
await DemonstrateComprehensiveHandlingAsync();

Console.WriteLine();
Console.WriteLine("Done!");

// Helper: Demonstrate Polly resilience with DI
static async Task DemonstratePollyResilienceAsync()
{
    Console.WriteLine("  Setting up DI with Polly resilience policies...");

    // Build a service collection with resilience policies
    var services = new ServiceCollection();

    // Add logging
    services.AddLogging(builder =>
    {
        builder.AddConsole();
        builder.SetMinimumLevel(LogLevel.Information);
    });

    // Add OpenCode client with all resilience policies
    services.AddOpenCodeClient(options =>
    {
        options.BaseUrl = "http://localhost:9123";

        // Retry settings
        options.EnableRetry = true;
        options.MaxRetryAttempts = 3;
        options.RetryDelaySeconds = 1;
        options.MaxRetryJitter = TimeSpan.FromMilliseconds(500);

        // Circuit breaker settings
        options.EnableCircuitBreaker = true;
        options.CircuitBreakerThreshold = 5;
        options.CircuitBreakerDuration = TimeSpan.FromSeconds(30);

        // Per-operation timeout
        options.EnableOperationTimeout = true;
        options.OperationTimeout = TimeSpan.FromSeconds(10);
    })
    .AddOpenCodeResiliencePolicies(); // Add all Polly policies

    var provider = services.BuildServiceProvider();
    var client = provider.GetRequiredService<IOpenCodeClient>();

    Console.WriteLine("  Configuration:");
    Console.WriteLine("    - Retry: 3 attempts with exponential backoff + jitter");
    Console.WriteLine("    - Circuit Breaker: Opens after 5 failures, stays open 30s");
    Console.WriteLine("    - Timeout: 10s per operation");
    Console.WriteLine();

    Console.WriteLine("  Making request with resilience policies...");
    try
    {
        var sessions = await client.ListSessionsAsync();
        Console.WriteLine($"  Success! Found {sessions.Count} session(s)");
    }
    catch (OpenCodeConnectionException)
    {
        Console.WriteLine("  Connection failed - but retries were attempted automatically");
    }
    catch (BrokenCircuitException)
    {
        Console.WriteLine("  Circuit breaker is open - requests failing fast");
    }
}

// Helper: Demonstrate circuit breaker behavior
static async Task DemonstrateCircuitBreakerAsync()
{
    Console.WriteLine("  Circuit Breaker Pattern prevents cascading failures:");
    Console.WriteLine();
    Console.WriteLine("  States:");
    Console.WriteLine("    CLOSED   -> Normal operation, requests allowed");
    Console.WriteLine("    OPEN     -> After threshold failures, requests fail fast");
    Console.WriteLine("    HALF-OPEN -> Testing if service recovered");
    Console.WriteLine();

    var services = new ServiceCollection();
    services.AddLogging(builder => builder.SetMinimumLevel(LogLevel.Warning));

    // Configure with low threshold for demonstration
    services.AddOpenCodeClient(options =>
    {
        options.BaseUrl = "http://localhost:9999"; // Non-existent server
        options.EnableRetry = false; // Disable retry for faster demo
        options.CircuitBreakerThreshold = 3;
        options.CircuitBreakerDuration = TimeSpan.FromSeconds(5);
    })
    .AddOpenCodeCircuitBreakerPolicy();

    var provider = services.BuildServiceProvider();
    var client = provider.GetRequiredService<IOpenCodeClient>();

    Console.WriteLine("  Simulating failures to trigger circuit breaker...");

    for (int i = 1; i <= 5; i++)
    {
        try
        {
            await client.ListSessionsAsync();
            Console.WriteLine($"    Request {i}: Success");
        }
        catch (BrokenCircuitException)
        {
            Console.WriteLine($"    Request {i}: CIRCUIT OPEN - Failing fast (no network call)");
        }
        catch (OpenCodeConnectionException)
        {
            Console.WriteLine($"    Request {i}: Connection failed (circuit still closed)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"    Request {i}: {ex.GetType().Name}");
        }
    }

    Console.WriteLine();
    Console.WriteLine("  After circuit opens, requests fail immediately without network calls.");
    Console.WriteLine("  This prevents overwhelming a struggling service.");
}

// Helper: Retry with exponential backoff
static async Task<T> ExecuteWithRetryAsync<T>(
    Func<Task<T>> operation,
    int maxAttempts = 3)
{
    for (int attempt = 1; attempt <= maxAttempts; attempt++)
    {
        try
        {
            Console.WriteLine($"  Attempt {attempt}...");
            return await operation();
        }
        catch (OpenCodeConnectionException) when (attempt < maxAttempts)
        {
            var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt));
            // Add jitter to prevent thundering herd
            var jitter = TimeSpan.FromMilliseconds(Random.Shared.NextDouble() * 1000);
            var totalDelay = delay + jitter;
            Console.WriteLine($"  Failed, waiting {totalDelay.TotalSeconds:F1}s before retry (with jitter)...");
            await Task.Delay(totalDelay);
        }
        catch (OpenCodeTimeoutException) when (attempt < maxAttempts)
        {
            var delay = TimeSpan.FromSeconds(5);
            Console.WriteLine($"  Timeout, waiting {delay.TotalSeconds}s before retry...");
            await Task.Delay(delay);
        }
    }

    throw new InvalidOperationException("Exhausted all retry attempts");
}

// Helper: Graceful degradation
static async Task<string> TryGetAIResponseAsync(string question)
{
    try
    {
        await using var client = new OpenCodeClient(new OpenCodeClientOptions
        {
            DefaultTimeout = TimeSpan.FromSeconds(5)
        });

        var session = await client.CreateSessionAsync();
        try
        {
            var response = await client.PromptAsync(session.Id, new SendMessageRequest
            {
                Parts = new List<PartInput> { PartInput.TextInput(question) }
            });
            return response.Parts?.FirstOrDefault(p => p.IsTextPart)?.Text ?? "[No response]";
        }
        finally
        {
            await client.DeleteSessionAsync(session.Id);
        }
    }
    catch (OpenCodeConnectionException)
    {
        // Fallback when OpenCode is unavailable
        return "[Fallback] OpenCode unavailable - using cached response: 4";
    }
    catch (OpenCodeTimeoutException)
    {
        return "[Fallback] Request timed out";
    }
    catch (OpenCodeException ex)
    {
        return $"[Error] {ex.Message}";
    }
}

// Helper: Comprehensive error handling
static async Task DemonstrateComprehensiveHandlingAsync()
{
    try
    {
        await using var client = new OpenCodeClient();
        var session = await client.CreateSessionAsync();

        try
        {
            await client.PromptAsync(session.Id, new SendMessageRequest
            {
                Parts = new List<PartInput> { PartInput.TextInput("Hello") }
            });
            Console.WriteLine("  Success!");
        }
        finally
        {
            await client.DeleteSessionAsync(session.Id);
        }
    }
    catch (BrokenCircuitException ex)
    {
        // Circuit breaker is open
        Console.WriteLine($"  [Circuit Open] {ex.Message}");
        Console.WriteLine("  Hint: Wait for circuit to reset or check service health");
        // Action: Use cached data, queue request, or show degraded experience
    }
    catch (OpenCodeConnectionException ex)
    {
        // Server not reachable
        Console.WriteLine($"  [Connection Error] {ex.Message}");
        Console.WriteLine($"  Hint: Ensure 'opencode serve' is running at {ex.BaseUrl}");
        // Action: Show offline mode, queue for retry, alert ops
    }
    catch (OpenCodeNotFoundException ex)
    {
        // Resource doesn't exist
        Console.WriteLine($"  [Not Found] {ex.Message}");
        // Action: Clean up stale references, recreate resource
    }
    catch (OpenCodeTimeoutException ex)
    {
        // Operation took too long
        Console.WriteLine($"  [Timeout] {ex.Message} after {ex.Timeout.TotalSeconds}s");
        Console.WriteLine($"  Hint: Increase timeout in OpenCodeClientOptions or simplify request");
        // Action: Increase timeout, simplify request, queue for retry
    }
    catch (OpenCodeApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
    {
        // Rate limited
        Console.WriteLine($"  [Rate Limited] {ex.Message}");
        // Action: Implement backoff, queue requests
    }
    catch (OpenCodeApiException ex)
    {
        // Other HTTP errors
        Console.WriteLine($"  [API Error {(int)ex.StatusCode}] {ex.Message}");
        // Action: Log, alert, possibly retry
    }
    catch (OpenCodeException ex)
    {
        // Any other SDK error
        Console.WriteLine($"  [SDK Error] {ex.GetType().Name}: {ex.Message}");
        // Action: Log, investigate
    }
}
