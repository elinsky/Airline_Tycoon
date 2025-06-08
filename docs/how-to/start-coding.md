# How to Start Coding in Airline Tycoon

This guide is for experienced developers who need to understand the Airline Tycoon development workflow and toolchain.

## Quick Start

```bash
# Clone and setup
git clone https://github.com/elinsky/airline-tycoon.git
cd airline-tycoon
./setup.sh    # Installs tools, configures git hooks, runs initial build

# Verify everything works
dotnet build && dotnet test

# Open in your IDE
code .                          # VS Code
# or open AirlineTycoon.sln     # Rider/Visual Studio
```

**Key directories:**
- `src/` - Source code
- `tests/` - Unit and integration tests  
- `docs/` - Documentation (you are here)

## Development Workflow

### Branching Strategy

```bash
git checkout -b feature/add-flight-scheduling    # New features
git checkout -b fix/route-calculation-bug        # Bug fixes
git checkout -b chore/update-dependencies        # Maintenance
```

### Commit Format

Follow conventional commits:
```bash
git commit -m "feat(flights): add dynamic pricing algorithm"
git commit -m "fix(ui): correct button alignment in booking view"
git commit -m "test(routes): add edge cases for international flights"
```

### Pre-commit Hooks

Hooks run automatically on commit:
1. **CSharpier** - Code formatting
2. **dotnet build** - Compilation with all analyzers
3. **dotnet test** - All unit tests
4. **dotnet-outdated** - Dependency check

Failed commits mean code quality issues. Fix them before committing.

## Code Quality Toolchain

| Tool | Purpose | Equivalent |
|------|---------|------------|
| **CSharpier** | Opinionated formatter | Black (Python), Prettier (JS) |
| **StyleCop** | Style enforcement | ESLint, Flake8 |
| **Roslynator** | 500+ code analyzers | Pylint, RuboCop |
| **Security Code Scan** | SAST vulnerability scanning | Bandit, Semgrep |

**Important:** All warnings are treated as errors. Your code won't build with any analyzer warnings.

## Essential Commands

```bash
# Development
dotnet build                    # Build with all analyzers
dotnet test                     # Run tests with coverage
dotnet run --project src/AirlineTycoon   # Run the application

# Code Quality
csharpier format .              # Format all code
csharpier check .               # Check formatting (CI mode)
dotnet-outdated                 # Check for outdated packages

# Testing
dotnet test --collect:"XPlat Code Coverage"     # Run with coverage
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

# Git Hooks (if needed)
git commit --no-verify          # Bypass pre-commit hooks (emergency only)
```

## IDE Integration

### Visual Studio Code

Extensions auto-install on first open. Key features:

- Format on save enabled
- Real-time analyzer feedback
- Integrated terminal with .NET tools
- Debugger pre-configured

### JetBrains Rider

- Open `AirlineTycoon.sln`
- Uses `.editorconfig` for all style rules
- Analyzers work out-of-the-box
- Same formatting rules as VS Code

## Testing Requirements

- **Minimum 80% code coverage** (build fails below this)
- **Stack:** xUnit + FluentAssertions + Moq
- **Location:** Tests go in `tests/AirlineTycoon.Tests/`
- **Naming:** `ClassName_MethodName_ExpectedBehavior`

Example test:
```csharp
[Fact]
public void FlightRoute_CalculatePrice_AppliesPeakSurcharge()
{
    // Arrange
    var route = new FlightRoute("JFK", "LAX");
    
    // Act  
    var price = route.CalculatePrice(isPeakTime: true);
    
    // Assert
    price.Should().BeGreaterThan(route.BasePrice);
}
```

## Configuration Files

| File | Purpose | Edit? |
|------|---------|-------|
| `.editorconfig` | Code style rules | ❌ No |
| `Directory.Build.props` | MSBuild/analyzer config | ❌ No |
| `.csharpierrc.json` | Formatter settings | ❌ No |
| `.husky/task-runner.json` | Git hooks | ⚠️ Rarely |
| `appsettings.json` | App configuration | ✅ Yes |

## Common Gotchas

### PATH not set
```bash
# Add to ~/.zprofile or ~/.bashrc
export PATH="$PATH:$HOME/.dotnet/tools"
```

### Pre-commit fails on formatting
```bash
# Just run the formatter
csharpier format .
git add -u
git commit  # Try again
```

### Analyzer warning unclear
```csharp
#pragma warning disable CA1062 // Validate arguments of public methods
public void Process(Order order) 
{
    // Document WHY you're disabling this
    order.Process(); // We validated in the controller
}
#pragma warning restore CA1062
```

### Build fails after git pull
```bash
# Clean rebuild usually fixes it
dotnet clean
rm -rf obj/ bin/
dotnet restore
dotnet build
```

## Performance Tips

- Use `dotnet watch run` for hot reload during development
- Run only affected tests: `dotnet test --filter "FullyQualifiedName~FlightRoute"`
- Use `dotnet build --no-restore` after first build for speed
- Disable analyzers temporarily: `dotnet build /p:RunAnalyzers=false` (local only!)

## Next Steps

- Review [Architecture Overview](../explanation/architecture.md)
- Check [API Conventions](../reference/api-conventions.md)
- Browse existing code in `src/AirlineTycoon/`
- Pick an issue labeled `good first issue`

---

**Remember:** The tooling is here to help. If something seems overly restrictive, ask in Discussions - there's usually a good reason or we can adjust it.