# Integration Option 3: Hybrid Workflow Architecture

## Overview

Combine Microsoft Agent Framework's workflow orchestration capabilities with OpencodeAI's specialized code generation expertise. This architecture uses Agent Framework for high-level task coordination and OpencodeAI agents for code-specific operations.

## Architecture

```
┌─────────────────────────────────────────────────────────────┐
│           Microsoft Agent Framework Workflow                │
│                 (WorkflowHostAgent)                         │
└───────┬──────────────┬────────────┬──────────────┬──────────┘
        │              │            │              │
        ▼              ▼            ▼              ▼
┌─────────────┐ ┌───────────┐ ┌──────────┐ ┌────────────┐
│  Research   │ │  Design   │ │ Generate │ │   Review   │
│   Agent     │ │  Agent    │ │  Agent   │ │   Agent    │
│  (OpenAI)   │ │(Anthropic)│ │(Opencode)│ │  (OpenAI)  │
└─────────────┘ └───────────┘ └──────────┘ └────────────┘
                                     │
                                     │ Uses
                                     ▼
                              ┌──────────────┐
                              │IOpencodeClient│
                              └──────────────┘
```

## Key Benefits

1. **Best of Both Worlds**: Agent Framework orchestration + OpencodeAI code expertise
2. **Clear Separation of Concerns**: General AI for planning, OpencodeAI for implementation
3. **Complex Workflows**: Multi-step processes (research → design → code → review → test)
4. **Parallel Processing**: Run independent tasks concurrently
5. **Human-in-the-Loop**: Built-in approval gates and checkpointing
6. **State Management**: Workflow state persists across long-running operations

## Workflow Patterns

### Pattern 1: Sequential Code Development

```csharp
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using OpencodeAI;

// Create specialized agents
var researchAgent = CreateOpenAIAgent("Researcher",
    "Research technical requirements and best practices");

var architectAgent = CreateOpenAIAgent("Architect",
    "Design software architecture and API contracts");

var opcodeClient = serviceProvider.GetRequiredService<IOpencodeClient>();
var codeAgent = new OpencodeAgent(opcodeClient, "CodeGenerator",
    "Generate production-ready code following the architecture");

var reviewAgent = CreateOpenAIAgent("CodeReviewer",
    "Review code for quality, security, and best practices");

var testAgent = new OpencodeAgent(opcodeClient, "TestGenerator",
    "Generate comprehensive unit tests for the code");

// Define workflow
var workflow = new WorkflowBuilder()
    .AddStep("research", async (input) =>
    {
        var response = await researchAgent.RunAsync(
            $"Research best practices for: {input.UserRequirement}");
        return new { Research = response.ToString() };
    })
    .AddStep("design", async (input) =>
    {
        var response = await architectAgent.RunAsync(
            $"Based on this research: {input.Research}\n" +
            $"Design an architecture for: {input.UserRequirement}");
        return new { Architecture = response.ToString() };
    })
    .AddStep("generate", async (input) =>
    {
        var response = await codeAgent.RunAsync(
            $"Implement this architecture: {input.Architecture}\n" +
            $"Requirements: {input.UserRequirement}");
        return new { Code = response.ToString() };
    })
    .AddStep("review", async (input) =>
    {
        var response = await reviewAgent.RunAsync(
            $"Review this code:\n{input.Code}");
        return new { ReviewFeedback = response.ToString() };
    })
    .AddStep("test", async (input) =>
    {
        var response = await testAgent.RunAsync(
            $"Generate tests for this code:\n{input.Code}");
        return new { Tests = response.ToString() };
    })
    .Build();

// Execute workflow
var result = await workflow.RunAsync(new
{
    UserRequirement = "Build a REST API for user management"
});

Console.WriteLine($"Code: {result.Code}");
Console.WriteLine($"Tests: {result.Tests}");
Console.WriteLine($"Review: {result.ReviewFeedback}");
```

### Pattern 2: Parallel Code Generation

```csharp
// Generate multiple components in parallel
var workflow = new WorkflowBuilder()
    .AddParallelSteps(
        ("backend", async (input) =>
        {
            return await backendCodeAgent.RunAsync(
                $"Generate backend API for: {input.Spec}");
        }),
        ("frontend", async (input) =>
        {
            return await frontendCodeAgent.RunAsync(
                $"Generate React frontend for: {input.Spec}");
        }),
        ("tests", async (input) =>
        {
            return await testCodeAgent.RunAsync(
                $"Generate E2E tests for: {input.Spec}");
        }))
    .AddStep("integrate", async (input) =>
    {
        return await integrationAgent.RunAsync(
            $"Create integration layer for:\n" +
            $"Backend: {input.Backend}\n" +
            $"Frontend: {input.Frontend}\n" +
            $"Tests: {input.Tests}");
    })
    .Build();
```

