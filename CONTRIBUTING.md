# Contributing to LionFire.OpenCode.Serve

Thank you for your interest in contributing to LionFire.OpenCode.Serve! This document provides guidelines and instructions for contributing.

## Table of Contents

- [Code of Conduct](#code-of-conduct)
- [Getting Started](#getting-started)
- [Development Setup](#development-setup)
- [Making Changes](#making-changes)
- [Submitting Changes](#submitting-changes)
- [Coding Guidelines](#coding-guidelines)
- [Testing Guidelines](#testing-guidelines)
- [Documentation](#documentation)
- [Getting Help](#getting-help)

## Code of Conduct

This project follows our [Code of Conduct](CODE_OF_CONDUCT.md). By participating, you are expected to uphold this code.

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later
- Git
- OpenCode (for integration testing)

### Fork and Clone

1. Fork the repository on GitHub
2. Clone your fork locally:
   ```bash
   git clone https://github.com/YOUR-USERNAME/opencode-dotnet.git
   cd opencode-dotnet
   ```
3. Add the upstream remote:
   ```bash
   git remote add upstream https://github.com/lionfire/opencode-dotnet.git
   ```

## Development Setup

### Build the Project

```bash
dotnet restore
dotnet build
```

### Run Tests

```bash
dotnet test
```

### Run Examples

```bash
# Start OpenCode server first
opencode serve

# Run an example
dotnet run --project examples/BasicSession
```

## Making Changes

### Branch Naming

Use descriptive branch names:
- `feature/add-websocket-support`
- `fix/connection-timeout-handling`
- `docs/update-api-reference`
- `refactor/simplify-message-parsing`

### Creating a Branch

```bash
git checkout main
git pull upstream main
git checkout -b feature/your-feature-name
```

### Commit Messages

Follow the [Conventional Commits](https://www.conventionalcommits.org/) specification:

```
type(scope): description

[optional body]

[optional footer]
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting, etc.)
- `refactor`: Code refactoring
- `test`: Adding or updating tests
- `chore`: Build process or auxiliary tool changes

**Examples:**
```
feat(client): add WebSocket support for PTY connections
fix(session): handle null response in ForkSessionAsync
docs(readme): update quick start example
test(message): add tests for MessageConverter
```

## Submitting Changes

### Before Submitting

1. **Update your branch:**
   ```bash
   git fetch upstream
   git rebase upstream/main
   ```

2. **Run all tests:**
   ```bash
   dotnet test
   ```

3. **Check for warnings:**
   ```bash
   dotnet build --configuration Release
   ```

4. **Format code:**
   Ensure code follows the project's style guidelines.

### Pull Request Process

1. Push your branch to your fork:
   ```bash
   git push origin feature/your-feature-name
   ```

2. Create a Pull Request on GitHub

3. Fill out the PR template completely

4. Wait for review and address any feedback

5. Once approved, a maintainer will merge your PR

### PR Requirements

- [ ] Tests pass
- [ ] No new warnings
- [ ] Documentation updated (if applicable)
- [ ] PR description explains the changes
- [ ] Linked to relevant issues

## Coding Guidelines

### General Principles

- Follow existing code style and patterns
- Write self-documenting code
- Keep methods small and focused
- Prefer composition over inheritance

### C# Style

- Use C# 12 features where appropriate
- Use `var` for obviously-typed variables
- Use records for DTOs and immutable types
- Use nullable reference types consistently
- Add XML documentation for public APIs

### Example

```csharp
/// <summary>
/// Gets the session by ID.
/// </summary>
/// <param name="sessionId">The session ID.</param>
/// <param name="cancellationToken">Cancellation token.</param>
/// <returns>The session.</returns>
/// <exception cref="OpenCodeNotFoundException">Session not found.</exception>
public async Task<Session> GetSessionAsync(
    string sessionId,
    CancellationToken cancellationToken = default)
{
    ArgumentException.ThrowIfNullOrEmpty(sessionId);

    var url = $"{ApiEndpoints.Sessions}/{sessionId}";
    return await GetAsync<Session>(url, cancellationToken)
        ?? throw new OpenCodeNotFoundException($"Session {sessionId} not found");
}
```

### Naming Conventions

| Element | Convention | Example |
|---------|------------|---------|
| Namespaces | PascalCase | `LionFire.OpenCode.Serve` |
| Classes | PascalCase | `OpenCodeClient` |
| Interfaces | I + PascalCase | `IOpenCodeClient` |
| Methods | PascalCase | `CreateSessionAsync` |
| Parameters | camelCase | `sessionId` |
| Private fields | _camelCase | `_httpClient` |
| Constants | PascalCase | `DefaultTimeout` |

## Testing Guidelines

### Test Structure

```csharp
[Fact]
public async Task MethodName_StateUnderTest_ExpectedBehavior()
{
    // Arrange
    var client = CreateTestClient();

    // Act
    var result = await client.MethodAsync();

    // Assert
    Assert.NotNull(result);
}
```

### Test Categories

- **Unit Tests**: Test individual methods in isolation
- **Integration Tests**: Test interactions with OpenCode server
- **Example Tests**: Ensure examples compile and run

### Running Specific Tests

```bash
# Run specific test class
dotnet test --filter "FullyQualifiedName~OpenCodeClientTests"

# Run tests with specific trait
dotnet test --filter "Category=Unit"
```

## Documentation

### XML Documentation

Add XML docs to all public members:

```csharp
/// <summary>
/// Creates a new session.
/// </summary>
/// <param name="request">Optional session creation request.</param>
/// <param name="cancellationToken">Cancellation token.</param>
/// <returns>The created session.</returns>
public Task<Session> CreateSessionAsync(
    CreateSessionRequest? request = null,
    CancellationToken cancellationToken = default);
```

### Markdown Documentation

- Update relevant docs in the `docs/` folder
- Keep README.md up to date
- Add examples for new features

## Getting Help

### Questions

- Check existing [issues](https://github.com/lionfire/opencode-dotnet/issues)
- Open a [discussion](https://github.com/lionfire/opencode-dotnet/discussions)
- Ask in the community Discord/Slack

### Reporting Bugs

Use the bug report issue template and include:
- .NET version
- OpenCode version
- Minimal reproduction steps
- Expected vs actual behavior
- Error messages and stack traces

### Suggesting Features

Use the feature request issue template and include:
- Problem you're trying to solve
- Proposed solution
- Alternatives considered
- Willingness to implement

## Recognition

Contributors are recognized in:
- GitHub Contributors page
- Release notes
- README acknowledgments

Thank you for contributing!
