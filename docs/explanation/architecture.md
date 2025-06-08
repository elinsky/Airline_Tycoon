# Airline Tycoon Architecture

This document describes the technical architecture of Airline Tycoon, a Unity-based airline simulation game. The architecture follows SOLID principles, Domain-Driven Design patterns, and Unity best practices to create a maintainable, testable, and extensible codebase.

## Overview

Airline Tycoon uses a layered architecture that separates Unity-specific concerns from core business logic. This separation enables:

- **Testability**: Domain logic can be tested without Unity
- **Modularity**: Components can be developed and modified independently
- **Performance**: Optimized for Unity's rendering and update systems
- **Maintainability**: Clear boundaries between layers

### Core Principles

1. **SOLID Compliance**: Every component follows SOLID principles
2. **Composition over Inheritance**: Prefer component composition except for Unity MonoBehaviours
3. **Domain Isolation**: Business logic is independent of Unity
4. **Event-Driven**: Loose coupling through events
5. **Data-Driven**: Configuration through ScriptableObjects

## System Architecture

```mermaid
graph TB
    subgraph "Unity Presentation Layer"
        UICanvas[UI Canvas System]
        MapCamera[Map Camera Controller]
        UIControllers[UI Controllers/MonoBehaviours]
        Prefabs[UI Prefabs]
        Effects[Visual Effects]
    end

    subgraph "Unity Integration Layer"
        GameManager[GameManager : MonoBehaviour]
        TimeManager[TimeManager : MonoBehaviour]
        InputManager[InputManager : MonoBehaviour]
        SaveManager[SaveManager : MonoBehaviour]
        AudioManager[AudioManager : MonoBehaviour]
    end

    subgraph "Application Layer (Pure C#)"
        GameEngine[Game Engine]
        CommandBus[Command Bus]
        QueryBus[Query Bus]
        EventBus[Event Dispatcher]
        Validators[Command Validators]
    end

    subgraph "Domain Layer (Pure C#)"
        subgraph "Core Entities"
            Airline[Airline]
            Aircraft[Aircraft]
            Route[Route]
            Flight[Flight]
            Airport[Airport]
        end
        
        subgraph "Economic Simulator"
            DemandCalc[Demand Calculator]
            PricingEngine[Pricing Engine]
            MarketSim[Market Simulator]
            ProfitCalc[Profitability Calculator]
        end
        
        subgraph "Domain Services"
            Scheduling[Flight Scheduling]
            Optimization[Route Optimization]
            Maintenance[Maintenance Service]
        end
    end

    subgraph "Unity Data Layer"
        ScriptableObjects[ScriptableObject Configs]
        Addressables[Addressable Assets]
        Serialization[Unity Serialization]
        CloudSave[Cloud Save Service]
    end

    UIControllers --> GameManager
    GameManager --> GameEngine
    GameEngine --> CommandBus
    GameEngine --> QueryBus
    CommandBus --> Validators
    Validators --> Domain Services
    TimeManager --> GameEngine
    SaveManager --> Serialization
    EventBus -.-> Unity Integration Layer
    ScriptableObjects --> Domain Layer
```

## Unity Component Architecture

The Unity-specific components handle presentation, input, and engine integration while delegating business logic to the domain layer.

```mermaid
classDiagram
    class MonoBehaviour {
        <<Unity>>
        +Start() void
        +Update() void
        +OnDestroy() void
    }

    class GameManager {
        -gameEngine: IGameEngine
        -timeManager: TimeManager
        -uiManager: UIManager
        -saveManager: SaveManager
        +CurrentGameState: GameState
        +CurrentAirline: Airline
        +Start() void
        +Update() void
        +OnApplicationPause(bool) void
        +OnApplicationFocus(bool) void
    }

    class TimeManager {
        +TimeScale: float
        +IsPaused: bool
        +CurrentDate: DateTime
        +GameSpeed: GameSpeed
        +OnDayChanged: UnityEvent~int~
        +OnHourChanged: UnityEvent~int~
        +OnMonthChanged: UnityEvent~int~
        +Update() void
        +SetSpeed(speed: GameSpeed) void
        +Pause() void
        +Resume() void
    }

    class UIManager {
        -routeMapView: RouteMapView
        -dashboardView: DashboardView
        -aircraftListView: AircraftListView
        -routePlannerView: RoutePlannerView
        +ShowRouteDetails(route: Route) void
        +UpdateFinancials(data: FinancialData) void
        +ShowNotification(message: string) void
        +OpenModal(modalType: ModalType) void
    }

    class InputManager {
        -camera: Camera
        -raycastResults: List~RaycastResult~
        +OnAirportClicked: UnityEvent~Airport~
        +OnRouteClicked: UnityEvent~Route~
        +Update() void
        -HandleMouseInput() void
        -HandleKeyboardShortcuts() void
    }

    class SaveManager {
        -saveSystem: ISaveSystem
        +AutoSaveInterval: float
        +SaveSlots: int
        +SaveGame(slot: int) void
        +LoadGame(slot: int) void
        +QuickSave() void
        +GetSaveInfo(slot: int) SaveInfo
    }

    MonoBehaviour <|-- GameManager
    MonoBehaviour <|-- TimeManager
    MonoBehaviour <|-- UIManager
    MonoBehaviour <|-- InputManager
    MonoBehaviour <|-- SaveManager
    GameManager --> TimeManager
    GameManager --> UIManager
    GameManager --> InputManager
    GameManager --> SaveManager
```

