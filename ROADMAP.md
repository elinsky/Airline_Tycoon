# Airline Tycoon - Product Roadmap

*Last Updated: 2025-01-20*

## Vision
Build the most compelling airline simulation game where every gate, route, and jet matters. Players progress from regional carriers to global aviation empires while competing against AI airlines in dynamic markets.

---

## ✅ Phase 1: Core Foundation (COMPLETE)

### MVP Features
- ✅ Basic airline management (cash, reputation, fleet)
- ✅ Route network system with 15 US airports
- ✅ Aircraft purchase and leasing (5 aircraft types)
- ✅ Daily operations simulation with demand/cost modeling
- ✅ Console-based UI with all management screens
- ✅ Save/load game system with JSON persistence
- ✅ Random events system (60+ events, 6 types, 4 severity levels)
- ✅ Financial reporting and performance tracking

---

## ✅ Phase 2: Competition & AI (COMPLETE)

### 2.1 AI Competitor Airlines
**Goal:** Transform from sandbox to competitive strategic game

**Features:**
- ✅ CompetitorAirline class with AI personality traits
  - Aggressive (rapid expansion, price wars)
  - Conservative (slow growth, high quality)
  - Budget (low prices, high volume)
  - Balanced (moderate approach)
- ✅ AI decision engine for route planning
  - Route profitability analysis with scoring system
  - Market entry/exit decisions based on personality
  - Fleet expansion logic (buy vs lease strategies)
  - Unprofitable route closure
- ✅ Dynamic pricing competition
  - AI adjusts prices based on load factors
  - Personality-driven pricing strategies
  - Premium vs budget positioning
- ✅ Market share calculation per route
  - Split passengers between competing airlines (40% price, 35% reputation, 25% service)
  - Demand adjusted based on market competition
  - Brand strength modifiers from personality traits
- ✅ Competitor dashboard UI
  - View all competitor stats (cash, reputation, fleet, routes)
  - See competitor routes with pricing and profitability
  - Market comparison table showing relative performance
  - Personality type display

**Impact:** Successfully transforms game from sandbox to competitive experience. Players must now monitor competitors, respond to price wars, and defend market share.

---

## 🎮 Phase 2.5: Pixel Art GUI & Retro Audio (NEXT)

### Goal: Transform console UI into classic RCT-style pixel art experience

**Why Now:** Moving GUI earlier makes development more engaging, enables better UX iteration, and lets us see the retro aesthetic come to life while building features.

### 2.5.1 MonoGame Foundation
**Goal:** Set up pixel-perfect 2D rendering framework

**Features:**
- [x] MonoGame project setup and configuration
  - Cross-platform support (Windows, Mac, Linux)
  - 1280x720 base resolution (scalable)
  - Pixel-perfect rendering pipeline
- [x] Retro UI framework
  - Button system with hover/click states
  - Panel/window system (draggable, closeable)
  - Tooltip system
  - Pixel font rendering (bitmap fonts) - *deferred to Phase 2.5.3*
- [x] Color palette system
  - Classic RCT color scheme (aviation theme: blues, grays)
  - UI element theming
  - Consistent visual style

### 2.5.2 Core Game Screens
**Goal:** Replace console UI with pixel art screens

**Features:**
- [x] Main dashboard (RCT-style top bar)
  - Current day, cash, reputation meters (placeholders)
  - Quick stats (passengers, profit, routes) (placeholders)
  - Notification ticker for events (placeholders)
- [x] Route management screen
  - List view with sortable columns
  - Route profitability visualization (red/green indicators)
  - Click to view route details
  - Open/close route buttons
- [x] Fleet management screen
  - Aircraft grid with cards
  - Condition bars (like RCT ride excitement ratings)
  - Buy/lease buttons
- [x] Competitor screen
  - Airline cards with stats
  - Comparative bar charts (passengers, revenue)
  - Competitor details panel
- [x] Financial report screen
  - Income statement panel
  - Expense breakdown (with legend)
  - Historical trend chart (placeholder graph)

**Note:** Text rendering deferred to Phase 2.5.3 (bitmap fonts). Screens use colored placeholders to show layout.

### 2.5.3 Pixel Art Assets
**Goal:** Create retro-style visual assets

**Features:**
- [x] **Bitmap font system (CRITICAL - COMPLETED!)**
  - MonoGame SpriteFont implementation
  - 10pt Courier New Bold for crisp pixel look
  - TextRenderer utility class with alignment options
  - Drop shadow support for readability
  - Currency/number formatting helpers
  - All buttons, panels, and screens now display text!
