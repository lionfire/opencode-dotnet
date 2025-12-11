# OpencodeAI.AgentFramework Examples

## Basic Usage

### Simple Prompt and Response

```csharp
using OpencodeAI;
using OpencodeAI.AgentFramework;

// Connect to local OpenCode server (started via `opencode serve --port 9123`)
var client = new OpencodeClient("http://localhost:9123");

// Create an agent
var agent = new OpencodeAgent(client, name: "CodeAssistant");

// Run a simple prompt
var response = await agent.RunAsync("Write a function to validate email addresses in C#");

Console.WriteLine(response.Text);
```

### Multi-Turn Conversation

```csharp
// Create a thread to maintain conversation context
var thread = agent.GetNewThread();

// First message
var response1 = await agent.RunAsync("Write a basic user class in C#", thread);
Console.WriteLine(response1.Text);

// Follow-up using context from first message
var response2 = await agent.RunAsync("Add email validation to the user class", thread);
Console.WriteLine(response2.Text);

// Another follow-up
var response3 = await agent.RunAsync("Now add password hashing", thread);
Console.WriteLine(response3.Text);
```

### Streaming Responses

```csharp
// Stream the response in real-time
await foreach (var update in agent.RunStreamingAsync("Explain the SOLID principles"))
{
    Console.Write(update.Text);
}
Console.WriteLine();
```

### Streaming with Thread

```csharp
var thread = agent.GetNewThread();

// First message
await foreach (var update in agent.RunStreamingAsync("What is dependency injection?", thread))
{
    Console.Write(update.Text);
}
Console.WriteLine();

// Follow-up with context
await foreach (var update in agent.RunStreamingAsync("Show me an example in C#", thread))
{
    Console.Write(update.Text);
}
Console.WriteLine();
```

## Dependency Injection

### ASP.NET Core Registration

```csharp
// Program.cs
var builder = WebApplication.CreateBuilder(args);

// Register OpencodeAI client (connects to local server, no API key needed)
builder.Services.AddOpencodeClient(options =>
{
    options.BaseUrl = builder.Configuration["OpenCode:BaseUrl"] ?? "http://localhost:9123";
});

// Register OpencodeAgent
builder.Services.AddOpencodeAgent(options =>
{
    options.Name = "CodeAssistant";
    options.Description = "A helpful coding assistant";
    options.SystemPrompt = "You are an expert C# and .NET developer.";
});

var app = builder.Build();
```

### Controller Usage

```csharp
[ApiController]
[Route("api/[controller]")]
public class CodeController : ControllerBase
{
    private readonly AIAgent _agent;

    public CodeController(AIAgent agent)
    {
        _agent = agent;
    }

    [HttpPost("generate")]
    public async Task<IActionResult> GenerateCode([FromBody] GenerateRequest request)
    {
        var response = await _agent.RunAsync(request.Prompt);
        return Ok(new { code = response.Text });
    }

    [HttpPost("generate/stream")]
    public async IAsyncEnumerable<string> GenerateCodeStream(
        [FromBody] GenerateRequest request,
        CancellationToken cancellationToken)
    {
        await foreach (var update in _agent.RunStreamingAsync(
            request.Prompt,
            cancellationToken: cancellationToken))
        {
            yield return update.Text;
        }
    }
}
```

### Named Agents

```csharp
// Register multiple agents with different configurations
builder.Services.AddOpencodeClient(options => options.BaseUrl = "http://localhost:9123");

builder.Services.AddOpencodeAgent("coder", options =>
{
    options.Name = "CodeGenerator";
    options.SystemPrompt = "You are an expert code generator. Output only code.";
});

builder.Services.AddOpencodeAgent("reviewer", options =>
{
    options.Name = "CodeReviewer";
    options.SystemPrompt = "You are a code reviewer focused on security and performance.";
});

// Usage
public class CodeService
{
    private readonly AIAgent _coder;
    private readonly AIAgent _reviewer;

    public CodeService(
        [FromKeyedServices("coder")] AIAgent coder,
        [FromKeyedServices("reviewer")] AIAgent reviewer)
    {
        _coder = coder;
        _reviewer = reviewer;
    }

    public async Task<CodeResult> GenerateAndReview(string prompt)
    {
        // Generate code
        var codeResponse = await _coder.RunAsync(prompt);
        var code = codeResponse.Text;

        // Review the generated code
        var reviewResponse = await _reviewer.RunAsync($"Review this code:\n{code}");
        var review = reviewResponse.Text;

        return new CodeResult(code, review);
    }
}
```

