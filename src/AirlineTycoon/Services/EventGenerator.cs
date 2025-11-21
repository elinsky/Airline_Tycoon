using AirlineTycoon.Domain;
using AirlineTycoon.Domain.Events;

namespace AirlineTycoon.Services;

/// <summary>
/// Generates random events that occur during gameplay.
/// Inspired by RCT's event system that creates memorable moments and strategic challenges.
/// </summary>
/// <remarks>
/// The EventGenerator uses probability-based selection to create varied, interesting events:
/// - Base probability for any event occurring each day
/// - Different probabilities for different event types
/// - Severity determined by random roll
/// - Event effects scale with severity
///
/// Event frequency is tuned to create tension without overwhelming the player:
/// - ~10-15% chance of any event per day
/// - Roughly 1 event every 7-10 days on average
/// - Multiple active events can overlap
/// - More severe events are rarer
/// </remarks>
public class EventGenerator
{
    private readonly Random random;
    private const double BaseEventProbability = 0.12; // 12% chance per day

    /// <summary>
    /// Initializes a new instance of the <see cref="EventGenerator"/> class.
    /// </summary>
    /// <param name="seed">Optional random seed for deterministic testing.</param>
    public EventGenerator(int? seed = null)
    {
        this.random = seed.HasValue ? new Random(seed.Value) : new Random();
    }

    /// <summary>
    /// Attempts to generate a random event for the current day.
    /// </summary>
    /// <param name="currentDay">The current game day.</param>
    /// <param name="airline">The player's airline (used for context-aware events).</param>
    /// <returns>A generated event, or null if no event occurs.</returns>
    public GameEvent? TryGenerateEvent(int currentDay, Airline airline)
    {
        // Roll for event occurrence
        if (this.random.NextDouble() > BaseEventProbability)
        {
            return null; // No event this day
        }

        // Determine event type
        var eventType = this.SelectEventType();

        // Determine severity (rarer events are more severe)
        var severity = this.SelectSeverity();

        // Generate the specific event
        return eventType switch
        {
            EventType.Weather => this.GenerateWeatherEvent(currentDay, severity, airline),
            EventType.Economic => this.GenerateEconomicEvent(currentDay, severity),
            EventType.Operational => this.GenerateOperationalEvent(currentDay, severity, airline),
            EventType.Market => this.GenerateMarketEvent(currentDay, severity),
            EventType.PositivePR => this.GeneratePositivePREvent(currentDay, severity, airline),
            EventType.NegativePR => this.GenerateNegativePREvent(currentDay, severity),
            _ => null
        };
    }

    /// <summary>
    /// Selects a random event type based on weighted probabilities.
    /// </summary>
    private EventType SelectEventType()
    {
        double roll = this.random.NextDouble();

        // Weighted distribution
        if (roll < 0.30) return EventType.Weather;         // 30% - Most common
        if (roll < 0.50) return EventType.Economic;        // 20%
        if (roll < 0.70) return EventType.Market;          // 20%
        if (roll < 0.85) return EventType.Operational;     // 15%
        if (roll < 0.92) return EventType.PositivePR;      // 7%
        return EventType.NegativePR;                        // 8%
    }

    /// <summary>
    /// Selects a random severity level (more severe = rarer).
    /// </summary>
    private EventSeverity SelectSeverity()
    {
        double roll = this.random.NextDouble();

        if (roll < 0.50) return EventSeverity.Minor;       // 50%
        if (roll < 0.80) return EventSeverity.Moderate;    // 30%
        if (roll < 0.95) return EventSeverity.Major;       // 15%
        return EventSeverity.Critical;                      // 5%
    }

