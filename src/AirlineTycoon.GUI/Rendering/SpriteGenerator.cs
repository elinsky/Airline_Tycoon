using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AirlineTycoon.GUI.Rendering;

/// <summary>
/// Generates simple procedural pixel art sprites for aircraft and airports.
/// Creates retro-style sprites without needing external art assets.
/// </summary>
/// <remarks>
/// This class generates sprites using pixel-by-pixel rendering:
/// - Aircraft sprites: Side-view silhouettes with fuselage, wings, tail
/// - Airport icons: Simple geometric shapes (circles, diamonds, squares)
///
/// Useful for prototyping or when you don't have art assets yet.
/// For production, professionally designed sprites are recommended.
///
/// All sprites use the retro color palette for visual consistency.
/// </remarks>
public static class SpriteGenerator
{
    /// <summary>
    /// Generates a simple aircraft sprite (side view).
    /// </summary>
    /// <param name="graphicsDevice">The graphics device for creating textures.</param>
    /// <param name="width">Width in pixels (aircraft length).</param>
    /// <param name="height">Height in pixels (aircraft height).</param>
    /// <param name="baseColor">The base color for the aircraft.</param>
    /// <param name="size">The size category of the aircraft.</param>
    /// <returns>A Texture2D containing the aircraft sprite.</returns>
    public static Texture2D GenerateAircraftSprite(
        GraphicsDevice graphicsDevice,
        int width,
        int height,
        Color baseColor,
        AircraftSize size)
    {
        var texture = new Texture2D(graphicsDevice, width, height);
        var pixels = new Color[width * height];

        // Initialize transparent background
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.Transparent;
        }

        // Calculate dimensions based on aircraft size
        int fuselageHeight = size switch
        {
            AircraftSize.Regional => height / 4,
            AircraftSize.NarrowBody => height / 3,
            AircraftSize.WideBody => height / 2,
            _ => height / 3
        };

        int wingWidth = width * 2 / 3;
        int wingHeight = height / 6;
        int tailHeight = height * 2 / 3;

        // Draw fuselage (horizontal ellipse)
        DrawFuselage(pixels, width, height, fuselageHeight, baseColor);

        // Draw wings (at midpoint)
        DrawWings(pixels, width, height, wingWidth, wingHeight, baseColor);

        // Draw tail (at back)
        DrawTail(pixels, width, height, tailHeight, baseColor);

        // Add windows (small dark rectangles)
        DrawWindows(pixels, width, height, fuselageHeight);

        // Add engine highlights (for jet engines)
        if (size != AircraftSize.Regional)
        {
            DrawEngines(pixels, width, height, baseColor);
        }

