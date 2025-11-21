using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AirlineTycoon.GUI.UI;

/// <summary>
/// A retro-style panel/window UI element.
/// Inspired by RollerCoaster Tycoon's window design.
/// </summary>
/// <remarks>
/// RCT windows had distinctive characteristics:
/// - Blue gradient title bar with close button
/// - Bordered frame with 3D effect
/// - Dark blue content area
/// - Optional scrollbars for content
/// - Draggable by title bar
///
/// This implementation recreates that aesthetic for pixel art rendering.
/// </remarks>
public class UIPanel : UIElement
{
    /// <summary>
    /// Gets or sets the panel's title text.
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets whether the panel has a title bar.
    /// </summary>
    public bool HasTitleBar { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the panel can be closed.
    /// </summary>
    public bool CanClose { get; set; } = true;

    /// <summary>
    /// Gets or sets whether the panel can be dragged.
    /// </summary>
    public bool IsDraggable { get; set; } = true;

    /// <summary>
    /// Gets or sets the height of the title bar.
    /// </summary>
    public int TitleBarHeight { get; set; } = 24;

    /// <summary>
    /// Gets whether the panel is currently being dragged.
    /// </summary>
    public bool IsDragging { get; private set; }

    /// <summary>
    /// Stores the drag offset when dragging starts.
    /// </summary>
    private Vector2 dragOffset;

    /// <summary>
    /// Event fired when the close button is clicked.
    /// </summary>
    public event EventHandler? CloseRequested;

    /// <summary>
    /// Initializes a new instance of the <see cref="UIPanel"/> class.
    /// </summary>
    /// <param name="title">Panel title.</param>
    /// <param name="position">Panel position.</param>
    /// <param name="size">Panel size.</param>
    public UIPanel(string title, Vector2 position, Vector2 size)
    {
        this.Title = title;
        this.Position = position;
        this.Size = size;
    }

    /// <summary>
    /// Gets the title bar bounds.
    /// </summary>
    public Rectangle TitleBarBounds
    {
        get
        {
            if (!this.HasTitleBar)
            {
                return Rectangle.Empty;
            }

            var bounds = this.Bounds;
            return new Rectangle(bounds.X, bounds.Y, bounds.Width, this.TitleBarHeight);
        }
    }

    /// <summary>
    /// Gets the content area bounds (below title bar).
    /// </summary>
    public Rectangle ContentBounds
    {
        get
        {
            var bounds = this.Bounds;
            int contentY = bounds.Y + (this.HasTitleBar ? this.TitleBarHeight : 0);
            int contentHeight = bounds.Height - (this.HasTitleBar ? this.TitleBarHeight : 0);

            return new Rectangle(bounds.X, contentY, bounds.Width, contentHeight);
        }
    }

    /// <summary>
    /// Gets the close button bounds.
    /// </summary>
    public Rectangle CloseButtonBounds
    {
        get
        {
            if (!this.HasTitleBar || !this.CanClose)
            {
                return Rectangle.Empty;
            }

            var titleBar = this.TitleBarBounds;
            int buttonSize = this.TitleBarHeight - 4;
            int buttonX = titleBar.Right - buttonSize - 4;
            int buttonY = titleBar.Y + 2;

            return new Rectangle(buttonX, buttonY, buttonSize, buttonSize);
        }
    }

    /// <summary>
    /// Renders the panel with title bar and borders.
    /// </summary>
    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (!this.IsVisible)
        {
            return;
        }

        Texture2D whitePixel = CreateWhitePixelTexture(spriteBatch.GraphicsDevice);
        var bounds = this.Bounds;
        int borderWidth = 2;

        // Draw main panel background
        spriteBatch.Draw(whitePixel, this.ContentBounds, RetroColorPalette.WindowBackground);

        // Draw title bar if enabled
        if (this.HasTitleBar)
        {
            var titleBar = this.TitleBarBounds;
            spriteBatch.Draw(whitePixel, titleBar, RetroColorPalette.TitleBarBackground);

            // Draw close button if enabled
            if (this.CanClose)
            {
                var closeButton = this.CloseButtonBounds;
                spriteBatch.Draw(whitePixel, closeButton, RetroColorPalette.Error);

                // Draw X in close button (simplified)
                // TODO: Use proper pixel art icon once we have sprite sheets
            }

            // TODO: Draw title text (requires bitmap font)
        }

        // Draw panel borders (3D effect)
        // Outer border (highlight on top-left, shadow on bottom-right)
        spriteBatch.Draw(whitePixel, new Rectangle(bounds.X, bounds.Y, bounds.Width, borderWidth), RetroColorPalette.PanelHighlight);
        spriteBatch.Draw(whitePixel, new Rectangle(bounds.X, bounds.Y, borderWidth, bounds.Height), RetroColorPalette.PanelHighlight);
        spriteBatch.Draw(whitePixel, new Rectangle(bounds.X, bounds.Bottom - borderWidth, bounds.Width, borderWidth), RetroColorPalette.PanelShadow);
        spriteBatch.Draw(whitePixel, new Rectangle(bounds.Right - borderWidth, bounds.Y, borderWidth, bounds.Height), RetroColorPalette.PanelShadow);

        whitePixel.Dispose();

        // Draw children (buttons, labels, etc. inside the panel)
        base.Draw(spriteBatch, gameTime);
    }

    /// <summary>
    /// Updates the panel and handles dragging.
    /// </summary>
    public override void Update(GameTime gameTime)
    {
        // TODO: Update dragging if mouse is down
        // This requires tracking mouse state across frames

        base.Update(gameTime);
    }

    /// <summary>
    /// Handles mouse down for dragging and close button.
    /// </summary>
    public override bool OnMouseDown(Vector2 position, MouseButton button)
    {
        if (!this.IsVisible || !this.IsEnabled)
        {
            return false;
        }

        if (button != MouseButton.Left)
        {
            return base.OnMouseDown(position, button);
        }

        // Check close button first
        if (this.CanClose && this.CloseButtonBounds.Contains(position))
        {
            this.OnCloseRequested();
            return true;
        }

        // Check if clicking on title bar for dragging
        if (this.IsDraggable && this.TitleBarBounds.Contains(position))
        {
            this.IsDragging = true;
            this.dragOffset = position - this.Position;
            return true;
        }

        return base.OnMouseDown(position, button);
    }

    /// <summary>
    /// Handles mouse up to stop dragging.
    /// </summary>
    public override bool OnMouseUp(Vector2 position, MouseButton button)
    {
        if (button == MouseButton.Left && this.IsDragging)
        {
            this.IsDragging = false;
            return true;
        }

        return base.OnMouseUp(position, button);
    }

    /// <summary>
    /// Handles mouse move for dragging.
    /// </summary>
    public override bool OnMouseMove(Vector2 position)
    {
        if (this.IsDragging)
        {
            this.Position = position - this.dragOffset;
            return true;
        }

        return base.OnMouseMove(position);
    }

    /// <summary>
    /// Fires the close requested event.
    /// </summary>
    protected virtual void OnCloseRequested()
    {
        this.CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Creates a 1x1 white texture for drawing solid color rectangles.
    /// </summary>
    /// <remarks>
    /// In production code, this would be cached and reused.
    /// This is a temporary implementation until we have proper texture management.
    /// </remarks>
    private static Texture2D CreateWhitePixelTexture(GraphicsDevice graphicsDevice)
    {
        var texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }
}