## Thread Persistence

### Saving Thread State

```csharp
var thread = agent.GetNewThread();

// Have a conversation
await agent.RunAsync("Create a user management system", thread);
await agent.RunAsync("Add role-based access control", thread);

// Serialize thread for storage
var serialized = thread.Serialize();
var json = serialized.GetRawText();

// Save to database
await database.SaveThreadAsync(userId, json);
```

### Restoring Thread State

```csharp
// Load from database
var json = await database.GetThreadAsync(userId);

// Parse and deserialize
var jsonDocument = JsonDocument.Parse(json);
var thread = agent.DeserializeThread(jsonDocument.RootElement);

// Continue conversation
var response = await agent.RunAsync("What features did we add so far?", thread);
Console.WriteLine(response.Text); // Will remember the previous context
```

## Agent Framework Workflows

### Multi-Agent Sequential Workflow

```csharp
using Microsoft.Agents.AI;
using OpencodeAI.AgentFramework;

// Create specialized agents
var plannerAgent = CreateOpenAIAgent("Planner", "Design software architecture");
var coderAgent = new OpencodeAgent(opcodeClient, "Coder", "Generate code");
var reviewerAgent = CreateOpenAIAgent("Reviewer", "Review code quality");

// Execute sequential workflow
async Task<WorkflowResult> ExecuteWorkflow(string requirement)
{
    var thread = coderAgent.GetNewThread();

    // Step 1: Plan
    var planResponse = await plannerAgent.RunAsync(
        $"Create a plan for: {requirement}");
    var plan = planResponse.Text;

    // Step 2: Generate code with OpencodeAI
    var codeResponse = await coderAgent.RunAsync(
        $"Implement this plan:\n{plan}", thread);
    var code = codeResponse.Text;

    // Step 3: Review
    var reviewResponse = await reviewerAgent.RunAsync(
        $"Review this code:\n{code}");
    var review = reviewResponse.Text;

    return new WorkflowResult(plan, code, review);
}
```

### Middleware Pipeline

```csharp
using Microsoft.Agents.AI;

// Create base agent
var baseAgent = new OpencodeAgent(client, "CodeAssistant");

// Build pipeline with middleware
var agent = new AIAgentBuilder(baseAgent)
    .Use(async (messages, thread, options, next, ct) =>
    {
        // Pre-processing: Log the request
        Console.WriteLine($"Request: {messages.LastOrDefault()?.Text}");

        // Call the next agent in the pipeline
        await next(messages, thread, options, ct);

        Console.WriteLine("Request completed");
    })
    .Use(innerAgent => new TelemetryDelegatingAgent(innerAgent))
    .Use(innerAgent => new RetryDelegatingAgent(innerAgent, maxRetries: 3))
    .Build();

// Use the wrapped agent
var response = await agent.RunAsync("Generate a REST API");
```

### Approval Middleware

```csharp
public class ApprovalDelegatingAgent : DelegatingAIAgent
{
    private readonly IApprovalService _approvalService;

    public ApprovalDelegatingAgent(AIAgent innerAgent, IApprovalService approvalService)
        : base(innerAgent)
    {
        _approvalService = approvalService;
    }

    public override async Task<AgentRunResponse> RunAsync(
        IEnumerable<ChatMessage> messages,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        // Get response from inner agent
        var response = await base.RunAsync(messages, thread, options, cancellationToken);

        // Check if code was generated
        if (response.Text.Contains("```"))
        {
            // Request approval
            var approved = await _approvalService.RequestApprovalAsync(
                "Code generated - please review",
                response.Text);

            if (!approved)
            {
                throw new ApprovalRejectedException("Code was rejected by reviewer");
            }
        }

        return response;
    }
}

// Usage
var agent = new AIAgentBuilder(baseAgent)
    .Use(inner => new ApprovalDelegatingAgent(inner, approvalService))
    .Build();
```

## Tool Integration

### Defining Tools

```csharp
// Define tools for the agent
var tools = new List<AITool>
{
    AIFunctionFactory.Create(
        (string filename) => File.ReadAllText(filename),
        "read_file",
        "Read the contents of a file"),

    AIFunctionFactory.Create(
        (string filename, string content) => File.WriteAllText(filename, content),
        "write_file",
        "Write content to a file"),

    AIFunctionFactory.Create(
        (string command) => ExecuteShellCommand(command),
        "run_command",
        "Execute a shell command")
};

