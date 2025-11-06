#!/bin/bash
# CoinPay Build Script
# This script builds all Docker images for CoinPay services

set -e

echo "====================================="
echo "  CoinPay Build Script"
echo "====================================="
echo ""

# Navigate to project root (two levels up from Deployment/Start)
SCRIPT_DIR="$( cd "$( dirname "${BASH_SOURCE[0]}" )" && pwd )"
PROJECT_ROOT="$( cd "$SCRIPT_DIR/../.." && pwd )"
cd "$PROJECT_ROOT"

echo "Project Root: $PROJECT_ROOT"
echo ""

# Check if docker-compose.yml exists
if [ ! -f "docker-compose.yml" ]; then
    echo "ERROR: docker-compose.yml not found in $PROJECT_ROOT"
    exit 1
fi

echo "[1/3] Building all Docker images..."
echo "This may take several minutes on first build..."
echo ""

docker-compose build

if [ $? -ne 0 ]; then
    echo ""
    echo "ERROR: Failed to build Docker images"
    exit 1
fi

echo ""
echo "[1/3] Docker images built successfully"
echo ""

echo "[2/3] Listing built images..."
docker images | grep coinpay

echo ""
echo "[2/3] Images listed"
echo ""

echo "[3/3] Verifying image sizes..."
docker images --filter "reference=coinpay*" --format "{{.Repository}}:{{.Tag}} - {{.Size}}" | while read line; do
    echo "  $line"
done

echo ""
echo "[3/3] Verification complete"
echo ""

echo "====================================="
echo "  Build Complete!"
echo "====================================="
echo ""
echo "Images built:"
echo "  - coinpay-api        (Backend API)"
echo "  - coinpay-gateway    (API Gateway)"
echo "  - coinpay-web        (Frontend)"
echo "  - coinpay-docfx      (Documentation)"
echo ""
echo "Next steps:"
echo "  1. Start services:  ./Deployment/Start/start-coinpay.sh"
echo "  2. Stop services:   ./Deployment/Start/stop-coinpay.sh"
echo ""
echo "Note: PostgreSQL and Vault use official images (no build needed)"
echo ""
