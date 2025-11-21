namespace AirlineTycoon.Domain.AI;

/// <summary>
/// AI decision engine for competitor airlines.
/// Makes strategic decisions about routes, pricing, and fleet management.
/// </summary>
/// <remarks>
/// The AI uses a combination of:
/// - Profitability analysis (evaluate route potential)
/// - Competitive positioning (respond to player actions)
/// - Personality traits (modify decision thresholds)
/// - Risk assessment (balance growth vs stability)
///
/// AI decisions happen each turn during ProcessTurn(), creating
/// dynamic competition that evolves throughout the game.
/// </remarks>
public class AirlineAI
{
    private readonly Random random;

    /// <summary>
    /// Initializes a new instance of the <see cref="AirlineAI"/> class.
    /// </summary>
    public AirlineAI()
    {
        this.random = new Random();
    }

    /// <summary>
    /// Initializes a new instance with a specific random seed for deterministic behavior.
    /// </summary>
    /// <param name="seed">Random seed for reproducible AI decisions.</param>
    public AirlineAI(int seed)
    {
        this.random = new Random(seed);
    }

    /// <summary>
    /// Processes one turn of AI decision-making for a competitor airline.
    /// AI may open routes, adjust pricing, buy aircraft, or close unprofitable routes.
    /// </summary>
    /// <param name="competitor">The AI competitor to make decisions for.</param>
    /// <param name="allAirlines">All airlines in the game (for competitive analysis).</param>
    /// <param name="currentDay">Current game day.</param>
    public void ProcessTurn(CompetitorAirline competitor, List<Airline> allAirlines, int currentDay)
    {
        if (!competitor.IsActive)
        {
            return;
        }

        var airline = competitor.Airline;
        var personality = competitor.Personality;

        // 1. Close unprofitable routes (if conservative or hurting financially)
        this.ConsiderClosingRoutes(airline, personality);

        // 2. Consider opening new routes (based on expansion rate and cash)
        this.ConsiderOpeningRoutes(airline, personality, allAirlines);

        // 3. Adjust pricing on existing routes (based on load factors and competition)
        this.AdjustRoutePricing(airline, personality, allAirlines);

        // 4. Consider fleet expansion (if routes need aircraft)
        this.ConsiderFleetExpansion(airline, personality);
    }

    /// <summary>
    /// Evaluates and closes unprofitable routes based on AI personality.
    /// </summary>
    private void ConsiderClosingRoutes(Airline airline, AIPersonality personality)
    {
        // Conservative AIs close unprofitable routes quickly
        // Aggressive AIs tolerate losses longer hoping for turnaround
        int daysTolerance = (int)(30 * personality.RiskTolerance);

        var routesToClose = airline.Routes
            .Where(r => r.IsActive && r.DailyProfit < 0 && r.DaysOperating > daysTolerance)
            .ToList();

        foreach (var route in routesToClose)
        {
            airline.CloseRoute(route);
        }
    }

    /// <summary>
    /// Evaluates potential new routes and opens them based on profitability and personality.
    /// </summary>
    private void ConsiderOpeningRoutes(
        Airline airline,
        AIPersonality personality,
        List<Airline> allAirlines
    )
    {
        // Check if AI wants to expand this turn (based on expansion rate)
        if (this.random.NextDouble() > personality.ExpansionRate * 0.2) // 20% base chance * expansion rate
        {
            return;
        }

        // Need cash to expand
        decimal minCashReserve = 500_000m / (decimal)personality.RiskTolerance;
        if (airline.Cash < minCashReserve)
        {
            return;
        }

        // Find best unserved route from home hub
        var homeAirport = Airport.Catalog.FindByCode(airline.HomeHub);
        if (homeAirport == null)
        {
            return;
        }

        var potentialDestinations = Airport.Catalog.All
            .Where(a => a.Code != airline.HomeHub)
            .Where(a => !airline.Routes.Any(r =>
                (r.Origin.Code == airline.HomeHub && r.Destination.Code == a.Code) ||
                (r.Destination.Code == airline.HomeHub && r.Origin.Code == a.Code)
            ))
            .ToList();

        if (!potentialDestinations.Any())
        {
            return;
        }

        // Score each potential route
        var scoredRoutes = potentialDestinations
            .Select(dest => new
            {
                Destination = dest,
                Score = this.ScoreRoute(homeAirport, dest, personality, allAirlines)
            })
            .OrderByDescending(r => r.Score)
            .ToList();

        // Open the best route if score is good enough
        var bestRoute = scoredRoutes.FirstOrDefault();
        if (bestRoute != null && bestRoute.Score > 0.5) // Threshold for opening
        {
            int distance = Route.CalculateDistance(homeAirport.Code, bestRoute.Destination.Code);
            decimal basePrice = distance * 0.13m;
            decimal aiPrice = basePrice * (decimal)personality.PricingModifier;

            var newRoute = airline.OpenRoute(homeAirport, bestRoute.Destination, aiPrice);

            // Try to assign an available aircraft
            var availableAircraft = airline.Fleet.FirstOrDefault(a => a.IsAvailable);
            if (availableAircraft != null)
            {
                newRoute.AssignAircraft(availableAircraft);
            }
        }
    }

