using AirlineTycoon.Domain;
using AirlineTycoon.Domain.Events;
using AirlineTycoon.Services;

namespace AirlineTycoon;

/// <summary>
/// Main game engine for Airline Tycoon.
/// Orchestrates the game session and manages the player's airline.
/// Similar to RollerCoaster Tycoon's game manager, this handles the core game loop.
/// </summary>
/// <remarks>
/// The Game class is responsible for:
/// - Creating and managing the player's airline
/// - Processing game turns (days)
/// - Tracking win/lose conditions
/// - Managing game state (running, paused, game over)
/// - Generating random events to create dynamic gameplay
///
/// Like RCT, the game runs in discrete time steps (days) where all operations
/// are processed and results are calculated.
/// </remarks>
public class Game
{
    private bool isRunning;
    private readonly EventGenerator eventGenerator;

    /// <summary>
    /// Gets the name of the game.
    /// </summary>
    public static string Name => "Airline Tycoon";

    /// <summary>
    /// Gets the current version of the game.
    /// </summary>
    public static string Version => "1.0.0";

    /// <summary>
    /// Gets a value indicating whether the game is currently running.
    /// </summary>
    public bool IsRunning => this.isRunning;

    /// <summary>
    /// Gets the player's airline.
    /// This is the core entity the player manages, like the park in RCT.
    /// </summary>
    public Airline? PlayerAirline { get; private set; }

    /// <summary>
    /// Gets the current scenario being played (null for free play mode).
    /// </summary>
    public Scenario? CurrentScenario { get; private set; }

    /// <summary>
    /// Gets whether the current game has been won (scenario objectives met).
    /// </summary>
    public bool HasWon { get; private set; }

    /// <summary>
    /// Gets whether the current game has been lost (bankruptcy or scenario failure).
    /// </summary>
    public bool HasLost => this.PlayerAirline?.IsBankrupt ?? false;

    /// <summary>
    /// Initializes a new instance of the <see cref="Game"/> class.
    /// </summary>
    public Game()
    {
        this.eventGenerator = new EventGenerator();
    }

    /// <summary>
    /// Starts a new game session with a starter airline.
    /// Creates a basic airline similar to RCT's starting park configuration.
    /// </summary>
    /// <param name="airlineName">The name for the player's airline.</param>
    /// <param name="homeHub">The IATA code of the home hub airport.</param>
    public void StartNewGame(string airlineName = "SkyWings Airlines", string homeHub = "JFK")
    {
        this.isRunning = true;
        this.HasWon = false;

        // Create the player's airline with starting conditions
        this.PlayerAirline = new Airline
        {
            Name = airlineName,
            HomeHub = homeHub,
            Cash = 5_000_000m, // Start with $5M like RCT's starting budget
            Reputation = 50 // Start at average reputation
        };

        // Give the player 2 starter aircraft (leased to preserve cash)
        var b737 = AircraftType.Catalog.Boeing737;
        var a320 = AircraftType.Catalog.AirbusA320;

        this.PlayerAirline.LeaseAircraft(b737);
        this.PlayerAirline.LeaseAircraft(a320);

        // Open 2 starter routes from home hub
        var homeAirport = Airport.Catalog.FindByCode(homeHub)!;
        var lax = Airport.Catalog.FindByCode("LAX")!;
        var mia = Airport.Catalog.FindByCode("MIA")!;

        var route1 = this.PlayerAirline.OpenRoute(homeAirport, lax, 250m);
        var route2 = this.PlayerAirline.OpenRoute(homeAirport, mia, 180m);

        // Assign aircraft to starter routes
        route1.AssignAircraft(this.PlayerAirline.Fleet[0]);
        route2.AssignAircraft(this.PlayerAirline.Fleet[1]);
    }

    /// <summary>
    /// Starts a new game with a specific scenario (objectives and constraints).
    /// Like RCT's scenario mode, this provides structured challenges.
    /// </summary>
    /// <param name="scenario">The scenario to play.</param>
    public void StartScenario(Scenario scenario)
    {
        this.StartNewGame(scenario.AirlineName, scenario.StartingHub);
        this.CurrentScenario = scenario;

        // Apply scenario-specific modifications
        this.PlayerAirline!.Cash = scenario.StartingCash;
    }

    /// <summary>
    /// Advances the game by one day, processing all airline operations.
    /// This is the core game loop iteration, like RCT's daily update.
    /// </summary>
    /// <returns>The daily operations summary showing what happened.</returns>
    /// <exception cref="InvalidOperationException">Thrown if game is not running or no airline exists.</exception>
    public DailyOperationsSummary ProcessDay()
    {
        if (!this.isRunning)
        {
            throw new InvalidOperationException("Cannot process day when game is not running.");
        }

        if (this.PlayerAirline == null)
        {
            throw new InvalidOperationException("Cannot process day without a player airline.");
        }

        // Try to generate a random event (12% chance per day)
        var newEvent = this.eventGenerator.TryGenerateEvent(
            this.PlayerAirline.CurrentDay + 1, // Next day since ProcessDay increments it
            this.PlayerAirline
        );

        // If an event was generated, add it to active events
        if (newEvent != null)
        {
            this.PlayerAirline.ActiveEvents.Add(newEvent);
        }

        // Process the day's operations
        var summary = this.PlayerAirline.ProcessDay();

        // Add new event to summary so UI can display it
        if (newEvent != null)
        {
            summary.NewEvents.Add(newEvent);
        }

        // Check win/lose conditions
        this.CheckWinConditions();
        this.CheckLoseConditions();

        return summary;
    }