    /// <summary>
    /// Generates a weather-related event.
    /// </summary>
    private GameEvent GenerateWeatherEvent(int currentDay, EventSeverity severity, Airline airline)
    {
        var weatherEvents = severity switch
        {
            EventSeverity.Minor => new[]
            {
                ("Light Fog Delays", "Morning fog causes minor delays at several airports", 0.95, 1.05, 1),
                ("Windy Conditions", "Strong winds increase fuel consumption slightly", 0.98, 1.08, 1),
                ("Rain Delays", "Rain showers cause some flight delays", 0.97, 1.03, 1)
            },
            EventSeverity.Moderate => new[]
            {
                ("Severe Thunderstorms", "Thunderstorms force cancellations across the network", 0.80, 1.15, 2),
                ("Heavy Snow", "Snowstorm disrupts operations at northern airports", 0.75, 1.20, 3),
                ("Heat Wave", "Extreme heat reduces aircraft performance", 0.90, 1.18, 2)
            },
            EventSeverity.Major => new[]
            {
                ("Hurricane Warning", "Major hurricane threatens coastal routes", 0.60, 1.30, 4),
                ("Blizzard", "Severe blizzard shuts down multiple airports", 0.50, 1.40, 5),
                ("Ice Storm", "Ice storm causes widespread cancellations", 0.55, 1.35, 4)
            },
            _ => new[] // Critical
            {
                ("Category 5 Hurricane", "Catastrophic hurricane forces mass cancellations", 0.30, 1.60, 7),
                ("Volcanic Ash Cloud", "Volcanic eruption grounds flights across region", 0.20, 1.50, 10)
            }
        };

        var (title, description, demandMod, costMod, duration) = weatherEvents[this.random.Next(weatherEvents.Length)];

        return new GameEvent
        {
            Type = EventType.Weather,
            Severity = severity,
            Title = title,
            Description = description,
            OccurredOnDay = currentDay,
            DurationDays = duration,
            DemandModifier = demandMod,
            CostModifier = costMod,
            FinancialImpact = -airline.Cash * 0.02m * (decimal)GetSeverityMultiplier(severity), // Lost revenue
            ReputationImpact = -1 * (int)severity // Minor weather hurt reputation less
        };
    }

    /// <summary>
    /// Generates an economic event.
    /// </summary>
    private GameEvent GenerateEconomicEvent(int currentDay, EventSeverity severity)
    {
        var economicEvents = severity switch
        {
            EventSeverity.Minor => new[]
            {
                ("Fuel Price Increase", "Oil prices rise slightly", 1.0, 1.10, 5, 0),
                ("Dollar Strengthens", "Strong dollar helps international routes", 1.05, 0.98, 3, 0),
                ("Tourism Tax Credit", "New tax credit boosts leisure travel", 1.08, 1.0, 7, 0)
            },
            EventSeverity.Moderate => new[]
            {
                ("Fuel Crisis", "Oil shortage drives up fuel costs significantly", 0.90, 1.30, 10, 0),
                ("Economic Slowdown", "Recession fears reduce business travel", 0.85, 1.05, 14, 0),
                ("Currency Fluctuation", "Exchange rates impact international demand", 0.92, 1.15, 8, 0)
            },
            EventSeverity.Major => new[]
            {
                ("Fuel Price Spike", "OPEC cuts production, fuel prices soar", 0.80, 1.50, 15, 0),
                ("Recession", "Economic recession hits travel demand hard", 0.70, 1.10, 30, -3),
                ("Inflation Surge", "High inflation increases all operating costs", 0.95, 1.35, 20, 0)
            },
            _ => new[] // Critical
            {
                ("Fuel Crisis Emergency", "Fuel shortage threatens operations", 0.60, 2.00, 21, -5),
                ("Market Crash", "Stock market crash devastates travel demand", 0.50, 1.20, 30, -5)
            }
        };

        var (title, description, demandMod, costMod, duration, repImpact) = economicEvents[this.random.Next(economicEvents.Length)];

        return new GameEvent
        {
            Type = EventType.Economic,
            Severity = severity,
            Title = title,
            Description = description,
            OccurredOnDay = currentDay,
            DurationDays = duration,
            DemandModifier = demandMod,
            CostModifier = costMod,
            ReputationImpact = repImpact
        };
    }

