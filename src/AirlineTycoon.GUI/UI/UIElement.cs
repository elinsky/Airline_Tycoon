using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AirlineTycoon.GUI.UI;

/// <summary>
/// Base class for all UI elements in the retro pixel art framework.
/// Provides common functionality for positioning, rendering, and input handling.
/// </summary>
/// <remarks>
/// The UI framework is inspired by RollerCoaster Tycoon's UI system:
/// - Pixel-perfect positioning and rendering
/// - Simple rectangular bounds-based layout
/// - Parent-child hierarchy for composable UI
/// - State-based rendering (normal, hover, pressed, disabled)
///
/// All coordinates are in game space (0,0 to 1280,720), not screen space.
/// </remarks>
public abstract class UIElement
{
    /// <summary>
    /// Gets or sets the position of this UI element relative to its parent.
    /// </summary>
    public Vector2 Position { get; set; }

    /// <summary>
    /// Gets or sets the size of this UI element.
    /// </summary>
    public Vector2 Size { get; set; }

    /// <summary>
    /// Gets or sets whether this UI element is visible.
    /// Invisible elements are not rendered or updated.
    /// </summary>
    public bool IsVisible { get; set; } = true;

    /// <summary>
    /// Gets or sets whether this UI element is enabled.
    /// Disabled elements are rendered differently and don't respond to input.
    /// </summary>
    public bool IsEnabled { get; set; } = true;

    /// <summary>
    /// Gets the absolute position of this element (accounting for parent offsets).
    /// </summary>
    public Vector2 AbsolutePosition
    {
        get
        {
            if (this.Parent == null)
            {
                return this.Position;
            }

            return this.Parent.AbsolutePosition + this.Position;
        }
    }

    /// <summary>
    /// Gets the bounding rectangle of this UI element in absolute coordinates.
    /// </summary>
    public Rectangle Bounds => new(
        (int)this.AbsolutePosition.X,
        (int)this.AbsolutePosition.Y,
        (int)this.Size.X,
        (int)this.Size.Y
    );

    /// <summary>
    /// Gets or sets the parent UI element.
    /// </summary>
    public UIElement? Parent { get; set; }

    /// <summary>
    /// Gets the list of child UI elements.
    /// </summary>
    public List<UIElement> Children { get; } = [];

    /// <summary>
    /// Adds a child UI element.
    /// </summary>
    /// <param name="child">The child element to add.</param>
    public void AddChild(UIElement child)
    {
        child.Parent = this;
        this.Children.Add(child);
    }

    /// <summary>
    /// Removes a child UI element.
    /// </summary>
    /// <param name="child">The child element to remove.</param>
    public void RemoveChild(UIElement child)
    {
        child.Parent = null;
        this.Children.Remove(child);
    }

    /// <summary>
    /// Checks if a point is within this UI element's bounds.
    /// </summary>
    /// <param name="point">Point to test in game coordinates.</param>
    /// <returns>True if the point is inside the bounds.</returns>
    public bool ContainsPoint(Vector2 point)
    {
        return this.Bounds.Contains(point);
    }

    /// <summary>
    /// Updates the UI element's state.
    /// Override this to add custom update logic.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public virtual void Update(GameTime gameTime)
    {
        if (!this.IsVisible)
        {
            return;
        }

        // Update all children
        foreach (var child in this.Children)
        {
            child.Update(gameTime);
        }
    }

    /// <summary>
    /// Renders the UI element.
    /// Override this to implement custom rendering.
    /// </summary>
    /// <param name="spriteBatch">SpriteBatch for rendering.</param>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    public virtual void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (!this.IsVisible)
        {
            return;
        }

        // Render children after this element (children draw on top)
        foreach (var child in this.Children)
        {
            child.Draw(spriteBatch, gameTime);
        }
    }

    /// <summary>
    /// Handles mouse move events.
    /// Override to implement hover behavior.
    /// </summary>
    /// <param name="position">Mouse position in game coordinates.</param>
    /// <returns>True if the event was handled.</returns>
    public virtual bool OnMouseMove(Vector2 position)
    {
        if (!this.IsVisible || !this.IsEnabled)
        {
            return false;
        }

        // Check children first (front to back)
        for (int i = this.Children.Count - 1; i >= 0; i--)
        {
            if (this.Children[i].OnMouseMove(position))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Handles mouse button down events.
    /// Override to implement click behavior.
    /// </summary>
    /// <param name="position">Mouse position in game coordinates.</param>
    /// <param name="button">Which mouse button was pressed.</param>
    /// <returns>True if the event was handled.</returns>
    public virtual bool OnMouseDown(Vector2 position, MouseButton button)
    {
        if (!this.IsVisible || !this.IsEnabled)
        {
            return false;
        }

        // Check children first (front to back)
        for (int i = this.Children.Count - 1; i >= 0; i--)
        {
            if (this.Children[i].OnMouseDown(position, button))
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Handles mouse button up events.
    /// Override to implement click behavior.
    /// </summary>
    /// <param name="position">Mouse position in game coordinates.</param>
    /// <param name="button">Which mouse button was released.</param>
    /// <returns>True if the event was handled.</returns>
    public virtual bool OnMouseUp(Vector2 position, MouseButton button)
    {
        if (!this.IsVisible || !this.IsEnabled)
        {
            return false;
        }

        // Check children first (front to back)
        for (int i = this.Children.Count - 1; i >= 0; i--)
        {
            if (this.Children[i].OnMouseUp(position, button))
            {
                return true;
            }
        }

        return false;
    }
}

/// <summary>
/// Represents mouse buttons.
/// </summary>
public enum MouseButton
{
    /// <summary>
    /// Left mouse button.
    /// </summary>
    Left,

    /// <summary>
    /// Right mouse button.
    /// </summary>
    Right,

    /// <summary>
    /// Middle mouse button.
    /// </summary>
    Middle
}