### Pattern 3: Conditional Workflow with Human Approval

```csharp
var workflow = new WorkflowBuilder()
    .AddStep("generate", async (input) =>
    {
        return await opcodeAgent.RunAsync(input.Requirement);
    })
    .AddStep("review", async (input) =>
    {
        return await reviewAgent.RunAsync($"Review: {input.Code}");
    })
    .AddConditionalStep("needsRevision",
        condition: (input) => input.ReviewScore < 8,
        thenStep: async (input) =>
        {
            // Revise code based on feedback
            return await opcodeAgent.RunAsync(
                $"Revise this code based on feedback:\n" +
                $"Code: {input.Code}\n" +
                $"Feedback: {input.ReviewFeedback}");
        },
        elseStep: async (input) =>
        {
            // Code is good, proceed
            return input;
        })
    .AddHumanApprovalStep("approve",
        message: "Review the generated code and approve or reject",
        onApprove: async (input) =>
        {
            // Deploy or commit the code
            return await deployAgent.RunAsync($"Deploy: {input.Code}");
        },
        onReject: async (input) =>
        {
            // Loop back to generate step
            throw new WorkflowRejectedException("Code rejected by human reviewer");
        })
    .Build();
```

### Pattern 4: Iterative Refinement Workflow

```csharp
public class IterativeCodeWorkflow
{
    private readonly IOpencodeClient _opcodeClient;
    private readonly AIAgent _reviewAgent;
    private const int MaxIterations = 3;

    public async Task<CodeResult> GenerateAndRefineAsync(
        string requirement,
        CancellationToken cancellationToken = default)
    {
        var iteration = 0;
        var currentCode = string.Empty;
        var reviewScore = 0;

        var workflow = new WorkflowBuilder()
            .AddStep("generate", async (input) =>
            {
                var result = await _opcodeClient.GenerateCodeAsync(
                    new GenerateCodeRequest
                    {
                        Prompt = iteration == 0
                            ? requirement
                            : $"{requirement}\n\nImprove based on: {input.Feedback}",
                        ProgrammingLanguage = ProgrammingLanguage.CSharp
                    },
                    cancellationToken);

                currentCode = result.Code;
                iteration++;

                return new { Code = result.Code };
            })
            .AddStep("review", async (input) =>
            {
                var review = await _opcodeClient.ReviewCodeAsync(
                    input.Code,
                    ProgrammingLanguage.CSharp,
                    cancellationToken: cancellationToken);

                reviewScore = review.Score;

                return new
                {
                    Score = review.Score,
                    Issues = review.Issues,
                    Feedback = string.Join("\n", review.Suggestions.Select(s => s.Description))
                };
            })
            .AddWhileLoop(
                condition: () => reviewScore < 8 && iteration < MaxIterations,
                loopBody: new WorkflowBuilder()
                    .AddStep("refine", async (input) =>
                    {
                        return await _opcodeClient.SuggestRefactoringAsync(
                            currentCode,
                            ProgrammingLanguage.CSharp,
                            goal: RefactoringGoal.Readability | RefactoringGoal.Maintainability,
                            cancellationToken: cancellationToken);
                    })
                    .Build())
            .Build();

        var result = await workflow.RunAsync(new { Requirement = requirement });

        return new CodeResult
        {
            Code = currentCode,
            ReviewScore = reviewScore,
            Iterations = iteration
        };
    }
}
```

## Real-World Workflow Examples

### Example 1: Full-Stack Development Workflow

