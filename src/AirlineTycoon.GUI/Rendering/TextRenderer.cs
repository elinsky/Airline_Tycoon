using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AirlineTycoon.GUI.Rendering;

/// <summary>
/// Utility class for rendering pixel art text.
/// Handles text alignment, shadows, and color formatting.
/// </summary>
/// <remarks>
/// Inspired by RCT's text rendering:
/// - Clean, readable pixel fonts
/// - Optional drop shadows for contrast
/// - Alignment options (left, center, right)
/// - Color coding for different data types
///
/// All text is rendered using the PixelFont bitmap font
/// loaded through MonoGame's content pipeline.
/// </remarks>
public class TextRenderer
{
    private readonly SpriteFont font;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextRenderer"/> class.
    /// </summary>
    /// <param name="font">The bitmap font to use for rendering.</param>
    public TextRenderer(SpriteFont font)
    {
        this.font = font;
    }

    /// <summary>
    /// Draws text at the specified position.
    /// </summary>
    /// <param name="spriteBatch">SpriteBatch for rendering.</param>
    /// <param name="text">Text to draw.</param>
    /// <param name="position">Position to draw at.</param>
    /// <param name="color">Text color.</param>
    /// <param name="shadow">Whether to draw a drop shadow.</param>
    public void DrawText(
        SpriteBatch spriteBatch,
        string text,
        Vector2 position,
        Color color,
        bool shadow = true)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        // Draw shadow first (offset down-right by 1 pixel)
        if (shadow)
        {
            spriteBatch.DrawString(
                this.font,
                text,
                position + new Vector2(1, 1),
                RetroColorPalette.TextShadow
            );
        }

        // Draw main text
        spriteBatch.DrawString(this.font, text, position, color);
    }

    /// <summary>
    /// Draws centered text within a bounding box.
    /// </summary>
    /// <param name="spriteBatch">SpriteBatch for rendering.</param>
    /// <param name="text">Text to draw.</param>
    /// <param name="bounds">Bounding rectangle to center within.</param>
    /// <param name="color">Text color.</param>
    /// <param name="shadow">Whether to draw a drop shadow.</param>
    public void DrawCenteredText(
        SpriteBatch spriteBatch,
        string text,
        Rectangle bounds,
        Color color,
        bool shadow = true)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var textSize = this.font.MeasureString(text);
        var position = new Vector2(
            bounds.X + (bounds.Width - textSize.X) / 2f,
            bounds.Y + (bounds.Height - textSize.Y) / 2f
        );

        this.DrawText(spriteBatch, text, position, color, shadow);
    }

    /// <summary>
    /// Draws right-aligned text.
    /// </summary>
    /// <param name="spriteBatch">SpriteBatch for rendering.</param>
    /// <param name="text">Text to draw.</param>
    /// <param name="position">Right edge position.</param>
    /// <param name="color">Text color.</param>
    /// <param name="shadow">Whether to draw a drop shadow.</param>
    public void DrawRightAlignedText(
        SpriteBatch spriteBatch,
        string text,
        Vector2 position,
        Color color,
        bool shadow = true)
    {
        if (string.IsNullOrEmpty(text))
        {
            return;
        }

        var textSize = this.font.MeasureString(text);
        var adjustedPosition = new Vector2(position.X - textSize.X, position.Y);

        this.DrawText(spriteBatch, text, adjustedPosition, color, shadow);
    }

    /// <summary>
    /// Formats currency values with proper color coding.
    /// </summary>
    /// <param name="amount">Amount to format.</param>
    /// <returns>Formatted currency string.</returns>
    public static string FormatCurrency(decimal amount)
    {
        if (amount >= 1_000_000)
        {
            return $"${amount / 1_000_000:F1}M";
        }
        else if (amount >= 1_000)
        {
            return $"${amount / 1_000:F0}K";
        }
        else
        {
            return $"${amount:F0}";
        }
    }

    /// <summary>
    /// Gets the color for a currency value (green for positive, red for negative).
    /// </summary>
    /// <param name="amount">Amount to get color for.</param>
    /// <returns>Color based on positive/negative value.</returns>
    public static Color GetCurrencyColor(decimal amount)
    {
        return amount >= 0 ? RetroColorPalette.Success : RetroColorPalette.Error;
    }

    /// <summary>
    /// Formats a percentage value.
    /// </summary>
    /// <param name="value">Value from 0.0 to 1.0.</param>
    /// <returns>Formatted percentage string.</returns>
    public static string FormatPercentage(double value)
    {
        return $"{value * 100:F0}%";
    }

    /// <summary>
    /// Formats a large number with K/M suffixes.
    /// </summary>
    /// <param name="number">Number to format.</param>
    /// <returns>Formatted number string.</returns>
    public static string FormatNumber(int number)
    {
        if (number >= 1_000_000)
        {
            return $"{number / 1_000_000.0:F1}M";
        }
        else if (number >= 1_000)
        {
            return $"{number / 1_000.0:F0}K";
        }
        else
        {
            return number.ToString();
        }
    }

    /// <summary>
    /// Measures the size of a text string.
    /// </summary>
    /// <param name="text">Text to measure.</param>
    /// <returns>Size of the text in pixels.</returns>
    public Vector2 MeasureString(string text)
    {
        return this.font.MeasureString(text);
    }
}
