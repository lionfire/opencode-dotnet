# Multi-Agent Workflow Examples

This document provides examples of using OpencodeAgent in multi-agent workflows with Microsoft Agent Framework.

## Sequential Code Development Workflow

A workflow that moves through planning, coding, reviewing, and testing phases.

```csharp
using Microsoft.Agents.AI;
using OpencodeAI;
using OpencodeAI.AgentFramework;

public class CodeDevelopmentWorkflow
{
    private readonly AIAgent _plannerAgent;
    private readonly OpencodeAgent _coderAgent;
    private readonly AIAgent _reviewerAgent;
    private readonly OpencodeAgent _testerAgent;

    public CodeDevelopmentWorkflow(
        AIAgent plannerAgent,
        IOpencodeClient opcodeClient,
        AIAgent reviewerAgent)
    {
        _plannerAgent = plannerAgent;
        _coderAgent = new OpencodeAgent(opcodeClient, new OpencodeAgentOptions
        {
            Name = "CodeGenerator",
            Description = "Generates production-ready code",
            SystemPrompt = "You are an expert software developer. Generate clean, well-documented code."
        });
        _reviewerAgent = reviewerAgent;
        _testerAgent = new OpencodeAgent(opcodeClient, new OpencodeAgentOptions
        {
            Name = "TestGenerator",
            Description = "Generates comprehensive test suites",
            SystemPrompt = "You are a test engineer. Generate thorough unit and integration tests."
        });
    }

    public async Task<DevelopmentResult> DevelopFeatureAsync(
        string featureRequirement,
        CancellationToken cancellationToken = default)
    {
        // Phase 1: Planning
        Console.WriteLine("Phase 1: Planning...");
        var planResponse = await _plannerAgent.RunAsync(
            $"Create a detailed technical plan for: {featureRequirement}",
            cancellationToken: cancellationToken);
        var plan = planResponse.Text;
        Console.WriteLine($"Plan created: {plan.Length} characters");

        // Phase 2: Code Generation (using OpencodeAI)
        Console.WriteLine("Phase 2: Generating code...");
        var coderThread = _coderAgent.GetNewThread();
        var codeResponse = await _coderAgent.RunAsync(
            $"Implement the following plan. Generate complete, production-ready code:\n\n{plan}",
            coderThread,
            cancellationToken: cancellationToken);
        var code = codeResponse.Text;
        Console.WriteLine($"Code generated: {code.Length} characters");

        // Phase 3: Code Review
        Console.WriteLine("Phase 3: Reviewing code...");
        var reviewResponse = await _reviewerAgent.RunAsync(
            $"Review this code for quality, security, and best practices:\n\n{code}",
            cancellationToken: cancellationToken);
        var review = reviewResponse.Text;
        Console.WriteLine($"Review completed");

        // Phase 4: Test Generation (using OpencodeAI)
        Console.WriteLine("Phase 4: Generating tests...");
        var testerThread = _testerAgent.GetNewThread();
        var testResponse = await _testerAgent.RunAsync(
            $"Generate comprehensive tests for this code:\n\n{code}",
            testerThread,
            cancellationToken: cancellationToken);
        var tests = testResponse.Text;
        Console.WriteLine($"Tests generated: {tests.Length} characters");

        return new DevelopmentResult
        {
            Plan = plan,
            Code = code,
            Review = review,
            Tests = tests
        };
    }
}

public record DevelopmentResult
{
    public string Plan { get; init; } = "";
    public string Code { get; init; } = "";
    public string Review { get; init; } = "";
    public string Tests { get; init; } = "";
}
```

## Parallel Code Generation

Generate multiple components simultaneously.

