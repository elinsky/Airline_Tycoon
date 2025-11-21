using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AirlineTycoon.GUI.UI;

/// <summary>
/// A retro-style button UI element with hover and click states.
/// Inspired by RollerCoaster Tycoon's button design.
/// </summary>
/// <remarks>
/// RCT buttons had distinct visual states:
/// - Normal: Medium gray/blue with light top-left border, dark bottom-right border
/// - Hover: Slightly lighter background
/// - Pressed: Inverted borders (dark top-left, light bottom-right)
/// - Disabled: Dark gray with muted borders
///
/// This implementation recreates that aesthetic using pixel art rendering.
/// </remarks>
public class UIButton : UIElement
{
    /// <summary>
    /// Gets or sets the button's text label.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the button is currently being hovered.
    /// </summary>
    public bool IsHovered { get; private set; }

    /// <summary>
    /// Gets or sets whether the button is currently being pressed.
    /// </summary>
    public bool IsPressed { get; private set; }

    /// <summary>
    /// Stores where the mouse press started (for trackpad-friendly click detection).
    /// </summary>
    private Vector2 pressStartPosition;

    /// <summary>
    /// Event fired when the button is clicked.
    /// </summary>
    public event EventHandler? Clicked;

    /// <summary>
    /// Initializes a new instance of the <see cref="UIButton"/> class.
    /// </summary>
    /// <param name="text">Button text.</param>
    /// <param name="position">Button position.</param>
    /// <param name="size">Button size.</param>
    public UIButton(string text, Vector2 position, Vector2 size)
    {
        this.Text = text;
        this.Position = position;
        this.Size = size;
    }

    /// <summary>
    /// Renders the button with appropriate state styling.
    /// </summary>
    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (!this.IsVisible)
        {
            return;
        }

        var bounds = this.Bounds;

        // Determine button color based on state
        Color backgroundColor = this.IsEnabled
            ? (this.IsPressed ? RetroColorPalette.ButtonPressed :
               this.IsHovered ? RetroColorPalette.ButtonHover :
               RetroColorPalette.ButtonNormal)
            : RetroColorPalette.ButtonDisabled;

        // Create a 1x1 white texture for drawing rectangles
        // (This is a simplified approach - in production we'd have a texture cache)
        if (AirlineTycoonGame.WhitePixel == null)
        {
            return;
        }

        Texture2D whitePixel = AirlineTycoonGame.WhitePixel;

        // Draw button background
        spriteBatch.Draw(whitePixel, bounds, backgroundColor);

        // Draw button borders (3D effect)
        Color lightBorder = this.IsPressed ? RetroColorPalette.PanelShadow : RetroColorPalette.PanelHighlight;
        Color darkBorder = this.IsPressed ? RetroColorPalette.PanelHighlight : RetroColorPalette.PanelShadow;

        int borderWidth = 2;

        // Top border (light when normal, dark when pressed)
        spriteBatch.Draw(whitePixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, borderWidth), lightBorder);

        // Left border (light when normal, dark when pressed)
        spriteBatch.Draw(whitePixel, new Rectangle(bounds.X, bounds.Y, borderWidth, bounds.Height), lightBorder);

        // Bottom border (dark when normal, light when pressed)
        spriteBatch.Draw(whitePixel, new Rectangle(bounds.X, bounds.Bottom - borderWidth, bounds.Width, borderWidth), darkBorder);

        // Right border (dark when normal, light when pressed)
        spriteBatch.Draw(whitePixel, new Rectangle(bounds.Right - borderWidth, bounds.Y, borderWidth, bounds.Height), darkBorder);

        // Draw button text
        if (AirlineTycoonGame.TextRenderer != null && !string.IsNullOrEmpty(this.Text))
        {
            Color textColor = this.IsEnabled ? RetroColorPalette.TextPrimary : RetroColorPalette.TextDisabled;
            AirlineTycoonGame.TextRenderer.DrawCenteredText(spriteBatch, this.Text, bounds, textColor);
        }

        base.Draw(spriteBatch, gameTime);
    }

    /// <summary>
    /// Handles mouse move events for hover state.
    /// </summary>
    public override bool OnMouseMove(Vector2 position)
    {
        if (!this.IsVisible || !this.IsEnabled)
        {
            this.IsHovered = false;
            return false;
        }

        bool wasHovered = this.IsHovered;
        this.IsHovered = this.ContainsPoint(position);

        // If we just started hovering, we could play a sound here
        if (this.IsHovered && !wasHovered)
        {
            // TODO: Play hover sound effect
        }

        return base.OnMouseMove(position);
    }

    /// <summary>
    /// Handles mouse button down events.
    /// </summary>
    public override bool OnMouseDown(Vector2 position, MouseButton button)
    {
        if (!this.IsVisible || !this.IsEnabled)
        {
            return false;
        }

        if (button == MouseButton.Left && this.ContainsPoint(position))
        {
            this.IsPressed = true;
            this.pressStartPosition = position;
            // TODO: Play button press sound effect
            return true;
        }

        return base.OnMouseDown(position, button);
    }

    /// <summary>
    /// Handles mouse button up events and fires click event.
    /// </summary>
    public override bool OnMouseUp(Vector2 position, MouseButton button)
    {
        if (!this.IsVisible || !this.IsEnabled)
        {
            return false;
        }

        if (button == MouseButton.Left && this.IsPressed)
        {
            this.IsPressed = false;

            // Fire click event if mouse is still over the button OR
            // if the mouse hasn't drifted too far (trackpad-friendly)
            // Allow up to 50 pixels of drift for trackpad users
            float drift = Vector2.Distance(position, this.pressStartPosition);
            if (this.ContainsPoint(position) || drift < 50f)
            {
                this.OnClick();
                // TODO: Play button click sound effect
                return true;
            }
        }

        return base.OnMouseUp(position, button);
    }

    /// <summary>
    /// Fires the click event.
    /// </summary>
    protected virtual void OnClick()
    {
        this.Clicked?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Creates a 1x1 white texture for drawing solid color rectangles.
    /// </summary>
    /// <remarks>
    /// In production code, this would be cached and reused.
    /// Creating a new texture every frame is inefficient.
    /// This is a temporary implementation until we have proper texture management.
    /// </remarks>
    private static Texture2D CreateWhitePixelTexture(GraphicsDevice graphicsDevice)
    {
        var texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }
}
