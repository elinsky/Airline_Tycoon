#!/bin/bash

# Setup script for Airline Tycoon C# development environment
# This script installs all necessary tools for C# development on macOS

set -e

echo "🚀 Setting up Airline Tycoon C# development environment..."
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

# Function to check if a command exists
command_exists() {
    command -v "$1" >/dev/null 2>&1
}

# Function to print status
print_status() {
    if [ $1 -eq 0 ]; then
        echo -e "${GREEN}✓${NC} $2"
    else
        echo -e "${RED}✗${NC} $2"
        return 1
    fi
}

# Check for Homebrew
echo "Checking for Homebrew..."
if ! command_exists brew; then
    echo -e "${YELLOW}Homebrew not found. Please install it from https://brew.sh${NC}"
    exit 1
else
    print_status 0 "Homebrew is installed"
fi

# Install .NET SDK
echo ""
echo "Installing .NET SDK..."
if ! command_exists dotnet; then
    echo "Installing .NET SDK via Homebrew (requires admin password)..."
    brew install --cask dotnet-sdk
    print_status $? ".NET SDK installed"
else
    DOTNET_VERSION=$(dotnet --version)
    print_status 0 ".NET SDK is already installed (version: $DOTNET_VERSION)"
fi

# Create solution and projects
echo ""
echo "Creating solution and project structure..."
if [ ! -f "AirlineTycoon.sln" ]; then
    dotnet new sln -n AirlineTycoon
    print_status $? "Solution created"
    
    # Create main project
    dotnet new classlib -n AirlineTycoon -o src/AirlineTycoon
    dotnet sln add src/AirlineTycoon/AirlineTycoon.csproj
    print_status $? "Main project created"
    
    # Create test project
    dotnet new xunit -n AirlineTycoon.Tests -o tests/AirlineTycoon.Tests
    dotnet sln add tests/AirlineTycoon.Tests/AirlineTycoon.Tests.csproj
    print_status $? "Test project created"
    
    # Add project reference
    dotnet add tests/AirlineTycoon.Tests/AirlineTycoon.Tests.csproj reference src/AirlineTycoon/AirlineTycoon.csproj
    print_status $? "Project reference added"
else
    print_status 0 "Solution already exists"
fi

# Install global tools
echo ""
echo "Installing global .NET tools..."

# CSharpier (code formatter)
if ! command_exists dotnet-csharpier; then
    dotnet tool install -g csharpier
    print_status $? "CSharpier installed"
else
    print_status 0 "CSharpier already installed"
fi

# dotnet-format (built-in formatter)
echo -e "${GREEN}✓${NC} dotnet-format is included with .NET SDK 6+"

# Coverage report generator
if ! command_exists reportgenerator; then
    dotnet tool install -g dotnet-reportgenerator-globaltool
    print_status $? "ReportGenerator installed"
else
    print_status 0 "ReportGenerator already installed"
fi

# Security scan tool
if ! command_exists security-scan; then
    dotnet tool install -g security-scan
    print_status $? "Security Code Scan CLI installed"
else
    print_status 0 "Security Code Scan CLI already installed"
fi

# Outdated packages checker
if ! command_exists dotnet-outdated; then
    dotnet tool install -g dotnet-outdated-tool
    print_status $? "dotnet-outdated installed"
else
    print_status 0 "dotnet-outdated already installed"
fi

# Husky.NET for git hooks
if ! command_exists husky; then
    dotnet tool install -g Husky
    print_status $? "Husky.NET installed"
else
    print_status 0 "Husky.NET already installed"
fi

# Update .NET tools
echo ""
echo "Updating .NET tools..."
dotnet tool list -g | tail -n +3 | awk '{print $1}' | xargs -I {} dotnet tool update -g {} 2>/dev/null || true
print_status 0 "Tools updated"

# Restore packages
echo ""
echo "Restoring NuGet packages..."
dotnet restore
print_status $? "Packages restored"

# Build solution
echo ""
echo "Building solution..."
dotnet build --no-restore
print_status $? "Solution built successfully"

# Run tests
echo ""
echo "Running tests..."
dotnet test --no-build --verbosity quiet
print_status $? "Tests passed"

# Initialize Husky
echo ""
echo "Setting up Git hooks with Husky.NET..."
if [ ! -d ".husky" ]; then
    husky install
    print_status $? "Husky initialized"
    
    # Add pre-commit hook
    husky add .husky/pre-commit "dotnet husky run"
    print_status $? "Pre-commit hook added"
else
    print_status 0 "Husky already initialized"
fi

# Create initial CLAUDE.md file
echo ""
echo "Creating CLAUDE.md file..."
if [ ! -f "CLAUDE.md" ]; then
    cat > CLAUDE.md << 'EOF'
# Airline Tycoon Project

## Project Overview
This is a C# project for building an airline tycoon simulation game.

## Development Environment
- **Language**: C# with .NET 8.0
- **IDE**: VS Code (primary) and JetBrains Rider (supported)
- **Build System**: MSBuild via .NET SDK
- **Test Framework**: xUnit
- **Code Style**: Enforced via .editorconfig

## Code Quality Tools
- **Formatting**: CSharpier (opinionated formatter like Black for Python)
- **Linting**: StyleCop.Analyzers, Roslynator
- **Security**: Security Code Scan, Puma Scan
- **Code Coverage**: Coverlet with 80% threshold
- **Git Hooks**: Husky.NET for pre-commit checks

## Important Commands
```bash
# Format code
dotnet csharpier .
dotnet format

# Run linting
dotnet build /p:TreatWarningsAsErrors=true

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate coverage report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

# Check for outdated packages
dotnet-outdated

# Security scan
dotnet build /p:SecurityCodeScannerAnalyze=true
```

## Project Structure
```
/
├── src/
│   └── AirlineTycoon/        # Main project
├── tests/
│   └── AirlineTycoon.Tests/  # Unit tests
├── docs/                     # Documentation
├── .vscode/                  # VS Code settings
├── .husky/                   # Git hooks
└── TestResults/              # Test coverage reports
```

## Coding Standards
- All warnings are treated as errors
- Code must be formatted with CSharpier before commit
- Minimum 80% code coverage required
- Security scans must pass
- All public APIs must be documented

## Getting Started
1. Run `./setup.sh` to install all dependencies
2. Open in VS Code or JetBrains Rider
3. Run `dotnet build` to verify setup
4. Start coding!

EOF
    print_status $? "CLAUDE.md created"
else
    print_status 0 "CLAUDE.md already exists"
fi

# Final instructions
echo ""
echo -e "${GREEN}✅ Setup complete!${NC}"
echo ""
echo "Next steps:"
echo "1. Open the project in VS Code: ${YELLOW}code .${NC}"
echo "2. Install recommended VS Code extensions when prompted"
echo "3. Start coding! The pre-commit hooks will ensure code quality"
echo ""
echo "Useful commands:"
echo "  ${YELLOW}dotnet build${NC}              - Build the project"
echo "  ${YELLOW}dotnet test${NC}               - Run tests"
echo "  ${YELLOW}dotnet csharpier .${NC}        - Format code"
echo "  ${YELLOW}dotnet-outdated${NC}           - Check for outdated packages"
echo "  ${YELLOW}reportgenerator ...${NC}       - Generate coverage report (see CLAUDE.md)"
echo ""
echo "For JetBrains Rider users:"
echo "  Just open ${YELLOW}AirlineTycoon.sln${NC} in Rider - it will handle everything!"