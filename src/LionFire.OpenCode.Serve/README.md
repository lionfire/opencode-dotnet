# LionFire.OpenCode.Serve

A .NET SDK for the OpenCode local headless server API (`opencode serve`).

## Installation

```bash
dotnet add package LionFire.OpenCode.Serve
```

## Quick Start

```csharp
using LionFire.OpenCode.Serve;
using LionFire.OpenCode.Serve.Models;

// Create a client (connects to localhost:9123 by default)
await using var client = new OpenCodeClient();

// Check server health
var health = await client.PingAsync();
Console.WriteLine($"OpenCode version: {health.Version}");

// Create a session and send a message
await using var scope = await client.CreateSessionScopeAsync();
var response = await client.SendMessageAsync(scope.SessionId, "Hello, OpenCode!");

// Get the response text
var text = response.Parts.OfType<TextPart>().FirstOrDefault()?.Text;
Console.WriteLine(text);
```

## Streaming Responses

```csharp
await foreach (var update in client.SendMessageStreamingAsync(sessionId, "Explain async/await"))
{
    if (update.Delta is not null)
    {
        Console.Write(update.Delta);
    }
}
```

## Dependency Injection

```csharp
services.AddOpenCodeClient(options =>
{
    options.BaseUrl = "http://localhost:9123";
    options.MessageTimeout = TimeSpan.FromMinutes(10);
});
```

## Requirements

- .NET 8.0 or later
- OpenCode server running (`opencode serve`)

## License

MIT
