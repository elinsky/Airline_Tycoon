using AirlineTycoon.GUI.Screens;
using AirlineTycoon;

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
}
