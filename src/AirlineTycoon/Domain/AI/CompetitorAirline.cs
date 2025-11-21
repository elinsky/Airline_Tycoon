using AirlineTycoon.Domain.Events;

namespace AirlineTycoon.Domain.AI;

/// <summary>
/// Represents an AI-controlled competitor airline.
/// Extends the base Airline class with AI decision-making capabilities.
/// </summary>
/// <remarks>
/// CompetitorAirlines operate autonomously each turn, making decisions about:
/// - Which routes to open/close
/// - How to price tickets
/// - When to buy/lease aircraft
/// - How to respond to player actions
///
/// Like competing parks in RCT, AI airlines create dynamic competition
/// and force the player to adapt their strategy.
/// </remarks>
public class CompetitorAirline
{
    /// <summary>
    /// Gets the underlying airline entity with all operational data.
    /// </summary>
    public required Airline Airline { get; init; }

    /// <summary>
    /// Gets the AI personality defining decision-making behavior.
    /// </summary>
    public required AIPersonality Personality { get; init; }

    /// <summary>
    /// Gets whether this competitor is currently active (not bankrupt).
    /// </summary>
    public bool IsActive => !this.Airline.IsBankrupt;

    /// <summary>
    /// Gets the competitor's market position relative to player and other AIs.
    /// Based on total passengers carried, route count, and reputation.
    /// </summary>
    public MarketPosition GetMarketPosition(List<CompetitorAirline> allCompetitors)
    {
        var totalPassengers = this.Airline.TotalPassengersCarried;
        var avgPassengers = allCompetitors.Average(c => c.Airline.TotalPassengersCarried);

        if (totalPassengers >= avgPassengers * 1.5)
        {
            return MarketPosition.Dominant;
        }
        else if (totalPassengers >= avgPassengers)
        {
            return MarketPosition.Strong;
        }
        else if (totalPassengers >= avgPassengers * 0.5)
        {
            return MarketPosition.Moderate;
        }
        else
        {
            return MarketPosition.Weak;
        }
    }

    /// <summary>
    /// Creates a new competitor airline with specified personality and starting conditions.
    /// </summary>
    /// <param name="name">Airline name.</param>
    /// <param name="homeHub">Home hub airport code.</param>
    /// <param name="personality">AI personality type.</param>
    /// <param name="startingCash">Initial cash balance.</param>
    /// <returns>New competitor airline ready to operate.</returns>
    public static CompetitorAirline Create(
        string name,
        string homeHub,
        AIPersonality personality,
        decimal startingCash = 3_000_000m
    )
    {
        var airline = new Airline
        {
            Name = name,
            HomeHub = homeHub,
            Cash = startingCash,
            Reputation = 50 + (int)(personality.ServiceQuality * 20) // Higher quality = better starting reputation
        };

        return new CompetitorAirline
        {
            Airline = airline,
            Personality = personality
        };
    }

    /// <summary>
    /// Creates a set of default competitors for a standard game.
    /// Returns 3 AI airlines with different personalities and home hubs.
    /// </summary>
    public static List<CompetitorAirline> CreateDefaultCompetitors()
    {
        return
        [
            Create("SkyHigh Airlines", "ATL", AIPersonality.CreateAggressive()),
            Create("Premium Airways", "LAX", AIPersonality.CreateConservative()),
            Create("ValueJet", "ORD", AIPersonality.CreateBudget())
        ];
    }
}

/// <summary>
/// Represents the relative market strength of an airline.
/// </summary>
public enum MarketPosition
{
    /// <summary>
    /// Market leader with 50%+ above average passengers.
    /// </summary>
    Dominant,

    /// <summary>
    /// Above-average performer.
    /// </summary>
    Strong,

    /// <summary>
    /// Average to below-average performer.
    /// </summary>
    Moderate,

    /// <summary>
    /// Struggling airline with &lt;50% of average passengers.
    /// </summary>
    Weak
}
