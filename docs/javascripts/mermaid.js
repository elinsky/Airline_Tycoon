// Mermaid initialization for Airline Tycoon documentation
// This script ensures Mermaid diagrams render properly with MkDocs Material theme

// Wait for the document to be ready
document$.subscribe(({ body }) => {
  // Initialize Mermaid with custom theme settings
  mermaid.initialize({
    startOnLoad: true,
    theme: 'default',
    themeVariables: {
      // Aviation-themed colors matching our documentation style
      primaryColor: '#0099cc',
      primaryTextColor: '#fff',
      primaryBorderColor: '#0077a3',
      lineColor: '#333',
      secondaryColor: '#33b3db',
      tertiaryColor: '#f8f9fa',
      background: '#ffffff',
      mainBkg: '#0099cc',
      secondBkg: '#33b3db',
      tertiaryBkg: '#f8f9fa',
      primaryFontFamily: '-apple-system, BlinkMacSystemFont, "Segoe UI", Roboto, sans-serif',
      fontSize: '16px',
      // Class diagram specific
      classText: '#333',
      // Sequence diagram specific
      actorBkg: '#0099cc',
      actorBorder: '#0077a3',
      actorTextColor: '#fff',
      actorLineColor: '#333',
      signalColor: '#333',
      signalTextColor: '#333',
      // State diagram specific
      stateBkg: '#f8f9fa',
      stateBorder: '#0099cc',
      // Flowchart specific
      nodeTextColor: '#333',
      nodeBorder: '#0099cc',
      clusterBkg: '#f8f9fa',
      clusterBorder: '#0099cc',
      edgeLabelBackground: '#fff'
    },
    flowchart: {
      useMaxWidth: true,
      htmlLabels: true,
      curve: 'basis'
    },
    sequence: {
      useMaxWidth: true,
      diagramMarginX: 8,
      diagramMarginY: 8,
      boxTextMargin: 5
    },
    gantt: {
      numberSectionStyles: 4,
      axisFormat: '%Y-%m-%d'
    }
  });

  // Run Mermaid on any diagrams that might have been added dynamically
  mermaid.run({
    querySelector: '.mermaid'
  });
});

// Re-run Mermaid when navigating between pages (for instant navigation)
location$.subscribe(() => {
  // Small delay to ensure content is loaded
  setTimeout(() => {
    if (typeof mermaid !== 'undefined') {
      mermaid.run({
        querySelector: '.mermaid'
      });
    }
  }, 100);
});