```csharp
public class ParallelCodeGenerationWorkflow
{
    private readonly IOpencodeClient _opcodeClient;

    public ParallelCodeGenerationWorkflow(IOpencodeClient opcodeClient)
    {
        _opcodeClient = opcodeClient;
    }

    public async Task<MultiComponentResult> GenerateComponentsAsync(
        ComponentSpec spec,
        CancellationToken cancellationToken = default)
    {
        // Create specialized agents for each component type
        var backendAgent = new OpencodeAgent(_opcodeClient, new OpencodeAgentOptions
        {
            Name = "BackendGenerator",
            SystemPrompt = "You are a backend developer. Generate C# .NET API code."
        });

        var frontendAgent = new OpencodeAgent(_opcodeClient, new OpencodeAgentOptions
        {
            Name = "FrontendGenerator",
            SystemPrompt = "You are a frontend developer. Generate React TypeScript code."
        });

        var databaseAgent = new OpencodeAgent(_opcodeClient, new OpencodeAgentOptions
        {
            Name = "DatabaseGenerator",
            SystemPrompt = "You are a database engineer. Generate SQL scripts and Entity Framework models."
        });

        // Generate all components in parallel
        var backendTask = backendAgent.RunAsync(
            $"Generate a REST API for: {spec.Description}\n\nEntities: {string.Join(", ", spec.Entities)}",
            cancellationToken: cancellationToken);

        var frontendTask = frontendAgent.RunAsync(
            $"Generate React components for: {spec.Description}\n\nEntities: {string.Join(", ", spec.Entities)}",
            cancellationToken: cancellationToken);

        var databaseTask = databaseAgent.RunAsync(
            $"Generate database schema and EF Core entities for: {spec.Description}\n\nEntities: {string.Join(", ", spec.Entities)}",
            cancellationToken: cancellationToken);

        // Wait for all to complete
        await Task.WhenAll(backendTask, frontendTask, databaseTask);

        return new MultiComponentResult
        {
            BackendCode = backendTask.Result.Text,
            FrontendCode = frontendTask.Result.Text,
            DatabaseCode = databaseTask.Result.Text
        };
    }
}

public record ComponentSpec
{
    public string Description { get; init; } = "";
    public List<string> Entities { get; init; } = new();
}

public record MultiComponentResult
{
    public string BackendCode { get; init; } = "";
    public string FrontendCode { get; init; } = "";
    public string DatabaseCode { get; init; } = "";
}
```

## Iterative Refinement Workflow

Generate code, review it, and refine based on feedback.

```csharp
public class IterativeRefinementWorkflow
{
    private readonly OpencodeAgent _coderAgent;
    private readonly AIAgent _reviewerAgent;
    private readonly int _maxIterations;

    public IterativeRefinementWorkflow(
        IOpencodeClient opcodeClient,
        AIAgent reviewerAgent,
        int maxIterations = 3)
    {
        _coderAgent = new OpencodeAgent(opcodeClient, "CodeRefiner");
        _reviewerAgent = reviewerAgent;
        _maxIterations = maxIterations;
    }

    public async Task<RefinementResult> RefineCodeAsync(
        string initialPrompt,
        CancellationToken cancellationToken = default)
    {
        var thread = _coderAgent.GetNewThread();
        var iterations = new List<RefinementIteration>();

        // Initial generation
        Console.WriteLine("Initial generation...");
        var codeResponse = await _coderAgent.RunAsync(initialPrompt, thread, cancellationToken: cancellationToken);
        var currentCode = codeResponse.Text;

        for (int i = 0; i < _maxIterations; i++)
        {
            Console.WriteLine($"Iteration {i + 1}: Reviewing...");

            // Review the code
            var reviewResponse = await _reviewerAgent.RunAsync(
                $"Review this code and provide specific improvement suggestions. Rate it 1-10:\n\n{currentCode}",
                cancellationToken: cancellationToken);
            var review = reviewResponse.Text;

            // Parse score from review
            var score = ParseScoreFromReview(review);

            iterations.Add(new RefinementIteration
            {
                Code = currentCode,
                Review = review,
                Score = score
            });

            // If score is good enough, stop iterating
            if (score >= 8)
            {
                Console.WriteLine($"Code quality acceptable (score: {score})");
                break;
            }

            // Refine based on feedback (using same thread for context)
            Console.WriteLine($"Refining based on feedback (current score: {score})...");
            var refineResponse = await _coderAgent.RunAsync(
                $"Please improve the code based on this feedback:\n\n{review}",
                thread,
                cancellationToken: cancellationToken);
            currentCode = refineResponse.Text;
        }

        return new RefinementResult
        {
            FinalCode = currentCode,
            Iterations = iterations
        };
    }

    private int ParseScoreFromReview(string review)
    {
        // Simple score extraction - in practice, use structured output
        var match = System.Text.RegularExpressions.Regex.Match(review, @"(\d+)/10");
        return match.Success ? int.Parse(match.Groups[1].Value) : 5;
    }
}

public record RefinementIteration
{
    public string Code { get; init; } = "";
    public string Review { get; init; } = "";
    public int Score { get; init; }
}

public record RefinementResult
{
    public string FinalCode { get; init; } = "";
    public List<RefinementIteration> Iterations { get; init; } = new();
}
```

## Human-in-the-Loop Workflow

Include human approval steps in the workflow.

