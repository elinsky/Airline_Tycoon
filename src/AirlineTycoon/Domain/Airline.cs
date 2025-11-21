using AirlineTycoon.Domain.AI;
using AirlineTycoon.Domain.Events;

namespace AirlineTycoon.Domain;

/// <summary>
/// Represents the player's airline company.
/// This is the central entity in Airline Tycoon, similar to the Park in RollerCoaster Tycoon.
/// </summary>
/// <remarks>
/// The Airline class manages all aspects of the player's aviation business:
/// - Financial state (cash, loans, profits)
/// - Fleet of aircraft (owned and leased)
/// - Route network
/// - Reputation and brand value
/// - Operational statistics
///
/// Like RCT's park management, success depends on balancing:
/// - Revenue generation (routes with good load factors)
/// - Cost control (efficient aircraft, optimal routing)
/// - Strategic expansion (new routes, fleet growth)
/// - Reputation management (on-time performance, service quality)
/// </remarks>
public class Airline
{
    /// <summary>
    /// Gets or sets the airline's unique identifier.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the airline's name (e.g., "SkyWings Airlines").
    /// </summary>
    public required string Name { get; set; }

    /// <summary>
    /// Gets or sets the current cash balance in USD.
    /// Like RCT's cash, this is the primary constraint on expansion.
    /// </summary>
    public decimal Cash { get; set; }

    /// <summary>
    /// Gets the airline's fleet of aircraft.
    /// Similar to RCT's list of rides, this is the core asset collection.
    /// </summary>
    public List<Aircraft> Fleet { get; init; } = [];

    /// <summary>
    /// Gets the airline's route network.
    /// Like RCT's paths and attractions, routes are where the business happens.
    /// </summary>
    public List<Route> Routes { get; init; } = [];

    /// <summary>
    /// Gets the list of active random events currently affecting the airline.
    /// Like RCT's event log, this tracks ongoing events and their effects.
    /// </summary>
    public List<GameEvent> ActiveEvents { get; init; } = [];

    /// <summary>
    /// Gets or sets the airline's reputation rating from 0 (terrible) to 100 (excellent).
    /// Like RCT's park rating, reputation affects passenger demand and ticket prices.
    /// </summary>
    public int Reputation { get; set; } = 50;

    /// <summary>
    /// Gets or sets the home hub airport code.
    /// The airline's main base of operations, like RCT's park entrance.
    /// </summary>
    public required string HomeHub { get; set; }

    /// <summary>
    /// Gets the total number of passengers carried (all-time).
    /// </summary>
    public int TotalPassengersCarried { get; private set; }

    /// <summary>
    /// Gets the total revenue generated (all-time).
    /// </summary>
    public decimal TotalRevenue { get; private set; }

    /// <summary>
    /// Gets the total operating costs incurred (all-time).
    /// </summary>
    public decimal TotalCosts { get; private set; }

    /// <summary>
    /// Gets the net profit/loss for all time (TotalRevenue - TotalCosts).
    /// </summary>
    public decimal NetProfit => this.TotalRevenue - this.TotalCosts;

    /// <summary>
    /// Gets the current game day (used for tracking aircraft age, route history, etc.).
    /// </summary>
    public int CurrentDay { get; private set; } = 1;

    /// <summary>
    /// Gets whether the airline is bankrupt (negative cash and unable to operate).
    /// Game over condition, like having zero guests and money in RCT.
    /// </summary>
    public bool IsBankrupt => this.Cash < 0 && this.GetDailyOperatingCost() > 0;

    /// <summary>
    /// Advances the game by one day, processing all operations.
    /// This is the core game loop, similar to RCT's daily park update.
    /// </summary>
    /// <returns>A summary of the day's operations.</returns>
    public DailyOperationsSummary ProcessDay()
    {
        return this.ProcessDay(null);
    }

