// Basic Session Example
// Demonstrates creating a session, sending a message, and getting a response

using LionFire.OpenCode.Serve;
using LionFire.OpenCode.Serve.Exceptions;
using LionFire.OpenCode.Serve.Models;

Console.WriteLine("LionFire.OpenCode.Serve - Basic Session Example");
Console.WriteLine("================================================");
Console.WriteLine();

try
{
    // Create the client (connects to localhost:9123 by default)
    await using var client = new OpenCodeClient();
    Console.WriteLine("✓ Connected to OpenCode server");

    // Create a new session
    var session = await client.CreateSessionAsync();
    Console.WriteLine($"✓ Created session: {session.Id}");

    try
    {
        // Send a message
        var request = new SendMessageRequest
        {
            Parts = new List<PartInput>
            {
                PartInput.TextInput("What is the capital of France? Answer in one sentence.")
            }
        };

        Console.WriteLine();
        Console.WriteLine("> Sending message: What is the capital of France?");
        Console.WriteLine();

        // Get the response
        var response = await client.PromptAsync(session.Id, request);

        // Extract and display the text response
        Console.WriteLine("< Response:");
        foreach (var part in response.Parts ?? new List<Part>())
        {
            if (part.IsTextPart)
            {
                Console.WriteLine(part.Text);
            }
        }

        // Display some metadata
        if (response.Message != null)
        {
            Console.WriteLine();
            Console.WriteLine($"Message ID: {response.Message.Id}");
            if (response.Message.Tokens != null)
            {
                Console.WriteLine($"Tokens used: Input={response.Message.Tokens.Input}, Output={response.Message.Tokens.Output}");
            }
            if (response.Message.Cost.HasValue)
            {
                Console.WriteLine($"Cost: ${response.Message.Cost:F6}");
            }
        }
    }
    finally
    {
        // Clean up the session
        await client.DeleteSessionAsync(session.Id);
        Console.WriteLine();
        Console.WriteLine("✓ Session cleaned up");
    }
}
catch (OpenCodeConnectionException ex)
{
    Console.WriteLine($"❌ Connection Error: {ex.Message}");
    Console.WriteLine();
    Console.WriteLine("Make sure the OpenCode server is running:");
    Console.WriteLine("  opencode serve");
}
catch (OpenCodeException ex)
{
    Console.WriteLine($"❌ Error: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("Done!");
