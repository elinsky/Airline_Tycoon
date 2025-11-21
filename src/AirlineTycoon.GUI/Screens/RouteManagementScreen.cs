using AirlineTycoon.GUI.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AirlineTycoon.GUI.Screens;

/// <summary>
/// Route management screen showing all routes and their performance.
/// Similar to RCT's ride list view.
/// </summary>
/// <remarks>
/// Displays:
/// - Sortable list of all routes
/// - Route profitability indicators (green/red)
/// - Load factors and passenger counts
/// - Buttons to open/close routes
/// - Route details panel
///
/// Like RCT's ride management, this gives players a quick overview of
/// what's profitable and what needs attention.
/// </remarks>
public class RouteManagementScreen : Screen
{
    private UIButton? backButton;
    private UIButton? openRouteButton;

    /// <inheritdoc/>
    public override string Title => "Route Management";

    /// <summary>
    /// Initializes a new instance of the <see cref="RouteManagementScreen"/> class.
    /// </summary>
    /// <param name="controller">The game controller.</param>
    public RouteManagementScreen(GameController controller)
    {
        this.SetGameController(controller);
        this.InitializeUI();
    }

    /// <summary>
    /// Initializes the UI elements.
    /// </summary>
    private void InitializeUI()
    {
        // Back button (top left)
        this.backButton = new UIButton(
            "← Back",
            new Vector2(20, 50),
            new Vector2(120, 35)
        );
        this.backButton.Clicked += (s, e) => this.OnBack();
        this.AddChild(this.backButton);

        // Open New Route button (top right)
        this.openRouteButton = new UIButton(
            "+ Open Route",
            new Vector2(AirlineTycoonGame.BaseWidth - 170, 50),
            new Vector2(150, 35)
        );
        this.openRouteButton.Clicked += (s, e) => this.OnOpenRoute();
        this.AddChild(this.openRouteButton);
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

        // Top bar
        this.DrawTopBar(spriteBatch, gameTime);

        // Route list panel
        this.DrawRouteList(spriteBatch);

        // Route details panel (right side)
        this.DrawRouteDetails(spriteBatch);

        // Draw buttons
        base.Draw(spriteBatch, gameTime);
    }

    /// <summary>
    /// Draws the list of routes.
    /// </summary>
    private void DrawRouteList(SpriteBatch spriteBatch)
    {
        // List panel background
        var listBounds = new Rectangle(20, 100, 780, 600);
        this.DrawFilledRectangle(spriteBatch, listBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.8f));
        this.Draw3DBorder(spriteBatch, listBounds);

        // Column headers
        var headerBounds = new Rectangle(30, 110, 760, 30);
        this.DrawFilledRectangle(spriteBatch, headerBounds, RetroColorPalette.TitleBarBackground);

        // TODO: Draw column header text once we have fonts:
        // Origin | Destination | Price | Load Factor | Daily Profit | Status

        // Route rows (placeholder - will be populated from game data)
        int rowY = 150;
        int rowHeight = 35;
        int rowSpacing = 40;

        for (int i = 0; i < 10; i++)
        {
            var rowBounds = new Rectangle(30, rowY + (i * rowSpacing), 760, rowHeight);

            // Alternate row colors for readability
            Color rowColor = i % 2 == 0
                ? RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.9f)
                : RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.7f);

            this.DrawFilledRectangle(spriteBatch, rowBounds, rowColor);

            // Profit indicator (left edge - green for profit, red for loss)
            Color profitColor = i % 3 == 0 ? RetroColorPalette.Error : RetroColorPalette.Success;
            var profitIndicator = new Rectangle(35, rowY + (i * rowSpacing) + 5, 10, rowHeight - 10);
            this.DrawFilledRectangle(spriteBatch, profitIndicator, profitColor);

            // TODO: Display actual route data once we have text rendering
        }
    }

    /// <summary>
    /// Draws the route details panel.
    /// </summary>
    private void DrawRouteDetails(SpriteBatch spriteBatch)
    {
        // Details panel background
        var detailsBounds = new Rectangle(820, 100, 440, 600);
        this.DrawFilledRectangle(spriteBatch, detailsBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.8f));
        this.Draw3DBorder(spriteBatch, detailsBounds);

        // Title area
        var titleBounds = new Rectangle(830, 110, 420, 30);
        this.DrawFilledRectangle(spriteBatch, titleBounds, RetroColorPalette.TitleBarBackground);

        // TODO: Display selected route details:
        // - Distance
        // - Aircraft assigned
        // - Average load factor
        // - Revenue breakdown
        // - Cost breakdown
        // - Historical performance chart

        // Stats placeholders
        var stat1 = new Rectangle(840, 160, 190, 60);
        this.DrawFilledRectangle(spriteBatch, stat1, RetroColorPalette.Info);
        this.Draw3DBorder(spriteBatch, stat1, 1);

        var stat2 = new Rectangle(1050, 160, 190, 60);
        this.DrawFilledRectangle(spriteBatch, stat2, RetroColorPalette.Success);
        this.Draw3DBorder(spriteBatch, stat2, 1);

        // Chart area
        var chartBounds = new Rectangle(840, 240, 400, 200);
        this.DrawFilledRectangle(spriteBatch, chartBounds, Color.Black);
        this.Draw3DBorder(spriteBatch, chartBounds, 1);

        // Action buttons
        var closeRouteButton = new Rectangle(840, 650, 180, 35);
        this.DrawFilledRectangle(spriteBatch, closeRouteButton, RetroColorPalette.Error);
        this.Draw3DBorder(spriteBatch, closeRouteButton, 2);

        var changePriceButton = new Rectangle(1060, 650, 180, 35);
        this.DrawFilledRectangle(spriteBatch, changePriceButton, RetroColorPalette.ButtonNormal);
        this.Draw3DBorder(spriteBatch, changePriceButton, 2);
    }

    /// <summary>
    /// Handles the back button click.
    /// </summary>
    private void OnBack()
    {
        this.Controller?.ShowDashboard();
    }

    /// <summary>
    /// Handles the open route button click.
    /// </summary>
    private void OnOpenRoute()
    {
        // TODO: Show route selection dialog
        System.Diagnostics.Debug.WriteLine("Open new route dialog");
    }
}
