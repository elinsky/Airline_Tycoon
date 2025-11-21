using AirlineTycoon.GUI.Screens;
using AirlineTycoon;
using System;
using System.Linq;

namespace AirlineTycoon.GUI;

/// <summary>
/// Controller class that manages the game instance and coordinates between
/// the game logic and the GUI.
/// </summary>
/// <remarks>
/// The GameController acts as a bridge between:
/// - The Game class (from AirlineTycoon core) which handles business logic
/// - The GUI screens which display data and handle user input
/// - The ScreenManager which handles screen transitions
///
/// This separation keeps the GUI code clean and testable.
/// </remarks>
public class GameController
{
    private readonly AirlineTycoon.Game game;
    private readonly ScreenManager screenManager;

    /// <summary>
    /// Gets the game instance.
    /// </summary>
    public AirlineTycoon.Game Game => this.game;

    /// <summary>
    /// Gets the screen manager.
    /// </summary>
    public ScreenManager ScreenManager => this.screenManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="GameController"/> class.
    /// </summary>
    /// <param name="screenManager">The screen manager to use for navigation.</param>
    public GameController(ScreenManager screenManager)
    {
        this.screenManager = screenManager;
        this.game = new AirlineTycoon.Game();
    }

    /// <summary>
    /// Starts a new game with default settings.
    /// </summary>
    public void StartNewGame()
    {
        // Start a new game with default airline
        this.game.StartNewGame("SkyWings Airlines", "JFK");

        // Navigate to dashboard
        this.ShowDashboard();
    }

    /// <summary>
    /// Advances the game by one day and returns the results.
    /// </summary>
    /// <returns>The daily operations summary.</returns>
    public AirlineTycoon.Domain.DailyOperationsSummary AdvanceDay()
    {
        return this.game.ProcessDay();
    }

    /// <summary>
    /// Navigates to the dashboard screen.
    /// </summary>
    public void ShowDashboard()
    {
        var dashboard = new DashboardScreen(this);
        this.screenManager.SwitchTo(dashboard);
    }

    /// <summary>
    /// Navigates to the route management screen.
    /// </summary>
    public void ShowRouteManagement()
    {
        var routeScreen = new RouteManagementScreen(this);
        this.screenManager.SwitchTo(routeScreen);
    }

    /// <summary>
    /// Navigates to the fleet management screen.
    /// </summary>
    public void ShowFleetManagement()
    {
        var fleetScreen = new FleetManagementScreen(this);
        this.screenManager.SwitchTo(fleetScreen);
    }

    /// <summary>
    /// Navigates to the competitor analysis screen.
    /// </summary>
    public void ShowCompetitors()
    {
        var competitorScreen = new CompetitorScreen(this);
        this.screenManager.SwitchTo(competitorScreen);
    }

    /// <summary>
    /// Navigates to the financial report screen.
    /// </summary>
    public void ShowFinancialReport()
    {
        var financialScreen = new FinancialReportScreen(this);
        this.screenManager.SwitchTo(financialScreen);
    }

    /// <summary>
    /// Navigates to the open route screen.
    /// </summary>
    public void ShowOpenRoute()
    {
        var openRouteScreen = new OpenRouteScreen(this);
        this.screenManager.SwitchTo(openRouteScreen);
    }

    /// <summary>
    /// Purchases an aircraft of the specified type.
    /// </summary>
    /// <param name="aircraftType">The type of aircraft to purchase.</param>
    /// <returns>True if the purchase was successful, false otherwise.</returns>
    public bool PurchaseAircraft(AirlineTycoon.Domain.AircraftType aircraftType)
    {
        try
        {
            this.game.PlayerAirline.PurchaseAircraft(aircraftType);
            return true;
        }
        catch (System.InvalidOperationException)
        {
            // Not enough cash
            return false;
        }
    }