```csharp
public class HumanApprovalWorkflow
{
    private readonly OpencodeAgent _codeAgent;
    private readonly IApprovalService _approvalService;

    public HumanApprovalWorkflow(
        IOpencodeClient opcodeClient,
        IApprovalService approvalService)
    {
        _codeAgent = new OpencodeAgent(opcodeClient, "CodeGenerator");
        _approvalService = approvalService;
    }

    public async Task<ApprovalResult> GenerateWithApprovalAsync(
        string requirement,
        CancellationToken cancellationToken = default)
    {
        // Generate code
        Console.WriteLine("Generating code...");
        var response = await _codeAgent.RunAsync(requirement, cancellationToken: cancellationToken);
        var code = response.Text;

        // Request human approval
        Console.WriteLine("Requesting approval...");
        var approvalRequest = new ApprovalRequest
        {
            Title = "Code Review Required",
            Description = $"Generated code for: {requirement}",
            Content = code,
            RequestedBy = "CodeGenerator Agent",
            RequestedAt = DateTimeOffset.UtcNow
        };

        var approvalDecision = await _approvalService.RequestApprovalAsync(
            approvalRequest,
            cancellationToken);

        if (approvalDecision.Approved)
        {
            Console.WriteLine("Code approved!");
            return new ApprovalResult
            {
                Code = code,
                Approved = true,
                ApprovedBy = approvalDecision.ApprovedBy,
                ApprovedAt = approvalDecision.DecisionTime
            };
        }
        else
        {
            Console.WriteLine($"Code rejected: {approvalDecision.Reason}");

            // Optionally, regenerate based on feedback
            if (!string.IsNullOrEmpty(approvalDecision.Feedback))
            {
                var thread = _codeAgent.GetNewThread();
                await _codeAgent.RunAsync(requirement, thread, cancellationToken: cancellationToken);
                var refinedResponse = await _codeAgent.RunAsync(
                    $"Please revise the code based on this feedback: {approvalDecision.Feedback}",
                    thread,
                    cancellationToken: cancellationToken);

                return new ApprovalResult
                {
                    Code = refinedResponse.Text,
                    Approved = false,
                    RejectionReason = approvalDecision.Reason,
                    RequiresReApproval = true
                };
            }

            return new ApprovalResult
            {
                Code = code,
                Approved = false,
                RejectionReason = approvalDecision.Reason
            };
        }
    }
}

public interface IApprovalService
{
    Task<ApprovalDecision> RequestApprovalAsync(
        ApprovalRequest request,
        CancellationToken cancellationToken = default);
}

public record ApprovalRequest
{
    public string Title { get; init; } = "";
    public string Description { get; init; } = "";
    public string Content { get; init; } = "";
    public string RequestedBy { get; init; } = "";
    public DateTimeOffset RequestedAt { get; init; }
}

public record ApprovalDecision
{
    public bool Approved { get; init; }
    public string? ApprovedBy { get; init; }
    public DateTimeOffset DecisionTime { get; init; }
    public string? Reason { get; init; }
    public string? Feedback { get; init; }
}

public record ApprovalResult
{
    public string Code { get; init; } = "";
    public bool Approved { get; init; }
    public string? ApprovedBy { get; init; }
    public DateTimeOffset? ApprovedAt { get; init; }
    public string? RejectionReason { get; init; }
    public bool RequiresReApproval { get; init; }
}
```

## Code Migration Workflow

Migrate code from one language to another.

