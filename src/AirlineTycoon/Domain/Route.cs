namespace AirlineTycoon.Domain;

/// <summary>
/// Represents a flight route between two airports operated by an airline.
/// Similar to how RollerCoaster Tycoon has rides that generate revenue, routes are the core revenue generators.
/// </summary>
/// <remarks>
/// Routes in Airline Tycoon work like rides in RCT:
/// - You "build" them by opening service between two airports
/// - They have operating costs (like ride maintenance)
/// - They generate revenue from passengers (like ride tickets)
/// - Performance metrics show profitability (like ride popularity ratings)
/// - You can open/close them strategically
///
/// Route profitability depends on:
/// - Distance (affects fuel costs and ticket prices)
/// - Market demand (based on airport sizes)
/// - Competition (other airlines on same route)
/// - Assigned aircraft efficiency
/// - Ticket pricing strategy
/// - Load factor (percentage of seats filled)
/// </remarks>
public class Route
{
    /// <summary>
    /// Gets the unique identifier for this route.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets the origin airport.
    /// </summary>
    public required Airport Origin { get; init; }

    /// <summary>
    /// Gets the destination airport.
    /// </summary>
    public required Airport Destination { get; init; }

    /// <summary>
    /// Gets the distance between airports in nautical miles.
    /// Like RCT ride length, longer distances mean higher costs but potentially higher revenue.
    /// </summary>
    public required int DistanceNauticalMiles { get; init; }

    /// <summary>
    /// Gets or sets the aircraft assigned to this route.
    /// Like assigning staff to a ride in RCT, aircraft assignment is critical for operations.
    /// </summary>
    public Aircraft? AssignedAircraft { get; set; }

    /// <summary>
    /// Gets or sets the base ticket price in USD.
    /// Like setting ride prices in RCT, pricing affects demand and revenue.
    /// </summary>
    public decimal TicketPrice { get; set; }

    /// <summary>
    /// Gets or sets the number of daily flights on this route.
    /// More flights mean more capacity and potential revenue, but higher costs.
    /// </summary>
    public int DailyFlights { get; set; } = 1;

    /// <summary>
    /// Gets or sets whether this route is currently active.
    /// Like opening/closing rides in RCT for maintenance or poor performance.
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Gets the current load factor (percentage of seats filled) as a decimal (0.0 - 1.0).
    /// Like RCT's ride popularity rating, this shows how well the route is performing.
    /// </summary>
    public double LoadFactor { get; private set; } = 0.75; // Start at 75% - typical industry average

    /// <summary>
    /// Gets the daily profit/loss for this route in USD.
    /// Calculated as: (Revenue from passengers) - (Fuel + Crew + Landing fees + Maintenance).
    /// </summary>
    public decimal DailyProfit { get; private set; }

    /// <summary>
    /// Gets the total number of passengers carried since route opened.
    /// </summary>
    public int TotalPassengers { get; private set; }

    /// <summary>
    /// Gets the day this route was opened.
    /// </summary>
    public int OpenedOnDay { get; init; }

    /// <summary>
    /// Gets a descriptive name for this route (e.g., "JFK → LAX").
    /// </summary>
    public string Name => $"{this.Origin.Code} → {this.Destination.Code}";

    /// <summary>
    /// Gets the estimated flight time in hours.
    /// Assumes average commercial aircraft speed of 450 knots (nautical miles per hour).
    /// </summary>
    public double FlightTimeHours => this.DistanceNauticalMiles / 450.0;

    /// <summary>
    /// Updates the route's performance metrics based on daily simulation.
    /// This is called each game day, similar to how RCT updates ride statistics.
    /// </summary>
    /// <param name="loadFactor">The load factor achieved (0.0 - 1.0).</param>
    /// <param name="profit">The profit/loss for the day.</param>
    /// <param name="passengers">Number of passengers carried.</param>
    public void UpdatePerformance(double loadFactor, decimal profit, int passengers)
    {
        this.LoadFactor = loadFactor;
        this.DailyProfit = profit;
        this.TotalPassengers += passengers;
    }