    /// <summary>
    /// Leases an aircraft of the specified type.
    /// </summary>
    /// <param name="aircraftType">The type of aircraft to lease.</param>
    /// <returns>True if the lease was successful, false otherwise.</returns>
    public bool LeaseAircraft(AirlineTycoon.Domain.AircraftType aircraftType)
    {
        System.Diagnostics.Debug.WriteLine($"GameController.LeaseAircraft called for {aircraftType.Name}");
        System.Diagnostics.Debug.WriteLine($"Player cash before: ${this.game.PlayerAirline.Cash:N0}");

        try
        {
            this.game.PlayerAirline.LeaseAircraft(aircraftType);
            System.Diagnostics.Debug.WriteLine($"Player cash after: ${this.game.PlayerAirline.Cash:N0}");
            System.Diagnostics.Debug.WriteLine($"Fleet count: {this.game.PlayerAirline.Fleet.Count}");
            return true;
        }
        catch (System.InvalidOperationException ex)
        {
            // Some error occurred
            System.Diagnostics.Debug.WriteLine($"LeaseAircraft failed: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Opens a route between two airports.
    /// </summary>
    /// <param name="origin">The origin airport.</param>
    /// <param name="destination">The destination airport.</param>
    /// <param name="ticketPrice">The ticket price for the route.</param>
    /// <returns>True if the route was opened successfully, false otherwise.</returns>
    public bool OpenRoute(AirlineTycoon.Domain.Airport origin, AirlineTycoon.Domain.Airport destination, decimal ticketPrice)
    {
        try
        {
            this.game.PlayerAirline.OpenRoute(origin, destination, ticketPrice);
            return true;
        }
        catch (System.Exception)
        {
            // Route already exists, invalid airports, etc.
            return false;
        }
    }

    /// <summary>
    /// Closes an existing route.
    /// </summary>
    /// <param name="originCode">The origin airport code.</param>
    /// <param name="destinationCode">The destination airport code.</param>
    /// <returns>True if the route was closed successfully, false otherwise.</returns>
    public bool CloseRoute(string originCode, string destinationCode)
    {
        var route = this.game.PlayerAirline.Routes
            .FirstOrDefault(r => r.Origin.Code == originCode && r.Destination.Code == destinationCode);

        if (route != null)
        {
            this.game.PlayerAirline.CloseRoute(route);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Assigns an aircraft to a route.
    /// </summary>
    /// <param name="routeId">The ID of the route.</param>
    /// <param name="aircraftRegistration">The registration number of the aircraft to assign.</param>
    /// <returns>True if the assignment was successful, false otherwise.</returns>
    public bool AssignAircraftToRoute(Guid routeId, string aircraftRegistration)
    {
        try
        {
            var route = this.game.PlayerAirline.Routes
                .FirstOrDefault(r => r.Id == routeId);

            var aircraft = this.game.PlayerAirline.Fleet
                .FirstOrDefault(a => a.RegistrationNumber == aircraftRegistration);

            if (route == null || aircraft == null)
            {
                return false;
            }

            // Check if aircraft is already assigned to another route
            var existingAssignment = this.game.PlayerAirline.Routes
                .FirstOrDefault(r => r.AssignedAircraft?.RegistrationNumber == aircraftRegistration);

            if (existingAssignment != null)
            {
                // Unassign from existing route first
                existingAssignment.UnassignAircraft();
            }

            // Assign to new route
            route.AssignAircraft(aircraft);
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }

    /// <summary>
    /// Unassigns an aircraft from a route.
    /// </summary>
    /// <param name="routeId">The ID of the route.</param>
    /// <returns>True if the unassignment was successful, false otherwise.</returns>
    public bool UnassignAircraftFromRoute(Guid routeId)
    {
        try
        {
            var route = this.game.PlayerAirline.Routes
                .FirstOrDefault(r => r.Id == routeId);

            if (route == null)
            {
                return false;
            }

            route.UnassignAircraft();
            return true;
        }
        catch (System.Exception)
        {
            return false;
        }
    }
}