    /// <summary>
    /// Advances the game by one day, processing all operations with competition.
    /// Calculates market share when multiple airlines serve the same routes.
    /// </summary>
    /// <param name="competitors">List of competitor airlines for market share calculations.</param>
    /// <returns>A summary of the day's operations.</returns>
    public DailyOperationsSummary ProcessDay(List<CompetitorAirline>? competitors)
    {
        this.CurrentDay++;

        // Remove expired events
        this.ActiveEvents.RemoveAll(e => !e.IsActive(this.CurrentDay));

        decimal dailyRevenue = 0m;
        decimal dailyCosts = 0m;
        int passengersCarried = 0;

        // Process each active route
        foreach (var route in this.Routes.Where(r => r.IsActive && r.AssignedAircraft != null))
        {
            // Simulate the route's daily operations (with competition if applicable)
            var routeResult = this.SimulateRouteDay(route, competitors);

            dailyRevenue += routeResult.Revenue;
            dailyCosts += routeResult.Costs;
            passengersCarried += routeResult.Passengers;

            // Update route performance metrics
            route.UpdatePerformance(routeResult.LoadFactor, routeResult.Profit, routeResult.Passengers);

            // Add flight hours to aircraft
            route.AssignedAircraft!.AddFlightHours(route.FlightTimeHours * route.DailyFlights);
        }

        // Add lease payments to daily costs
        dailyCosts += this.GetDailyLeaseCosts();

        // Apply event financial impacts (one-time costs/bonuses from new events)
        decimal eventImpact = this.ActiveEvents
            .Where(e => e.OccurredOnDay == this.CurrentDay)
            .Sum(e => e.FinancialImpact);

        // Update airline financial state
        decimal dailyProfit = dailyRevenue - dailyCosts + eventImpact;
        this.Cash += dailyProfit;
        this.TotalRevenue += dailyRevenue;
        this.TotalCosts += dailyCosts;
        this.TotalPassengersCarried += passengersCarried;

        // Update reputation based on performance
        this.UpdateReputation(passengersCarried);

        // Apply event reputation impacts
        int eventReputationImpact = this.ActiveEvents
            .Where(e => e.OccurredOnDay == this.CurrentDay)
            .Sum(e => e.ReputationImpact);
        this.Reputation = Math.Clamp(this.Reputation + eventReputationImpact, 0, 100);

        return new DailyOperationsSummary
        {
            Day = this.CurrentDay,
            Revenue = dailyRevenue,
            Costs = dailyCosts,
            Profit = dailyProfit,
            PassengersCarried = passengersCarried,
            CashBalance = this.Cash,
            Reputation = this.Reputation
        };
    }

