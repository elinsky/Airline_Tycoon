using AirlineTycoon.GUI.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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
    private UIButton? advanceDayButton;

    /// <inheritdoc/>
    public override string Title => "Dashboard - Airline Tycoon";

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardScreen"/> class.
    /// </summary>
    public DashboardScreen()
    {
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

        // TODO: Once we have bitmap fonts, display actual stats here:
        // - Total passengers carried
        // - Today's profit
        // - Active routes count
        // - Fleet size
        // - Reputation trend

        // For now, draw placeholder colored boxes showing layout
        var stat1Bounds = new Rectangle(280, 80, 220, 80);
        this.DrawFilledRectangle(spriteBatch, stat1Bounds, RetroColorPalette.Info);
        this.Draw3DBorder(spriteBatch, stat1Bounds, 1);

        var stat2Bounds = new Rectangle(520, 80, 220, 80);
        this.DrawFilledRectangle(spriteBatch, stat2Bounds, RetroColorPalette.Success);
        this.Draw3DBorder(spriteBatch, stat2Bounds, 1);

        var stat3Bounds = new Rectangle(760, 80, 220, 80);
        this.DrawFilledRectangle(spriteBatch, stat3Bounds, RetroColorPalette.Warning);
        this.Draw3DBorder(spriteBatch, stat3Bounds, 1);

        var stat4Bounds = new Rectangle(1000, 80, 220, 80);
        this.DrawFilledRectangle(spriteBatch, stat4Bounds, RetroColorPalette.Error);
        this.Draw3DBorder(spriteBatch, stat4Bounds, 1);

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

        // TODO: Display recent events here once we have text rendering
        // For now, show placeholder event boxes
        var event1Bounds = new Rectangle(280, 500, 940, 30);
        this.DrawFilledRectangle(spriteBatch, event1Bounds, RetroColorPalette.Info);

        var event2Bounds = new Rectangle(280, 540, 940, 30);
        this.DrawFilledRectangle(spriteBatch, event2Bounds, RetroColorPalette.Warning);

        var event3Bounds = new Rectangle(280, 580, 940, 30);
        this.DrawFilledRectangle(spriteBatch, event3Bounds, RetroColorPalette.Success);
    }

    /// <summary>
    /// Handles the View Routes button click.
    /// </summary>
    private void OnViewRoutes()
    {
        // TODO: Switch to RouteManagementScreen
        System.Diagnostics.Debug.WriteLine("View Routes clicked!");
    }

    /// <summary>
    /// Handles the View Fleet button click.
    /// </summary>
    private void OnViewFleet()
    {
        // TODO: Switch to FleetManagementScreen
        System.Diagnostics.Debug.WriteLine("View Fleet clicked!");
    }

    /// <summary>
    /// Handles the View Competitors button click.
    /// </summary>
    private void OnViewCompetitors()
    {
        // TODO: Switch to CompetitorScreen
        System.Diagnostics.Debug.WriteLine("View Competitors clicked!");
    }

    /// <summary>
    /// Handles the Financial Report button click.
    /// </summary>
    private void OnViewFinancials()
    {
        // TODO: Switch to FinancialReportScreen
        System.Diagnostics.Debug.WriteLine("View Financials clicked!");
    }

    /// <summary>
    /// Handles the Advance Day button click.
    /// </summary>
    private void OnAdvanceDay()
    {
        // TODO: Call game.ProcessDay() and refresh UI
        System.Diagnostics.Debug.WriteLine("Advance Day clicked!");

        if (this.GameInstance != null)
        {
            // Process the day
            // var summary = this.GameInstance.ProcessDay();
            // Refresh UI with new data
        }
    }
}
