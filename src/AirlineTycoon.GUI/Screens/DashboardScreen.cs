using AirlineTycoon.GUI.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Linq;

namespace AirlineTycoon.GUI.Screens;

/// <summary>
/// Main dashboard screen showing airline overview and quick stats.
/// Inspired by RollerCoaster Tycoon's main park view.
/// </summary>
/// <remarks>
/// The dashboard displays:
/// - Current day, cash, and reputation in top bar
/// - Quick stats panel (passengers today, profit, active routes)
/// - Recent events ticker
/// - Navigation buttons to other screens
/// - Notification area for important alerts
///
/// This is the "home" screen where players start and return to frequently.
/// </remarks>
public class DashboardScreen : Screen
{
    private UIButton? viewRoutesButton;
    private UIButton? viewFleetButton;
    private UIButton? viewCompetitorsButton;
    private UIButton? viewFinancialsButton;
    private UIButton? saveLoadButton;
    private UIButton? advanceDayButton;

    /// <inheritdoc/>
    public override string Title => "Dashboard - Airline Tycoon";

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardScreen"/> class.
    /// </summary>
    /// <param name="controller">The game controller.</param>
    public DashboardScreen(GameController controller)
    {
        this.SetGameController(controller);
        this.InitializeUI();
    }

    /// <summary>
    /// Initializes the UI elements for the dashboard.
    /// </summary>
    private void InitializeUI()
    {
        // Main navigation buttons (left side panel)
        int buttonX = 20;
        int buttonY = 60;
        int buttonWidth = 200;
        int buttonHeight = 40;
        int buttonSpacing = 50;

        this.viewRoutesButton = new UIButton(
            "View Routes",
            new Vector2(buttonX, buttonY),
            new Vector2(buttonWidth, buttonHeight)
        );
        this.viewRoutesButton.Clicked += (s, e) => this.OnViewRoutes();
        this.AddChild(this.viewRoutesButton);

        this.viewFleetButton = new UIButton(
            "View Fleet",
            new Vector2(buttonX, buttonY + buttonSpacing),
            new Vector2(buttonWidth, buttonHeight)
        );
        this.viewFleetButton.Clicked += (s, e) => this.OnViewFleet();
        this.AddChild(this.viewFleetButton);

        this.viewCompetitorsButton = new UIButton(
            "View Competitors",
            new Vector2(buttonX, buttonY + buttonSpacing * 2),
            new Vector2(buttonWidth, buttonHeight)
        );
        this.viewCompetitorsButton.Clicked += (s, e) => this.OnViewCompetitors();
        this.AddChild(this.viewCompetitorsButton);

        this.viewFinancialsButton = new UIButton(
            "Financial Report",
            new Vector2(buttonX, buttonY + buttonSpacing * 3),
            new Vector2(buttonWidth, buttonHeight)
        );
        this.viewFinancialsButton.Clicked += (s, e) => this.OnViewFinancials();
        this.AddChild(this.viewFinancialsButton);

        this.saveLoadButton = new UIButton(
            "Save / Load Game",
            new Vector2(buttonX, buttonY + buttonSpacing * 4),
            new Vector2(buttonWidth, buttonHeight)
        );
        this.saveLoadButton.Clicked += (s, e) => this.OnSaveLoad();
        this.AddChild(this.saveLoadButton);

        // Advance day button (prominent, bottom left)
        this.advanceDayButton = new UIButton(
            "Advance Day >>",
            new Vector2(buttonX, AirlineTycoonGame.BaseHeight - 60),
            new Vector2(buttonWidth, 50)
        );
        this.advanceDayButton.Clicked += (s, e) => this.OnAdvanceDay();
        this.AddChild(this.advanceDayButton);
    }

    /// <inheritdoc/>
    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (!this.IsVisible)
        {
            return;
        }

        // Background
        this.DrawFilledRectangle(spriteBatch, this.Bounds, RetroColorPalette.WindowBackground);

        // Top bar with stats
        this.DrawTopBar(spriteBatch, gameTime);

        // Stats panel (center area)
        this.DrawStatsPanel(spriteBatch);

        // Events ticker (bottom area)
        this.DrawEventsTicker(spriteBatch);

