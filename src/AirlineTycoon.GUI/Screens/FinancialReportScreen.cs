using System.Linq;
using AirlineTycoon.GUI.UI;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace AirlineTycoon.GUI.Screens;

/// <summary>
/// Financial report screen showing detailed financial data and charts.
/// Similar to RCT's financial summary screen.
/// </summary>
/// <remarks>
/// Displays:
/// - Revenue and cost breakdown
/// - Profit/loss trends over time
/// - Expense categories (fuel, crew, airport fees, leases)
/// - Income statement
/// - Historical performance charts
///
/// This gives players detailed insight into their financial performance
/// and helps identify areas for optimization.
/// </remarks>
public class FinancialReportScreen : Screen
{
    private UIButton? backButton;

    /// <inheritdoc/>
    public override string Title => "Financial Report";

    /// <summary>
    /// Initializes a new instance of the <see cref="FinancialReportScreen"/> class.
    /// </summary>
    /// <param name="controller">The game controller.</param>
    public FinancialReportScreen(GameController controller)
    {
        this.SetGameController(controller);
        this.InitializeUI();
    }

    /// <summary>
    /// Initializes the UI elements.
    /// </summary>
    private void InitializeUI()
    {
        this.backButton = new UIButton(
            "< Back",
            new Vector2(20, 50),
            new Vector2(120, 35)
        );
        this.backButton.Clicked += (s, e) => this.OnBack();
        this.AddChild(this.backButton);
    }

    /// <inheritdoc/>
    public override void Draw(SpriteBatch spriteBatch, GameTime gameTime)
    {
        if (!this.IsVisible)
        {
            return;
        }

        // Background
        this.DrawFilledRectangle(spriteBatch, this.Bounds, RetroColorPalette.WindowBackground);

        // Top bar
        this.DrawTopBar(spriteBatch, gameTime);

        // Income statement (left side)
        this.DrawIncomeStatement(spriteBatch);

        // Expense breakdown pie chart (top right)
        this.DrawExpenseBreakdown(spriteBatch);

        // Historical trend chart (bottom)
        this.DrawHistoricalChart(spriteBatch);

        // Draw buttons
        base.Draw(spriteBatch, gameTime);
    }

    /// <summary>
    /// Draws the income statement panel.
    /// </summary>
    private void DrawIncomeStatement(SpriteBatch spriteBatch)
    {
        // Panel background
        var panelBounds = new Rectangle(20, 100, 500, 600);
        this.DrawFilledRectangle(spriteBatch, panelBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.8f));
        this.Draw3DBorder(spriteBatch, panelBounds);

        // Title
        var titleBounds = new Rectangle(30, 110, 480, 30);
        this.DrawFilledRectangle(spriteBatch, titleBounds, RetroColorPalette.TitleBarBackground);

