using AirlineTycoon.GUI.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

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
    private int selectedCompetitorIndex;

    /// <inheritdoc/>
    public override string Title => "Competitor Analysis";

    /// <summary>
    /// Initializes a new instance of the <see cref="CompetitorScreen"/> class.
    /// </summary>
    /// <param name="controller">The game controller.</param>
    public CompetitorScreen(GameController controller)
    {
        this.SetGameController(controller);
        this.InitializeUI();
    }

    /// <summary>
    /// Initializes the UI elements.
    /// </summary>
    private void InitializeUI()
    {
        this.backButton = new UIButton(
            "< Back",
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

        if (AirlineTycoonGame.TextRenderer != null)
        {
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Competitors", new Vector2(40, 117), Color.White);
        }

        // Get actual competitors
        var competitors = this.Controller?.Game.Competitors ?? new List<AirlineTycoon.Domain.AI.CompetitorAirline>();

        if (competitors.Count == 0)
        {
            if (AirlineTycoonGame.TextRenderer != null)
            {
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    "No competitors found",
                    new Vector2(40, 160),
                    RetroColorPalette.TextSecondary
                );
            }
            return;
        }

        // Competitor cards
        int cardY = 150;
        int cardHeight = 80;
        int cardSpacing = 10;

        // Card background colors
        Color[] competitorColors = new Color[]
        {
            RetroColorPalette.Darken(RetroColorPalette.Error, 0.7f),
            RetroColorPalette.Darken(RetroColorPalette.Info, 0.7f),
            RetroColorPalette.Darken(RetroColorPalette.Warning, 0.7f)
        };

        for (int i = 0; i < System.Math.Min(competitors.Count, 3); i++)
        {
            var competitor = competitors[i];
            var cardBounds = new Rectangle(30, cardY + (i * (cardHeight + cardSpacing)), 360, cardHeight);

            // Highlight selected competitor
            Color cardColor = (i == this.selectedCompetitorIndex)
                ? RetroColorPalette.Darken(competitorColors[i % 3], 0.9f)
                : competitorColors[i % 3];

            this.DrawFilledRectangle(spriteBatch, cardBounds, cardColor);
            this.Draw3DBorder(spriteBatch, cardBounds, (i == this.selectedCompetitorIndex) ? 3 : 2);

            // Logo area
            var logoBounds = new Rectangle(40, cardY + (i * (cardHeight + cardSpacing)) + 10, 60, 60);
            this.DrawFilledRectangle(spriteBatch, logoBounds, RetroColorPalette.AircraftWhite);
            this.Draw3DBorder(spriteBatch, logoBounds, 1);

            if (AirlineTycoonGame.TextRenderer != null)
            {
                // Airline name (above stats)
                string name = competitor.Airline.Name;
                if (name.Length > 15)
                {
                    name = string.Concat(name.AsSpan(0, 13), "..");
                }
                AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, name, new Vector2(110, cardY + (i * (cardHeight + cardSpacing)) + 12), Color.White);

                // Stats
                int statsX = 110;
                int statsY = cardY + (i * (cardHeight + cardSpacing)) + 30;

                // Fleet size
                string fleetText = $"Fleet: {competitor.Airline.Fleet.Count}";
                AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, fleetText, new Vector2(statsX, statsY), RetroColorPalette.Info);

                // Routes
                string routesText = $"Routes: {competitor.Airline.Routes.Count}";
                AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, routesText, new Vector2(statsX, statsY + 13), RetroColorPalette.Success);

                // Reputation
                string repText = $"Rep: {competitor.Airline.Reputation}";
                AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, repText, new Vector2(statsX, statsY + 26), RetroColorPalette.Warning);

                // Personality indicator (top right of card)
                string personalityCode = competitor.Personality.Type switch
                {
                    AirlineTycoon.Domain.AI.AIPersonalityType.Aggressive => "AGG",
                    AirlineTycoon.Domain.AI.AIPersonalityType.Conservative => "CON",
                    AirlineTycoon.Domain.AI.AIPersonalityType.Budget => "BUD",
                    AirlineTycoon.Domain.AI.AIPersonalityType.Balanced => "BAL",
                    _ => "???"
                };

                var personalityBounds = new Rectangle(320, cardY + (i * (cardHeight + cardSpacing)) + 10, 60, 20);
                this.DrawFilledRectangle(spriteBatch, personalityBounds, RetroColorPalette.ButtonNormal);
                this.Draw3DBorder(spriteBatch, personalityBounds, 1);
                AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, personalityCode, new Vector2(330, cardY + (i * (cardHeight + cardSpacing)) + 13), Color.White);
            }
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

        if (AirlineTycoonGame.TextRenderer != null)
        {
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Market Comparison", new Vector2(440, 117), Color.White);
        }

        // Get actual data
        var player = this.Controller?.Game.PlayerAirline;
        var competitors = this.Controller?.Game.Competitors ?? new List<AirlineTycoon.Domain.AI.CompetitorAirline>();

        if (player == null || competitors.Count == 0)
        {
            return;
        }

        // Bar chart area
        var barChartBounds = new Rectangle(440, 160, 800, 250);
        this.DrawFilledRectangle(spriteBatch, barChartBounds, Color.Black);
        this.Draw3DBorder(spriteBatch, barChartBounds, 1);

        // Calculate metrics
        var allAirlines = new List<(string name, int passengers, int routes, int reputation)>
        {
            (player.Name, player.TotalPassengersCarried, player.Routes.Count, player.Reputation)
        };

        foreach (var comp in competitors.Take(3))
        {
            allAirlines.Add((comp.Airline.Name, comp.Airline.TotalPassengersCarried, comp.Airline.Routes.Count, comp.Airline.Reputation));
        }

        // Find max values for scaling
        int maxPassengers = allAirlines.Max(a => a.passengers);
        int maxRoutes = allAirlines.Max(a => a.routes);

        // Comparative bars (player + competitors)
        int barX = 480;
        int barWidth = 60;
        int barSpacing = 100;
        int maxBarHeight = 200;

        // Bar colors (player is always first)
        Color[] barColors = new Color[]
        {
            RetroColorPalette.Info,        // Player
            RetroColorPalette.Error,       // Competitor 1
            RetroColorPalette.Warning,     // Competitor 2
            RetroColorPalette.Success      // Competitor 3
        };

        // Draw passengers comparison (left set of bars)
        if (AirlineTycoonGame.TextRenderer != null)
        {
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Passengers", new Vector2(490, 150), RetroColorPalette.Info);
        }

        for (int i = 0; i < allAirlines.Count; i++)
        {
            int x = barX + (i * barSpacing);
            int height = maxPassengers > 0 ? (int)((float)allAirlines[i].passengers / maxPassengers * maxBarHeight) : 0;
            height = System.Math.Max(height, 5); // Minimum height for visibility
            int y = 390 - height;

            var bar = new Rectangle(x, y, barWidth, height);
            this.DrawFilledRectangle(spriteBatch, bar, barColors[i]);
            this.Draw3DBorder(spriteBatch, bar, 1);

            // Draw value on top of bar
            if (AirlineTycoonGame.TextRenderer != null)
            {
                string valueText = allAirlines[i].passengers.ToString("N0");
                AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, valueText, new Vector2(x, y - 15), Color.White);
            }
        }

        // Draw routes comparison (right set of bars)
        barX = 850;

        if (AirlineTycoonGame.TextRenderer != null)
        {
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Routes", new Vector2(870, 150), RetroColorPalette.Success);
        }

        for (int i = 0; i < allAirlines.Count; i++)
        {
            int x = barX + (i * barSpacing);
            int height = maxRoutes > 0 ? (int)((float)allAirlines[i].routes / maxRoutes * maxBarHeight) : 0;
            height = System.Math.Max(height, 5); // Minimum height for visibility
            int y = 390 - height;

            var bar = new Rectangle(x, y, barWidth, height);
            this.DrawFilledRectangle(spriteBatch, bar, barColors[i]);
            this.Draw3DBorder(spriteBatch, bar, 1);

            // Draw value on top of bar
            if (AirlineTycoonGame.TextRenderer != null)
            {
                string valueText = allAirlines[i].routes.ToString();
                AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, valueText, new Vector2(x + 20, y - 15), Color.White);
            }
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

        // Get selected competitor
        var competitors = this.Controller?.Game.Competitors ?? new List<AirlineTycoon.Domain.AI.CompetitorAirline>();
        if (competitors.Count == 0 || this.selectedCompetitorIndex >= competitors.Count)
        {
            return;
        }

        var selectedComp = competitors[this.selectedCompetitorIndex];

        // Title
        var titleBounds = new Rectangle(30, 460, 1220, 30);
        this.DrawFilledRectangle(spriteBatch, titleBounds, RetroColorPalette.TitleBarBackground);

        if (AirlineTycoonGame.TextRenderer != null)
        {
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                $"Details: {selectedComp.Airline.Name}",
                new Vector2(40, 467),
                Color.White
            );
        }

        // Competitor routes section
        var routesBounds = new Rectangle(40, 510, 580, 170);
        this.DrawFilledRectangle(spriteBatch, routesBounds, Color.Black);
        this.Draw3DBorder(spriteBatch, routesBounds, 1);

        // Display competitor's routes
        var compRoutes = selectedComp.Airline.Routes.Take(4).ToList();
        var playerRoutes = this.Controller?.Game.PlayerAirline.Routes ?? new List<AirlineTycoon.Domain.Route>();

        if (AirlineTycoonGame.TextRenderer != null)
        {
            if (compRoutes.Count == 0)
            {
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    "No routes",
                    new Vector2(50, 530),
                    RetroColorPalette.TextSecondary
                );
            }
            else
            {
                for (int i = 0; i < compRoutes.Count; i++)
                {
                    var route = compRoutes[i];
                    var routeRow = new Rectangle(50, 520 + (i * 38), 560, 30);
                    this.DrawFilledRectangle(spriteBatch, routeRow, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.7f));

                    // Check if we compete on this route
                    bool weCompete = playerRoutes.Any(r =>
                        (r.Origin.Code == route.Origin.Code && r.Destination.Code == route.Destination.Code) ||
                        (r.Origin.Code == route.Destination.Code && r.Destination.Code == route.Origin.Code)
                    );

                    if (weCompete)
                    {
                        // Overlapping route indicator
                        var overlapIndicator = new Rectangle(55, 525 + (i * 38), 10, 20);
                        this.DrawFilledRectangle(spriteBatch, overlapIndicator, RetroColorPalette.Error);
                    }

                    // Route info
                    string routeText = $"{route.Origin.Code}-{route.Destination.Code}  ${route.TicketPrice:N0}  {route.LoadFactor:P0}";
                    AirlineTycoonGame.TextRenderer.DrawText(
                        spriteBatch,
                        routeText,
                        new Vector2(70, 525 + (i * 38)),
                        weCompete ? RetroColorPalette.Error : Color.White
                    );
                }
            }
        }

        // Financial stats section
        var statsBounds = new Rectangle(640, 510, 600, 170);
        this.DrawFilledRectangle(spriteBatch, statsBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.9f));
        this.Draw3DBorder(spriteBatch, statsBounds, 1);

        if (AirlineTycoonGame.TextRenderer != null)
        {
            // Stats grid (2x2)
            // Stat 1: Cash
            var stat1 = new Rectangle(660, 530, 270, 60);
            this.DrawFilledRectangle(spriteBatch, stat1, RetroColorPalette.Darken(RetroColorPalette.Info, 0.8f));
            this.Draw3DBorder(spriteBatch, stat1, 1);
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Cash", new Vector2(670, 538), Color.White);
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, $"${selectedComp.Airline.Cash:N0}", new Vector2(670, 558), RetroColorPalette.Info);

            // Stat 2: Fleet Size
            var stat2 = new Rectangle(950, 530, 270, 60);
            this.DrawFilledRectangle(spriteBatch, stat2, RetroColorPalette.Darken(RetroColorPalette.Success, 0.8f));
            this.Draw3DBorder(spriteBatch, stat2, 1);
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Fleet Size", new Vector2(960, 538), Color.White);
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, selectedComp.Airline.Fleet.Count.ToString(), new Vector2(960, 558), RetroColorPalette.Success);

            // Stat 3: Total Passengers
            var stat3 = new Rectangle(660, 610, 270, 60);
            this.DrawFilledRectangle(spriteBatch, stat3, RetroColorPalette.Darken(RetroColorPalette.Warning, 0.8f));
            this.Draw3DBorder(spriteBatch, stat3, 1);
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Total Passengers", new Vector2(670, 618), Color.White);
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, selectedComp.Airline.TotalPassengersCarried.ToString("N0"), new Vector2(670, 638), RetroColorPalette.Warning);

            // Stat 4: Reputation
            var stat4 = new Rectangle(950, 610, 270, 60);
            this.DrawFilledRectangle(spriteBatch, stat4, RetroColorPalette.Darken(RetroColorPalette.ButtonNormal, 0.8f));
            this.Draw3DBorder(spriteBatch, stat4, 1);
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Reputation", new Vector2(960, 618), Color.White);
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, selectedComp.Airline.Reputation.ToString(), new Vector2(960, 638), Color.White);
        }
    }

    /// <summary>
    /// Handles the back button click.
    /// </summary>
    private void OnBack()
    {
        this.Controller?.ShowDashboard();
    }
}