    /// <summary>
    /// Simulates one day of operations for a specific route.
    /// Calculates revenue, costs, load factor, and passenger count.
    /// </summary>
    /// <param name="route">The route to simulate.</param>
    /// <param name="competitors">Optional list of competitors for market share calculations.</param>
    private RouteOperationsResult SimulateRouteDay(Route route, List<CompetitorAirline>? competitors)
    {
        if (route.AssignedAircraft == null)
        {
            return new RouteOperationsResult();
        }

        var aircraft = route.AssignedAircraft;

        // Calculate potential demand based on market sizes
        int basedemand = CalculateBaseDemand(route.Origin.MarketSize, route.Destination.MarketSize);

        // Apply reputation modifier (higher reputation = more demand)
        double reputationModifier = 0.5 + (this.Reputation / 100.0); // 0.5x to 1.5x

        // Apply event demand modifiers (weather, market events, etc.)
        double eventDemandModifier = this.GetActiveEventDemandModifier();

        int adjustedDemand = (int)(basedemand * reputationModifier * eventDemandModifier);

        // Apply market share if there are competitors on this route
        if (competitors != null && competitors.Any())
        {
            var competingAirlines = MarketCompetition.FindCompetingAirlines(
                route.Origin.Code,
                route.Destination.Code,
                this,
                competitors
            );

            if (competingAirlines.Count > 1)
            {
                // Multiple airlines serve this route - split demand
                double marketShare = MarketCompetition.CalculateMarketShare(
                    this,
                    route,
                    competingAirlines,
                    competitors
                );

                adjustedDemand = (int)(adjustedDemand * marketShare);
            }
        }

        // Calculate capacity
        int dailyCapacity = aircraft.Type.Capacity * route.DailyFlights;

        // Load factor is demand vs capacity, capped at 95% (never 100% in real world)
        double loadFactor = Math.Min(0.95, adjustedDemand / (double)dailyCapacity);

        // Actual passengers carried
        int passengers = (int)(dailyCapacity * loadFactor);

        // Revenue = passengers × ticket price
        decimal revenue = passengers * route.TicketPrice;

        // Calculate costs
        decimal fuelCost = CalculateFuelCost(route.DistanceNauticalMiles, aircraft.Type, route.DailyFlights);
        decimal crewCost = CalculateCrewCost(route.FlightTimeHours, route.DailyFlights);
        decimal airportFees = (route.Origin.LandingFee + route.Destination.LandingFee) * route.DailyFlights;
        decimal maintenanceCost = aircraft.Type.OperatingCostPerHour * (decimal)route.FlightTimeHours * route.DailyFlights * 0.15m;

        decimal baseCosts = fuelCost + crewCost + airportFees + maintenanceCost;

        // Apply event cost modifiers (fuel price spikes, strikes, etc.)
        double eventCostModifier = this.GetActiveEventCostModifier();
        decimal totalCosts = baseCosts * (decimal)eventCostModifier;

        decimal profit = revenue - totalCosts;

        return new RouteOperationsResult
        {
            Revenue = revenue,
            Costs = totalCosts,
            Profit = profit,
            Passengers = passengers,
            LoadFactor = loadFactor
        };
    }

    /// <summary>
    /// Calculates base passenger demand based on origin and destination market sizes.
    /// Larger markets generate more potential passengers.
    /// </summary>
    private static int CalculateBaseDemand(MarketSize origin, MarketSize destination)
    {
        // Demand is based on combined market sizes
        // Similar to RCT's park visitor calculations based on park rating and attractions
        int originValue = origin switch
        {
            MarketSize.Small => 100,
            MarketSize.Medium => 300,
            MarketSize.Large => 600,
            MarketSize.VeryLarge => 1000,
            _ => 100
        };

        int destinationValue = destination switch
        {
            MarketSize.Small => 100,
            MarketSize.Medium => 300,
            MarketSize.Large => 600,
            MarketSize.VeryLarge => 1000,
            _ => 100
        };

        // Average the two markets and add variance
        return (originValue + destinationValue) / 2;
    }

    /// <summary>
    /// Calculates fuel cost for a flight segment.
    /// Based on distance, aircraft fuel consumption, and current fuel price.
    /// </summary>
    private static decimal CalculateFuelCost(int distanceNM, AircraftType aircraftType, int flights)
    {
        // Fuel price approximately $3.00 per gallon (typical jet fuel)
        const decimal fuelPricePerGallon = 3.00m;

        // Total fuel consumption for all flights
        decimal totalFuelGallons = aircraftType.FuelConsumptionPerHour * flights;

        return totalFuelGallons * fuelPricePerGallon;
    }

    /// <summary>
    /// Calculates crew cost for flight operations.
    /// Includes pilots and flight attendants based on aircraft size.
    /// </summary>
    private static decimal CalculateCrewCost(double flightTimeHours, int flights)
    {
        // Typical crew hourly cost: $500/hour (pilots + attendants)
        const decimal crewCostPerHour = 500m;

        return (decimal)flightTimeHours * flights * crewCostPerHour;
    }