    /// <summary>
    /// Scores a potential route based on market size, distance, competition, and AI personality.
    /// Returns 0.0 to 1.0, with higher scores indicating better opportunities.
    /// </summary>
    private double ScoreRoute(
        Airport origin,
        Airport destination,
        AIPersonality personality,
        List<Airline> allAirlines
    )
    {
        // Base score from market sizes
        double marketScore = ((int)origin.MarketSize + (int)destination.MarketSize) / 8.0; // Max VeryLarge + VeryLarge = 8

        // Distance considerations (prefer medium-range for most personalities)
        int distance = Route.CalculateDistance(origin.Code, destination.Code);
        double distanceScore = distance switch
        {
            < 500 => 0.6,    // Short routes: decent
            < 1500 => 1.0,   // Medium routes: best
            < 2500 => 0.8,   // Long routes: good
            _ => 0.5         // Very long: risky
        };

        // Competition penalty (how many airlines already serve this route?)
        int competitorCount = allAirlines
            .SelectMany(a => a.Routes)
            .Count(r =>
                (r.Origin.Code == origin.Code && r.Destination.Code == destination.Code) ||
                (r.Destination.Code == origin.Code && r.Origin.Code == destination.Code)
            );

        double competitionScore = competitorCount switch
        {
            0 => 1.0,  // Unserved market: excellent
            1 => 0.7,  // One competitor: okay if aggressive
            2 => 0.4,  // Two competitors: risky
            _ => 0.2   // Saturated: avoid
        };

        // Adjust competition score by competitive aggression
        competitionScore = competitionScore * (1.0 - personality.CompetitiveAggression * 0.5);

        // Weighted average
        double finalScore = (marketScore * 0.4) + (distanceScore * 0.3) + (competitionScore * 0.3);

        return Math.Clamp(finalScore, 0.0, 1.0);
    }

    /// <summary>
    /// Adjusts pricing on existing routes based on load factors and competition.
    /// </summary>
    private void AdjustRoutePricing(Airline airline, AIPersonality personality, List<Airline> allAirlines)
    {
        foreach (var route in airline.Routes.Where(r => r.IsActive))
        {
            // High load factor (>85%) = raise prices
            // Low load factor (<60%) = lower prices
            // Budget carriers are more aggressive with price cuts
            if (route.LoadFactor > 0.85)
            {
                // Raise price by 5-10%
                decimal increase = route.TicketPrice * 0.05m * (decimal)personality.ServiceQuality;
                route.TicketPrice += increase;
            }
            else if (route.LoadFactor < 0.60)
            {
                // Lower price by 5-15%
                decimal decrease = route.TicketPrice * 0.10m * (decimal)(1.0 - personality.PricingModifier);
                route.TicketPrice = Math.Max(50m, route.TicketPrice - decrease); // Floor at $50
            }
        }
    }

    /// <summary>
    /// Considers expanding the fleet if routes need aircraft.
    /// </summary>
    private void ConsiderFleetExpansion(Airline airline, AIPersonality personality)
    {
        // Count routes without aircraft
        int unassignedRoutes = airline.Routes.Count(r => r.IsActive && r.AssignedAircraft == null);

        if (unassignedRoutes == 0)
        {
            return;
        }

        // Check if we can afford aircraft
        var aircraftType = AircraftType.Catalog.Boeing737; // Default to workhorse aircraft
        decimal purchaseThreshold = aircraftType.PurchasePrice * (decimal)personality.RiskTolerance;

        if (airline.Cash >= purchaseThreshold)
        {
            // Buy aircraft if conservative or well-funded
            if (personality.Type == AIPersonalityType.Conservative || airline.Cash > aircraftType.PurchasePrice * 2)
            {
                try
                {
                    var newAircraft = airline.PurchaseAircraft(aircraftType);

                    // Assign to first unassigned route
                    var routeToAssign = airline.Routes.FirstOrDefault(r => r.IsActive && r.AssignedAircraft == null);
                    routeToAssign?.AssignAircraft(newAircraft);
                }
                catch (InvalidOperationException)
                {
                    // Not enough cash, try leasing instead
                    this.TryLeaseAircraft(airline);
                }
            }
            else
            {
                // Lease aircraft if aggressive or budget
                this.TryLeaseAircraft(airline);
            }
        }
        else
        {
            // Can't afford to buy, try leasing
            this.TryLeaseAircraft(airline);
        }
    }

    /// <summary>
    /// Attempts to lease an aircraft and assign it to an unassigned route.
    /// </summary>
    private void TryLeaseAircraft(Airline airline)
    {
        var aircraftType = AircraftType.Catalog.Boeing737;
        var newAircraft = airline.LeaseAircraft(aircraftType);

        // Assign to first unassigned route
        var routeToAssign = airline.Routes.FirstOrDefault(r => r.IsActive && r.AssignedAircraft == null);
        routeToAssign?.AssignAircraft(newAircraft);
    }
}
