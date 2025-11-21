using AirlineTycoon.GUI.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Linq;

namespace AirlineTycoon.GUI.Screens;

/// <summary>
/// Fleet management screen showing all aircraft and their status.
/// Similar to RCT's ride list but for aircraft.
/// </summary>
/// <remarks>
/// Displays:
/// - Grid/list of all owned and leased aircraft
/// - Aircraft condition bars (like RCT's excitement/intensity/nausea ratings)
/// - Assignment status (which route each plane is on)
/// - Maintenance scheduling
/// - Buy/lease buttons with aircraft previews
///
/// This is where players manage their most valuable assets - the planes.
/// </remarks>
public class FleetManagementScreen : Screen
{
    private UIButton? backButton;
    private UIButton? buyAircraftButton;
    private UIButton? leaseAircraftButton;
    private List<UIButton> maintenanceButtons = new();

    /// <inheritdoc/>
    public override string Title => "Fleet Management";

    /// <summary>
    /// Initializes a new instance of the <see cref="FleetManagementScreen"/> class.
    /// </summary>
    /// <param name="controller">The game controller.</param>
    public FleetManagementScreen(GameController controller)
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

        // Buy Aircraft button
        this.buyAircraftButton = new UIButton(
            "Buy Aircraft",
            new Vector2(AirlineTycoonGame.BaseWidth - 320, 50),
            new Vector2(140, 35)
        );
        this.buyAircraftButton.Clicked += (s, e) => this.OnBuyAircraft();
        this.AddChild(this.buyAircraftButton);