## Economic Simulator Design

The economic simulator is the heart of the game's business simulation. It's designed as a modular system that can be extended and tested independently of Unity.

```mermaid
classDiagram
    class IEconomicSimulator {
        <<interface>>
        +SimulateDay(date: DateTime) MarketState
        +CalculateRouteDemand(route: Route) PassengerDemand
        +OptimizePricing(route: Route) Money
        +PredictMarketTrends() MarketForecast
    }

    class IDemandCalculator {
        <<interface>>
        +CalculateDemand(route: Route, date: DateTime) PassengerDemand
    }

    class IPricingStrategy {
        <<interface>>
        +CalculateOptimalPrice(route: Route, demand: PassengerDemand) Money
        +GetCompetitorPricing(route: Route) CompetitorPricing
    }

    class IMarketSimulator {
        <<interface>>
        +SimulateMarket(airlines: List~Airline~) MarketConditions
        +CalculateMarketShare(route: Route) MarketShare
        +GenerateMarketEvents() List~MarketEvent~
    }

    class IProfitabilityCalculator {
        <<interface>>
        +CalculateProfitability(flight: Flight) FlightProfitability
        +CalculateRouteProfitability(route: Route) RouteProfitability
        +ProjectProfitability(route: Route, days: int) ProfitProjection
    }

    class CompositeDemandCalculator {
        -baseDemand: IBaseDemandCalculator
        -modifiers: List~IDemandModifier~
        +CalculateDemand(route: Route, date: DateTime) PassengerDemand
    }

    class IDemandModifier {
        <<interface>>
        +Modify(demand: PassengerDemand, context: DemandContext) PassengerDemand
        +GetImpactFactor() float
    }

    class SeasonalDemandModifier {
        -seasonalData: SeasonalDataSO
        +Modify(demand: PassengerDemand, context: DemandContext) PassengerDemand
        +GetSeasonMultiplier(date: DateTime) float
    }

    class EventDemandModifier {
        -activeEvents: List~EconomicEvent~
        +Modify(demand: PassengerDemand, context: DemandContext) PassengerDemand
        +AddEvent(event: EconomicEvent) void
    }

    class CompetitionDemandModifier {
        -marketAnalyzer: IMarketAnalyzer
        +Modify(demand: PassengerDemand, context: DemandContext) PassengerDemand
        +AnalyzeCompetition(route: Route) CompetitionLevel
    }

    class DynamicPricingStrategy {
        -demandElasticity: float
        -competitorAnalyzer: ICompetitorAnalyzer
        +CalculateOptimalPrice(route: Route, demand: PassengerDemand) Money
        -CalculateElasticityAdjustment(baseDemand: int) float
    }

    IEconomicSimulator --> IDemandCalculator
    IEconomicSimulator --> IPricingStrategy
    IEconomicSimulator --> IMarketSimulator
    IEconomicSimulator --> IProfitabilityCalculator
    IDemandCalculator <|.. CompositeDemandCalculator
    CompositeDemandCalculator --> IDemandModifier
    IDemandModifier <|.. SeasonalDemandModifier
    IDemandModifier <|.. EventDemandModifier
    IDemandModifier <|.. CompetitionDemandModifier
    IPricingStrategy <|.. DynamicPricingStrategy
```

## Domain Model

The domain model represents the core business concepts of an airline. These are pure C# classes that don't depend on Unity.