    /// <summary>
    /// Assigns an aircraft to this route.
    /// The aircraft must not be assigned to another route.
    /// </summary>
    /// <param name="aircraft">The aircraft to assign.</param>
    /// <exception cref="InvalidOperationException">Thrown if aircraft is already assigned elsewhere.</exception>
    public void AssignAircraft(Aircraft aircraft)
    {
        if (aircraft.AssignedRoute != null && aircraft.AssignedRoute.Id != this.Id)
        {
            throw new InvalidOperationException(
                $"Aircraft {aircraft.RegistrationNumber} is already assigned to route {aircraft.AssignedRoute.Name}"
            );
        }

        this.AssignedAircraft = aircraft;
        aircraft.AssignedRoute = this;
    }

    /// <summary>
    /// Removes the assigned aircraft from this route.
    /// </summary>
    public void UnassignAircraft()
    {
        if (this.AssignedAircraft != null)
        {
            this.AssignedAircraft.AssignedRoute = null;
            this.AssignedAircraft = null;
        }
    }

    /// <summary>
    /// Calculates the great circle distance between two airports in nautical miles.
    /// Uses the Haversine formula for accurate distance calculation.
    /// </summary>
    /// <remarks>
    /// This is a simplified calculation for the MVP. In a full version, we'd use actual
    /// airport coordinates from a database. For now, we estimate based on typical US distances.
    /// </remarks>
    /// <param name="origin">Origin airport code.</param>
    /// <param name="destination">Destination airport code.</param>
    /// <returns>Distance in nautical miles.</returns>
    public static int CalculateDistance(string origin, string destination)
    {
        // Simplified distance matrix for MVP - typical flight distances
        // In production, this would use real GPS coordinates and Haversine formula
        var distances = new Dictionary<(string, string), int>
        {
            // From JFK
            { ("JFK", "LAX"), 2144 },
            { ("JFK", "MIA"), 926 },
            { ("JFK", "BOS"), 162 },
            { ("JFK", "ORD"), 636 },
            { ("JFK", "ATL"), 762 },
            { ("JFK", "DFW"), 1215 },
            { ("JFK", "SEA"), 2138 },
            { ("JFK", "LAS"), 2037 },
            { ("JFK", "SFO"), 2242 },
            { ("JFK", "DEN"), 1479 },

            // From LAX
            { ("LAX", "SFO"), 292 },
            { ("LAX", "LAS"), 190 },
            { ("LAX", "SEA"), 834 },
            { ("LAX", "PHX"), 325 },
            { ("LAX", "DEN"), 758 },
            { ("LAX", "ORD"), 1514 },
            { ("LAX", "ATL"), 1747 },
            { ("LAX", "MIA"), 2161 },

            // From ORD
            { ("ORD", "ATL"), 491 },
            { ("ORD", "DFW"), 719 },
            { ("ORD", "DEN"), 820 },
            { ("ORD", "SEA"), 1485 },
            { ("ORD", "MIA"), 1046 },
            { ("ORD", "BOS"), 748 },
            { ("ORD", "PHX"), 1261 },

            // Additional key routes
            { ("ATL", "MIA"), 547 },
            { ("ATL", "DFW"), 645 },
            { ("DFW", "PHX"), 728 },
            { ("DFW", "DEN"), 547 },
            { ("SEA", "SFO"), 641 },
            { ("BOS", "MIA"), 1094 },
            { ("DEN", "PHX"), 485 },
            { ("MSP", "ORD"), 290 },
            { ("DTW", "ORD"), 191 },
            { ("PHL", "ATL"), 612 }
        };

        // Try forward direction
        if (distances.TryGetValue((origin, destination), out int distance))
        {
            return distance;
        }

        // Try reverse direction (same distance)
        if (distances.TryGetValue((destination, origin), out distance))
        {
            return distance;
        }

        // Default fallback for unmapped routes - estimate based on typical transcontinental distance
        return 1000; // Roughly mid-range flight
    }
}
