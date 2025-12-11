# OpencodeAgent API Reference

## Namespace

```csharp
namespace OpencodeAI.AgentFramework;
```

## Classes

### OpencodeAgent

An AI agent implementation that wraps `IOpencodeClient` for use with Microsoft Agent Framework.

```csharp
public sealed class OpencodeAgent : AIAgent
```

#### Constructors

##### OpencodeAgent(IOpencodeClient)

Creates a new OpencodeAgent with default settings.

```csharp
public OpencodeAgent(IOpencodeClient client);
```

**Parameters:**
- `client` - The OpencodeAI client to wrap.

**Exceptions:**
- `ArgumentNullException` - Thrown when `client` is null.

**Example:**
```csharp
// Connect to local OpenCode server
var client = new OpencodeClient("http://localhost:9123");
var agent = new OpencodeAgent(client);
```

##### OpencodeAgent(IOpencodeClient, OpencodeAgentOptions)

Creates a new OpencodeAgent with specified options.

```csharp
public OpencodeAgent(IOpencodeClient client, OpencodeAgentOptions options);
```

**Parameters:**
- `client` - The OpencodeAI client to wrap.
- `options` - Configuration options for the agent.

**Exceptions:**
- `ArgumentNullException` - Thrown when `client` is null.

**Example:**
```csharp
// Connect to local OpenCode server with options
var client = new OpencodeClient("http://localhost:9123");
var options = new OpencodeAgentOptions
{
    Name = "CodeAssistant",
    Description = "A helpful coding assistant",
    SystemPrompt = "You are an expert C# developer."
};
var agent = new OpencodeAgent(client, options);
```

##### OpencodeAgent(IOpencodeClient, string?, string?)

Creates a new OpencodeAgent with name and description.

```csharp
public OpencodeAgent(IOpencodeClient client, string? name, string? description);
```

**Parameters:**
- `client` - The OpencodeAI client to wrap.
- `name` - Optional display name for the agent.
- `description` - Optional description of the agent's purpose.

**Example:**
```csharp
var agent = new OpencodeAgent(client, "CodeReviewer", "Reviews code for quality and security");
```

#### Properties

##### Id

Gets the unique identifier for this agent instance.

```csharp
public override string Id { get; }
```

**Returns:** A unique string identifier (GUID by default).

##### Name

Gets the human-readable name of the agent.

```csharp
public override string? Name { get; }
```

**Returns:** The agent's name, or null if not specified.

##### Description

Gets a description of the agent's purpose.

```csharp
public override string? Description { get; }
```

**Returns:** The agent's description, or null if not specified.

##### DisplayName

Gets a display-friendly name for the agent.

```csharp
public override string DisplayName { get; }
```

**Returns:** The Name if available, otherwise the Id.

##### Client

Gets the underlying OpencodeAI client.

```csharp
public IOpencodeClient Client { get; }
```

**Returns:** The wrapped `IOpencodeClient` instance.

#### Methods

##### GetNewThread()

Creates a new conversation thread.

```csharp
public override AgentThread GetNewThread();
```

**Returns:** A new `OpencodeAgentThread` instance.

**Example:**
```csharp
var thread = agent.GetNewThread();
await agent.RunAsync("Hello!", thread);
await agent.RunAsync("How are you?", thread); // Maintains context
```

##### DeserializeThread(JsonElement, JsonSerializerOptions?)

Restores a thread from its serialized representation.

```csharp
public override AgentThread DeserializeThread(
    JsonElement serializedThread,
    JsonSerializerOptions? jsonSerializerOptions = null);
```

**Parameters:**
- `serializedThread` - The serialized thread state.
- `jsonSerializerOptions` - Optional JSON serialization options.

**Returns:** A restored `OpencodeAgentThread` instance.

**Exceptions:**
- `ArgumentException` - The serialized data is not in the expected format.
- `JsonException` - The serialized data is invalid.

**Example:**
```csharp
// Save thread state
var threadJson = thread.Serialize();
SaveToDatabase(threadJson);

// Later, restore the thread
var savedJson = LoadFromDatabase();
var restoredThread = agent.DeserializeThread(savedJson);
await agent.RunAsync("Continue our conversation", restoredThread);
```

##### RunAsync(IEnumerable\<ChatMessage\>, AgentThread?, AgentRunOptions?, CancellationToken)

Runs the agent with a collection of chat messages.

```csharp
public override Task<AgentRunResponse> RunAsync(
    IEnumerable<ChatMessage> messages,
    AgentThread? thread = null,
    AgentRunOptions? options = null,
    CancellationToken cancellationToken = default);
```

**Parameters:**
- `messages` - The messages to send to the agent.
- `thread` - Optional conversation thread. Created if null.
- `options` - Optional run configuration.
- `cancellationToken` - Cancellation token.