```mermaid
classDiagram
    class Airline {
        +Id: AirlineId
        +Name: string
        +Type: AirlineType
        +Cash: Money
        +Reputation: float
        +Hub: Airport
        +Fleet: IReadOnlyList~Aircraft~
        +Routes: IReadOnlyList~Route~
        +Employees: StaffRoster
        +PurchaseAircraft(model: AircraftModel, financing: FinancingType) PurchaseResult
        +LeaseAircraft(model: AircraftModel, term: LeaseTerm) LeaseResult
        +OpenRoute(origin: Airport, destination: Airport) RouteResult
        +SetTicketPrice(route: Route, price: Money) void
        +ScheduleFlight(route: Route, aircraft: Aircraft, time: TimeSlot) Flight
    }

    class Aircraft {
        +Id: AircraftId
        +Model: AircraftModel
        +Registration: string
        +Configuration: SeatConfiguration
        +Range: Distance
        +FuelEfficiency: FuelConsumption
        +MaintenanceState: MaintenanceStatus
        +Age: TimeSpan
        +FlightHours: float
        +CurrentLocation: Airport
        +IsAvailable() bool
        +RequiresMaintenance() bool
        +CalculateOperatingCost(distance: Distance) Money
    }

    class Route {
        +Id: RouteId
        +Origin: Airport
        +Destination: Airport
        +Distance: Distance
        +FlightTime: TimeSpan
        +Status: RouteStatus
        +Frequency: FlightFrequency
        +Flights: IReadOnlyList~Flight~
        +Demand: PassengerDemand
        +Competition: List~CompetitorRoute~
        +AddFlight(flight: Flight) void
        +RemoveFlight(flightId: FlightId) void
        +CalculateProfitability() RouteProfitability
        +GetLoadFactor() float
    }

    class Flight {
        +Id: FlightId
        +FlightNumber: string
        +Route: Route
        +Aircraft: Aircraft
        +ScheduledDeparture: DateTime
        +ActualDeparture: DateTime?
        +Status: FlightStatus
        +Passengers: PassengerManifest
        +Pricing: TicketPricing
        +CalculateRevenue() Money
        +CalculateCosts() Money
        +GetLoadFactor() float
    }

    class Airport {
        +Code: AirportCode
        +Name: string
        +City: string
        +Country: string
        +Location: Coordinates
        +Size: AirportSize
        +RunwayCount: int
        +TerminalCount: int
        +SlotCapacity: SlotCapacity
        +PassengerVolume: int
        +HasAvailableSlot(time: TimeSlot) bool
        +GetLandingFee(aircraftType: AircraftType) Money
        +GetParkingFee(duration: TimeSpan) Money
    }

    class PassengerDemand {
        +Business: int
        +Economy: int
        +Cargo: float
        +Total: int
        +PriceElasticity: float
        +SeasonalFactor: float
        +CalculateAtPrice(price: Money) int
    }

    Airline "1" --> "*" Aircraft : owns/leases
    Airline "1" --> "*" Route : operates
    Airline "*" --> "1" Airport : hub
    Route "1" --> "*" Flight : schedules
    Route "*" --> "2" Airport : connects
    Flight "*" --> "1" Aircraft : uses
    Flight --> PassengerDemand : satisfies
```

## Unity-Domain Bridge Pattern

This pattern allows Unity components to interact with the domain layer without coupling them directly.

```mermaid
classDiagram
    class IGameEngine {
        <<interface>>
        +Initialize(saveData: SaveData?) void
        +ProcessDay() void
        +ProcessHour() void
        +ExecuteCommand(command: ICommand) CommandResult
        +Query~T~(query: IQuery~T~) T
        +Subscribe(handler: IEventHandler) void
    }

    class UnityGameAdapter {
        -domainEngine: DomainGameEngine
        -eventBridge: UnityEventBridge
        -commandValidator: ICommandValidator
        +Initialize(saveData: SaveData?) void
        +ProcessDay() void
        +ExecuteCommand(command: ICommand) CommandResult
        -ValidateCommand(command: ICommand) ValidationResult
    }

    class DomainGameEngine {
        <<Pure C#>>
        -economicSimulator: IEconomicSimulator
        -airlineService: IAirlineService
        -routeService: IRouteService
        -eventDispatcher: IEventDispatcher
        +ProcessDay() void
        +ExecuteCommand(command: ICommand) CommandResult
    }

    class UnityEventBridge {
        +OnRouteOpened: UnityEvent~RouteOpenedData~
        +OnAircraftPurchased: UnityEvent~AircraftData~
        +OnDemandChanged: UnityEvent~DemandChangeData~
        +OnMoneyChanged: UnityEvent~float~
        +OnEventOccurred: UnityEvent~GameEvent~
        +BridgeDomainEvent(event: IDomainEvent) void
    }

    class RouteMapView {
        <<MonoBehaviour>>
        -routeLinePrefab: GameObject
        -airportIconPrefab: GameObject
        -routeLinePool: ObjectPool~RouteLine~
        +DrawRoute(route: Route) void
        +UpdateRouteColor(route: Route, profitability: float) void
        +AnimateFlights(flights: List~Flight~) void
        -GetRouteColor(profitability: float) Color
    }

    class ICommand {
        <<interface>>
        +Validate() ValidationResult
    }

    class OpenRouteCommand {
        +OriginAirportCode: string
        +DestinationAirportCode: string
        +InitialFrequency: int
        +Validate() ValidationResult
    }

    IGameEngine <|.. UnityGameAdapter
    UnityGameAdapter --> DomainGameEngine
    UnityGameAdapter --> UnityEventBridge
    UnityEventBridge --> RouteMapView : Unity Events
    ICommand <|.. OpenRouteCommand
```

