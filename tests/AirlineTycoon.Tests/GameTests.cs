using Xunit;

namespace AirlineTycoon.Tests;

public class GameTests
{
    [Fact]
    public void Game_Name_ReturnsCorrectValue()
    {
        // Act
        var name = Game.Name;

        // Assert
        Assert.Equal("Airline Tycoon", name);
    }

    [Fact]
    public void Game_Version_ReturnsCorrectValue()
    {
        // Act
        var version = Game.Version;

        // Assert
        Assert.Equal("1.0.0", version);
    }

    [Fact]
    public void Game_Start_SetsIsRunningToTrue()
    {
        // Arrange
        var game = new Game();

        // Act
        game.Start();

        // Assert
        Assert.True(game.IsRunning);
    }

    [Fact]
    public void Game_Stop_SetsIsRunningToFalse()
    {
        // Arrange
        var game = new Game();
        game.Start();

        // Act
        game.Stop();

        // Assert
        Assert.False(game.IsRunning);
    }

    [Fact]
    public void Game_InitialState_IsNotRunning()
    {
        // Arrange
        var game = new Game();

        // Assert
        Assert.False(game.IsRunning);
    }
}
