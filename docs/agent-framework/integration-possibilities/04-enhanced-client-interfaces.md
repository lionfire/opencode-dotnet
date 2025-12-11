# Integration Option 4: Enhanced IOpencodeClient with Agent Interfaces

## Overview

Extend `IOpencodeClient` to directly implement Microsoft Agent Framework interfaces, allowing the OpencodeAI client to participate in Agent Framework scenarios without requiring a wrapper. This approach provides seamless integration while maintaining OpencodeAI's native API.

## Architecture

```
┌─────────────────────────────────────────────────┐
│           IOpencodeClient                       │
│  (with Agent Framework Interface Extensions)   │
├─────────────────────────────────────────────────┤
│                                                 │
│  OpencodeAI Native API        Agent Framework  │
│  ├─ GenerateCodeAsync()       ├─ RunAsync()    │
│  ├─ StreamCodeAsync()         ├─ RunStreaming()│
│  ├─ IConversation             ├─ GetThread()   │
│  ├─ ExplainCodeAsync()        ├─ AsAgent()     │
│  └─ ReviewCodeAsync()         └─ AsTool()      │
│                                                 │
└─────────────────────────────────────────────────┘
```

## Key Benefits

1. **No Wrapper Needed**: OpencodeAI client directly usable in Agent Framework
2. **Dual API Access**: Use native OpencodeAI methods OR Agent Framework methods
3. **Seamless DI**: Single registration works for both APIs
4. **Type Safety**: Full IntelliSense for both interfaces
5. **Incremental Adoption**: Add Agent capabilities without breaking existing code
6. **Natural Extension**: Leverages C# interface composition

## How Agent Capabilities Help

### Capability 1: Unified Conversation Threading

**Problem with Current API:**
```csharp
// OpencodeAI conversation - separate from other agents
var conversation = client.CreateConversation();
await conversation.SendMessageAsync("Generate a class");
await conversation.SendMessageAsync("Add a method");

// Other agent conversation - different interface
var otherThread = otherAgent.GetNewThread();
await otherAgent.RunAsync("Plan the architecture", otherThread);
```

**Solution with Agent Interface:**
```csharp
// Unified threading across all agents
var opcodeAgent = client.AsAgent();
var thread = opcodeAgent.GetNewThread(); // AgentThread compatible with all agents

// Use OpencodeAI
await opcodeAgent.RunAsync("Generate a class", thread);

// Share thread with other agents
await architectAgent.RunAsync("Review the design", thread); // Same thread!

// OpencodeAI automatically sees previous conversation
await opcodeAgent.RunAsync("Refactor based on architect's feedback", thread);
```

**Why This Helps:**
- **Shared Context**: All agents see the same conversation history
- **Coordination**: Agents can build on each other's work
- **State Management**: One thread to persist, restore, and manage

### Capability 2: Middleware Integration

**Problem with Current API:**
```csharp
// Manual telemetry for each OpencodeAI call
using var activity = ActivitySource.StartActivity("GenerateCode");
try
{
    var result = await client.GenerateCodeAsync(request);
    activity?.SetTag("code.length", result.Code.Length);
    return result;
}
catch (Exception ex)
{
    activity?.SetStatus(ActivityStatusCode.Error);
    throw;
}
```

**Solution with Agent Interface:**
```csharp
// Register middleware once
var opcodeAgent = client.AsAgent()
    .WithMiddleware(new TelemetryMiddleware())
    .WithMiddleware(new LoggingMiddleware())
    .WithMiddleware(new ApprovalMiddleware());

// All calls automatically get middleware applied
var result = await opcodeAgent.RunAsync("Generate user authentication");
// Telemetry, logging, approval all happen automatically
```

**Built-in Middleware Examples:**