```csharp
public class FullStackWorkflow
{
    public async Task<ProjectResult> BuildFullStackAppAsync(string appDescription)
    {
        var workflow = new WorkflowBuilder()
            // Phase 1: Planning
            .AddStep("analyze-requirements", async (input) =>
            {
                var agent = CreatePlanningAgent();
                return await agent.RunAsync(
                    $"Analyze these requirements and create a detailed spec: {appDescription}");
            })

            // Phase 2: Database Design
            .AddStep("design-database", async (input) =>
            {
                var agent = CreateDatabaseAgent();
                return await agent.RunAsync(
                    $"Design database schema for: {input.Spec}");
            })

            // Phase 3: Parallel Implementation
            .AddParallelSteps(
                ("api", async (input) =>
                {
                    var opcodeAgent = new OpencodeAgent(_opcodeClient);
                    return await opcodeAgent.RunAsync(
                        $"Generate .NET Web API with EF Core for schema: {input.DatabaseSchema}");
                }),
                ("frontend", async (input) =>
                {
                    var opcodeAgent = new OpencodeAgent(_opcodeClient);
                    return await opcodeAgent.RunAsync(
                        $"Generate React TypeScript frontend for spec: {input.Spec}");
                }),
                ("tests", async (input) =>
                {
                    var opcodeAgent = new OpencodeAgent(_opcodeClient);
                    return await opcodeAgent.RunAsync(
                        $"Generate integration tests for: {input.Spec}");
                }))

            // Phase 4: Integration
            .AddStep("integrate", async (input) =>
            {
                var agent = CreateIntegrationAgent();
                return await agent.RunAsync(
                    $"Create deployment configs and Docker compose for:\n" +
                    $"API: {input.Api}\nFrontend: {input.Frontend}\nTests: {input.Tests}");
            })

            // Phase 5: Documentation
            .AddStep("document", async (input) =>
            {
                var agent = CreateDocAgent();
                return await agent.RunAsync(
                    $"Generate README, API docs, and deployment guide for: {input}");
            })
            .Build();

        return await workflow.RunAsync(new { AppDescription = appDescription });
    }
}
```

### Example 2: Code Migration Workflow

```csharp
public class CodeMigrationWorkflow
{
    public async Task<MigrationResult> MigrateCodebaseAsync(
        string sourceLanguage,
        string targetLanguage,
        string codebasePath)
    {
        var workflow = new WorkflowBuilder()
            // Analyze source code
            .AddStep("analyze-source", async (input) =>
            {
                var files = Directory.GetFiles(codebasePath, $"*.{sourceLanguage}");
                var analysis = new List<FileAnalysis>();

                foreach (var file in files)
                {
                    var content = await File.ReadAllTextAsync(file);
                    var explanation = await _opcodeClient.ExplainCodeAsync(
                        content,
                        Enum.Parse<ProgrammingLanguage>(sourceLanguage, true));

                    analysis.Add(new FileAnalysis
                    {
                        Path = file,
                        Content = content,
                        Explanation = explanation
                    });
                }

                return new { SourceFiles = analysis };
            })

            // Migrate each file
            .AddForEachStep("migrate-files",
                source: (input) => input.SourceFiles,
                action: async (file) =>
                {
                    var migrationPrompt =
                        $"Migrate this {sourceLanguage} code to {targetLanguage}:\n" +
                        $"{file.Content}\n\n" +
                        $"Explanation: {file.Explanation.Summary}\n" +
                        $"Preserve all functionality and improve where possible.";

                    var result = await _opcodeClient.GenerateCodeAsync(
                        new GenerateCodeRequest
                        {
                            Prompt = migrationPrompt,
                            ProgrammingLanguage = Enum.Parse<ProgrammingLanguage>(targetLanguage, true)
                        });

                    return new MigratedFile
                    {
                        OriginalPath = file.Path,
                        NewPath = file.Path.Replace($".{sourceLanguage}", $".{targetLanguage}"),
                        Code = result.Code
                    };
                })

            // Review migrated code
            .AddStep("review-migration", async (input) =>
            {
                var reviews = new List<CodeReview>();

                foreach (var migratedFile in input.MigratedFiles)
                {
                    var review = await _opcodeClient.ReviewCodeAsync(
                        migratedFile.Code,
                        Enum.Parse<ProgrammingLanguage>(targetLanguage, true));

                    reviews.Add(new CodeReview
                    {
                        File = migratedFile.NewPath,
                        Review = review
                    });
                }

                return new { Reviews = reviews };
            })

            // Generate migration report
            .AddStep("generate-report", async (input) =>
            {
                var reportAgent = CreateReportAgent();
                return await reportAgent.RunAsync(
                    $"Generate migration report for:\n" +
                    $"Source: {sourceLanguage}\n" +
                    $"Target: {targetLanguage}\n" +
                    $"Files: {input.MigratedFiles.Count}\n" +
                    $"Reviews: {string.Join("\n", input.Reviews)}");
            })
            .Build();

        return await workflow.RunAsync(new
        {
            SourceLanguage = sourceLanguage,
            TargetLanguage = targetLanguage
        });
    }
}
```

### Example 3: AI-Assisted Code Review Pipeline

