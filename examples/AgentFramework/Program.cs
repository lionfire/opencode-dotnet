// Agent Framework Example
// Demonstrates using Microsoft.Extensions.AI abstractions with OpenCode

using LionFire.OpenCode.Serve;
using LionFire.OpenCode.Serve.AgentFramework;
using LionFire.OpenCode.Serve.Exceptions;
using Microsoft.Extensions.AI;

Console.WriteLine("LionFire.OpenCode.Serve - Agent Framework Example");
Console.WriteLine("==================================================");
Console.WriteLine();

try
{
    // Create the OpenCode client
    var openCodeClient = new OpenCodeClient();
    Console.WriteLine("✓ Connected to OpenCode server");

    // Wrap it as an IChatClient
    var chatClient = new OpenCodeChatClient(openCodeClient, new OpenCodeChatClientOptions
    {
        ModelId = "opencode"
    });
    Console.WriteLine("✓ Created IChatClient wrapper");
    Console.WriteLine();

    // Example 1: Simple single-turn conversation
    Console.WriteLine("Example 1: Single-turn conversation");
    Console.WriteLine("------------------------------------");

    var response1 = await chatClient.GetResponseAsync(new[]
    {
        new ChatMessage(ChatRole.User, "What programming language is C# similar to?")
    });

    Console.WriteLine($"Response: {response1.Messages.FirstOrDefault()?.Text}");
    Console.WriteLine();

    // Example 2: Multi-turn conversation
    Console.WriteLine("Example 2: Multi-turn conversation");
    Console.WriteLine("-----------------------------------");

    var conversation = new List<ChatMessage>
    {
        new ChatMessage(ChatRole.System, "You are a helpful assistant that gives brief answers."),
        new ChatMessage(ChatRole.User, "What is .NET?")
    };

    var response2 = await chatClient.GetResponseAsync(conversation);
    Console.WriteLine($"Q: What is .NET?");
    Console.WriteLine($"A: {response2.Messages.FirstOrDefault()?.Text}");

    // Continue the conversation
    conversation.Add(new ChatMessage(ChatRole.Assistant, response2.Messages.FirstOrDefault()?.Text ?? ""));
    conversation.Add(new ChatMessage(ChatRole.User, "What languages can I use with it?"));

    var response3 = await chatClient.GetResponseAsync(conversation);
    Console.WriteLine();
    Console.WriteLine($"Q: What languages can I use with it?");
    Console.WriteLine($"A: {response3.Messages.FirstOrDefault()?.Text}");
    Console.WriteLine();

    // Example 3: Streaming response
    Console.WriteLine("Example 3: Streaming response");
    Console.WriteLine("------------------------------");

    Console.Write("Response: ");
    await foreach (var update in chatClient.GetStreamingResponseAsync(new[]
    {
        new ChatMessage(ChatRole.User, "Count from 1 to 3.")
    }))
    {
        foreach (var content in update.Contents)
        {
            if (content is TextContent text)
            {
                Console.Write(text.Text);
            }
        }
    }
    Console.WriteLine();
    Console.WriteLine();

    // Example 4: Using GetService to access underlying client
    Console.WriteLine("Example 4: Accessing underlying services");
    Console.WriteLine("-----------------------------------------");

    var underlyingClient = chatClient.GetService<IOpenCodeClient>();
    if (underlyingClient != null)
    {
        var projects = await underlyingClient.ListProjectsAsync();
        Console.WriteLine($"Found {projects.Count} project(s) via underlying client");
    }

    Console.WriteLine();
    Console.WriteLine("All examples completed successfully!");
}
catch (OpenCodeConnectionException ex)
{
    Console.WriteLine($"❌ Connection Error: {ex.Message}");
    Console.WriteLine("Make sure the OpenCode server is running: opencode serve");
}
catch (OpenCodeException ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("Done!");