    /// <summary>
    /// Checks if scenario objectives have been met (win condition).
    /// </summary>
    private void CheckWinConditions()
    {
        if (this.CurrentScenario == null || this.PlayerAirline == null)
        {
            return;
        }

        bool allObjectivesMet = this.CurrentScenario.CheckObjectives(this.PlayerAirline);

        if (allObjectivesMet && !this.HasWon)
        {
            this.HasWon = true;
        }
    }

    /// <summary>
    /// Checks if the player has lost (bankruptcy).
    /// </summary>
    private void CheckLoseConditions()
    {
        if (this.HasLost)
        {
            this.Stop();
        }
    }

    /// <summary>
    /// Loads a game from saved state.
    /// Restores the airline, scenario, and win status.
    /// </summary>
    /// <param name="airline">The saved airline state.</param>
    /// <param name="scenario">The saved scenario (null for free play).</param>
    /// <param name="hasWon">Whether the player had won.</param>
    public void LoadFromSave(Airline airline, Scenario? scenario, bool hasWon)
    {
        this.PlayerAirline = airline;
        this.CurrentScenario = scenario;
        this.HasWon = hasWon;
        this.isRunning = true;
    }

    /// <summary>
    /// Stops the current game session.
    /// </summary>
    public void Stop()
    {
        this.isRunning = false;
    }
}

/// <summary>
/// Represents a game scenario with specific objectives and starting conditions.
/// Like RCT's scenario challenges, these provide structured gameplay goals.
/// </summary>
/// <remarks>
/// Scenarios make the game more engaging by providing clear objectives:
/// - Reach X passengers by day Y
/// - Achieve X profit within Y days
/// - Build a network of X routes
/// - Maintain reputation above X
///
/// This creates the satisfying progression that made RCT so compelling.
/// </remarks>
public class Scenario
{
    /// <summary>
    /// Gets or sets the scenario name.
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets or sets the scenario description.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Gets or sets the starting airline name.
    /// </summary>
    public required string AirlineName { get; init; }

    /// <summary>
    /// Gets or sets the starting hub airport code.
    /// </summary>
    public required string StartingHub { get; init; }

    /// <summary>
    /// Gets or sets the starting cash amount.
    /// </summary>
    public required decimal StartingCash { get; init; }

    /// <summary>
    /// Gets the list of objectives that must be completed to win.
    /// </summary>
    public required List<ScenarioObjective> Objectives { get; init; }

    /// <summary>
    /// Checks if all scenario objectives have been met.
    /// </summary>
    /// <param name="airline">The player's airline.</param>
    /// <returns>True if all objectives are met, false otherwise.</returns>
    public bool CheckObjectives(Airline airline)
    {
        return this.Objectives.All(obj => obj.IsMet(airline));
    }
}

/// <summary>
/// Represents a single objective within a scenario.
/// </summary>
public abstract class ScenarioObjective
{
    /// <summary>
    /// Gets or sets the objective description.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Checks if this objective has been met.
    /// </summary>
    /// <param name="airline">The player's airline.</param>
    /// <returns>True if objective is met, false otherwise.</returns>
    public abstract bool IsMet(Airline airline);
}

/// <summary>
/// Objective: Reach a target number of total passengers carried.
/// </summary>
public class PassengerObjective : ScenarioObjective
{
    /// <summary>
    /// Gets or sets the target passenger count.
    /// </summary>
    public required int TargetPassengers { get; init; }

    /// <inheritdoc/>
    public override bool IsMet(Airline airline) =>
        airline.TotalPassengersCarried >= this.TargetPassengers;
}

/// <summary>
/// Objective: Reach a target net profit.
/// </summary>
public class ProfitObjective : ScenarioObjective
{
    /// <summary>
    /// Gets or sets the target net profit.
    /// </summary>
    public required decimal TargetProfit { get; init; }

    /// <inheritdoc/>
    public override bool IsMet(Airline airline) => airline.NetProfit >= this.TargetProfit;
}

/// <summary>
/// Objective: Operate a minimum number of routes.
/// </summary>
public class RouteCountObjective : ScenarioObjective
{
    /// <summary>
    /// Gets or sets the target route count.
    /// </summary>
    public required int TargetRouteCount { get; init; }

    /// <inheritdoc/>
    public override bool IsMet(Airline airline) =>
        airline.Routes.Count(r => r.IsActive) >= this.TargetRouteCount;
}

/// <summary>
/// Objective: Reach a target reputation level.
/// </summary>
public class ReputationObjective : ScenarioObjective
{
    /// <summary>
    /// Gets or sets the target reputation score.
    /// </summary>
    public required int TargetReputation { get; init; }

    /// <inheritdoc/>
    public override bool IsMet(Airline airline) => airline.Reputation >= this.TargetReputation;
}