    /// <summary>
    /// Gets the total daily lease costs for all leased aircraft.
    /// </summary>
    private decimal GetDailyLeaseCosts()
    {
        // Convert monthly lease to daily (30 days per month)
        return this.Fleet.Where(a => a.IsLeased).Sum(a => a.MonthlyLeasePayment / 30m);
    }

    /// <summary>
    /// Gets the total daily operating cost (used for bankruptcy check).
    /// </summary>
    private decimal GetDailyOperatingCost()
    {
        decimal leaseCosts = this.GetDailyLeaseCosts();
        // Add fixed costs (office, staff, etc.) - roughly $10k/day for small airline
        decimal fixedCosts = 10000m;
        return leaseCosts + fixedCosts;
    }

    /// <summary>
    /// Updates airline reputation based on performance metrics.
    /// Like RCT's park rating, reputation fluctuates based on service quality.
    /// </summary>
    private void UpdateReputation(int passengersCarried)
    {
        // Reputation slowly drifts toward 50 (average)
        // Good performance (high passenger counts) pulls it up
        // Poor performance pulls it down

        int targetReputation = passengersCarried > 1000 ? 70 : passengersCarried > 500 ? 60 : 40;

        // Gradually move toward target (10% per day)
        int reputationChange = (targetReputation - this.Reputation) / 10;
        this.Reputation = Math.Clamp(this.Reputation + reputationChange, 0, 100);
    }

    /// <summary>
    /// Gets the aggregate demand modifier from all active events.
    /// Multiple events multiply together (e.g., 1.2 * 0.8 = 0.96).
    /// </summary>
    private double GetActiveEventDemandModifier()
    {
        if (!this.ActiveEvents.Any())
        {
            return 1.0;
        }

        return this.ActiveEvents
            .Where(e => e.IsActive(this.CurrentDay))
            .Aggregate(1.0, (current, evt) => current * evt.DemandModifier);
    }

    /// <summary>
    /// Gets the aggregate cost modifier from all active events.
    /// Multiple events multiply together (e.g., 1.3 * 1.1 = 1.43).
    /// </summary>
    private double GetActiveEventCostModifier()
    {
        if (!this.ActiveEvents.Any())
        {
            return 1.0;
        }

        return this.ActiveEvents
            .Where(e => e.IsActive(this.CurrentDay))
            .Aggregate(1.0, (current, evt) => current * evt.CostModifier);
    }

    /// <summary>
    /// Opens a new route between two airports.
    /// Like building a new ride in RCT, this expands the business.
    /// </summary>
    /// <param name="origin">Origin airport.</param>
    /// <param name="destination">Destination airport.</param>
    /// <param name="ticketPrice">Base ticket price.</param>
    /// <returns>The newly created route.</returns>
    public Route OpenRoute(Airport origin, Airport destination, decimal ticketPrice)
    {
        int distance = Route.CalculateDistance(origin.Code, destination.Code);

        var route = new Route
        {
            Origin = origin,
            Destination = destination,
            DistanceNauticalMiles = distance,
            TicketPrice = ticketPrice,
            DailyFlights = 1,
            OpenedOnDay = this.CurrentDay
        };

        this.Routes.Add(route);
        return route;
    }

    /// <summary>
    /// Closes a route and removes it from the network.
    /// Like removing a ride in RCT, this frees up resources.
    /// </summary>
    public void CloseRoute(Route route)
    {
        route.UnassignAircraft();
        this.Routes.Remove(route);
    }

    /// <summary>
    /// Purchases an aircraft outright, deducting the cost from cash.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown if insufficient funds.</exception>
    public Aircraft PurchaseAircraft(AircraftType type)
    {
        if (this.Cash < type.PurchasePrice)
        {
            throw new InvalidOperationException(
                $"Insufficient funds to purchase {type.Name}. Need ${type.PurchasePrice:N0}, have ${this.Cash:N0}"
            );
        }

        this.Cash -= type.PurchasePrice;

        var aircraft = Aircraft.CreateOwned(
            type,
            Aircraft.GenerateRegistrationNumber(),
            this.CurrentDay
        );

        this.Fleet.Add(aircraft);
        return aircraft;
    }

