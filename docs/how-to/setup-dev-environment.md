# How to Set Up Your Development Environment

This guide will help you set up a comprehensive C# development environment with enterprise-grade tooling for the Airline Tycoon project.

## Prerequisites

Before starting, ensure you have:

- **macOS** (Intel or Apple Silicon), **Windows 10/11**, or **Linux**
- **Homebrew** (macOS) or **Chocolatey** (Windows) package manager
- **Git** version control system
- Administrator/sudo access for installing tools

## Installation Steps

### 1. Install .NET SDK

The project uses .NET 9.0, the latest version with long-term support.

**macOS:**
```bash
brew install --cask dotnet-sdk
```

**Windows:**
```powershell
choco install dotnet-sdk
```

**Linux:**
```bash
# Ubuntu/Debian
wget https://dot.net/v1/dotnet-install.sh
chmod +x dotnet-install.sh
./dotnet-install.sh --channel 9.0
```

Verify installation:
```bash
dotnet --version
# Should output: 9.0.x
```

### 2. Clone and Set Up the Project

```bash
# Clone the repository
git clone https://github.com/your-org/airline-tycoon.git
cd airline-tycoon

# Run the automated setup script
./setup.sh  # macOS/Linux
# or
.\setup.ps1  # Windows
```

The setup script will:
- Create the solution and project structure
- Install all required .NET tools
- Configure Git hooks for code quality
- Run initial build and tests

### 3. Configure Your IDE

#### Visual Studio Code (Recommended)

1. Install VS Code from [code.visualstudio.com](https://code.visualstudio.com/)
2. Open the project: `code .`
3. Install recommended extensions when prompted
4. All settings are pre-configured in `.vscode/`

#### JetBrains Rider

1. Install Rider from [jetbrains.com/rider](https://www.jetbrains.com/rider/)
2. Open `AirlineTycoon.sln`
3. Code style settings are shared via `.editorconfig`

### 4. Add .NET Tools to PATH

The setup script installs tools to `~/.dotnet/tools`. Add this to your PATH:

**macOS/Linux (zsh):**
```bash
echo 'export PATH="$PATH:$HOME/.dotnet/tools"' >> ~/.zprofile
source ~/.zprofile
```

**Windows (PowerShell):**
```powershell
[Environment]::SetEnvironmentVariable("PATH", "$env:PATH;$env:USERPROFILE\.dotnet\tools", [EnvironmentVariableTarget]::User)
```

## Installed Tools

Your environment now includes:

### Code Quality Tools

| Tool | Purpose | Similar to (Python) |
|------|---------|-------------------|
| **CSharpier** | Opinionated code formatter | Black |
| **StyleCop** | Style and consistency rules | Flake8 |
| **Roslynator** | 500+ code analyzers | Pylint |
| **SonarAnalyzer** | Code quality and security | Bandit |

### Testing Tools

| Tool | Purpose | Similar to (Python) |
|------|---------|-------------------|
| **xUnit** | Testing framework | pytest |
| **FluentAssertions** | Readable test assertions | pytest assertions |
| **Moq** | Mocking framework | unittest.mock |
| **Coverlet** | Code coverage | coverage.py |

### Development Tools

- **dotnet-outdated** - Check for outdated packages
- **ReportGenerator** - Generate HTML coverage reports
- **Husky.NET** - Git hooks management
- **Security Code Scan** - Security vulnerability detection

## Essential Commands

### Building and Testing

```bash
# Build the project
dotnet build

# Run tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate coverage report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

### Code Quality

```bash
# Format code
csharpier format .

# Check formatting
csharpier check .

# Run all analyzers
dotnet build /p:TreatWarningsAsErrors=true

# Check for outdated packages
dotnet-outdated
```

### Git Hooks

Pre-commit hooks automatically run:
1. Code formatting check
2. Build with all analyzers
3. All unit tests
4. Outdated package check

To bypass hooks in emergencies:
```bash
git commit --no-verify -m "Emergency fix"
```

## Configuration Files

| File | Purpose |
|------|---------|
| `.editorconfig` | Code style rules (shared between IDEs) |
| `Directory.Build.props` | MSBuild configuration and analyzers |
| `.csharpierrc.json` | Code formatter settings |
| `.vscode/settings.json` | VS Code specific settings |
| `.husky/task-runner.json` | Git hook configuration |

## Quality Standards

The project enforces:

- **Zero Warnings** - All warnings treated as errors
- **80% Coverage** - Minimum code coverage threshold
- **Security Scans** - Automated vulnerability detection
- **Format on Save** - Consistent code style
- **Pre-commit Checks** - Quality gates before commits

## Troubleshooting

### .NET Command Not Found

Ensure .NET is in your PATH:
```bash
# macOS/Linux
export PATH="$PATH:/usr/local/share/dotnet"

# Windows
set PATH=%PATH%;C:\Program Files\dotnet
```

### Build Errors After Git Pull

Clean and rebuild:
```bash
dotnet clean
rm -rf obj bin  # or delete obj/ and bin/ folders on Windows
dotnet restore
dotnet build
```

### CSharpier Not Found

Reinstall global tools:
```bash
dotnet tool uninstall -g csharpier
dotnet tool install -g csharpier
```

### Permission Denied on setup.sh

Make the script executable:
```bash
chmod +x setup.sh
```

## Next Steps

Now that your environment is set up:

1. Read the [Architecture Overview](../explanation/architecture.md)
2. Follow the [Getting Started Tutorial](../tutorials/getting-started.md)
3. Check the [Contribution Guidelines](./contributing.md)
4. Explore the [API Reference](../reference/api/index.md)

## Getting Help

- Check existing [GitHub Issues](https://github.com/your-org/airline-tycoon/issues)
- Ask in [Discussions](https://github.com/your-org/airline-tycoon/discussions)
- Review the [FAQ](../reference/faq.md)

---

<small>Last updated: January 2025</small>