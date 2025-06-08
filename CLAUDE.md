# Claude AI Assistant Guide for Airline Tycoon

Welcome Claude! This document contains essential context and guidelines
for working on the Airline Tycoon project. Please review this
carefully before making any code changes.

## License and Copyright

This project is proprietary software. Copyright © 2025 Brian Elinsky — All rights reserved.

- Do not distribute or share code without permission
- All contributions become property of the copyright holder
- See LICENSE file for full terms

## Project Overview

**Airline Tycoon** is a simulation and management game inspired by *RollerCoaster Tycoon*, where players build and operate their own airline empire. The game combines route planning, fleet management, airport operations, and business strategy in a stylized, accessible format.

### Vision
Create the most compelling airline simulation game where every gate, route, and jet matters. Players progress from regional carriers to global aviation empires while competing against AI airlines in dynamic markets.

### Target Audience
- Simulation and tycoon game enthusiasts
- Aviation enthusiasts
- Players who enjoy *RollerCoaster Tycoon*, *Transport Tycoon*, *Cities: Skylines*

## Development Workflow

### Pre-commit Hooks
IMPORTANT: Git hooks run automatically via Husky.NET:
```bash
# Hooks will run on commit, but to test manually:
dotnet husky run
```

The hooks include:
- CSharpier (code formatting - modifies files!)
- dotnet build (with all analyzers)
- dotnet test (all unit tests)
- dotnet-outdated (dependency checking)

Note: CSharpier will modify files, so you may need to stage changes
and commit again if formatting was applied.

### Development Environment
```bash
# Ensure .NET SDK 9.0 is installed
dotnet --version

# Clone and setup
git clone <repository-url>
cd airline-tycoon
./setup.sh  # Installs tools and configures environment

# Add tools to PATH
export PATH="$PATH:$HOME/.dotnet/tools"
```

## Architecture and Design Guidelines

- Prefer composition over inheritance
- Analyze all design work using SOLID principles as a checklist
- When designing new features, always create multiple alternatives
- Iterate on designs multiple times - never stop at the first idea
- Play devil's advocate when evaluating ideas to find the best solution
- Use Domain-Driven Design (DDD) patterns

## Code Style Guidelines

### General Principles

- Write production-quality code suitable for Steam release
- Each class should represent a real-world airline/aviation concept
- Design interfaces first, then write tests, then implementation
- Use C#'s strong type system extensively
- Maximum line length: 120 characters (enforced by .editorconfig)

### C# Conventions

- Follow .editorconfig settings (auto-enforced)
- Use `this.` prefix for instance members
- PascalCase for public members, camelCase for private fields
- Comprehensive XML documentation on all public APIs
- Leverage modern C# features (records, pattern matching, etc.)

### Code Organization

```
src/
├── AirlineTycoon/
│   ├── Domain/          # Core business logic
│   ├── Services/        # Application services
│   ├── Infrastructure/  # External concerns
│   └── UI/             # User interface
```

## Documentation Standards

### XML Documentation Requirements

ALL public classes, methods, and properties MUST have comprehensive
XML documentation with these sections:

1. **Summary** - Brief description
2. **Remarks** - Extended explanation, algorithms, business rules
3. **Parameters** - Each parameter with type and purpose
4. **Returns** - Return type and meaning
5. **Exceptions** - What exceptions can be thrown and why
6. **Example** - Usage examples

### Example Documentation
```csharp
/// <summary>
/// Calculates the profitability of a flight route based on demand and costs.
/// </summary>
/// <remarks>
/// Uses a dynamic pricing model that considers:
/// - Base ticket price
/// - Seasonal demand multiplier
/// - Competition factor
/// - Fuel costs
/// - Airport fees
/// 
/// The formula for route profitability is:
/// <code>
/// Profit = (TicketPrice * LoadFactor * Capacity) - (FuelCost + CrewCost + AirportFees)
/// </code>
/// </remarks>
/// <param name="route">The flight route to analyze</param>
/// <param name="aircraft">The aircraft assigned to this route</param>
/// <param name="season">Current season affecting demand</param>
/// <returns>The calculated profit margin as a decimal (0.15 = 15% margin)</returns>
/// <exception cref="ArgumentNullException">Thrown when route or aircraft is null</exception>
/// <example>
/// <code>
/// var route = new FlightRoute("JFK", "LAX");
/// var aircraft = new Aircraft(AircraftType.Boeing737);
/// var profitability = calculator.CalculateRouteProfitability(route, aircraft, Season.Summer);
/// Console.WriteLine($"Profit margin: {profitability:P}");
/// </code>
/// </example>
public decimal CalculateRouteProfitability(FlightRoute route, Aircraft aircraft, Season season)
{
    // Implementation
}
```

## Testing Guidelines

### Test Structure

- Use xUnit with FluentAssertions
- Follow Arrange-Act-Assert pattern with detailed comments
- Each section must explain business context
- Write domain-specific scenarios
- Aim for 80% code coverage minimum