    /// <summary>
    /// Leases an aircraft, adding it to the fleet with monthly payments.
    /// Like taking a loan in RCT, this provides immediate access without large capital outlay.
    /// </summary>
    public Aircraft LeaseAircraft(AircraftType type)
    {
        var aircraft = Aircraft.CreateLeased(
            type,
            Aircraft.GenerateRegistrationNumber(),
            this.CurrentDay
        );

        this.Fleet.Add(aircraft);
        return aircraft;
    }

    /// <summary>
    /// Sells an owned aircraft, adding 70% of purchase price to cash.
    /// Like selling a ride in RCT, this provides immediate cash but reduces capacity.
    /// </summary>
    /// <param name="aircraft">The aircraft to sell.</param>
    /// <exception cref="InvalidOperationException">Thrown if aircraft is leased or assigned to a route.</exception>
    public void SellAircraft(Aircraft aircraft)
    {
        if (aircraft.IsLeased)
        {
            throw new InvalidOperationException(
                $"Cannot sell leased aircraft {aircraft.RegistrationNumber}. Use ReturnLeasedAircraft instead."
            );
        }

        if (aircraft.AssignedRoute != null)
        {
            throw new InvalidOperationException(
                $"Cannot sell aircraft {aircraft.RegistrationNumber} while assigned to route {aircraft.AssignedRoute.Name}. Unassign first."
            );
        }

        // Sell for 70% of purchase price
        decimal salePrice = aircraft.Type.PurchasePrice * 0.70m;
        this.Cash += salePrice;
        this.Fleet.Remove(aircraft);
    }

    /// <summary>
    /// Returns a leased aircraft, paying early termination penalty.
    /// Penalty is 2 months of lease payments.
    /// </summary>
    /// <param name="aircraft">The aircraft to return.</param>
    /// <exception cref="InvalidOperationException">Thrown if aircraft is owned or assigned to a route.</exception>
    public void ReturnLeasedAircraft(Aircraft aircraft)
    {
        if (!aircraft.IsLeased)
        {
            throw new InvalidOperationException(
                $"Cannot return owned aircraft {aircraft.RegistrationNumber}. Use SellAircraft instead."
            );
        }

        if (aircraft.AssignedRoute != null)
        {
            throw new InvalidOperationException(
                $"Cannot return aircraft {aircraft.RegistrationNumber} while assigned to route {aircraft.AssignedRoute.Name}. Unassign first."
            );
        }

        // Early termination penalty: 2 months of lease payments
        decimal penalty = aircraft.MonthlyLeasePayment * 2m;

        if (this.Cash < penalty)
        {
            throw new InvalidOperationException(
                $"Insufficient funds to pay early termination penalty. Need ${penalty:N0}, have ${this.Cash:N0}"
            );
        }

        this.Cash -= penalty;
        this.Fleet.Remove(aircraft);
    }
}

/// <summary>
/// Summary of daily airline operations.
/// Like RCT's end-of-month park report, this shows performance metrics.
/// </summary>
public record DailyOperationsSummary
{
    public required int Day { get; init; }
    public required decimal Revenue { get; init; }
    public required decimal Costs { get; init; }
    public required decimal Profit { get; init; }
    public required int PassengersCarried { get; init; }
    public required decimal CashBalance { get; init; }
    public required int Reputation { get; init; }
    public List<GameEvent> NewEvents { get; init; } = [];
}

/// <summary>
/// Result of simulating one day of route operations.
/// Internal data structure for route profitability calculations.
/// </summary>
internal record RouteOperationsResult
{
    public decimal Revenue { get; init; }
    public decimal Costs { get; init; }
    public decimal Profit { get; init; }
    public int Passengers { get; init; }
    public double LoadFactor { get; init; }
}
