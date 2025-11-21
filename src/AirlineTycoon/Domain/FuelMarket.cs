namespace AirlineTycoon.Domain;

/// <summary>
/// Represents the global fuel market with dynamic pricing.
/// Models oil market volatility, seasonal variations, and geopolitical impacts.
/// </summary>
/// <remarks>
/// The fuel market adds economic realism to the simulation by creating:
/// - Base price that fluctuates around historical averages
/// - Seasonal variations (winter demand spikes, summer travel peaks)
/// - Random market volatility (supply disruptions, demand shocks)
/// - Long-term price trends (boom/bust cycles)
///
/// This creates strategic decisions:
/// - Timing aircraft purchases when fuel is cheap
/// - Adjusting route profitability calculations
/// - Managing operating costs dynamically
/// - Hedging strategies (future feature)
///
/// Historical context:
/// - Jet fuel typically ranges from $1.50 to $4.50 per gallon
/// - Major spikes occurred during Gulf War (1990), Iraq War (2003-2008), COVID recovery (2021-2022)
/// - Seasonal variation is typically ±15% from baseline
/// </remarks>
public class FuelMarket
{
    private const decimal BaselineFuelPrice = 3.00m; // Historical average in USD per gallon
    private const decimal MinFuelPrice = 1.50m;  // Floor price (extreme oversupply)
    private const decimal MaxFuelPrice = 6.00m;  // Ceiling price (extreme shortage)

    private readonly Random random;
    private decimal currentPrice;
    private int daysSinceLastUpdate;
    private decimal priceTrend; // Long-term trend factor (-0.2 to +0.2)

    /// <summary>
    /// Gets the current fuel price per gallon in USD.
    /// </summary>
    public decimal CurrentPricePerGallon => this.currentPrice;

    /// <summary>
    /// Gets the 30-day moving average price (for trend analysis).
    /// </summary>
    public decimal MovingAveragePrice { get; private set; }

    /// <summary>
    /// Gets the percentage change from baseline (for display).
    /// </summary>
    public decimal PercentageFromBaseline =>
        ((this.currentPrice - BaselineFuelPrice) / BaselineFuelPrice) * 100m;

    /// <summary>
    /// Initializes a new instance of the <see cref="FuelMarket"/> class.
    /// </summary>
    /// <param name="seed">Optional random seed for deterministic testing.</param>
    public FuelMarket(int? seed = null)
    {
        this.random = seed.HasValue ? new Random(seed.Value) : new Random();
        this.currentPrice = BaselineFuelPrice;
        this.MovingAveragePrice = BaselineFuelPrice;
        this.priceTrend = 0m;
        this.daysSinceLastUpdate = 0;
    }

    /// <summary>
    /// Updates the fuel market for a new day.
    /// Applies daily volatility, seasonal factors, and long-term trends.
    /// </summary>
    /// <param name="currentDay">The current game day (for seasonal calculation).</param>
    /// <remarks>
    /// Daily volatility:
    /// - 70% chance: small change (±2%)
    /// - 20% chance: moderate change (±5%)
    /// - 10% chance: significant change (±10%)
    ///
    /// Seasonal factors (based on day of year):
    /// - Winter (Dec-Feb): +8% (heating oil demand, harsh weather)
    /// - Spring (Mar-May): +2% (moderate demand)
    /// - Summer (Jun-Aug): +12% (peak travel season)
    /// - Fall (Sep-Nov): -5% (lowest demand)
    ///
    /// Long-term trend:
    /// - Random walk that changes every 30 days
    /// - Simulates multi-month supply/demand cycles
    /// </remarks>
    public void UpdateMarket(int currentDay)
    {
        this.daysSinceLastUpdate++;

        // Update long-term trend every 30 days
        if (this.daysSinceLastUpdate >= 30)
        {
            // Random walk: -0.05 to +0.05 change to trend
            this.priceTrend += (decimal)(this.random.NextDouble() * 0.10 - 0.05);
            this.priceTrend = Math.Clamp(this.priceTrend, -0.2m, 0.2m); // Keep trend moderate
            this.daysSinceLastUpdate = 0;
        }

        // Calculate daily volatility
        decimal dailyChange = this.CalculateDailyVolatility();

        // Calculate seasonal multiplier
        decimal seasonalMultiplier = CalculateSeasonalMultiplier(currentDay);

        // Apply changes to price
        decimal trendEffect = BaselineFuelPrice * this.priceTrend;
        decimal newPrice = this.currentPrice + dailyChange + trendEffect;

        // Apply seasonal multiplier
        newPrice *= seasonalMultiplier;

        // Clamp to realistic bounds
        this.currentPrice = Math.Clamp(newPrice, MinFuelPrice, MaxFuelPrice);

        // Update moving average (simple exponential moving average)
        this.MovingAveragePrice = (this.MovingAveragePrice * 29m + this.currentPrice) / 30m;
    }