**Approval Middleware:**
```csharp
public class CodeApprovalMiddleware : IAgentMiddleware
{
    public async Task<AgentRunResponse> InvokeAsync(
        AgentContext context,
        Func<Task<AgentRunResponse>> next)
    {
        var response = await next();

        // Extract generated code
        var code = response.Messages
            .SelectMany(m => m.Contents.OfType<TextContent>())
            .FirstOrDefault()?.Text;

        if (code?.Contains("dangerous operation") == true)
        {
            Console.WriteLine("Code requires approval:");
            Console.WriteLine(code);
            Console.Write("Approve? (y/n): ");

            if (Console.ReadLine()?.ToLower() != "y")
            {
                throw new ApprovalRejectedException("Code generation rejected by user");
            }
        }

        return response;
    }
}
```

**Cost Tracking Middleware:**
```csharp
public class CostTrackingMiddleware : IAgentMiddleware
{
    private readonly ICostCalculator _calculator;
    private decimal _totalCost;

    public async Task<AgentRunResponse> InvokeAsync(
        AgentContext context,
        Func<Task<AgentRunResponse>> next)
    {
        var response = await next();

        // Track token usage and calculate cost
        if (response.Metadata.TryGetValue("token_usage", out var usage))
        {
            var cost = _calculator.Calculate(usage);
            _totalCost += cost;

            response.Metadata["cumulative_cost"] = _totalCost;

            if (_totalCost > 100) // $100 budget limit
            {
                throw new BudgetExceededException($"Budget exceeded: ${_totalCost:F2}");
            }
        }

        return response;
    }
}
```

**Content Filtering Middleware:**
```csharp
public class ContentFilterMiddleware : IAgentMiddleware
{
    private readonly IContentFilter _filter;

    public async Task<AgentRunResponse> InvokeAsync(
        AgentContext context,
        Func<Task<AgentRunResponse>> next)
    {
        // Filter input
        var inputText = context.Messages.LastOrDefault()?.Text;
        if (_filter.ContainsBlockedContent(inputText))
        {
            throw new ContentFilterException("Input contains blocked content");
        }

        var response = await next();

        // Filter output
        foreach (var message in response.Messages)
        {
            if (_filter.ContainsBlockedContent(message.Text))
            {
                throw new ContentFilterException("Generated content violates policy");
            }
        }

        return response;
    }
}
```

**Why This Helps:**
- **Cross-Cutting Concerns**: Apply telemetry, logging, security to all operations
- **Reusable Logic**: Write middleware once, use with all agents
- **Composable**: Stack multiple middleware in any order
- **Enterprise Ready**: Built-in support for approval flows, budgets, compliance

### Capability 3: Multi-Agent Workflows

**Problem with Current API:**
```csharp
// Manual coordination between OpencodeAI and other services
var plannerResponse = await plannerClient.GenerateAsync("Plan the system");
var codeResponse = await opcodeClient.GenerateCodeAsync(new GenerateCodeRequest
{
    Prompt = $"Implement this plan: {plannerResponse}"
});
var reviewResponse = await reviewerClient.AnalyzeAsync(codeResponse.Code);

// Manual state tracking
var workflowState = new
{
    Plan = plannerResponse,
    Code = codeResponse,
    Review = reviewResponse
};
```

**Solution with Agent Interface:**
```csharp
// OpencodeAI as a workflow participant
var workflow = new WorkflowHostAgent()
    .AddAgent("planner", plannerAgent)
    .AddAgent("coder", client.AsAgent()) // OpencodeAI directly in workflow
    .AddAgent("reviewer", reviewerAgent)
    .DefineFlow(builder => builder
        .Start("planner")
        .Then("coder")
        .Then("reviewer")
        .End());

var result = await workflow.RunAsync("Build a REST API for user management");

// Workflow handles all coordination and state
```

**Why This Helps:**
- **Simplified Coordination**: Framework handles agent communication
- **State Management**: Automatic persistence and recovery
- **Parallel Execution**: Run OpencodeAI in parallel with other agents
- **Error Handling**: Built-in retry, fallback, and error recovery