### Example Test
```csharp
[Fact]
public void FlightScheduler_AssignAircraft_ShouldOptimizeForProfitability()
{
    // Arrange - Setting up a typical airline scheduling scenario
    // The airline has 3 Boeing 737s and needs to assign them to routes
    // for maximum profitability during peak summer season.
    var scheduler = new FlightScheduler();
    var fleet = new List<Aircraft>
    {
        new Aircraft("B737-1", AircraftType.Boeing737),
        new Aircraft("B737-2", AircraftType.Boeing737),
        new Aircraft("B737-3", AircraftType.Boeing737)
    };
    var routes = new List<FlightRoute>
    {
        new FlightRoute("JFK", "LAX", distance: 2475), // Long haul
        new FlightRoute("JFK", "BOS", distance: 187),  // Short haul
        new FlightRoute("JFK", "MIA", distance: 1089)  // Medium haul
    };

    // Act - The scheduler assigns aircraft to maximize profit
    // It should assign aircraft to longer routes first as they
    // typically have higher profit margins
    var assignments = scheduler.AssignAircraftToRoutes(fleet, routes, Season.Summer);

    // Assert - Verify optimal assignment
    // The longest route (JFK-LAX) should be assigned first
    // as it has the highest profit potential
    assignments.Should().HaveCount(3);
    assignments.First().Route.Distance.Should().Be(2475);
    assignments.First().Aircraft.Should().NotBeNull();
    assignments.All(a => a.EstimatedProfit > 0).Should().BeTrue();
}
```

## Architecture & Design Patterns

### Key Documentation

- `/docs/vision.md` - Game design and vision
- `/docs/technical/architecture.md` - System architecture
- `/docs/how-to/` - Development guides

### Core Components

1. **GameEngine** - Main game loop and state management
2. **FlightScheduler** - Route planning and optimization
3. **DemandModel** - Dynamic demand simulation
4. **SeasonalSystem** - Seasonal variation management
5. **EventSystem** - Random events (weather, strikes, etc.)
6. **AirlineAI** - Competitor airline behavior
7. **FleetManager** - Aircraft purchase/lease/maintenance
8. **AirportNetwork** - Airport relationships and slots

### Design Principles

- Each component has single responsibility
- Use dependency injection (Microsoft.Extensions.DI)
- Prefer immutable data models where possible
- Event-driven architecture for game events
- Clear separation between game logic and UI

## Common Commands

### Building and Testing
```bash
# Build the project
dotnet build

# Run all tests
dotnet test

# Run tests with coverage
dotnet test --collect:"XPlat Code Coverage"

# Generate coverage report
reportgenerator -reports:"**/coverage.cobertura.xml" -targetdir:"coveragereport" -reporttypes:Html

# Run the game
dotnet run --project src/AirlineTycoon
```

### Code Quality
```bash
# Format code
csharpier format .

# Check formatting
csharpier check .

# Run with all analyzers
dotnet build /p:TreatWarningsAsErrors=true

# Security scan
dotnet build /p:SecurityCodeScannerAnalyze=true

# Check outdated packages
dotnet-outdated
```

## Game-Specific Context

### Core Game Loop

- Daily turns with multiple phases
- Morning: Review reports, adjust schedules
- Afternoon: Handle events, make decisions  
- Evening: Financial updates, competitor moves

### Seasonal Demand System

- **Low Season**: 60-70% base demand
- **Shoulder Season**: 80-90% base demand  
- **Peak Season**: 110-130% base demand
- **Holiday Peaks**: Up to 150% demand

### Airline Types

- **Budget Carrier**: Low costs, high volume, limited routes
- **Premium Airline**: High service, premium pricing, global reach
- **Regional Carrier**: Short routes, small aircraft, feed major hubs
- **Cargo Airline**: Freight focus, different demand patterns

### Event System

Event types and impacts:
- **Weather Events** (30%): Cancel flights, reduce demand
- **Economic Events** (25%): Affect overall demand levels  
- **Competitor Actions** (25%): New routes, price wars
- **Regulatory Changes** (20%): New costs or opportunities

### Key Game Concepts

- **Load Factor**: Percentage of seats filled (target: 80%+)
- **Hub-and-Spoke**: Centralized routing through major airports
- **Point-to-Point**: Direct routes between cities
- **Slot Restrictions**: Limited takeoff/landing slots at busy airports
- **Fleet Commonality**: Cost savings from similar aircraft types
- **Route Profitability**: Revenue minus all operational costs

### Calculation Examples

Route profitability factors:
```csharp
// Base calculation
var revenue = ticketPrice * passengers;
var fuelCost = distance * fuelPerMile * fuelPrice;
var crewCost = flightHours * crewHourlyRate;
var airportFees = departureFeee + landingFee;
var profit = revenue - (fuelCost + crewCost + airportFees + maintenance);
```

## Important Reminders

1. ALWAYS let Git hooks run before committing
2. Write comprehensive XML documentation for ALL public APIs
3. Include business context in test descriptions
4. Keep methods focused and under 50 lines
5. Design interfaces before implementation
6. Each class represents a real airline concept
7. Use FluentAssertions for readable tests
8. Security scans must pass before merge

## Troubleshooting

### Build Issues
- Run `dotnet clean` then rebuild
- Check for analyzer warnings in Error List
- Ensure all NuGet packages restored

### Test Failures  
- Check test output for detailed assertions
- Run single test in debug mode
- Verify test data setup is complete

### Performance Issues
- Use BenchmarkDotNet for performance testing
- Profile with dotMemory or PerfView
- Check for LINQ performance anti-patterns