        // Draw all child UI elements (buttons)
        base.Draw(spriteBatch, gameTime);
    }

    /// <summary>
    /// Draws the quick stats panel in the center area.
    /// </summary>
    private void DrawStatsPanel(SpriteBatch spriteBatch)
    {
        // Stats panel background
        var panelBounds = new Rectangle(260, 60, 1000, 400);
        this.DrawFilledRectangle(spriteBatch, panelBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.8f));
        this.Draw3DBorder(spriteBatch, panelBounds);

        // Get actual game data
        var airline = this.Controller?.Game.PlayerAirline;
        if (airline == null || AirlineTycoonGame.TextRenderer == null)
        {
            return;
        }

        // Calculate statistics
        int passengersToday = airline.Routes.Sum(r => r.TotalPassengers);
        decimal profitToday = airline.Routes.Sum(r => r.DailyProfit);
        int activeRoutes = airline.Routes.Count(r => r.IsActive);
        int totalFleet = airline.Fleet.Count;
        int assignedAircraft = airline.Routes.Count(r => r.AssignedAircraft != null);
        decimal fleetUtilization = totalFleet > 0 ? (decimal)assignedAircraft / totalFleet : 0m;

        // Stat 1: Today's Passengers
        var stat1Bounds = new Rectangle(280, 80, 220, 80);
        this.DrawFilledRectangle(spriteBatch, stat1Bounds, RetroColorPalette.Darken(RetroColorPalette.Info, 0.8f));
        this.Draw3DBorder(spriteBatch, stat1Bounds, 1);
        AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Passengers Today", new Vector2(290, 90), Color.White);
        AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, passengersToday.ToString("N0"), new Vector2(290, 115), RetroColorPalette.Info);

        // Stat 2: Today's Profit
        var stat2Bounds = new Rectangle(520, 80, 220, 80);
        Color profitColor = profitToday >= 0 ? RetroColorPalette.Success : RetroColorPalette.Error;
        this.DrawFilledRectangle(spriteBatch, stat2Bounds, RetroColorPalette.Darken(profitColor, 0.8f));
        this.Draw3DBorder(spriteBatch, stat2Bounds, 1);
        AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Today's Profit", new Vector2(530, 90), Color.White);
        string profitText = profitToday >= 0 ? $"+${profitToday:N0}" : $"-${System.Math.Abs(profitToday):N0}";
        AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, profitText, new Vector2(530, 115), profitColor);

        // Stat 3: Active Routes
        var stat3Bounds = new Rectangle(760, 80, 220, 80);
        this.DrawFilledRectangle(spriteBatch, stat3Bounds, RetroColorPalette.Darken(RetroColorPalette.Warning, 0.8f));
        this.Draw3DBorder(spriteBatch, stat3Bounds, 1);
        AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Active Routes", new Vector2(770, 90), Color.White);
        AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, activeRoutes.ToString(), new Vector2(770, 115), RetroColorPalette.Warning);

        // Stat 4: Fleet Utilization
        var stat4Bounds = new Rectangle(1000, 80, 220, 80);
        Color utilizationColor = fleetUtilization > 0.8m ? RetroColorPalette.Success :
                                fleetUtilization > 0.5m ? RetroColorPalette.Warning :
                                RetroColorPalette.Error;
        this.DrawFilledRectangle(spriteBatch, stat4Bounds, RetroColorPalette.Darken(utilizationColor, 0.8f));
        this.Draw3DBorder(spriteBatch, stat4Bounds, 1);
        AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Fleet Utilization", new Vector2(1010, 90), Color.White);
        AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, $"{fleetUtilization:P0}", new Vector2(1010, 115), utilizationColor);

        // Graph area (bottom half of panel)
        var graphBounds = new Rectangle(280, 180, 940, 260);
        this.DrawFilledRectangle(spriteBatch, graphBounds, Color.Black);
        this.Draw3DBorder(spriteBatch, graphBounds, 1);

        // TODO: Draw actual profit/passenger trend graph here
    }

    /// <summary>
    /// Draws the events ticker showing recent game events.
    /// </summary>
    private void DrawEventsTicker(SpriteBatch spriteBatch)
    {
        // Events ticker background
        var tickerBounds = new Rectangle(260, 480, 1000, 220);
        this.DrawFilledRectangle(spriteBatch, tickerBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.8f));
        this.Draw3DBorder(spriteBatch, tickerBounds);

        // Header
        var headerBounds = new Rectangle(270, 490, 980, 25);
        this.DrawFilledRectangle(spriteBatch, headerBounds, RetroColorPalette.TitleBarBackground);

        if (AirlineTycoonGame.TextRenderer != null)
        {
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                "Recent Events",
                new Vector2(280, 495),
                Color.White
            );
        }

        // Get actual game events
        var airline = this.Controller?.Game.PlayerAirline;
        if (airline == null || AirlineTycoonGame.TextRenderer == null)
        {
            return;
        }

        // Get recent events (sorted by day, most recent first)
        var allEvents = airline.ActiveEvents
            .OrderByDescending(e => e.OccurredOnDay)
            .Take(5)
            .ToList();

        if (allEvents.Count == 0)
        {
            // Show "No events" message
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                "No recent events",
                new Vector2(280, 530),
                RetroColorPalette.TextSecondary
            );
            return;
        }

        // Draw event rows
        int eventY = 525;
        int eventRowHeight = 35;
        int eventSpacing = 37;

        for (int i = 0; i < allEvents.Count; i++)
        {
            var gameEvent = allEvents[i];
            var eventBounds = new Rectangle(270, eventY + (i * eventSpacing), 980, eventRowHeight);

            // Color based on event severity
            Color eventColor = gameEvent.Severity switch
            {
                AirlineTycoon.Domain.Events.EventSeverity.Minor => RetroColorPalette.Info,
                AirlineTycoon.Domain.Events.EventSeverity.Moderate => RetroColorPalette.Warning,
                AirlineTycoon.Domain.Events.EventSeverity.Major => RetroColorPalette.Error,
                AirlineTycoon.Domain.Events.EventSeverity.Critical => RetroColorPalette.Darken(RetroColorPalette.Error, 0.7f),
                _ => RetroColorPalette.Info
            };

            // Darker background for event row
            this.DrawFilledRectangle(spriteBatch, eventBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.95f));

            // Severity indicator (left edge)
            var severityIndicator = new Rectangle(275, eventY + (i * eventSpacing) + 3, 8, eventRowHeight - 6);
            this.DrawFilledRectangle(spriteBatch, severityIndicator, eventColor);

            // Event title
            string eventText = $"Day {gameEvent.OccurredOnDay}: {gameEvent.Title}";
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                eventText,
                new Vector2(290, eventY + (i * eventSpacing) + 5),
                Color.White
            );

            // Event summary (impact details)
            string summary = gameEvent.GetSummary();
            if (!string.IsNullOrEmpty(summary))
            {
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    summary,
                    new Vector2(290, eventY + (i * eventSpacing) + 18),
                    RetroColorPalette.TextSecondary
                );
            }
        }
    }

    /// <summary>
    /// Handles the View Routes button click.
    /// </summary>
    private void OnViewRoutes()
    {
        System.Diagnostics.Debug.WriteLine("OnViewRoutes called");
        try
        {
            this.Controller?.ShowRouteManagement();
            System.Diagnostics.Debug.WriteLine("ShowRouteManagement completed");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR in OnViewRoutes: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    /// <summary>
    /// Handles the View Fleet button click.
    /// </summary>
    private void OnViewFleet()
    {
        this.Controller?.ShowFleetManagement();
    }

    /// <summary>
    /// Handles the View Competitors button click.
    /// </summary>
    private void OnViewCompetitors()
    {
        this.Controller?.ShowCompetitors();
    }

    /// <summary>
    /// Handles the Financial Report button click.
    /// </summary>
    private void OnViewFinancials()
    {
        this.Controller?.ShowFinancialReport();
    }

    /// <summary>
    /// Handles the Save/Load Game button click.
    /// </summary>
    private void OnSaveLoad()
    {
        this.Controller?.ShowSaveLoad();
    }

    /// <summary>
    /// Handles the Advance Day button click.
    /// </summary>
    private void OnAdvanceDay()
    {
        try
        {
            if (this.Controller != null)
            {
                // Play day advance sound
                AirlineTycoonGame.AudioManager?.PlayDayAdvance();

                // Process the day and get results
                var summary = this.Controller.AdvanceDay();

                // TODO: Show a summary modal or notification with the day's results
                // For now, just log it
                System.Diagnostics.Debug.WriteLine($"Day advanced! Passengers: {summary.PassengersCarried}, " +
                    $"Revenue: ${summary.Revenue:N0}, Costs: ${summary.Costs:N0}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ERROR in OnAdvanceDay: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}