    /// <summary>
    /// Generates an operational event.
    /// </summary>
    private GameEvent GenerateOperationalEvent(int currentDay, EventSeverity severity, Airline airline)
    {
        var operationalEvents = severity switch
        {
            EventSeverity.Minor => new[]
            {
                ("IT System Glitch", "Brief computer system issues cause delays", 0.98, 1.05, 1, -1, -5000m),
                ("Catering Delay", "Food service delays hold up some departures", 0.99, 1.03, 1, -1, -2000m),
                ("Minor Maintenance", "Routine checks find small issues fleet-wide", 1.0, 1.08, 2, 0, -8000m)
            },
            EventSeverity.Moderate => new[]
            {
                ("Ground Crew Strike", "Ground workers strike for better pay", 0.85, 1.20, 3, -3, -25000m),
                ("Equipment Failure", "Key equipment fails, disrupting operations", 0.80, 1.15, 2, -2, -35000m),
                ("Security Scare", "Security incident causes delays and screening backups", 0.90, 1.12, 2, -2, -15000m)
            },
            EventSeverity.Major => new[]
            {
                ("Pilot Strike", "Pilots union stages major work stoppage", 0.60, 1.30, 5, -5, -100000m),
                ("System-Wide Outage", "Computer systems crash, grounding flights", 0.50, 1.25, 3, -8, -150000m),
                ("Maintenance Crisis", "Safety inspection grounds part of fleet", 0.70, 1.40, 7, -4, -200000m)
            },
            _ => new[] // Critical
            {
                ("Major Strike", "All unionized employees walk out", 0.30, 1.50, 10, -10, -500000m),
                ("Safety Grounding", "FAA grounds entire fleet for safety review", 0.20, 1.60, 14, -15, -1000000m)
            }
        };

        var (title, description, demandMod, costMod, duration, repImpact, financialImpact) =
            operationalEvents[this.random.Next(operationalEvents.Length)];

        return new GameEvent
        {
            Type = EventType.Operational,
            Severity = severity,
            Title = title,
            Description = description,
            OccurredOnDay = currentDay,
            DurationDays = duration,
            DemandModifier = demandMod,
            CostModifier = costMod,
            ReputationImpact = repImpact,
            FinancialImpact = financialImpact
        };
    }

    /// <summary>
    /// Generates a market-related event.
    /// </summary>
    private GameEvent GenerateMarketEvent(int currentDay, EventSeverity severity)
    {
        var marketEvents = severity switch
        {
            EventSeverity.Minor => new[]
            {
                ("Convention Season", "Business conventions boost demand", 1.12, 1.0, 5, 1),
                ("Sports Tournament", "Major sporting event increases travel", 1.10, 1.0, 3, 1),
                ("Long Weekend", "Holiday weekend drives leisure travel", 1.15, 1.0, 3, 0)
            },
            EventSeverity.Moderate => new[]
            {
                ("Tourism Campaign", "Destination marketing boosts routes", 1.25, 1.0, 10, 2),
                ("Competitor Exits", "Rival airline closes routes, opening opportunity", 1.30, 0.95, 15, 3),
                ("Festival Season", "Cultural festivals attract international visitors", 1.20, 1.0, 7, 2)
            },
            EventSeverity.Major => new[]
            {
                ("World Event", "Global event (Olympics, World Cup) surges demand", 1.50, 1.10, 14, 5),
                ("Competitor Bankruptcy", "Major competitor goes out of business", 1.40, 1.0, 30, 4),
                ("Tourism Boom", "Destination becomes viral travel trend", 1.45, 1.05, 21, 3)
            },
            _ => new[] // Critical
            {
                ("Mega Event", "Once-in-a-lifetime event creates unprecedented demand", 1.80, 1.15, 21, 10),
                ("Market Monopoly", "All competitors exit, leaving you dominant", 1.70, 0.90, 60, 8)
            }
        };

        var (title, description, demandMod, costMod, duration, repImpact) = marketEvents[this.random.Next(marketEvents.Length)];

        return new GameEvent
        {
            Type = EventType.Market,
            Severity = severity,
            Title = title,
            Description = description,
            OccurredOnDay = currentDay,
            DurationDays = duration,
            DemandModifier = demandMod,
            CostModifier = costMod,
            ReputationImpact = repImpact
        };
    }