**Returns:** An `AgentRunResponse` containing the agent's output.

**Example:**
```csharp
var messages = new[]
{
    new ChatMessage(ChatRole.User, "Write a function to sort a list")
};
var response = await agent.RunAsync(messages);
Console.WriteLine(response.Text);
```

##### RunAsync(string, AgentThread?, AgentRunOptions?, CancellationToken)

Runs the agent with a text message.

```csharp
public Task<AgentRunResponse> RunAsync(
    string message,
    AgentThread? thread = null,
    AgentRunOptions? options = null,
    CancellationToken cancellationToken = default);
```

**Parameters:**
- `message` - The user message text.
- `thread` - Optional conversation thread.
- `options` - Optional run configuration.
- `cancellationToken` - Cancellation token.

**Returns:** An `AgentRunResponse` containing the agent's output.

**Example:**
```csharp
var response = await agent.RunAsync("Explain dependency injection");
Console.WriteLine(response.Text);
```

##### RunStreamingAsync(IEnumerable\<ChatMessage\>, AgentThread?, AgentRunOptions?, CancellationToken)

Runs the agent in streaming mode.

```csharp
public override IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(
    IEnumerable<ChatMessage> messages,
    AgentThread? thread = null,
    AgentRunOptions? options = null,
    CancellationToken cancellationToken = default);
```

**Parameters:**
- `messages` - The messages to send to the agent.
- `thread` - Optional conversation thread.
- `options` - Optional run configuration.
- `cancellationToken` - Cancellation token.

**Returns:** An async enumerable of response updates.

**Example:**
```csharp
await foreach (var update in agent.RunStreamingAsync("Write a long explanation"))
{
    Console.Write(update.Text);
}
Console.WriteLine();
```

##### RunStreamingAsync(string, AgentThread?, AgentRunOptions?, CancellationToken)

Runs the agent in streaming mode with a text message.

```csharp
public IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(
    string message,
    AgentThread? thread = null,
    AgentRunOptions? options = null,
    CancellationToken cancellationToken = default);
```

**Parameters:**
- `message` - The user message text.
- `thread` - Optional conversation thread.
- `options` - Optional run configuration.
- `cancellationToken` - Cancellation token.

**Returns:** An async enumerable of response updates.

##### GetService\<TService\>(object?)

Gets a service of the specified type.

```csharp
public TService? GetService<TService>(object? serviceKey = null);
```

**Type Parameters:**
- `TService` - The type of service to retrieve.

**Parameters:**
- `serviceKey` - Optional key to identify the service.

**Returns:** The service instance, or null if not available.

**Example:**
```csharp
var client = agent.GetService<IOpencodeClient>();
var metadata = agent.GetService<AIAgentMetadata>();
```

---

### OpencodeAgentThread

Thread implementation for OpencodeAgent that maintains conversation state.

```csharp
public sealed class OpencodeAgentThread : AgentThread
```

#### Properties

##### ThreadId

Gets the unique identifier for this thread.

```csharp
public string ThreadId { get; }
```

##### SessionId

Gets the underlying OpenCode session ID (if available).

```csharp
public string? SessionId { get; }
```

**Remarks:** May be null before the first message is sent. Corresponds to an OpenCode session on the local server.

##### Messages

Gets the conversation message history.

```csharp
public IReadOnlyList<ChatMessage> Messages { get; }
```

#### Methods

##### Serialize(JsonSerializerOptions?)

Serializes the thread state to JSON.

```csharp
public override JsonElement Serialize(JsonSerializerOptions? jsonSerializerOptions = null);
```

**Parameters:**
- `jsonSerializerOptions` - Optional JSON serialization options.

**Returns:** A `JsonElement` containing the serialized state.

**Example:**
```csharp
var serialized = thread.Serialize();
var json = serialized.GetRawText();
File.WriteAllText("thread.json", json);
```

---

### OpencodeAgentOptions

Configuration options for OpencodeAgent.

```csharp
public sealed class OpencodeAgentOptions
```

#### Properties

##### Id

Gets or sets the agent's unique identifier.

```csharp
public string? Id { get; set; }
```

**Default:** Auto-generated GUID.

##### Name

Gets or sets the agent's display name.

```csharp
public string? Name { get; set; }
```

**Default:** `"OpencodeAI"`.

##### Description

Gets or sets the agent's description.

```csharp
public string? Description { get; set; }
```

**Default:** `null`.

##### DefaultModel

Gets or sets the default model for requests.

```csharp
public string? DefaultModel { get; set; }
```

**Default:** Inherited from client options.

##### SystemPrompt

Gets or sets the default system instructions.

```csharp
public string? SystemPrompt { get; set; }
```

**Default:** `null`.

