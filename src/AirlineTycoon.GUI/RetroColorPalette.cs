using Microsoft.Xna.Framework;

namespace AirlineTycoon.GUI;

/// <summary>
/// Defines the retro color palette for Airline Tycoon GUI.
/// Inspired by RollerCoaster Tycoon's classic color scheme.
/// </summary>
/// <remarks>
/// RCT used a carefully crafted color palette that evoked:
/// - Natural park colors (greens, browns, blues)
/// - Vibrant but not overwhelming saturation
/// - Excellent readability for UI elements
/// - Distinct colors for different UI states
///
/// This palette recreates that aesthetic for an airline management game,
/// using aviation-appropriate colors (sky blues, airport grays, etc.)
/// </remarks>
public static class RetroColorPalette
{
    // === Primary UI Colors ===

    /// <summary>
    /// Dark blue used for window backgrounds and panels.
    /// Similar to RCT's menu background color.
    /// </summary>
    public static readonly Color WindowBackground = new(28, 40, 79);

    /// <summary>
    /// Light blue used for window title bars.
    /// </summary>
    public static readonly Color TitleBarBackground = new(60, 100, 180);

    /// <summary>
    /// Very light blue/gray for panel highlights.
    /// </summary>
    public static readonly Color PanelHighlight = new(140, 170, 220);

    /// <summary>
    /// Dark gray for panel shadows and borders.
    /// </summary>
    public static readonly Color PanelShadow = new(20, 25, 40);

    // === Button Colors ===

    /// <summary>
    /// Normal button background (medium blue-gray).
    /// </summary>
    public static readonly Color ButtonNormal = new(80, 90, 120);

    /// <summary>
    /// Hover button background (lighter blue).
    /// </summary>
    public static readonly Color ButtonHover = new(100, 120, 160);

    /// <summary>
    /// Pressed button background (darker blue).
    /// </summary>
    public static readonly Color ButtonPressed = new(60, 70, 100);

    /// <summary>
    /// Disabled button background (dark gray).
    /// </summary>
    public static readonly Color ButtonDisabled = new(50, 55, 70);

    /// <summary>
    /// Button border color (light blue-gray).
    /// </summary>
    public static readonly Color ButtonBorder = new(120, 140, 180);

    // === Text Colors ===

    /// <summary>
    /// Primary text color (nearly white, slightly warm).
    /// </summary>
    public static readonly Color TextPrimary = new(240, 240, 235);

    /// <summary>
    /// Secondary text color (light gray).
    /// </summary>
    public static readonly Color TextSecondary = new(180, 180, 175);

    /// <summary>
    /// Disabled text color (dark gray).
    /// </summary>
    public static readonly Color TextDisabled = new(100, 100, 100);

    /// <summary>
    /// Text shadow color (for crisp pixel text).
    /// </summary>
    public static readonly Color TextShadow = new(20, 20, 25);

    // === Status Colors ===

    /// <summary>
    /// Success/profit color (green).
    /// </summary>
    public static readonly Color Success = new(60, 180, 75);

    /// <summary>
    /// Warning color (yellow/orange).
    /// </summary>
    public static readonly Color Warning = new(240, 180, 40);

    /// <summary>
    /// Error/loss color (red).
    /// </summary>
    public static readonly Color Error = new(220, 60, 60);

    /// <summary>
    /// Info color (cyan/light blue).
    /// </summary>
    public static readonly Color Info = new(80, 180, 220);

    // === Game-Specific Colors ===

    /// <summary>
    /// Sky blue for backgrounds and aviation theme.
    /// </summary>
    public static readonly Color SkyBlue = new(135, 206, 235);

    /// <summary>
    /// Runway gray for airport elements.
    /// </summary>
    public static readonly Color RunwayGray = new(100, 100, 110);

    /// <summary>
    /// Aircraft white (slightly off-white for contrast).
    /// </summary>
    public static readonly Color AircraftWhite = new(230, 230, 240);

    /// <summary>
    /// Route line color (bright blue for visibility).
    /// </summary>
    public static readonly Color RouteLine = new(100, 150, 255);

    // === Special Effects ===

    /// <summary>
    /// Semi-transparent overlay for modal dialogs.
    /// </summary>
    public static readonly Color ModalOverlay = new(0, 0, 0, 180);

    /// <summary>
    /// Tooltip background (slightly transparent dark gray).
    /// </summary>
    public static readonly Color TooltipBackground = new(40, 40, 50, 240);

    /// <summary>
    /// Tooltip border (light gray).
    /// </summary>
    public static readonly Color TooltipBorder = new(180, 180, 190);

    /// <summary>
    /// Selection highlight (semi-transparent blue).
    /// </summary>
    public static readonly Color SelectionHighlight = new(100, 150, 255, 100);

    // === Helper Methods ===

    /// <summary>
    /// Creates a darker version of a color (for shadows and pressed states).
    /// </summary>
    /// <param name="color">Base color.</param>
    /// <param name="factor">Darkening factor (0.0 to 1.0, where 0.0 is black).</param>
    /// <returns>Darkened color.</returns>
    public static Color Darken(Color color, float factor)
    {
        factor = Math.Clamp(factor, 0f, 1f);
        return new Color(
            (int)(color.R * factor),
            (int)(color.G * factor),
            (int)(color.B * factor),
            color.A
        );
    }

    /// <summary>
    /// Creates a lighter version of a color (for highlights and hover states).
    /// </summary>
    /// <param name="color">Base color.</param>
    /// <param name="factor">Lightening factor (0.0 to 1.0, where 1.0 adds maximum brightness).</param>
    /// <returns>Lightened color.</returns>
    public static Color Lighten(Color color, float factor)
    {
        factor = Math.Clamp(factor, 0f, 1f);
        return new Color(
            (int)(color.R + (255 - color.R) * factor),
            (int)(color.G + (255 - color.G) * factor),
            (int)(color.B + (255 - color.B) * factor),
            color.A
        );
    }

    /// <summary>
    /// Creates a semi-transparent version of a color.
    /// </summary>
    /// <param name="color">Base color.</param>
    /// <param name="alpha">Alpha value (0 to 255).</param>
    /// <returns>Color with adjusted alpha.</returns>
    public static Color WithAlpha(Color color, byte alpha)
    {
        return new Color(color.R, color.G, color.B, alpha);
    }
}