- [ ] UI sprite sheets **(Deferred to future phase)**
  - Buttons (normal, hover, pressed, disabled)
  - Windows and panels (9-slice borders)
  - Icons (planes, airports, money, reputation stars)
  - Cursors (pointer, hand, busy)
- [ ] Aircraft sprites **(Deferred to future phase)**
  - Top-down view pixel art for aircraft types:
    - **Regional:** CRJ-700, ERJ-175
    - **Narrow-body:** Boeing 737 (737-800, 737 MAX), Airbus A320 (A320neo)
    - **Wide-body:** Boeing 787 Dreamliner, Boeing 777 (777-300ER), Airbus A350, Airbus A330
    - **Jumbo:** Boeing 747 (passenger/cargo variants)
  - Multiple angles (8 directions) for flight animation
  - Size variants to visually distinguish aircraft categories
- [ ] Airport sprites **(Deferred to future phase)**
  - Simple terminal icons
  - Runway indicators
  - Hub badges/markers
- [ ] Map visualization **(Deferred to future phase)**
  - US map with airport locations
  - Route lines between cities
  - Animated planes moving along routes (optional MVP feature)

**Status:** Bitmap fonts completed! GUI is now fully functional with text rendering. Sprite assets deferred to allow focus on game logic integration.

### 2.5.4 Retro Audio System
**Goal:** Old-school game audio like SNES/NES era

**Features:**
- [ ] Audio engine setup
  - MonoGame audio framework (XACT or simple WAV playback)
  - Volume controls (music, SFX separately)
  - Audio mixing for overlapping sounds
- [ ] Sound effects (8-bit/chiptune style)
  - Button clicks and UI interactions (blip, bloop)
  - Route opened (success jingle)
  - Aircraft purchased (cash register cha-ching)
  - Daily advance (soft tick/whoosh)
  - Competitor actions (subtle alert)
  - Event notifications (warning beep for bad events, happy chime for good)
  - Bankruptcy warning (dramatic low tone)
- [ ] Background music (optional for MVP)
  - Looping chiptune track (upbeat airline/travel theme)
  - Fade in/out system
  - Music toggle option
- [ ] Audio settings screen
  - Volume sliders (pixel art)
  - Mute toggles
  - Sound test buttons

### 2.5.5 Game Integration
**Goal:** Wire up existing game logic to GUI to create a fully playable game loop

**Phase A: Foundation (COMPLETE)**
- [x] Game loop refactor
  - Separate game logic from rendering via GameController
  - Update() and Draw() pattern implemented
  - Input handling (mouse) fully functional
- [x] Screen state management
  - ScreenManager handles screen transitions
  - Navigation between screens working
  - All screens access game data via Controller
- [x] Basic data binding
  - Top bar displays real game data (Day, Cash, Reputation)
  - GameController bridges GUI and game logic
  - Advance Day button processes actual game turns
- [x] Aircraft purchase/lease functionality
  - AircraftPurchaseScreen with all 5 aircraft types
  - Buy and Lease buttons functional
  - Returns to Fleet Management after purchase
- [x] Fleet data display
  - FleetManagementScreen shows actual aircraft from player fleet
  - Displays: Registration, Type, Condition, Lease status
  - Shows "No aircraft" message when fleet is empty

**Phase B: Core Gameplay (COMPLETE)**

**Priority 1: Aircraft Assignment System (COMPLETE)**
- [x] Add `AssignAircraftToRoute()` method to GameController
- [x] Add `UnassignAircraftFromRoute()` method to GameController
- [x] Create aircraft assignment dialog/dropdown in Route Management screen
- [x] Show assigned aircraft in route list
- [x] Show assignment status in Fleet Management screen
- [x] Enable aircraft reassignment between routes

**Priority 2: Route Management System (COMPLETE)**
- [x] Display actual routes in RouteManagementScreen
  - Replace placeholder rows with real route data from `PlayerAirline.Routes`
  - Show: Origin, Destination, Distance, Ticket Price, Load Factor
  - Show profitability indicator (green/red)
  - Show assigned aircraft (or "Unassigned")
- [x] Implement "Open Route" dialog
  - Airport picker for origin (dropdown from Airport.All)
  - Airport picker for destination (dropdown from Airport.All)
  - Ticket price input field with suggested price
  - "Confirm" button calls GameController.OpenRoute()
- [x] Wire up "Close Route" functionality
  - Add close button to each route row
  - Confirmation dialog
  - Call GameController.CloseRoute()