##### Tools

Gets or sets the tools available to the agent.

```csharp
public IList<AITool>? Tools { get; set; }
```

**Default:** `null`.

##### EnableTelemetry

Gets or sets whether telemetry is enabled.

```csharp
public bool EnableTelemetry { get; set; }
```

**Default:** `true`.

---

### OpencodeAgentRunOptions

Options for a single agent run.

```csharp
public class OpencodeAgentRunOptions : AgentRunOptions
```

#### Properties

##### Model

Gets or sets the model to use for this request.

```csharp
public string? Model { get; set; }
```

##### MaxTokens

Gets or sets the maximum tokens for the response.

```csharp
public int? MaxTokens { get; set; }
```

##### Temperature

Gets or sets the temperature for response generation.

```csharp
public float? Temperature { get; set; }
```

---

## Extension Methods

### ServiceCollectionExtensions

Extension methods for registering OpencodeAgent with DI.

```csharp
public static class ServiceCollectionExtensions
```

#### AddOpencodeAgent(IServiceCollection, Action\<OpencodeAgentOptions\>)

Registers OpencodeAgent with the service collection.

```csharp
public static IServiceCollection AddOpencodeAgent(
    this IServiceCollection services,
    Action<OpencodeAgentOptions> configureOptions);
```

**Parameters:**
- `services` - The service collection.
- `configureOptions` - Action to configure agent options.

**Returns:** The service collection for chaining.

**Remarks:** Requires `AddOpencodeClient()` to be called first.

**Example:**
```csharp
// Register client with local server URL (no API key needed)
services.AddOpencodeClient(options => options.BaseUrl = "http://localhost:9123");
services.AddOpencodeAgent(options =>
{
    options.Name = "CodeAssistant";
    options.SystemPrompt = "You are a helpful coding assistant.";
});
```

#### AddOpencodeAgent(IServiceCollection)

Registers OpencodeAgent with default options.

```csharp
public static IServiceCollection AddOpencodeAgent(this IServiceCollection services);
```

**Example:**
```csharp
services.AddOpencodeClient(options => options.BaseUrl = "http://localhost:9123");
services.AddOpencodeAgent();
```

#### AddOpencodeAgentFactory(IServiceCollection)

Registers an OpencodeAgent factory for creating named agents.

```csharp
public static IServiceCollection AddOpencodeAgentFactory(this IServiceCollection services);
```

**Example:**
```csharp
services.AddOpencodeClient(options => options.BaseUrl = "http://localhost:9123");
services.AddOpencodeAgentFactory();

// Usage
var factory = serviceProvider.GetRequiredService<IOpencodeAgentFactory>();
var agent = factory.CreateAgent("CodeReviewer");
```

---

### AIAgentBuilderExtensions

Extension methods for building OpencodeAgent pipelines.

```csharp
public static class AIAgentBuilderExtensions
```

#### UseOpencodeMiddleware(AIAgentBuilder, Action\<OpencodeMiddlewareOptions\>)

Adds OpencodeAI-specific middleware to the agent pipeline.

```csharp
public static AIAgentBuilder UseOpencodeMiddleware(
    this AIAgentBuilder builder,
    Action<OpencodeMiddlewareOptions>? configureOptions = null);
```

**Example:**
```csharp
var agent = new AIAgentBuilder(baseAgent)
    .UseOpencodeMiddleware(options => options.EnableCodeFormatting = true)
    .Build();
```

---

## Interfaces

### IOpencodeAgentFactory

Factory for creating OpencodeAgent instances.

```csharp
public interface IOpencodeAgentFactory
{
    OpencodeAgent CreateAgent(OpencodeAgentOptions? options = null);
    OpencodeAgent CreateAgent(string name);
}
```

---

## Static Classes

### MessageConverter

Utility class for converting between message formats.

```csharp
public static class MessageConverter
```

#### ToConversationMessage(ChatMessage)

Converts a ChatMessage to a ConversationMessage.

```csharp
public static ConversationMessage ToConversationMessage(ChatMessage chatMessage);
```

#### ToChatMessage(ConversationMessage)

Converts a ConversationMessage to a ChatMessage.

```csharp
public static ChatMessage ToChatMessage(ConversationMessage message);
```

#### ToConversationMessages(IEnumerable\<ChatMessage\>)

Converts multiple ChatMessages to ConversationMessages.

```csharp
public static IEnumerable<ConversationMessage> ToConversationMessages(
    IEnumerable<ChatMessage> messages);
```

#### ToChatMessages(IEnumerable\<ConversationMessage\>)

Converts multiple ConversationMessages to ChatMessages.

```csharp
public static IEnumerable<ChatMessage> ToChatMessages(
    IEnumerable<ConversationMessage> messages);
```
