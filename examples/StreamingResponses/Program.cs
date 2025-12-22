// Streaming Responses Example
// Demonstrates subscribing to events and handling streaming responses
// With automatic model fallback for free models

using System.Threading.Channels;
using LionFire.OpenCode.Serve;
using LionFire.OpenCode.Serve.Exceptions;
using LionFire.OpenCode.Serve.Models;
using LionFire.OpenCode.Serve.Models.Events;

Console.WriteLine("LionFire.OpenCode.Serve - Streaming Responses Example");
Console.WriteLine("======================================================");
Console.WriteLine();

try
{
    await using var client = new OpenCodeClient();
    Console.WriteLine("✓ Connected to OpenCode server");

    // List available providers and their models
    Console.WriteLine();
    Console.WriteLine("Checking available providers and models...");
    var providers = await client.ListProvidersAsync();

    // Find free models (where cost is 0 or null)
    var freeModels = new List<(string ProviderId, string ModelId, string ModelName)>();
    foreach (var provider in providers)
    {
        if (provider.ModelsDict != null)
        {
            foreach (var (modelId, model) in provider.ModelsDict)
            {
                var isFree = model.Cost == null || (model.Cost.Input == 0 && model.Cost.Output == 0);
                if (isFree)
                {
                    freeModels.Add((provider.Id, modelId, model.Name));
                }
            }
        }
    }

    // Prefer OpenCode provider models first
    var preferredModelNames = new[] { "gpt-5-nano", "big-pickle", "grok-code-fast" };
    var openCodeModels = freeModels
        .Where(m => m.ProviderId.Equals("opencode", StringComparison.OrdinalIgnoreCase))
        .OrderBy(m =>
        {
            // Prioritize known good models
            var idx = Array.FindIndex(preferredModelNames, p =>
                m.ModelId.Contains(p, StringComparison.OrdinalIgnoreCase) ||
                m.ModelName.Contains(p, StringComparison.OrdinalIgnoreCase));
            return idx >= 0 ? idx : 999;
        })
        .ToList();

    var otherFreeModels = freeModels
        .Where(m => !m.ProviderId.Equals("opencode", StringComparison.OrdinalIgnoreCase))
        .ToList();

    Console.WriteLine($"Found {freeModels.Count} free models total");
    Console.WriteLine();
    Console.WriteLine($"OpenCode provider models ({openCodeModels.Count}):");
    foreach (var (providerId, modelId, modelName) in openCodeModels.Take(10))
    {
        Console.WriteLine($"  - {modelId} ({modelName})");
    }
    if (openCodeModels.Count > 10)
    {
        Console.WriteLine($"  ... and {openCodeModels.Count - 10} more");
    }

    if (openCodeModels.Count == 0)
    {
        Console.WriteLine("  (none found)");
        Console.WriteLine();
        Console.WriteLine($"Other free models ({otherFreeModels.Count}):");
        foreach (var (providerId, modelId, modelName) in otherFreeModels.Take(5))
        {
            Console.WriteLine($"  - {providerId}/{modelId} ({modelName})");
        }
    }

    // Create session
    Console.WriteLine();
    var session = await client.CreateSessionAsync();
    Console.WriteLine($"✓ Created session: {session.Id}");

    try
    {
        // Try OpenCode models first, then default, then other free models
        var modelsToTry = new List<ModelReference?>();

        // Add OpenCode provider models first (preferred)
        modelsToTry.AddRange(openCodeModels.Take(5).Select(m => new ModelReference
        {
            ProviderId = m.ProviderId,
            ModelId = m.ModelId
        }));

        // Then try default model
        modelsToTry.Add(null);

        // Then other free models
        modelsToTry.AddRange(otherFreeModels.Take(3).Select(m => new ModelReference
        {
            ProviderId = m.ProviderId,
            ModelId = m.ModelId
        }));

        foreach (var modelRef in modelsToTry)
        {
            var modelDesc = modelRef == null
                ? "default model"
                : $"{modelRef.ProviderId}/{modelRef.ModelId}";

            Console.WriteLine();
            Console.WriteLine($"Trying {modelDesc}...");

            var (success, actualModel, wasStreaming) = await TryStreamingWithModel(client, session.Id, modelRef);

            if (success)
            {
                Console.WriteLine();
                var streamingStatus = wasStreaming ? "✓ STREAMED" : "(non-streaming)";
                if (actualModel != null)
                {
                    Console.WriteLine($"{streamingStatus} using: {actualModel.Value.ProviderId}/{actualModel.Value.ModelId}");
                }
                else
                {
                    Console.WriteLine($"{streamingStatus} with {modelDesc}");
                }
                break;
            }
            else
            {
                Console.WriteLine($"✗ Failed with {modelDesc}, trying next...");
            }
        }
    }
    finally
    {
        await client.DeleteSessionAsync(session.Id);
        Console.WriteLine();
        Console.WriteLine("✓ Session cleaned up");
    }
}
catch (OpenCodeConnectionException ex)
{
    Console.WriteLine($"❌ Connection Error: {ex.Message}");
    Console.WriteLine("Make sure the OpenCode server is running: opencode serve");
}
catch (OperationCanceledException)
{
    Console.WriteLine();
    Console.WriteLine("Operation timed out or was cancelled.");
}
catch (OpenCodeException ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("Done!");

// Helper method to try streaming with a specific model
static async Task<(bool Success, (string ProviderId, string ModelId)? ActualModel, bool WasStreaming)> TryStreamingWithModel(
    OpenCodeClient client, string sessionId, ModelReference? model)
{
    (string ProviderId, string ModelId)? actualModel = null;
    var gotStreamingContent = false;

    try
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(60));

        // Use a channel to buffer events from the background subscription
        var eventChannel = Channel.CreateUnbounded<Event>();
        var connectionReady = new TaskCompletionSource<bool>();

        // Start event subscription in background BEFORE sending prompt
        var subscriptionTask = Task.Run(async () =>
        {
            try
            {
                var firstEvent = true;
                await foreach (var evt in client.SubscribeToEventsAsync(cancellationToken: cts.Token))
                {
                    if (firstEvent)
                    {
                        // Signal that the connection is ready after receiving first event
                        // or we can signal immediately after the connection is established
                        firstEvent = false;
                    }
                    await eventChannel.Writer.WriteAsync(evt, cts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                // Expected when cancelled
            }
            finally
            {
                eventChannel.Writer.Complete();
            }
        }, cts.Token);

        // Give the SSE connection a moment to establish
        // The connection is established when we first iterate, so we need to wait
        await Task.Delay(100, cts.Token);

        // Now send the prompt
        var request = new SendMessageRequest
        {
            Parts = new List<PartInput>
            {
                PartInput.TextInput("Write a C# console Program.cs file that finds and prints the first 10 prime numbers. Include comments explaining the logic.")
            },
            Model = model
        };

        await client.PromptAsyncNonBlocking(sessionId, request);

        Console.Write("> ");

        var responseComplete = false;
        var gotContent = false;

        // Read events from the channel
        await foreach (var evt in eventChannel.Reader.ReadAllAsync(cts.Token))
        {
            switch (evt)
            {
                case MessagePartUpdatedEvent partUpdated when
                    (partUpdated.Properties?.SessionId == sessionId ||
                     partUpdated.Properties?.Part?.SessionId == sessionId ||
                     string.IsNullOrEmpty(partUpdated.Properties?.SessionId)):
                    if (partUpdated.Properties?.Delta != null)
                    {
                        Console.Write(partUpdated.Properties.Delta);
                        gotContent = true;
                        gotStreamingContent = true;
                    }
                    break;

                case MessageUpdatedEvent messageUpdated when messageUpdated.Properties?.Info?.SessionId == sessionId:
                    // Capture the model info
                    if (messageUpdated.Properties?.Info?.Model != null)
                    {
                        var modelInfo = messageUpdated.Properties.Info.Model;
                        if (!string.IsNullOrEmpty(modelInfo.ProviderId) && !string.IsNullOrEmpty(modelInfo.ModelId))
                        {
                            actualModel = (modelInfo.ProviderId, modelInfo.ModelId);
                        }
                    }
                    break;

                case SessionIdleEvent idle when idle.Properties?.SessionId == sessionId:
                    Console.WriteLine();
                    responseComplete = true;
                    break;

                case SessionErrorEvent error when error.Properties?.SessionId == sessionId:
                    Console.WriteLine();
                    Console.WriteLine($"  [Error: {error.Properties?.Error}]");
                    responseComplete = true;
                    break;

                case SessionStatusEvent status when status.Properties?.SessionId == sessionId:
                    if (status.Properties?.Status is SessionStatusIdle)
                    {
                        Console.WriteLine();
                        responseComplete = true;
                    }
                    break;
            }

            if (responseComplete)
            {
                cts.Cancel(); // Stop the subscription
                break;
            }
        }

        // If we didn't get streaming events, try to fetch the final response
        if (!gotContent)
        {
            Console.WriteLine();
            Console.WriteLine("  (No streaming events, checking final response...)");

            using var fetchCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var messages = await client.ListMessagesAsync(sessionId, limit: 2, cancellationToken: fetchCts.Token);
            var assistantMessage = messages.LastOrDefault(m => m.Message?.Role == "assistant");

            if (assistantMessage?.Message != null)
            {
                // Get the model used
                var msg = assistantMessage.Message;
                if (!string.IsNullOrEmpty(msg.ProviderId) && !string.IsNullOrEmpty(msg.ModelId))
                {
                    actualModel = (msg.ProviderId, msg.ModelId);
                }
            }

            if (assistantMessage?.Parts != null)
            {
                foreach (var part in assistantMessage.Parts)
                {
                    if (part.IsTextPart && !string.IsNullOrEmpty(part.Text))
                    {
                        Console.WriteLine($"  Response: {part.Text}");
                        gotContent = true;
                    }
                }
            }

            // Check for errors
            if (assistantMessage?.Message?.Error != null)
            {
                Console.WriteLine($"  Error: {assistantMessage.Message.Error.ErrorMessage}");
                return (false, null, false);
            }
        }

        return (gotContent, actualModel, gotStreamingContent);
    }
    catch (TimeoutException)
    {
        Console.WriteLine();
        Console.WriteLine("  [Timeout]");
        return (false, null, false);
    }
    catch (OperationCanceledException)
    {
        // This is expected when we cancel after completion
        return (true, actualModel, gotStreamingContent);
    }
    catch (OpenCodeException ex)
    {
        Console.WriteLine();
        Console.WriteLine($"  [Error: {ex.Message}]");
        return (false, null, false);
    }
}
