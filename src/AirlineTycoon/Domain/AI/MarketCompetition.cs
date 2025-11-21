namespace AirlineTycoon.Domain.AI;

/// <summary>
/// Handles market competition calculations when multiple airlines serve the same route.
/// Splits passenger demand based on pricing, reputation, and service quality.
/// </summary>
/// <remarks>
/// Competition mechanics work similar to competing theme parks in RCT:
/// - Better pricing attracts more customers
/// - Higher reputation/brand strength increases market share
/// - Service quality creates passenger loyalty
///
/// Market share is calculated using weighted factors:
/// - Price competitiveness (40%) - lower prices attract budget travelers
/// - Reputation (35%) - brand strength and reliability
/// - Service quality (25%) - derived from AI personality traits
/// </remarks>
public static class MarketCompetition
{
    /// <summary>
    /// Calculates market share for a specific airline on a route with competition.
    /// Returns a value from 0.0 to 1.0 representing percentage of market captured.
    /// </summary>
    /// <param name="airline">The airline to calculate share for.</param>
    /// <param name="route">The route being analyzed.</param>
    /// <param name="allAirlinesOnRoute">All airlines operating this route (including the airline).</param>
    /// <param name="competitors">All competitor airlines (for accessing AI personality traits).</param>
    /// <returns>Market share as decimal (0.0 to 1.0).</returns>
    public static double CalculateMarketShare(
        Airline airline,
        Route route,
        List<RouteCompetitor> allAirlinesOnRoute,
        List<CompetitorAirline> competitors
    )
    {
        if (allAirlinesOnRoute.Count == 1)
        {
            // No competition = 100% market share
            return 1.0;
        }

        // Calculate scores for each airline on the route
        var scores = new Dictionary<Airline, double>();

        foreach (var competitor in allAirlinesOnRoute)
        {
            double priceScore = CalculatePriceCompetitiveness(competitor.TicketPrice, allAirlinesOnRoute);
            double reputationScore = competitor.Airline.Reputation / 100.0;

            // Service quality from AI personality (or default for player)
            double serviceScore = GetServiceQuality(competitor.Airline, competitors);

            // Weighted average: Price (40%), Reputation (35%), Service (25%)
            double totalScore = (priceScore * 0.40) + (reputationScore * 0.35) + (serviceScore * 0.25);

            scores[competitor.Airline] = totalScore;
        }

        // Calculate market share as proportion of total scores
        double totalScores = scores.Values.Sum();
        double airlineScore = scores[airline];

        return airlineScore / totalScores;
    }

    /// <summary>
    /// Calculates price competitiveness score (0.0 to 1.0).
    /// Lower prices relative to competitors = higher score.
    /// </summary>
    private static double CalculatePriceCompetitiveness(decimal ticketPrice, List<RouteCompetitor> competitors)
    {
        if (competitors.Count == 1)
        {
            return 1.0;
        }

        var prices = competitors.Select(c => c.TicketPrice).ToList();
        decimal avgPrice = prices.Average();
        decimal minPrice = prices.Min();
        decimal maxPrice = prices.Max();

        if (maxPrice == minPrice)
        {
            return 1.0; // All prices same
        }

        // Score inversely proportional to price
        // Lowest price gets 1.0, highest gets ~0.3
        double pricePosition = (double)(maxPrice - ticketPrice) / (double)(maxPrice - minPrice);
        return 0.3 + (pricePosition * 0.7);
    }

    /// <summary>
    /// Gets service quality score for an airline.
    /// For AI airlines, uses personality trait. For player, assumes medium quality (0.6).
    /// </summary>
    private static double GetServiceQuality(Airline airline, List<CompetitorAirline> competitors)
    {
        var competitorAirline = competitors.FirstOrDefault(c => c.Airline.Id == airline.Id);

        if (competitorAirline != null)
        {
            return competitorAirline.Personality.ServiceQuality;
        }

        // Player airline - assume medium service quality
        return 0.6;
    }

    /// <summary>
    /// Finds all airlines operating a specific route (same origin and destination).
    /// </summary>
    /// <param name="origin">Route origin airport code.</param>
    /// <param name="destination">Route destination airport code.</param>
    /// <param name="playerAirline">The player's airline.</param>
    /// <param name="competitors">List of AI competitors.</param>
    /// <returns>List of airlines and their routes on this city pair.</returns>
    public static List<RouteCompetitor> FindCompetingAirlines(
        string origin,
        string destination,
        Airline playerAirline,
        List<CompetitorAirline> competitors
    )
    {
        var result = new List<RouteCompetitor>();

        // Check player's routes
        var playerRoute = playerAirline.Routes.FirstOrDefault(r =>
            r.IsActive &&
            ((r.Origin.Code == origin && r.Destination.Code == destination) ||
             (r.Origin.Code == destination && r.Destination.Code == origin))
        );

        if (playerRoute != null)
        {
            result.Add(new RouteCompetitor
            {
                Airline = playerAirline,
                Route = playerRoute,
                TicketPrice = playerRoute.TicketPrice
            });
        }

        // Check competitor routes
        foreach (var competitor in competitors.Where(c => c.IsActive))
        {
            var competitorRoute = competitor.Airline.Routes.FirstOrDefault(r =>
                r.IsActive &&
                ((r.Origin.Code == origin && r.Destination.Code == destination) ||
                 (r.Origin.Code == destination && r.Destination.Code == origin))
            );

            if (competitorRoute != null)
            {
                result.Add(new RouteCompetitor
                {
                    Airline = competitor.Airline,
                    Route = competitorRoute,
                    TicketPrice = competitorRoute.TicketPrice
                });
            }
        }

        return result;
    }
}

/// <summary>
/// Represents an airline and its route in a competitive market.
/// Used for market share calculations.
/// </summary>
public class RouteCompetitor
{
    /// <summary>
    /// Gets or sets the airline operating the route.
    /// </summary>
    public required Airline Airline { get; init; }

    /// <summary>
    /// Gets or sets the specific route instance.
    /// </summary>
    public required Route Route { get; init; }

    /// <summary>
    /// Gets or sets the ticket price for this airline on this route.
    /// </summary>
    public required decimal TicketPrice { get; init; }
}
