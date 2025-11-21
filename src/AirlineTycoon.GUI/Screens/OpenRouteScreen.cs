using AirlineTycoon.Domain;
using AirlineTycoon.GUI.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace AirlineTycoon.GUI.Screens;

/// <summary>
/// Screen for opening new routes.
/// Allows player to select origin/destination airports and set ticket price.
/// </summary>
public class OpenRouteScreen : Screen
{
    private UIButton? backButton;
    private UIButton? confirmButton;
    private List<UIButton> originButtons = new();
    private List<UIButton> destinationButtons = new();

    private Airport? selectedOrigin;
    private Airport? selectedDestination;
    private decimal ticketPrice = 150m; // Default suggested price

    /// <inheritdoc/>
    public override string Title => "Open New Route";

    /// <summary>
    /// Initializes a new instance of the <see cref="OpenRouteScreen"/> class.
    /// </summary>
    /// <param name="controller">The game controller.</param>
    public OpenRouteScreen(GameController controller)
    {
        this.SetGameController(controller);
        this.InitializeUI();
    }

    /// <summary>
    /// Initializes the UI elements.
    /// </summary>
    private void InitializeUI()
    {
        // Back button
        this.backButton = new UIButton(
            "< Back",
            new Vector2(20, 50),
            new Vector2(120, 35)
        );
        this.backButton.Clicked += (s, e) => this.OnBack();
        this.AddChild(this.backButton);

        // Confirm button (bottom right)
        this.confirmButton = new UIButton(
            "Confirm Route",
            new Vector2(AirlineTycoonGame.BaseWidth - 200, 650),
            new Vector2(180, 40)
        );
        this.confirmButton.Clicked += (s, e) => this.OnConfirm();
        this.AddChild(this.confirmButton);
    }

    /// <inheritdoc/>
    public override void Update(GameTime gameTime)
    {
        // Rebuild airport buttons when screen becomes visible
        if (this.IsVisible && this.originButtons.Count == 0)
        {
            this.RebuildAirportButtons();
        }

        base.Update(gameTime);
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

        // Draw airport selection panels
        this.DrawAirportPanels(spriteBatch);

        // Draw route preview
        this.DrawRoutePreview(spriteBatch);

        // Draw buttons
        base.Draw(spriteBatch, gameTime);
    }

    /// <summary>
    /// Rebuilds the airport selection buttons.
    /// </summary>
    private void RebuildAirportButtons()
    {
        // Clear old buttons
        foreach (var btn in this.originButtons)
        {
            this.RemoveChild(btn);
        }
        foreach (var btn in this.destinationButtons)
        {
            this.RemoveChild(btn);
        }
        this.originButtons.Clear();
        this.destinationButtons.Clear();

        var airports = Airport.Catalog.All.ToList();
        int buttonHeight = 30;
        int spacing = 5;

        // Origin buttons (left panel)
        for (int i = 0; i < airports.Count; i++)
        {
            var airport = airports[i];
            var btn = new UIButton(
                $"{airport.Code} - {airport.City}",
                new Vector2(40, 160 + (i * (buttonHeight + spacing))),
                new Vector2(260, buttonHeight)
            );

            var capturedAirport = airport;
            btn.Clicked += (s, e) => this.OnOriginSelected(capturedAirport);
            this.AddChild(btn);
            this.originButtons.Add(btn);
        }

        // Destination buttons (right panel)
        for (int i = 0; i < airports.Count; i++)
        {
            var airport = airports[i];
            var btn = new UIButton(
                $"{airport.Code} - {airport.City}",
                new Vector2(340, 160 + (i * (buttonHeight + spacing))),
                new Vector2(260, buttonHeight)
            );

            var capturedAirport = airport;
            btn.Clicked += (s, e) => this.OnDestinationSelected(capturedAirport);
            this.AddChild(btn);
            this.destinationButtons.Add(btn);
        }
    }

    /// <summary>
    /// Draws the airport selection panels.
    /// </summary>
    private void DrawAirportPanels(SpriteBatch spriteBatch)
    {
        // Origin panel
        var originPanel = new Rectangle(30, 100, 280, 550);
        this.DrawFilledRectangle(spriteBatch, originPanel, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.8f));
        this.Draw3DBorder(spriteBatch, originPanel);

        var originHeader = new Rectangle(40, 110, 260, 30);
        this.DrawFilledRectangle(spriteBatch, originHeader, RetroColorPalette.TitleBarBackground);

        if (AirlineTycoonGame.TextRenderer != null)
        {
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                "Origin Airport",
                new Vector2(50, 117),
                Color.White
            );