### Capability 4: Tool Registration

**Problem with Current API:**
```csharp
// OpencodeAI can't be used as a tool by other agents
// Must manually call OpencodeAI and format results

var chatAgent = CreateChatAgent();
var userMessage = "Generate a validation function";

// Manual delegation
if (userMessage.Contains("generate"))
{
    var code = await opcodeClient.GenerateCodeAsync(...);
    await chatAgent.RunAsync($"Here's the generated code: {code}");
}
```

**Solution with Agent Interface:**
```csharp
// Register OpencodeAI as a tool
var tools = new AIFunctionFactory()
    .CreateFromAgent(client.AsAgent(), new AgentToolOptions
    {
        Name = "CodeGenerator",
        Description = "Generate production-quality code",
        ExposeOperations = new[] { "GenerateCode", "ExplainCode", "ReviewCode" }
    });

var chatAgent = CreateChatAgent(tools);

// Chat agent can now invoke OpencodeAI automatically
await chatAgent.RunAsync("Generate a validation function");
// Chat agent detects code task and calls OpencodeAI tool automatically
```

**Why This Helps:**
- **Automatic Tool Selection**: LLM decides when to use OpencodeAI
- **Type Safety**: Strong typing for tool parameters and results
- **Standardized Interface**: All tools follow same patterns
- **Discoverability**: Tools are documented and easily registered

### Capability 5: Dependency Injection Unification

**Problem with Current API:**
```csharp
// Separate registrations for OpencodeAI and Agent Framework
services.AddOpencodeClient(options => ...);
services.AddSingleton<OpencodeAgent>(); // Wrapper needed
```

**Solution with Agent Interface:**
```csharp
// Single registration for both APIs
services.AddOpencodeClient(options => ...)
    .AsAgent(agentOptions =>
    {
        agentOptions.Name = "CodeExpert";
        agentOptions.Description = "Specialized code generation agent";
        agentOptions.EnableTelemetry = true;
        agentOptions.EnableMiddleware = true;
    });

// Use as OpencodeAI client
var client = serviceProvider.GetRequiredService<IOpencodeClient>();
await client.GenerateCodeAsync(...);

// Use as Agent Framework agent
var agent = serviceProvider.GetRequiredService<AIAgent>();
await agent.RunAsync("Generate code");
```

**Why This Helps:**
- **Single Source of Truth**: One configuration for both APIs
- **Simplified Setup**: No wrapper classes to register
- **Better DI**: Proper lifetime management and scoping
- **Configuration Flexibility**: Different settings per interface

## Implementation Approach

### Extension Interfaces

