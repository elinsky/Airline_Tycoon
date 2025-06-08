# Economic Model

The economic simulation in Airline Tycoon creates a realistic and challenging business environment. This guide explains the financial systems and economic factors that affect your airline.

## Revenue Streams

### Passenger Revenue
The primary income source for most airlines:

```csharp
PassengerRevenue = TicketPrice × PassengerCount × LoadFactor
```

### Ancillary Revenue
Additional income beyond ticket sales:

- **Baggage Fees**: $25-50 per checked bag
- **Seat Selection**: $10-100 for preferred seats
- **In-flight Sales**: Food, beverages, duty-free
- **Change Fees**: $75-200 per ticket change

### Cargo Operations
Freight transport using passenger aircraft belly space:

- Higher margins than passengers
- Less seasonal variation
- Requires cargo handling infrastructure

## Cost Structure

### Variable Costs
Costs that scale with operations:

#### Fuel Costs (30-40% of total)
```csharp
FuelCost = Distance × FuelBurnRate × FuelPrice
```

Factors affecting fuel costs:
- Aircraft efficiency
- Fuel hedging strategies
- Global oil prices

#### Crew Costs (20-25% of total)
```csharp
CrewCost = (Pilots + FlightAttendants) × HourlyRate × FlightHours
```

### Fixed Costs
Costs independent of flight volume:

- **Aircraft Leases**: Monthly payments
- **Airport Gates**: Annual lease costs
- **Insurance**: Based on fleet value
- **Headquarters**: Administrative overhead

### Semi-Variable Costs

- **Maintenance**: Increases with aircraft age
- **Ground Handling**: Partially fixed, partially per-flight
- **Marketing**: Base budget plus route-specific campaigns

## Profitability Calculations

### Route Profitability
```csharp
public decimal CalculateRouteProfitability(Route route, Aircraft aircraft)
{
    var revenue = CalculateRevenue(route, aircraft.Capacity);
    var costs = CalculateCosts(route, aircraft);
    
    var profit = revenue - costs;
    var margin = profit / revenue;
    
    return margin;
}
```

### Break-Even Analysis
```csharp
BreakEvenLoadFactor = TotalCosts / (TicketPrice × Capacity)
```

## Financial Management

### Cash Flow Management
- **Working Capital**: 2-3 months operating expenses
- **Seasonal Variations**: Build reserves for low season
- **Investment Timing**: Aircraft purchases during downturns

### Financing Options

| Option | Cost | Risk | Flexibility |
|--------|------|------|-------------|
| Cash Purchase | Low | High | Low |
| Bank Loan | Medium | Medium | Medium |
| Operating Lease | High | Low | High |
| Finance Lease | Medium | Medium | Low |

### Financial Metrics

#### Operating Margin
```
Operating Margin = (Revenue - Operating Costs) / Revenue × 100
```
Target: 8-12% for healthy airlines

#### Return on Assets (ROA)
```
ROA = Net Income / Total Assets × 100
```
Target: 5-8% annually

## Market Dynamics

### Demand Elasticity
How price changes affect demand:

- **Business Travel**: Low elasticity (less price sensitive)
- **Leisure Travel**: High elasticity (very price sensitive)
- **Visiting Friends/Family**: Medium elasticity

### Seasonal Patterns

| Season | Demand | Pricing Power | Typical Routes |
|--------|---------|--------------|----------------|
| Peak Summer | 130% | High | Vacation destinations |
| Winter Holidays | 150% | Very High | All routes |
| Shoulder | 90% | Medium | Mixed |
| Off-Peak | 70% | Low | Business routes only |

### Economic Cycles

1. **Growth Phase**: Expand routes, add capacity
2. **Peak Phase**: Maximize pricing, optimize efficiency  
3. **Recession Phase**: Cut unprofitable routes, defer purchases
4. **Recovery Phase**: Selective growth, fleet renewal

## Strategic Considerations

### Network Effects
- Hub airports create connection opportunities
- Feeder routes support long-haul profitability
- Market dominance improves pricing power

### Cost Advantages
- Fleet commonality reduces training/maintenance
- Scale economies in purchasing
- Efficient crew scheduling

### Revenue Optimization
- Yield management systems
- Loyalty program value
- Corporate contracts

## Next Steps

- Learn about different [Airline Types](./airline-types.md)
- Understand the [Seasonal System](./seasonal-system.md)
- Review [Core Concepts](./core-concepts.md)