            // Show selected origin
            if (this.selectedOrigin != null)
            {
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    $"Selected: {this.selectedOrigin.Code}",
                    new Vector2(50, 145),
                    RetroColorPalette.Success
                );
            }
        }

        // Destination panel
        var destPanel = new Rectangle(330, 100, 280, 550);
        this.DrawFilledRectangle(spriteBatch, destPanel, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.8f));
        this.Draw3DBorder(spriteBatch, destPanel);

        var destHeader = new Rectangle(340, 110, 260, 30);
        this.DrawFilledRectangle(spriteBatch, destHeader, RetroColorPalette.TitleBarBackground);

        if (AirlineTycoonGame.TextRenderer != null)
        {
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                "Destination Airport",
                new Vector2(350, 117),
                Color.White
            );

            // Show selected destination
            if (this.selectedDestination != null)
            {
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    $"Selected: {this.selectedDestination.Code}",
                    new Vector2(350, 145),
                    RetroColorPalette.Success
                );
            }
        }
    }

    /// <summary>
    /// Draws the route preview panel.
    /// </summary>
    private void DrawRoutePreview(SpriteBatch spriteBatch)
    {
        var previewPanel = new Rectangle(630, 100, 630, 550);
        this.DrawFilledRectangle(spriteBatch, previewPanel, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.8f));
        this.Draw3DBorder(spriteBatch, previewPanel);

        var previewHeader = new Rectangle(640, 110, 610, 30);
        this.DrawFilledRectangle(spriteBatch, previewHeader, RetroColorPalette.TitleBarBackground);

        if (AirlineTycoonGame.TextRenderer != null)
        {
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                "Route Preview",
                new Vector2(650, 117),
                Color.White
            );

            if (this.selectedOrigin != null && this.selectedDestination != null)
            {
                int textY = 160;
                int lineHeight = 25;

                // Route name
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    $"{this.selectedOrigin.Code} -> {this.selectedDestination.Code}",
                    new Vector2(650, textY),
                    Color.White
                );
                textY += lineHeight;

                // Distance (calculate from airport data)
                var distance = this.CalculateDistance(this.selectedOrigin, this.selectedDestination);
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    $"Distance: {distance:N0} miles",
                    new Vector2(650, textY),
                    RetroColorPalette.TextSecondary
                );
                textY += lineHeight;

                // Suggested ticket price
                var suggestedPrice = this.CalculateSuggestedPrice(distance);
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    $"Suggested Price: ${suggestedPrice:N0}",
                    new Vector2(650, textY),
                    RetroColorPalette.TextSecondary
                );
                textY += lineHeight + 10;

                // Ticket price label
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    $"Ticket Price: ${this.ticketPrice:N0}",
                    new Vector2(650, textY),
                    RetroColorPalette.Success
                );
                textY += lineHeight + 10;

                // Instructions
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    "Press + to increase price",
                    new Vector2(650, textY),
                    RetroColorPalette.TextSecondary
                );
                textY += 20;
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    "Press - to decrease price",
                    new Vector2(650, textY),
                    RetroColorPalette.TextSecondary
                );
            }
            else
            {
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    "Select origin and destination airports",
                    new Vector2(650, 160),
                    RetroColorPalette.TextSecondary
                );
            }
        }
    }

    /// <summary>
    /// Handles origin airport selection.
    /// </summary>
    private void OnOriginSelected(Airport airport)
    {
        this.selectedOrigin = airport;
        if (this.selectedOrigin != null && this.selectedDestination != null)
        {
            var distance = this.CalculateDistance(this.selectedOrigin, this.selectedDestination);
            this.ticketPrice = this.CalculateSuggestedPrice(distance);
        }
    }

    /// <summary>
    /// Handles destination airport selection.
    /// </summary>
    private void OnDestinationSelected(Airport airport)
    {
        this.selectedDestination = airport;
        if (this.selectedOrigin != null && this.selectedDestination != null)
        {
            var distance = this.CalculateDistance(this.selectedOrigin, this.selectedDestination);
            this.ticketPrice = this.CalculateSuggestedPrice(distance);
        }
    }

    /// <summary>
    /// Calculates distance between two airports.
    /// Uses the domain's distance calculation method.
    /// </summary>
    private int CalculateDistance(Airport origin, Airport destination)
    {
        return AirlineTycoon.Domain.Route.CalculateDistance(origin.Code, destination.Code);
    }

    /// <summary>
    /// Calculates suggested ticket price based on distance.
    /// </summary>
    private decimal CalculateSuggestedPrice(int distance)
    {
        // Base fare + per-mile rate
        return 50m + (distance * 0.15m);
    }

    /// <summary>
    /// Handles the confirm button click.
    /// </summary>
    private void OnConfirm()
    {
        if (this.Controller == null || this.selectedOrigin == null || this.selectedDestination == null)
        {
            System.Diagnostics.Debug.WriteLine("Cannot open route: missing origin, destination, or controller");
            return;
        }

        if (this.selectedOrigin.Code == this.selectedDestination.Code)
        {
            System.Diagnostics.Debug.WriteLine("Cannot open route: origin and destination are the same");
            return;
        }

        bool success = this.Controller.OpenRoute(this.selectedOrigin, this.selectedDestination, this.ticketPrice);

        if (success)
        {
            // Return to route management screen
            this.Controller.ShowRouteManagement();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("Failed to open route - route may already exist");
        }
    }

    /// <summary>
    /// Handles the back button click.
    /// </summary>
    private void OnBack()
    {
        this.Controller?.ShowRouteManagement();
    }
}