```csharp
namespace OpencodeAI.AgentFramework
{
    /// <summary>
    /// Extension methods to add Agent Framework capabilities to IOpencodeClient
    /// </summary>
    public static class OpencodeAgentExtensions
    {
        /// <summary>
        /// Wrap IOpencodeClient as an AIAgent for use in Agent Framework scenarios
        /// </summary>
        public static AIAgent AsAgent(
            this IOpencodeClient client,
            string? name = null,
            string? description = null)
        {
            return new OpencodeClientAgentAdapter(client, name, description);
        }

        /// <summary>
        /// Create Agent Framework tools from OpencodeAI client methods
        /// </summary>
        public static IEnumerable<AIFunction> AsTools(
            this IOpencodeClient client,
            OpencodeToolOptions? options = null)
        {
            var factory = new AIFunctionFactory();
            return factory.CreateFromObject(new OpencodeToolWrapper(client, options));
        }

        /// <summary>
        /// Get or create an AgentThread compatible with OpencodeAI conversations
        /// </summary>
        public static AgentThread GetAgentThread(
            this IConversation conversation)
        {
            return new OpencodeConversationThreadAdapter(conversation);
        }

        /// <summary>
        /// Convert an AgentThread to an OpencodeAI conversation
        /// </summary>
        public static IConversation ToConversation(
            this AgentThread thread,
            IOpencodeClient client)
        {
            if (thread is OpencodeConversationThreadAdapter adapter)
            {
                return adapter.Conversation;
            }

            // Create new conversation and populate with thread messages
            var conversation = client.CreateConversation();
            // ... populate from thread
            return conversation;
        }
    }

    /// <summary>
    /// Adapter that makes IOpencodeClient compatible with AIAgent
    /// </summary>
    internal class OpencodeClientAgentAdapter : AIAgent
    {
        private readonly IOpencodeClient _client;

        public OpencodeClientAgentAdapter(
            IOpencodeClient client,
            string? name = null,
            string? description = null)
        {
            _client = client;
            Name = name ?? "OpencodeAI";
            Description = description ?? "Specialized code generation and analysis agent";
        }

        public override string? Name { get; }
        public override string? Description { get; }

        public override AgentThread GetNewThread()
        {
            var conversation = _client.CreateConversation();
            return new OpencodeConversationThreadAdapter(conversation);
        }

        public override async Task<AgentRunResponse> RunAsync(
            IEnumerable<ChatMessage> messages,
            AgentThread? thread = null,
            AgentRunOptions? options = null,
            CancellationToken cancellationToken = default)
        {
            // Get or create conversation
            var conversation = thread?.ToConversation(_client)
                ?? _client.CreateConversation();

            // Convert messages to OpencodeAI format
            var lastUserMessage = messages
                .Where(m => m.Role == ChatRole.User)
                .LastOrDefault();

            if (lastUserMessage == null)
            {
                throw new ArgumentException("At least one user message is required");
            }

            // Send to OpencodeAI
            var response = await conversation.SendMessageAsync(
                lastUserMessage.Text ?? string.Empty,
                cancellationToken);

            // Convert response back to ChatMessage
            var responseMessage = new ChatMessage(ChatRole.Assistant, response)
            {
                AuthorName = this.DisplayName
            };

            return new AgentRunResponse
            {
                AgentId = this.Id,
                Messages = new[] { responseMessage }
            };
        }

        public override async IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(
            IEnumerable<ChatMessage> messages,
            AgentThread? thread = null,
            AgentRunOptions? options = null,
            [EnumeratorCancellation] CancellationToken cancellationToken = default)
        {
            // Similar to RunAsync but using StreamMessageAsync
            var conversation = thread?.ToConversation(_client)
                ?? _client.CreateConversation();

            var lastUserMessage = messages
                .Where(m => m.Role == ChatRole.User)
                .LastOrDefault()?.Text ?? string.Empty;

            await foreach (var chunk in conversation.StreamMessageAsync(
                lastUserMessage,
                cancellationToken))
            {
                yield return new AgentRunResponseUpdate
                {
                    AgentId = this.Id,
                    Role = ChatRole.Assistant,
                    Contents = new[] { new TextContent(chunk.Text) },
                    AuthorName = this.DisplayName
                };
            }
        }
    }
}
```

### DI Registration Extensions

```csharp
namespace Microsoft.Extensions.DependencyInjection
{
    public static class OpencodeAgentServiceCollectionExtensions
    {
        /// <summary>
        /// Add OpencodeAI client with Agent Framework integration
        /// </summary>
        public static IServiceCollection AddOpencodeClientAsAgent(
            this IServiceCollection services,
            Action<OpencodeClientOptions> configureClient,
            Action<OpencodeAgentOptions>? configureAgent = null)
        {
            // Register OpencodeAI client
            services.AddOpencodeClient(configureClient);

            // Register as AIAgent
            services.AddSingleton<AIAgent>(sp =>
            {
                var client = sp.GetRequiredService<IOpencodeClient>();
                var agentOptions = new OpencodeAgentOptions();
                configureAgent?.Invoke(agentOptions);

                var agent = client.AsAgent(
                    agentOptions.Name,
                    agentOptions.Description);

                // Apply middleware
                foreach (var middleware in agentOptions.Middleware)
                {
                    agent = agent.WithMiddleware(middleware);
                }

                return agent;
            });

            return services;
        }
    }

    public class OpencodeAgentOptions
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public List<IAgentMiddleware> Middleware { get; } = new();
    }
}
```

