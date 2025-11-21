namespace AirlineTycoon.Domain;

/// <summary>
/// Defines different types of aircraft available in the game.
/// </summary>
/// <remarks>
/// Each aircraft type has different characteristics:
/// - Capacity: Number of passengers
/// - Range: Maximum flight distance in miles
/// - Cost: Purchase price
/// - Operating cost per hour
///
/// Aircraft are categorized by size and purpose, similar to real-world aviation.
/// </remarks>
public enum AircraftCategory
{
    /// <summary>Regional jets for short routes (50-100 passengers)</summary>
    Regional,

    /// <summary>Narrow-body jets for domestic routes (100-200 passengers)</summary>
    NarrowBody,

    /// <summary>Wide-body jets for international routes (200-400 passengers)</summary>
    WideBody,

    /// <summary>Jumbo jets for high-demand long-haul routes (400+ passengers)</summary>
    Jumbo
}

/// <summary>
/// Represents a specific aircraft model with its performance and cost characteristics.
/// </summary>
/// <remarks>
/// Immutable record representing aircraft specifications.
/// Used for fleet planning and route optimization decisions.
/// </remarks>
public record AircraftType
{
    /// <summary>Gets the display name of the aircraft (e.g., "Boeing 737").</summary>
    public required string Name { get; init; }

    /// <summary>Gets the aircraft category.</summary>
    public required AircraftCategory Category { get; init; }

    /// <summary>Gets the passenger capacity.</summary>
    public required int Capacity { get; init; }

    /// <summary>Gets the maximum range in miles.</summary>
    public required int Range { get; init; }

    /// <summary>Gets the purchase price in dollars.</summary>
    public required decimal PurchasePrice { get; init; }

    /// <summary>Gets the operating cost per flight hour in dollars.</summary>
    public required decimal OperatingCostPerHour { get; init; }

    /// <summary>Gets the fuel consumption in gallons per hour.</summary>
    public required int FuelConsumptionPerHour { get; init; }

    /// <summary>
    /// Predefined aircraft types available in the game.
    /// </summary>
    public static class Catalog
    {
        /// <summary>Regional jet - good for starting routes</summary>
        public static readonly AircraftType EmbraerE175 =
            new()
            {
                Name = "Embraer E175",
                Category = AircraftCategory.Regional,
                Capacity = 76,
                Range = 2200,
                PurchasePrice = 30_000_000m,
                OperatingCostPerHour = 2_500m,
                FuelConsumptionPerHour = 450
            };

        /// <summary>Popular narrow-body for domestic routes</summary>
        public static readonly AircraftType Boeing737 =
            new()
            {
                Name = "Boeing 737-800",
                Category = AircraftCategory.NarrowBody,
                Capacity = 162,
                Range = 3000,
                PurchasePrice = 90_000_000m,
                OperatingCostPerHour = 4_500m,
                FuelConsumptionPerHour = 850
            };

        /// <summary>Alternative narrow-body option</summary>
        public static readonly AircraftType AirbusA320 =
            new()
            {
                Name = "Airbus A320",
                Category = AircraftCategory.NarrowBody,
                Capacity = 150,
                Range = 3300,
                PurchasePrice = 85_000_000m,
                OperatingCostPerHour = 4_200m,
                FuelConsumptionPerHour = 820
            };

        /// <summary>Wide-body for international routes</summary>
        public static readonly AircraftType Boeing787 =
            new()
            {
                Name = "Boeing 787-9",
                Category = AircraftCategory.WideBody,
                Capacity = 280,
                Range = 7635,
                PurchasePrice = 250_000_000m,
                OperatingCostPerHour = 8_500m,
                FuelConsumptionPerHour = 1650
            };

        /// <summary>Jumbo jet for high-capacity routes</summary>
        public static readonly AircraftType AirbusA380 =
            new()
            {
                Name = "Airbus A380",
                Category = AircraftCategory.Jumbo,
                Capacity = 525,
                Range = 8000,
                PurchasePrice = 445_000_000m,
                OperatingCostPerHour = 15_000m,
                FuelConsumptionPerHour = 3100
            };

        /// <summary>Gets all available aircraft types.</summary>
        public static IReadOnlyList<AircraftType> All =>
            new[]
            {
                EmbraerE175,
                Boeing737,
                AirbusA320,
                Boeing787,
                AirbusA380
            };
    }
}