- [x] Add route assignment from Route Management screen
  - "Assign Aircraft" button for each unassigned route
  - Dropdown showing available (unassigned) aircraft
  - Visual feedback when route becomes operational

**Priority 3: Dashboard Real Data (COMPLETE)**
- [x] Display actual game statistics
  - Today's passengers carried (sum across all routes)
  - Today's profit/loss (from DailyOperationsSummary)
  - Active routes count
  - Fleet utilization percentage
- [x] Display recent events in events ticker
  - Show last 5 events from PlayerAirline.ActiveEvents
  - Color-coded by event severity (Minor/Moderate/Major/Critical)

**Phase C: Enhanced Screens (COMPLETE)**
- [x] Competitor screen with real data
  - Display actual competitors from Game.Competitors
  - Show their fleet size, route count, cash
  - Show overlapping routes (where you compete)
- [x] Financial report with real data
  - Calculate revenue/cost breakdown from PlayerAirline
  - Show expense percentages (fuel, crew, airports, etc.)
  - Display profit trend (need to track historical data)
- [x] Aircraft maintenance functionality
  - "Perform Maintenance" button in Fleet Management
  - Show maintenance cost preview
  - Call Aircraft.PerformMaintenance() via GameController
- [x] Sell/return aircraft functionality
  - "Sell" button for owned aircraft (get 70% of purchase price)
  - "Return" button for leased aircraft (early termination penalty)
- [x] Route price adjustment (moved from Phase D)
  - Edit ticket price for existing routes with +/- buttons
  - $10 increment/decrement
  - Minimum price of $10

**Phase D: Polish (DEFERRED)**
- [ ] Save/load integration
  - Keep existing JSON save system
  - Add "Load Game" screen with save previews
  - Auto-save option
- [ ] Route frequency adjustment
  - Change daily flights per route
  - Affects revenue and costs

**Current Status:** Phase 2.5.5 Game Integration is COMPLETE:
- ✅ **Phase A (Foundation)**: Complete - Screen management, data binding, basic UI
- ✅ **Phase B (Core Gameplay)**: Complete - Aircraft assignment, route management, fully playable game loop
- ✅ **Phase C (Enhanced Screens)**: Complete - Competitor analysis, financial reports, aircraft lifecycle management, price adjustment
- ✅ **Priority 3 (Dashboard Real Data)**: Complete - Live statistics display, event tracking
- ⏸️ **Phase D (Polish)**: Deferred - Save/load and route frequency features postponed

**The game is now fully playable with complete GUI integration!** All major screens display real game data. Players can purchase/lease aircraft, open routes, assign planes, adjust pricing, perform maintenance, and sell/return aircraft. Dashboard shows live statistics and recent events. The entire Phase 2.5 (MonoGame GUI Implementation) is complete.

### Implementation Approach