## Usage Examples

### Example 1: Direct Agent Usage

```csharp
var client = new OpencodeClient("oc_sk_...");
var agent = client.AsAgent(
    name: "CodeExpert",
    description: "Production code generation specialist");

// Use as Agent Framework agent
var thread = agent.GetNewThread();
var response = await agent.RunAsync("Generate a repository pattern implementation", thread);

// Or use streaming
await foreach (var update in agent.RunStreamingAsync("Explain dependency injection", thread))
{
    Console.Write(update.Text);
}
```

### Example 2: In Workflows

```csharp
var workflow = new WorkflowHostAgent()
    .AddAgent("architect", architectAgent)
    .AddAgent("coder", opcodeClient.AsAgent()) // Direct integration
    .AddAgent("tester", testAgent)
    .DefineFlow(flow => flow
        .Start("architect")
        .Then("coder")
        .Then("tester"));

await workflow.RunAsync("Build microservice");
```

### Example 3: With Middleware

```csharp
var agent = opcodeClient
    .AsAgent()
    .WithMiddleware(new TelemetryMiddleware())
    .WithMiddleware(new CostTrackingMiddleware(maxBudget: 100))
    .WithMiddleware(new ApprovalMiddleware());

// All middleware automatically applied
var result = await agent.RunAsync("Generate database migration");
```

### Example 4: As Tools for Other Agents

```csharp
var tools = opcodeClient.AsTools(new OpencodeToolOptions
{
    ExposeMethods = new[] { "GenerateCode", "ReviewCode", "ExplainCode" }
});

var chatAgent = chatClient
    .CreateAIAgent(tools: tools);

// Chat agent can now use OpencodeAI as a tool
await chatAgent.RunAsync("Create a validation library for emails and phone numbers");
```

## Comparison with Other Options

| Aspect | Option 4 (Enhanced Client) | Option 1 (OpencodeAgent) |
|--------|---------------------------|------------------------|
| **API Access** | Both OpencodeAI + Agent APIs | Primarily Agent API |
| **Wrapper Needed** | No - direct extension | Yes - wrapper class |
| **Code Changes** | Extension methods only | New agent implementation |
| **DI Registration** | Single registration | Separate registrations |
| **Breaking Changes** | None - fully backward compatible | None if optional |
| **Flexibility** | Use either API as needed | Agent API only |

## Best Use Cases

1. **Incremental Migration**: Add Agent capabilities without changing existing code
2. **Dual API Requirements**: Need both OpencodeAI-specific and Agent Framework APIs
3. **Simplified DI**: Want single registration for all scenarios
4. **Backward Compatibility**: Can't break existing OpencodeAI consumers
5. **Maximum Flexibility**: Choose API per use case

## Implementation Checklist

- [ ] Create `OpencodeClientAgentAdapter` class
- [ ] Implement `AsAgent()` extension method
- [ ] Implement `AsTools()` extension method
- [ ] Create thread conversion helpers (`GetAgentThread()`, `ToConversation()`)
- [ ] Add DI registration extensions
- [ ] Implement middleware support
- [ ] Create integration tests
- [ ] Update documentation
- [ ] Create migration guide
- [ ] Add samples for common scenarios

## Next Steps

After implementing Option 1 (OpencodeAgent), Option 4 can be added as a convenience layer that:
1. Uses OpencodeAgent internally
2. Exposes extension methods for easier access
3. Provides seamless DI integration
4. Maintains full backward compatibility

This makes Option 4 a natural evolution of Option 1 rather than a separate implementation.
