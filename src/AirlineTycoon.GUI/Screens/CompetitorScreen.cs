using AirlineTycoon.GUI.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AirlineTycoon.GUI.Screens;

/// <summary>
/// Competitor analysis screen showing rival airlines and market comparison.
/// Inspired by business simulation competitive intelligence views.
/// </summary>
/// <remarks>
/// Displays:
/// - List of competitor airlines with key stats
/// - Market share visualization
/// - Comparative bar charts (passengers, revenue, reputation)
/// - Competitor routes overlay
/// - AI personality indicators
///
/// This helps players understand the competitive landscape and identify
/// threats and opportunities.
/// </remarks>
public class CompetitorScreen : Screen
{
    private UIButton? backButton;

    /// <inheritdoc/>
    public override string Title => "Competitor Analysis";

    /// <summary>
    /// Initializes a new instance of the <see cref="CompetitorScreen"/> class.
    /// </summary>
    public CompetitorScreen()
    {
        this.InitializeUI();
    }

    /// <summary>
    /// Initializes the UI elements.
    /// </summary>
    private void InitializeUI()
    {
        this.backButton = new UIButton(
            "← Back",
            new Vector2(20, 50),
            new Vector2(120, 35)
        );
        this.backButton.Clicked += (s, e) => this.OnBack();
        this.AddChild(this.backButton);
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

        // Competitor list (left side)
        this.DrawCompetitorList(spriteBatch);

        // Market comparison charts (right side)
        this.DrawMarketComparison(spriteBatch);

        // Selected competitor details (bottom)
        this.DrawCompetitorDetails(spriteBatch);

        // Draw buttons
        base.Draw(spriteBatch, gameTime);
    }

    /// <summary>
    /// Draws the list of competitor airlines.
    /// </summary>
    private void DrawCompetitorList(SpriteBatch spriteBatch)
    {
        // List panel
        var listBounds = new Rectangle(20, 100, 380, 330);
        this.DrawFilledRectangle(spriteBatch, listBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.8f));
        this.Draw3DBorder(spriteBatch, listBounds);

        // Title
        var titleBounds = new Rectangle(30, 110, 360, 30);
        this.DrawFilledRectangle(spriteBatch, titleBounds, RetroColorPalette.TitleBarBackground);

        // Competitor cards (3 competitors)
        int cardY = 150;
        int cardHeight = 80;
        int cardSpacing = 10;