        if (AirlineTycoonGame.TextRenderer != null)
        {
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                "Income Statement",
                new Vector2(40, 117),
                Color.White
            );
        }

        // Statement area
        var statementBounds = new Rectangle(40, 160, 460, 520);
        this.DrawFilledRectangle(spriteBatch, statementBounds, Color.Black);
        this.Draw3DBorder(spriteBatch, statementBounds, 1);

        // Get actual financial data
        var airline = this.Controller?.Game.PlayerAirline;
        if (airline == null || AirlineTycoonGame.TextRenderer == null)
        {
            return;
        }

        // Calculate financial metrics from available data
        // Note: We calculate aggregated totals from route profits
        decimal totalProfit = airline.Routes.Sum(r => r.DailyProfit);
        decimal dailyLeaseCosts = airline.Fleet.Where(a => a.IsLeased).Sum(a => a.MonthlyLeasePayment / 30); // Approximate daily cost

        // Estimate revenue and costs based on profit margin
        // Typical airline operating margin is 8-10%, so we can back-calculate
        decimal estimatedRevenue = totalProfit > 0 ? totalProfit / 0.08m : 0;
        decimal estimatedExpenses = estimatedRevenue - totalProfit;

        // Rough breakdown of expenses (industry averages)
        decimal fuelCosts = estimatedExpenses * 0.35m;        // ~35% fuel
        decimal crewCosts = estimatedExpenses * 0.25m;        // ~25% labor
        decimal airportFees = estimatedExpenses * 0.15m;      // ~15% airport fees
        decimal maintenanceCosts = estimatedExpenses * 0.15m; // ~15% maintenance
        decimal leaseCosts = dailyLeaseCosts;                  // Actual lease costs

        decimal totalExpenses = fuelCosts + crewCosts + airportFees + maintenanceCosts + leaseCosts;
        decimal netProfit = totalProfit;

        // Revenue section
        int lineY = 180;
        int lineHeight = 30;
        int lineSpacing = 35;

        // Revenue line (green background)
        var revenueLine = new Rectangle(50, lineY, 440, lineHeight);
        this.DrawFilledRectangle(spriteBatch, revenueLine, RetroColorPalette.Darken(RetroColorPalette.Success, 0.5f));
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            "Revenue",
            new Vector2(60, lineY + 5),
            Color.White
        );
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            $"${estimatedRevenue:N0}",
            new Vector2(350, lineY + 5),
            RetroColorPalette.Success
        );

        // Expenses section
        lineY += lineSpacing + 20;

        // Section header
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            "Expenses:",
            new Vector2(60, lineY),
            RetroColorPalette.Warning
        );
        lineY += 25;

        // Fuel costs
        var fuelLine = new Rectangle(50, lineY, 440, lineHeight);
        this.DrawFilledRectangle(spriteBatch, fuelLine, RetroColorPalette.Darken(RetroColorPalette.Error, 0.7f));
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            "Fuel",
            new Vector2(60, lineY + 5),
            Color.White
        );
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            $"${fuelCosts:N0}",
            new Vector2(350, lineY + 5),
            RetroColorPalette.Error
        );
        lineY += lineSpacing;

        // Crew costs
        var crewLine = new Rectangle(50, lineY, 440, lineHeight);
        this.DrawFilledRectangle(spriteBatch, crewLine, RetroColorPalette.Darken(RetroColorPalette.Error, 0.7f));
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            "Crew",
            new Vector2(60, lineY + 5),
            Color.White
        );
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            $"${crewCosts:N0}",
            new Vector2(350, lineY + 5),
            RetroColorPalette.Error
        );
        lineY += lineSpacing;

        // Airport fees
        var airportLine = new Rectangle(50, lineY, 440, lineHeight);
        this.DrawFilledRectangle(spriteBatch, airportLine, RetroColorPalette.Darken(RetroColorPalette.Error, 0.7f));
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            "Airport Fees",
            new Vector2(60, lineY + 5),
            Color.White
        );
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            $"${airportFees:N0}",
            new Vector2(350, lineY + 5),
            RetroColorPalette.Error
        );
        lineY += lineSpacing;

        // Maintenance
        var maintenanceLine = new Rectangle(50, lineY, 440, lineHeight);
        this.DrawFilledRectangle(spriteBatch, maintenanceLine, RetroColorPalette.Darken(RetroColorPalette.Error, 0.7f));
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            "Maintenance",
            new Vector2(60, lineY + 5),
            Color.White
        );
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            $"${maintenanceCosts:N0}",
            new Vector2(350, lineY + 5),
            RetroColorPalette.Error
        );
        lineY += lineSpacing;

        // Lease payments
        var leaseLine = new Rectangle(50, lineY, 440, lineHeight);
        this.DrawFilledRectangle(spriteBatch, leaseLine, RetroColorPalette.Darken(RetroColorPalette.Error, 0.7f));
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            "Leases",
            new Vector2(60, lineY + 5),
            Color.White
        );
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            $"${leaseCosts:N0}",
            new Vector2(350, lineY + 5),
            RetroColorPalette.Error
        );
        lineY += lineSpacing + 20;

        // Total expenses line
        var totalExpensesLine = new Rectangle(50, lineY, 440, lineHeight);
        this.DrawFilledRectangle(spriteBatch, totalExpensesLine, RetroColorPalette.Darken(RetroColorPalette.Error, 0.5f));
        this.Draw3DBorder(spriteBatch, totalExpensesLine, 1);
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            "Total Expenses",
            new Vector2(60, lineY + 5),
            Color.White
        );
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            $"${totalExpenses:N0}",
            new Vector2(350, lineY + 5),
            RetroColorPalette.Error
        );
        lineY += lineSpacing + 30;

        // Net profit line (larger, highlighted)
        var profitLine = new Rectangle(50, lineY, 440, lineHeight + 10);
        Color profitColor = netProfit >= 0 ? RetroColorPalette.Success : RetroColorPalette.Error;
        this.DrawFilledRectangle(spriteBatch, profitLine, RetroColorPalette.Darken(profitColor, 0.7f));
        this.Draw3DBorder(spriteBatch, profitLine, 3);
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            "Net Profit",
            new Vector2(60, lineY + 8),
            Color.White
        );
        string profitText = netProfit >= 0 ? $"+${netProfit:N0}" : $"-${System.Math.Abs(netProfit):N0}";
        AirlineTycoonGame.TextRenderer.DrawText(
            spriteBatch,
            profitText,
            new Vector2(350, lineY + 8),
            profitColor
        );
    }

    /// <summary>
    /// Draws the expense breakdown as a pie chart.
    /// </summary>
    private void DrawExpenseBreakdown(SpriteBatch spriteBatch)
    {
        // Panel background
        var panelBounds = new Rectangle(540, 100, 720, 300);
        this.DrawFilledRectangle(spriteBatch, panelBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.8f));
        this.Draw3DBorder(spriteBatch, panelBounds);

        // Title
        var titleBounds = new Rectangle(550, 110, 700, 30);
        this.DrawFilledRectangle(spriteBatch, titleBounds, RetroColorPalette.TitleBarBackground);

        if (AirlineTycoonGame.TextRenderer != null)
        {
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                "Expense Breakdown",
                new Vector2(560, 117),
                Color.White
            );
        }

        // Get actual financial data
        var airline = this.Controller?.Game.PlayerAirline;
        if (airline == null || AirlineTycoonGame.TextRenderer == null)
        {
            return;
        }

        // Calculate expense breakdown (using same estimation as income statement)
        decimal totalProfit = airline.Routes.Sum(r => r.DailyProfit);
        decimal dailyLeaseCosts = airline.Fleet.Where(a => a.IsLeased).Sum(a => a.MonthlyLeasePayment / 30);

        decimal estimatedRevenue = totalProfit > 0 ? totalProfit / 0.08m : Math.Abs(totalProfit) * 12;
        decimal estimatedExpenses = estimatedRevenue - totalProfit;

        // Rough breakdown of expenses (industry averages)
        decimal fuelCosts = estimatedExpenses * 0.35m;        // ~35% fuel
        decimal crewCosts = estimatedExpenses * 0.25m;        // ~25% labor
        decimal airportFees = estimatedExpenses * 0.15m;      // ~15% airport fees
        decimal maintenanceCosts = estimatedExpenses * 0.15m; // ~15% maintenance
        decimal leaseCosts = dailyLeaseCosts;                  // Actual lease costs

        decimal totalExpenses = fuelCosts + crewCosts + airportFees + maintenanceCosts + leaseCosts;

        // Calculate percentages
        decimal fuelPercent = totalExpenses > 0 ? (fuelCosts / totalExpenses) * 100 : 0;
        decimal crewPercent = totalExpenses > 0 ? (crewCosts / totalExpenses) * 100 : 0;
        decimal airportPercent = totalExpenses > 0 ? (airportFees / totalExpenses) * 100 : 0;
        decimal maintenancePercent = totalExpenses > 0 ? (maintenanceCosts / totalExpenses) * 100 : 0;
        decimal leasePercent = totalExpenses > 0 ? (leaseCosts / totalExpenses) * 100 : 0;

        // Pie chart area (left side)
        var chartBounds = new Rectangle(570, 160, 220, 220);
        this.DrawFilledRectangle(spriteBatch, chartBounds, Color.Black);
        this.Draw3DBorder(spriteBatch, chartBounds, 1);

        // Draw simplified bar chart instead of pie chart (easier to render accurately)
        int centerX = 680;
        int centerY = 270;
        int barWidth = 180;
        int maxBarHeight = 180;

        decimal[] percentages = new decimal[] { fuelPercent, crewPercent, airportPercent, maintenancePercent, leasePercent };
        Color[] expenseColors = new Color[]
        {
            RetroColorPalette.Error,
            RetroColorPalette.Warning,
            RetroColorPalette.Info,
            RetroColorPalette.Success,
            RetroColorPalette.ButtonNormal
        };

        // Draw stacked bars
        int stackY = centerY + (maxBarHeight / 2);
        for (int i = 0; i < percentages.Length; i++)
        {
            if (percentages[i] > 0)
            {
                int barHeight = (int)((float)percentages[i] / 100 * maxBarHeight);
                stackY -= barHeight;
                var bar = new Rectangle(centerX - (barWidth / 2), stackY, barWidth, barHeight);
                this.DrawFilledRectangle(spriteBatch, bar, expenseColors[i]);
                this.Draw3DBorder(spriteBatch, bar, 1);
            }
        }

        // Legend (right side)
        int legendX = 820;
        int legendY = 170;
        int legendItemHeight = 35;

        string[] expenseLabels = new string[]
        {
            "Fuel",
            "Crew",
            "Airports",
            "Maintenance",
            "Leases"
        };

        decimal[] expenseAmounts = new decimal[] { fuelCosts, crewCosts, airportFees, maintenanceCosts, leaseCosts };

        for (int i = 0; i < expenseColors.Length; i++)
        {
            int y = legendY + (i * legendItemHeight);

            // Color swatch
            var swatch = new Rectangle(legendX, y, 30, 25);
            this.DrawFilledRectangle(spriteBatch, swatch, expenseColors[i]);
            this.Draw3DBorder(spriteBatch, swatch, 1);

            // Label background
            var labelBounds = new Rectangle(legendX + 40, y, 180, 25);
            this.DrawFilledRectangle(spriteBatch, labelBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.9f));
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                expenseLabels[i],
                new Vector2(legendX + 50, y + 4),
                Color.White
            );

            // Percentage background
            var percentBounds = new Rectangle(legendX + 230, y, 80, 25);
            this.DrawFilledRectangle(spriteBatch, percentBounds, expenseColors[i]);
            this.Draw3DBorder(spriteBatch, percentBounds, 1);
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                $"{percentages[i]:F1}%",
                new Vector2(legendX + 240, y + 4),
                Color.White
            );
        }
    }

    /// <summary>
    /// Draws the historical profit/loss trend chart.
    /// </summary>
    private void DrawHistoricalChart(SpriteBatch spriteBatch)
    {
        // Panel background
        var panelBounds = new Rectangle(540, 420, 720, 280);
        this.DrawFilledRectangle(spriteBatch, panelBounds, RetroColorPalette.Darken(RetroColorPalette.WindowBackground, 0.8f));
        this.Draw3DBorder(spriteBatch, panelBounds);

        // Title
        var titleBounds = new Rectangle(550, 430, 700, 30);
        this.DrawFilledRectangle(spriteBatch, titleBounds, RetroColorPalette.TitleBarBackground);

        if (AirlineTycoonGame.TextRenderer != null)
        {
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                "Profit Trend (Last 10 Days)",
                new Vector2(560, 437),
                Color.White
            );
        }

        // Chart area
        var chartBounds = new Rectangle(560, 480, 680, 200);
        this.DrawFilledRectangle(spriteBatch, chartBounds, Color.Black);
        this.Draw3DBorder(spriteBatch, chartBounds, 1);

        // Get actual financial data
        var airline = this.Controller?.Game.PlayerAirline;
        if (airline == null || AirlineTycoonGame.TextRenderer == null)
        {
            return;
        }

        // Grid lines (horizontal)
        for (int i = 1; i < 5; i++)
        {
            int y = 480 + (i * 40);
            var gridLine = new Rectangle(560, y, 680, 1);
            this.DrawFilledRectangle(spriteBatch, gridLine, RetroColorPalette.Darken(RetroColorPalette.PanelHighlight, 0.3f));
        }

        // Zero line (thicker, highlighted)
        var zeroLine = new Rectangle(560, 580, 680, 2);
        this.DrawFilledRectangle(spriteBatch, zeroLine, RetroColorPalette.Warning);

        // TODO: Once we track historical profit data, use actual values
        // For now, calculate current day profit
        decimal currentProfit = airline.Routes.Sum(r => r.DailyProfit);

        // Sample trend line (showing progression toward current profit)
        // In a future version, this should use actual historical data
        int[] trendPoints = new int[]
        {
            600,  // Day 1 (past)
            590,  // Day 2
            570,  // Day 3
            550,  // Day 4
            540,  // Day 5
            560,  // Day 6
            580,  // Day 7
            570,  // Day 8
            555,  // Day 9
            540   // Day 10 (current - will be replaced with actual data)
        };

        // Draw bar chart instead of line (easier to see profit/loss)
        for (int i = 0; i < trendPoints.Length; i++)
        {
            int x = 580 + (i * 60);
            int y = trendPoints[i];
            int zeroY = 580;

            int barHeight = System.Math.Abs(y - zeroY);
            int barY = System.Math.Min(y, zeroY);

            var bar = new Rectangle(x - 5, barY, 10, System.Math.Max(2, barHeight));

            // Color based on whether profit or loss
            Color barColor = y < zeroY ? RetroColorPalette.Success : RetroColorPalette.Error;
            this.DrawFilledRectangle(spriteBatch, bar, barColor);
        }

        // Draw labels for current day
        if (AirlineTycoonGame.TextRenderer != null)
        {
            // Profit label
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                "Current:",
                new Vector2(570, 490),
                RetroColorPalette.TextSecondary
            );

            string profitText = currentProfit >= 0 ? $"+${currentProfit:N0}" : $"-${System.Math.Abs(currentProfit):N0}";
            Color profitColor = currentProfit >= 0 ? RetroColorPalette.Success : RetroColorPalette.Error;
            AirlineTycoonGame.TextRenderer.DrawText(
                spriteBatch,
                profitText,
                new Vector2(640, 490),
                profitColor
            );
        }
    }

    /// <summary>
    /// Handles the back button click.
    /// </summary>
    private void OnBack()
    {
        this.Controller?.ShowDashboard();
    }
}
