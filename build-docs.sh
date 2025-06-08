#!/bin/bash
# Build script for Airline Tycoon documentation

set -e

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
BLUE='\033[0;34m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

echo -e "${BLUE}✈️  Building Airline Tycoon Documentation...${NC}"
echo ""

# Check if Python is installed
if ! command -v python3 &> /dev/null; then
    echo -e "${RED}❌ Python 3 is not installed. Please install Python 3.x to continue.${NC}"
    exit 1
fi

# Check if pip is installed
if ! command -v pip &> /dev/null && ! command -v pip3 &> /dev/null; then
    echo -e "${RED}❌ pip is not installed. Please install pip to continue.${NC}"
    exit 1
fi

# Use pip3 if available, otherwise pip
PIP_CMD="pip"
if command -v pip3 &> /dev/null; then
    PIP_CMD="pip3"
fi

# Check if virtual environment exists
if [ ! -d "venv" ] && [ "$1" != "--no-venv" ]; then
    echo -e "${YELLOW}📦 Creating virtual environment...${NC}"
    python3 -m venv venv
    echo -e "${GREEN}✅ Virtual environment created${NC}"
    
    # Activate virtual environment
    source venv/bin/activate
    echo -e "${GREEN}✅ Virtual environment activated${NC}"
else
    if [ -d "venv" ] && [ "$1" != "--no-venv" ]; then
        source venv/bin/activate
        echo -e "${GREEN}✅ Virtual environment activated${NC}"
    fi
fi

# Check if mkdocs is installed
if ! command -v mkdocs &> /dev/null; then
    echo -e "${YELLOW}📦 Installing documentation dependencies...${NC}"
    $PIP_CMD install -r requirements-docs.txt
    echo -e "${GREEN}✅ Dependencies installed${NC}"
else
    # Check if dependencies are up to date
    echo -e "${BLUE}🔍 Checking dependencies...${NC}"
    $PIP_CMD install -r requirements-docs.txt --upgrade --quiet
fi

# Clean previous build
if [ -d "site" ]; then
    echo -e "${YELLOW}🧹 Cleaning previous build...${NC}"
    rm -rf site
fi

# Build the documentation
echo -e "${BLUE}🔨 Building documentation...${NC}"
mkdocs build --strict

if [ $? -eq 0 ]; then
    echo -e "${GREEN}✅ Documentation built successfully!${NC}"
    echo -e "${BLUE}📁 Output location: ./site/${NC}"
    echo ""
    
    # Count statistics
    HTML_COUNT=$(find site -name "*.html" | wc -l)
    SIZE=$(du -sh site | cut -f1)
    echo -e "${BLUE}📊 Build Statistics:${NC}"
    echo -e "   Pages generated: ${HTML_COUNT}"
    echo -e "   Total size: ${SIZE}"
else
    echo -e "${RED}❌ Build failed! Check the error messages above.${NC}"
    exit 1
fi

# Handle command line arguments
case "$1" in
    --serve)
        echo ""
        echo -e "${BLUE}🚀 Starting local documentation server...${NC}"
        echo -e "${YELLOW}   Documentation will be available at: http://127.0.0.1:8000${NC}"
        echo -e "${YELLOW}   Press Ctrl+C to stop the server${NC}"
        echo ""
        mkdocs serve
        ;;
    --open)
        echo ""
        echo -e "${BLUE}🌐 Opening documentation in browser...${NC}"
        if [[ "$OSTYPE" == "darwin"* ]]; then
            open site/index.html
        elif [[ "$OSTYPE" == "linux-gnu"* ]]; then
            xdg-open site/index.html
        elif [[ "$OSTYPE" == "msys" ]] || [[ "$OSTYPE" == "win32" ]]; then
            start site/index.html
        else
            echo -e "${YELLOW}⚠️  Cannot auto-open browser on this platform${NC}"
            echo -e "${BLUE}   Please open: file://$(pwd)/site/index.html${NC}"
        fi
        ;;
    --help|-h)
        echo ""
        echo "Usage: ./build-docs.sh [OPTIONS]"
        echo ""
        echo "Options:"
        echo "  --serve      Build and serve documentation locally"
        echo "  --open       Build and open in browser"
        echo "  --no-venv    Skip virtual environment usage"
        echo "  --help, -h   Show this help message"
        echo ""
        echo "Examples:"
        echo "  ./build-docs.sh              # Build documentation"
        echo "  ./build-docs.sh --serve      # Build and serve locally"
        echo "  ./build-docs.sh --open       # Build and open in browser"
        ;;
    "")
        # No additional action needed, just build
        ;;
    *)
        echo -e "${RED}Unknown option: $1${NC}"
        echo "Use --help for usage information"
        exit 1
        ;;
esac