        texture.SetData(pixels);
        return texture;
    }

    /// <summary>
    /// Generates a simple airport icon.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device for creating textures.</param>
    /// <param name="size">Size in pixels (square icon).</param>
    /// <param name="color">The color for the icon.</param>
    /// <param name="airportType">The type of airport.</param>
    /// <returns>A Texture2D containing the airport icon.</returns>
    public static Texture2D GenerateAirportIcon(
        GraphicsDevice graphicsDevice,
        int size,
        Color color,
        AirportType airportType)
    {
        var texture = new Texture2D(graphicsDevice, size, size);
        var pixels = new Color[size * size];

        // Initialize transparent background
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = Color.Transparent;
        }

        // Draw different shapes based on airport type
        switch (airportType)
        {
            case AirportType.Hub:
                // Large filled circle with runway cross
                DrawFilledCircle(pixels, size, size, size / 2, size / 2, size / 2, color);
                DrawRunwayCross(pixels, size, size, Color.White);
                break;

            case AirportType.Regional:
                // Diamond shape with center dot
                DrawDiamond(pixels, size, size, color);
                DrawFilledCircle(pixels, size, size, size / 2, size / 2, 2, Color.White);
                break;

            case AirportType.Small:
                // Simple filled square
                DrawFilledSquare(pixels, size, size, color);
                break;
        }

        texture.SetData(pixels);
        return texture;
    }

    /// <summary>
    /// Draws the aircraft fuselage (horizontal ellipse).
    /// </summary>
    private static void DrawFuselage(Color[] pixels, int width, int height, int fuselageHeight, Color color)
    {
        int centerY = height / 2;
        int startX = width / 8;
        int endX = width * 7 / 8;

        for (int x = startX; x < endX; x++)
        {
            for (int y = centerY - fuselageHeight / 2; y < centerY + fuselageHeight / 2; y++)
            {
                if (y >= 0 && y < height)
                {
                    SetPixel(pixels, width, height, x, y, color);
                }
            }
        }

        // Rounded nose (front)
        for (int x = 0; x < startX; x++)
        {
            float progress = (float)x / startX;
            int halfHeight = (int)(fuselageHeight / 2 * progress);
            for (int y = centerY - halfHeight; y < centerY + halfHeight; y++)
            {
                if (y >= 0 && y < height)
                {
                    SetPixel(pixels, width, height, x, y, color);
                }
            }
        }

        // Rounded tail (back)
        for (int x = endX; x < width; x++)
        {
            float progress = (float)(width - x) / (width - endX);
            int halfHeight = (int)(fuselageHeight / 2 * progress);
            for (int y = centerY - halfHeight; y < centerY + halfHeight; y++)
            {
                if (y >= 0 && y < height)
                {
                    SetPixel(pixels, width, height, x, y, color);
                }
            }
        }
    }

    /// <summary>
    /// Draws the aircraft wings.
    /// </summary>
    private static void DrawWings(Color[] pixels, int width, int height, int wingWidth, int wingHeight, Color color)
    {
        int centerY = height / 2;
        int wingStartX = width / 3;
        int wingEndX = wingStartX + wingWidth;

        // Main wing (extending down from fuselage)
        for (int x = wingStartX; x < wingEndX && x < width; x++)
        {
            for (int y = centerY; y < centerY + wingHeight && y < height; y++)
            {
                SetPixel(pixels, width, height, x, y, Darken(color, 0.9f));
            }
        }
    }

    /// <summary>
    /// Draws the aircraft tail.
    /// </summary>
    private static void DrawTail(Color[] pixels, int width, int height, int tailHeight, Color color)
    {
        int centerY = height / 2;
        int tailX = width * 7 / 8;
        int tailWidth = width / 10;

        // Vertical tail fin
        for (int x = tailX; x < tailX + tailWidth && x < width; x++)
        {
            for (int y = centerY - tailHeight / 2; y < centerY && y < height; y++)
            {
                if (y >= 0)
                {
                    SetPixel(pixels, width, height, x, y, Darken(color, 0.85f));
                }
            }
        }
    }

    /// <summary>
    /// Draws windows on the fuselage.
    /// </summary>
    private static void DrawWindows(Color[] pixels, int width, int height, int fuselageHeight)
    {
        int centerY = height / 2;
        int windowY = centerY - fuselageHeight / 4;
        int windowSize = 2;
        int windowSpacing = 4;

        for (int x = width / 6; x < width * 3 / 4; x += windowSpacing)
        {
            for (int wx = 0; wx < windowSize; wx++)
            {
                for (int wy = 0; wy < windowSize; wy++)
                {
                    int px = x + wx;
                    int py = windowY + wy;
                    if (px < width && py >= 0 && py < height)
                    {
                        SetPixel(pixels, width, height, px, py, new Color(40, 60, 80));
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draws engine highlights.
    /// </summary>
    private static void DrawEngines(Color[] pixels, int width, int height, Color color)
    {
        int centerY = height / 2;
        int engineY = centerY + height / 6;
        int engineX = width / 2;
        int engineSize = 4;

        // Draw engine pod under wing
        for (int x = engineX; x < engineX + engineSize && x < width; x++)
        {
            for (int y = engineY; y < engineY + engineSize && y < height; y++)
            {
                SetPixel(pixels, width, height, x, y, Darken(color, 0.7f));
            }
        }
    }

    /// <summary>
    /// Draws a filled circle.
    /// </summary>
    private static void DrawFilledCircle(Color[] pixels, int width, int height, int centerX, int centerY, int radius, Color color)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int dx = x - centerX;
                int dy = y - centerY;
                if (dx * dx + dy * dy <= radius * radius)
                {
                    SetPixel(pixels, width, height, x, y, color);
                }
            }
        }
    }

    /// <summary>
    /// Draws a runway cross pattern.
    /// </summary>
    private static void DrawRunwayCross(Color[] pixels, int width, int height, Color color)
    {
        int centerX = width / 2;
        int centerY = height / 2;
        int lineThickness = 2;

        // Horizontal line
        for (int x = 0; x < width; x++)
        {
            for (int t = -lineThickness / 2; t <= lineThickness / 2; t++)
            {
                int y = centerY + t;
                if (y >= 0 && y < height)
                {
                    SetPixel(pixels, width, height, x, y, color);
                }
            }
        }

        // Vertical line
        for (int y = 0; y < height; y++)
        {
            for (int t = -lineThickness / 2; t <= lineThickness / 2; t++)
            {
                int x = centerX + t;
                if (x >= 0 && x < width)
                {
                    SetPixel(pixels, width, height, x, y, color);
                }
            }
        }
    }

    /// <summary>
    /// Draws a diamond shape.
    /// </summary>
    private static void DrawDiamond(Color[] pixels, int width, int height, Color color)
    {
        int centerX = width / 2;
        int centerY = height / 2;

        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int dx = Math.Abs(x - centerX);
                int dy = Math.Abs(y - centerY);
                if (dx + dy <= width / 2)
                {
                    SetPixel(pixels, width, height, x, y, color);
                }
            }
        }
    }

    /// <summary>
    /// Draws a filled square.
    /// </summary>
    private static void DrawFilledSquare(Color[] pixels, int width, int height, Color color)
    {
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                SetPixel(pixels, width, height, x, y, color);
            }
        }
    }

    /// <summary>
    /// Sets a pixel in the color array with bounds checking.
    /// </summary>
    private static void SetPixel(Color[] pixels, int width, int height, int x, int y, Color color)
    {
        if (x >= 0 && x < width && y >= 0 && y < height)
        {
            pixels[y * width + x] = color;
        }
    }

    /// <summary>
    /// Darkens a color by a factor.
    /// </summary>
    private static Color Darken(Color color, float factor)
    {
        return new Color(
            (int)(color.R * factor),
            (int)(color.G * factor),
            (int)(color.B * factor),
            color.A
        );
    }

    /// <summary>
    /// Generates a 9-slice panel border texture for scalable UI panels.
    /// Creates corners, edges, and center that can be tiled.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device.</param>
    /// <param name="size">Total size of the 9-slice texture (should be at least 24px).</param>
    /// <param name="borderColor">Color for the border.</param>
    /// <param name="fillColor">Color for the panel fill.</param>
    /// <returns>A 9-slice panel texture.</returns>
    public static Texture2D Generate9SlicePanel(
        GraphicsDevice graphicsDevice,
        int size,
        Color borderColor,
        Color fillColor)
    {
        var texture = new Texture2D(graphicsDevice, size, size);
        var pixels = new Color[size * size];

        // Fill with background
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = fillColor;
        }

        int borderWidth = size / 8; // Border is 1/8 of total size

        // Draw borders
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                // Top and bottom borders
                if (y < borderWidth || y >= size - borderWidth)
                {
                    SetPixel(pixels, size, size, x, y, borderColor);
                }
                // Left and right borders
                else if (x < borderWidth || x >= size - borderWidth)
                {
                    SetPixel(pixels, size, size, x, y, borderColor);
                }
            }
        }

        // Add corner highlights (lighter pixels for 3D effect)
        Color highlightColor = new Color(
            Math.Min(255, borderColor.R + 40),
            Math.Min(255, borderColor.G + 40),
            Math.Min(255, borderColor.B + 40)
        );

        for (int i = 0; i < borderWidth; i++)
        {
            // Top-left highlight
            SetPixel(pixels, size, size, i, 0, highlightColor);
            SetPixel(pixels, size, size, 0, i, highlightColor);
        }

        texture.SetData(pixels);
        return texture;
    }

    /// <summary>
    /// Generates a simplified US map showing major airport locations.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device.</param>
    /// <param name="width">Map width in pixels.</param>
    /// <param name="height">Map height in pixels.</param>
    /// <returns>A texture containing a simplified US map.</returns>
    public static Texture2D GenerateUSMap(GraphicsDevice graphicsDevice, int width, int height)
    {
        var texture = new Texture2D(graphicsDevice, width, height);
        var pixels = new Color[width * height];

        // Background (ocean blue)
        Color oceanColor = new Color(40, 60, 100);
        Color landColor = new Color(60, 80, 60);
        Color borderColor = new Color(100, 120, 100);

        // Fill with ocean
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = oceanColor;
        }

        // Draw simplified US landmass (very approximate rectangle with some detail)
        // US roughly spans 70-130° W longitude, 25-50° N latitude
        // Map these to pixel coordinates

        int landLeft = width / 8;
        int landRight = width * 7 / 8;
        int landTop = height / 6;
        int landBottom = height * 5 / 6;

        // Fill land area
        for (int y = landTop; y < landBottom; y++)
        {
            for (int x = landLeft; x < landRight; x++)
            {
                // Add some variation to make it look more natural
                bool isLand = true;

                // Cut out Florida peninsula (southeast)
                if (x > landRight - width / 10 && y > landBottom - height / 8)
                {
                    isLand = (x - (landRight - width / 10)) < (landBottom - y) / 2;
                }

                // Cut out Gulf of Mexico indent
                if (x > landRight - width / 4 && x < landRight - width / 10 && y > landBottom - height / 4)
                {
                    isLand = false;
                }

                if (isLand)
                {
                    SetPixel(pixels, width, height, x, y, landColor);
                }
            }
        }

        // Draw border around land
        for (int y = landTop; y < landBottom; y++)
        {
            for (int x = landLeft; x < landRight; x++)
            {
                // Check if this is a border pixel
                bool isBorder = false;
                Color currentPixel = pixels[y * width + x];

                if (currentPixel.Equals(landColor))
                {
                    // Check adjacent pixels
                    for (int dy = -1; dy <= 1; dy++)
                    {
                        for (int dx = -1; dx <= 1; dx++)
                        {
                            int nx = x + dx;
                            int ny = y + dy;
                            if (nx >= 0 && nx < width && ny >= 0 && ny < height)
                            {
                                if (pixels[ny * width + nx].Equals(oceanColor))
                                {
                                    isBorder = true;
                                    break;
                                }
                            }
                        }
                        if (isBorder)
                        {
                            break;
                        }
                    }

                    if (isBorder)
                    {
                        SetPixel(pixels, width, height, x, y, borderColor);
                    }
                }
            }
        }

        texture.SetData(pixels);
        return texture;
    }
}
