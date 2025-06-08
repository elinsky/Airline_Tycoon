namespace AirlineTycoon;

/// <summary>
/// Main game engine for Airline Tycoon.
/// </summary>
public class Game
{
    private bool isRunning;

    /// <summary>
    /// Gets the name of the game.
    /// </summary>
    public static string Name => "Airline Tycoon";

    /// <summary>
    /// Gets the current version of the game.
    /// </summary>
    public static string Version => "1.0.0";

    /// <summary>
    /// Gets a value indicating whether the game is currently running.
    /// </summary>
    public bool IsRunning => this.isRunning;

    /// <summary>
    /// Starts a new game session.
    /// </summary>
    public void Start()
    {
        this.isRunning = true;

        // Game initialization will be implemented here
    }

    /// <summary>
    /// Stops the current game session.
    /// </summary>
    public void Stop()
    {
        this.isRunning = false;
    }
}
