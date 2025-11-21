using AirlineTycoon.GUI.UI;
using AirlineTycoon.Services;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AirlineTycoon.GUI.Screens;

/// <summary>
/// Screen for saving and loading games.
/// Displays list of existing saves with metadata and provides save/load/delete functionality.
/// </summary>
public class SaveLoadScreen : Screen
{
    private List<SaveGameMetadata> saves;
    private List<UIButton> saveButtons;
    private List<UIButton> loadButtons;
    private List<UIButton> deleteButtons;
    private UIButton backButton;
    private UIButton newSaveButton;
    private string newSaveName;
    private bool isEnteringSaveName;
    private int selectedSaveIndex;

    /// <summary>
    /// Gets the screen title.
    /// </summary>
    public override string Title => "Save / Load Game";

    /// <summary>
    /// Initializes a new instance of the <see cref="SaveLoadScreen"/> class.
    /// </summary>
    /// <param name="controller">The game controller.</param>
    public SaveLoadScreen(GameController controller)
    {
        this.SetGameController(controller);
        this.saves = new List<SaveGameMetadata>();
        this.saveButtons = new List<UIButton>();
        this.loadButtons = new List<UIButton>();
        this.deleteButtons = new List<UIButton>();
        this.newSaveName = string.Empty;
        this.isEnteringSaveName = false;
        this.selectedSaveIndex = -1;

        this.InitializeUI();
    }

    private void InitializeUI()
    {
        // Back button
        this.backButton = new UIButton
        {
            Position = new Vector2(20, 660),
            Size = new Vector2(120, 40),
            Text = "Back",
            OnClick = () => this.Controller?.ShowDashboard()
        };

        // New Save button
        this.newSaveButton = new UIButton
        {
            Position = new Vector2(1140, 660),
            Size = new Vector2(120, 40),
            Text = "New Save",
            OnClick = this.StartNewSave
        };

        // Load save list
        this.RefreshSaveList();
    }

    private void RefreshSaveList()
    {
        if (this.Controller == null)
        {
            return;
        }

        this.saves = this.Controller.ListSaves();
        this.saveButtons.Clear();
        this.loadButtons.Clear();
        this.deleteButtons.Clear();

        // Create buttons for each save
        for (int i = 0; i < Math.Min(this.saves.Count, 8); i++)
        {
            int index = i;
            var save = this.saves[i];

            // Load button
            var loadBtn = new UIButton
            {
                Position = new Vector2(50, 100 + i * 65),
                Size = new Vector2(120, 50),
                Text = "Load",
                OnClick = () => this.LoadSave(index)
            };
            this.loadButtons.Add(loadBtn);

            // Save/Overwrite button
            var saveBtn = new UIButton
            {
                Position = new Vector2(940, 100 + i * 65),
                Size = new Vector2(140, 50),
                Text = "Overwrite",
                OnClick = () => this.OverwriteSave(index)
            };
            this.saveButtons.Add(saveBtn);

            // Delete button
            var deleteBtn = new UIButton
            {
                Position = new Vector2(1090, 100 + i * 65),
                Size = new Vector2(120, 50),
                Text = "Delete",
                OnClick = () => this.DeleteSave(index)
            };
            this.deleteButtons.Add(deleteBtn);
        }
    }

    private void StartNewSave()
    {
        this.isEnteringSaveName = true;
        this.newSaveName = $"Save_{DateTime.Now:yyyyMMdd_HHmmss}";
    }