// Create agent with tools
var agent = new OpencodeAgent(client, new OpencodeAgentOptions
{
    Name = "FileAgent",
    Tools = tools,
    SystemPrompt = "You can read and write files. Use the tools when needed."
});
```

### Using Tools in Conversation

```csharp
var thread = agent.GetNewThread();

// The agent will automatically use tools when needed
var response = await agent.RunAsync(
    "Read the file 'config.json' and update the 'version' field to '2.0.0'",
    thread);

Console.WriteLine(response.Text);
// Output: I've read config.json and updated the version field. The file has been saved.
```

## Error Handling

### Basic Error Handling

```csharp
try
{
    var response = await agent.RunAsync("Generate some code");
    Console.WriteLine(response.Text);
}
catch (OpencodeConnectionException ex)
{
    // Server not running or unreachable
    Console.WriteLine($"Cannot connect to OpenCode server: {ex.Message}");
    Console.WriteLine("Make sure 'opencode serve --port 9123' is running");
}
catch (OpencodeSessionNotFoundException ex)
{
    // Session was deleted or doesn't exist
    Console.WriteLine($"Session not found: {ex.SessionId}");
    // Create a new session...
}
catch (OpencodeApiException ex)
{
    Console.WriteLine($"API error: {ex.Message}");
}
catch (OperationCanceledException)
{
    Console.WriteLine("Request was cancelled");
}
```

### Streaming Error Handling

```csharp
try
{
    await foreach (var update in agent.RunStreamingAsync("Generate code"))
    {
        Console.Write(update.Text);
    }
}
catch (OpencodeApiException ex)
{
    Console.WriteLine($"\nStreaming error: {ex.Message}");
}
```

## Cancellation

### Request Cancellation

```csharp
using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));

try
{
    var response = await agent.RunAsync(
        "Generate a very long explanation",
        cancellationToken: cts.Token);
}
catch (OperationCanceledException)
{
    Console.WriteLine("Request timed out");
}
```

### Streaming Cancellation

```csharp
using var cts = new CancellationTokenSource();

var streamingTask = Task.Run(async () =>
{
    await foreach (var update in agent.RunStreamingAsync(
        "Explain everything about C#",
        cancellationToken: cts.Token))
    {
        Console.Write(update.Text);
    }
});

// Cancel after 5 seconds
await Task.Delay(5000);
cts.Cancel();

try
{
    await streamingTask;
}
catch (OperationCanceledException)
{
    Console.WriteLine("\nStreaming cancelled");
}
```

## Advanced Scenarios

### Custom Thread Implementation

```csharp
// Access the underlying conversation for advanced scenarios
var thread = agent.GetNewThread() as OpencodeAgentThread;

// Access the raw conversation
var conversation = thread?.Conversation;
if (conversation != null)
{
    // Get token count
    var tokenCount = conversation.EstimatedTokenCount;
    Console.WriteLine($"Current token count: {tokenCount}");

    // Clear history if too long
    if (tokenCount > 50000)
    {
        conversation.ClearHistory(keepSystemPrompt: true);
    }
}
```

### Parallel Execution

```csharp
// Create multiple agents for parallel work
var agents = new[]
{
    new OpencodeAgent(client, "Agent1"),
    new OpencodeAgent(client, "Agent2"),
    new OpencodeAgent(client, "Agent3")
};

var prompts = new[]
{
    "Write a user service",
    "Write an order service",
    "Write a payment service"
};

// Execute in parallel
var tasks = agents.Zip(prompts, (agent, prompt) => agent.RunAsync(prompt));
var responses = await Task.WhenAll(tasks);

foreach (var response in responses)
{
    Console.WriteLine($"=== Generated ===\n{response.Text}\n");
}
```

### Integration with Semantic Kernel

```csharp
using Microsoft.SemanticKernel;

// Create OpencodeAgent
var opcodeAgent = new OpencodeAgent(client, "CodeGenerator");

// Create Semantic Kernel with OpencodeAgent as a plugin
var kernel = Kernel.CreateBuilder()
    .AddAzureOpenAIChatCompletion(deploymentName, endpoint, apiKey)
    .Build();

// Register OpencodeAgent as a function
kernel.ImportPluginFromFunctions("code", [
    KernelFunctionFactory.CreateFromMethod(
        async (string prompt) =>
        {
            var response = await opcodeAgent.RunAsync(prompt);
            return response.Text;
        },
        "generate",
        "Generate code using OpencodeAI")
]);

// Use in Semantic Kernel
var result = await kernel.InvokePromptAsync(
    "Use the code.generate function to create a REST API for users");
```