        // Lease Aircraft button
        this.leaseAircraftButton = new UIButton(
            "Lease Aircraft",
            new Vector2(AirlineTycoonGame.BaseWidth - 170, 50),
            new Vector2(150, 35)
        );
        this.leaseAircraftButton.Clicked += (s, e) => this.OnLeaseAircraft();
        this.AddChild(this.leaseAircraftButton);
    }

    /// <inheritdoc/>
    public override void Update(GameTime gameTime)
    {
        if (this.IsVisible)
        {
            // Rebuild maintenance buttons if fleet changes
            this.RebuildMaintenanceButtons();
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

        // Fleet list
        this.DrawFleetList(spriteBatch);

        // Aircraft details panel
        this.DrawAircraftDetails(spriteBatch);

        // Draw buttons
        base.Draw(spriteBatch, gameTime);
    }

    /// <summary>
    /// Draws the fleet list with aircraft cards.
    /// </summary>
    private void DrawFleetList(SpriteBatch spriteBatch)
    {
        // List panel
        var listBounds = new Rectangle(20, 100, 780, 600);
        this.DrawFilledRectangle(spriteBatch, listBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.8f));
        this.Draw3DBorder(spriteBatch, listBounds);

        // Title
        var titleBounds = new Rectangle(30, 110, 760, 30);
        this.DrawFilledRectangle(spriteBatch, titleBounds, RetroColorPalette.TitleBarBackground);

        if (AirlineTycoonGame.TextRenderer != null)
        {
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                "Your Fleet",
                new Vector2(40, 117),
                Color.White
            );
        }

        // Get actual fleet data
        var fleet = this.Controller?.Game.PlayerAirline.Fleet.ToList() ?? new List<AirlineTycoon.Domain.Aircraft>();

        if (fleet.Count == 0)
        {
            // Show "No aircraft" message
            if (AirlineTycoonGame.TextRenderer != null)
            {
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    "No aircraft in your fleet. Click 'Buy Aircraft' or 'Lease Aircraft' to add planes.",
                    new Vector2(40, 160),
                    RetroColorPalette.TextSecondary
                );
            }
            return;
        }

        // Aircraft grid (2 columns)
        int cardWidth = 360;
        int cardHeight = 120;
        int cardSpacing = 20;
        int startX = 40;
        int startY = 160;
        int cardsPerRow = 2;

        for (int i = 0; i < fleet.Count; i++)
        {
            var aircraft = fleet[i];
            int row = i / cardsPerRow;
            int col = i % cardsPerRow;
            int x = startX + (col * (cardWidth + cardSpacing));
            int y = startY + (row * (cardHeight + cardSpacing));

            var cardBounds = new Rectangle(x, y, cardWidth, cardHeight);

            // Card background
            this.DrawFilledRectangle(spriteBatch, cardBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.9f));
            this.Draw3DBorder(spriteBatch, cardBounds, 2);

            // Aircraft icon area (left side)
            var iconBounds = new Rectangle(x + 10, y + 10, 100, 100);
            this.DrawFilledRectangle(spriteBatch, iconBounds, RetroColorPalette.AircraftWhite);
            this.Draw3DBorder(spriteBatch, iconBounds, 1);

            // TODO: Draw actual aircraft sprite here

            // Info area (right side)
            int infoX = x + 120;
            int infoY = y + 10;

            if (AirlineTycoonGame.TextRenderer != null)
            {
                // Registration number
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    aircraft.RegistrationNumber,
                    new Vector2(infoX, infoY),
                    Color.White
                );

                // Aircraft type
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    aircraft.Type.Name,
                    new Vector2(infoX, infoY + 20),
                    RetroColorPalette.TextSecondary
                );

                // Condition percentage
                AirlineTycoonGame.TextRenderer.DrawText(
                    spriteBatch,
                    $"Condition: {aircraft.Condition:P0}",
                    new Vector2(infoX, infoY + 40),
                    RetroColorPalette.TextSecondary
                );
            }

            // Condition bar (like RCT's ratings)
            var conditionBounds = new Rectangle(infoX, infoY + 60, 230, 15);
            this.DrawFilledRectangle(spriteBatch, conditionBounds, Color.DarkGray);

            // Fill based on condition (green = good, yellow = medium, red = poor)
            int conditionFill = (int)(aircraft.Condition * 230);
            Color conditionColor = aircraft.Condition > 0.7 ? RetroColorPalette.Success :
                                  aircraft.Condition > 0.4 ? RetroColorPalette.Warning :
                                  RetroColorPalette.Error;

            var conditionFillBounds = new Rectangle(infoX, infoY + 60, conditionFill, 15);
            this.DrawFilledRectangle(spriteBatch, conditionFillBounds, conditionColor);
            this.Draw3DBorder(spriteBatch, conditionBounds, 1);

            // Lease indicator (if leased)
            if (aircraft.IsLeased)
            {
                var leaseBounds = new Rectangle(infoX, infoY + 85, 230, 20);
                this.DrawFilledRectangle(spriteBatch, leaseBounds, RetroColorPalette.Warning);

                if (AirlineTycoonGame.TextRenderer != null)
                {
                    AirlineTycoonGame.TextRenderer.DrawText(
                        spriteBatch,
                        $"LEASED - ${aircraft.MonthlyLeasePayment:N0}/mo",
                        new Vector2(infoX + 5, infoY + 88),
                        Color.Black
                    );
                }
            }
            else
            {
                // Owned indicator
                if (AirlineTycoonGame.TextRenderer != null)
                {
                    AirlineTycoonGame.TextRenderer.DrawText(
                        spriteBatch,
                        "OWNED",
                        new Vector2(infoX, infoY + 88),
                        RetroColorPalette.Success
                    );
                }
            }
        }
    }

    /// <summary>
    /// Draws the aircraft details panel.
    /// </summary>
    private void DrawAircraftDetails(SpriteBatch spriteBatch)
    {
        // Details panel
        var detailsBounds = new Rectangle(820, 100, 440, 600);
        this.DrawFilledRectangle(spriteBatch, detailsBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.8f));
        this.Draw3DBorder(spriteBatch, detailsBounds);

        // Title
        var titleBounds = new Rectangle(830, 110, 420, 30);
        this.DrawFilledRectangle(spriteBatch, titleBounds, RetroColorPalette.TitleBarBackground);

        // Aircraft image (larger view)
        var imageBounds = new Rectangle(880, 160, 320, 180);
        this.DrawFilledRectangle(spriteBatch, imageBounds, RetroColorPalette.AircraftWhite);
        this.Draw3DBorder(spriteBatch, imageBounds, 2);

        // TODO: Draw actual aircraft sprite

        // Specs panel
        var specsBounds = new Rectangle(840, 360, 400, 240);
        this.DrawFilledRectangle(spriteBatch, specsBounds, Color.Black);
        this.Draw3DBorder(spriteBatch, specsBounds, 1);

        // TODO: Display specs:
        // - Type (Boeing 737, Airbus A320, etc.)
        // - Capacity
        // - Range
        // - Fuel consumption
        // - Operating cost per hour
        // - Purchase price / lease payment
        // - Current route assignment
        // - Total flight hours
        // - Maintenance schedule

        // Maintenance button
        var maintenanceButton = new Rectangle(840, 650, 180, 35);
        this.DrawFilledRectangle(spriteBatch, maintenanceButton, RetroColorPalette.Warning);
        this.Draw3DBorder(spriteBatch, maintenanceButton, 2);

        // Sell/Return button
        var sellButton = new Rectangle(1060, 650, 180, 35);
        this.DrawFilledRectangle(spriteBatch, sellButton, RetroColorPalette.Error);
        this.Draw3DBorder(spriteBatch, sellButton, 2);
    }

    /// <summary>
    /// Handles the back button click.
    /// </summary>
    private void OnBack()
    {
        this.Controller?.ShowDashboard();
    }

    /// <summary>
    /// Handles the buy aircraft button click.
    /// </summary>
    private void OnBuyAircraft()
    {
        if (this.Controller != null)
        {
            var purchaseScreen = new AircraftPurchaseScreen(this.Controller, isLease: false);
            this.Controller.ScreenManager.SwitchTo(purchaseScreen);
        }
    }

    /// <summary>
    /// Handles the lease aircraft button click.
    /// </summary>
    private void OnLeaseAircraft()
    {
        if (this.Controller != null)
        {
            var purchaseScreen = new AircraftPurchaseScreen(this.Controller, isLease: true);
            this.Controller.ScreenManager.SwitchTo(purchaseScreen);
        }
    }

    /// <summary>
    /// Rebuilds the maintenance buttons for each aircraft in the fleet.
    /// Called during Update to keep buttons in sync with fleet.
    /// </summary>
    private void RebuildMaintenanceButtons()
    {
        var fleet = this.Controller?.Game.PlayerAirline.Fleet.ToList() ?? new List<AirlineTycoon.Domain.Aircraft>();

        // If fleet count matches button count, no rebuild needed
        if (fleet.Count == this.maintenanceButtons.Count)
        {
            return;
        }

        // Remove existing maintenance buttons
        foreach (var button in this.maintenanceButtons)
        {
            this.RemoveChild(button);
        }

        this.maintenanceButtons.Clear();

        // Create new maintenance buttons for each aircraft
        int cardWidth = 360;
        int cardHeight = 120;
        int cardSpacing = 20;
        int startX = 40;
        int startY = 160;
        int cardsPerRow = 2;

        for (int i = 0; i < fleet.Count; i++)
        {
            var aircraft = fleet[i];
            int row = i / cardsPerRow;
            int col = i % cardsPerRow;
            int x = startX + (col * (cardWidth + cardSpacing));
            int y = startY + (row * (cardHeight + cardSpacing));

            // Calculate maintenance cost
            decimal maintenanceCost = this.Controller?.CalculateMaintenanceCost(aircraft.RegistrationNumber) ?? 0;

            // Create maintenance button (bottom right of card)
            var maintenanceButton = new UIButton(
                $"Maintain (${maintenanceCost:N0})",
                new Vector2(x + cardWidth - 150, y + cardHeight - 35),
                new Vector2(140, 30)
            );

            // Disable button if aircraft is already in perfect condition or insufficient funds
            bool canAfford = this.Controller?.Game.PlayerAirline.Cash >= maintenanceCost;
            bool needsMaintenance = aircraft.Condition < 1.0;

            if (!needsMaintenance || !canAfford)
            {
                maintenanceButton.IsEnabled = false;
            }

            // Store aircraft registration in button tag for event handler
            string registration = aircraft.RegistrationNumber;
            maintenanceButton.Clicked += (s, e) => this.OnPerformMaintenance(registration);

            this.maintenanceButtons.Add(maintenanceButton);
            this.AddChild(maintenanceButton);
        }
    }

    /// <summary>
    /// Handles the maintenance button click for a specific aircraft.
    /// </summary>
    /// <param name="aircraftRegistration">The registration number of the aircraft to maintain.</param>
    private void OnPerformMaintenance(string aircraftRegistration)
    {
        if (this.Controller == null)
        {
            return;
        }

        bool success = this.Controller.PerformAircraftMaintenance(aircraftRegistration);

        if (success)
        {
            System.Diagnostics.Debug.WriteLine($"Maintenance performed on {aircraftRegistration}");
            // Force button rebuild to update button states and costs
            this.maintenanceButtons.Clear();
        }
        else
        {
            System.Diagnostics.Debug.WriteLine($"Maintenance failed for {aircraftRegistration} - insufficient funds or aircraft not found");
        }
    }
}
