using AirlineTycoon.GUI.UI;
using AirlineTycoon.Domain;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace AirlineTycoon.GUI.Screens;

/// <summary>
/// Screen for purchasing or leasing aircraft.
/// Displays available aircraft types with specs and prices.
/// </summary>
public class AircraftPurchaseScreen : Screen
{
    private UIButton? backButton;
    private readonly bool isLease;
    private List<UIButton> purchaseButtons = new();

    /// <inheritdoc/>
    public override string Title => this.isLease ? "Lease Aircraft" : "Buy Aircraft";

    /// <summary>
    /// Initializes a new instance of the <see cref="AircraftPurchaseScreen"/> class.
    /// </summary>
    /// <param name="controller">The game controller.</param>
    /// <param name="isLease">True for leasing, false for purchasing.</param>
    public AircraftPurchaseScreen(GameController controller, bool isLease = false)
    {
        this.isLease = isLease;
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

        // Create aircraft cards
        var aircraftTypes = new List<AircraftType>
        {
            AircraftType.Catalog.EmbraerE175,
            AircraftType.Catalog.Boeing737,
            AircraftType.Catalog.AirbusA320,
            AircraftType.Catalog.Boeing787,
            AircraftType.Catalog.AirbusA380
        };
        int cardWidth = 370;
        int cardHeight = 200;
        int cardsPerRow = 3;
        int spacing = 20;
        int startX = 40;
        int startY = 110;

        for (int i = 0; i < aircraftTypes.Count; i++)
        {
            var aircraftType = aircraftTypes[i];
            int row = i / cardsPerRow;
            int col = i % cardsPerRow;
            int x = startX + (col * (cardWidth + spacing));
            int y = startY + (row * (cardHeight + spacing));

            // Store position for drawing
            // We'll draw these manually in Draw() method
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

        // Draw aircraft cards
        this.DrawAircraftCards(spriteBatch);

        // Draw buttons
        base.Draw(spriteBatch, gameTime);
    }

    /// <summary>
    /// Draws aircraft selection cards.
    /// </summary>
    private void DrawAircraftCards(SpriteBatch spriteBatch)
    {
        var aircraftTypes = new List<AircraftType>
        {
            AircraftType.Catalog.EmbraerE175,
            AircraftType.Catalog.Boeing737,
            AircraftType.Catalog.AirbusA320,
            AircraftType.Catalog.Boeing787,
            AircraftType.Catalog.AirbusA380
        };
        int cardWidth = 370;
        int cardHeight = 200;
        int cardsPerRow = 3;
        int spacing = 20;
        int startX = 40;
        int startY = 110;

        var playerCash = this.Controller?.Game.PlayerAirline.Cash ?? 0;

        // Clear old buttons
        foreach (var btn in this.purchaseButtons)
        {
            this.RemoveChild(btn);
        }
        this.purchaseButtons.Clear();

        for (int i = 0; i < aircraftTypes.Count; i++)
        {
            var aircraftType = aircraftTypes[i];
            int row = i / cardsPerRow;
            int col = i % cardsPerRow;
            int x = startX + (col * (cardWidth + spacing));
            int y = startY + (row * (cardHeight + spacing));

            var cardBounds = new Rectangle(x, y, cardWidth, cardHeight);

            // Check if player can afford
            // For lease, monthly payment is 1.2% of purchase price
            decimal price = this.isLease ? (aircraftType.PurchasePrice * 0.012m) : aircraftType.PurchasePrice;
            bool canAfford = playerCash >= price;

            // Card background
            Color cardColor = canAfford
                ? RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.9f)
                : RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.7f);
            this.DrawFilledRectangle(spriteBatch, cardBounds, cardColor);
            this.Draw3DBorder(spriteBatch, cardBounds, 2);

            // Aircraft name
            if (AirlineTycoonGame.TextRenderer != null)
            {
                var namePos = new Vector2(x + 10, y + 10);
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    aircraftType.Name,
                    namePos,
                    Color.White
                );

                // Category
                var categoryPos = new Vector2(x + 10, y + 30);
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    $"Category: {aircraftType.Category}",
                    categoryPos,
                    RetroColorPalette.TextSecondary
                );

                // Specs
                int specY = y + 55;
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    $"Capacity: {aircraftType.Capacity} passengers",
                    new Vector2(x + 10, specY),
                    RetroColorPalette.TextSecondary
                );

                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    $"Range: {aircraftType.Range:N0} miles",
                    new Vector2(x + 10, specY + 20),
                    RetroColorPalette.TextSecondary
                );

                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    $"Operating Cost: ${aircraftType.OperatingCostPerHour:N0}/hr",
                    new Vector2(x + 10, specY + 40),
                    RetroColorPalette.TextSecondary
                );

                // Price
                string priceText = this.isLease
                    ? $"Lease: ${price:N0}/month"
                    : $"Buy: ${price:N0}";
                Color priceColor = canAfford ? RetroColorPalette.Success : RetroColorPalette.Error;
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    priceText,
                    new Vector2(x + 10, y + cardHeight - 55),
                    priceColor
                );
            }

            // Buy/Lease button (only add if player can afford)
            if (canAfford)
            {
                string buttonText = this.isLease ? "Lease" : "Buy";
                var buyButton = new UIButton(
                    buttonText,
                    new Vector2(x + cardWidth - 130, y + cardHeight - 45),
                    new Vector2(120, 35)
                );

                // Capture the aircraft type in a local variable for the lambda
                var capturedType = aircraftType;
                buyButton.Clicked += (s, e) => this.OnPurchaseAircraft(capturedType);

                this.AddChild(buyButton);
                this.purchaseButtons.Add(buyButton);
            }
            else
            {
                // Draw "Can't Afford" text
                if (AirlineTycoonGame.TextRenderer != null)
                {
                    AirlineTycoonGame.TextRenderer.DrawText(
                        spriteBatch,
                        "Insufficient Funds",
                        new Vector2(x + cardWidth - 150, y + cardHeight - 35),
                        RetroColorPalette.Error
                    );
                }
            }
        }
    }

    /// <summary>
    /// Handles aircraft purchase/lease.
    /// </summary>
    private void OnPurchaseAircraft(AircraftType aircraftType)
    {
        System.Diagnostics.Debug.WriteLine($"OnPurchaseAircraft called: isLease={this.isLease}, aircraft={aircraftType.Name}");

        if (this.Controller == null)
        {
            System.Diagnostics.Debug.WriteLine("Controller is null!");
            return;
        }

        bool success = this.isLease
            ? this.Controller.LeaseAircraft(aircraftType)
            : this.Controller.PurchaseAircraft(aircraftType);

        System.Diagnostics.Debug.WriteLine($"Operation result: success={success}");

        if (success)
        {
            // Return to fleet management screen
            this.Controller.ShowFleetManagement();
        }
        else
        {
            // TODO: Show error message
            System.Diagnostics.Debug.WriteLine($"Failed to {(this.isLease ? "lease" : "purchase")} aircraft");
        }
    }

    /// <summary>
    /// Handles the back button click.
    /// </summary>
    private void OnBack()
    {
        this.Controller?.ShowFleetManagement();
    }
}
