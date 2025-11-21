using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace AirlineTycoon.GUI;

/// <summary>
/// Main game class for Airline Tycoon GUI.
/// Handles pixel-perfect rendering and core game loop.
/// </summary>
/// <remarks>
/// Implements RollerCoaster Tycoon-style pixel art rendering:
/// - Base resolution: 1280x720 (16:9 aspect ratio)
/// - Pixel-perfect scaling to maintain crisp retro aesthetic
/// - No texture filtering (Point sampling for sharp pixels)
///
/// The game uses a render target approach for pixel-perfect scaling:
/// 1. Render everything to a 1280x720 render target
/// 2. Scale the render target to fit the window while maintaining aspect ratio
/// 3. Use Point sampling to keep pixels sharp (no blurring)
/// </remarks>
public class AirlineTycoonGame : Game
{
    private readonly GraphicsDeviceManager graphics;
    private SpriteBatch spriteBatch = null!;
    private RenderTarget2D renderTarget = null!;

    /// <summary>
    /// Base resolution for pixel-perfect rendering.
    /// All UI elements are designed for this resolution.
    /// </summary>
    public const int BaseWidth = 1280;

    /// <summary>
    /// Base resolution height.
    /// </summary>
    public const int BaseHeight = 720;

    /// <summary>
    /// Gets the current scale factor for the render target.
    /// Used to convert mouse coordinates from screen space to game space.
    /// </summary>
    public float Scale { get; private set; }

    /// <summary>
    /// Gets the offset for letterboxing (black bars when aspect ratio doesn't match).
    /// </summary>
    public Vector2 RenderOffset { get; private set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="AirlineTycoonGame"/> class.
    /// </summary>
    public AirlineTycoonGame()
    {
        this.graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;

        // Set window to 2x base resolution by default (feels good on modern displays)
        this.graphics.PreferredBackBufferWidth = BaseWidth * 2;
        this.graphics.PreferredBackBufferHeight = BaseHeight * 2;

        // Allow window resizing
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += this.OnWindowResize;
    }

    /// <summary>
    /// Initializes the game.
    /// Creates the render target for pixel-perfect rendering.
    /// </summary>
    protected override void Initialize()
    {
        // Calculate initial scale and offset
        this.CalculateRenderScaling();

        base.Initialize();
    }

    /// <summary>
    /// Loads game content and creates render targets.
    /// </summary>
    protected override void LoadContent()
    {
        this.spriteBatch = new SpriteBatch(GraphicsDevice);

        // Create render target at base resolution
        this.renderTarget = new RenderTarget2D(
            GraphicsDevice,
            BaseWidth,
            BaseHeight,
            false,
            SurfaceFormat.Color,
            DepthFormat.None,
            0,
            RenderTargetUsage.DiscardContents
        );
    }

    /// <summary>
    /// Updates game logic.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Update(GameTime gameTime)
    {
        // Exit on Escape key
        if (Keyboard.GetState().IsKeyDown(Keys.Escape))
        {
            Exit();
        }

        // TODO: Add game update logic here
        // This will eventually process:
        // - Input handling (mouse clicks, keyboard)
        // - Screen state management (main menu, dashboard, route screen, etc.)
        // - Game simulation updates

        base.Update(gameTime);
    }

    /// <summary>
    /// Renders the game.
    /// Uses pixel-perfect scaling technique to maintain retro aesthetic.
    /// </summary>
    /// <param name="gameTime">Provides a snapshot of timing values.</param>
    protected override void Draw(GameTime gameTime)
    {
        // Step 1: Render everything to the base resolution render target
        GraphicsDevice.SetRenderTarget(this.renderTarget);
        GraphicsDevice.Clear(Color.Black);

        this.spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp, // CRITICAL: Point sampling for pixel-perfect rendering
            DepthStencilState.None,
            RasterizerState.CullNone
        );

        // TODO: Draw game screens here
        // For now, draw a test pattern to verify pixel-perfect rendering
        this.DrawTestPattern();

        this.spriteBatch.End();

        // Step 2: Draw the render target to the back buffer with scaling
        GraphicsDevice.SetRenderTarget(null);
        GraphicsDevice.Clear(Color.Black);

        this.spriteBatch.Begin(
            SpriteSortMode.Deferred,
            BlendState.AlphaBlend,
            SamplerState.PointClamp, // CRITICAL: Point sampling maintains sharp pixels when scaling
            DepthStencilState.None,
            RasterizerState.CullNone
        );

        // Draw the render target scaled to fit the window
        this.spriteBatch.Draw(
            this.renderTarget,
            this.RenderOffset,
            null,
            Color.White,
            0f,
            Vector2.Zero,
            this.Scale,
            SpriteEffects.None,
            0f
        );

        this.spriteBatch.End();

        base.Draw(gameTime);
    }

