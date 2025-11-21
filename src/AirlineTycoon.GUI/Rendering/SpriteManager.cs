using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AirlineTycoon.GUI.Rendering;

/// <summary>
/// Manages sprite assets for the game, including aircraft and airport icons.
/// Provides procedurally generated pixel art sprites for visual representation.
/// </summary>
/// <remarks>
/// The SpriteManager handles:
/// - Loading and caching sprite textures
/// - Procedural generation of simple pixel art sprites
/// - Sprite lookup by aircraft type or airport
///
/// Similar to AudioManager, this uses procedural generation to avoid
/// needing external art assets during development.
///
/// Sprites are generated at small pixel art sizes (16x16 to 64x64) to maintain
/// the retro RollerCoaster Tycoon aesthetic.
/// </remarks>
public class SpriteManager
{
    private readonly GraphicsDevice graphicsDevice;
    private readonly Dictionary<string, Texture2D> sprites;

    /// <summary>
    /// Initializes a new instance of the <see cref="SpriteManager"/> class.
    /// </summary>
    /// <param name="graphicsDevice">The graphics device for creating textures.</param>
    public SpriteManager(GraphicsDevice graphicsDevice)
    {
        this.graphicsDevice = graphicsDevice;
        this.sprites = new Dictionary<string, Texture2D>();
    }

    /// <summary>
    /// Loads a sprite into the cache.
    /// </summary>
    /// <param name="spriteName">The name to register the sprite under.</param>
    /// <param name="texture">The texture to cache.</param>
    public void LoadSprite(string spriteName, Texture2D texture)
    {
        if (!this.sprites.ContainsKey(spriteName))
        {
            this.sprites[spriteName] = texture;
            System.Diagnostics.Debug.WriteLine($"Loaded sprite: {spriteName}");
        }
    }

    /// <summary>
    /// Gets a sprite by name.
    /// </summary>
    /// <param name="spriteName">The name of the sprite to retrieve.</param>
    /// <returns>The sprite texture, or null if not found.</returns>
    public Texture2D? GetSprite(string spriteName)
    {
        return this.sprites.TryGetValue(spriteName, out var sprite) ? sprite : null;
    }

    /// <summary>
    /// Generates all aircraft sprites procedurally.
    /// Creates simple pixel art representations for each aircraft type.
    /// </summary>
    public void GenerateAircraftSprites()
    {
        // Regional jet (Embraer E175) - Small, narrow body
        this.LoadSprite("aircraft_embraer_e175",
            SpriteGenerator.GenerateAircraftSprite(this.graphicsDevice, 48, 24,
                new Color(200, 200, 220), AircraftSize.Regional));

        // Narrow-body jets (Boeing 737, Airbus A320) - Medium, single aisle
        this.LoadSprite("aircraft_boeing_737",
            SpriteGenerator.GenerateAircraftSprite(this.graphicsDevice, 56, 28,
                new Color(180, 200, 230), AircraftSize.NarrowBody));

        this.LoadSprite("aircraft_airbus_a320",
            SpriteGenerator.GenerateAircraftSprite(this.graphicsDevice, 56, 28,
                new Color(210, 180, 200), AircraftSize.NarrowBody));

        // Wide-body jets (Boeing 787, Airbus A380) - Large, double aisle
        this.LoadSprite("aircraft_boeing_787",
            SpriteGenerator.GenerateAircraftSprite(this.graphicsDevice, 64, 32,
                new Color(170, 190, 210), AircraftSize.WideBody));

        this.LoadSprite("aircraft_airbus_a380",
            SpriteGenerator.GenerateAircraftSprite(this.graphicsDevice, 72, 36,
                new Color(190, 170, 200), AircraftSize.WideBody));

        System.Diagnostics.Debug.WriteLine("Generated all aircraft sprites");
    }

    /// <summary>
    /// Generates airport icon sprites.
    /// Creates simple pixel art icons for airports on the route map.
    /// </summary>
    public void GenerateAirportSprites()
    {
        // Hub airports (major international hubs) - Large icon
        this.LoadSprite("airport_hub",
            SpriteGenerator.GenerateAirportIcon(this.graphicsDevice, 24,
                RetroColorPalette.Info, AirportType.Hub));

        // Regional airports - Medium icon
        this.LoadSprite("airport_regional",
            SpriteGenerator.GenerateAirportIcon(this.graphicsDevice, 16,
                RetroColorPalette.Warning, AirportType.Regional));

        // Small airports - Small icon
        this.LoadSprite("airport_small",
            SpriteGenerator.GenerateAirportIcon(this.graphicsDevice, 12,
                RetroColorPalette.TextSecondary, AirportType.Small));

        System.Diagnostics.Debug.WriteLine("Generated all airport sprites");
    }

    /// <summary>
    /// Generates UI element sprites including 9-slice panels and icons.
    /// </summary>
    public void GenerateUISprites()
    {
        // Generate 9-slice panel templates
        this.LoadSprite("panel_9slice",
            SpriteGenerator.Generate9SlicePanel(
                this.graphicsDevice,
                48,
                RetroColorPalette.PanelShadow,
                RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.8f)
            ));

        System.Diagnostics.Debug.WriteLine("Generated UI sprites");
    }

    /// <summary>
    /// Generates the US map visualization.
    /// </summary>
    /// <param name="width">Map width in pixels.</param>
    /// <param name="height">Map height in pixels.</param>
    public void GenerateUSMapSprite(int width = 800, int height = 500)
    {
        this.LoadSprite("us_map", SpriteGenerator.GenerateUSMap(this.graphicsDevice, width, height));
        System.Diagnostics.Debug.WriteLine($"Generated US map sprite ({width}x{height})");
    }

    /// <summary>
    /// Unloads all sprite resources.
    /// Call this when disposing the SpriteManager.
    /// </summary>
    public void Unload()
    {
        foreach (var sprite in this.sprites.Values)
        {
            sprite.Dispose();
        }
        this.sprites.Clear();
    }
}

/// <summary>
/// Aircraft size categories for sprite generation.
/// </summary>
public enum AircraftSize
{
    /// <summary>Regional jet (70-100 seats)</summary>
    Regional,

    /// <summary>Narrow-body jet (150-200 seats)</summary>
    NarrowBody,

    /// <summary>Wide-body jet (250-500+ seats)</summary>
    WideBody
}

/// <summary>
/// Airport type categories for icon generation.
/// </summary>
public enum AirportType
{
    /// <summary>Major international hub</summary>
    Hub,

    /// <summary>Regional airport</summary>
    Regional,

    /// <summary>Small local airport</summary>
    Small
}
