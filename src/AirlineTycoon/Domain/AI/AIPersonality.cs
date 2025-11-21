namespace AirlineTycoon.Domain.AI;

/// <summary>
/// Defines the behavioral traits of an AI competitor airline.
/// Like different park managers in RCT, each AI has a distinct playstyle.
/// </summary>
/// <remarks>
/// AI personalities affect decision-making in multiple areas:
/// - Route selection (aggressive expansion vs conservative growth)
/// - Pricing strategy (premium vs budget positioning)
/// - Risk tolerance (debt levels, expansion speed)
/// - Competitive behavior (price wars, market entry timing)
///
/// This creates varied competition and makes each game playthrough unique.
/// </remarks>
public class AIPersonality
{
    /// <summary>
    /// Gets the personality type (Aggressive, Conservative, Budget).
    /// </summary>
    public required AIPersonalityType Type { get; init; }

    /// <summary>
    /// Gets the expansion aggressiveness (0.0 to 1.0).
    /// Higher values = more willing to open new routes quickly.
    /// </summary>
    public required double ExpansionRate { get; init; }

    /// <summary>
    /// Gets the pricing strategy modifier (0.5 to 1.5).
    /// &lt;1.0 = discount pricing, &gt;1.0 = premium pricing.
    /// </summary>
    public required double PricingModifier { get; init; }

    /// <summary>
    /// Gets the risk tolerance (0.0 to 1.0).
    /// Higher values = more willing to take debt, operate unprofitable routes longer.
    /// </summary>
    public required double RiskTolerance { get; init; }

    /// <summary>
    /// Gets how aggressively the AI responds to competition (0.0 to 1.0).
    /// Higher values = more likely to engage in price wars, match routes.
    /// </summary>
    public required double CompetitiveAggression { get; init; }

    /// <summary>
    /// Gets the service quality level (0.0 to 1.0).
    /// Affects reputation growth rate and passenger loyalty.
    /// </summary>
    public required double ServiceQuality { get; init; }

    /// <summary>
    /// Creates an Aggressive personality - rapid expansion, price wars, high risk.
    /// </summary>
    public static AIPersonality CreateAggressive()
    {
        return new AIPersonality
        {
            Type = AIPersonalityType.Aggressive,
            ExpansionRate = 0.9,
            PricingModifier = 0.85, // Undercut competitors
            RiskTolerance = 0.8,
            CompetitiveAggression = 0.95,
            ServiceQuality = 0.5 // Lower quality for speed
        };
    }

    /// <summary>
    /// Creates a Conservative personality - slow growth, premium pricing, low risk.
    /// </summary>
    public static AIPersonality CreateConservative()
    {
        return new AIPersonality
        {
            Type = AIPersonalityType.Conservative,
            ExpansionRate = 0.3,
            PricingModifier = 1.15, // Premium pricing
            RiskTolerance = 0.2,
            CompetitiveAggression = 0.3,
            ServiceQuality = 0.85 // High quality service
        };
    }

    /// <summary>
    /// Creates a Budget personality - high volume, low prices, efficiency focus.
    /// </summary>
    public static AIPersonality CreateBudget()
    {
        return new AIPersonality
        {
            Type = AIPersonalityType.Budget,
            ExpansionRate = 0.6,
            PricingModifier = 0.70, // Very low prices
            RiskTolerance = 0.5,
            CompetitiveAggression = 0.6,
            ServiceQuality = 0.3 // Minimal service
        };
    }

    /// <summary>
    /// Creates a Balanced personality - moderate in all aspects.
    /// </summary>
    public static AIPersonality CreateBalanced()
    {
        return new AIPersonality
        {
            Type = AIPersonalityType.Balanced,
            ExpansionRate = 0.5,
            PricingModifier = 1.0,
            RiskTolerance = 0.5,
            CompetitiveAggression = 0.5,
            ServiceQuality = 0.6
        };
    }
}

/// <summary>
/// Types of AI personalities defining strategic approach.
/// </summary>
public enum AIPersonalityType
{
    /// <summary>
    /// Aggressive expansion, price wars, high risk/reward.
    /// Like Southwest Airlines' early strategy.
    /// </summary>
    Aggressive,

    /// <summary>
    /// Slow growth, premium service, conservative finances.
    /// Like Singapore Airlines or Emirates.
    /// </summary>
    Conservative,

    /// <summary>
    /// Low-cost carrier, high volume, efficiency focus.
    /// Like Ryanair or Spirit Airlines.
    /// </summary>
    Budget,

    /// <summary>
    /// Balanced approach across all dimensions.
    /// Like Delta or American Airlines.
    /// </summary>
    Balanced
}
