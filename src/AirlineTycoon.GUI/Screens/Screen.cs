using AirlineTycoon.GUI.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AirlineTycoon.GUI.Screens;

/// <summary>
/// Base class for all game screens in the retro UI.
/// Represents a full-screen view (dashboard, route management, fleet, etc.).
/// </summary>
/// <remarks>
/// Screens in Airline Tycoon are similar to RCT's different view modes:
/// - Dashboard (main overview screen)
/// - Route Management (list of routes)
/// - Fleet Management (aircraft inventory)
/// - Competitor View (market analysis)
/// - Financial Reports (charts and numbers)
///
/// Each screen fills the entire 1280x720 game area and handles its own
/// rendering and input. The ScreenManager handles transitions between screens.
/// </remarks>
public abstract class Screen : UIElement
{
    /// <summary>
    /// Gets the screen's title (displayed in top bar).
    /// </summary>
    public abstract string Title { get; }

    /// <summary>
    /// Gets the game instance for accessing game data.
    /// </summary>
    protected Game? GameInstance { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Screen"/> class.
    /// </summary>
    protected Screen()
    {
        // Screens always fill the entire game area
        this.Position = Vector2.Zero;
        this.Size = new Vector2(AirlineTycoonGame.BaseWidth, AirlineTycoonGame.BaseHeight);
    }

    /// <summary>
    /// Sets the game instance for this screen.
    /// Called by ScreenManager when the screen is activated.
    /// </summary>
    /// <param name="game">The game instance.</param>
    public void SetGameInstance(Game? game)
    {
        this.GameInstance = game;
        this.OnGameInstanceSet();
    }

    /// <summary>
    /// Called when the game instance is set.
    /// Override to load data or initialize UI based on game state.
    /// </summary>
    protected virtual void OnGameInstanceSet()
    {
        // Override in derived classes
    }

    /// <summary>
    /// Called when the screen becomes active.
    /// Override to refresh data or reset state.
    /// </summary>
    public virtual void OnActivated()
    {
        // Override in derived classes
    }

    /// <summary>
    /// Called when the screen becomes inactive.
    /// Override to save state or cleanup.
    /// </summary>
    public virtual void OnDeactivated()
    {
        // Override in derived classes
    }

    /// <summary>
    /// Draws a filled rectangle (helper for backgrounds).
    /// </summary>
    protected void DrawFilledRectangle(SpriteBatch spriteBatch, Rectangle bounds, Color color)
    {
        // Create a 1x1 white pixel texture
        var whitePixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        whitePixel.SetData(new[] { Color.White });

        spriteBatch.Draw(whitePixel, bounds, color);

        whitePixel.Dispose();
    }

    /// <summary>
    /// Draws the RCT-style top bar with title and stats.
    /// </summary>
    protected void DrawTopBar(SpriteBatch spriteBatch, GameTime gameTime)
    {
        // Top bar background (dark blue)
        var topBarBounds = new Rectangle(0, 0, (int)this.Size.X, 32);
        this.DrawFilledRectangle(spriteBatch, topBarBounds, RetroColorPalette.TitleBarBackground);

        if (AirlineTycoonGame.TextRenderer == null)
        {
            return;
        }

        // Day indicator (left side)
        var dayAreaBounds = new Rectangle(8, 6, 120, 20);
        this.DrawFilledRectangle(spriteBatch, dayAreaBounds, RetroColorPalette.Darken(RetroColorPalette.TitleBarBackground, 0.7f));

        // TODO: Get actual day from game instance
        string dayText = "Day 1";
        AirlineTycoonGame.TextRenderer.DrawCenteredText(
            spriteBatch,
            dayText,
            dayAreaBounds,
            RetroColorPalette.TextPrimary,
            shadow: false
        );

        // Cash indicator (center-left)
        var cashAreaBounds = new Rectangle(136, 6, 180, 20);
        this.DrawFilledRectangle(spriteBatch, cashAreaBounds, RetroColorPalette.Darken(RetroColorPalette.Success, 0.5f));

        // TODO: Get actual cash from game instance
        string cashText = "$5.0M";
        AirlineTycoonGame.TextRenderer.DrawCenteredText(
            spriteBatch,
            cashText,
            cashAreaBounds,
            RetroColorPalette.TextPrimary,
            shadow: false
        );

        // Reputation indicator (center-right)
        var repAreaBounds = new Rectangle(324, 6, 140, 20);
        this.DrawFilledRectangle(spriteBatch, repAreaBounds, RetroColorPalette.Darken(RetroColorPalette.Info, 0.5f));

        // TODO: Get actual reputation from game instance
        string repText = "Rep: 50";
        AirlineTycoonGame.TextRenderer.DrawCenteredText(
            spriteBatch,
            repText,
            repAreaBounds,
            RetroColorPalette.TextPrimary,
            shadow: false
        );

        // Screen title (right side)
        var titleAreaBounds = new Rectangle((int)this.Size.X - 320, 6, 312, 20);
        string titleText = this.Title;
        AirlineTycoonGame.TextRenderer.DrawRightAlignedText(
            spriteBatch,
            titleText,
            new Vector2(titleAreaBounds.Right, titleAreaBounds.Y + 4),
            RetroColorPalette.TextPrimary,
            shadow: true
        );
    }

    /// <summary>
    /// Draws a simple 3D-style border around a rectangle.
    /// </summary>
    protected void Draw3DBorder(SpriteBatch spriteBatch, Rectangle bounds, int borderWidth = 2)
    {
        var whitePixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        whitePixel.SetData(new[] { Color.White });

        // Top border (light)
        spriteBatch.Draw(whitePixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, borderWidth),
            RetroColorPalette.PanelHighlight);

        // Left border (light)
        spriteBatch.Draw(whitePixel, new Rectangle(bounds.X, bounds.Y, borderWidth, bounds.Height),
            RetroColorPalette.PanelHighlight);

        // Bottom border (dark)
        spriteBatch.Draw(whitePixel, new Rectangle(bounds.X, bounds.Bottom - borderWidth, bounds.Width, borderWidth),
            RetroColorPalette.PanelShadow);

        // Right border (dark)
        spriteBatch.Draw(whitePixel, new Rectangle(bounds.Right - borderWidth, bounds.Y, borderWidth, bounds.Height),
            RetroColorPalette.PanelShadow);

        whitePixel.Dispose();
    }
}
