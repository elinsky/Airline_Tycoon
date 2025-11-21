using System.Text;
using AirlineTycoon.Domain;
using AirlineTycoon.Domain.Events;
using AirlineTycoon.Services;

namespace AirlineTycoon.UI;

/// <summary>
/// Provides a console-based user interface for Airline Tycoon.
/// Inspired by RollerCoaster Tycoon's clear information presentation and intuitive menus.
/// </summary>
/// <remarks>
/// The console UI provides:
/// - Clear status dashboard (like RCT's top bar showing park info)
/// - Menu-driven interaction (like RCT's construction and management menus)
/// - Daily operation reports (like RCT's monthly financial report)
/// - Route and fleet management screens
///
/// While this is a text interface, it follows RCT's principles:
/// - Show important info at a glance
/// - Make common actions easily accessible
/// - Provide detailed reports when needed
/// - Give immediate feedback on actions
/// </remarks>
public class ConsoleUI
{
    private readonly Game game;
    private readonly SaveGameService saveService;

    /// <summary>
    /// Initializes a new instance of the <see cref="ConsoleUI"/> class.
    /// </summary>
    /// <param name="game">The game instance to display UI for.</param>
    public ConsoleUI(Game game)
    {
        this.game = game;
        this.saveService = new SaveGameService();
    }

    /// <summary>
    /// Runs the main game loop with the console UI.
    /// Like RCT's main game screen, this is the central hub for all interactions.
    /// </summary>
    public void Run()
    {
        Console.Clear();
        this.ShowWelcomeScreen();

        // Main game loop
        while (this.game.IsRunning && !this.game.HasLost && !this.game.HasWon)
        {
            Console.Clear();
            this.ShowDashboard();
            this.ShowMainMenu();

            string? input = Console.ReadLine();
            this.HandleMainMenuInput(input);
        }

        // Game over - show final screen
        if (this.game.HasWon)
        {
            this.ShowVictoryScreen();
        }
        else if (this.game.HasLost)
        {
            this.ShowGameOverScreen();
        }
    }

    /// <summary>
    /// Shows the welcome screen with game title and start options.
    /// </summary>
    private void ShowWelcomeScreen()
    {
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                                                            ║");
        Console.WriteLine("║              ✈️  AIRLINE TYCOON  ✈️                        ║");
        Console.WriteLine("║                                                            ║");
        Console.WriteLine("║          Build Your Aviation Empire!                      ║");
        Console.WriteLine("║                                                            ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("Welcome to Airline Tycoon!");
        Console.WriteLine("Inspired by the legendary RollerCoaster Tycoon,");
        Console.WriteLine("your goal is to build a profitable airline empire.");
        Console.WriteLine();
        Console.WriteLine("[1] New Game");
        Console.WriteLine("[2] Load Game");
        Console.WriteLine("[3] Quit");
        Console.WriteLine();
        Console.Write("Enter your choice: ");
        string? choice = Console.ReadLine();

        switch (choice?.Trim())
        {
            case "1":
                this.StartNewGameFlow();
                break;
            case "2":
                this.LoadGameFlow();
                break;
            case "3":
                Environment.Exit(0);
                break;
            default:
                Console.WriteLine("Invalid choice. Starting new game...");
                System.Threading.Thread.Sleep(1000);
                this.StartNewGameFlow();
                break;
        }
    }

    /// <summary>
    /// Handles the new game flow (asking for airline name, starting game).
    /// </summary>
    private void StartNewGameFlow()
    {
        Console.Clear();
        Console.WriteLine("═══════════════════ NEW GAME ═══════════════════");
        Console.WriteLine();
        Console.Write("Enter your airline name (or press Enter for 'SkyWings Airlines'): ");
        string? airlineName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(airlineName))
        {
            airlineName = "SkyWings Airlines";
        }

        Console.WriteLine();
        Console.WriteLine("Starting your airline at JFK (New York)...");
        Console.WriteLine("You begin with $5,000,000 and 2 leased aircraft.");
        Console.WriteLine();
        Console.WriteLine("Press any key to start...");
        Console.ReadKey();

        this.game.StartNewGame(airlineName);
    }

