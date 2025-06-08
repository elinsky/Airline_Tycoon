# Game Mechanics

This guide details the core mechanics that power Airline Tycoon's gameplay systems.

## Flight Operations

### Route Planning
Routes connect two airports and form the backbone of your airline network:

- **Point-to-Point**: Direct flights between cities
- **Hub-and-Spoke**: Centralized routing through major airports
- **Milk Runs**: Multi-stop routes for smaller markets

### Schedule Management
Each route requires careful scheduling:

- **Frequency**: How often flights operate (daily, weekly, multiple per day)
- **Timing**: Departure and arrival times affect demand
- **Aircraft Assignment**: Matching plane capacity to route demand

## Airport Operations

### Gate Management
- Limited gates at each airport
- Peak-hour congestion affects turnaround times
- Gate leases as fixed costs

### Slot Restrictions
Major airports have limited takeoff/landing slots:

- **Peak Slots**: High-demand morning/evening slots
- **Off-Peak Slots**: Lower demand but cheaper
- **Slot Trading**: Buy/sell slots with other airlines

## Fleet Management

### Aircraft Types

| Category | Capacity | Range | Example |
|----------|----------|--------|----------|
| Regional | 50-100 | Short | CRJ-900 |
| Narrow-body | 100-200 | Medium | Boeing 737 |
| Wide-body | 200-400 | Long | Boeing 777 |
| Jumbo | 400+ | Ultra-long | Airbus A380 |

### Maintenance System
- **Scheduled Maintenance**: Regular checks reduce breakdown risk
- **Line Maintenance**: Quick fixes between flights
- **Heavy Maintenance**: Major overhauls (aircraft out of service)

## Pricing Strategy

### Dynamic Pricing
Ticket prices adjust based on:

- Days until departure
- Current load factor
- Competitor pricing
- Seasonal demand

### Fare Classes
- **Economy**: Base pricing, majority of seats
- **Premium Economy**: 20-30% premium, extra legroom
- **Business**: 200-300% premium, flexibility
- **First Class**: 400%+ premium, luxury service

## Competition System

### AI Airlines
Competing airlines have distinct strategies:

- **Budget Carriers**: Low prices, high volume
- **Legacy Carriers**: Full service, global networks
- **Regional Airlines**: Niche markets, partnerships

### Market Share
Compete for passengers through:

- Price competitiveness
- Schedule convenience
- Service quality
- Brand loyalty

## Event System

Random events add challenge and opportunity:

### Weather Events (30% probability)
- Storms cancel flights
- Snow increases delays
- Clear skies boost on-time performance

### Economic Events (25% probability)
- Fuel price spikes
- Economic downturns
- Tourism booms

### Operational Events (25% probability)
- Aircraft breakdowns
- Staff strikes
- Air traffic control delays

### Regulatory Events (20% probability)
- New safety requirements
- Environmental regulations
- Route authority changes

## Next Steps

- Understand the [Economic Model](./economic-model.md)
- Learn about [Seasonal Systems](./seasonal-system.md)
- Explore different [Airline Types](./airline-types.md)