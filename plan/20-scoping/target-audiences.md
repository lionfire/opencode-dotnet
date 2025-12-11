# Target Audiences

## Primary Audience: .NET Application Developers

**Description**: Professional .NET developers building applications (web apps, desktop apps, CLI tools, services) that need to integrate AI-powered coding assistance capabilities into their solutions.

**Needs**:
- Simple, reliable way to programmatically interact with OpenCode
- Strong typing and IntelliSense for rapid development
- Production-ready features (error handling, retries, cancellation)
- Easy integration into existing .NET applications
- Clear documentation and working examples

**Technical Level**: Intermediate to Advanced
- Comfortable with async/await patterns
- Familiar with HTTP APIs and REST conventions
- Experienced with NuGet package management
- May or may not be familiar with OpenCode itself

**Primary Use Cases**:
- Building developer productivity tools
- Creating custom code generation workflows
- Integrating AI assistance into internal applications
- Automating repetitive coding tasks
- Building AI-powered code review tools

**Pain Points Addressed**:
- No need to manually implement HTTP client logic
- No dealing with raw JSON serialization/deserialization
- No reinventing error handling and retry logic
- Type safety prevents runtime errors
- Simplified session management

---

## Primary Audience: .NET Library/SDK Authors

**Description**: Developers building higher-level abstractions, frameworks, or libraries on top of OpenCode for specific domains or use cases. They want to build specialized tools without dealing with low-level API details.

**Needs**:
- Stable, well-tested foundation for building higher-level APIs
- Extensibility points for customization
- Interface-based design for mocking and testing
- Comprehensive API coverage
- Clean architecture that doesn't impose constraints

**Technical Level**: Advanced to Expert
- Deep understanding of .NET patterns and practices
- Experience building reusable libraries
- Familiar with dependency injection patterns
- Strong focus on API design and developer experience

**Primary Use Cases**:
- Building domain-specific AI coding tools
- Creating VS Code or Visual Studio extensions
- Developing specialized agent workflows
- Building framework integrations (e.g., Blazor components)
- Creating testing/mocking frameworks for AI interactions

**Pain Points Addressed**:
- Provides stable API surface to build upon
- Handles low-level concerns (HTTP, JSON, retries)
- Interface-based design enables testing and mocking
- Minimal dependencies reduce conflicts
- Source-generated JSON enables AOT scenarios

---

## Primary Audience: Enterprise Development Teams

**Description**: Teams in enterprise organizations looking to leverage OpenCode for internal developer productivity, code generation pipelines, or building internal developer platforms. Focus on reliability, maintainability, and integration with existing systems.

**Needs**:
- Production-ready, enterprise-grade SDK
- Clear error handling and logging
- Integration with enterprise patterns (DI, configuration, telemetry)
- Security considerations (TLS support, no credential leakage)
- Support and maintenance roadmap

**Technical Level**: Intermediate to Advanced
- Building internal tools and platforms
- Focus on reliability and maintainability
- Need to integrate with existing enterprise infrastructure
- Concerned with security and compliance

**Primary Use Cases**:
- Building internal developer portals
- Automating code generation in CI/CD pipelines
- Creating code quality automation tools
- Building AI-assisted code review systems
- Developing training and onboarding tools

**Pain Points Addressed**:
- Production-ready from day one
- Integrates with ASP.NET Core DI
- Configuration via appsettings.json
- Observability (OpenTelemetry integration in future)
- Minimal security surface (localhost-only by default)
- Clear versioning and stability guarantees

---

## Secondary Audience: Open Source Contributors

**Description**: Developers who want to contribute to the SDK itself, either fixing bugs, adding features, or improving documentation. May come from any of the primary audiences.

**Needs**:
- Clear contribution guidelines
- Well-structured codebase that's easy to understand
- Comprehensive test suite for confidence
- Responsive maintainers
- Welcoming community

**Technical Level**: Intermediate to Advanced
- Comfortable with C# and .NET ecosystem
- Familiar with Git/GitHub workflows
- May or may not know OpenCode API deeply

**Primary Use Cases**:
- Adding support for new OpenCode API endpoints
- Improving error handling or retry logic
- Adding tests and documentation
- Fixing bugs discovered in their own usage
- Adding convenience methods or helpers

**Pain Points Addressed**:
- Clean, maintainable codebase structure
- Test suite makes changes safe
- Clear separation of concerns
- Minimal dependencies make changes easier

---

## Secondary Audience: Educators and Researchers

**Description**: Academics, educators, and researchers studying AI-assisted development, teaching programming, or conducting research on code generation patterns.

**Needs**:
- Simple, understandable examples
- Ability to programmatically analyze AI interactions
- Session management for experiments
- Access to raw API data for research

**Technical Level**: Varies (Beginner to Advanced)
- May be new to .NET
- Focused on AI/research aspects more than SDK details
- Need clear documentation

**Primary Use Cases**:
- Teaching AI-assisted programming
- Conducting research on code generation
- Building educational tools and demos
- Analyzing AI interaction patterns
- Creating reproducible experiments

**Pain Points Addressed**:
- Simple API makes teaching easier
- Session management enables experimental workflows
- Full API access enables research scenarios
- Clear examples provide learning paths
