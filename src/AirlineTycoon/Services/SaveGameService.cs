using System.Text.Json;
using System.Text.Json.Serialization;
using AirlineTycoon.Domain;

namespace AirlineTycoon.Services;

/// <summary>
/// Provides functionality to save and load game state to/from JSON files.
/// Similar to RollerCoaster Tycoon's save system, allows players to resume games later.
/// </summary>
/// <remarks>
/// Save files are stored in JSON format in the user's game saves directory.
/// Each save includes:
/// - Complete airline state (fleet, routes, finances)
/// - Game metadata (save time, airline name, current day)
/// - All relationships (aircraft assignments, route configurations)
///
/// The service handles:
/// - Circular references (Route ↔ Aircraft) using reference preservation
/// - File I/O with proper error handling
/// - Save file management (list, delete)
/// </remarks>
public class SaveGameService
{
    private readonly string savesDirectory;
    private readonly JsonSerializerOptions jsonOptions;

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveGameService"/> class.
    /// </summary>
    /// <param name="savesDirectory">Optional custom saves directory. Defaults to "saves" in current directory.</param>
    public SaveGameService(string? savesDirectory = null)
    {
        // Default to "saves" folder in current directory
        this.savesDirectory = savesDirectory ?? Path.Combine(Directory.GetCurrentDirectory(), "saves");

        // Ensure saves directory exists
        Directory.CreateDirectory(this.savesDirectory);

        // Configure JSON serialization to handle circular references
        this.jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true, // Pretty-print for debugging
            ReferenceHandler = ReferenceHandler.Preserve, // Handle circular refs
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.Never
        };
    }

    /// <summary>
    /// Saves the current game state to a JSON file.
    /// </summary>
    /// <param name="game">The game to save.</param>
    /// <param name="saveName">The name for this save file (without extension).</param>
    /// <returns>The full path to the saved file.</returns>
    /// <exception cref="InvalidOperationException">Thrown if game has no active airline.</exception>
    /// <exception cref="IOException">Thrown if file cannot be written.</exception>
    public string SaveGame(Game game, string saveName)
    {
        if (game.PlayerAirline == null)
        {
            throw new InvalidOperationException("Cannot save game without an active airline.");
        }

        // Sanitize filename
        string safeFileName = SanitizeFileName(saveName);
        string filePath = Path.Combine(this.savesDirectory, $"{safeFileName}.json");

        // Create save data with metadata
        var saveData = new SaveGameData
        {
            SaveName = saveName,
            SavedAt = DateTime.UtcNow,
            GameVersion = Game.Version,
            PlayerAirline = game.PlayerAirline,
            CurrentScenario = game.CurrentScenario,
            HasWon = game.HasWon
        };

        // Serialize to JSON
        string json = JsonSerializer.Serialize(saveData, this.jsonOptions);

        // Write to file
        File.WriteAllText(filePath, json);

        return filePath;
    }

    /// <summary>
    /// Loads a game from a save file.
    /// </summary>
    /// <param name="saveName">The name of the save file (without extension).</param>
    /// <returns>A restored Game instance.</returns>
    /// <exception cref="FileNotFoundException">Thrown if save file doesn't exist.</exception>
    /// <exception cref="JsonException">Thrown if save file is corrupted.</exception>
    public Game LoadGame(string saveName)
    {
        string safeFileName = SanitizeFileName(saveName);
        string filePath = Path.Combine(this.savesDirectory, $"{safeFileName}.json");

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Save file not found: {saveName}", filePath);
        }

        // Read JSON
        string json = File.ReadAllText(filePath);

        // Deserialize
        var saveData = JsonSerializer.Deserialize<SaveGameData>(json, this.jsonOptions);

        if (saveData == null || saveData.PlayerAirline == null)
        {
            throw new JsonException("Save file is corrupted or invalid.");
        }

        // Reconstruct game
        var game = new Game();
        game.LoadFromSave(saveData.PlayerAirline, saveData.CurrentScenario, saveData.HasWon);

        return game;
    }

    /// <summary>
    /// Lists all available save files.
    /// </summary>
    /// <returns>A list of save file metadata.</returns>
    public List<SaveGameMetadata> ListSaves()
    {
        var saves = new List<SaveGameMetadata>();

        var files = Directory.GetFiles(this.savesDirectory, "*.json");

        foreach (var file in files)
        {
            try
            {
                string json = File.ReadAllText(file);
                var saveData = JsonSerializer.Deserialize<SaveGameData>(json, this.jsonOptions);

                if (saveData?.PlayerAirline != null)
                {
                    saves.Add(new SaveGameMetadata
                    {
                        SaveName = saveData.SaveName,
                        SavedAt = saveData.SavedAt,
                        AirlineName = saveData.PlayerAirline.Name,
                        CurrentDay = saveData.PlayerAirline.CurrentDay,
                        Cash = saveData.PlayerAirline.Cash,
                        Reputation = saveData.PlayerAirline.Reputation,
                        FileName = Path.GetFileNameWithoutExtension(file)
                    });
                }
            }
            catch
            {
                // Skip corrupted files
                continue;
            }
        }

        return saves.OrderByDescending(s => s.SavedAt).ToList();
    }

    /// <summary>
    /// Deletes a save file.
    /// </summary>
    /// <param name="saveName">The name of the save file to delete.</param>
    /// <returns>True if deleted successfully, false if file didn't exist.</returns>
    public bool DeleteSave(string saveName)
    {
        string safeFileName = SanitizeFileName(saveName);
        string filePath = Path.Combine(this.savesDirectory, $"{safeFileName}.json");

        if (File.Exists(filePath))
        {
            File.Delete(filePath);
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if a save file exists.
    /// </summary>
    /// <param name="saveName">The name of the save file.</param>
    /// <returns>True if the save exists.</returns>
    public bool SaveExists(string saveName)
    {
        string safeFileName = SanitizeFileName(saveName);
        string filePath = Path.Combine(this.savesDirectory, $"{safeFileName}.json");
        return File.Exists(filePath);
    }

    /// <summary>
    /// Sanitizes a filename by removing invalid characters.
    /// </summary>
    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = string.Join("_", fileName.Split(invalidChars));
        return sanitized;
    }
}

