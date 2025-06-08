# Mermaid Test Page

This page tests that Mermaid diagrams are rendering correctly.

## Simple Flowchart

```mermaid
graph TD
    A[Start] --> B{Is it working?}
    B -->|Yes| C[Great!]
    B -->|No| D[Debug]
    D --> B
```

## Sequence Diagram

```mermaid
sequenceDiagram
    participant User
    participant MkDocs
    participant Mermaid
    
    User->>MkDocs: Build Documentation
    MkDocs->>Mermaid: Process Diagrams
    Mermaid-->>MkDocs: Rendered SVG
    MkDocs-->>User: HTML with Diagrams
```

## Class Diagram

```mermaid
classDiagram
    class Aircraft {
        +String model
        +int capacity
        +float range
        +fly() void
        +maintain() void
    }
    
    class Airline {
        +String name
        +List~Aircraft~ fleet
        +addAircraft(aircraft) void
        +scheduleFlights() void
    }
    
    Airline "1" --> "*" Aircraft : owns
```

If you can see these diagrams rendered properly, then Mermaid is working correctly!