**Technology Stack:**
- **MonoGame** - C# 2D game framework (same as Stardew Valley, Celeste, Terraria)
- **Pixel art tools** - Aseprite or Piskel for sprite creation
- **Audio tools** - Bfxr, ChipTone, or BeepBox for 8-bit sound effects
- **Font** - Pixel bitmap font (like RCT's classic font)

**Development Strategy:**
1. Build MonoGame shell with one screen (dashboard)
2. Create pixel art style guide and color palette
3. Implement UI framework (buttons, windows, etc.)
4. Port console screens one-by-one to GUI
5. Add pixel art assets progressively
6. Integrate retro audio last (so gameplay is functional first)
7. Keep console UI as fallback/debug mode

**Why This Matters:**
- Makes the game MUCH more engaging to play and develop
- Retro pixel art aesthetic attracts players nostalgic for RCT/Transport Tycoon
- GUI enables better visualization of routes, competition, and economy
- Audio creates atmosphere and feedback that console can't provide
- Sets foundation for future polish (animations, particles, effects)

---

## 📊 Phase 3: Sophisticated Economy Simulator

### 3.1 Macroeconomic System
**Goal:** Create realistic economic cycles that affect the aviation industry

**Features:**
- [ ] Economic cycle simulation
  - Boom, growth, recession, depression phases
  - GDP growth tracking
  - Unemployment rate effects on demand
- [ ] Dynamic fuel pricing
  - Oil market simulation with OPEC decisions
  - Seasonal variations
  - Geopolitical event impacts
- [ ] Currency exchange rates
  - Multi-currency system for international routes
  - Forex volatility
  - Hedging opportunities
- [ ] Interest rate system
  - Affects aircraft financing costs
  - Loan availability changes
  - Inflation impacts

### 3.2 Microeconomic Mechanics
**Goal:** Detailed supply/demand modeling at route level

**Features:**
- [ ] Advanced demand modeling
  - Price elasticity of demand
  - Substitute goods (other airlines, other transport)
  - Complementary goods (hotels, tourism)
  - Consumer surplus calculations
- [ ] Market segmentation
  - Business travelers (inelastic demand, high prices)
  - Leisure travelers (elastic demand, price sensitive)
  - Premium/economy class split
- [ ] Route-specific economics
  - Tourism seasonality by destination
  - Business travel patterns (weekday peaks)
  - Holiday demand spikes
- [ ] Cost structure breakdown
  - Fixed costs (airport slots, staff, overhead)
  - Variable costs (fuel, crew, maintenance per flight)
  - Economies of scale (fleet commonality savings)
  - Learning curve effects

### 3.3 Financial Instruments
**Goal:** Advanced financial management tools

**Features:**
- [ ] Debt and equity financing
  - Bank loans with interest rates
  - Bond issuance
  - Equity rounds (dilution mechanics)
- [ ] Hedging strategies
  - Fuel price hedging
  - Currency hedging for international routes
- [ ] Investment opportunities
  - Airport slot purchases
  - Codeshare agreements
  - Airline acquisitions

**Why This Matters:** Provides depth for experienced players, creates realistic business simulation, enables advanced strategies

---

## 🛠️ Phase 4: Operational Depth

### 4.1 Aircraft Maintenance System
**Goal:** Make fleet management consequential

**Features:**
- [ ] Maintenance scheduling
  - A-checks (every 500 hours)
  - C-checks (every 3000 hours)
  - D-checks (every 6 years)
- [ ] Aircraft condition decay
  - Flight hours reduce condition
  - Low condition = higher breakdown risk
- [ ] Breakdown mechanics
  - Flight cancellations (revenue loss, reputation hit)
  - Emergency repairs (expensive)
  - AOG (Aircraft On Ground) situations
- [ ] Preventive maintenance
  - Schedule downtime for maintenance
  - Balance cost vs reliability

### 4.2 Crew Management
**Goal:** Add human resource complexity

**Features:**
- [ ] Pilot and crew hiring
  - Training costs and time
  - Experience levels affect safety
- [ ] Crew scheduling
  - FAA rest requirements
  - Crew bases and positioning
- [ ] Labor relations
  - Union negotiations
  - Strike threats during contract talks
  - Morale affects performance

### 4.3 Airport Slots & Gates
**Goal:** Constrained resources create strategic choices

**Features:**
- [ ] Slot restrictions at major hubs
  - Limited takeoff/landing slots
  - Peak vs off-peak pricing
  - Slot trading market
- [ ] Gate management
  - Lease gates at airports
  - Turnaround time optimization
  - Hub saturation

**Why This Matters:** Adds operational realism, creates capacity constraints, rewards strategic planning

---

## 🌍 Phase 5: Global Expansion

### 5.1 International Routes
**Goal:** Expand from US-only to worldwide network

**Features:**
- [ ] 100+ global airports
  - Europe (LHR, CDG, FRA, AMS)
  - Asia (HKG, NRT, SIN, DXB)
  - South America (GRU, EZE)
  - Africa (JNB, CAI)
- [ ] Long-haul aircraft
  - Boeing 777 (777-200LR, 777-300ER), 787 Dreamliner (787-8, 787-9, 787-10)
  - Airbus A350 (A350-900, A350-1000), A330 (A330-200, A330-300, A330neo)
  - Ultra-long-range routes (SIN-JFK, PER-LHR)
- [ ] International regulations
  - Bilateral air service agreements
  - Foreign ownership restrictions
  - Cabotage rules
- [ ] Multi-stop routing
  - Technical stops for fuel
  - Hub-and-spoke optimization

### 5.2 Alliance System
**Goal:** Cooperative strategy with other airlines

**Features:**
- [ ] Airline alliances (like Star Alliance, SkyTeam, OneWorld)
  - Codeshare agreements
  - Shared lounges and benefits
  - Network effects
- [ ] Interline agreements
  - Baggage transfers
  - Revenue sharing
- [ ] Joint ventures
  - Coordinate pricing and schedules
  - Share revenues on routes

**Why This Matters:** Adds strategic depth, enables global reach without owning every route, realistic airline business model

---

## 🎮 Phase 6: Player Experience

### 6.1 Scenario Mode
**Goal:** Structured challenges with clear objectives

**Features:**
- [ ] 10+ scenarios with unique challenges
  - "East Coast Startup" - Build profitable network from JFK ($2M starting cash)
  - "Budget Carrier Challenge" - Achieve 90% load factors with <$100 ticket prices
  - "Recession Recovery" - Survive 2-year recession and return to profitability
  - "Hub Dominance" - Control 60% of slots at ATL
  - "Long-Haul Pioneer" - Launch profitable transpacific routes
  - "Turnaround Artist" - Save bankrupt airline in 3 years
- [ ] Scenario leaderboards
- [ ] Custom scenario creator

### 6.2 Advanced Analytics
**Goal:** Better decision-making tools

**Features:**
- [ ] Profitability dashboards
  - Route profitability trends over time
  - Aircraft utilization metrics
  - Break-even analysis
- [ ] Route recommendations
  - AI suggests profitable routes based on fleet
  - Market opportunity scoring
- [ ] "What-if" calculator
  - Model new route profitability
  - Aircraft purchase ROI analysis
  - Pricing strategy simulator
- [ ] Historical charts
  - Revenue/profit trends
  - Market share over time
  - Reputation tracking

### 6.3 UI/UX Improvements
**Goal:** Make the game more polished and accessible

**Features:**
- [ ] Enhanced console UI with colors
- [ ] ASCII art visualizations (route maps, charts)
- [ ] Keyboard shortcuts for power users
- [ ] Tutorial mode for new players
- [ ] Achievement system

**Why This Matters:** Makes game more accessible, provides clear goals, improves player satisfaction

---

## 🚀 Phase 7: Advanced Features

### 7.1 Cargo Operations
**Goal:** Add freight as alternative business model

**Features:**
- [ ] Cargo aircraft (Boeing 747F, MD-11F)
- [ ] Freight demand modeling
- [ ] Cargo-only routes
- [ ] Combined passenger/cargo operations
- [ ] Express delivery premium pricing

### 7.2 Airline Customization
**Goal:** Brand identity and differentiation

**Features:**
- [ ] Livery design system
- [ ] Service class configuration
  - First, Business, Premium Economy, Economy
  - Seat pitch and configuration
- [ ] Ancillary revenue
  - Baggage fees
  - Seat selection fees
  - In-flight purchases
- [ ] Frequent flyer program
  - Loyalty rewards
  - Customer retention

### 7.3 Airport Development
**Goal:** Vertical integration into airport ownership

**Features:**
- [ ] Build/buy airport infrastructure
  - Add terminals and gates
  - Expand runways
- [ ] Airport revenue streams
  - Landing fees from other airlines
  - Retail and parking revenue
- [ ] Slot control
  - Priority access to your own airports

**Why This Matters:** New strategic dimensions, enables unique playstyles, long-term goals for advanced players

---

## 🔮 Future Considerations

### Potential Features (Not Yet Scheduled)
- Multiplayer mode (competitive or cooperative)
- Real-time mode (vs turn-based)
- Climate/sustainability mechanics (carbon credits, electric aircraft)
- Regulatory changes (noise restrictions, slot auctions)
- Safety ratings and incident management
- Charter operations and seasonal routes
- Aircraft leasing business (become a lessor)
- Private jet operations
- Regional airline subsidiaries

---

## Development Principles

### Quality Standards
- All features must pass pre-commit hooks (format, build, test)
- Maintain 80%+ code coverage
- Comprehensive XML documentation
- Security scans must pass
- Use SOLID principles and DDD patterns

### Feature Prioritization
Features are prioritized based on:
1. **Player Impact** - Does it make the game more fun?
2. **Strategic Depth** - Does it create meaningful choices?
3. **Replayability** - Does it make each playthrough different?
4. **Realism** - Does it reflect real airline business?
5. **Implementation Cost** - Effort required vs value delivered

### Release Philosophy
- Release early, release often
- Commit after each major feature
- Maintain playable builds on main branch
- Iterate based on playtesting feedback

---

## How to Contribute Ideas

Have ideas for the roadmap? Consider:
- Does it create interesting strategic decisions?
- Is it realistic to airline operations?
- Does it add depth without adding complexity?
- Would it be fun to play?

This roadmap is a living document and will evolve based on development progress and feedback.

---

*Inspired by RollerCoaster Tycoon's addictive gameplay loop and attention to detail.*