## ScriptableObject Data Architecture

ScriptableObjects provide data-driven configuration that designers can modify without touching code.

```mermaid
classDiagram
    class ScriptableObject {
        <<Unity>>
    }

    class AircraftModelSO {
        +modelName: string
        +manufacturer: string
        +aircraftType: AircraftType
        +passengerCapacity: int
        +cargoCapacity: float
        +range: float
        +cruiseSpeed: float
        +fuelCapacity: float
        +fuelConsumption: AnimationCurve
        +purchasePrice: float
        +monthlyLeaseCost: float
        +maintenanceCostPerHour: float
        +aircraftSprite: Sprite
        +aircraftModel3D: GameObject
    }

    class AirportDataSO {
        +airportCode: string
        +airportName: string
        +cityName: string
        +countryName: string
        +position: Vector2
        +size: AirportSize
        +passengerVolume: int
        +cargoVolume: float
        +runwayCount: int
        +terminalCount: int
        +landingFee: float
        +parkingFeePerHour: float
        +demandMultiplier: float
        +airportIcon: Sprite
    }

    class SeasonalDataSO {
        +seasonName: string
        +demandCurve: AnimationCurve
        +businessTravelMultiplier: float
        +leisureTravelMultiplier: float
        +cargoMultiplier: float
        +weatherEventProbability: float
        +fuelPriceModifier: float
    }

    class GameEventSO {
        +eventName: string
        +eventDescription: string
        +eventIcon: Sprite
        +eventType: EventType
        +probability: float
        +duration: int
        +demandImpact: float
        +costImpact: float
        +reputationImpact: float
        +affectedRoutes: RouteFilter
        +newsHeadline: string
    }

    class GameConfigSO {
        +startingCash: float
        +difficultySettings: DifficultySettings
        +economicSettings: EconomicSettings
        +unlockProgression: List~UnlockMilestone~
        +tutorialSettings: TutorialSettings
    }

    ScriptableObject <|-- AircraftModelSO
    ScriptableObject <|-- AirportDataSO
    ScriptableObject <|-- SeasonalDataSO
    ScriptableObject <|-- GameEventSO
    ScriptableObject <|-- GameConfigSO
```

## Event Flow Architecture

The event system enables loose coupling between components while maintaining clear data flow.

```mermaid
sequenceDiagram
    participant UnityUI
    participant GameManager
    participant UnityAdapter
    participant CommandBus
    participant DomainService
    participant EconomicSim
    participant EventBus
    participant UnityBridge
    participant UIComponents

    UnityUI->>GameManager: Player clicks "Open Route"
    GameManager->>UnityAdapter: Create OpenRouteCommand
    UnityAdapter->>CommandBus: Execute Command
    CommandBus->>DomainService: Process Command
    DomainService->>EconomicSim: Calculate Initial Demand
    EconomicSim-->>DomainService: Return Demand Data
    DomainService->>EventBus: Publish RouteOpenedEvent
    EventBus-->>UnityBridge: Notify Event
    UnityBridge-->>UIComponents: Unity Event
    UIComponents-->>UnityUI: Update UI
    DomainService-->>CommandBus: Return Result
    CommandBus-->>UnityAdapter: Command Result
    UnityAdapter-->>GameManager: Success/Failure
    GameManager-->>UnityUI: Show Result
```

## Game Loop Integration

The game loop integrates Unity's update cycle with the domain's time-based simulation.