        for (int i = 0; i < 3; i++)
        {
            var cardBounds = new Rectangle(30, cardY + (i * (cardHeight + cardSpacing)), 360, cardHeight);

            // Card background (different colors to distinguish)
            Color[] competitorColors = new Color[]
            {
                RetroColorPalette.Darken(RetroColorPalette.Error, 0.7f),
                RetroColorPalette.Darken(RetroColorPalette.Info, 0.7f),
                RetroColorPalette.Darken(RetroColorPalette.Warning, 0.7f)
            };

            this.DrawFilledRectangle(spriteBatch, cardBounds, competitorColors[i]);
            this.Draw3DBorder(spriteBatch, cardBounds, 2);

            // Logo area
            var logoounds = new Rectangle(40, cardY + (i * (cardHeight + cardSpacing)) + 10, 60, 60);
            this.DrawFilledRectangle(spriteBatch, logoounds, RetroColorPalette.AircraftWhite);
            this.Draw3DBorder(spriteBatch, logoounds, 1);

            // Stats bars (passengers, revenue, reputation)
            int statsX = 110;
            int statsY = cardY + (i * (cardHeight + cardSpacing)) + 15;

            var passengersBar = new Rectangle(statsX, statsY, 100 + (i * 30), 15);
            this.DrawFilledRectangle(spriteBatch, passengersBar, RetroColorPalette.Info);
            this.Draw3DBorder(spriteBatch, passengersBar, 1);

            var revenueBar = new Rectangle(statsX, statsY + 23, 80 + (i * 40), 15);
            this.DrawFilledRectangle(spriteBatch, revenueBar, RetroColorPalette.Success);
            this.Draw3DBorder(spriteBatch, revenueBar, 1);

            var reputationBar = new Rectangle(statsX, statsY + 46, 90 + (i * 20), 15);
            this.DrawFilledRectangle(spriteBatch, reputationBar, RetroColorPalette.Warning);
            this.Draw3DBorder(spriteBatch, reputationBar, 1);

            // Personality indicator (top right of card)
            string[] personalities = new string[] { "AGG", "CON", "BUD" };
            var personalityBounds = new Rectangle(340, cardY + (i * (cardHeight + cardSpacing)) + 10, 40, 20);
            this.DrawFilledRectangle(spriteBatch, personalityBounds, RetroColorPalette.ButtonNormal);
            this.Draw3DBorder(spriteBatch, personalityBounds, 1);
        }
    }

    /// <summary>
    /// Draws market comparison charts.
    /// </summary>
    private void DrawMarketComparison(SpriteBatch spriteBatch)
    {
        // Chart panel
        var chartBounds = new Rectangle(420, 100, 840, 330);
        this.DrawFilledRectangle(spriteBatch, chartBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.8f));
        this.Draw3DBorder(spriteBatch, chartBounds);

        // Title
        var titleBounds = new Rectangle(430, 110, 820, 30);
        this.DrawFilledRectangle(spriteBatch, titleBounds, RetroColorPalette.TitleBarBackground);

        // Bar chart area
        var barChartBounds = new Rectangle(440, 160, 800, 250);
        this.DrawFilledRectangle(spriteBatch, barChartBounds, Color.Black);
        this.Draw3DBorder(spriteBatch, barChartBounds, 1);

        // Comparative bars (player vs competitors)
        int barX = 480;
        int barWidth = 60;
        int barSpacing = 100;

        // Draw bars for 4 airlines (player + 3 competitors)
        Color[] barColors = new Color[]
        {
            RetroColorPalette.Info,        // Player
            RetroColorPalette.Error,       // Competitor 1
            RetroColorPalette.Warning,     // Competitor 2
            RetroColorPalette.Success      // Competitor 3
        };

        int[] barHeights = new int[] { 180, 140, 160, 120 };

        for (int i = 0; i < 4; i++)
        {
            int x = barX + (i * barSpacing);
            int height = barHeights[i];
            int y = 390 - height;

            var bar = new Rectangle(x, y, barWidth, height);
            this.DrawFilledRectangle(spriteBatch, bar, barColors[i]);
            this.Draw3DBorder(spriteBatch, bar, 1);
        }

        // Second metric (revenue)
        barX = 850;

        int[] revenueHeights = new int[] { 200, 120, 100, 150 };

        for (int i = 0; i < 4; i++)
        {
            int x = barX + (i * barSpacing);
            int height = revenueHeights[i];
            int y = 390 - height;

            var bar = new Rectangle(x, y, barWidth, height);
            this.DrawFilledRectangle(spriteBatch, bar, barColors[i]);
            this.Draw3DBorder(spriteBatch, bar, 1);
        }
    }

    /// <summary>
    /// Draws detailed competitor information for selected airline.
    /// </summary>
    private void DrawCompetitorDetails(SpriteBatch spriteBatch)
    {
        // Details panel
        var detailsBounds = new Rectangle(20, 450, 1240, 250);
        this.DrawFilledRectangle(spriteBatch, detailsBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.8f));
        this.Draw3DBorder(spriteBatch, detailsBounds);

        // Title
        var titleBounds = new Rectangle(30, 460, 1220, 30);
        this.DrawFilledRectangle(spriteBatch, titleBounds, RetroColorPalette.TitleBarBackground);

        // Competitor routes section
        var routesBounds = new Rectangle(40, 510, 580, 170);
        this.DrawFilledRectangle(spriteBatch, routesBounds, Color.Black);
        this.Draw3DBorder(spriteBatch, routesBounds, 1);

        // TODO: Display competitor's routes
        // Show routes where we compete head-to-head

        // Route rows (placeholder)
        for (int i = 0; i < 4; i++)
        {
            var routeRow = new Rectangle(50, 520 + (i * 38), 560, 30);
            this.DrawFilledRectangle(spriteBatch, routeRow, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.7f));

            // Overlapping route indicator
            if (i % 2 == 0)
            {
                var overlapIndicator = new Rectangle(55, 525 + (i * 38), 10, 20);
                this.DrawFilledRectangle(spriteBatch, overlapIndicator, RetroColorPalette.Error);
            }
        }

        // Financial stats section
        var statsBounds = new Rectangle(640, 510, 600, 170);
        this.DrawFilledRectangle(spriteBatch, statsBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.9f));
        this.Draw3DBorder(spriteBatch, statsBounds, 1);

        // Stats grid (2x2)
        var stat1 = new Rectangle(660, 530, 270, 60);
        this.DrawFilledRectangle(spriteBatch, stat1, RetroColorPalette.Info);
        this.Draw3DBorder(spriteBatch, stat1, 1);

        var stat2 = new Rectangle(950, 530, 270, 60);
        this.DrawFilledRectangle(spriteBatch, stat2, RetroColorPalette.Success);
        this.Draw3DBorder(spriteBatch, stat2, 1);

        var stat3 = new Rectangle(660, 610, 270, 60);
        this.DrawFilledRectangle(spriteBatch, stat3, RetroColorPalette.Warning);
        this.Draw3DBorder(spriteBatch, stat3, 1);

        var stat4 = new Rectangle(950, 610, 270, 60);
        this.DrawFilledRectangle(spriteBatch, stat4, RetroColorPalette.Error);
        this.Draw3DBorder(spriteBatch, stat4, 1);

        // TODO: Display actual stats:
        // - Cash reserves
        // - Fleet size
        // - Total passengers
        // - Market share
    }

    /// <summary>
    /// Handles the back button click.
    /// </summary>
    private void OnBack()
    {
        System.Diagnostics.Debug.WriteLine("Back to dashboard");
    }
}