    /// <summary>
    /// Handles the load game flow (showing saved games, loading selected game).
    /// </summary>
    private void LoadGameFlow()
    {
        Console.Clear();
        Console.WriteLine("═══════════════════ LOAD GAME ═══════════════════");
        Console.WriteLine();

        var saves = this.saveService.ListSaves();

        if (saves.Count == 0)
        {
            Console.WriteLine("No saved games found.");
            Console.WriteLine();
            Console.WriteLine("Press any key to return to main menu...");
            Console.ReadKey();
            this.ShowWelcomeScreen();
            return;
        }

        Console.WriteLine("Available Saves:");
        Console.WriteLine();
        for (int i = 0; i < saves.Count; i++)
        {
            var save = saves[i];
            Console.WriteLine($"[{i + 1}] {save.AirlineName}");
            Console.WriteLine($"    Day {save.CurrentDay} | Cash: ${save.Cash:N0} | Reputation: {save.Reputation}/100");
            Console.WriteLine($"    Saved: {save.SavedAt.ToLocalTime():g}");
            Console.WriteLine();
        }

        Console.WriteLine("[B] Back to Main Menu");
        Console.WriteLine();
        Console.Write("Select save to load: ");
        string? input = Console.ReadLine();

        if (input?.ToUpper() == "B")
        {
            this.ShowWelcomeScreen();
            return;
        }

        if (int.TryParse(input, out int index) && index >= 1 && index <= saves.Count)
        {
            var selectedSave = saves[index - 1];
            try
            {
                Console.WriteLine();
                Console.WriteLine($"Loading {selectedSave.AirlineName}...");

                var loadedGame = this.saveService.LoadGame(selectedSave.FileName);

                // Replace the current game with the loaded one
                // We need to transfer the loaded state to our current game instance
                this.game.LoadFromSave(
                    loadedGame.PlayerAirline!,
                    loadedGame.CurrentScenario,
                    loadedGame.HasWon
                );

                Console.WriteLine("✓ Game loaded successfully!");
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"✗ Error loading game: {ex.Message}");
                Console.WriteLine();
                Console.WriteLine("Press any key to return to main menu...");
                Console.ReadKey();
                this.ShowWelcomeScreen();
            }
        }
        else
        {
            Console.WriteLine("Invalid selection.");
            Console.WriteLine();
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
            this.LoadGameFlow();
        }
    }

    /// <summary>
    /// Shows the main dashboard with key airline statistics.
    /// Like RCT's status bar, this gives an at-a-glance view of performance.
    /// </summary>
    private void ShowDashboard()
    {
        var airline = this.game.PlayerAirline!;

        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine($"║  DAY {airline.CurrentDay,-3}  │  {airline.Name,-40}  ║");
        Console.WriteLine("╠════════════════════════════════════════════════════════════╣");
        Console.WriteLine(
            $"║  Cash: {FormatCurrency(airline.Cash),-15} │ Reputation: {GetReputationStars(airline.Reputation),-10} ({airline.Reputation}/100)  ║"
        );
        Console.WriteLine(
            $"║  Fleet: {airline.Fleet.Count} aircraft{(airline.Fleet.Count == 1 ? " " : "s"),-8} │ Routes: {airline.Routes.Count(r => r.IsActive)} active              ║"
        );
        Console.WriteLine(
            $"║  Total Passengers: {airline.TotalPassengersCarried,-10:N0}                          ║"
        );
        Console.WriteLine(
            $"║  Net Profit: {FormatCurrency(airline.NetProfit),-15}                        ║"
        );
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
    }

    /// <summary>
    /// Shows the main menu with available actions.
    /// </summary>
    private void ShowMainMenu()
    {
        Console.WriteLine("┌─────────────── MAIN MENU ───────────────┐");
        Console.WriteLine("│                                         │");
        Console.WriteLine("│  [1] View Routes                        │");
        Console.WriteLine("│  [2] View Fleet                         │");
        Console.WriteLine("│  [3] Open New Route                     │");
        Console.WriteLine("│  [4] Buy/Lease Aircraft                 │");
        Console.WriteLine("│  [5] View Financial Report              │");
        Console.WriteLine("│  [6] Advance Day ⏭                      │");
        Console.WriteLine("│  [7] Save Game                          │");
        Console.WriteLine("│  [8] Quit                               │");
        Console.WriteLine("│                                         │");
        Console.WriteLine("└─────────────────────────────────────────┘");
        Console.WriteLine();
        Console.Write("Enter your choice: ");
    }

    /// <summary>
    /// Handles main menu input and routes to appropriate actions.
    /// </summary>
    private void HandleMainMenuInput(string? input)
    {
        switch (input?.Trim())
        {
            case "1":
                this.ShowRoutesScreen();
                break;
            case "2":
                this.ShowFleetScreen();
                break;
            case "3":
                this.ShowOpenRouteScreen();
                break;
            case "4":
                this.ShowBuyAircraftScreen();
                break;
            case "5":
                this.ShowFinancialReport();
                break;
            case "6":
                this.AdvanceDay();
                break;
            case "7":
                this.SaveGame();
                break;
            case "8":
                this.game.Stop();
                break;
            default:
                Console.WriteLine("Invalid choice. Press any key to continue...");
                Console.ReadKey();
                break;
        }
    }

    /// <summary>
    /// Shows all routes with performance metrics.
    /// Like RCT's ride list, this shows how each route is performing.
    /// </summary>
    private void ShowRoutesScreen()
    {
        Console.Clear();
        var airline = this.game.PlayerAirline!;

        Console.WriteLine("═══════════════════ ROUTE NETWORK ═══════════════════");
        Console.WriteLine();

        if (airline.Routes.Count == 0)
        {
            Console.WriteLine("No routes opened yet. Open your first route from the main menu!");
        }
        else
        {
            Console.WriteLine(
                "Route              Aircraft       Load    Daily Profit   Pax Total"
            );
            Console.WriteLine(
                "────────────────────────────────────────────────────────────────────"
            );

            foreach (var route in airline.Routes.OrderByDescending(r => r.DailyProfit))
            {
                string aircraftName = route.AssignedAircraft?.Type.Name ?? "NONE";
                string loadFactor = route.AssignedAircraft != null ? $"{route.LoadFactor:P0}" : "N/A";
                string profit = FormatCurrency(route.DailyProfit);
                string status = route.IsActive ? "" : " [CLOSED]";

                Console.WriteLine(
                    $"{route.Name,-18} {aircraftName,-14} {loadFactor,4}  {profit,13}  {route.TotalPassengers,9:N0}{status}"
                );
            }
        }

        Console.WriteLine();
        Console.WriteLine("[C] Close a route  |  [A] Assign aircraft  |  [Back] Return");
        Console.Write("Choice: ");

        string? input = Console.ReadLine();
        if (input?.ToUpper() == "C")
        {
            this.CloseRouteInteractive();
        }
        else if (input?.ToUpper() == "A")
        {
            this.AssignAircraftInteractive();
        }
    }

    /// <summary>
    /// Shows fleet with aircraft details and assignments.
    /// </summary>
    private void ShowFleetScreen()
    {
        Console.Clear();
        var airline = this.game.PlayerAirline!;

        Console.WriteLine("═══════════════════ FLEET MANAGEMENT ═══════════════════");
        Console.WriteLine();

        if (airline.Fleet.Count == 0)
        {
            Console.WriteLine("No aircraft in fleet. Buy or lease aircraft from the main menu!");
        }
        else
        {
            Console.WriteLine("Reg Number  Type            Assignment           Condition  Lease/Own");
            Console.WriteLine(
                "────────────────────────────────────────────────────────────────────────"
            );

            foreach (var aircraft in airline.Fleet)
            {
                string assignment = aircraft.AssignedRoute?.Name ?? "UNASSIGNED";
                string condition = $"{aircraft.Condition:P0}";
                string ownership = aircraft.IsLeased
                    ? $"Lease ${aircraft.MonthlyLeasePayment:N0}/mo"
                    : "Owned";

                Console.WriteLine(
                    $"{aircraft.RegistrationNumber,-11} {aircraft.Type.Name,-15} {assignment,-20} {condition,9}  {ownership}"
                );
            }

            Console.WriteLine();
            decimal totalLeaseCost = airline.Fleet
                .Where(a => a.IsLeased)
                .Sum(a => a.MonthlyLeasePayment);
            if (totalLeaseCost > 0)
            {
                Console.WriteLine($"Total Monthly Lease Cost: ${totalLeaseCost:N0}");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
    }

    /// <summary>
    /// Shows screen for opening a new route.
    /// </summary>
    private void ShowOpenRouteScreen()
    {
        Console.Clear();
        var airline = this.game.PlayerAirline!;

        Console.WriteLine("═══════════════════ OPEN NEW ROUTE ═══════════════════");
        Console.WriteLine();
        Console.WriteLine("Available Airports:");
        Console.WriteLine();

        var airports = Airport.Catalog.All.OrderBy(a => a.Code).ToList();
        for (int i = 0; i < airports.Count; i++)
        {
            var airport = airports[i];
            string hubMarker = airport.IsHub ? "★" : " ";
            Console.WriteLine(
                $"[{i + 1,2}] {airport.Code} - {airport.City,-20} (Market: {airport.MarketSize,-10} Fee: ${airport.LandingFee:N0}) {hubMarker}"
            );
        }

        Console.WriteLine();
        Console.Write("Select origin airport (number or 'cancel'): ");
        string? originInput = Console.ReadLine();
        if (originInput?.ToLower() == "cancel")
        {
            return;
        }

        if (!int.TryParse(originInput, out int originIndex) || originIndex < 1 || originIndex > airports.Count)
        {
            Console.WriteLine("Invalid selection.");
            Console.ReadKey();
            return;
        }

        Console.Write("Select destination airport (number or 'cancel'): ");
        string? destInput = Console.ReadLine();
        if (destInput?.ToLower() == "cancel")
        {
            return;
        }

        if (!int.TryParse(destInput, out int destIndex) || destIndex < 1 || destIndex > airports.Count)
        {
            Console.WriteLine("Invalid selection.");
            Console.ReadKey();
            return;
        }

        var origin = airports[originIndex - 1];
        var destination = airports[destIndex - 1];

        if (origin.Code == destination.Code)
        {
            Console.WriteLine("Origin and destination cannot be the same!");
            Console.ReadKey();
            return;
        }

        int distance = Route.CalculateDistance(origin.Code, destination.Code);
        decimal suggestedPrice = CalculateSuggestedTicketPrice(distance);

        Console.WriteLine();
        Console.WriteLine($"Route: {origin.Code} → {destination.Code}");
        Console.WriteLine($"Distance: {distance:N0} nautical miles");
        Console.WriteLine($"Suggested ticket price: ${suggestedPrice:N0}");
        Console.WriteLine();
        Console.Write($"Enter ticket price (or press Enter for ${suggestedPrice:N0}): $");
        string? priceInput = Console.ReadLine();

        decimal ticketPrice = suggestedPrice;
        if (!string.IsNullOrWhiteSpace(priceInput) && decimal.TryParse(priceInput, out decimal customPrice))
        {
            ticketPrice = customPrice;
        }

        var route = airline.OpenRoute(origin, destination, ticketPrice);
        Console.WriteLine();
        Console.WriteLine($"✓ Route {route.Name} opened successfully!");
        Console.WriteLine("Don't forget to assign an aircraft to this route.");
        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    /// <summary>
    /// Shows screen for buying or leasing aircraft.
    /// </summary>
    private void ShowBuyAircraftScreen()
    {
        Console.Clear();
        var airline = this.game.PlayerAirline!;

        Console.WriteLine("═══════════════════ AIRCRAFT ACQUISITION ═══════════════════");
        Console.WriteLine();
        Console.WriteLine($"Current Cash: {FormatCurrency(airline.Cash)}");
        Console.WriteLine();
        Console.WriteLine("Available Aircraft:");
        Console.WriteLine();

        var aircraftTypes = AircraftType.Catalog.All.ToList();
        for (int i = 0; i < aircraftTypes.Count; i++)
        {
            var type = aircraftTypes[i];
            decimal monthlyLease = type.PurchasePrice * 0.012m;
            Console.WriteLine($"[{i + 1}] {type.Name}");
            Console.WriteLine($"    Category: {type.Category}  |  Capacity: {type.Capacity} seats  |  Range: {type.Range:N0} NM");
            Console.WriteLine($"    Purchase: ${type.PurchasePrice:N0}  |  Lease: ${monthlyLease:N0}/month");
            Console.WriteLine($"    Operating Cost: ${type.OperatingCostPerHour:N0}/hour");
            Console.WriteLine();
        }

        Console.Write("Select aircraft (number) or 'cancel': ");
        string? input = Console.ReadLine();
        if (input?.ToLower() == "cancel")
        {
            return;
        }

        if (!int.TryParse(input, out int typeIndex) || typeIndex < 1 || typeIndex > aircraftTypes.Count)
        {
            Console.WriteLine("Invalid selection.");
            Console.ReadKey();
            return;
        }

        var selectedType = aircraftTypes[typeIndex - 1];

        Console.WriteLine();
        Console.WriteLine("[B] Buy  |  [L] Lease  |  [C] Cancel");
        Console.Write("Choice: ");
        string? choice = Console.ReadLine()?.ToUpper();

        try
        {
            Aircraft? newAircraft = null;
            if (choice == "B")
            {
                newAircraft = airline.PurchaseAircraft(selectedType);
                Console.WriteLine($"✓ Purchased {newAircraft.DisplayName} for ${selectedType.PurchasePrice:N0}");
            }
            else if (choice == "L")
            {
                newAircraft = airline.LeaseAircraft(selectedType);
                Console.WriteLine(
                    $"✓ Leased {newAircraft.DisplayName} for ${newAircraft.MonthlyLeasePayment:N0}/month"
                );
            }
            else
            {
                return;
            }

            Console.WriteLine($"New cash balance: {FormatCurrency(airline.Cash)}");
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"✗ {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    /// <summary>
    /// Shows financial report with revenue, costs, and profitability.
    /// Like RCT's financial report, this provides detailed performance analysis.
    /// </summary>
    private void ShowFinancialReport()
    {
        Console.Clear();
        var airline = this.game.PlayerAirline!;

        Console.WriteLine("═══════════════════ FINANCIAL REPORT ═══════════════════");
        Console.WriteLine();
        Console.WriteLine($"Company: {airline.Name}");
        Console.WriteLine($"Day: {airline.CurrentDay}");
        Console.WriteLine();
        Console.WriteLine("────────────────────────────────────────────────────────");
        Console.WriteLine("ALL-TIME PERFORMANCE");
        Console.WriteLine("────────────────────────────────────────────────────────");
        Console.WriteLine($"Total Revenue:           {FormatCurrency(airline.TotalRevenue),15}");
        Console.WriteLine($"Total Costs:             {FormatCurrency(airline.TotalCosts),15}");
        Console.WriteLine($"Net Profit:              {FormatCurrency(airline.NetProfit),15}");
        Console.WriteLine($"Total Passengers:        {airline.TotalPassengersCarried,15:N0}");
        Console.WriteLine();
        Console.WriteLine("────────────────────────────────────────────────────────");
        Console.WriteLine("ASSETS");
        Console.WriteLine("────────────────────────────────────────────────────────");
        Console.WriteLine($"Cash on Hand:            {FormatCurrency(airline.Cash),15}");
        Console.WriteLine($"Fleet Size:              {airline.Fleet.Count,15}");
        Console.WriteLine($"  - Owned Aircraft:      {airline.Fleet.Count(a => !a.IsLeased),15}");
        Console.WriteLine($"  - Leased Aircraft:     {airline.Fleet.Count(a => a.IsLeased),15}");
        Console.WriteLine();
        Console.WriteLine("────────────────────────────────────────────────────────");
        Console.WriteLine("NETWORK");
        Console.WriteLine("────────────────────────────────────────────────────────");
        Console.WriteLine($"Active Routes:           {airline.Routes.Count(r => r.IsActive),15}");
        Console.WriteLine($"Average Load Factor:     {(airline.Routes.Any() ? airline.Routes.Average(r => r.LoadFactor) : 0),14:P1}");
        Console.WriteLine($"Reputation:              {airline.Reputation,15}/100");
        Console.WriteLine();
        Console.WriteLine("Press any key to return...");
        Console.ReadKey();
    }

    /// <summary>
    /// Advances the game by one day and shows results.
    /// </summary>
    private void AdvanceDay()
    {
        Console.Clear();
        Console.WriteLine("Processing day...");
        Console.WriteLine();

        var summary = this.game.ProcessDay();

        Console.WriteLine("═══════════════════ DAILY REPORT ═══════════════════");
        Console.WriteLine();
        Console.WriteLine($"Day {summary.Day} Complete!");
        Console.WriteLine();
        Console.WriteLine($"Revenue:             {FormatCurrency(summary.Revenue),15}");
        Console.WriteLine($"Costs:               {FormatCurrency(summary.Costs),15}");
        Console.WriteLine($"─────────────────────────────────────────────────");
        Console.WriteLine($"Daily Profit:        {FormatCurrency(summary.Profit),15}");
        Console.WriteLine();
        Console.WriteLine($"Passengers Carried:  {summary.PassengersCarried,15:N0}");
        Console.WriteLine($"Cash Balance:        {FormatCurrency(summary.CashBalance),15}");
        Console.WriteLine($"Reputation:          {GetReputationStars(summary.Reputation)}  ({summary.Reputation}/100)");
        Console.WriteLine();

        if (summary.Profit > 0)
        {
            Console.WriteLine("📈 Profitable day! Keep it up!");
        }
        else if (summary.Profit < 0)
        {
            Console.WriteLine("📉 Warning: Losing money! Review your routes and pricing.");
        }

        // Display any new events that occurred
        if (summary.NewEvents.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("═══════════════════ EVENTS ═══════════════════");
            Console.WriteLine();
            foreach (var evt in summary.NewEvents)
            {
                // Event icon based on type
                string icon = evt.Type switch
                {
                    Domain.Events.EventType.Weather => "🌩️",
                    Domain.Events.EventType.Economic => "💹",
                    Domain.Events.EventType.Operational => "⚙️",
                    Domain.Events.EventType.Market => "📊",
                    Domain.Events.EventType.PositivePR => "✨",
                    Domain.Events.EventType.NegativePR => "⚠️",
                    _ => "📢"
                };

                // Severity indicator
                string severityColor = evt.Severity switch
                {
                    Domain.Events.EventSeverity.Critical => "!!",
                    Domain.Events.EventSeverity.Major => "!",
                    Domain.Events.EventSeverity.Moderate => "*",
                    _ => ""
                };

                Console.WriteLine($"{icon} {evt.Title} {severityColor}");
                Console.WriteLine($"   {evt.Description}");
                Console.WriteLine();

                // Show event effects
                string effects = evt.GetSummary();
                if (!string.IsNullOrEmpty(effects))
                {
                    Console.WriteLine($"   Effects: {effects}");
                    Console.WriteLine();
                }
            }
        }

        // Show active ongoing events
        var activeEvents = this.game.PlayerAirline!.ActiveEvents
            .Where(e => e.IsActive(summary.Day))
            .ToList();

        if (activeEvents.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("─────────────────────────────────────────────────");
            Console.WriteLine($"Active Events ({activeEvents.Count}):");
            Console.WriteLine();
            foreach (var evt in activeEvents)
            {
                int daysRemaining = evt.ExpiresOnDay - summary.Day;
                Console.WriteLine($"• {evt.Title} ({daysRemaining} days remaining)");
            }
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    /// <summary>
    /// Interactive route closing.
    /// </summary>
    private void CloseRouteInteractive()
    {
        Console.WriteLine();
        Console.Write("Enter route name to close (e.g., 'JFK → LAX'): ");
        string? routeName = Console.ReadLine();

        var route = this.game.PlayerAirline!.Routes.FirstOrDefault(r =>
            r.Name.Equals(routeName, StringComparison.OrdinalIgnoreCase)
        );

        if (route != null)
        {
            this.game.PlayerAirline.CloseRoute(route);
            Console.WriteLine($"✓ Route {route.Name} closed.");
        }
        else
        {
            Console.WriteLine("Route not found.");
        }

        Console.ReadKey();
    }

    /// <summary>
    /// Interactive aircraft assignment to routes.
    /// </summary>
    private void AssignAircraftInteractive()
    {
        Console.WriteLine();
        Console.Write("Enter route name (e.g., 'JFK → LAX'): ");
        string? routeName = Console.ReadLine();

        var route = this.game.PlayerAirline!.Routes.FirstOrDefault(r =>
            r.Name.Equals(routeName, StringComparison.OrdinalIgnoreCase)
        );

        if (route == null)
        {
            Console.WriteLine("Route not found.");
            Console.ReadKey();
            return;
        }

        var availableAircraft = this.game.PlayerAirline.Fleet.Where(a => a.IsAvailable).ToList();

        if (!availableAircraft.Any())
        {
            Console.WriteLine("No available aircraft. All aircraft are assigned.");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("Available aircraft:");
        for (int i = 0; i < availableAircraft.Count; i++)
        {
            Console.WriteLine($"[{i + 1}] {availableAircraft[i].DisplayName}");
        }

        Console.Write("Select aircraft number: ");
        if (int.TryParse(Console.ReadLine(), out int index) && index >= 1 && index <= availableAircraft.Count)
        {
            var aircraft = availableAircraft[index - 1];
            route.AssignAircraft(aircraft);
            Console.WriteLine($"✓ {aircraft.DisplayName} assigned to {route.Name}");
        }
        else
        {
            Console.WriteLine("Invalid selection.");
        }

        Console.ReadKey();
    }

    /// <summary>
    /// Saves the current game to a file.
    /// </summary>
    private void SaveGame()
    {
        Console.Clear();
        Console.WriteLine("═══════════════════ SAVE GAME ═══════════════════");
        Console.WriteLine();

        if (this.game.PlayerAirline == null)
        {
            Console.WriteLine("✗ No active game to save!");
            Console.WriteLine();
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            return;
        }

        // Show existing saves to help user choose unique name
        var existingSaves = this.saveService.ListSaves();
        if (existingSaves.Count > 0)
        {
            Console.WriteLine("Existing saves:");
            foreach (var save in existingSaves.Take(5))
            {
                Console.WriteLine($"  - {save.SaveName} ({save.AirlineName}, Day {save.CurrentDay})");
            }
            Console.WriteLine();
        }

        // Generate default save name
        string defaultName = $"{this.game.PlayerAirline.Name}_Day{this.game.PlayerAirline.CurrentDay}";

        Console.Write($"Enter save name (or press Enter for '{defaultName}'): ");
        string? saveName = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(saveName))
        {
            saveName = defaultName;
        }

        // Check if save exists
        if (this.saveService.SaveExists(saveName))
        {
            Console.WriteLine();
            Console.Write($"Save '{saveName}' already exists. Overwrite? (y/n): ");
            string? confirm = Console.ReadLine();
            if (confirm?.ToLower() != "y")
            {
                Console.WriteLine("Save cancelled.");
                Console.WriteLine();
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                return;
            }
        }

        try
        {
            Console.WriteLine();
            Console.WriteLine("Saving game...");

            string filePath = this.saveService.SaveGame(this.game, saveName);

            Console.WriteLine($"✓ Game saved successfully to: {Path.GetFileName(filePath)}");
            Console.WriteLine($"   Location: {this.saveService.GetType().Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Error saving game: {ex.Message}");
        }

        Console.WriteLine();
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }

    /// <summary>
    /// Shows the victory screen when player wins.
    /// </summary>
    private void ShowVictoryScreen()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                                                            ║");
        Console.WriteLine("║                  🎉 CONGRATULATIONS! 🎉                    ║");
        Console.WriteLine("║                                                            ║");
        Console.WriteLine("║              You've built a successful airline!           ║");
        Console.WriteLine("║                                                            ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        this.ShowFinancialReport();
    }

    /// <summary>
    /// Shows the game over screen when player loses.
    /// </summary>
    private void ShowGameOverScreen()
    {
        Console.Clear();
        Console.WriteLine("╔════════════════════════════════════════════════════════════╗");
        Console.WriteLine("║                                                            ║");
        Console.WriteLine("║                      GAME OVER                            ║");
        Console.WriteLine("║                                                            ║");
        Console.WriteLine("║           Your airline has gone bankrupt!                 ║");
        Console.WriteLine("║                                                            ║");
        Console.WriteLine("╚════════════════════════════════════════════════════════════╝");
        Console.WriteLine();
        Console.WriteLine("Better luck next time!");
        Console.WriteLine();
        Console.WriteLine("Press any key to exit...");
        Console.ReadKey();
    }

    /// <summary>
    /// Formats currency for display.
    /// </summary>
    private static string FormatCurrency(decimal amount)
    {
        if (amount >= 0)
        {
            return $"${amount:N0}";
        }
        else
        {
            return $"-${Math.Abs(amount):N0}";
        }
    }

    /// <summary>
    /// Gets star rating for reputation display.
    /// </summary>
    private static string GetReputationStars(int reputation)
    {
        int stars = reputation / 20; // 0-100 maps to 0-5 stars
        return new string('★', stars) + new string('☆', 5 - stars);
    }

    /// <summary>
    /// Calculates suggested ticket price based on distance.
    /// Uses industry-standard pricing: roughly $0.12-0.15 per mile.
    /// </summary>
    private static decimal CalculateSuggestedTicketPrice(int distanceNM)
    {
        // Base price: $0.13 per nautical mile
        decimal basePrice = distanceNM * 0.13m;

        // Round to nearest $10
        return Math.Round(basePrice / 10m) * 10m;
    }
}