```mermaid
stateDiagram-v2
    [*] --> UnityAwake
    UnityAwake --> UnityStart: Awake()
    UnityStart --> LoadConfiguration: Start()
    LoadConfiguration --> InitializeDomain
    InitializeDomain --> GameRunning
    
    GameRunning --> UnityUpdate: Every Frame
    
    state UnityUpdate {
        [*] --> CheckInput
        CheckInput --> ProcessInput: Has Input
        CheckInput --> CheckTime: No Input
        ProcessInput --> CheckTime
        CheckTime --> AdvanceTime: Not Paused
        CheckTime --> [*]: Paused
        AdvanceTime --> CheckHourPassed
        CheckHourPassed --> ProcessHour: Hour Passed
        CheckHourPassed --> [*]: Same Hour
        ProcessHour --> CheckDayPassed
        CheckDayPassed --> ProcessDay: Day Passed
        CheckDayPassed --> [*]: Same Day
        ProcessDay --> [*]
    }
    
    state ProcessDay {
        [*] --> Morning
        Morning --> CalculateDemand
        CalculateDemand --> ScheduleFlights
        ScheduleFlights --> Afternoon
        Afternoon --> ProcessEvents
        ProcessEvents --> HandleCompetitors
        HandleCompetitors --> Evening
        Evening --> CalculateFinancials
        CalculateFinancials --> UpdateStats
        UpdateStats --> [*]
    }
    
    GameRunning --> Paused: Pause Event
    Paused --> GameRunning: Resume Event
    GameRunning --> SaveGame: Auto-Save Timer
    SaveGame --> GameRunning
    GameRunning --> [*]: Application Quit
```

## Performance Optimization Strategies

### Object Pooling
```csharp
public class RouteLinePool : MonoBehaviour
{
    private Queue<RouteLine> pool = new Queue<RouteLine>();
    
    public RouteLine Get()
    {
        if (pool.Count > 0)
            return pool.Dequeue();
        return Instantiate(routeLinePrefab);
    }
    
    public void Return(RouteLine line)
    {
        line.gameObject.SetActive(false);
        pool.Enqueue(line);
    }
}
```

### LOD System for Map
- **Close**: Full route details, animated planes
- **Medium**: Simple lines, static plane icons  
- **Far**: Just route lines, no planes

### Update Batching
- Economic calculations run once per game day
- UI updates batched every 0.1 seconds
- Visual effects use Unity Job System

## Testing Strategy

### Domain Layer Testing (NUnit)
```csharp
[Test]
public void Route_CalculateProfitability_ConsidersAllCosts()
{
    // Arrange
    var route = new Route(jfk, lax);
    var flight = new Flight(route, boeing737, morningSlot);
    
    // Act
    var profitability = route.CalculateProfitability();
    
    // Assert
    profitability.Revenue.Should().BeGreaterThan(Money.Zero);
    profitability.Costs.Should().Include(c => c.Type == CostType.Fuel);
}
```

### Unity Integration Testing
```csharp
[UnityTest]
public IEnumerator GameManager_ProcessDay_UpdatesUI()
{
    // Arrange
    var gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    var initialDate = gameManager.CurrentDate;
    
    // Act
    gameManager.ProcessDay();
    yield return new WaitForSeconds(0.1f);
    
    // Assert
    Assert.AreNotEqual(initialDate, gameManager.CurrentDate);
}
```

## Extension Points

The architecture provides clear extension points for new features:

1. **New Aircraft Types**: Add AircraftModelSO
2. **New Events**: Create GameEventSO  
3. **New Pricing Strategies**: Implement IPricingStrategy
4. **New Demand Modifiers**: Implement IDemandModifier
5. **New UI Views**: Create MonoBehaviour, subscribe to events

## SOLID Principles Applied

### Single Responsibility
- `DemandCalculator`: Only calculates demand
- `RouteMapView`: Only handles route visualization
- `SaveManager`: Only handles game persistence

### Open/Closed
- New features extend interfaces without modifying existing code
- ScriptableObjects allow data changes without code changes

### Liskov Substitution
- All strategy implementations are interchangeable
- Mock implementations for testing

### Interface Segregation
- Small, focused interfaces (e.g., `IDemandCalculator`, `IPricingStrategy`)
- Unity components only depend on needed interfaces

### Dependency Inversion
- Domain layer has no Unity dependencies
- All dependencies flow inward toward domain

## Conclusion

This architecture provides a solid foundation for Airline Tycoon that:
- Separates Unity concerns from business logic
- Enables comprehensive testing
- Supports iterative development
- Optimizes for Unity performance
- Allows designer-friendly configuration

The modular design ensures that new features can be added without disrupting existing functionality, while the event-driven architecture keeps components loosely coupled and maintainable.