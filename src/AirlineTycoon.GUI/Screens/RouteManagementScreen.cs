using AirlineTycoon.GUI.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

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
    private List<UIButton> assignButtons = new();
    private List<UIButton> priceButtons = new();
    private List<UIButton> frequencyButtons = new();
    private bool needsButtonRebuild = true;

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
            "< Back",
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
    public override void Update(GameTime gameTime)
    {
        if (this.IsVisible && this.needsButtonRebuild)
        {
            this.RebuildRouteButtons();
            this.needsButtonRebuild = false;
        }

        base.Update(gameTime);
    }

    /// <summary>
    /// Rebuilds the route action buttons (assign/unassign/close/price adjustment).
    /// </summary>
    private void RebuildRouteButtons()
    {
        // Get routes and fleet
        var routes = this.Controller?.Game.PlayerAirline.Routes.ToList() ?? new List<AirlineTycoon.Domain.Route>();
        var fleet = this.Controller?.Game.PlayerAirline.Fleet.ToList() ?? new List<AirlineTycoon.Domain.Aircraft>();

        // Clear old buttons
        foreach (var btn in this.assignButtons)
        {
            this.RemoveChild(btn);
        }
        this.assignButtons.Clear();

        foreach (var btn in this.priceButtons)
        {
            this.RemoveChild(btn);
        }
        this.priceButtons.Clear();

        foreach (var btn in this.frequencyButtons)
        {
            this.RemoveChild(btn);
        }
        this.frequencyButtons.Clear();

        if (routes.Count == 0)
        {
            return;
        }

        // Route button positioning
        int rowY = 150;
        int rowSpacing = 40;

        for (int i = 0; i < routes.Count; i++)
        {
            var route = routes[i];

            // Add price decrease button (-)
            var decreasePriceButton = new UIButton(
                "-",
                new Vector2(305, rowY + (i * rowSpacing) + 5),
                new Vector2(20, 25)
            );
            var capturedRouteDecrease = route;
            decreasePriceButton.Clicked += (s, e) => this.OnDecreasePrice(capturedRouteDecrease);
            this.AddChild(decreasePriceButton);
            this.priceButtons.Add(decreasePriceButton);

            // Add price increase button (+)
            var increasePriceButton = new UIButton(
                "+",
                new Vector2(330, rowY + (i * rowSpacing) + 5),
                new Vector2(20, 25)
            );
            var capturedRouteIncrease = route;
            increasePriceButton.Clicked += (s, e) => this.OnIncreasePrice(capturedRouteIncrease);
            this.AddChild(increasePriceButton);
            this.priceButtons.Add(increasePriceButton);

            // Add frequency decrease button (-)
            var decreaseFrequencyButton = new UIButton(
                "-",
                new Vector2(375, rowY + (i * rowSpacing) + 5),
                new Vector2(20, 25)
            );
            var capturedRouteFreqDecrease = route;
            decreaseFrequencyButton.Clicked += (s, e) => this.OnDecreaseFrequency(capturedRouteFreqDecrease);
            this.AddChild(decreaseFrequencyButton);
            this.frequencyButtons.Add(decreaseFrequencyButton);

            // Add frequency increase button (+)
            var increaseFrequencyButton = new UIButton(
                "+",
                new Vector2(400, rowY + (i * rowSpacing) + 5),
                new Vector2(20, 25)
            );
            var capturedRouteFreqIncrease = route;
            increaseFrequencyButton.Clicked += (s, e) => this.OnIncreaseFrequency(capturedRouteFreqIncrease);
            this.AddChild(increaseFrequencyButton);
            this.frequencyButtons.Add(increaseFrequencyButton);

            // Add assign/unassign button
            if (route.AssignedAircraft == null)
            {
                // Get available (unassigned) aircraft
                var availableAircraft = fleet.Where(a =>
                    !routes.Any(r => r.AssignedAircraft?.RegistrationNumber == a.RegistrationNumber)
                ).ToList();

                if (availableAircraft.Count > 0)
                {
                    var assignButton = new UIButton(
                        "Assign",
                        new Vector2(675, rowY + (i * rowSpacing) + 5),
                        new Vector2(60, 25)
                    );

                    // Capture variables for lambda
                    var capturedRoute = route;
                    var capturedAvailable = availableAircraft;

                    assignButton.Clicked += (s, e) => this.OnAssignAircraft(capturedRoute, capturedAvailable);
                    this.AddChild(assignButton);
                    this.assignButtons.Add(assignButton);
                }
            }
            else
            {
                // Unassign button
                var unassignButton = new UIButton(
                    "X",
                    new Vector2(675, rowY + (i * rowSpacing) + 5),
                    new Vector2(25, 25)
                );

                var capturedRoute = route;
                unassignButton.Clicked += (s, e) => this.OnUnassignAircraft(capturedRoute);
                this.AddChild(unassignButton);
                this.assignButtons.Add(unassignButton);
            }

            // Add close route button
            var closeButton = new UIButton(
                "Close",
                new Vector2(760, rowY + (i * rowSpacing) + 5),
                new Vector2(50, 25)
            );

            var capturedRouteForClose = route;
            closeButton.Clicked += (s, e) => this.OnCloseRoute(capturedRouteForClose);
            this.AddChild(closeButton);
            this.assignButtons.Add(closeButton);
        }
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

        if (AirlineTycoonGame.TextRenderer != null)
        {
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Route", new Vector2(55, 117), Color.White);
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Price", new Vector2(260, 117), Color.White);
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Freq", new Vector2(360, 117), Color.White);
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Load", new Vector2(430, 117), Color.White);
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Profit/Day", new Vector2(490, 117), Color.White);
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Aircraft", new Vector2(600, 117), Color.White);
            AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, "Status", new Vector2(710, 117), Color.White);
        }

        // Get actual routes
        var routes = this.Controller?.Game.PlayerAirline.Routes.ToList() ?? new List<AirlineTycoon.Domain.Route>();

        if (routes.Count == 0)
        {
            // Show "No routes" message
            if (AirlineTycoonGame.TextRenderer != null)
            {
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    "No routes. Click '+ Open Route' to add your first route.",
                    new Vector2(40, 160),
                    RetroColorPalette.TextSecondary
                );
            }
            return;
        }

        // Route rows
        int rowY = 150;
        int rowHeight = 35;
        int rowSpacing = 40;

        for (int i = 0; i < routes.Count; i++)
        {
            var route = routes[i];
            var rowBounds = new Rectangle(30, rowY + (i * rowSpacing), 760, rowHeight);

            // Alternate row colors for readability
            Color rowColor = i % 2 == 0
                ? RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.9f)
                : RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.7f);

            this.DrawFilledRectangle(spriteBatch, rowBounds, rowColor);

            // Profit indicator (left edge - green for profit, red for loss)
            Color profitColor = route.DailyProfit >= 0 ? RetroColorPalette.Success : RetroColorPalette.Error;
            var profitIndicator = new Rectangle(35, rowY + (i * rowSpacing) + 5, 10, rowHeight - 10);
            this.DrawFilledRectangle(spriteBatch, profitIndicator, profitColor);

            if (AirlineTycoonGame.TextRenderer != null)
            {
                int textY = rowY + (i * rowSpacing) + 10;

                // Route name (Origin → Destination)
                string routeName = $"{route.Origin.Code} -> {route.Destination.Code}";
                AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, routeName, new Vector2(55, textY), Color.White);

                // Ticket price
                AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, $"${route.TicketPrice:N0}", new Vector2(260, textY), RetroColorPalette.TextSecondary);

                // Frequency (daily flights)
                AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, $"{route.DailyFlights}x", new Vector2(360, textY), Color.White);

                // Load factor
                Color loadColor = route.LoadFactor > 0.8 ? RetroColorPalette.Success :
                                 route.LoadFactor > 0.6 ? RetroColorPalette.Warning :
                                 RetroColorPalette.Error;
                AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, $"{route.LoadFactor:P0}", new Vector2(430, textY), loadColor);

                // Daily profit
                Color profitTextColor = route.DailyProfit >= 0 ? RetroColorPalette.Success : RetroColorPalette.Error;
                string profitText = route.DailyProfit >= 0 ? $"+${route.DailyProfit:N0}" : $"-${Math.Abs(route.DailyProfit):N0}";
                AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, profitText, new Vector2(490, textY), profitTextColor);

                // Assigned aircraft
                string aircraftText = route.AssignedAircraft != null
                    ? route.AssignedAircraft.RegistrationNumber
                    : "UNASSIGNED";
                Color aircraftColor = route.AssignedAircraft != null ? RetroColorPalette.TextSecondary : RetroColorPalette.Warning;
                AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, aircraftText, new Vector2(600, textY), aircraftColor);

                // Status
                string statusText = route.IsActive ? "Active" : "Closed";
                Color statusColor = route.IsActive ? RetroColorPalette.Success : RetroColorPalette.Error;
                AirlineTycoonGame.TextRenderer.DrawText(spriteBatch, statusText, new Vector2(710, textY), statusColor);
            }
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

        // Route visualization area (mini map showing origin and destination)
        var mapBounds = new Rectangle(840, 240, 400, 200);
        this.DrawFilledRectangle(spriteBatch, mapBounds, new Color(20, 30, 40)); // Dark blue background
        this.Draw3DBorder(spriteBatch, mapBounds, 1);

        // Get the first route for visualization (simplified)
        var routes = this.Controller?.Game.PlayerAirline.Routes.ToList() ?? new List<AirlineTycoon.Domain.Route>();
        if (routes.Count > 0 && AirlineTycoonGame.SpriteManager != null)
        {
            var route = routes[0]; // Show first route for now

            // Calculate positions for origin and destination airports
            int centerX = mapBounds.X + mapBounds.Width / 2;
            int centerY = mapBounds.Y + mapBounds.Height / 2;

            // Origin on left, destination on right
            var originPos = new Vector2(mapBounds.X + 80, centerY);
            var destPos = new Vector2(mapBounds.X + mapBounds.Width - 80, centerY);

            // Draw flight path line
            this.DrawRouteLine(spriteBatch, originPos, destPos, route.IsActive ? RetroColorPalette.Success : RetroColorPalette.TextSecondary);

            // Draw airport icons
            var hubIcon = AirlineTycoonGame.SpriteManager.GetSprite("airport_hub");
            if (hubIcon != null)
            {
                // Draw origin airport
                spriteBatch.Draw(
                    hubIcon,
                    new Vector2(originPos.X - hubIcon.Width / 2, originPos.Y - hubIcon.Height / 2),
                    Color.White
                );

                // Draw destination airport
                spriteBatch.Draw(
                    hubIcon,
                    new Vector2(destPos.X - hubIcon.Width / 2, destPos.Y - hubIcon.Height / 2),
                    Color.White
                );
            }

            // Draw airport codes
            if (AirlineTycoonGame.TextRenderer != null)
            {
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    route.Origin.Code,
                    new Vector2(originPos.X - 15, originPos.Y + 20),
                    Color.White
                );

                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    route.Destination.Code,
                    new Vector2(destPos.X - 15, destPos.Y + 20),
                    Color.White
                );
            }
        }

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
        this.Controller?.ShowOpenRoute();
    }

    /// <summary>
    /// Handles assigning an aircraft to a route.
    /// Cycles through available aircraft on each click for simplicity.
    /// </summary>
    private void OnAssignAircraft(AirlineTycoon.Domain.Route route, List<AirlineTycoon.Domain.Aircraft> availableAircraft)
    {
        System.Diagnostics.Debug.WriteLine("OnAssignAircraft called!");

        if (this.Controller == null || availableAircraft.Count == 0)
        {
            System.Diagnostics.Debug.WriteLine($"OnAssignAircraft: Controller null or no available aircraft. Controller={this.Controller != null}, Available={availableAircraft.Count}");
            return;
        }

        // For now, just assign the first available aircraft
        // TODO: Show a proper selection dialog
        var aircraft = availableAircraft[0];

        System.Diagnostics.Debug.WriteLine($"Attempting to assign aircraft {aircraft.RegistrationNumber} to route {route.Name}");
        bool success = this.Controller.AssignAircraftToRoute(route.Id, aircraft.RegistrationNumber);

        if (!success)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to assign aircraft {aircraft.RegistrationNumber} to route {route.Name}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"Successfully assigned aircraft {aircraft.RegistrationNumber} to route {route.Name}");
            this.needsButtonRebuild = true; // Rebuild buttons to show new assignment
        }
    }

    /// <summary>
    /// Handles unassigning an aircraft from a route.
    /// </summary>
    private void OnUnassignAircraft(AirlineTycoon.Domain.Route route)
    {
        if (this.Controller == null)
        {
            return;
        }

        bool success = this.Controller.UnassignAircraftFromRoute(route.Id);

        if (!success)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to unassign aircraft from route {route.Name}");
        }
        else
        {
            this.needsButtonRebuild = true; // Rebuild buttons to show unassignment
        }
    }

    /// <summary>
    /// Handles closing a route.
    /// </summary>
    private void OnCloseRoute(AirlineTycoon.Domain.Route route)
    {
        if (this.Controller == null)
        {
            return;
        }

        // TODO: Add confirmation dialog in future
        // For now, close immediately
        bool success = this.Controller.CloseRoute(route.Origin.Code, route.Destination.Code);

        if (!success)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to close route {route.Name}");
        }
        else
        {
            this.needsButtonRebuild = true; // Rebuild buttons after route closure
        }
    }

    /// <summary>
    /// Handles increasing the ticket price for a route.
    /// Increases price by $10 increments.
    /// </summary>
    private void OnIncreasePrice(AirlineTycoon.Domain.Route route)
    {
        // Increase ticket price by $10
        route.TicketPrice += 10m;
        System.Diagnostics.Debug.WriteLine($"Increased ticket price for {route.Name} to ${route.TicketPrice:N0}");
    }

    /// <summary>
    /// Handles decreasing the ticket price for a route.
    /// Decreases price by $10 increments, minimum $10.
    /// </summary>
    private void OnDecreasePrice(AirlineTycoon.Domain.Route route)
    {
        // Decrease ticket price by $10, but don't go below $10
        if (route.TicketPrice > 10m)
        {
            route.TicketPrice -= 10m;
            System.Diagnostics.Debug.WriteLine($"Decreased ticket price for {route.Name} to ${route.TicketPrice:N0}");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"Cannot decrease price below $10 for {route.Name}");
        }
    }

    /// <summary>
    /// Handles increasing the frequency (daily flights) for a route.
    /// </summary>
    private void OnIncreaseFrequency(AirlineTycoon.Domain.Route route)
    {
        // Increase daily flights by 1, with a reasonable maximum (e.g., 10 flights per day)
        if (route.DailyFlights < 10)
        {
            route.DailyFlights++;
            System.Diagnostics.Debug.WriteLine($"Increased daily flights for {route.Name} to {route.DailyFlights}x");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"Cannot increase frequency above 10 flights/day for {route.Name}");
        }
    }

    /// <summary>
    /// Handles decreasing the frequency (daily flights) for a route.
    /// </summary>
    private void OnDecreaseFrequency(AirlineTycoon.Domain.Route route)
    {
        // Decrease daily flights by 1, but don't go below 1
        if (route.DailyFlights > 1)
        {
            route.DailyFlights--;
            System.Diagnostics.Debug.WriteLine($"Decreased daily flights for {route.Name} to {route.DailyFlights}x");
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"Cannot decrease frequency below 1 flight/day for {route.Name}");
        }
    }

    /// <summary>
    /// Draws a route line connecting two airports on the map.
    /// Uses a simple dashed line effect for visual interest.
    /// </summary>
    /// <param name="spriteBatch">The sprite batch for drawing.</param>
    /// <param name="start">Starting position (origin airport).</param>
    /// <param name="end">Ending position (destination airport).</param>
    /// <param name="color">Color of the route line.</param>
    private void DrawRouteLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
    {
        if (AirlineTycoonGame.WhitePixel == null)
        {
            return;
        }

        // Calculate line parameters
        float distance = Vector2.Distance(start, end);
        float angle = (float)System.Math.Atan2(end.Y - start.Y, end.X - start.X);
        int thickness = 2;

        // Draw dashed line (segments with gaps)
        int dashLength = 10;
        int gapLength = 5;
        int totalSegmentLength = dashLength + gapLength;
        int numSegments = (int)(distance / totalSegmentLength);

        for (int i = 0; i < numSegments; i++)
        {
            float segmentStart = i * totalSegmentLength;
            float segmentX = start.X + segmentStart * (float)System.Math.Cos(angle);
            float segmentY = start.Y + segmentStart * (float)System.Math.Sin(angle);

            spriteBatch.Draw(
                AirlineTycoonGame.WhitePixel,
                new Rectangle((int)segmentX, (int)segmentY, dashLength, thickness),
                null,
                color,
                angle,
                Vector2.Zero,
                SpriteEffects.None,
                0
            );
        }
    }
}