    /// <summary>
    /// Applies a one-time market shock (geopolitical event, major disruption).
    /// </summary>
    /// <param name="magnitude">Price change multiplier (e.g., 1.30 for +30% shock, 0.85 for -15% shock).</param>
    /// <param name="duration">Number of days the shock persists.</param>
    /// <remarks>
    /// This method is called by the event system when major fuel market events occur.
    /// Examples:
    /// - OPEC production cut: +25% for 45 days
    /// - Major refinery fire: +18% for 20 days
    /// - New oil field discovery: -12% for 60 days
    /// - Recession demand drop: -15% for 90 days
    /// </remarks>
    public void ApplyMarketShock(decimal magnitude, int duration)
    {
        this.currentPrice *= magnitude;
        this.currentPrice = Math.Clamp(this.currentPrice, MinFuelPrice, MaxFuelPrice);

        // TODO: Track shock duration and gradually return to baseline
        // For MVP, shock is applied immediately and then market naturally corrects
    }

    /// <summary>
    /// Calculates daily price volatility based on random market forces.
    /// </summary>
    /// <returns>Price change in USD per gallon.</returns>
    private decimal CalculateDailyVolatility()
    {
        double roll = this.random.NextDouble();
        double percentChange;

        if (roll < 0.70) // 70% chance: small change
        {
            percentChange = (this.random.NextDouble() * 0.04 - 0.02); // ±2%
        }
        else if (roll < 0.90) // 20% chance: moderate change
        {
            percentChange = (this.random.NextDouble() * 0.10 - 0.05); // ±5%
        }
        else // 10% chance: significant change
        {
            percentChange = (this.random.NextDouble() * 0.20 - 0.10); // ±10%
        }

        return this.currentPrice * (decimal)percentChange;
    }

    /// <summary>
    /// Calculates seasonal multiplier based on time of year.
    /// </summary>
    /// <param name="currentDay">Current game day.</param>
    /// <returns>Seasonal multiplier (e.g., 1.12 for +12% summer peak).</returns>
    private static decimal CalculateSeasonalMultiplier(int currentDay)
    {
        // Simplified seasonal model (365-day year)
        int dayOfYear = currentDay % 365;

        // Winter (days 0-59 and 335-364): +8%
        if (dayOfYear < 60 || dayOfYear >= 335)
        {
            return 1.08m;
        }

        // Spring (days 60-151): +2%
        if (dayOfYear < 152)
        {
            return 1.02m;
        }

        // Summer (days 152-243): +12% (peak travel season)
        if (dayOfYear < 244)
        {
            return 1.12m;
        }

        // Fall (days 244-334): -5% (lowest demand)
        return 0.95m;
    }

    /// <summary>
    /// Gets a human-readable market status description.
    /// </summary>
    /// <returns>Market status string (e.g., "Rising Prices", "Stable Market").</returns>
    public string GetMarketStatus()
    {
        decimal change = this.PercentageFromBaseline;

        return change switch
        {
            >= 25m => "Crisis: Extreme High Prices",
            >= 15m => "Alert: Significantly High Prices",
            >= 8m => "Warning: Rising Prices",
            >= -8m => "Stable: Normal Market Conditions",
            >= -15m => "Favorable: Declining Prices",
            _ => "Opportunity: Historic Low Prices"
        };
    }
}