    private void LoadSave(int index)
    {
        if (index < 0 || index >= this.saves.Count)
        {
            return;
        }

        var save = this.saves[index];
        bool success = this.Controller?.LoadGame(save.FileName) ?? false;

        if (!success)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load save: {save.SaveName}");
        }
    }

    private void OverwriteSave(int index)
    {
        if (index < 0 || index >= this.saves.Count)
        {
            return;
        }

        var save = this.saves[index];
        bool success = this.Controller?.SaveGame(save.FileName) ?? false;

        if (success)
        {
            this.RefreshSaveList();
        }
    }

    private void DeleteSave(int index)
    {
        if (index < 0 || index >= this.saves.Count)
        {
            return;
        }

        var save = this.saves[index];
        bool success = this.Controller?.DeleteSave(save.FileName) ?? false;

        if (success)
        {
            this.RefreshSaveList();
        }
    }

    private void CompleteSave()
    {
        if (string.IsNullOrWhiteSpace(this.newSaveName))
        {
            this.isEnteringSaveName = false;
            return;
        }

        bool success = this.Controller?.SaveGame(this.newSaveName) ?? false;

        if (success)
        {
            this.isEnteringSaveName = false;
            this.newSaveName = string.Empty;
            this.RefreshSaveList();
        }
    }

    /// <summary>
    /// Updates the screen state.
    /// </summary>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        this.backButton.Update(gameTime);
        this.newSaveButton.Update(gameTime);

        foreach (var btn in this.loadButtons)
        {
            btn.Update(gameTime);
        }

        foreach (var btn in this.saveButtons)
        {
            btn.Update(gameTime);
        }

        foreach (var btn in this.deleteButtons)
        {
            btn.Update(gameTime);
        }

        // Handle text input for new save name
        if (this.isEnteringSaveName)
        {
            this.HandleTextInput();
        }
    }

    private void HandleTextInput()
    {
        var keyboardState = Keyboard.GetState();

        // Enter key completes the save
        if (keyboardState.IsKeyDown(Keys.Enter))
        {
            this.CompleteSave();
            return;
        }

        // Escape cancels
        if (keyboardState.IsKeyDown(Keys.Escape))
        {
            this.isEnteringSaveName = false;
            this.newSaveName = string.Empty;
            return;
        }

        // Note: Full text input would require tracking key states
        // For now, we'll use the auto-generated name
    }

    /// <summary>
    /// Draws the screen.
    /// </summary>
    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (AirlineTycoonGame.TextRenderer == null || AirlineTycoonGame.WhitePixel == null)
        {
            return;
        }

        // Draw top bar
        this.DrawTopBar(spriteBatch, gameTime);

        // Draw main panel background
        var panelBounds = new Rectangle(20, 50, (int)this.Size.X - 40, 600);
        this.DrawFilledRectangle(spriteBatch, panelBounds, RetroColorPalette.PanelBackground);
        this.Draw3DBorder(spriteBatch, panelBounds);

        // Draw header
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            "Saved Games",
            new Vector2(40, 60),
            RetroColorPalette.TextPrimary,
            shadow: true
        );

        // Draw save list
        this.DrawSaveList(spriteBatch);

        // Draw new save input if active
        if (this.isEnteringSaveName)
        {
            this.DrawSaveNameInput(spriteBatch);
        }

        // Draw buttons
        this.backButton.Draw(spriteBatch, gameTime);
        this.newSaveButton.Draw(spriteBatch, gameTime);

        foreach (var btn in this.loadButtons)
        {
            btn.Draw(spriteBatch, gameTime);
        }

        foreach (var btn in this.saveButtons)
        {
            btn.Draw(spriteBatch, gameTime);
        }

        foreach (var btn in this.deleteButtons)
        {
            btn.Draw(spriteBatch, gameTime);
        }
    }

    private void DrawSaveList(SpriteBatch spriteBatch)
    {
        if (AirlineTycoonGame.TextRenderer == null)
        {
            return;
        }

        for (int i = 0; i < Math.Min(this.saves.Count, 8); i++)
        {
            var save = this.saves[i];
            int y = 100 + i * 65;

            // Draw save entry background
            var entryBounds = new Rectangle(180, y, 750, 50);
            this.DrawFilledRectangle(spriteBatch, entryBounds, RetroColorPalette.Darken(RetroColorPalette.PanelBackground, 0.8f));
            this.Draw3DBorder(spriteBatch, entryBounds, 1);

            // Draw save name
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                save.SaveName,
                new Vector2(190, y + 5),
                RetroColorPalette.TextPrimary
            );

            // Draw airline name and day
            string info = $"{save.AirlineName} - Day {save.CurrentDay}";
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                info,
                new Vector2(190, y + 23),
                RetroColorPalette.TextSecondary
            );

            // Draw cash and date
            string details = $"{Rendering.TextRenderer.FormatCurrency(save.Cash)} - {save.SavedAt:yyyy-MM-dd HH:mm}";
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                details,
                new Vector2(520, y + 23),
                RetroColorPalette.TextSecondary
            );
        }

        // Show message if no saves
        if (this.saves.Count == 0)
        {
            AirlineTycoonGame.TextRenderer.DrawCenteredText(
                spriteBatch,
                "No saved games found",
                new Rectangle(180, 300, 750, 50),
                RetroColorPalette.TextSecondary
            );
        }
    }

    private void DrawSaveNameInput(SpriteBatch spriteBatch)
    {
        if (AirlineTycoonGame.TextRenderer == null)
        {
            return;
        }

        // Draw overlay
        this.DrawFilledRectangle(spriteBatch, new Rectangle(0, 0, (int)this.Size.X, (int)this.Size.Y),
            new Color(0, 0, 0, 180));

        // Draw input panel
        var inputPanelBounds = new Rectangle(340, 280, 600, 160);
        this.DrawFilledRectangle(spriteBatch, inputPanelBounds, RetroColorPalette.PanelBackground);
        this.Draw3DBorder(spriteBatch, inputPanelBounds, 3);

        // Draw title
        AirlineTycoonGame.TextRenderer.DrawCenteredText(
            spriteBatch,
            "Enter Save Name",
            new Rectangle(340, 295, 600, 30),
            RetroColorPalette.TextPrimary,
            shadow: true
        );

        // Draw input box
        var inputBoxBounds = new Rectangle(360, 340, 560, 40);
        this.DrawFilledRectangle(spriteBatch, inputBoxBounds, Color.White);
        this.Draw3DBorder(spriteBatch, inputBoxBounds, 2);

        // Draw save name
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            this.newSaveName,
            new Vector2(370, 350),
            Color.Black
        );

        // Draw instructions
        AirlineTycoonGame.TextRenderer.DrawCenteredText(
            spriteBatch,
            "Press ENTER to save, ESC to cancel",
            new Rectangle(340, 395, 600, 30),
            RetroColorPalette.TextSecondary
        );
    }
}