/// <summary>
/// Represents the data stored in a save file.
/// Contains the complete game state needed to restore a session.
/// </summary>
public class SaveGameData
{
    /// <summary>
    /// Gets or sets the name of this save.
    /// </summary>
    public required string SaveName { get; set; }

    /// <summary>
    /// Gets or sets when this save was created.
    /// </summary>
    public required DateTime SavedAt { get; set; }

    /// <summary>
    /// Gets or sets the game version that created this save.
    /// </summary>
    public required string GameVersion { get; set; }

    /// <summary>
    /// Gets or sets the player's airline state.
    /// </summary>
    public required Airline PlayerAirline { get; set; }

    /// <summary>
    /// Gets or sets the current scenario (null for free play).
    /// </summary>
    public Scenario? CurrentScenario { get; set; }

    /// <summary>
    /// Gets or sets whether the player has won.
    /// </summary>
    public bool HasWon { get; set; }
}

/// <summary>
/// Metadata about a save file, used for displaying the save list.
/// </summary>
public class SaveGameMetadata
{
    /// <summary>
    /// Gets or sets the save name.
    /// </summary>
    public required string SaveName { get; set; }

    /// <summary>
    /// Gets or sets when this was saved.
    /// </summary>
    public required DateTime SavedAt { get; set; }

    /// <summary>
    /// Gets or sets the airline name.
    /// </summary>
    public required string AirlineName { get; set; }

    /// <summary>
    /// Gets or sets the current day.
    /// </summary>
    public required int CurrentDay { get; set; }

    /// <summary>
    /// Gets or sets the cash balance.
    /// </summary>
    public required decimal Cash { get; set; }

    /// <summary>
    /// Gets or sets the reputation.
    /// </summary>
    public required int Reputation { get; set; }

    /// <summary>
    /// Gets or sets the filename (without extension).
    /// </summary>
    public required string FileName { get; set; }
}
