namespace AirlineTycoon.Domain.Events;

/// <summary>
/// Represents a random event that occurs during gameplay.
/// Inspired by RollerCoaster Tycoon's random events that create memorable moments and strategic challenges.
/// </summary>
/// <remarks>
/// Events in Airline Tycoon work like RCT's events:
/// - Weather disasters (like rides breaking down in storms)
/// - Economic shifts (like park attendance changes)
/// - Operational challenges (like staff strikes)
/// - Competitive actions (like rival parks opening nearby)
///
/// Events create:
/// - Unpredictability (no two games play the same)
/// - Strategic decisions (adapt to changing conditions)
/// - Memorable moments ("Remember when fuel prices doubled?")
/// - Replayability (different events each playthrough)
/// </remarks>
public class GameEvent
{
    /// <summary>
    /// Gets the unique identifier for this event instance.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets the type of event.
    /// </summary>
    public required EventType Type { get; init; }

    /// <summary>
    /// Gets the severity level of this event.
    /// </summary>
    public required EventSeverity Severity { get; init; }

    /// <summary>
    /// Gets the title of the event (displayed to player).
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Gets the description explaining what happened.
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Gets the game day when this event occurred.
    /// </summary>
    public required int OccurredOnDay { get; init; }

    /// <summary>
    /// Gets the duration in days this event lasts (0 for instant events).
    /// </summary>
    public int DurationDays { get; init; }

    /// <summary>
    /// Gets the financial impact (positive or negative).
    /// </summary>
    public decimal FinancialImpact { get; init; }

    /// <summary>
    /// Gets the reputation impact (positive or negative).
    /// </summary>
    public int ReputationImpact { get; init; }

    /// <summary>
    /// Gets the demand modifier percentage (1.0 = no change, 1.5 = +50%, 0.7 = -30%).
    /// </summary>
    public double DemandModifier { get; init; } = 1.0;

    /// <summary>
    /// Gets the cost modifier percentage (1.0 = no change, 1.3 = +30%, 0.9 = -10%).
    /// </summary>
    public double CostModifier { get; init; } = 1.0;

    /// <summary>
    /// Gets whether this event affects all routes or specific ones.
    /// </summary>
    public bool AffectsAllRoutes { get; init; } = true;

    /// <summary>
    /// Gets specific route IDs affected (if AffectsAllRoutes is false).
    /// </summary>
    public List<Guid> AffectedRouteIds { get; init; } = [];

    /// <summary>
    /// Gets when this event expires (OccurredOnDay + DurationDays).
    /// </summary>
    public int ExpiresOnDay => this.OccurredOnDay + this.DurationDays;

    /// <summary>
    /// Checks if this event is still active on the given day.
    /// </summary>
    /// <param name="currentDay">The current game day.</param>
    /// <returns>True if event is still active.</returns>
    public bool IsActive(int currentDay)
    {
        if (this.DurationDays == 0)
        {
            return false; // Instant event
        }

        return currentDay < this.ExpiresOnDay;
    }

    /// <summary>
    /// Gets a summary string for displaying in event log.
    /// </summary>
    /// <returns>Formatted event summary.</returns>
    public string GetSummary()
    {
        var parts = new List<string>();

        if (this.FinancialImpact != 0)
        {
            parts.Add(this.FinancialImpact > 0
                ? $"+${this.FinancialImpact:N0}"
                : $"-${Math.Abs(this.FinancialImpact):N0}");
        }

        if (this.ReputationImpact != 0)
        {
            parts.Add(this.ReputationImpact > 0
                ? $"+{this.ReputationImpact} reputation"
                : $"{this.ReputationImpact} reputation");
        }

        if (this.DemandModifier != 1.0)
        {
            int demandPercent = (int)((this.DemandModifier - 1.0) * 100);
            parts.Add($"{(demandPercent > 0 ? "+" : "")}{demandPercent}% demand");
        }

        if (this.CostModifier != 1.0)
        {
            int costPercent = (int)((this.CostModifier - 1.0) * 100);
            parts.Add($"{(costPercent > 0 ? "+" : "")}{costPercent}% costs");
        }

        if (this.DurationDays > 0)
        {
            parts.Add($"{this.DurationDays} days");
        }

        return string.Join(" | ", parts);
    }
}

/// <summary>
/// Types of random events that can occur.
/// </summary>
public enum EventType
{
    /// <summary>
    /// Weather-related events (storms, hurricanes, fog, heat waves).
    /// </summary>
    Weather,

    /// <summary>
    /// Economic events (recession, boom, inflation, fuel price changes).
    /// </summary>
    Economic,

    /// <summary>
    /// Operational events (strikes, maintenance issues, technology failures).
    /// </summary>
    Operational,

    /// <summary>
    /// Market events (tourism booms, seasonal demand, competitor actions).
    /// </summary>
    Market,

    /// <summary>
    /// Positive PR events (awards, good press, viral marketing).
    /// </summary>
    PositivePR,

    /// <summary>
    /// Negative PR events (complaints, accidents at other airlines, bad press).
    /// </summary>
    NegativePR
}

/// <summary>
/// Severity levels for events determining their impact magnitude.
/// </summary>
public enum EventSeverity
{
    /// <summary>
    /// Minor event with small impact (5-10% effects).
    /// </summary>
    Minor,

    /// <summary>
    /// Moderate event with noticeable impact (15-25% effects).
    /// </summary>
    Moderate,

    /// <summary>
    /// Major event with significant impact (30-50% effects).
    /// </summary>
    Major,

    /// <summary>
    /// Critical event with dramatic impact (50%+ effects).
    /// </summary>
    Critical
}
