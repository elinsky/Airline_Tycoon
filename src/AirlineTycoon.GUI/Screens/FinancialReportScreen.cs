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
            "← Back",
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

        // Statement area
        var statementBounds = new Rectangle(40, 160, 460, 520);
        this.DrawFilledRectangle(spriteBatch, statementBounds, Color.Black);
        this.Draw3DBorder(spriteBatch, statementBounds, 1);

        // Revenue section
        int lineY = 180;
        int lineHeight = 30;
        int lineSpacing = 35;

        // Revenue line (green background)
        var revenueLine = new Rectangle(50, lineY, 440, lineHeight);
        this.DrawFilledRectangle(spriteBatch, revenueLine, RetroColorPalette.Darken(RetroColorPalette.Success, 0.5f));

        // TODO: Display actual revenue number

        // Expenses section
        lineY += lineSpacing + 20;

        // Fuel costs
        var fuelLine = new Rectangle(50, lineY, 300, lineHeight);
        this.DrawFilledRectangle(spriteBatch, fuelLine, RetroColorPalette.Darken(RetroColorPalette.Error, 0.7f));
        lineY += lineSpacing;

        // Crew costs
        var crewLine = new Rectangle(50, lineY, 250, lineHeight);
        this.DrawFilledRectangle(spriteBatch, crewLine, RetroColorPalette.Darken(RetroColorPalette.Error, 0.7f));
        lineY += lineSpacing;

        // Airport fees
        var airportLine = new Rectangle(50, lineY, 200, lineHeight);
        this.DrawFilledRectangle(spriteBatch, airportLine, RetroColorPalette.Darken(RetroColorPalette.Error, 0.7f));
        lineY += lineSpacing;

        // Maintenance
        var maintenanceLine = new Rectangle(50, lineY, 150, lineHeight);
        this.DrawFilledRectangle(spriteBatch, maintenanceLine, RetroColorPalette.Darken(RetroColorPalette.Error, 0.7f));
        lineY += lineSpacing;

        // Lease payments
        var leaseLine = new Rectangle(50, lineY, 180, lineHeight);
        this.DrawFilledRectangle(spriteBatch, leaseLine, RetroColorPalette.Darken(RetroColorPalette.Error, 0.7f));
        lineY += lineSpacing + 20;

        // Total expenses line
        var totalExpensesLine = new Rectangle(50, lineY, 440, lineHeight);
        this.DrawFilledRectangle(spriteBatch, totalExpensesLine, RetroColorPalette.Darken(RetroColorPalette.Error, 0.5f));
        lineY += lineSpacing + 30;

        // Net profit line (larger, highlighted)
        var profitLine = new Rectangle(50, lineY, 440, lineHeight + 10);
        this.DrawFilledRectangle(spriteBatch, profitLine, RetroColorPalette.Info);
        this.Draw3DBorder(spriteBatch, profitLine, 2);

        // TODO: Display actual financial data once we have text rendering
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

        // Pie chart area (left side)
        var chartBounds = new Rectangle(570, 160, 220, 220);
        this.DrawFilledRectangle(spriteBatch, chartBounds, Color.Black);
        this.Draw3DBorder(spriteBatch, chartBounds, 1);

        // TODO: Draw actual pie chart
        // For now, draw concentric circles to show the layout
        var circle1 = new Rectangle(650, 240, 60, 60);
        this.DrawFilledRectangle(spriteBatch, circle1, RetroColorPalette.Error);

        // Legend (right side)
        int legendX = 820;
        int legendY = 170;
        int legendItemHeight = 35;

        Color[] expenseColors = new Color[]
        {
            RetroColorPalette.Error,
            RetroColorPalette.Warning,
            RetroColorPalette.Info,
            RetroColorPalette.Success,
            RetroColorPalette.ButtonNormal
        };

        string[] expenseLabels = new string[]
        {
            "Fuel",
            "Crew",
            "Airports",
            "Maintenance",
            "Leases"
        };

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

            // Percentage background
            var percentBounds = new Rectangle(legendX + 230, y, 80, 25);
            this.DrawFilledRectangle(spriteBatch, percentBounds, expenseColors[i]);
            this.Draw3DBorder(spriteBatch, percentBounds, 1);

            // TODO: Display actual labels and percentages
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

        // Chart area
        var chartBounds = new Rectangle(560, 480, 680, 200);
        this.DrawFilledRectangle(spriteBatch, chartBounds, Color.Black);
        this.Draw3DBorder(spriteBatch, chartBounds, 1);

        // TODO: Draw actual line chart showing profit/loss over time
        // For now, draw a sample trend line

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

        // Sample trend line (profit above zero, loss below)
        int[] trendPoints = new int[] { 600, 590, 570, 550, 540, 560, 580, 570, 555, 540 };

        for (int i = 0; i < trendPoints.Length - 1; i++)
        {
            int x1 = 580 + (i * 60);
            int y1 = trendPoints[i];
            int x2 = 580 + ((i + 1) * 60);
            int y2 = trendPoints[i + 1];

            // Vertical line segment (approximation of line)
            int minY = Math.Min(y1, y2);
            int maxY = Math.Max(y1, y2);
            int height = Math.Max(2, maxY - minY);

            var segment = new Rectangle(x1, minY, 3, height);

            // Color based on whether profit or loss
            Color lineColor = y1 < 580 ? RetroColorPalette.Success : RetroColorPalette.Error;
            this.DrawFilledRectangle(spriteBatch, segment, lineColor);
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