```csharp
public class CodeMigrationWorkflow
{
    private readonly OpencodeAgent _analyzerAgent;
    private readonly OpencodeAgent _migratorAgent;
    private readonly OpencodeAgent _validatorAgent;

    public CodeMigrationWorkflow(IOpencodeClient opcodeClient)
    {
        _analyzerAgent = new OpencodeAgent(opcodeClient, new OpencodeAgentOptions
        {
            Name = "CodeAnalyzer",
            SystemPrompt = "Analyze code and explain its structure, patterns, and dependencies."
        });

        _migratorAgent = new OpencodeAgent(opcodeClient, new OpencodeAgentOptions
        {
            Name = "CodeMigrator",
            SystemPrompt = "Migrate code between languages while preserving functionality and idioms."
        });

        _validatorAgent = new OpencodeAgent(opcodeClient, new OpencodeAgentOptions
        {
            Name = "CodeValidator",
            SystemPrompt = "Validate migrated code for correctness and identify potential issues."
        });
    }

    public async Task<MigrationResult> MigrateCodeAsync(
        string sourceCode,
        string sourceLanguage,
        string targetLanguage,
        CancellationToken cancellationToken = default)
    {
        // Step 1: Analyze the source code
        Console.WriteLine($"Analyzing {sourceLanguage} code...");
        var analysisResponse = await _analyzerAgent.RunAsync(
            $"Analyze this {sourceLanguage} code. Identify patterns, dependencies, and key functionality:\n\n```{sourceLanguage}\n{sourceCode}\n```",
            cancellationToken: cancellationToken);
        var analysis = analysisResponse.Text;

        // Step 2: Migrate the code
        Console.WriteLine($"Migrating to {targetLanguage}...");
        var migrationThread = _migratorAgent.GetNewThread();
        var migrationResponse = await _migratorAgent.RunAsync(
            $"Migrate this {sourceLanguage} code to {targetLanguage}. Use idiomatic {targetLanguage} patterns.\n\nAnalysis:\n{analysis}\n\nSource code:\n```{sourceLanguage}\n{sourceCode}\n```",
            migrationThread,
            cancellationToken: cancellationToken);
        var migratedCode = migrationResponse.Text;

        // Step 3: Validate the migration
        Console.WriteLine("Validating migration...");
        var validationResponse = await _validatorAgent.RunAsync(
            $"Compare these two code snippets. Verify the {targetLanguage} code preserves the functionality of the {sourceLanguage} code. List any issues.\n\nOriginal ({sourceLanguage}):\n```{sourceLanguage}\n{sourceCode}\n```\n\nMigrated ({targetLanguage}):\n```{targetLanguage}\n{migratedCode}\n```",
            cancellationToken: cancellationToken);
        var validation = validationResponse.Text;

        // Step 4: Fix any issues identified
        var issues = ExtractIssues(validation);
        if (issues.Count > 0)
        {
            Console.WriteLine($"Fixing {issues.Count} issues...");
            var fixResponse = await _migratorAgent.RunAsync(
                $"Fix these issues in the migrated code:\n{string.Join("\n", issues)}",
                migrationThread, // Continue in same thread for context
                cancellationToken: cancellationToken);
            migratedCode = fixResponse.Text;
        }

        return new MigrationResult
        {
            SourceLanguage = sourceLanguage,
            TargetLanguage = targetLanguage,
            OriginalCode = sourceCode,
            MigratedCode = migratedCode,
            Analysis = analysis,
            Validation = validation,
            IssuesFixed = issues
        };
    }

    private List<string> ExtractIssues(string validation)
    {
        // Simple extraction - in practice, use structured output
        var issues = new List<string>();
        foreach (var line in validation.Split('\n'))
        {
            if (line.TrimStart().StartsWith("- ") || line.TrimStart().StartsWith("* "))
            {
                issues.Add(line.Trim());
            }
        }
        return issues;
    }
}

public record MigrationResult
{
    public string SourceLanguage { get; init; } = "";
    public string TargetLanguage { get; init; } = "";
    public string OriginalCode { get; init; } = "";
    public string MigratedCode { get; init; } = "";
    public string Analysis { get; init; } = "";
    public string Validation { get; init; } = "";
    public List<string> IssuesFixed { get; init; } = new();
}
```

## Complete Workflow Registration

Register all workflows with dependency injection.

```csharp
// Program.cs or Startup.cs
public static class WorkflowServiceExtensions
{
    public static IServiceCollection AddCodeWorkflows(this IServiceCollection services)
    {
        // Register OpencodeAI client (connects to local server, no API key needed)
        services.AddOpencodeClient(options =>
        {
            options.BaseUrl = Environment.GetEnvironmentVariable("OPENCODE_URL")
                ?? "http://localhost:9123";
        });

        // Register individual workflows
        services.AddScoped<CodeDevelopmentWorkflow>();
        services.AddScoped<ParallelCodeGenerationWorkflow>();
        services.AddScoped<IterativeRefinementWorkflow>();
        services.AddScoped<HumanApprovalWorkflow>();
        services.AddScoped<CodeMigrationWorkflow>();

        return services;
    }
}

// Usage in a controller
[ApiController]
[Route("api/[controller]")]
public class WorkflowController : ControllerBase
{
    private readonly CodeDevelopmentWorkflow _developmentWorkflow;
    private readonly CodeMigrationWorkflow _migrationWorkflow;

    public WorkflowController(
        CodeDevelopmentWorkflow developmentWorkflow,
        CodeMigrationWorkflow migrationWorkflow)
    {
        _developmentWorkflow = developmentWorkflow;
        _migrationWorkflow = migrationWorkflow;
    }

    [HttpPost("develop")]
    public async Task<IActionResult> DevelopFeature([FromBody] DevelopRequest request)
    {
        var result = await _developmentWorkflow.DevelopFeatureAsync(request.Requirement);
        return Ok(result);
    }

    [HttpPost("migrate")]
    public async Task<IActionResult> MigrateCode([FromBody] MigrateRequest request)
    {
        var result = await _migrationWorkflow.MigrateCodeAsync(
            request.Code,
            request.SourceLanguage,
            request.TargetLanguage);
        return Ok(result);
    }
}

public record DevelopRequest(string Requirement);
public record MigrateRequest(string Code, string SourceLanguage, string TargetLanguage);
```