    /// <summary>
    /// Generates a positive PR event.
    /// </summary>
    private GameEvent GeneratePositivePREvent(int currentDay, EventSeverity severity, Airline airline)
    {
        var prEvents = severity switch
        {
            EventSeverity.Minor => new[]
            {
                ("Positive Review", "Travel blogger praises your service", 1.05, 1.0, 3, 2, 5000m),
                ("Social Media Buzz", "Viral video showcases great customer service", 1.08, 1.0, 5, 3, 10000m),
                ("Local Award", "Local business group recognizes your airline", 1.03, 1.0, 2, 2, 3000m)
            },
            EventSeverity.Moderate => new[]
            {
                ("Industry Award", "Win major airline industry award", 1.15, 0.98, 10, 5, 25000m),
                ("Celebrity Endorsement", "Celebrity posts about great flight experience", 1.20, 1.0, 7, 6, 50000m),
                ("Media Feature", "Major publication features your airline positively", 1.12, 1.0, 8, 4, 30000m)
            },
            EventSeverity.Major => new[]
            {
                ("Best Airline Award", "Named best airline in customer satisfaction", 1.30, 0.95, 15, 10, 100000m),
                ("Heroic Crew", "Your crew's heroic actions make national news", 1.25, 1.0, 12, 12, 75000m),
                ("Innovation Award", "Recognized for industry-leading innovation", 1.22, 0.98, 14, 8, 90000m)
            },
            _ => new[] // Critical
            {
                ("Airline of the Year", "Win prestigious Airline of the Year award", 1.50, 0.90, 30, 20, 250000m),
                ("Viral Success", "Unprecedented viral marketing success", 1.45, 0.95, 21, 15, 200000m)
            }
        };

        var (title, description, demandMod, costMod, duration, repImpact, financialImpact) =
            prEvents[this.random.Next(prEvents.Length)];

        return new GameEvent
        {
            Type = EventType.PositivePR,
            Severity = severity,
            Title = title,
            Description = description,
            OccurredOnDay = currentDay,
            DurationDays = duration,
            DemandModifier = demandMod,
            CostModifier = costMod,
            ReputationImpact = repImpact,
            FinancialImpact = financialImpact
        };
    }

    /// <summary>
    /// Generates a negative PR event.
    /// </summary>
    private GameEvent GenerateNegativePREvent(int currentDay, EventSeverity severity)
    {
        var prEvents = severity switch
        {
            EventSeverity.Minor => new[]
            {
                ("Customer Complaint", "Viral complaint video gets attention", 0.97, 1.0, 2, -2, -5000m),
                ("Baggage Issue", "Lost luggage incident reported in media", 0.98, 1.02, 3, -2, -8000m),
                ("Delay Complaints", "Social media complaints about delays", 0.96, 1.0, 2, -3, -6000m)
            },
            EventSeverity.Moderate => new[]
            {
                ("Service Scandal", "Poor service incident goes viral", 0.85, 1.05, 5, -6, -35000m),
                ("Safety Concern", "Minor safety concern reported by media", 0.80, 1.08, 7, -8, -50000m),
                ("Customer Lawsuit", "High-profile customer files lawsuit", 0.90, 1.10, 5, -5, -75000m)
            },
            EventSeverity.Major => new[]
            {
                ("PR Crisis", "Major PR disaster requires damage control", 0.70, 1.15, 10, -12, -150000m),
                ("Regulatory Fine", "FAA fines airline for violations", 0.85, 1.20, 8, -10, -250000m),
                ("Class Action", "Class action lawsuit filed over practices", 0.75, 1.12, 14, -15, -300000m)
            },
            _ => new[] // Critical
            {
                ("Major Scandal", "Devastating scandal rocks company", 0.50, 1.30, 21, -25, -750000m),
                ("Criminal Investigation", "Federal investigation launched", 0.40, 1.40, 30, -30, -1000000m)
            }
        };

        var (title, description, demandMod, costMod, duration, repImpact, financialImpact) =
            prEvents[this.random.Next(prEvents.Length)];

        return new GameEvent
        {
            Type = EventType.NegativePR,
            Severity = severity,
            Title = title,
            Description = description,
            OccurredOnDay = currentDay,
            DurationDays = duration,
            DemandModifier = demandMod,
            CostModifier = costMod,
            ReputationImpact = repImpact,
            FinancialImpact = financialImpact
        };
    }

    /// <summary>
    /// Gets a multiplier based on severity for scaling effects.
    /// </summary>
    private static double GetSeverityMultiplier(EventSeverity severity)
    {
        return severity switch
        {
            EventSeverity.Minor => 1.0,
            EventSeverity.Moderate => 2.0,
            EventSeverity.Major => 4.0,
            EventSeverity.Critical => 8.0,
            _ => 1.0
        };
    }
}