    /// <summary>
    /// Draws a test pattern to verify pixel-perfect rendering.
    /// Will be removed once UI framework is implemented.
    /// </summary>
    private void DrawTestPattern()
    {
        // Draw a simple pixel grid to verify sharp rendering
        // This will be replaced with actual UI once the framework is built

        // TODO: Remove this once UI framework is implemented
        // For now, just clear to a dark blue (RCT-like color)
        GraphicsDevice.Clear(new Color(28, 40, 79)); // Dark blue similar to RCT menu background
    }

    /// <summary>
    /// Handles window resize events.
    /// Recalculates scaling to maintain pixel-perfect rendering.
    /// </summary>
    private void OnWindowResize(object? sender, EventArgs e)
    {
        this.CalculateRenderScaling();
    }

    /// <summary>
    /// Calculates the scale factor and offset for pixel-perfect rendering.
    /// Ensures the game renders at the correct size while maintaining aspect ratio.
    /// </summary>
    /// <remarks>
    /// Uses the "fit" approach:
    /// - Scales the render target to fit within the window
    /// - Maintains 16:9 aspect ratio
    /// - Adds letterboxing (black bars) if aspect ratios don't match
    /// - Always uses integer scaling for perfect pixels (optional, can be float for smoother scaling)
    /// </remarks>
    private void CalculateRenderScaling()
    {
        int windowWidth = this.graphics.PreferredBackBufferWidth;
        int windowHeight = this.graphics.PreferredBackBufferHeight;

        // Calculate scale factors for width and height
        float scaleX = (float)windowWidth / BaseWidth;
        float scaleY = (float)windowHeight / BaseHeight;

        // Use the smaller scale to fit within the window (maintains aspect ratio)
        this.Scale = Math.Min(scaleX, scaleY);

        // For ultra-crisp pixels, use integer scaling (uncomment if desired)
        // this.Scale = (float)Math.Floor(this.Scale);

        // Calculate the actual rendered size
        float renderedWidth = BaseWidth * this.Scale;
        float renderedHeight = BaseHeight * this.Scale;

        // Calculate offset to center the image (letterboxing)
        float offsetX = (windowWidth - renderedWidth) / 2f;
        float offsetY = (windowHeight - renderedHeight) / 2f;

        this.RenderOffset = new Vector2(offsetX, offsetY);
    }

    /// <summary>
    /// Converts screen coordinates (mouse position) to game coordinates.
    /// Accounts for scaling and letterboxing.
    /// </summary>
    /// <param name="screenPosition">Mouse position in screen space.</param>
    /// <returns>Position in game space (0,0 to BaseWidth,BaseHeight).</returns>
    public Vector2 ScreenToGameCoordinates(Vector2 screenPosition)
    {
        // Subtract the render offset and divide by scale
        float gameX = (screenPosition.X - this.RenderOffset.X) / this.Scale;
        float gameY = (screenPosition.Y - this.RenderOffset.Y) / this.Scale;

        return new Vector2(gameX, gameY);
    }

    /// <summary>
    /// Disposes of game resources.
    /// </summary>
    /// <param name="disposing">Whether to dispose managed resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            this.renderTarget?.Dispose();
            this.spriteBatch?.Dispose();
        }

        base.Dispose(disposing);
    }
}