```csharp
public class CodeReviewPipeline
{
    public async Task<ReviewResult> ReviewPullRequestAsync(string prNumber)
    {
        var workflow = new WorkflowBuilder()
            // Fetch PR changes
            .AddStep("fetch-changes", async (input) =>
            {
                // Use GitHub API or similar
                var changes = await FetchPullRequestChanges(prNumber);
                return new { Changes = changes };
            })

            // Parallel review tasks
            .AddParallelSteps(
                ("security-review", async (input) =>
                {
                    var reviews = new List<SecurityReview>();
                    foreach (var change in input.Changes)
                    {
                        var review = await _opcodeClient.ReviewCodeAsync(
                            change.Code,
                            change.Language,
                            focus: ReviewFocus.Security | ReviewFocus.BugRisk);
                        reviews.Add(new SecurityReview { File = change.Path, Review = review });
                    }
                    return new { SecurityReviews = reviews };
                }),
                ("performance-review", async (input) =>
                {
                    var reviews = new List<PerformanceReview>();
                    foreach (var change in input.Changes)
                    {
                        var review = await _opcodeClient.ReviewCodeAsync(
                            change.Code,
                            change.Language,
                            focus: ReviewFocus.Performance);
                        reviews.Add(new PerformanceReview { File = change.Path, Review = review });
                    }
                    return new { PerformanceReviews = reviews };
                }),
                ("style-review", async (input) =>
                {
                    var reviews = new List<StyleReview>();
                    foreach (var change in input.Changes)
                    {
                        var review = await _opcodeClient.ReviewCodeAsync(
                            change.Code,
                            change.Language,
                            focus: ReviewFocus.Style | ReviewFocus.Documentation);
                        reviews.Add(new StyleReview { File = change.Path, Review = review });
                    }
                    return new { StyleReviews = reviews };
                }))

            // Aggregate reviews
            .AddStep("aggregate", async (input) =>
            {
                var aggregatorAgent = CreateAggregatorAgent();
                return await aggregatorAgent.RunAsync(
                    $"Aggregate these reviews into a summary:\n" +
                    $"Security: {input.SecurityReviews}\n" +
                    $"Performance: {input.PerformanceReviews}\n" +
                    $"Style: {input.StyleReviews}");
            })

            // Post review comment
            .AddStep("post-comment", async (input) =>
            {
                await PostPullRequestComment(prNumber, input.Summary);
                return new { Posted = true };
            })
            .Build();

        return await workflow.RunAsync(new { PrNumber = prNumber });
    }
}
```

## Workflow State Management

```csharp
// Persist workflow state for long-running operations
public class PersistentCodeWorkflow
{
    private readonly IWorkflowStateStore _stateStore;

    public async Task<string> RunWithCheckpointsAsync(string workflowId)
    {
        var workflow = new WorkflowBuilder()
            .AddStep("step1", async (input) =>
            {
                var result = await ProcessStep1(input);
                await _stateStore.SaveCheckpointAsync(workflowId, "step1", result);
                return result;
            })
            .AddStep("step2", async (input) =>
            {
                var result = await ProcessStep2(input);
                await _stateStore.SaveCheckpointAsync(workflowId, "step2", result);
                return result;
            })
            .WithCheckpointing(enabled: true)
            .WithErrorRecovery(strategy: ErrorRecoveryStrategy.RetryWithBackoff)
            .Build();

        // Can resume from last checkpoint if workflow fails
        var state = await _stateStore.LoadCheckpointAsync(workflowId);
        return await workflow.ResumeAsync(state);
    }
}
```

## Benefits Summary

| Aspect | Benefit |
|--------|---------|
| **Orchestration** | Complex multi-agent workflows with clear flow |
| **Specialization** | Right agent for each task (OpenAI planning, OpencodeAI coding) |
| **Parallelization** | Run independent code generation tasks concurrently |
| **State Management** | Persist workflow state, resume from checkpoints |
| **Error Handling** | Retry logic, fallback strategies, human intervention |
| **Cost Optimization** | Use cheaper models for simple tasks, OpencodeAI for code |
| **Observability** | Built-in telemetry, logging, progress tracking |

## Best Use Cases

1. **Full-Stack Development**: Coordinate multiple code generation agents for different tiers
2. **Code Migration**: Systematic codebase transformation workflows
3. **CI/CD Pipelines**: Automated code review, testing, deployment workflows
4. **Iterative Refinement**: Generate → Review → Refine loops
5. **Multi-Language Projects**: Coordinate code generation across multiple languages
6. **Enterprise Workflows**: Complex approval gates, compliance checks, documentation

## Next Steps

1. Implement `OpencodeAgent` (see Integration Option 1)
2. Create workflow templates for common patterns
3. Build workflow state persistence layer
4. Add telemetry and monitoring
5. Create developer documentation with examples
