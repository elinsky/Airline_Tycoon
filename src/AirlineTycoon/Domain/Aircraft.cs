namespace AirlineTycoon.Domain;

/// <summary>
/// Represents a specific aircraft owned or leased by an airline.
/// Similar to how RollerCoaster Tycoon has individual rides with maintenance needs, each aircraft is a unique asset.
/// </summary>
/// <remarks>
/// Aircraft in Airline Tycoon work like rides in RCT:
/// - You purchase or lease them (like building rides)
/// - They have ongoing costs (maintenance, like ride upkeep)
/// - They must be assigned to routes to generate revenue
/// - Condition degrades over time requiring maintenance
/// - Different types have different capacities and efficiencies
///
/// Aircraft management is a key strategic element:
/// - Larger aircraft are more efficient but need high demand routes
/// - Smaller aircraft are flexible for thin routes
/// - Leasing vs purchasing affects cash flow
/// - Fleet commonality reduces operating costs
/// </remarks>
public class Aircraft
{
    /// <summary>
    /// Gets the unique identifier for this aircraft.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets the aircraft registration number (tail number), like "N12345".
    /// </summary>
    public required string RegistrationNumber { get; init; }

    /// <summary>
    /// Gets the type of aircraft (e.g., Boeing 737-800).
    /// Determines capacity, range, and operating costs.
    /// </summary>
    public required AircraftType Type { get; init; }

    /// <summary>
    /// Gets whether this aircraft is leased (true) or owned (false).
    /// Leasing requires monthly payments but less upfront capital, like RCT's loan system.
    /// </summary>
    public bool IsLeased { get; init; }

    /// <summary>
    /// Gets the monthly lease payment if leased, or 0 if owned.
    /// </summary>
    public decimal MonthlyLeasePayment { get; init; }

    /// <summary>
    /// Gets or sets the route this aircraft is assigned to.
    /// Null if aircraft is in the hangar (not assigned).
    /// </summary>
    public Route? AssignedRoute { get; set; }

    /// <summary>
    /// Gets or sets the current condition rating from 0.0 (needs major maintenance) to 1.0 (perfect condition).
    /// Like RCT ride reliability, condition affects operational efficiency and passenger satisfaction.
    /// </summary>
    public double Condition { get; set; } = 1.0;

    /// <summary>
    /// Gets the total flight hours accumulated on this aircraft.
    /// Used for maintenance scheduling, like RCT ride age.
    /// </summary>
    public int TotalFlightHours { get; private set; }

    /// <summary>
    /// Gets the game day when this aircraft was acquired.
    /// </summary>
    public int AcquiredOnDay { get; init; }

    /// <summary>
    /// Gets whether this aircraft is currently available for assignment.
    /// An aircraft is available if it's not assigned to a route and not in maintenance.
    /// </summary>
    public bool IsAvailable => this.AssignedRoute == null && this.Condition > 0.3;

    /// <summary>
    /// Gets a display name for this aircraft (e.g., "N12345 (Boeing 737-800)").
    /// </summary>
    public string DisplayName => $"{this.RegistrationNumber} ({this.Type.Name})";

    /// <summary>
    /// Records flight hours for maintenance tracking and condition degradation.
    /// Called each game day for assigned aircraft, similar to RCT ride aging.
    /// </summary>
    /// <param name="hours">Number of flight hours to add.</param>
    public void AddFlightHours(double hours)
    {
        this.TotalFlightHours += (int)Math.Ceiling(hours);

        // Degrade condition slightly based on flight hours
        // Aircraft loses approximately 1% condition per 100 flight hours
        // This creates maintenance pressure similar to RCT ride breakdowns
        double degradation = hours / 10000.0;
        this.Condition = Math.Max(0.0, this.Condition - degradation);
    }

    /// <summary>
    /// Performs maintenance to restore aircraft condition.
    /// Like repairing rides in RCT, maintenance costs money but keeps aircraft operational.
    /// </summary>
    /// <param name="maintenanceLevel">Level of maintenance: 0.0 (basic) to 1.0 (complete overhaul).</param>
    /// <returns>The cost of maintenance in USD.</returns>
    public decimal PerformMaintenance(double maintenanceLevel = 1.0)
    {
        if (maintenanceLevel < 0.0 || maintenanceLevel > 1.0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maintenanceLevel),
                "Maintenance level must be between 0.0 and 1.0"
            );
        }

        // Restore condition proportional to maintenance level
        double conditionGain = maintenanceLevel * (1.0 - this.Condition);
        this.Condition = Math.Min(1.0, this.Condition + conditionGain);

        // Cost scales with aircraft size and maintenance level
        // Base cost is ~2% of aircraft value, scaled by maintenance level
        decimal baseCost = this.Type.PurchasePrice * 0.02m;
        return baseCost * (decimal)maintenanceLevel;
    }

    /// <summary>
    /// Creates a new owned aircraft (purchased outright).
    /// </summary>
    /// <param name="type">The type of aircraft.</param>
    /// <param name="registrationNumber">The registration number (tail number).</param>
    /// <param name="acquiredOnDay">The game day when acquired.</param>
    /// <returns>A new owned aircraft.</returns>
    public static Aircraft CreateOwned(
        AircraftType type,
        string registrationNumber,
        int acquiredOnDay
    )
    {
        return new Aircraft
        {
            Type = type,
            RegistrationNumber = registrationNumber,
            IsLeased = false,
            MonthlyLeasePayment = 0m,
            AcquiredOnDay = acquiredOnDay
        };
    }

    /// <summary>
    /// Creates a new leased aircraft.
    /// </summary>
    /// <remarks>
    /// Lease payments are typically 1-2% of aircraft value per month.
    /// Like RCT loans, leasing provides immediate access without large capital outlay.
    /// </remarks>
    /// <param name="type">The type of aircraft.</param>
    /// <param name="registrationNumber">The registration number (tail number).</param>
    /// <param name="acquiredOnDay">The game day when leased.</param>
    /// <returns>A new leased aircraft with calculated monthly payment.</returns>
    public static Aircraft CreateLeased(
        AircraftType type,
        string registrationNumber,
        int acquiredOnDay
    )
    {
        // Monthly lease is ~1.2% of purchase price
        // Over 5 years, this totals ~72% of purchase price, giving player flexibility
        decimal monthlyPayment = type.PurchasePrice * 0.012m;

        return new Aircraft
        {
            Type = type,
            RegistrationNumber = registrationNumber,
            IsLeased = true,
            MonthlyLeasePayment = monthlyPayment,
            AcquiredOnDay = acquiredOnDay
        };
    }

    /// <summary>
    /// Generates a random registration number for US-registered aircraft.
    /// Format: N + 5 digits (e.g., "N12345").
    /// </summary>
    /// <returns>A random but realistic registration number.</returns>
    public static string GenerateRegistrationNumber()
    {
        var random = new Random();
        int number = random.Next(10000, 99999);
        return $"N{number}";
    }
}
