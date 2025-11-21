using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AirlineTycoon.GUI.Screens;

/// <summary>
/// Manages screen transitions and rendering for the game.
/// Similar to a state machine for UI screens.
/// </summary>
/// <remarks>
/// The ScreenManager handles:
/// - Switching between different screens (dashboard, routes, fleet, etc.)
/// - Maintaining screen stack for modals/overlays
/// - Delegating update and draw calls to active screen
/// - Passing game instance to screens
///
/// Like RCT, we maintain one primary screen at a time, with support for
/// modal dialogs that overlay on top of the current screen.
/// </remarks>
public class ScreenManager
{
    private Screen? currentScreen;

    /// <summary>
    /// Gets the currently active screen.
    /// </summary>
    public Screen? CurrentScreen => this.currentScreen;

    /// <summary>
    /// Initializes a new instance of the <see cref="ScreenManager"/> class.
    /// </summary>
    public ScreenManager()
    {
    }

    /// <summary>
    /// Switches to a new screen.
    /// </summary>
    /// <param name="newScreen">The screen to switch to.</param>
    public void SwitchTo(Screen newScreen)
    {
        // Deactivate current screen
        this.currentScreen?.OnDeactivated();

        // Switch to new screen
        this.currentScreen = newScreen;
        this.currentScreen.OnActivated();
    }

    /// <summary>
    /// Updates the current screen.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public void Update(GameTime gameTime)
    {
        this.currentScreen?.Update(gameTime);
    }

    /// <summary>
    /// Draws the current screen.
    /// </summary>
    /// <param name="spriteBatch">SpriteBatch for rendering.</param>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        this.currentScreen?.Draw(spriteBatch, gameTime);
    }

    /// <summary>
    /// Handles mouse move events.
    /// </summary>
    /// <param name="position">Mouse position in game coordinates.</param>
    public void OnMouseMove(Vector2 position)
    {
        this.currentScreen?.OnMouseMove(position);
    }

    /// <summary>
    /// Handles mouse button down events.
    /// </summary>
    /// <param name="position">Mouse position in game coordinates.</param>
    /// <param name="button">Which mouse button was pressed.</param>
    public void OnMouseDown(Vector2 position, UI.MouseButton button)
    {
        this.currentScreen?.OnMouseDown(position, button);
    }

    /// <summary>
    /// Handles mouse button up events.
    /// </summary>
    /// <param name="position">Mouse position in game coordinates.</param>
    /// <param name="button">Which mouse button was released.</param>
    public void OnMouseUp(Vector2 position, UI.MouseButton button)
    {
        this.currentScreen?.OnMouseUp(position, button);
    }
}
