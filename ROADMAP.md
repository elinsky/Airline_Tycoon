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
- [ ] Main dashboard (RCT-style top bar)
  - Current day, cash, reputation meters
  - Quick stats (passengers, profit, routes)
  - Notification ticker for events
- [ ] Route management screen
  - List view with sortable columns
  - Route profitability visualization (red/green indicators)
  - Click to view route details
  - Open/close route buttons
- [ ] Fleet management screen
  - Aircraft list with icons
  - Condition bars (like RCT ride excitement ratings)
  - Buy/lease dialogs with pixel art aircraft previews
- [ ] Competitor screen
  - Airline logos/icons
  - Comparative bar charts (passengers, revenue)
  - Competitor route overlay
- [ ] Financial report screen
  - Pixel art graphs (line charts for trends)
  - Expense breakdown pie chart
  - Historical data view

### 2.5.3 Pixel Art Assets
**Goal:** Create retro-style visual assets

**Features:**
- [ ] UI sprite sheets
  - Buttons (normal, hover, pressed, disabled)
  - Windows and panels (9-slice borders)
  - Icons (planes, airports, money, reputation stars)
  - Cursors (pointer, hand, busy)
- [ ] Aircraft sprites
  - Top-down view pixel art for 5 aircraft types
  - Multiple angles (8 directions) for flight animation
  - Size variants (regional, narrow-body, wide-body, jumbo)
- [ ] Airport sprites
  - Simple terminal icons
  - Runway indicators
  - Hub badges/markers
- [ ] Map visualization
  - US map with airport locations
  - Route lines between cities
  - Animated planes moving along routes (optional MVP feature)

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
**Goal:** Wire up existing game logic to GUI

**Features:**
- [ ] Game loop refactor
  - Separate game logic from rendering
  - Update() and Draw() pattern
  - Input handling (mouse, keyboard)
- [ ] Screen state management
  - Screen stack system (for overlays)
  - Transitions between screens
  - Pause/resume functionality
- [ ] Data binding
  - Connect UI elements to game state
  - Real-time updates when data changes
  - Efficient rendering (only redraw when needed)
- [ ] Save/load integration
  - Keep existing JSON save system
  - Add "Load Game" screen with save previews
  - Auto-save option

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
  - Boeing 777, 787-9
  - Airbus A350, A330
  - Ultra-long-range routes
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
