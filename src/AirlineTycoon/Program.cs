using AirlineTycoon;
using AirlineTycoon.UI;

// Create and start the game
var game = new Game();
var ui = new ConsoleUI(game);

Console.OutputEncoding = System.Text.Encoding.UTF8; // Enable emoji support

ui.Run();
