namespace AirlineTycoon.Domain;

/// <summary>
/// Represents an airport where airlines can operate flights.
/// Similar to how RollerCoaster Tycoon has park entrances and exits, airports are connection points in the network.
/// </summary>
/// <remarks>
/// Airports have different characteristics that affect gameplay:
/// - Market size determines potential passenger demand
/// - Landing fees affect route profitability
/// - Regional hubs connect to many destinations
/// - International hubs have higher volume but more competition
///
/// The airport system is inspired by RCT's path network - players connect airports like building paths between attractions.
/// </remarks>
public record Airport
{
    /// <summary>
    /// Gets the unique three-letter IATA airport code (e.g., "JFK", "LAX", "ORD").
    /// </summary>
    public required string Code { get; init; }

    /// <summary>
    /// Gets the full name of the airport (e.g., "John F. Kennedy International Airport").
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// Gets the city where the airport is located (e.g., "New York", "Los Angeles").
    /// </summary>
    public required string City { get; init; }

    /// <summary>
    /// Gets the market size category that determines passenger demand.
    /// Similar to RCT's park entrance footfall, larger markets generate more potential passengers.
    /// </summary>
    public required MarketSize MarketSize { get; init; }

    /// <summary>
    /// Gets the landing fee charged per flight operation in USD.
    /// Like RCT's land purchase costs, these fees affect profitability of routes.
    /// </summary>
    public required decimal LandingFee { get; init; }

    /// <summary>
    /// Gets whether this airport serves as a hub (major connection point).
    /// Hub airports are like RCT's main park areas - central to your network strategy.
    /// </summary>
    public required bool IsHub { get; init; }

    /// <summary>
    /// Provides a catalog of real-world airports available in the game.
    /// Similar to RCT's pre-built scenarios, these airports form the game map.
    /// </summary>
    public static class Catalog
    {
        /// <summary>
        /// Gets all available airports in the game.
        /// Start with major US airports, expandable to international in future versions.
        /// </summary>
        public static readonly IReadOnlyList<Airport> All =
        [
            // Major Hub Airports - Like RCT's premium park locations
            new Airport
            {
                Code = "JFK",
                Name = "John F. Kennedy International Airport",
                City = "New York",
                MarketSize = MarketSize.VeryLarge,
                LandingFee = 2500m,
                IsHub = true
            },
            new Airport
            {
                Code = "LAX",
                Name = "Los Angeles International Airport",
                City = "Los Angeles",
                MarketSize = MarketSize.VeryLarge,
                LandingFee = 2200m,
                IsHub = true
            },
            new Airport
            {
                Code = "ORD",
                Name = "Chicago O'Hare International Airport",
                City = "Chicago",
                MarketSize = MarketSize.VeryLarge,
                LandingFee = 2000m,
                IsHub = true
            },
            new Airport
            {
                Code = "ATL",
                Name = "Hartsfield-Jackson Atlanta International Airport",
                City = "Atlanta",
                MarketSize = MarketSize.VeryLarge,
                LandingFee = 1800m,
                IsHub = true
            },
            new Airport
            {
                Code = "DFW",
                Name = "Dallas/Fort Worth International Airport",
                City = "Dallas",
                MarketSize = MarketSize.Large,
                LandingFee = 1900m,
                IsHub = true
            },

            // Large Markets - Secondary hubs
            new Airport
            {
                Code = "MIA",
                Name = "Miami International Airport",
                City = "Miami",
                MarketSize = MarketSize.Large,
                LandingFee = 1700m,
                IsHub = false
            },
            new Airport
            {
                Code = "SEA",
                Name = "Seattle-Tacoma International Airport",
                City = "Seattle",
                MarketSize = MarketSize.Large,
                LandingFee = 1600m,
                IsHub = false
            },
            new Airport
            {
                Code = "LAS",
                Name = "Harry Reid International Airport",
                City = "Las Vegas",
                MarketSize = MarketSize.Large,
                LandingFee = 1500m,
                IsHub = false
            },
            new Airport
            {
                Code = "BOS",
                Name = "Boston Logan International Airport",
                City = "Boston",
                MarketSize = MarketSize.Large,
                LandingFee = 1800m,
                IsHub = false
            },
            new Airport
            {
                Code = "SFO",
                Name = "San Francisco International Airport",
                City = "San Francisco",
                MarketSize = MarketSize.Large,
                LandingFee = 2100m,
                IsHub = false
            },

            // Medium Markets - Regional connections
            new Airport
            {
                Code = "DEN",
                Name = "Denver International Airport",
                City = "Denver",
                MarketSize = MarketSize.Medium,
                LandingFee = 1400m,
                IsHub = false
            },
            new Airport
            {
                Code = "PHX",
                Name = "Phoenix Sky Harbor International Airport",
                City = "Phoenix",
                MarketSize = MarketSize.Medium,
                LandingFee = 1300m,
                IsHub = false
            },
            new Airport
            {
                Code = "MSP",
                Name = "Minneapolis-St Paul International Airport",
                City = "Minneapolis",
                MarketSize = MarketSize.Medium,
                LandingFee = 1200m,
                IsHub = false
            },
            new Airport
            {
                Code = "DTW",
                Name = "Detroit Metropolitan Airport",
                City = "Detroit",
                MarketSize = MarketSize.Medium,
                LandingFee = 1100m,
                IsHub = false
            },
            new Airport
            {
                Code = "PHL",
                Name = "Philadelphia International Airport",
                City = "Philadelphia",
                MarketSize = MarketSize.Medium,
                LandingFee = 1300m,
                IsHub = false
            }
        ];

        /// <summary>
        /// Finds an airport by its IATA code.
        /// </summary>
        /// <param name="code">The three-letter IATA code.</param>
        /// <returns>The airport if found, null otherwise.</returns>
        public static Airport? FindByCode(string code) =>
            All.FirstOrDefault(a => a.Code.Equals(code, StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// Defines the size of an airport's market, affecting passenger demand.
/// Inspired by RCT's park visitor capacity levels - larger markets mean more potential customers.
/// </summary>
public enum MarketSize
{
    /// <summary>
    /// Small regional market (e.g., smaller cities) - 100-500k annual passengers potential.
    /// </summary>
    Small,

    /// <summary>
    /// Medium market (e.g., mid-size cities) - 500k-2M annual passengers potential.
    /// </summary>
    Medium,

    /// <summary>
    /// Large market (e.g., major cities) - 2M-10M annual passengers potential.
    /// </summary>
    Large,

    /// <summary>
    /// Very large market (e.g., global hubs) - 10M+ annual passengers potential.
    /// </summary>
    VeryLarge
}
