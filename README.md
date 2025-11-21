# Airline Tycoon

A C# airline simulation and management game inspired by RollerCoaster Tycoon. Build and operate your own airline empire from a regional carrier to a global aviation powerhouse.

## 🎮 About the Game

**Airline Tycoon** combines route planning, fleet management, and business strategy in a turn-based simulation. Manage your airline through:
- **Route Network** - Open profitable routes between 15+ airports
- **Fleet Management** - Buy or lease aircraft from regional jets to wide-bodies
- **Financial Strategy** - Balance revenue, costs, and cash flow
- **Competition** - Compete against AI airlines for market share
- **Random Events** - Adapt to weather, economic shifts, and market changes
- **Reputation System** - Build your brand to attract more passengers

### Current Status
✅ **Playable MVP** - Core gameplay loop complete with save/load system
🚧 **In Development** - See [ROADMAP.md](ROADMAP.md) for upcoming features

## 📋 Roadmap

See our comprehensive [Product Roadmap](ROADMAP.md) for planned features including:
- AI Competitor Airlines
- Sophisticated Economy Simulator
- Aircraft Maintenance & Crew Management
- International Routes & Global Expansion
- Scenario Mode with challenges
- And much more!

## 🚀 Quick Start

### Playing the Game

```bash
# Clone and setup
git clone <repository-url>
cd Airline_Tycoon
./setup.sh

# Run the game
dotnet run --project src/AirlineTycoon
```

### Development Setup

**Prerequisites:**
- macOS (Intel or Apple Silicon)
- Homebrew package manager
- Admin access (for .NET SDK installation)

**Setup Steps:**
1. Clone this repository
2. Run the setup script:
   ```bash
   ./setup.sh
   ```
3. Open in your preferred IDE:
   - **VS Code**: `code .`
   - **JetBrains Rider**: Open `AirlineTycoon.sln`

## Development Tools

This project uses enterprise-grade C# development tools:

- **Code Formatting**: CSharpier (like Black for Python)
- **Linting**: StyleCop, Roslynator, SonarAnalyzer
- **Security**: Security Code Scan, Puma Scan
- **Testing**: xUnit with FluentAssertions
- **Coverage**: Coverlet with 80% minimum threshold
- **Git Hooks**: Husky.NET for pre-commit checks

## Key Commands

```bash
# Build
dotnet build

# Test
dotnet test

# Format code
dotnet csharpier .

# Check security
dotnet build /p:SecurityCodeScannerAnalyze=true

# Generate coverage report
dotnet test --collect:"XPlat Code Coverage"
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html
```

## IDE Support

Both VS Code and JetBrains Rider are fully supported with shared project settings.

### VS Code
- Install recommended extensions when prompted
- All settings are pre-configured in `.vscode/`
- Format on save is enabled

### JetBrains Rider
- Just open the solution file
- Code style settings are shared via `.editorconfig`
- All analyzers work out of the box

## Project Structure

```
/
├── src/                      # Source code
│   └── AirlineTycoon/       # Main project
├── tests/                    # Test projects
│   └── AirlineTycoon.Tests/ # Unit tests
├── docs/                     # Documentation
├── .vscode/                  # VS Code settings
├── .editorconfig            # Shared code style
├── Directory.Build.props    # MSBuild configuration
└── AirlineTycoon.sln        # Solution file
```

## Contributing

1. All code must pass formatting checks
2. Maintain 80%+ code coverage
3. Fix all analyzer warnings
4. Security scans must pass
5. Write tests for new features

See [CLAUDE.md](CLAUDE.md) for detailed development guidelines.